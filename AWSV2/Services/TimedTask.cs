using AWSV2.ViewModels;
using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common.Utility;
using AWSV2.Models;
using Common.Utility.AJ;
using Common.Model;
using static Quartz.Logging.OperationName;
using System.Windows;
using Common.Utility.AJ.MobileConfiguration.WeightSystem;
using Common.Utility.AJ.MobileConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AWSV2.Services
{
    public class TimedTask
    {
        /// <summary>
        /// 任务调度的使用过程
        /// </summary>
        /// <returns></returns>
        public async static Task Run()
        {

            // 1.创建scheduler的引用
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler scheduler = await schedFact.GetScheduler();

            //2.启动 scheduler
            await scheduler.Start();

            // 创建自动清理任务 --阿吉 2023年10月18日09点26分
            var autoCleanupJob = JobBuilder.Create<AutoCleanupJob>().WithIdentity("autoCleanupJob").Build();
            var autoCleanupTrigger = TriggerBuilder.Create().StartAt(DateTime.Today.AddHours(12))
                .WithSchedule(CronScheduleBuilder.CronSchedule("0 0 0 1/1 * ? *")).Build();

            await scheduler.ScheduleJob(autoCleanupJob, autoCleanupTrigger);

            // 创建自动上传称重数据任务, MQTT平台查看缓存使用 --阿吉 2024年4月20日14点05分
            var autoUploadWeightingRecordJob
                = JobBuilder.Create<AutoUploadWeightingRecordsJob>().WithIdentity(nameof(AutoUploadWeightingRecordsJob)).Build();
            var autoUploadWeightingRecordJobTrigger = TriggerBuilder.Create().StartAt(DateTime.Today.AddHours(12))
                .WithSchedule(CronScheduleBuilder.CronSchedule("0 0/30 * 1/1 * ? *")).Build();

            await scheduler.ScheduleJob(autoUploadWeightingRecordJob, autoUploadWeightingRecordJobTrigger);

            await BackupHelper.InitBackupJobAsync(scheduler);

            //// 3.创建 job
            //IJobDetail job = JobBuilder.Create<SimpleJob>()
            //        .WithIdentity("job1", "group1")
            //        .Build();

            //// 4.创建 trigger
            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithIdentity("trigger1", "group1")
            //    .StartNow()
            //     .WithSchedule(CronScheduleBuilder.CronSchedule("0 59 23 * * ?"))
            //    //.WithSchedule(CronScheduleBuilder.CronSchedule("0 25 12 * * ?"))
            //    .Build();


            ////// 创建触发器
            ////ITrigger trigger = TriggerBuilder.Create()
            ////                          .WithIdentity("trigger1", "group1")
            ////                            .StartNow()                        //现在开始
            ////                            .WithSimpleSchedule(x => x         //触发时间，10秒一次。
            ////                                .WithIntervalInSeconds(10)
            ////                                .RepeatForever())              //不间断重复执行
            ////                            .Build();

            //// 5.使用trigger规划执行任务job
            //await sched.ScheduleJob(job, trigger);
        }
    }
    ///// <summary>
    ///// 任务
    ///// </summary>
    //public class SimpleJob : IJob
    //{
    //    private static readonly log4net.ILog log = LogHelper.GetLogger();
    //    public virtual Task Execute(IJobExecutionContext context)
    //    {
    //        return Task.Run(() =>
    //        {

    //            try
    //            {

    //                string directoryPath = $"{AppContext.BaseDirectory}Data\\Backup\\";
    //                //排除掉待.名称的文件夹，因为这些文件夹是 自动升级程序生成的。不能上传。
    //                var directory = Directory.GetDirectories(directoryPath).Where(p => !p.Contains(".")).OrderByDescending(o => o).FirstOrDefault();
    //                if (!string.IsNullOrEmpty(directory))
    //                {

    //                    var cleanResult = Common.SyncData.CleanBackup();

    //                    var files = Directory.GetFiles(directory);

    //                    foreach (var filePath in files)
    //                    {
    //                        var result = Common.SyncData.UpLoadBackup(filePath);
    //                    }
    //                }

    //            }
    //            catch (Exception e)
    //            {
    //                log.Info($"SimpleJob 定时上传 备份文件出崔");
    //            }
    //        });


    //    }

    //}

    /// <summary>
    /// 自动清理任务
    /// </summary>
    public class AutoCleanupJob : IJob
    {

        public virtual async Task Execute(IJobExecutionContext context)
        {
            var cfg = AJUtil.TryGetJSONObject<CleanupConfig>(SettingsHelper.AWSV2Settings.Settings[nameof(CleanupConfig)]?.Value ?? string.Empty) ?? new CleanupConfig();

            if (!cfg.EnableAuto)
            {
                return;
            }

            var ret = await cfg.ProcessAsync();

            AJLog4NetLogger.Instance().Debug($"自动清理任何处理完成: 删除记录:{ret.total} 删除文件:{ret.files}");
        }
    }

    public class AutoDatabaseBackupJob : IJob
    {
        public virtual async Task Execute(IJobExecutionContext context)
        {
            await BackupHelper.AutoBackupAsync();
        }
    }


    /// <summary>
    /// 自动上传称重记录任务
    /// </summary>
    public class AutoUploadWeightingRecordsJob : IJob
    {

        public virtual async Task Execute(IJobExecutionContext context)
        {
            var logContent = $"开始同步称重记录:{DateTime.Now}";
            try
            {
                using var db = AJDatabaseService.GetDbContext();
                var list = await db.WeighingRecords.AsNoTracking().ToListAsync();
                var total = list.Count();
                if (total == 0)
                {
                    logContent += ";本地无数据无需上传";
                    AJLog4NetLogger.Instance().Debug(logContent);
                    return;
                }

                // 构建磅单字段,也要缓存
                var pound = new PoundCfg();
                var mgr = new MobileConfigurationMgr(true)
                {
                    WeightFormTemplatePath = Globalspace._weightFormTemplatePath
                };
                var newFields = await pound.InitFieldsAsync(mgr, replaceUnderline: true, needOptions: false);

                using (var api = new CloudAPI())
                {
                    var result = await api.UploadWeightingRecordsAsync(list, newFields);
                    logContent += result.Success ? $";已成功上传 {total} 条记录" : $";上传失败:{result.Message}";
                }

                AJLog4NetLogger.Instance().Info(logContent);
            }
            catch (Exception e)
            {
                logContent += "上传发生异常";
                AJLog4NetLogger.Instance().Error(logContent, e);
            }
        }
    }
}
