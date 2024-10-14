using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace AWSV2.Extensions
{
    public class PassWordExtensions
    {
        public static string GetPassWord(DependencyObject obj)
        {
            return (string)obj.GetValue(PassWordProperty);
        }

        public static void SetPassWord(DependencyObject obj, string value)
        {
            obj.SetValue(PassWordProperty, value);
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassWordProperty =
            DependencyProperty.RegisterAttached("PassWord", typeof(string), typeof(PassWordExtensions), new PropertyMetadata(string.Empty, OnPassWordPropertyChanged));

        static void OnPassWordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passWord = sender as PasswordBox;
            string pw = (string)e.NewValue;
            if (passWord != null && passWord.Password != pw)
            {
                passWord.Password = pw;
            }
        }

    }
    public class PasswordBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += AssociatedObject_PasswordChanged;
        }
        /// <summary>
        /// 读内容与更新内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passWord = sender as PasswordBox;
            string pw = PassWordExtensions.GetPassWord(passWord);
            if (passWord != null && passWord.Password != pw)
            {
                PassWordExtensions.SetPassWord(passWord, passWord.Password);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching(); AssociatedObject.PasswordChanged -= AssociatedObject_PasswordChanged;
        }
    }

}
