using FluentValidation;
using Stylet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AWSV2.Models;
using AWSV2.Services;

namespace AWSV2.ViewModels
{
    public class WeighingRecordViewModel : Screen
    {
        public string Title { get; set; }
        public string Input_PlateNo { get; set; }
        public string Input_Wz { get; set; }
        public WeighingRecordModel Wrm { get; set; }
        public List<CustomerModel> CustomerList { get; set; }
        public CustomerModel SelectedCustomer { get; set; }
        public CustomerModel SelectedCustomer2 { get; set; }
        public CustomerModel SelectedCustomer3 { get; set; }
        public List<CarModel> CarList { get; set; }
        public CarModel SelectedCar { get; set; }
        public List<GoodsModel> GoodsList { get; set; }
        public List<Common.Models.GoodsSpecModel> SpecList { get; set; }
        GoodsModel _SelectedGoods;
        public GoodsModel SelectedGoods
        {
            get
            {
                return _SelectedGoods;
            }
            set
            {
                _SelectedGoods = value;

                if (value != null)
                    SpecList = Common.Data.SQLDataAccess.GetGoodsSpecs(value.Name).ToList();
            }
        }

        public bool KzEnable
        {
            get { return !_mainSetting.Settings["Discount"].Value.Equals("1"); }
        }
        public bool KlEnable
        {
            get { return !_mainSetting.Settings["Discount"].Value.Equals("2"); }
        }

        public Common.Models.GoodsSpecModel SelectedSpec { get; set; }
        public List<UserModel> UserList { get; set; }
        private string mzrq = string.Empty;
        public string Mzrq
        {
            get { return mzrq; }
            set
            {
                mzrq = value;
                //Bh = Common.Data.SQLDataAccess.CreateBh(DateTime.Parse(value));
                Bh = CreateBh(DateTime.Parse(value));
                // Bh = Convert.ToDateTime(value).ToString("yyyyMMddHHmm");
                //Bh = Convert.ToDateTime(value).ToString("yyyyMMdd");
                //WeighingRecordModel queryBh = SQLDataAccess.LoadWeighingRecord(Bh);
                //if (queryBh != null)
                //{
                //    int iBh = Convert.ToInt32(queryBh.Bh.Substring(queryBh.Bh.Length - 4, 4)) + 1;
                //    Bh += iBh.ToString().PadLeft(4, '0');
                //}
                //else
                //{
                //    Bh += "0001";
                //}
            }
        }
        public string Pzrq { get; set; }
        public UserModel Mzsby { get; set; }
        public UserModel Pzsby { get; set; }
        public string Bh { get; set; }
        string _mz;
        public string Mz
        {
            get { return _mz; }
            set
            {
                _mz = value;
                //SetValues();

                CalcJz();
            }
        }
        string _pz;
        public string Pz
        {
            get { return _pz; }
            set
            {
                _pz = value;
                //SetValues();

                CalcJz();
            }
        }
        string _jz;
        public string Jz
        {
            get { return _jz; }
            set
            {
                _jz = value;
                SetValues();
            }
        }

        public string Kz { get; set; }
        public string Kl { get; set; }
        public string Sz { get; set; }
        public string JE { get; set; }

        public List<string> GblxItems { get; set; } = new List<string>() { "销售", "采购", "其他" };
        public List<string> ZsItems { get; set; } = new List<string>() { "2", "3", "4", "5", "6" };
        public List<string> WeighingTimes { get; set; } = new List<string>() { "1", "2" };
        public List<string> IsFinish { get; set; } = new List<string>() { "未完成", "完成" };

        public string WeighingTime { get; set; }
        public string Finish { get; set; }

        public bool ShowLabelBhInfo { get; set; }
        public bool ShowKh { get; set; }
        public bool ShowCh { get; set; }
        public bool ShowWz { get; set; }
        public bool ShowGuiGe { get; set; }
        public bool ShowMz { get; set; }
        public bool ShowMzrq { get; set; }
        public bool ShowMzsby { get; set; }
        public bool ShowPz { get; set; }
        public bool ShowPzrq { get; set; }
        public bool ShowPzsby { get; set; }
        public bool ShowJz { get; set; }
        public bool ShowKz { get; set; }
        public bool ShowKl { get; set; }
        public bool ShowSz { get; set; }
        public bool ShowBz { get; set; }
        public bool ShowJe { get; set; }

        public bool ShowBy1 { get; set; }
        public bool ShowBy2 { get; set; }
        public bool ShowBy3 { get; set; }
        public bool ShowBy4 { get; set; }
        public bool ShowBy5 { get; set; }

        public bool ShowBy6 { get; set; }
        public bool ShowBy7 { get; set; }
        public bool ShowBy8 { get; set; }
        public bool ShowBy9 { get; set; }
        public bool ShowBy10 { get; set; }

        public bool ShowBy11 { get; set; }
        public bool ShowBy12 { get; set; }
        public bool ShowBy13 { get; set; }
        public bool ShowBy14 { get; set; }
        public bool ShowBy15 { get; set; }

        public bool ShowBy16 { get; set; }
        public bool ShowBy17 { get; set; }
        public bool ShowBy18 { get; set; }
        public bool ShowBy19 { get; set; }
        public bool ShowBy20 { get; set; }
        public bool ShowGblx { get; set; }
        public bool ShowZs { get; set; }

        public string By1Name { get; set; }
        public string By2Name { get; set; }
        public string By3Name { get; set; }
        public string By4Name { get; set; }
        public string By5Name { get; set; }

        public string By6Name { get; set; }
        public string By7Name { get; set; }
        public string By8Name { get; set; }
        public string By9Name { get; set; }
        public string By10Name { get; set; }

        public string By11Name { get; set; }
        public string By12Name { get; set; }
        public string By13Name { get; set; }
        public string By14Name { get; set; }
        public string By15Name { get; set; }

        public string By16Name { get; set; }
        public string By17Name { get; set; }
        public string By18Name { get; set; }
        public string By19Name { get; set; }
        public string By20Name { get; set; }

        public string By1Value { get; set; }
        public string By2Value { get; set; }
        public string By3Value { get; set; }
        public string By4Value { get; set; }
        public string By5Value { get; set; }


        public string By6Value { get; set; }
        public string By7Value { get; set; }
        public string By8Value { get; set; }
        public string By9Value { get; set; }
        public string By10Value { get; set; }

        public string By11Value { get; set; }
        public string By12Value { get; set; }
        public string By13Value { get; set; }
        public string By14Value { get; set; }
        public string By15Value { get; set; }

        public string By16Value { get; set; }
        public string By17Value { get; set; }
        public string By18Value { get; set; }
        public string By19Value { get; set; }
        public string By20Value { get; set; }
        public string Gblx { get; set; }
        public string Zs { get; set; }

        public bool EnableUpdatePz { get; set; } = false;
        public bool EnableUpdateMz { get; set; } = false;
        public bool EnableUpdateJz { get; set; } = false;
        public bool EnableUpdateSz { get; set; } = false;

        public bool ShowKh2 { get; set; }
        public bool ShowKh3 { get; set; }


        public string KhName { get; set; }
        public string Kh2Name { get; set; }
        public string Kh3Name { get; set; }
        public string WzName { get; set; }
        public string GuiGeName { get; set; }
        public string ChName { get; set; }
        public string MzName { get; set; }
        public string MzrqName { get; set; }
        public string MzsbyName { get; set; }
        public string PzName { get; set; }
        public string PzsbyName { get; set; }
        public string PzrqName { get; set; }
        public string JzName { get; set; }
        public string KzName { get; set; }
        public string KlName { get; set; }
        public string SzName { get; set; }
        public string BzName { get; set; }
        public string JeName { get; set; }
        public string GblxName { get; set; }
        public string ZsName { get; set; }

        private IWindowManager windowManager;

        private AppSettingsSection _mainSetting;

        public WeighingRecordViewModel(IModelValidator<WeighingRecordViewModel> validator, Dictionary<string, string> templateFieldDic, IWindowManager _windowManager, WeighingRecordModel wrm = null) : base(validator)
        {
            _mainSetting = Common.Utility.SettingsHelper.AWSV2Settings;
            windowManager = _windowManager;
            CustomerList = SQLDataAccess.LoadActiveCustomer();
            CarList = SQLDataAccess.LoadActiveCar();
            UserList = SQLDataAccess.LoadActiveUser();
            GoodsList = SQLDataAccess.LoadActiveGoods();
            SpecList = Common.Data.SQLDataAccess.GetGoodsSpecs();
            GblxItems = Common.Data.SQLDataAccess.GetGblxItems().ToList();

            string rolePermission = SQLDataAccess.LoadLoginRolePermission(Globalspace._currentUser.LoginId);
            if (rolePermission != null)
            {
                if (rolePermission.Contains("修改皮重")) EnableUpdatePz = true;
                if (rolePermission.Contains("修改毛重")) EnableUpdateMz = true;
                if (rolePermission.Contains("修改净重")) EnableUpdateJz = true;
                if (rolePermission.Contains("修改实重")) EnableUpdateSz = true;
            }

            if (wrm == null)
            {
                Wrm = new WeighingRecordModel();
                Title = "新增称重记录";
                ShowLabelBhInfo = true;
            }
            else
            {
                Wrm = wrm;
                Title = "修改称重记录";

                SelectedCustomer = CustomerList.Find(u => u.Name == Wrm.Kh);
                SelectedCustomer2 = CustomerList.Find(u => u.Name == Wrm.Kh2);
                SelectedCustomer3 = CustomerList.Find(u => u.Name == Wrm.Kh3);
                SelectedCar = CarList.Find(c => c.PlateNo == Wrm.Ch);
                SelectedGoods = GoodsList.Find(g => g.Name == Wrm.Wz);
                Input_PlateNo = Wrm.Ch;
                Input_Wz = Wrm.Wz;
                SelectedSpec = SpecList.Find(g => g.Name == Wrm.GoodsSpec);
                Mzsby = UserList.Find(u => u.UserName == Wrm.Mzsby);
                Pzsby = UserList.Find(u => u.UserName == Wrm.Pzsby);
                Mzrq = Wrm.Mzrq;
                Pzrq = Wrm.Pzrq;
                Bh = Wrm.Bh;
                Mz = Wrm.Mz;
                Pz = Wrm.Pz;
                Jz = Wrm.Jz;
                Kz = Wrm.Kz;
                Kl = Wrm.Kl;
                Sz = Wrm.Sz;
                JE = Wrm.Je;
                Gblx = Wrm.Gblx;
                Zs = Wrm.AxleNum;

                By1Value = wrm.By1;
                By2Value = wrm.By2;
                By3Value = wrm.By3;
                By4Value = wrm.By4;
                By5Value = wrm.By5;

                By6Value = wrm.By6;
                By7Value = wrm.By7;
                By8Value = wrm.By8;
                By9Value = wrm.By9;
                By10Value = wrm.By10;

                By11Value = wrm.By11;
                By12Value = wrm.By12;
                By13Value = wrm.By13;
                By14Value = wrm.By14;
                By15Value = wrm.By15;

                By16Value = wrm.By16;
                By17Value = wrm.By17;
                By18Value = wrm.By18;
                By19Value = wrm.By19;
                By20Value = wrm.By20;

                WeighingTime = WeighingTimes.Find(s => s == Wrm.WeighingTimes.ToString());
                if (Wrm.IsFinish)
                {
                    Finish = IsFinish[1];
                }
                else
                {
                    Finish = IsFinish[0];
                }
            }

            foreach (var kv in templateFieldDic)
            {
                switch (kv.Key)
                {
                    case "_kh":
                        ShowKh = true;
                        KhName = kv.Value;
                        break;
                    case "_kh2":
                        ShowKh2 = true;
                        Kh2Name = kv.Value;
                        break;
                    case "_kh3":
                        ShowKh3 = true;
                        Kh3Name = kv.Value;
                        break;
                    case "_ch":
                        ShowCh = true;
                        ChName = kv.Value;
                        break;
                    case "_wz":
                        ShowWz = true;
                        WzName = kv.Value;
                        break;
                    case "_goodsSpec":
                        ShowGuiGe = true;
                        GuiGeName = kv.Value;
                        break;
                    case "_mz":
                        ShowMz = true;
                        MzName = kv.Value;
                        break;
                    case "_mzrq":
                        ShowMzrq = true;
                        MzrqName = kv.Value;
                        break;
                    case "_mzsby":
                        ShowMzsby = true;
                        MzsbyName = kv.Value;
                        break;
                    case "_pz":
                        ShowPz = true;
                        PzName = kv.Value;
                        break;
                    case "_pzrq":
                        ShowPzrq = true;
                        PzrqName = kv.Value;
                        break;
                    case "_pzsby":
                        ShowPzsby = true;
                        PzsbyName = kv.Value;
                        break;
                    case "_jz":
                        ShowJz = true;
                        JzName = kv.Value;
                        break;
                    case "_kz":
                        ShowKz = true;
                        KzName = kv.Value;
                        break;
                    case "_kl":
                        ShowKl = true;
                        KlName = kv.Value;
                        break;
                    case "_sz":
                        ShowSz = true;
                        SzName = kv.Value;
                        break;
                    case "_bz":
                        ShowBz = true;
                        BzName = kv.Value;
                        break;
                    case "_je":
                        ShowJe = true;
                        JeName = kv.Value;
                        break;
                    case "_gblx":
                        ShowGblx = true;
                        GblxName = kv.Value;
                        break;
                    case "_axleNum":
                        ShowZs = true;
                        ZsName = kv.Value;
                        break;
                    case "_by1":
                        ShowBy1 = true;
                        By1Name = kv.Value;
                        break;
                    case "_by2":
                        ShowBy2 = true;
                        By2Name = kv.Value;
                        break;
                    case "_by3":
                        ShowBy3 = true;
                        By3Name = kv.Value;
                        break;
                    case "_by4":
                        ShowBy4 = true;
                        By4Name = kv.Value;
                        break;
                    case "_by5":
                        ShowBy5 = true;
                        By5Name = kv.Value;
                        break;
                    case "_by6":
                        ShowBy6 = true;
                        By6Name = kv.Value;
                        break;
                    case "_by7":
                        ShowBy7 = true;
                        By7Name = kv.Value;
                        break;
                    case "_by8":
                        ShowBy8 = true;
                        By8Name = kv.Value;
                        break;
                    case "_by9":
                        ShowBy9 = true;
                        By9Name = kv.Value;
                        break;
                    case "_by10":
                        ShowBy10 = true;
                        By10Name = kv.Value;
                        break;
                    case "_by11":
                        ShowBy11 = true;
                        By11Name = kv.Value;
                        break;
                    case "_by12":
                        ShowBy12 = true;
                        By12Name = kv.Value;
                        break;
                    case "_by13":
                        ShowBy13 = true;
                        By13Name = kv.Value;
                        break;
                    case "_by14":
                        ShowBy14 = true;
                        By14Name = kv.Value;
                        break;
                    case "_by15":
                        ShowBy15 = true;
                        By15Name = kv.Value;
                        break;
                    case "_by16":
                        ShowBy16 = true;
                        By16Name = kv.Value;
                        break;
                    case "_by17":
                        ShowBy17 = true;
                        By17Name = kv.Value;
                        break;
                    case "_by18":
                        ShowBy18 = true;
                        By18Name = kv.Value;
                        break;
                    case "_by19":
                        ShowBy19 = true;
                        By19Name = kv.Value;
                        break;
                    case "_by20":
                        ShowBy20 = true;
                        By20Name = kv.Value;
                        break;
                    default:
                        break;
                }
            }

            //系统设置了扣重为固定值，不可以由用户输入
            if (_mainSetting.Settings["Discount"].Value.Equals("1"))
            {
                Kz = _mainSetting.Settings["DiscountWeight"].Value;
            }
            //系统设置了扣率为固定值，不可以由用户输入
            if (_mainSetting.Settings["Discount"].Value.Equals("2"))
            {
                Kz = _mainSetting.Settings["DiscountRate"].Value;
            }
        }

        public void SetValues()
        {
            if (!string.IsNullOrEmpty(Mz) && Mz != "0" && string.IsNullOrEmpty(Jz) && string.IsNullOrEmpty(Pz))
            {
                WeighingTime = "1";
                Finish = "未完成";
            }

            if (!string.IsNullOrEmpty(Jz) && Jz != "0")
            {
                WeighingTime = "2";
                Finish = "完成";
            }

        }

        public void SaveItem()
        {
            //判断MZRQ是否为空，如果是空的，就要自动生成编号。
            if (string.IsNullOrWhiteSpace(Mzrq))
            {
                //Bh = Common.Data.SQLDataAccess.CreateBh(DateTime.Now);
                Bh = CreateBh(DateTime.Now);
                Wrm.Bh = Bh;
            }

            ////2022-09-23 要求 毛重、皮重、净重都允许为空，所以加此if判断、
            //if (string.IsNullOrEmpty(Mz) && string.IsNullOrEmpty(Pz))
            //{
            //    windowManager.ShowMessageBox("毛重、皮重至少输入一个！");
            //    return;
            //}

            if (Validate())
            {
                Wrm.Mz = Mz;
                Wrm.Pz = Pz;
                Wrm.Kh = SelectedCustomer?.Name;
                Wrm.Kh2 = SelectedCustomer2?.Name;
                Wrm.Kh3 = SelectedCustomer3?.Name;

                Wrm.Wz = SelectedGoods == null ? Input_Wz : SelectedGoods.Name;
                Wrm.GoodsSpec = SelectedSpec?.Name;

                Wrm.Mzrq = Mzrq;
                if (!string.IsNullOrWhiteSpace(Wrm.Mz) && string.IsNullOrWhiteSpace(Wrm.Mzrq))
                {
                    Wrm.Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                Mzsby = Mzsby == null ? SQLDataAccess.LoadUser(Globalspace._currentUser.LoginId) : Mzsby;
                Wrm.Mzsby = Mzsby?.UserName;

                Wrm.Pzrq = Pzrq;
                if (!string.IsNullOrWhiteSpace(Wrm.Pz) && string.IsNullOrWhiteSpace(Wrm.Pzrq))
                {
                    Wrm.Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }

                Pzsby = Pzsby == null ? SQLDataAccess.LoadUser(Globalspace._currentUser.LoginId) : Pzsby;
                Wrm.Pzsby = Pzsby?.UserName;

                Wrm.Bh = Bh;
                Wrm.Jz = Jz;
                Wrm.Kz = Kz;
                Wrm.Kl = Kl;
                Wrm.Sz = Sz;
                Wrm.Je = JE;
                Wrm.Gblx = Gblx;
                Wrm.AxleNum = Zs;

                Wrm.By1 = By1Value;
                Wrm.By2 = By2Value;
                Wrm.By3 = By3Value;
                Wrm.By4 = By4Value;
                Wrm.By5 = By5Value;
                Wrm.By6 = By6Value;
                Wrm.By7 = By7Value;
                Wrm.By8 = By8Value;
                Wrm.By9 = By9Value;
                Wrm.By10 = By10Value;
                Wrm.By11 = By11Value;
                Wrm.By12 = By12Value;
                Wrm.By13 = By13Value;
                Wrm.By14 = By14Value;
                Wrm.By15 = By15Value;
                Wrm.By16 = By16Value;
                Wrm.By17 = By17Value;
                Wrm.By18 = By18Value;
                Wrm.By19 = By19Value;
                Wrm.By20 = By20Value;
                Wrm.EntryTime = DateTime.Now;

                Wrm.WeighingTimes = Convert.ToInt32(WeighingTime);

                //先处理用户输入的车牌号和物资，因为它们是输入的，不是选择的，数据库里没有。
                //此时为了保持上下文正常，需要再插入前先保存它们。
                if (SelectedCar == null && !string.IsNullOrEmpty(Input_PlateNo))
                {
                    try
                    {
                        //新增车辆信息
                        Common.Models.CarModel car = new Common.Models.CarModel();
                        car.PlateNo = Input_PlateNo;
                        car.Valid = true;
                        Common.Data.SQLDataAccess.SaveCar(car);

                        //获取出来给SelectedCar
                        SelectedCar = SQLDataAccess.LoadCar(Input_PlateNo);
                    }
                    catch { }
                }

                Wrm.Ch = SelectedCar == null ? Input_PlateNo : SelectedCar.PlateNo;

                //此时为了保持上下文正常，需要再插入前先保存它们。
                if (SelectedGoods == null && !string.IsNullOrEmpty(Input_Wz))
                {
                    try
                    {
                        //新增物资
                        Common.Models.GoodsModel goods = new Common.Models.GoodsModel();
                        goods.Num = Guid.NewGuid().ToString();
                        goods.Name = Input_Wz;
                        Common.Data.SQLDataAccess.SaveGoods(goods);
                    }
                    catch { }
                }

                VildOverLoad(Wrm);//超载验证处理
                SetValues();//保存之前再设置一边，以防止用户什么都不输入。。。

                Wrm.WeighName = _mainSetting.Settings["WeighName"].Value;

                if (Finish == "完成")
                {
                    //Wrm.Weigh2Name = _mainSetting.Settings["Weigh2Name"].Value;
                    Wrm.Weigh2Name = _mainSetting.Settings["WeighName"].Value;
                    Wrm.IsFinish = true;
                    if (!string.IsNullOrEmpty(Wrm.Pz))
                    {
                        if (!string.IsNullOrEmpty(Wrm.Mz))
                        {
                            var pz = decimal.Parse(Wrm.Pz);
                            var mz = decimal.Parse(Wrm.Mz);
                            if (pz > mz)
                            {
                                windowManager.ShowMessageBox("皮重不能大于毛重，请修改！");
                                return;
                            }
                            Wrm.Jz = (mz - pz).ToString();
                            Wrm.Jzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            windowManager.ShowMessageBox("毛重不能为空，请修改！");
                            return;
                        }
                    }
                    else
                    {
                        windowManager.ShowMessageBox("皮重不能为空，请修改！");
                        return;
                    }

                    //判断扣重、扣率是否都填写了。如果都填写了。需要提示。
                    if (
                        !string.IsNullOrWhiteSpace(Wrm.Kz) &&
                        !string.IsNullOrWhiteSpace(Wrm.Kl) &&
                        Wrm.Kz.Trim() != "0" &&
                        Wrm.Kl.Trim() != "0")
                    {
                        windowManager.ShowMessageBox("扣重、扣率只允许有一个，请处理！");
                        return;
                    }
                    else
                    {
                        //计算实重多少
                        if (!string.IsNullOrWhiteSpace(Wrm.Kz) && Wrm.Kz.Trim() != "0")
                        {
                            double val = 0;
                            double.TryParse(Wrm.Kz, out val);
                            // jz-kz=sz
                            Wrm.Sz = (double.Parse(Wrm.Jz) - val).ToString();
                        }
                        else if (!string.IsNullOrWhiteSpace(Wrm.Kl) && Wrm.Kl.Trim() != "0")
                        {
                            double val = 0;
                            double.TryParse(Wrm.Kl, out val);
                            //  jz*kl=sz
                            var t_jz = double.Parse(Wrm.Jz);
                            Wrm.Sz = (t_jz - (t_jz) * (val / 100)).ToString();

                        }
                    }
                }
                else
                    Wrm.IsFinish = false;

                if (Title == "新增称重记录")
                {
                    Wrm.IsPay = 0;
                    Wrm.SerialNumber = Common.Data.SQLDataAccess.CreateRecordSerialNumber();
                    SQLDataAccess.SaveWeighingRecord(Wrm);
                    SyncToCloud(Wrm.Bh, "insert");
                }

                if (Title == "修改称重记录")
                {
                    SQLDataAccess.UpdateWeighingRecord(Wrm);
                    SyncToCloud(Wrm.Bh, "update");
                    //上传平台
                    var obj = Common.Data.SQLDataAccess.GetWeighingRecordByBh(Wrm.Bh);
                    if (obj != null)
                    {
                        Common.SyncData.Instal.ModifyWeinightRecord(obj, Common.Api.edit);
                    }
                }

                this.RequestClose(true);
            }
        }

        private void VildOverLoad(WeighingRecordModel weighingRecord)
        {
            var overloadLog = _mainSetting.Settings["OverloadLog"].Value;
            var OverloadWarning = _mainSetting.Settings["OverloadWarning"].Value;
            var OverloadWarningWeight = _mainSetting.Settings["OverloadWarningWeight"].Value;
            var wighingTimes = weighingRecord.WeighingTimes == 2 ? "Second" : "First";

            if (OverloadWarning == "1") //毛重超载报警
            {
                string WeightStr = weighingRecord.Mz;
                decimal maxWeight = Convert.ToDecimal(OverloadWarningWeight);
                if (maxWeight > 0)
                {
                    if (Convert.ToDecimal(WeightStr) > maxWeight)
                    {
                        weighingRecord.IsLimit = true;
                        weighingRecord.LimitedValue = maxWeight.ToString();
                        weighingRecord.LimitType = "毛重超载";


                        SQLDataAccess.SavOverloadLog(new OverloadLog()
                        {
                            PlateNo = weighingRecord.Ch,
                            AxleCount = weighingRecord.AxleNum,
                            Constraints = "毛重超载",
                            OverloadWeight = WeightStr,
                            StandardWeight = maxWeight.ToString(),
                            Times = wighingTimes,
                            CreateDate = DateTime.Now
                        }, overloadLog);
                    }
                }
            }
            else if (OverloadWarning == "2") //净重超载报警
            {
                if (Convert.ToDecimal(weighingRecord.Jz) > Convert.ToDecimal(OverloadWarningWeight))
                {
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = OverloadWarningWeight;
                    weighingRecord.LimitType = "净重超载";

                    SQLDataAccess.SavOverloadLog(new OverloadLog()
                    {
                        PlateNo = weighingRecord.Ch,
                        AxleCount = weighingRecord.AxleNum,
                        Constraints = "净重超载",
                        OverloadWeight = weighingRecord.Jz,
                        StandardWeight = OverloadWarningWeight,
                        Times = wighingTimes,
                        CreateDate = DateTime.Now
                    }, overloadLog);
                }
            }
            else if (OverloadWarning == "3") //车轴计算超载报警
            {
                string WeightStr = weighingRecord.Mz;
                var apps = _mainSetting.Settings;
                var standard =
                     weighingRecord.AxleNum == "2" ? apps["OverloadAxle2"].Value :
                     weighingRecord.AxleNum == "3" ? apps["OverloadAxle3"].Value :
                     weighingRecord.AxleNum == "4" ? apps["OverloadAxle4"].Value :
                     weighingRecord.AxleNum == "5" ? apps["OverloadAxle5"].Value :
                     weighingRecord.AxleNum == "6" ? apps["OverloadAxle6"].Value : "0";

                if (Convert.ToDecimal(WeightStr) > Convert.ToDecimal(standard))
                {
                    weighingRecord.IsLimit = true;
                    weighingRecord.LimitedValue = standard;
                    weighingRecord.LimitType = "车轴计算超载";

                    SQLDataAccess.SavOverloadLog(new OverloadLog()
                    {
                        PlateNo = weighingRecord.Ch,
                        AxleCount = weighingRecord.AxleNum,
                        Constraints = "车轴计算超载",
                        OverloadWeight = WeightStr,
                        StandardWeight = standard,
                        Times = wighingTimes,
                        CreateDate = DateTime.Now
                    }, overloadLog);
                }

            }
        }


        public void GetCurrentTime(string s)
        {
            if (s == "Mzrq")
            {
                Mzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (s == "Pzrq")
            {
                Pzrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public void CalcJz()
        {
            if (Mz == null || Mz == "" || Pz == null || Pz == "")
            {
                return;
            }
            Jz = (Convert.ToDecimal(Mz) - Convert.ToDecimal(Pz)).ToString();
        }
        public void CalcSz()
        {
            //判断扣重、扣率是否都填写了。如果都填写了。需要提示。

            if (string.IsNullOrEmpty(Jz))
            {
                windowManager.ShowMessageBox("请先填写净重！");
                return;
            }

            if (
                !string.IsNullOrWhiteSpace(Kz) &&
                !string.IsNullOrWhiteSpace(Kl) &&
                Kz.Trim() != "0" &&
                Kl.Trim() != "0")
            {
                windowManager.ShowMessageBox("扣重、扣率只允许有一个，请处理！");
                return;
            }
            else
            {
                //计算实重多少
                if (!string.IsNullOrWhiteSpace(Kz) && Kz.Trim() != "0")
                {
                    double val = 0;
                    double.TryParse(Kz, out val);
                    // jz-kz=sz
                    Sz = (double.Parse(Jz) - val).ToString();
                }
                else if (!string.IsNullOrWhiteSpace(Kl) && Kl.Trim() != "0")
                {
                    double val = 0;
                    double.TryParse(Kl, out val);

                    //  jz*kl=sz
                    var t_jz = double.Parse(Jz);
                    Sz = (t_jz - (t_jz) * (val / 100)).ToString();
                }
            }
        }
        /// <summary>
        /// 生成称重编号
        /// </summary>
        /// <returns></returns>
        private string CreateBh(DateTime dt)
        {
            var prefix = _mainSetting.Settings["Prefix"]?.Value ?? string.Empty;
            var type = _mainSetting.Settings["GenerationType"]?.Value ?? string.Empty;
            return Common.Data.SQLDataAccess.CreateBh(dt, prefix, type);
        }
        private void SyncToCloud(string recordBh, string syncMode)
        {
            if (_mainSetting.Settings["SyncDataEnable"].Value == "True")
            {
                var SyncData = new SyncDataModel
                {
                    WeighingRecordBh = recordBh,
                    SyncMode = syncMode
                };
                SQLDataAccess.SaveSyncData(SyncData);
            }
        }
    }

    public class WeighingRecordViewModelValidator : AbstractValidator<WeighingRecordViewModel>
    {
        public WeighingRecordViewModelValidator()
        {
            RuleFor(x => x.Bh).NotEmpty().WithMessage("编号不能为空，请输入毛重日期生成编号！");
            //RuleFor(x => x.SelectedCar).NotEmpty().WithMessage("车号不能为空！");
            RuleFor(x => x.Mz).Custom((text, context) =>
            {  //2022-09-23 要求 毛重、皮重、净重都允许为空，所以加此if判断
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        decimal i = Convert.ToDecimal(text);
                    }
                    catch
                    {
                        context.AddFailure("请输入数字");
                    }
                }
            });
            RuleFor(x => x.Pz).Custom((text, context) =>
            {  //2022-09-23 要求 毛重、皮重、净重都允许为空，所以加此if判断
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        decimal i = Convert.ToDecimal(text);
                    }
                    catch
                    {
                        context.AddFailure("请输入数字");
                    }
                }
            });
            RuleFor(x => x.Jz).Custom((text, context) =>
            {
                //2022-09-23 要求 毛重、皮重、净重都允许为空，所以加此if判断
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        decimal i = Convert.ToDecimal(text);
                    }
                    catch
                    {
                        context.AddFailure("请输入数字");
                    }
                }

            });
            RuleFor(x => x.Kz).Custom((text, context) =>
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        decimal i = Convert.ToDecimal(text);
                    }
                    catch
                    {
                        context.AddFailure("请输入数字");
                    }
                }
            });
            RuleFor(x => x.Kl).Custom((text, context) =>
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        int i = Convert.ToInt32(text);
                        if (i > 100)
                        {
                            context.AddFailure("扣率不能大于100%");
                        }
                        if (i < 0)
                        {
                            context.AddFailure("扣率不能小于0");
                        }
                    }
                    catch
                    {
                        context.AddFailure("请输入数字");
                    }
                }
            });
            RuleFor(x => x.Sz).Custom((text, context) =>
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        decimal i = Convert.ToDecimal(text);
                    }
                    catch
                    {
                        context.AddFailure("请输入数字");
                    }
                }
            });
        }
    }
}
