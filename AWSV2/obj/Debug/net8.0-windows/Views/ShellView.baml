   M S B A M L   `   `   `  ����A  =Common, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullE/clr-namespace:Common.Converters;assembly=CommonCommon.Converters  5      F B篮圈AVS1, Version=3.0.0.14, Culture=neutral, PublicKeyToken=null1clr-namespace:AWSV2.ConverterAWSV2.Converter 5      )clr-namespace:AWSV2.ViewsAWSV2.Views 5      S OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls 5	      _4clr-namespace:AWSV2.ViewModels.ShellViewHomeTemplate&AWSV2.ViewModels.ShellViewHomeTemplate 5         AWSV2.Views.ShellView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      LcommonConverters/clr-namespace:Common.Converters;assembly=Common     5      3	converterclr-namespace:AWSV2.Converter     5      +localclr-namespace:AWSV2.Views     5      _	 [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes 	 5      [mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions 	 5	      A
 =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null)s!https://github.com/canton7/Stylet 
 5
      @vmTpl4clr-namespace:AWSV2.ViewModels.ShellViewHomeTemplate 
 5        � Title$  
篮圈AVS1��5      $
��1366��5      $	��768��5         MaterialDesignPaper#��[  5      $��0q�5       � Icon$ /Resources/Img/32.ico��5      $��Microsoft YaHei)�5      $��12(�5      $��Medium"�5        MaterialDesignBody#���  5       � WindowStartupLocation$ CenterScreen=�5       � WindowStyle$
 None=�5       � StateChanged 5      ! 
 Stylet.Xaml.ActionExtension  *MainWindowStateChanged+ � 	Resources 5       �� MergedDictionaries �� Source+   %Common.Converters.BooleanMapConverter  activeBtnVisibleCvt  System.Windows.Visibility   Visible	   
FalseValue
   	Collapsed   	TrueValue3  -AWSV2.Converter.HomeTemplateTypeMenuConverter ! homeTemplateTypeMenuConverter:  4AWSV2.Converter.WeighingControlFormatStringConverter ( $weighingControlFormatStringConverter=  7AWSV2.ViewModels.ShellViewHomeTemplate.HomeTemplateType  DefaultEnumTemplate  MonitorEnumTemplateG  AAWSV2.ViewModels.ShellViewHomeTemplate.HomeLayoutTemplateSelector  homeTemplateSelector  HomeDefaultDataTemplate   Default 	 HomeMonitorDataTemplate   Monitor�� 5   
    5      �� 5      $N Hpack://application:,,,/ViewModels/ShellViewHomeTemplate/DefaultView.xaml?�6%   �� 5      $N Hpack://application:,,,/ViewModels/ShellViewHomeTemplate/MonitorView.xaml?�6%   %�   &	       &	 .     &	 A     &	 T     &	 s     &	 �     7  7 	   5!      #	 Z 5#      # Z
 5$        5%      66     5&      6=    5(      Default=�6J    5)      Monitor=�6J     5+      8   8  5.      50      .��53      � -   5        $��	Grid_Main��53       � Loaded 6     *MainGridLoadedCmd+ � RowDefinitions 54   
   �� 55      $��64 �6   �� 56      $��* �6   .+�59   
   ]� $��
12,12,12,0q�6   .+�5;      �� $��0,0,8,0q�5<      
������ 
������ $��0q�5?      $
��Left=�5@       
 MaterialDesignIconButton#��[
 5A      .��5B      �� $��0��5C      $��8q�5D      $��Center=�5E      $��Center=�5F      $��/Resources/Img/logo.png��5G      5H      �� 5I      $��28��5J      
�������$	��0.5q�5L      $
��Left=�5M      �� 5N      $��Center=�5O      $
��Left=�5P       �� Orientation$ 
Horizontal=�5Q      .+�5R      o� ) 	 #MaterialDesignThemes.Wpf.MenuAssist   TopLevelMenuItemHeight$ 32Q�6   .@�5S      m� $
��12,0q�5T       m� CommandParameter$
 User��5W      $��16(�5X      #��[ 5Y      $��过磅记录��5Z      ��5U      Z�  m� Command 5V        *ShowWeighingRecordWindow+5Z      5[      o� 5]      $ 32Q�6   .@�5^      m� $
��12,0q�5_      $
 User��5a      $��16(�5b      #��[ 5c      $��数据录入��5d       5`        *ShowDataManagementWindow+{�5e      �� *EnableDataManage+5f      o� 5h      $ 32Q�6   .@�5i      m� $
��12,0q�5j      $
 User��5l      $��16(�5m      #��[ 5n      $��计划下单��5o       5k        *OpenWeighPlanWindow+{�5p      �� *EnableDataManage+5q      o� 5s      $ 32Q�6   .@�5t      m� $
��12,0q�5u      $
 User��5w      $��16(�5x      #��[ 5y      $��系统设置��5z       5v        *ShowSettingWindow+{�5{      �� *EnableSetting+5|      5�      �� 5�      $��Right=�5�      $��Right=�5�      $ 
Horizontal=�5�      .+�5�      �� $ 
Horizontal=�6   .+�5�      �� $��Center=�5�      $ 
Horizontal=�5�      y�5�      �� *AppLicenseInfo.OnTrial+  boolToVisibleConverter �� 	Converter# [ .+�5�      �� $��试用期剩余：��6$   �� 5�      $	��4,0q�6$   ��61   �� *AppLicenseInfo.LeftDays+5�      �� 5�      $��32��5�      $��8,0,0,0q�5�      ���#00197d$��立即激活��5�      $��14(�5�      
���������5�        *ShowActiveDialog+y�5�      �� *AppLicenseInfo.IsActive+# [ o� 5�      $	��4,0q�6   .@�5�      �� .+�5�      '	 	 !MaterialDesignThemes.Wpf.PackIcon	  $��20��5�   !   $��20��5�   !   $��0,0,0,4q�5�   !   $��Center=�5�   !    	  Kind$ HelpCircleOutline=�5�   !   �� 5�      $��Center=�5�   !   $��12(�5�   !   $��新手引导��5�   !   5�      5�      o� 5�      $	��4,0q�6   .@�5�      m� #��[ 6#   {�6T   �� *EnableSetting+��5�      �� 5�   "   .+�5�   &   	  $��20��5�   )   $��20��5�   )   $��0,0,0,4q�5�   )   $��Center=�5�   )   $ FormatListBulleted=�5�   )   �� 5�   &   $��Center=�5�   )   $��12(�5�   )   $��更多��5�   )   5�   #   .@�5�      m� $��测试��6'   .@�5�   "   m� $
��12,0q�5�   %   $��16(�5�   %   #��[ 5�   %   $��读身份证��5�   %    5�   %     *TestIDCardRead+5�   %   m� 5�   "   $
��12,0q�5�   %     	TrueValue# [ 5�   %   $��16(�5�   %   #��[ 5�   %   $��开闸��5�   %    5�   %     *TestGateOpen+5�   %   m� 5�   "   $
��12,0q�5�   %     
FalseValue# [ 5�   %   #��[ 5�   %   $��关闸��5�   %    5�   %     *TestGateOpen+5�   %   m� 5�   "   $
��12,0q�5�   %   $��16(�5�   %   #��[ 5�   %   $��海康抓图��5�   %    5�   %     *TestHIKCaptureImage+5�   %   m� 5�   "   $
��12,0q�5�   %   $��16(�5�   %   #��[ 5�   %   $ ��测试TTS(第二次称重)��5   %    5�   %     *	TestTTS+5   %   5     m� 5     $��调试工具��6'   .@�5	  "   m� $$ CarmeraTools\LPRConfigTool.exe��5  %   $��臻识��5  %    5
  %     *StartProcess+5  %   m� 5  "   $4 .ControllerTools\串口调试助手 v4.3.29.exe��5  %   $��串口��5  %    5  %     *StartProcess+5  %   5     m� 5     $
 User��5  !   $��关于��5  !    5  !     *ShowAboutAWS+5  !   5     5     �� 5     $��4��5       PrimaryHueDarkBrush#��[ 5      J� Opacity$ .6Q�5     o� 5�     $	��4,0q�6   $��Center=�6(   .@�5�     m� ��5�     �� 5�  "   .+�5�  &   	  $��24��5�  )   $��24��5�  )   $��0,0,0,0q�5�  )   $��Center=�5�  )   $ AccountOutline=�5�  )   �� 5�  &   $��Center=�5�  )   $��12(�5�  )   ��5�  )   �� *CurrentUserName+ �� FallbackValue$ --��5�  #   .@�5�     m� $��修改密码��6U    6'     *OpenChangePasswordDialog+6U   m� 5�     $��退出登录��6K    6'     *WindowCloseCmd+6K   5�     5�     5�     �� 5�     $��1��5�     $	��8,0q�5�     $ 
Horizontal=�5�     5�     5�     5�     )
 	 #MaterialDesignThemes.Wpf.DialogHost
  5�  
   $��1��5�     $
}�True��5�      
  DialogMargin$ 0q�5�      
  
Identifier$
 root��5�     .��5�     �� $��12q�5�     #��[ 5�     ��5�     ��  �� ElementName$ 	Grid_Main�� �� Path$ DataContext�5�     5�     5�      J� InputBindings 5�     �� 5�  
   
 �� Key$ F1��5�      �� CommandParameter$	 up1��5�      �� Gesture$ F1��5�       �� Command  5�       *HandToBarrier+5�     �� 5�  
   $ F2��5�     $	 up2��5�     $ F2��5�       5�       *HandToBarrier+5�     �� 5�  
   $ F3��5�     $ upall��5�     $ F3��5�       5�       *HandToBarrier+5�     �� 5�  
   $ F4��5�     $ downall��5�     $ F4��5        5�       *HandToBarrier+5      �� 5  
   $ F5��5     # [ 5     $ F5��5       5       *RefreshWeighForm+5     �� 5  
   $	 F11��5     $	 f11��5
     $	 F11��5       5	       *FastWeigthCorrect+5     �� 5  
   $	 F12��5     $	 f12��5     $	 F12��5       5       *FastWeigthCorrect+5     5     