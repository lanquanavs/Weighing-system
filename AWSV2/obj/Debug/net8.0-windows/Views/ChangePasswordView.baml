   M S B A M L   `   `   `  ����F  B篮圈AVS1, Version=3.0.0.14, Culture=neutral, PublicKeyToken=null1clr-namespace:AWSV2.ConverterAWSV2.Converter  5      3clr-namespace:AWSV2.ExtensionsAWSV2.Extensions  5      S OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls 5
      $    AWSV2.Views.ChangePasswordView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      AControls,http://metro.mahapps.com/winfx/xaml/controls     5      3	converterclr-namespace:AWSV2.Converter     5      C	iconPacks-http://metro.mahapps.com/winfx/xaml/iconpacks     5      0localclr-namespace:AWSV2.Extensions     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  5	      [mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions  5
      A	 =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null)s!https://github.com/canton7/Stylet 	 5      $	��480��5      $	��600��5        � AllowsTransparency$
  True��5      6  0MaterialDesignExtensions.Controls.MaterialWindow   BorderBackgroundBrush
 ����    FadeContentIfInactive$ False��5       � 
ResizeMode$ NoResize=�5      $��Microsoft YaHei)�5      $��16(�5      $��Medium"�5         MaterialDesignBody#��[  5       � WindowStartupLocation$ CenterScreen=�5       � WindowStyle$
 None=�5      ��5      Z� .��5      �� $��12q�5   	     MaterialDesignPaper#��[ 5   	    �� CornerRadius$ 0��5   	    J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5         
BlurRadius$ 18Q�5       	   	Direction$		 120Q�5!      
   Opacity$

 0.42Q�5"         ShadowDepth$ 0Q�5#        CommonWhiteColor   Color# [ 5$      ._�5&   
   �� .+�5'      � $��8q�6   .+�5(      �� $
��Left=�5)      #��[  5+        MaterialDesignIconButton#��[ 5,      ��5*      ! 	 Stylet.Xaml.ActionExtension  *Exit+.��5-      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ ChevronLeft=�6"   5.      �� 5/      $��Center=�50      $��Center=�51      $��24(�52      $
��Bold"�53      $��修改密码��54      56      �� 57      $��Center=�58      $3��-请记住修改后的密码避免登录失败��59      $��12(�5:      $
��Bold"�5;      �� 5=      $��48,24q�6   .+�5>      �� $��0,0,0,20q�5?       �� Padding$ 8q�5@        TextBox#��[ 5A      $ 0��5B      y�5C      �� *	FromLogin+  boolToVisibleConverter �� 	Converter# [ ._�5D      ]� .+�5E        $��22��5F      $��22��5G      $
��12,0q�5H      $��Center=�5I      �� 5J      #��[ 6!   $��1,0,0,0q�6T   �� 5K      $
��12,0q�5L      $��Center=�5M        PrimaryHueMidBrush)  #MaterialDesignThemes.Wpf.HintAssist   
Foreground# [ 5N         Hint$ 请输入账号��5O         
IsFloating$ False��5P      .  (MaterialDesignThemes.Wpf.TextFieldAssist   DecorationVisibility$ 	Collapsed=�5Q      $��0q�5R        MaterialDesignTextBox#��[ 5S      ��5T      �� *UserId+ �� UpdateSourceTrigger$ PropertyChanged=�5U      5W      �� 5X      $��0,0,0,20q�5Y      $ 8q�5Z      #��[ 5[      $ 0��5\      ._�5]      ]� .+�5^        $��22��5_      $��22��5`      $
��12,0q�5a      $��Center=�5b      $
 Lock=�5c      �� 5d      #��[ 6!   $��1,0,0,0q�6T   G� 5e      $
��12,0q�5f      $��Center=�5g      # [ 5h      $ 请输入旧密码��5i      $ False��5j      $ 	Collapsed=�5l      $��0q�5m       # MaterialDesignRevealPasswordBox#��[ 5n      0  *MaterialDesignThemes.Wpf.PasswordBoxAssist   Password 5k      �� *OldPwd+$ PropertyChanged=�5n      5o      5q      �� 5s      $��0,0,0,20q�5t      $ 8q�5u      #��[ 5v      $ 0��5w      ._�5x      ]� .+�5y        $��22��5z      $��22��5{      $
��12,0q�5|      $��Center=�5}      $
 Lock=�5~      �� 5      #��[ 6!   $��1,0,0,0q�6T   G� 5�      $
��12,0q�5�      $��Center=�5�      $ 请输入新密码��5�      $ False��5�      $ 	Collapsed=�5�      $��0q�5�      #��[ 5�       5�      �� *NewPwd+$ PropertyChanged=�5�      5�      5�      �� 5�      $��0,0,0,20q�5�      $ 8q�5�      #��[ 5�      $ 0��5�      ._�5�      ]� .+�5�        $��22��5�      $��22��5�      $
��12,0q�5�      $��Center=�5�      $
 Lock=�5�      �� 5�      #��[ 6!   $��1,0,0,0q�6T   G� 5�      $
��12,0q�5�      $��Center=�5�      # [ 5�      $ 确认新密码��5�      $ False��5�      $ 	Collapsed=�5�      $��0q�5�      #��[ 5�       5�      �� *
ReNewPwd+$ PropertyChanged=�5�      5�      5�      �� 5�      $��8q�5�       		 AJRed#��[	 5�       �� TextAlignment$ Center=�5�      ��5�      �� *	ErrorInfo+5�      �� 5�      $��0,36,0,0q�5�      3  -MaterialDesignThemes.Wpf.ButtonProgressAssist   IsIndicatorVisible$
 True��5�         Value$ -1Q�5�       
 PrimaryHueDarkBrush#��[
 5�      $��确认修改��5�       � Cursor$
 Hand��5�       �� 	IsDefault$
 True��5�         IsIndeterminate 5�      �� *	Loading+��5�        *	Confirm+5�      5�      �� 5�      $
��24,0q�5�      $��Right=�5�       �� Orientation$ 
Horizontal=�5�      .+�5�      �� ��6   �� *
ActiveInfo+�� 5�      $��版本号：��6   �� 5�      ��6   �� *VersionName+5�      5�      5�      5     