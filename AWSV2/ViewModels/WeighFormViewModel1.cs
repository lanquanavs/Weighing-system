using Aspose.Cells;
using AWSV2.Models;
using System;
using System.Windows;
using System.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using AWSV2.Services;
using System.Windows.Extensions;
using System.Configuration;
using System.Reflection;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace AWSV2.ViewModels
{
    public class WeighFormViewModel1
    {
        //页面上绑定的属性
        public Grid WeighFormGrid { get; set; } = new Grid();
        public StackPanel WeighFormStackPanel { get; set; } = new StackPanel();

        //用来存放excel过磅单模板的DataTable,List
        DataTable dataTable;
        Font[,] styleTable;
        List<WeighFormModel> WeighFormList = new List<WeighFormModel>();
        Common.Models.GoodsSpecModel specItem = new Common.Models.GoodsSpecModel();
        public Common.Models.GoodsSpecModel SpecItem
        {
            get
            {
                return specItem;
            }
            set
            {
                specItem = value;
            }
        }
        ObservableCollection<Common.Models.GoodsSpecModel> specList = new ObservableCollection<Common.Models.GoodsSpecModel>();
        public ObservableCollection<Common.Models.GoodsSpecModel> SpecList
        {
            get
            {
                return specList;
            }
            set
            {
                specList = value;
            }
        }

        GoodsModel _goodsItem;
        public GoodsModel GoodsItem
        {
            get { return _goodsItem; }
            set
            {
                _goodsItem = value;
                if (value != null)
                {

                    SpecList = new ObservableCollection<Common.Models.GoodsSpecModel>();
                    foreach (var i in Common.Data.SQLDataAccess.GetGoodsSpecs(value.Name))
                    {

                        SpecList.Add(i);
                    }

                    foreach (var control in this.WeighFormGrid.Children)
                    {
                        var comboBox = control as Control;
                        if (comboBox != null && comboBox.Name == "GoodsSpec")
                        {
                            (comboBox as AutoCompleteBox).ItemsSource = SpecList;
                        }
                    }

                }
                //SpecList = new ObservableCollection<Common.Models.GoodsSpecModel>(Common.Data.SQLDataAccess.GetGoodsSpecs(value.Name));
            }
        }
        string _ch;
        public string CH
        {
            get
            {
                return _ch;
            }
            set
            {
                _ch = value;
                if (!string.IsNullOrEmpty(value))
                {
                    var carInfo = Common.Data.SQLDataAccess.GetCarModel(value);
                    if (carInfo != null)
                    {

                        foreach (var control in this.WeighFormGrid.Children)
                        {
                            var txtbox = control as Control;
                            if (txtbox != null && txtbox.Name == "Driver")
                            {
                                (txtbox as TextBox).Text = carInfo.Driver;
                            }
                            else if (txtbox != null && txtbox.Name == "DriverPhone")
                            {
                                (txtbox as TextBox).Text = carInfo.DriverPhone;
                            }
                        }

                        //Driver = carInfo.Driver;
                        //DriverPhone = carInfo.DriverPhone;
                        //Source.Driver = carInfo.Driver;
                        //Source.DriverPhone = carInfo.DriverPhone;

                        //WeighFormGrid.DataContext = Source;
                        //WeighFormStackPanel.DataContext = Source;
                    }
                }
            }
        }

        string _driver;
        public string Driver
        {
            get
            {
                return _driver;
            }
            set
            {
                _driver = value;
            }
        }

        string _driverPhone;
        public string DriverPhone
        {
            get
            {
                return _driverPhone;
            }
            set
            {
                _driverPhone = value;
            }
        }
        WeighingRecordModel _source;
        public WeighingRecordModel Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }

        //aspose.cell破解100次限制
        internal static void InitializeAsposeCells()
        {
            const BindingFlags BINDING_FLAGS_ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            const string CLASS_LICENSER = "\u0092\u0092\u0008.\u001C";
            const string CLASS_LICENSERHELPER = "\u0011\u0001\u0006.\u001A";
            const string ENUM_ISTRIAL = "\u0092\u0092\u0008.\u001B";

            const string FIELD_LICENSER_CREATED_LICENSE = "\u0001";     // static
            const string FIELD_LICENSER_EXPIRY_DATE = "\u0002";         // instance
            const string FIELD_LICENSER_ISTRIAL = "\u0001";             // instance

            const string FIELD_LICENSERHELPER_INT128 = "\u0001";        // static
            const string FIELD_LICENSERHELPER_BOOLFALSE = "\u0001";     // static

            const int CONST_LICENSER_ISTRIAL = 1;
            const int CONST_LICENSERHELPER_INT128 = 128;
            const bool CONST_LICENSERHELPER_BOOLFALSE = false;

            //- Field setter for convinient
            Action<FieldInfo, Type, string, object, object> setValue =
                delegate (FieldInfo field, Type chkType, string chkName, object obj, object value)
                {
                    if ((field.FieldType == chkType) && (field.Name == chkName))
                    {
                        field.SetValue(obj, value);
                    }
                };


            //- Get types
            Assembly assembly = Assembly.GetAssembly(typeof(Aspose.Cells.License));
            Type typeLic = null, typeIsTrial = null, typeHelper = null;
            foreach (Type type in assembly.GetTypes())
            {
                if ((typeLic == null) && (type.FullName == CLASS_LICENSER))
                {
                    typeLic = type;
                }
                else if ((typeIsTrial == null) && (type.FullName == ENUM_ISTRIAL))
                {
                    typeIsTrial = type;
                }
                else if ((typeHelper == null) && (type.FullName == CLASS_LICENSERHELPER))
                {
                    typeHelper = type;
                }
            }
            if (typeLic == null || typeIsTrial == null || typeHelper == null)
            {
                throw new Exception();
            }

            //- In class_Licenser
            object license = Activator.CreateInstance(typeLic);
            foreach (FieldInfo field in typeLic.GetFields(BINDING_FLAGS_ALL))
            {
                setValue(field, typeLic, FIELD_LICENSER_CREATED_LICENSE, null, license);
                setValue(field, typeof(DateTime), FIELD_LICENSER_EXPIRY_DATE, license, DateTime.MaxValue);
                setValue(field, typeIsTrial, FIELD_LICENSER_ISTRIAL, license, CONST_LICENSER_ISTRIAL);
            }

            //- In class_LicenserHelper
            foreach (FieldInfo field in typeHelper.GetFields(BINDING_FLAGS_ALL))
            {
                setValue(field, typeof(int), FIELD_LICENSERHELPER_INT128, null, CONST_LICENSERHELPER_INT128);
                setValue(field, typeof(bool), FIELD_LICENSERHELPER_BOOLFALSE, null, CONST_LICENSERHELPER_BOOLFALSE);
            }
        }

        public WeighFormViewModel1(WeighingRecordModel weighingRecord, string templateName)
        {
            Source = weighingRecord;
            //绑定页面数据
            WeighFormGrid.DataContext = Source;
            WeighFormStackPanel.DataContext = Source;

            InitializeAsposeCells();

            //设置边框线的颜色
            var border = new System.Windows.Controls.Border();
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(0, 0, 0, 0);
            //Grid.SetRow(border, WeighFormGrid.RowDefinitions.Count - 1);
            Grid.SetRow(border, 0);
            Grid.SetColumn(border, 0);
            WeighFormGrid.Children.Add(border);
            WeighFormGrid.Width= double.NaN;

            //通过excel过磅单模板绘制xaml过磅单到页面
            if (ConfigurationManager.AppSettings["WeighFormDisplayMode"].Equals("Priview"))
            {
                LoadExcelToDT(templateName); //读取excel，存放数据和字体到全局变量里
                DisplayToGrid(weighingRecord); //使用dataTable、styleTable绘制xaml
            }
            else
            {
                LoadExcelToList(templateName);
                DisplayToStackPanel(weighingRecord);
            }
        }

        private void LoadExcelToDT(string templateName)
        {
            try
            {
                Workbook workbook = new Workbook(Globalspace._weightFormTemplatePath);
                Worksheet worksheet = workbook.Worksheets[templateName];
                Cells cells = worksheet.Cells;

                int maxRow = cells.MaxRow + 1;
                int maxColumn = cells.MaxColumn + 1;

                dataTable = cells.ExportDataTable(0, 0, maxRow, maxColumn);

                styleTable = new Font[maxRow, maxColumn];

                for (int i = 0; i < maxRow; i++)
                {
                    for (int j = 0; j < maxColumn; j++)
                    {
                        styleTable[i, j] = cells.GetCellStyle(i, j).Font;
                    }
                }
            }
            catch { }
        }

        private void LoadExcelToList(string templateName)
        {
            //读取过磅单中的字段
            try
            {
                //打开过磅单模板
                Worksheet worksheet = new Workbook(Globalspace._weightFormTemplatePath).Worksheets[templateName];

                //设置查找区域：有内容的区域
                var range = worksheet.Cells.MaxDisplayRange;
                string cellRange = range.RefersTo.Split('!')[1].Replace("$", "");
                string startCellName = cellRange.Split(':')[0];
                string endCellName = cellRange.Split(':')[1];
                CellArea area = CellArea.CreateCellArea(startCellName, endCellName);
                //查找规则
                FindOptions opts = new FindOptions();
                opts.LookInType = LookInType.Values;
                opts.LookAtType = LookAtType.Contains;
                opts.SetRange(area);

                Cell cell = null;
                do
                {
                    // Search the cell contain value search within range
                    cell = worksheet.Cells.Find("_", cell, opts);

                    // If no such cell found, then break the loop
                    if (cell == null)
                        break;

                    // 通过读取配置文件把key-value放到WeighFormList中
                    WeighFormModel wf = new WeighFormModel
                    {
                        Key = cell.Value.ToString(),
                        Value = ConfigurationManager.AppSettings[cell.Value.ToString()]
                    };
                    WeighFormList.Add(wf);

                } while (true);
            }
            catch { }
        }

        private void DisplayToGrid(WeighingRecordModel weighingRecord)
        {
            //画grid的行列
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                WeighFormGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(65, GridUnitType.Pixel) });
            }
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                WeighFormGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength((i == 0 || i == 2) ? 100 : 450, GridUnitType.Pixel) });
            }

            //FrameworkElementFactory GridFactory = new FrameworkElementFactory(typeof(Grid));
            //GridFactory.SetValue(Grid.b, new Binding("Key"));
            //dt.VisualTree = fef;
            //WeighFormGrid. =false;


            //给grid填充数据
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    string gridContent = dataTable.Rows[i][j].ToString();

                    if (!gridContent.Equals("")) //datatable不为空填充数据，else为空合并到前一个单元格
                    {
                        if (gridContent.StartsWith("_")) //以 _ 开头为变量，else为textblock
                        {
                            //去掉_，首字母大写
                            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
                            string bdName = gridContent.Substring(1);
                            bdName = bdName.Replace(bdName[0], ti.ToUpper(bdName[0]));
                           
                            if (ConfigurationManager.AppSettings["WeighingControl"].Equals("Hand"))
                            {
                                if (bdName == "GoodsSpec" || bdName == "AxleNum" || bdName == "Gblx" || bdName == "Ch" || bdName == "Wz" || bdName.StartsWith("Kh") || bdName.StartsWith("By"))
                                {
                                    AutoCompleteBox acb = new AutoCompleteBox
                                    {
                                        FilterMode = AutoCompleteFilterMode.Contains,
                                        //BorderBrush = System.Windows.Media.Brushes.LightGray
                                    }; 
                                    //acb.Style = (System.Windows.Style)new FrameworkElement().TryFindResource("");
                                    acb.BorderBrush= new SolidColorBrush(System.Windows.Media.Color.FromArgb(((byte)0xFF), ((byte)0xE0), ((byte)0xE6), ((byte)0xEC)));
                                    acb.Height = 40;
                                    acb.FontSize = 16;
                                    acb.Padding = new Thickness(0, 0, 20, 0);
                                    acb.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    acb.Width = double.NaN;

                                    Binding binding = new Binding(bdName)
                                    {
                                        Mode = BindingMode.TwoWay
                                    };
                                    acb.SetBinding(AutoCompleteBox.TextProperty, binding);
                                    
                                    var style = new System.Windows.Style(typeof(TextBox));
                                    style.Setters.Add(new Setter(TextBox.VerticalContentAlignmentProperty, System.Windows.VerticalAlignment.Center));
                                    acb.TextBoxStyle = style;

                                    acb.MinimumPopulateDelay = 100;
                                    acb.MinimumPrefixLength = 0;
                                    acb.GotFocus += (sender, args) =>
                                    {
                                        if (string.IsNullOrEmpty(acb.Text))
                                        {
                                            acb.IsDropDownOpen = true;
                                        }
                                    };

                                    //添加到grid对应格子中
                                    WeighFormGrid.Children.Add(acb);
                                    acb.SetValue(Grid.RowProperty, i);
                                    acb.SetValue(Grid.ColumnProperty, j);
                                    acb.MinWidth = 90;

                                    if (bdName == "AxleNum")
                                    {
                                        DataTemplate dt = new DataTemplate();
                                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                        fef.SetValue(Label.ContentProperty, new Binding("Key"));
                                        dt.VisualTree = fef;

                                        Dictionary<string, string> dic = new Dictionary<string, string>();
                                        dic.Add("2", "2");
                                        dic.Add("3", "3");
                                        dic.Add("4", "4");
                                        dic.Add("5", "5");
                                        dic.Add("6", "6");

                                        acb.ItemsSource = dic;
                                        acb.ValueMemberPath = "Key";
                                        acb.SelectedItem = weighingRecord.AxleNum;
                                        acb.ItemTemplate = dt;
                                    }
                                    if (bdName == "Gblx")
                                    {
                                        DataTemplate dt = new DataTemplate();
                                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                        fef.SetValue(Label.ContentProperty, new Binding("Key"));
                                        dt.VisualTree = fef;

                                        Dictionary<string, string> dic = new Dictionary<string, string>();

                                       var items= Common.Data.SQLDataAccess.GetGblxItems();
                                        foreach (var item in items)
                                        {
                                            dic.Add(item, item);
                                        }

                                        //dic.Add("销售", "销售");
                                        //dic.Add("采购", "采购");
                                        //dic.Add("其他", "其他");

                                        acb.ItemsSource = dic;
                                        acb.ValueMemberPath = "Key";
                                        acb.SelectedItem = weighingRecord.Gblx;
                                        acb.ItemTemplate = dt;
                                    }
                                    if (bdName == "Wz")
                                    {
                                        Binding binding1 = new Binding()
                                        {
                                            Mode = BindingMode.TwoWay,
                                            Path = new PropertyPath(nameof(GoodsItem)),
                                            Source = this
                                        };
                                        acb.SetBinding(AutoCompleteBox.SelectedItemProperty, binding1);

                                        DataTemplate dt = new DataTemplate();
                                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                        fef.SetValue(Label.ContentProperty, new Binding("Name"));
                                        dt.VisualTree = fef;

                                        List<GoodsModel> goodsList = SQLDataAccess.LoadActiveGoods();
                                        //var groupByNames = goodsList.GroupBy(g => g.Name).ToList();
                                        // GoodsItem= goodsList.Find(g => g.Name.Equals(_weighingRecord.Wz));
                                        acb.ItemsSource = goodsList;
                                        acb.ValueMemberPath = "Name";
                                        acb.SelectedItem = goodsList.Find(g => g.Name.Equals(weighingRecord.Wz));
                                        acb.ItemTemplate = dt;
                                    }
                                    if (bdName == "GoodsSpec")
                                    {
                                        //Binding binding1 = new Binding()
                                        //{
                                        //    Mode = BindingMode.TwoWay,
                                        //    Path = new PropertyPath(nameof(SpecList)),
                                        //    Source = this
                                        //};

                                        //acb.SetBinding(AutoCompleteBox.ItemsSourceProperty, binding1);

                                        DataTemplate dt = new DataTemplate();
                                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                        fef.SetValue(Label.ContentProperty, new Binding("Name"));
                                        dt.VisualTree = fef;
                                        acb.Name = "GoodsSpec";
                                        // acb.PreviewKeyDown += Acb_PreviewKeyDown;
                                        acb.KeyDown += Acb_KeyDown;
                                        acb.KeyUp += Acb_KeyUp;
                                        // List<Common.Models.GoodsSpecModel> specList = Common.Data.SQLDataAccess.GetGoodsSpecs(_weighingRecord.Wz);
                                        //SpecList = new ObservableCollection<Common.Models.GoodsSpecModel>(Common.Data.SQLDataAccess.GetGoodsSpecs(_weighingRecord.Wz));
                                        acb.ItemsSource = new ObservableCollection<Common.Models.GoodsSpecModel>(Common.Data.SQLDataAccess.GetGoodsSpecs(weighingRecord.Wz));
                                        acb.ValueMemberPath = "Name";
                                        acb.SelectedItem = SpecList.FirstOrDefault(g => g.Name.Equals(weighingRecord.GoodsSpec));
                                        acb.ItemTemplate = dt;
                                    }
                                    if (bdName == "Ch")
                                    {
                                        //Binding binding_CH_Text = new Binding()
                                        //{
                                        //    Mode = BindingMode.TwoWay,
                                        //    Path = new PropertyPath(nameof(CH)),
                                        //    Source = this
                                        //};
                                        //acb.SetBinding(AutoCompleteBox.TextProperty, binding_CH_Text);
                                        //acb.KeyUp += Acb_KeyUp1;
                                        acb.SelectionChanged += Acb_SelectionChanged;

                                        DataTemplate dt = new DataTemplate();
                                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                        fef.SetValue(Label.ContentProperty, new Binding("PlateNo"));
                                        dt.VisualTree = fef;

                                        List<CarModel> carList = SQLDataAccess.LoadActiveCar();
                                        acb.ItemsSource = carList;
                                        acb.ValueMemberPath = "PlateNo";
                                        acb.SelectedItem = carList.Find(c => c.PlateNo.Equals(weighingRecord.Ch));
                                        acb.ItemTemplate = dt;
                                    }
                                    if (bdName.StartsWith("Kh"))
                                    {
                                        DataTemplate dt = new DataTemplate();
                                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                        fef.SetValue(Label.ContentProperty, new Binding("Name"));
                                        dt.VisualTree = fef;

                                        List<CustomerModel> customerList = SQLDataAccess.LoadActiveCustomer();
                                        acb.ItemsSource = customerList;
                                        acb.ValueMemberPath = "Name";
                                        acb.SelectedItem = customerList.Find(c => c.Name.Equals(weighingRecord.Kh));
                                        acb.ItemTemplate = dt;
                                        AutoCompleteBoxHelper.SetOtherPaths(acb, "Num,Name");
                                    }
                                    if (bdName.StartsWith("Je"))
                                    {
                                        DataTemplate dt = new DataTemplate();
                                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                        fef.SetValue(Label.ContentProperty, new Binding("Name"));
                                        dt.VisualTree = fef;

                                        List<CustomerModel> customerList = SQLDataAccess.LoadActiveCustomer();
                                        acb.ItemsSource = customerList;
                                        acb.ValueMemberPath = "Name";
                                        acb.SelectedItem = customerList.Find(c => c.Name.Equals(weighingRecord.Je));
                                        acb.ItemTemplate = dt;
                                        AutoCompleteBoxHelper.SetOtherPaths(acb, "Num,Name");
                                    }
                                    if (bdName.StartsWith("By"))
                                    {
                                        List<string> byValueList = SQLDataAccess.LoadByxValue(bdName);
                                        acb.ItemsSource = byValueList;
                                    }
                                }
                                else
                                {
                                    TextBox tb = new TextBox();
                                    tb.SetBinding(TextBox.TextProperty, new Binding(bdName));
                                    //添加到grid对应格子中
                                    WeighFormGrid.Children.Add(tb);
                                    tb.SetValue(Grid.RowProperty, i);
                                    tb.SetValue(Grid.ColumnProperty, j);
                                    tb.MinWidth = 90;
                                    tb.IsReadOnly = true;

                                    //tb.Template = (ControlTemplate)new FrameworkElement().TryFindResource("TextBoxTemplatePlaceHolder");
                                    tb.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(((byte)0xFF), ((byte)0xE0), ((byte)0xE6), ((byte)0xEC)));
                                    tb.Height = 40;
                                    tb.FontSize = 16;
                                    tb.Padding = new Thickness(0, 0, 20, 0);
                                    tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    tb.Width = double.NaN;

                                    if (bdName == "Kz")
                                    {
                                        tb.IsReadOnly = ConfigurationManager.AppSettings["Discount"].Equals("1");
                                    }

                                    if (bdName == "Kl")
                                    {
                                        tb.IsReadOnly = ConfigurationManager.AppSettings["Discount"].Equals("2");
                                    }


                                    if (bdName == "Bz" || bdName == "Je" || bdName == "Fhdw" || bdName == "Driver" || bdName == "DriverPhone")
                                    {
                                        tb.IsReadOnly = false;
                                    }
                                    if (ConfigurationManager.AppSettings["WeighingMode"].Equals("Once") && bdName == "Pz")
                                    {
                                        tb.IsReadOnly = false;
                                    }
                                    if (bdName == "Driver")
                                    {
                                        tb.Name = "Driver";
                                    }
                                    if (bdName == "DriverPhone")
                                    {
                                        tb.Name = "DriverPhone";
                                    }
                                    //if (bdName == "Driver")
                                    //{
                                    //    Binding binding_Driver_Text = new Binding()
                                    //    {
                                    //        Mode = BindingMode.TwoWay,
                                    //        Path = new PropertyPath(nameof(Driver)),
                                    //        Source = this
                                    //    };
                                    //    tb.SetBinding(TextBox.TextProperty, binding_Driver_Text);
                                    //}
                                    //if (bdName == "DriverPhone")
                                    //{
                                    //    Binding binding_DriverPhone_Text = new Binding()
                                    //    {
                                    //        Mode = BindingMode.TwoWay,
                                    //        Path = new PropertyPath(nameof(DriverPhone)),
                                    //        Source = this
                                    //    };
                                    //    tb.SetBinding(TextBox.TextProperty, binding_DriverPhone_Text);
                                    //}

                                }
                            }
                            else //自动称重模式，全部为readonly的textbox
                            {
                                TextBox tb = new TextBox();
                                tb.SetBinding(TextBox.TextProperty, new Binding(bdName));

                                //if (bdName == "Ch")
                                //{
                                //    Binding binding_CH_Text = new Binding()
                                //    {
                                //        Mode = BindingMode.TwoWay,
                                //        Path = new PropertyPath(nameof(CH)),
                                //        Source = this
                                //    };
                                //    tb.SetBinding(TextBox.TextProperty, binding_CH_Text);
                                //}
                                tb.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(((byte)0xFF), ((byte)0xE0), ((byte)0xE6), ((byte)0xEC)));
                                tb.Height = 40;
                                tb.FontSize = 16;
                                tb.Padding = new Thickness(0, 0, 20, 0);
                                tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                                tb.Width = double.NaN;
                                //tb.Template = (ControlTemplate)new FrameworkElement().TryFindResource("TextBoxTemplatePlaceHolder");

                                //添加到grid对应格子中
                                WeighFormGrid.Children.Add(tb);
                                tb.SetValue(Grid.RowProperty, i);
                                tb.SetValue(Grid.ColumnProperty, j);
                                tb.MinWidth = 90;
                                tb.IsReadOnly = true;
                            }

                        } //以 _ 开头为变量，else为textblock
                        else //静态内容，textblock
                        {
                            var font = styleTable[i, j];
                            Label tb = new Label
                            {
                                //设置静态文本的内容和格式
                                Content = gridContent,
                                //HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                                //VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                FontSize = font.Size,
                                FontFamily = new System.Windows.Media.FontFamily(font.Name)
                            };
                            tb.Foreground= new SolidColorBrush(System.Windows.Media.Color.FromRgb(((byte)0x22), ((byte)0x18), ((byte)0x16)));
                            tb.Height = 40;
                            tb.FontSize = 16;
                            tb.VerticalContentAlignment = VerticalAlignment.Center;
                            tb.HorizontalAlignment = HorizontalAlignment.Right;
                            tb.Width = double.NaN;

                            if (font.IsBold)
                                tb.FontWeight = FontWeights.Bold;
                            if (font.IsItalic)
                                tb.FontStyle = FontStyles.Italic;

                            //添加到grid对应格子中
                            WeighFormGrid.Children.Add(tb);
                            tb.SetValue(Grid.RowProperty, i);
                            tb.SetValue(Grid.ColumnProperty, j);
                        } //静态内容，textblock
                    } //datatable不为空填充数据，else为空合并到前一个单元格
                    else //grid中显示单元格合并
                    {
                        if (WeighFormGrid.Children.Count <= 0) continue;
                        var item = WeighFormGrid.Children[WeighFormGrid.Children.Count - 1];
                        int columnSpanNum = Convert.ToInt32(item.GetValue(Grid.ColumnSpanProperty)) + 1;
                        item.SetValue(Grid.ColumnSpanProperty, columnSpanNum);
                    } //grid中显示单元格合并
                }
            } //给grid填充数据

            GridHelper.SetShowBorder(WeighFormGrid, false);
        }

        private void Acb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var comboBox = (sender as AutoCompleteBox);

                if (!comboBox.IsLoaded)
                    return;
                SetValues(comboBox.Text);
            }
            catch { }
        }

        private void SetValues(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var carInfo = Common.Data.SQLDataAccess.GetCarModel(value);
                if (carInfo != null)
                {

                    foreach (var control in this.WeighFormGrid.Children)
                    {
                        var txtbox = control as Control;
                        if (txtbox != null && txtbox.Name == "Driver")
                        {
                            (this.WeighFormGrid.DataContext as WeighingRecordModel).Driver = carInfo.Driver;

                            //WeighFormStackPanel.DataContext = Source;
                            (txtbox as TextBox).Text = carInfo.Driver;
                        }
                        else if (txtbox != null && txtbox.Name == "DriverPhone")
                        {
                            (this.WeighFormGrid.DataContext as WeighingRecordModel).DriverPhone = carInfo.DriverPhone;
                            (txtbox as TextBox).Text = carInfo.DriverPhone;
                        }
                    }

                }
            }
        }

        private void Acb_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Back && e.Key != System.Windows.Input.Key.Delete)
            {
                (sender as AutoCompleteBox).Text = String.Empty;
                e.Handled = true;
            }
        }

        private void Acb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Back && e.Key != System.Windows.Input.Key.Delete)
            {
                (sender as AutoCompleteBox).Text = String.Empty;
                e.Handled = true;
            }
        }

        private void DisplayToStackPanel(WeighingRecordModel weighingRecord)
        {
            DockPanel dp = new DockPanel();
            dp.MinWidth = 150;
            Queue<DockPanel> dpList = new Queue<DockPanel>();

            string[] sortList = ConfigurationManager.AppSettings["ListSort"].Split(',');
            List<string> sortKeyList = new List<string>(sortList);

            //把有顺序的项目按顺序显示
            foreach (var sortItem in sortKeyList)
            {
                for (int i = 0; i < WeighFormList.Count; i++)
                {
                    if (sortItem == WeighFormList[i].Key)
                    {
                        WeighFormList.Remove(WeighFormList[i]);
                        //去掉_，首字母大写，用来匹配WeighingRecordModel数据库字段
                        TextInfo ti = new CultureInfo("en-US", false).TextInfo;
                        string bdName = sortItem.Substring(1);
                        bdName = bdName.Replace(bdName[0], ti.ToUpper(bdName[0]));

                        TextBlock tbk = new TextBlock();
                        tbk.Text = ConfigurationManager.AppSettings[sortItem];
                        tbk.VerticalAlignment = VerticalAlignment.Center;
                        tbk.TextAlignment = TextAlignment.Right;

                        tbk.Margin = new Thickness(5);
                        dp.Children.Add(tbk);

                        if (ConfigurationManager.AppSettings["WeighingControl"].Equals("Hand"))
                        {

                            if (bdName == "Ch" || bdName == "Wz" || bdName.StartsWith("Kh") || bdName.StartsWith("By"))
                            {
                                AutoCompleteBox acb = new AutoCompleteBox
                                {
                                    FilterMode = AutoCompleteFilterMode.Contains,
                                    BorderBrush = System.Windows.Media.Brushes.LightGray
                                };

                                Binding binding = new Binding(bdName)
                                {
                                    Mode = BindingMode.TwoWay
                                };
                                acb.SetBinding(AutoCompleteBox.TextProperty, binding);

                                var style = new System.Windows.Style(typeof(TextBox));
                                style.Setters.Add(new Setter(TextBox.VerticalContentAlignmentProperty, System.Windows.VerticalAlignment.Center));
                                acb.TextBoxStyle = style;

                                acb.MinimumPopulateDelay = 100;
                                acb.MinimumPrefixLength = 0;
                                acb.GotFocus += (sender, args) =>
                                {
                                    if (string.IsNullOrEmpty(acb.Text))
                                    {
                                        acb.IsDropDownOpen = true;
                                    }
                                };

                                acb.Margin = new Thickness(5);

                                //添加到grid对应格子中
                                dp.Children.Add(acb);
                                //acb.MinWidth = 90;
                                if (bdName == "Wz")
                                {
                                    DataTemplate dt = new DataTemplate();
                                    FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                    fef.SetValue(Label.ContentProperty, new Binding("Name"));
                                    dt.VisualTree = fef;

                                    List<GoodsModel> goodsList = SQLDataAccess.LoadActiveGoods();
                                    acb.ItemsSource = goodsList;
                                    acb.ValueMemberPath = "Name";
                                    acb.SelectedItem = goodsList.Find(g => g.Id.Equals(weighingRecord.Wz));
                                    acb.ItemTemplate = dt;
                                }
                                if (bdName == "Ch")
                                {
                                    DataTemplate dt = new DataTemplate();
                                    FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                    fef.SetValue(Label.ContentProperty, new Binding("PlateNo"));
                                    dt.VisualTree = fef;

                                    List<CarModel> carList = SQLDataAccess.LoadActiveCar();
                                    acb.ItemsSource = carList;
                                    acb.ValueMemberPath = "PlateNo";
                                    acb.SelectedItem = carList.Find(c => c.AutoNo.Equals(weighingRecord.Ch));
                                    acb.ItemTemplate = dt;
                                }
                                if (bdName.StartsWith("Kh"))
                                {
                                    DataTemplate dt = new DataTemplate();
                                    FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                                    fef.SetValue(Label.ContentProperty, new Binding("Name"));
                                    dt.VisualTree = fef;

                                    List<CustomerModel> customerList = SQLDataAccess.LoadActiveCustomer();
                                    acb.ItemsSource = customerList;
                                    acb.ValueMemberPath = "Name";
                                    acb.SelectedItem = customerList.Find(c => c.Id.Equals(weighingRecord.Kh));
                                    acb.ItemTemplate = dt;
                                    AutoCompleteBoxHelper.SetOtherPaths(acb, "Num,Name");
                                }
                                if (bdName.StartsWith("By"))
                                {
                                    List<string> byValueList = SQLDataAccess.LoadByxValue(bdName);
                                    acb.ItemsSource = byValueList;
                                }
                            }
                            else
                            {
                                TextBox tb = new TextBox();
                                tb.SetBinding(TextBox.TextProperty, new Binding(bdName));
                                //添加到grid对应格子中
                                dp.Children.Add(tb);
                                //tb.MinWidth = 90;
                                tb.IsReadOnly = true;
                                tb.Margin = new Thickness(5);

                                if (ConfigurationManager.AppSettings["WeighingControl"].Equals("Hand"))
                                {
                                    if (bdName == "Kz" || bdName == "Kl" || bdName == "Bz" || bdName.StartsWith("By"))
                                    {
                                        tb.IsReadOnly = false;
                                    }
                                    if (ConfigurationManager.AppSettings["WeighingMode"].Equals("Once") && bdName == "Pz")
                                    {
                                        tb.IsReadOnly = false;
                                    }
                                }
                            }
                        }
                        else //自动称重模式，全部为readonly的textbox
                        {
                            TextBox tb = new TextBox();
                            tb.SetBinding(TextBox.TextProperty, new Binding(bdName));
                            //添加到grid对应格子中
                            dp.Children.Add(tb);
                            //tb.MinWidth = 90;
                            tb.IsReadOnly = true;
                            tb.Margin = new Thickness(5);
                        }

                        dpList.Enqueue(dp);
                        dp = new DockPanel();
                        dp.MinWidth = 150;
                    }

                }
            }
            for (int i = 0; i < WeighFormList.Count; i++)
            {
                //去掉_，首字母大写，用来匹配WeighingRecordModel数据库字段
                TextInfo ti = new CultureInfo("en-US", false).TextInfo;
                string bdName = WeighFormList[i].Key.Substring(1);
                bdName = bdName.Replace(bdName[0], ti.ToUpper(bdName[0]));

                TextBlock tbk = new TextBlock();
                tbk.Text = WeighFormList[i].Value;
                tbk.VerticalAlignment = VerticalAlignment.Center;
                tbk.TextAlignment = TextAlignment.Right;

                tbk.Margin = new Thickness(5);
                dp.Children.Add(tbk);

                if (bdName == "Ch" || bdName == "Wz" || bdName == "Kh")
                {
                    AutoCompleteBox acb = new AutoCompleteBox
                    {
                        FilterMode = AutoCompleteFilterMode.Contains,
                        BorderBrush = System.Windows.Media.Brushes.LightGray
                    };

                    Binding binding = new Binding(bdName)
                    {
                        Mode = BindingMode.TwoWay
                    };
                    acb.SetBinding(AutoCompleteBox.TextProperty, binding);

                    var style = new System.Windows.Style(typeof(TextBox));
                    style.Setters.Add(new Setter(TextBox.VerticalContentAlignmentProperty, System.Windows.VerticalAlignment.Center));
                    acb.TextBoxStyle = style;

                    acb.MinimumPopulateDelay = 100;
                    acb.MinimumPrefixLength = 0;
                    acb.GotFocus += (sender, args) =>
                    {
                        if (string.IsNullOrEmpty(acb.Text))
                        {
                            acb.IsDropDownOpen = true;
                        }
                    };

                    acb.Margin = new Thickness(5);

                    //添加到grid对应格子中
                    dp.Children.Add(acb);
                    //acb.MinWidth = 90;
                    if (bdName == "Wz")
                    {
                        DataTemplate dt = new DataTemplate();
                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                        fef.SetValue(Label.ContentProperty, new Binding("Name"));
                        dt.VisualTree = fef;

                        List<GoodsModel> goodsList = SQLDataAccess.LoadActiveGoods();
                        acb.ItemsSource = goodsList;
                        acb.ValueMemberPath = "Name";
                        acb.SelectedItem = goodsList.Find(g => g.Id.Equals(weighingRecord.Wz));
                        acb.ItemTemplate = dt;
                    }
                    if (bdName == "Ch")
                    {
                        DataTemplate dt = new DataTemplate();
                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                        fef.SetValue(Label.ContentProperty, new Binding("PlateNo"));
                        dt.VisualTree = fef;

                        List<CarModel> carList = SQLDataAccess.LoadActiveCar();
                        acb.ItemsSource = carList;
                        acb.ValueMemberPath = "PlateNo";
                        acb.SelectedItem = carList.Find(c => c.AutoNo.Equals(weighingRecord.Ch));
                        acb.ItemTemplate = dt;
                    }
                    if (bdName == "Kh")
                    {
                        DataTemplate dt = new DataTemplate();
                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Label));
                        fef.SetValue(Label.ContentProperty, new Binding("Name"));
                        dt.VisualTree = fef;

                        List<CustomerModel> customerList = SQLDataAccess.LoadActiveCustomer();
                        acb.ItemsSource = customerList;
                        acb.ValueMemberPath = "Name";
                        acb.SelectedItem = customerList.Find(c => c.Id.Equals(weighingRecord.Kh));
                        acb.ItemTemplate = dt;
                    }
                }
                else
                {
                    TextBox tb = new TextBox();
                    tb.SetBinding(TextBox.TextProperty, new Binding(bdName));
                    //添加到grid对应格子中
                    dp.Children.Add(tb);
                    //tb.MinWidth = 90;
                    tb.IsReadOnly = true;
                    tb.Margin = new Thickness(5);

                    if (ConfigurationManager.AppSettings["WeighingControl"].Equals("Hand"))
                    {
                        if (bdName == "Kz" || bdName == "Kl" || bdName == "Bz" || bdName.StartsWith("By"))
                        {
                            tb.IsReadOnly = false;
                        }
                        if (ConfigurationManager.AppSettings["WeighingMode"].Equals("Once") && bdName == "Pz")
                        {
                            tb.IsReadOnly = false;
                        }
                    }
                }

                dpList.Enqueue(dp);
                dp = new DockPanel();
                dp.MinWidth = 150;

            }

            int n;
            if (dpList.Count < 13) n = 3;
            else if (dpList.Count < 21) n = 4;
            else n = 5;

            while (dpList.Count > 0)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                for (int i = 0; i < n; i++)
                {
                    sp.Children.Add(dpList.Dequeue());
                    if (dpList.Count == 0)
                        break;
                }
                WeighFormStackPanel.Children.Add(sp);
            }
        }
    }
}
