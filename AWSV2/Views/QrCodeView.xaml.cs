using AWSV2.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThoughtWorks.QRCode.Codec;

namespace AWSV2.Views
{
    /// <summary>
    /// RegView.xaml 的交互逻辑
    /// </summary>
    public partial class QrCodeView
    {
        public QrCodeView()
        {
            InitializeComponent();
            try
            {
                CreateCode(Register.GetHdInfo());
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="version">版本 1 ~ 40</param>
        /// <param name="pixel">像素点大小</param>
        /// <param name="icon_path">图标路径</param>
        /// <param name="icon_size">图标尺寸</param>
        /// <param name="icon_border">图标边框厚度</param>
        /// <param name="white_edge">二维码白边</param>
        /// <returns>位图</returns>
        public void CreateCode(string content)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 8;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

            System.Drawing.Bitmap bmp = qrCodeEncoder.Encode(content, Encoding.UTF8);
            MemoryStream stream = new MemoryStream();

            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            System.Windows.Media.ImageBrush imgBrush = new System.Windows.Media.ImageBrush();
            System.Windows.Media.ImageSourceConverter isConverter = new System.Windows.Media.ImageSourceConverter();
            imgBrush.ImageSource = (System.Windows.Media.ImageSource)isConverter.ConvertFrom(stream);

            this.img1.Source = imgBrush.ImageSource;
        }



    }
}
