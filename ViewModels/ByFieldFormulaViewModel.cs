using Stylet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSV2.ViewModels
{
    public class ByFieldFormulaViewModel : Screen
    {
        //加载其他窗口
        private IWindowManager windowManager;

        public string StatusBar { get; set; }
        public string By1Formula { get; set; }
        public string By2Formula { get; set; }
        public string By3Formula { get; set; }
        public string By4Formula { get; set; }
        public string By5Formula { get; set; }

        public string By6Formula { get; set; }
        public string By7Formula { get; set; }
        public string By8Formula { get; set; }
        public string By9Formula { get; set; }
        public string By10Formula { get; set; }


        public string By11Formula { get; set; }
        public string By12Formula { get; set; }
        public string By13Formula { get; set; }
        public string By14Formula { get; set; }
        public string By15Formula { get; set; }


        public string By16Formula { get; set; }
        public string By17Formula { get; set; }
        public string By18Formula { get; set; }
        public string By19Formula { get; set; }
        public string By20Formula { get; set; }

        private AppSettingsSection config;

        public ByFieldFormulaViewModel(IWindowManager windowManager,ref AppSettingsSection cfg)
        {
            this.windowManager = windowManager;
            config = cfg;
            By1Formula = config.Settings["By1Formula"].Value;
            By2Formula = config.Settings["By2Formula"].Value;
            By3Formula = config.Settings["By3Formula"].Value;
            By4Formula = config.Settings["By4Formula"].Value;
            By5Formula = config.Settings["By5Formula"].Value;

            By6Formula = config.Settings["By6Formula"].Value;
            By7Formula = config.Settings["By7Formula"].Value;
            By8Formula = config.Settings["By8Formula"].Value;
            By9Formula = config.Settings["By9Formula"].Value;
            By10Formula = config.Settings["By10Formula"].Value;

            By11Formula = config.Settings["By11Formula"].Value;
            By12Formula = config.Settings["By12Formula"].Value;
            By13Formula = config.Settings["By13Formula"].Value;
            By14Formula = config.Settings["By14Formula"].Value;
            By15Formula = config.Settings["By15Formula"].Value;

            By16Formula = config.Settings["By16Formula"].Value;
            By17Formula = config.Settings["By17Formula"].Value;
            By18Formula = config.Settings["By18Formula"].Value;
            By19Formula = config.Settings["By19Formula"].Value;
            By20Formula = config.Settings["By20Formula"].Value;

        }

        public void SaveFormula()
        {
            if (WordsIScn(By1Formula))
            {
                StatusBar = "备用字段1公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By2Formula))
            {
                StatusBar = "备用字段2公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By3Formula))
            {
                StatusBar = "备用字段3公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By4Formula))
            {
                StatusBar = "备用字段4公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By5Formula))
            {
                StatusBar = "备用字段5公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By6Formula))
            {
                StatusBar = "备用字段6公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By7Formula))
            {
                StatusBar = "备用字段7公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By8Formula))
            {
                StatusBar = "备用字段8公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By9Formula))
            {
                StatusBar = "备用字段9公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By10Formula))
            {
                StatusBar = "备用字段10公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By11Formula))
            {
                StatusBar = "备用字段11公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By12Formula))
            {
                StatusBar = "备用字段12公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By13Formula))
            {
                StatusBar = "备用字段13公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By14Formula))
            {
                StatusBar = "备用字段14公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By15Formula))
            {
                StatusBar = "备用字段15公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By16Formula))
            {
                StatusBar = "备用字段16公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By17Formula))
            {
                StatusBar = "备用字段17公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By18Formula))
            {
                StatusBar = "备用字段18公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By19Formula))
            {
                StatusBar = "备用字段19公式中包含中文字符，请修改";
                return;
            }
            if (WordsIScn(By20Formula))
            {
                StatusBar = "备用字段20公式中包含中文字符，请修改";
                return;
            }

            config.Settings["By1Formula"].Value = By1Formula;
            config.Settings["By2Formula"].Value = By2Formula;
            config.Settings["By3Formula"].Value = By3Formula;
            config.Settings["By4Formula"].Value = By4Formula;
            config.Settings["By5Formula"].Value = By5Formula;
                  
            config.Settings["By6Formula"].Value = By6Formula;
            config.Settings["By7Formula"].Value = By7Formula;
            config.Settings["By8Formula"].Value = By8Formula;
            config.Settings["By9Formula"].Value = By9Formula;
            config.Settings["By10Formula"].Value = By10Formula;
                  
            config.Settings["By11Formula"].Value = By11Formula;
            config.Settings["By12Formula"].Value = By12Formula;
            config.Settings["By13Formula"].Value = By13Formula;
            config.Settings["By14Formula"].Value = By14Formula;
            config.Settings["By15Formula"].Value = By15Formula;

            config.Settings["By16Formula"].Value = By16Formula;
            config.Settings["By17Formula"].Value = By17Formula;
            config.Settings["By18Formula"].Value = By18Formula;
            config.Settings["By19Formula"].Value = By19Formula;
            config.Settings["By20Formula"].Value = By20Formula;

            StatusBar = "保存完成";
        }

        private bool WordsIScn(string words)
        {
            string TmmP;

            for (int i = 0; i < words.Length; i++)
            {
                TmmP = words.Substring(i, 1);

                byte[] sarr = System.Text.Encoding.GetEncoding("gb2312").GetBytes(TmmP);

                if (sarr.Length == 2)
                {
                    return true;
                }
            }
            return false;
        }

        public void ShowHelpChm()
        {
            //打开chm文档
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\无人值守汽车衡称重系统V2.2 说明书.chm");
        }

        protected override void OnClose()
        {
            SaveFormula();
            base.OnClose();
        }
    }
}
