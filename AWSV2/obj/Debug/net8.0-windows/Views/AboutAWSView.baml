   M S B A M L   `   `   `  ����A  =Common, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullE/clr-namespace:Common.AJControls;assembly=CommonCommon.AJControls  5      E/clr-namespace:Common.Converters;assembly=CommonCommon.Converters  5      S OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls 5
       
篮圈AVS1   AWSV2.Views.AboutAWSView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      AControls,http://metro.mahapps.com/winfx/xaml/controls     5      CajCtrls/clr-namespace:Common.AJControls;assembly=Common     5      F
converters/clr-namespace:Common.Converters;assembly=Common     5      C	iconPacks-http://metro.mahapps.com/winfx/xaml/iconpacks     5      _	 [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes 	 5	      [mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions 	 5
      A
 =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null)s!https://github.com/canton7/Stylet 
 5        � Title$  软件信息��5      $	��448��5      $	��400��5       � AllowsTransparency$
 True��5      6  0MaterialDesignExtensions.Controls.MaterialWindow   BorderBackgroundBrush
 ����    FadeContentIfInactive$ False��5       � 
ResizeMode$ NoResize=�5       � SizeToContent$ Height=�5      $��Microsoft YaHei)�5      $��16(�5      $��Medium"�5         MaterialDesignBody#���   5       � WindowStartupLocation$ CenterScreen=�5       � WindowStyle$
 None=�5      ��5      Z�  � 	Resources 5        TextBlockGray	 �� 
TargetType
 �� Property �� Value+   %Common.Converters.BooleanMapConverter  activeSuccessConverter   
FalseValue   	TrueValue  boolToStrConverter��%X  &	       7   &	 �     &	 �     �� 5   
   "	 ��6&   .�5      �� 
 � ��8   6+   �� 5      
 � ��$ 12(�6)   �� 5      
 � ��$ 0,8,0,0q�6'   �� 5       
 � ��$	 100��6)   5!        5"   
   $ 	测试版��5$      $ 	正式版��5%        5&   
   $ 	未开通��5(      $ 	已开通��5)      5*      .��5+      �� $��12q�5,   	    �� Padding$ 12q�5-   	     PrimaryHueDarkBrush#��[ 5.   	    �� CornerRadius$ 0��5/   	    J� Effect 50   
   3  -System.Windows.Media.Effects.DropShadowEffect  51         
BlurRadius$ 18Q�52         	Direction$	 120Q�53         Opacity$
 0.42Q�54         ShadowDepth$ 0Q�55        
ThemeColor   Color# [ 56      ._�58   
   � .+�59      ]� .+�5:      �� $	��Top=�6   .+�5;      ]� $	��0,8q�6    $	��Top=�6-   .+�5<      �� $��42��5=      $��42��5>      $��Right=�5?      $	��Top=�5@      
������ $��Right=�5C      #��[  5D        MaterialDesignIconButton#��[ 5E      ��5B      ! 
 Stylet.Xaml.ActionExtension  *
CloseCmd+.��5F      ' 	 !MaterialDesignThemes.Wpf.PackIcon     Kind$ Close=�6*   5G      5H      �� 5I      $	��200��6!   $��Center=�60   .+�5J      �� $��64��5K      $��64��5L      $��/Resources/Img/logo.png��5M      �� 5N      $	��200��5O      $	��0,8q�5P      3 	 -MaterialDesignThemes.Wpf.ButtonProgressAssist   IsIndicatorVisible$
 True��5R         Value$ -1Q�5S      #��[ 5T       � Cursor$
 Hand��5V      #��[  5W         IsIndeterminate 5Q      �� *	Loading+��5U        *CheckUpdate+.��5W   N   检查更新5Y      �� 5Z      $��Center=�6%    �� Orientation$ 
Horizontal=�6B   .+�5[      �� $
��Bold"�5\   !   #��[  5]   !   $��	版本：��5^   !   �� 5_      $
��Bold"�5`   !   #��[  5a   !   ��5b   !   �� *VersionName+ �� FallbackValue$ --��5c      �� 5d      $��Center=�6%   $ 
Horizontal=�6B   .+�5e      9� $��0,0,0,64q�5f   !   $��Stretch=�5g   !   - 	 'MaterialDesignThemes.Wpf.ExpanderAssist   HeaderFontSize$ 12Q�5h   !   #��[ 5i   !   $��12(�5j   !   #��[  5k   !   $��查看机器码��5l   !   .��5m   "   �� #���   6-   .+�5n   &   ]� .+�5o   *   �� $
��Left=�5p   -   #��[ 5q   -   $��机器码：��5r   -   �� 5s   *   $��4,0,0,0q�5t   -   $��Right=�5v   -   #��[  5w   -     Button NoneStyle#��[ 5x   -   ��5u   -     *CopyHardDiskNo+.��5x   W   复制5z   +   �� 5{   *   $��Right=�5|   -   #��[ 5}   -    �� TextAlignment$ Right=�5   -   ��5~   -   �� *ActiveCode.HardwareCode+$ --��5   -   5�   '   ]� 5�   &   .+�5�   *   �� $
��Left=�5�   -   #��[ 5�   -   $��UI版本框架：��5�   -   �� 5�   *   $��Right=�5�   -   #��[ 5�   -   $&�� MaterialDesignThemes.Wpf 4.9.0.0��5�   -   $ Right=�5�   -   5�   '   ]� 5�   &   .+�5�   *   �� $
��Left=�5�   -   #��[ 5�   -   $��系统状态：��5�   -   �� 5�   *   $��Right=�5�   -   #��[ 5�   -   $ Right=�5�   -   ��5�   -   �� *ActiveSuccess+ �� 	Converter# [ $ 	测试版��5�   -   5�   '   ]� 5�   &   .+�5�   *   �� $
��Left=�5�   -   #��[ 5�   -   $��远程数据查询：��5�   -   �� 5�   *   $��Right=�5�   -   #��[ 5�   -   $ Right=�5�   -   ��5�   -   �� *!ActiveCode.EnableRemoteDataView+# [ $ 	未开通��5�   -   5�   '   ]� 5�   &   .+�5�   *   �� $
��Left=�5�   -   #��[ 5�   -   $��远程查看图：��5�   -   �� 5�   *   $��Right=�5�   -   #��[ 5�   -   $ Right=�5�   -   ��5�   -   �� *$"ActiveCode.EnableRemotePictureView+# [ $ 	未开通��5�   -   5�   '   ]� 5�   &   .+�5�   *   �� $
��Left=�5�   -   #��[ 5�   -   $��预约功能：��5�   -   �� 5�   *   $��Right=�5�   -   #��[ 5�   -   $ Right=�5�   -   ��5�   -   �� *ActiveCode.EnableReserveWeigh+# [ $ 	未开通��5�   -   5�   '   �� 5�   &   ��63   �� *OtherParams+��5�   *   �� 5�   .   .R�5�   2   �� 5�   /   ��5�   *   �� 5�   .   .R�5�   2   ]� .+�5�   6   �� $
��Left=�5�   9   #��[ 5�   9   ��5�   9   �� *label+�� 5�   6   $��Right=�5�   9   #��[ 5�   9   $ Right=�5�   9   ��5�   9   �� *value+5�   9   5�   3   5�   /   5�   '   5�   #   5�      5�      5�      5�      �� 5�      $��Bottom=�6   $��Bottom=�68   .+�5�      ]� .+�5�      �� .+�5�      �� $ 
Horizontal=�6)   .+�5�   "   �� $	��0,8q�5�   %   $��12(�5�   %   $��
服务商@��5�   %   �� 5�   "   $	��0,8q�5�   %   $��12(�5�   %   ��5�   %   �� *ActiveCode.CompanyName+$ --��5�      �� 5�      $��12(�6(   $��服务电话：--��66   5�      �� 5�      $��Bottom=�6%   $��Right=�6@   .+�5�      �� $ Right=�6(   �� 5�   "   $��12(�5�   %     �� TextDecorations$
  None{�5�   %   ! �� Command! 5�   %     *ToUrl+" �� CommandParameter" 5�   %   �� # �� RelativeSource# �� $ �� Mode$
$ Self=�% �� Path$% NavigateUri���5�   %   �� *ActiveCode.CompanyWebsite+$ --��.�5�   <   .�5�   &   �� �6*   �� *ActiveCode.CompanyWebsite+$ --��5�   #   5�      5�      5�      5�      5�         Common.AJControls.AJToast  5�      $��0,12,0,0q�5�      &   Message$& 复制成功!��5�      '   Type$' Success=�5�      (   Visible( 5�      �� *HardDiskNoCopySuccess+) �� Mode$) TwoWay=�5�      5�      5�      