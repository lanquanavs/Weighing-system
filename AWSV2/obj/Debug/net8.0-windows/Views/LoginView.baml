   M S B A M L   `   `   `  ����A  =Common, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullE/clr-namespace:Common.Converters;assembly=CommonCommon.Converters  5      F B篮圈AVS1, Version=3.0.0.14, Culture=neutral, PublicKeyToken=null3clr-namespace:AWSV2.ExtensionsAWSV2.Extensions 5      S OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls 5
         AWSV2.Views.LoginView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      AControls,http://metro.mahapps.com/winfx/xaml/controls     5      E	converter/clr-namespace:Common.Converters;assembly=Common     5      C	iconPacks-http://metro.mahapps.com/winfx/xaml/iconpacks     5      0localclr-namespace:AWSV2.Extensions     5      _	 [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes 	 5	      [mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions 	 5
      A
 =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null)s!https://github.com/canton7/Stylet 
 5        � Title$  登录��5      $
��1020��5      $	��500��5       � AllowsTransparency$
 True��5      6  0MaterialDesignExtensions.Controls.MaterialWindow   BorderBackgroundBrush
 ����    FadeContentIfInactive$ False��5       � 
ResizeMode$ NoResize=�5      $��Microsoft YaHei)�5      $��16(�5      $��Medium"�5         MaterialDesignBody#��[  5       � WindowStartupLocation$ CenterScreen=�5       � WindowStyle$
 None=�5      ��5      Z�  � Loaded 5      ! 
 Stylet.Xaml.ActionExtension  *WindowLoaded+ � 	Resources 5      >   8Common.Converters.MultiBooleanToOneBooleanValueConverter  loginBtnIndeterminate	   Default��%(   &	         5   
   $
	 True��6Y   5      .��5      �� $��12q�5   	   
�������
 �� CornerRadius$
 0��5!   	    J� Effect 5"   
   3  -System.Windows.Media.Effects.DropShadowEffect  5#         
BlurRadius$ 18Q�5$         	Direction$	 120Q�5%         Opacity$
 0.42Q�5&         ShadowDepth$ 0Q�5'        CommonWhiteColor   Color# [ 5(      ._�5*   
   � $	��Top=�6    � ColumnDefinitions 5+      �� 5,      $��3* �6#   �� 5-      $
��1.4* �6#   .+�5/      �� $��Center=�6   .+�50      �� $	��260��51      $��UniformToFill=�53      ��52      �� *ActiveCode.ImageSource+53      55      �� 57      $��0,0,0,20q�6   $��Bottom=�6+   .+�58      �� $��8q�59      $��Center=�5:       �� Orientation$ 
Horizontal=�5;      .+�5<      �� $��12(�6    $ ��HS-AW @ 2024 -服务商：��6.   �� 5=      $��12(�6    ��6.   �� *ActiveCode.CompanyName+ �� FallbackValue$ CompanyName��5>      �� 5?      $��4q�5@      $��Center=�5A      $ 
Horizontal=�5B      .+�5C      �� $��12(�6    $��服务热线：��6.   �� 5D      $��12(�6    ��6.   �� *ActiveCode.ServicePhone+$ ServicePhone��5E      5F      �� 5H      $��1��6   .+�5J      � $��8q�6   .+�5K      �� $��Right=�5L        MaterialDesignIconButton#��[ 5N      ��5M        *Exit+.��5O      ' 	 !MaterialDesignThemes.Wpf.PackIcon     Kind$ Close=�6&   5P      5Q      �� 5T      $��	8,35,24,0q�6   .+�5U      �� $��	50,0,0,20q�5V      $��30(�5W      $
��Bold"�5X      $��登录��5Y       �� TextAlignment$ Center=�5Z      �� 5[      $��0,0,0,20q�5\       �� Padding$ 4q�5]      $
 80��5^      ._�5_      �  5`      �� 5a   "   $��80 �63   �� 5b   "   .+�5d      �� $��0,0,12,0q�5e   !   $��Right=�5f   !   $��Center=�5g   !   $��	登录名��5h   !   $��12(�5i   !   �� 5q      $��1��5r   !   $	��4,0q�5s   !   $	 4,0q�5t   !   #��[  5u   !   $��1q�5v   !   ._�5w   "   �� $��Center=�5x   %   ) 	 #MaterialDesignThemes.Wpf.HintAssist   
IsFloating$ False��5y   %   . 	 (MaterialDesignThemes.Wpf.TextFieldAssist   DecorationVisibility$ 	Collapsed=�5z   %      TextFieldCornerRadius$ 8��5{   %   $��0q�5|   %     MaterialDesignTextBox#��[ 5}   %   ��5~   %   �� *UserId+ �� UpdateSourceTrigger$ PropertyChanged=�5      5�      5�      �� 5�      $��0,0,0,20q�5�      $ 4q�5�      $
 0��5�      ._�5�      �  5�      �� 5�   "   $��80 �63   �� 5�   "   .+�5�      �� $��0,0,12,0q�5�   !   $��Right=�5�   !   $��Center=�5�   !   $��密码��5�   !   $��12(�5�   !   �� 5�      $��1��5�   !   $	��4,0q�5�   !   $	 4,0q�5�   !   #��[  5�   !   $��1q�5�   !   ._�5�   "   G� $��Center=�5�   %   $ False��5�   %   $ 	Collapsed=�5�   %   $ 8��5�   %   $��0q�5�   %     MaterialDesignPasswordBox#��[ 5�   %   0 	 *MaterialDesignThemes.Wpf.PasswordBoxAssist   Password 5�   %   �� *
Password+$ PropertyChanged=�5�   %   5�      5�      5�      �� 5�      $ 4q�6   $
 0��6)   ._�5�      �  5�      �� 5�   "   $��80 �63   �� 5�   "   .+�5�      �� $��1��5�   !   $	��4,0q�5�   !   +	 	 %MaterialDesignThemes.Wpf.ButtonAssist 	  CornerRadius$ 0��5�   !   3
 	 -MaterialDesignThemes.Wpf.ButtonProgressAssist 
  IsIndicatorVisible$
 True��5�   !    
  Value$ -1Q�5�   !   ���#00197d$��立即登录��5�   !    � Cursor$
 Hand��5�   !   $��12(�5�   !   
�������  �� 	IsDefault$
  True��5�   !   ��5�   !     *Login+! 
  IsIndeterminate! 5�   "   `� 5�   &   " `� 	Converter#" [ 63   .1�5�   *   �� # �� Path$# Logining�62   �� 5�   *   $# LicenseRunning�62   �� 5�   *   $# PrepareRunning�62   5�   '   5�      5�      5�      �� 5�      $ 4q�6   $
 0��6)   ._�5�      �  5�      �� 5�   "   $��80 �63   �� 5�   "   .+�5�      ]� $��1��6(   .+�5�   "   �� -   5        $ ��Name$$ 
cbStorePwd��5�   %   $��记住密码��5�   %   $
��Left=�5�   %   $��12(�5�   %   % d� 	IsChecked% 5�   %   �� *
StorePwd+�� 5�   "   $��Right=�5�   %   $��自动登录��5�   %   $��Right=�5�   %   $��12(�5�   %   % 5�   %   �� *	AutoLogin+{�5   %   �� & �� ElementName$& 
cbStorePwd��$# 	IsChecked�5     5     5     �� 5     $	��0,4q�5      	 AJRed#��[ 5	     $ Center=�5     ��5
     �� *
TextInfo+$ error��5     �� 5     $��Right=�6!   $ 
Horizontal=�6=   .+�5     �� $	��8,0q�5     $��修改密码��5     $��12(�5     #��[  5       Button NoneStyle#��[ 5     ��5       *OpenChangePassword+5     �� 5     $��数据库设置��5     $��12(�5     #��[  5     #��[ 5     ��5       *OpenDataSet+5     5     5     5     �� 5      $��1��5!     $��20q�5"     $��Right=�5#     $��Bottom=�5$     $��Bottom=�5%     $ 
Horizontal=�5&      5'     ' �� 
TargetType( �� Property  AJTabItemForeground) �� Value��%v   '��       7  �� 5(     "' ��6   .�5)     �� ( � ��8)   67   �� 5*     ( � ��$) 12(�65   5+     5,     .+�5-     �� ��6   �� *
ActiveInfo+�� 5.     * J� RenderTransformOrigin$* -1.185,0.692&�6   $��HS-AW：��6A   �� 5/     ��6   �� *VersionName+50     51     54     55     