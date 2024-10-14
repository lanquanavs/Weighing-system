using AWSV2.Models;
using Common.Model;
using Common.Utility;
using Dapper;
using MySql.Data.MySqlClient;
using Quartz.Xml.JobSchedulingData20;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using static NPOI.HSSF.Util.HSSFColor;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Common.Utility.AJ;
using Common.Utility.AJ.Extension;

namespace AWSV2.Services
{
    public class SQLDataAccess
    {
        //log
        private static readonly log4net.ILog log = LogHelper.GetLogger();
        private static readonly AppSettingsSection _mainSetting = SettingsHelper.AWSV2Settings;
        private static readonly string db = _mainSetting.Settings["DBSelect"]?.Value ?? "sqllite";

        public static bool IsMysql { get { return "mysql".Equals(db, StringComparison.OrdinalIgnoreCase); } }

        /// <summary>
        /// 获取mysql basaeDir， 数据备份调用命令要使用 --阿吉 2023年7月1日17点20分
        /// </summary>
        /// <returns></returns>
        public static string QueryMySqlBaseDir()
        {
            var connStr = LoadConnectionString("AWSMYSQL");
            using (var db = new MySqlConnection(connStr))
            {
                return db.QueryFirstOrDefault<string>("select @@basedir as basePath from dual;", new DynamicParameters());
            }
        }

        public static (string host, string port, string user, string database, string password) DecryptMySqlConnStr()
        {
            var connStr = LoadConnectionString("AWSMYSQL");
            var array = connStr.Split(';');

            var fields = new string[] { "data source", "port", "userid", "initial catalog", "password" };

            var dic = new Dictionary<string, string>();
            foreach (var item in array)
            {
                var parts = item.Split('=');
                var key = (parts.ElementAt(0) ?? string.Empty).ToLower();
                if (fields.Contains(key))
                {
                    dic.Add(key, parts.ElementAt(1));
                }
            }

            return (dic[fields[0]], dic[fields[1]], dic[fields[2]], dic[fields[3]], dic[fields[4]]);
        }

        #region UserModel
        public static UserModel LoadUser(string loginId)
        {
            IDbConnection cnn;
            string sql = string.Format("select * from User where Valid = 1 and LoginId = '{0}'", loginId);

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<UserModel>(sql, new DynamicParameters());

            cnn.Close();

            return output;
        }
        public static List<UserModel> LoadActiveUser()
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.Query<UserModel>("select UserId, UserName, LoginId, LoginPwd, Valid from user where Valid = 1;", new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }

        public static void UpdateUser(UserModel user)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute("UPDATE User SET UserName = @UserName, LoginId = @LoginId, LoginPwd = @LoginPwd WHERE UserId = @UserId;", user);

            cnn.Close();
        }

        #endregion

        #region RoleModel

        public static string LoadLoginRolePermission(string userId)
        {
            IDbConnection cnn;
            string sql = string.Format("select Role.RolePermission from Role left join User on Role.RoleName = User.UserRole where user.LoginId = '{0}';", userId);

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<string>(sql, new DynamicParameters());

            cnn.Close();

            return output;
        }

        #endregion

        #region GoodsModel

        public static List<GoodsModel> LoadActiveGoods(int flag = 1)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            string queryStr = "select * from Goods ";
            if (flag == 1)
            {
                queryStr += "where Valid = 1";
            }
            else if (flag == 0)
            {
                queryStr += "where Valid = 0";
            }
            queryStr += "; ";

            var output = cnn.Query<GoodsModel>(queryStr, new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }

        public static GoodsModel LoadGoods(string goodsName)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<GoodsModel>("select * from Goods where Valid = 1 and Name = '" + goodsName + "';", new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static GoodsModel LoadGoods(int? goodsID)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<GoodsModel>("select * from Goods where Valid = 1 and Id = " + goodsID.ToString(), new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static GoodsModel LoadDisabledGoods(string goodsName)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<GoodsModel>("select * from Goods where Valid = 0 and Name = '" + goodsName + "';", new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static void SaveGoods(GoodsModel goods)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute("INSERT INTO Goods (Num,Name) VALUES (@Num, @Name);", goods);

            cnn.Close();

            log.Info("新增物资:" + goods.Name);
        }

        #endregion

        #region CustomerModel

        public static List<CustomerModel> LoadActiveCustomer(int flag = 1)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            string queryStr = "select * from Customer ";
            if (flag == 1)
            {
                queryStr += "where Valid = 1";
            }
            else if (flag == 0)
            {
                queryStr += "where Valid = 0";
            }
            queryStr += "; ";

            var output = cnn.Query<CustomerModel>(queryStr, new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }

        public static CustomerModel LoadCustomer(string customerName)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<CustomerModel>("select * from Customer where Valid = 1 and Name = '" + customerName + "';", new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static CustomerModel LoadCustomer(int? customerId)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<CustomerModel>("select * from Customer where Valid = 1 and Id = '" + customerId + "';", new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static CustomerModel LoadDisabledCustomer(string customerName)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<CustomerModel>("select * from Customer where Valid = 0 and Name = '" + customerName + "';", new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static void SaveCustomer(CustomerModel customer)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute("INSERT INTO Customer (Num, Name, Manager, Phone, Comment) VALUES (@Num, @Name, @Manager, @Phone, @Comment);", customer);

            cnn.Close();

            log.Info("新增客户:" + customer.Name);
        }

        #endregion

        #region ICCardModel

        public static ICCardModel LoadICCard(string cardId)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<ICCardModel>("select * from ICCard where Id = '" + cardId + "';", new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static ICCardModel LoadICCard(string plateNo, int i)
        {
            IDbConnection cnn;
            string sql = string.Format("select * from ICCard where CarAutoNo = (select AutoNo from Car where PlateNo = '{0}')", plateNo);

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<ICCardModel>(sql, new DynamicParameters());

            cnn.Close();

            return output;
        }

        //public static void UpdateICCard(ICCardModel ICCard)
        //{
        //    IDbConnection cnn;

        //    if (db == "mysql")
        //        cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
        //    else
        //        cnn = new SQLiteConnection(LoadConnectionString());

        //    // cnn.Execute("UPDATE ICCard SET CustomerId = @CustomerId, GoodsId = @GoodsId, CarAutoNo = @CarAutoNo, By1 = @By1, By2 = @By2, By3 = @By3, By4 = @By4, By5 = @By5,By6 = @By6,By7 = @By7,By8 = @By8,By9 = @By9,By10 = @By10,By11 = @By11,By12 = @By2,By13 = @By13,By14 = @By14,By15 = @By15,By16 = @By16,By17 = @By17,By18 = @By18,By19 = @By19,By20 = @By20 WHERE Id = @Id;", ICCard);
        //    cnn.Execute("UPDATE ICCard SET CustomerId = @CustomerId, GoodsId = @GoodsId, CarAutoNo = @CarAutoNo, By1 = @By1, By2 = @By2, By3 = @By3, By4 = @By4, By5 = @By5 WHERE Id = @Id;", ICCard);

        //    cnn.Stop();
        //}
        public static void UpdateICCard(ICCardModel ICCard)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute("UPDATE ICCard SET CustomerId = @CustomerId, GoodsId = @GoodsId,SpecId=@SpecId, CarAutoNo = @CarAutoNo, By1 = @By1, By2 = @By2, By3 = @By3, By4 = @By4, By5 = @By5, Fhdw = @Fhdw  WHERE Id = @Id;", ICCard);

            cnn.Close();
        }

        #endregion

        #region CarModel

        public static List<CarModel> LoadActiveCar(int flag = 1)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            string queryStr = "select * from Car ";
            if (flag == 1)
            {
                queryStr += "where Valid = 1";
            }
            else if (flag == 0)
            {
                queryStr += "where Valid = 0";
            }
            queryStr += "; ";
            var output = cnn.Query<CarModel>(queryStr, new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }

        public static CarModel LoadCar(string PlateNo)
        {
            IDbConnection cnn;
            string sql = string.Format("select * from Car where Valid = 1 and PlateNo = '{0}';", PlateNo);

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<CarModel>(sql, new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static CarModel LoadCar(int? CarAutoNo)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<CarModel>("select * from Car where Valid = 1 and AutoNo = " + CarAutoNo.ToString(), new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static CarModel LoadDisabledCar(string PlateNo)
        {
            IDbConnection cnn;
            string sql = string.Format("select * from Car where Valid = 0 and PlateNo = '{0}';", PlateNo);

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<CarModel>(sql, new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static void SaveCar(CarModel car)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute("INSERT INTO Car (PlateNo, VehicleWeight, CarOwner) VALUES (@PlateNo, @VehicleWeight, @CarOwner);", car);

            cnn.Close();

            log.Info("新增车辆:" + car.PlateNo);
        }

        #endregion

        #region WeighingRecord
        public static List<WeighingRecord> GetFhdws()
        {
            IDbConnection cnn;
            string sql = $"SELECT Fhdw FROM WEIGHINGRECORD GROUP BY FHDW;";

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.Query<WeighingRecord>(sql, new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }

        //查未完成的称重记录
        public static WeighingRecord LoadWeighingRecord(string PlateNo, bool IsFinish)
        {
            try
            {
                IDbConnection cnn;
                string sql = $"select {string.Join(",", typeof(WeighingRecord).GetColumnsSql())} from WeighingRecord where (by20 is null or by20 <> 1) and Valid = 1 and Ch = '{PlateNo}' and IsFinish = {IsFinish}";

                if (db == "mysql")
                    cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
                else
                    cnn = new SQLiteConnection(LoadConnectionString());

                var output = cnn.QueryFirstOrDefault<WeighingRecord>(sql, new DynamicParameters());

                cnn.Close();

                return output;
            }
            catch
            {
                return null;
            }
        }

        public static WeighingRecord LoadWeighingRecord(string Bh)
        {
            IDbConnection cnn;
            string sql = string.Format("select Bh from WeighingRecord where Bh like '{0}%' order by Bh desc limit 1;", Bh);

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<WeighingRecord>(sql, new DynamicParameters());

            cnn.Close();

            return output;
        }

        /// <summary>
        /// 获取该车辆最近的场内称重记录
        /// </summary>
        /// <param name="plateNo"></param>
        /// <returns></returns>
        public static WeighingRecord GetWeighingRecord(string plateNo)
        {
            IDbConnection cnn;
            string sql = $"select {string.Join(",", typeof(WeighingRecord).GetColumnsSql())} from WeighingRecord where Ch = '{plateNo}' order by Bh desc limit 1;";

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<WeighingRecord>(sql, new DynamicParameters());

            cnn.Close();

            return output;
        }

        /// <summary>
        /// 获取该车辆最近的场内称重记录
        /// </summary>
        /// <param name="plateNo"></param>
        /// <returns></returns>
        public static WeighingRecord GetWeighingRecordByBh(string bh)
        {
            IDbConnection cnn;
            string sql = $"select {string.Join(",", typeof(WeighingRecord).GetColumnsSql())} from WeighingRecord where Bh = '{bh}' order by Bh desc limit 1;";

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<WeighingRecord>(sql, new DynamicParameters());

            cnn.Close();

            return output;
        }

        public static void SaveWeighingRecord(WeighingRecord WeighingRecord)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            try
            {
                var autoNoField = nameof(WeighingRecord.AutoNo);
                var columns = typeof(WeighingRecord).GetColumnsSql(false).Where(p => p != autoNoField).ToArray() ;
                cnn.Execute($"INSERT INTO WeighingRecord ({string.Join(",", columns)}) values({string.Join(",", columns.Select(p=>$"@{p}"))});", WeighingRecord);


            }
            catch (Exception e)
            {
                log.Error($"{nameof(SaveWeighingRecord)} 异常", e);
                if (e.Message.ToLower().Contains("duplicate entry"))
                {
                    if (WeighingRecord.Bh.StartsWith("X"))
                    {
                        WeighingRecord.Bh = $"X{WeighingRecord.Bh}";
                    }
                    else
                    {
                        var prefix = _mainSetting.Settings["Prefix"]?.Value ?? string.Empty;
                        var type = _mainSetting.Settings["GenerationType"]?.Value ?? string.Empty;
                        WeighingRecord.Bh = Common.Data.SQLDataAccess.CreateBh(DateTime.Now, prefix, type);
                    }

                    SaveWeighingRecord(WeighingRecord);
                }
            }
            finally
            {
                cnn.Close();
            }

        }


        public static List<WeighingRecord> GetLimits(int? finish = null, int? entryTimeDays = null,
            decimal maxAbnormalData = 100000m,
            int limit = 5)
        {
            IDbConnection cnn;
            var convertMZStr = IsMysql ? $"ifnull(CAST(mz as decimal(18,2)),0)" : $"ifnull(CAST(mz as real),0)";
            var where = $" WHERE Valid = 1 and  (by20 is null or by20 <> 1) and {convertMZStr} <= {maxAbnormalData}";

            if (finish.HasValue)
            {
                where += $" and IsFinish = '{finish}'";
            }

            if (entryTimeDays.HasValue)
            {
                var entryTimeField = nameof(WeighingRecord.EntryTime);

                var now =  DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);

                if (entryTimeDays == 0)
                {
                    where += $" and date({entryTimeField}) = '{now.ToString("yyyy-MM-dd")}'";
                }
                else
                {
                    var days = entryTimeDays.GetValueOrDefault();
                    var start = now.Date.AddDays(-days);
                    where += $" and {entryTimeField} >= '{start.ToString("yyyy-MM-dd HH:mm:ss")}' and {entryTimeField} <= '{now.ToString("yyyy-MM-dd HH:mm:ss")}'";
                }
                
            }

            string sql = (@"SELECT " + string.Join(",", typeof(WeighingRecord).GetColumnsSql()) + " FROM WEIGHINGRECORD " + where + " ORDER BY entrytime DESC LIMIT " + limit + " ; ");

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.Query<WeighingRecord>(sql, new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }

        public static void UpdateWeighingRecord(WeighingRecord WeighingRecord)
        {
            IDbConnection cnn = null;

            try
            {
                if (db == "mysql")
                    cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
                else
                    cnn = new SQLiteConnection(LoadConnectionString());

                cnn.Execute($"UPDATE WeighingRecord SET {string.Join(",", typeof(WeighingRecord).GetColumnsSql(false).Select(p => $"{p}=@{p}"))}  WHERE {nameof(WeighingRecord.Bh)} = @{nameof(WeighingRecord.Bh)};", WeighingRecord);

                cnn.Close();
            }
            catch (Exception e)
            {
                log.Error($"{nameof(UpdateWeighingRecord)} 异常",e);
            }
            finally
            {
                cnn?.Close();
            }
            
        }

        public static int LoadWeighingRecordCount()
        {
            IDbConnection cnn;
            string sql = string.Format("select AutoNo from WeighingRecord;");

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.Query<WeighingRecord>(sql, new DynamicParameters());

            cnn.Close();

            return output.Count();
        }
        public static List<WeighingRecord> LoadWeighingRecordCount(string plateno, string date)
        {
            IDbConnection cnn;
            string sql = $"select {string.Join(",", typeof(WeighingRecord).GetColumnsSql())} from weighingrecord where Ch = '{plateno}' and IsFinish = '1' and Jzrq like '{date}%';";

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.Query<WeighingRecord>(sql, new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }


        public static void DeleteWeighingRecord(WeighingRecord weighingRecord)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute("UPDATE WeighingRecord SET Valid = 0 WHERE Bh = @Bh;", weighingRecord);

            cnn.Close();

            log.Info("删除称重记录:" + weighingRecord.Bh);
        }

        #endregion

        #region WeighingImgModel
        public static void SaveWeighingImg(WeighingImgModel WeighingImg)
        {
            IDbConnection cnn;
            string sql = "INSERT INTO WeighingImg (WRBh, Pic1, Pic2, Pic3, Pic4, Pic5, Pic6, Video1, Video2) VALUES (@WRBh, @Pic1, @Pic2, @Pic3, @Pic4, @Pic5, @Pic6, @Video1, @Video2);";

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute(sql, WeighingImg);

            cnn.Close();
        }
        #endregion

        #region SyncDataModel
        public static void SaveSyncData(SyncDataModel SyncData)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute("INSERT INTO SyncData (WeighingRecordBh, SyncMode) VALUES (@WeighingRecordBh, @SyncMode);", SyncData);

            cnn.Close();
        }
        #endregion

        #region Overload
        public static void SavOverloadLog(OverloadLog SyncData, string flag)
        {
            if (flag != "1") return;//不储存超载日志。1、存储，0、不储存

            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            cnn.Execute("INSERT INTO OverloadLog (PlateNo, Constraints,AverageWeight,OverloadWeight,StandardWeight,AxleCount,Times,CreateDate) VALUES (@PlateNo, @Constraints,@AverageWeight,@OverloadWeight,@StandardWeight,@AxleCount,@Times,@CreateDate);", SyncData);

            cnn.Close();
        }
        #endregion

        #region 备用字段
        public static List<string> LoadByxValue(string byName)
        {
            IDbConnection cnn;
            string sql = string.Format("select distinct {0} from WeighingRecord where valid = 1 order by {0} desc limit 1000;", byName);

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.Query<string>(sql, new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }

        #endregion

        #region 通用查询
        public static List<string> LoadFildValue(string fildName, string orderFild = "bh", int limit = 10)
        {
            IDbConnection cnn;
            string sql = string.Format("select distinct {0} from WeighingRecord where valid = 1 AND {0} <> '' AND {0} IS NOT NULL order by {1} desc limit {2};", fildName, orderFild, limit);

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.Query<string>(sql, new DynamicParameters());

            cnn.Close();

            return output.ToList();
        }

        #endregion

        #region SysLog
        public static List<SysLogModel> LoadSysLog(string keywords)
        {
            string sql = string.Format("select * from Log where Message like '%{0}%' or Date like '%{0}%' limit 1000;", keywords);
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString("LogDB")))
            {
                var output = cnn.Query<SysLogModel>(sql, new DynamicParameters());
                return output.ToList();
            }
        }

        public static void DeleteSysLog(SysLogModel log)
        {
            string sql = string.Format("DELETE FROM Log WHERE LogId = {0};", log.LogId);

            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString("LogDB")))
            {
                cnn.Execute(sql, log);
            }
        }
        #endregion

        #region Data Clean
        public static bool DataClean()
        {
            IDbConnection cnn;
            StringBuilder sb = new StringBuilder();
            sb.Append($"DELETE FROM Car; ");
            sb.Append($"DELETE FROM CarFee; ");
            sb.Append($"DELETE FROM CarLabel; ");
            sb.Append($"DELETE FROM GoodsVsCustomerPrice; ");
            sb.Append($"DELETE FROM Customer; ");
            sb.Append($"DELETE FROM Goods; ");
            sb.Append($"DELETE FROM GoodsSpec; ");
            sb.Append($"DELETE FROM ICCard; ");
            sb.Append($"DELETE FROM SyncData; ");
            sb.Append($"DELETE FROM WeighingImg; ");
            sb.Append($"DELETE FROM WeighingRecord; ");

            //sb.Append($"DELETE FROM User WHERE LoginId<>'admin'; ");

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            int recordCount = cnn.Execute(sb.ToString());

            cnn.Close();

            log.Info("删除所有表数据（数据清理）:" + DateTime.Now.ToString("yyyy-MM-dd"));

            return Convert.ToBoolean(recordCount);

        }

        #endregion


        private static string LoadConnectionString(string id = "AWSDB") => ConfigurationManager.ConnectionStrings[id].ConnectionString;

        public static bool CheckConnectionStatus(AppSettingsSection mainSettings)
        {
            bool status = false;

            var id = mainSettings.Settings["DBSelect"].TryGetString("sqlite").Equals("sqlite",StringComparison.OrdinalIgnoreCase) ? "AWSDB" : "AWSMYSQL";

            if (id == "AWSDB")
            {
                IDbConnection cnn = new SQLiteConnection(LoadConnectionString(id));
                try
                {
                    cnn.Open();
                    if (cnn.State == ConnectionState.Open)
                    {
                        status = true;
                        cnn.Close();
                    }
                }
                catch(Exception e) {
                    log.Debug($"CheckConnectionStatus : {e.Message}" ,e);
                }
            }
            if (id == "AWSMYSQL")
            {
                IDbConnection cnn = new MySqlConnection(LoadConnectionString(id));

                try
                {
                    cnn.Open();
                    if (cnn.State == ConnectionState.Open)
                    {
                        status = true;
                        cnn.Close();
                    }
                }
                catch (System.Exception e)
                {
                    log.Debug($"CheckConnectionStatus : {e.Message}", e);
                }
            }

            return status;
        }

    }
}
