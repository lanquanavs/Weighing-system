using Common.HardwareSDKS.Models;
using Common.Utility.AJ;
using MaterialDesignThemes.Wpf;
using Stylet.Xaml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AWSV2.ViewModels.ShellViewHomeTemplate
{
    /// <summary>
    /// MonitorGrid.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorGrid : UserControl
    {

        public class DeviceMessageToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                {
                    return Visibility.Collapsed;
                }

                var paramStr = parameter.ToString();

                if (paramStr == "visible")
                {
                    return string.IsNullOrEmpty(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;
                }

                var condition = value.ToString().Contains("加载中");

                if (paramStr == "loading")
                {
                    return condition ? Visibility.Visible : Visibility.Collapsed;
                }
                return !condition ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return null;
            }
        }

        private IList<DeviceInfo> _prevDevices;

        public static DependencyProperty MonitorsProperty =
            DependencyProperty.Register(nameof(Monitors), typeof(IEnumerable<DeviceInfo>), typeof(MonitorGrid), new PropertyMetadata(MonitorsPropertyChanged));


        public IEnumerable<DeviceInfo> Monitors
        {
            get { return (IEnumerable<DeviceInfo>)GetValue(MonitorsProperty); }
            set { SetValue(MonitorsProperty, value); }
        }

        private static void MonitorsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is MonitorGrid dataGrid)
            {
                dataGrid.MonitorsPropertyChanged(e);
            }
        }

        private void MonitorsPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_prevDevices?.Count() > 0)
            {
                foreach (var monitor in _prevDevices)
                {
                    try
                    {
                        monitor.Close();
                    }
                    catch
                    {
                    }
                }
                MainGrid.Children.Clear();
                _prevDevices.Clear();
            }

            var matchedSource = Monitors?.Where(p => p.Enable).ToList() ?? new List<DeviceInfo>();

            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();

            // <= 2 显示两个, > 2 四个, > 4 六个

            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            if (matchedSource.Count <= 2)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }
            else
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());
                MainGrid.RowDefinitions.Add(new RowDefinition());
            }

            if (matchedSource.Count > 4)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            if (matchedSource.Count > 0)
            {
                if (_prevDevices == null)
                {
                    _prevDevices = new List<DeviceInfo>();
                }
                var columnIndex = 0;
                var rowIndex = 0;
                var columnCount = MainGrid.ColumnDefinitions.Count;

                foreach (var monitor in matchedSource)
                {
                    var infoStackPanel = CreateContainer();
                    var drawer = new MaterialDesignThemes.Wpf.DrawerHost()
                    {
                        Margin = new Thickness(8),
                        DataContext = monitor,
                        BottomDrawerContent = CreateDrawerBottomContent(monitor)
                    };
                    drawer.MouseEnter += (s, e) =>
                    {
                        e.Handled = true;
                        ((DrawerHost)s).IsBottomDrawerOpen = true;
                    };
                    drawer.MouseLeave += (s, e) =>
                    {
                        e.Handled = true;
                        ((DrawerHost)s).IsBottomDrawerOpen = false;
                    };
                    var grid = new Grid
                    {
                        DataContext = monitor
                    };

                    //border.ContextMenu.Items.Add(new MenuItem
                    //{
                    //    Header = "重新加载",
                    //});

                    //var img = new Image
                    //{
                    //    Stretch = Stretch.Fill, 
                    //};

                    var videoBorder = new Border
                    {
                        Background = Application.Current.FindResource("MaterialDesignPaper") as SolidColorBrush,
                        CornerRadius = new CornerRadius(0)
                    };

                    //videoBorder.Child = img;
                    grid.Children.Add(videoBorder);
                    grid.Children.Add(infoStackPanel);

                    if (monitor.Type == DeviceType.臻识)
                    {
                        //var buttonsGroup = new StackPanel
                        //{
                        //    Margin = new Thickness(4),
                        //    HorizontalAlignment = HorizontalAlignment.Right,
                        //    Orientation = Orientation.Horizontal
                        //};

                        //buttonsGroup.Children.Add(CreateButton("开闸", new int?(0), monitor));

                        //var closeBtn = CreateButton("关闸", new int?(1), monitor);
                        //closeBtn.Margin = new Thickness(8, 0, 8, 0);
                        //buttonsGroup.Children.Add(closeBtn);
                        //buttonsGroup.Children.Add(CreateButton("常开", new int?(), monitor));

                        //stackPanel.Children.Add(buttonsGroup);
                    }

                    Grid.SetRow(drawer, rowIndex);
                    Grid.SetColumn(drawer, columnIndex);

                    if (columnIndex > 0 && columnIndex % (columnCount - 1) == 0)
                    {
                        columnIndex = 0;
                        rowIndex++;
                    }
                    else
                    {
                        columnIndex++;
                    }
                    drawer.Content = grid;
                    MainGrid.Children.Add(drawer);

                    monitor.DeviceControlLoadCmd(videoBorder);

                    _prevDevices.Add(monitor);
                }
            }

        }

        public MonitorGrid()
        {
            InitializeComponent();
        }

        private Border CreateContainer()
        {
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Application.Current.FindResource("MaterialDesignPaper") as SolidColorBrush,
                Padding = new Thickness(8),
                CornerRadius = new CornerRadius(0)
            };

            var stackPanel = new StackPanel
            {

            };

            border.SetBinding(VisibilityProperty, new Binding(nameof(DeviceInfo.Message))
            {
                Converter = new DeviceMessageToVisibilityConverter(),
                ConverterParameter = "visible"
            });

            //// 加载进度条
            //// 不能加这个进度条了, 否则窗口最小化导致内存溢出 --阿吉 2024年5月23日06点48分
            //var progressBar = new ProgressBar
            //{
            //    Height = 20,
            //    Width = 20,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    IsIndeterminate = true,
            //    Value = 0,
            //    Margin = new Thickness(4),
            //    Style = Application.Current.FindResource("MaterialDesignCircularProgressBar") as Style
            //};

            //ElevationAssist.SetElevation(progressBar, Elevation.Dp2);
            //progressBar.SetBinding(VisibilityProperty, new Binding(nameof(DeviceInfo.Message))
            //{

            //    Converter = new DeviceMessageToVisibilityConverter(),
            //    ConverterParameter = "loading"
            //});

            //stackPanel.Children.Add(progressBar);

            //错误图标
            var icon = new PackIcon
            {
                Margin = new Thickness(4),
                Kind = PackIconKind.CctvOff,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            icon.SetBinding(VisibilityProperty, new Binding(nameof(DeviceInfo.Message))
            {

                Converter = new DeviceMessageToVisibilityConverter(),
                ConverterParameter = "error"
            });
            stackPanel.Children.Add(icon);

            var textBlock = new TextBlock
            {
                Margin = new Thickness(8),
                TextAlignment = TextAlignment.Center,
                Foreground = Application.Current.FindResource("MaterialDesignBody") as SolidColorBrush
            };
            textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(DeviceInfo.Message)));

            stackPanel.Children.Add(textBlock);
            border.Child = stackPanel;
            return border;
        }

        private UIElement CreateDrawerBottomContent(DeviceInfo device)
        {
            var stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            if (!device.IsCarIdentification)
            {
                return stackPanel;
            }

            var openBtn = new Button
            {
                Height = 32,
                Content = "开闸",
                DataContext = device,
                IsEnabled = device.Enable,
                Margin = new Thickness(8, 4, 8, 4),
            };

            var closeBtn = new Button
            {
                Height = 32,
                Content = "关闸",
                DataContext = device,
                IsEnabled = device.Enable,
                Margin = new Thickness(8,4,8,4),
            };

            var triggerBtn = new Button
            {
                Height = 32,
                Content = "强制识别车牌",
                DataContext = device,
                IsEnabled = device.Enable,
                Margin = new Thickness(8, 4, 8, 4),
            };

            openBtn.Click += async (s, e) =>
            {
                var btnInnser = (Button)s;

                await (btnInnser.DataContext as DeviceInfo).ToggleGateSwitchCmdAsync(true);
            };

            closeBtn.Click += async (s, e) =>
            {
                var btnInnser = (Button)s;

                await (btnInnser.DataContext as DeviceInfo).ToggleGateSwitchCmdAsync(false);
            };

            triggerBtn.Click += async (s, _) =>
            {
                var btnInnser = (Button)s;

               await (btnInnser.DataContext as DeviceInfo).RaiseCarPlatResultCmdAsync();
            };

            stackPanel.Children.Add(openBtn);
            stackPanel.Children.Add(closeBtn);
            stackPanel.Children.Add(triggerBtn);

            return stackPanel;
        }

        private void MainGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Monitors?.Count() > 0)
            {
                foreach (var monitor in Monitors)
                {
                    try
                    {
                        monitor.Close();
                    }
                    catch
                    {
                    }
                }
                MainGrid.Children.Clear();
            }
        }
    }
}
