   M S B A M L   `   `   `  ����A  =Common, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullE/clr-namespace:Common.Converters;assembly=CommonCommon.Converters  5      E/clr-namespace:Common.AJControls;assembly=CommonCommon.AJControls  5      F B篮圈AVS1, Version=3.0.0.14, Culture=neutral, PublicKeyToken=null)clr-namespace:AWSV2.ViewsAWSV2.Views 5      S OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls 5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      &    AWSV2.Views.ActiveCodeDialogView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R	 NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml   	  5      NajCommonConverters/clr-namespace:Common.Converters;assembly=Common   	  5      CajCtrls/clr-namespace:Common.AJControls;assembly=Common   	  5      @d2http://schemas.microsoft.com/expression/blend/2008   	  5      +localclr-namespace:AWSV2.Views   	  5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006   	  5	      _
 [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes 
 5
      [mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions 
 5      A =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null)s!https://github.com/canton7/Stylet  5      3system&clr-namespace:System;assembly=mscorlib  5      $	��480��5      $	��360��5        � AllowsTransparency$
  True��5      6  0MaterialDesignExtensions.Controls.MaterialWindow   BorderBackgroundBrush
 ����    FadeContentIfInactive$ False��5       � 
ResizeMode$ NoResize=�5      $��Microsoft YaHei)�5      $��16(�5      $��Medium"�5         MaterialDesignBody#���   5       � WindowStartupLocation$ CenterScreen=�5       � WindowStyle$
 None=�5      ��5      Z�  � 	Resources 5      +   %Common.Converters.BooleanMapConverter " activeCodeTextboxHineConverter   
FalseValue   	NullValue	   	TrueValue # activeCodeTextboxColorConverter 	 AJRed 
 AJGray  AJGreen��%�   &	       &	 �     7  7  7    5   
   $$ 此密钥无效请重新输入��5       $ 请输入您的密钥��5!      $	 密钥验证正确��5"        5#   
   8   8  8	  5'      5(      .��5)      �� -   5        $
��user��5*   	   $��12q�5+   	   
 �� Padding$

 12,0q�5,   	     MaterialDesignPaper#���  5-   	    �� CornerRadius$ 0��5.   	    J� Effect 5/   
   3  -System.Windows.Media.Effects.DropShadowEffect  50         
BlurRadius$ 18Q�51         	Direction$	 120Q�52         Opacity$
 0.42Q�53         ShadowDepth$ 0Q�54        
ThemeColor   Color# [ 55      ._�57   
   � .+�59      ]� .+�5;      ]� $	��0,8q�6   $	��Top=�6)   .+�5<      ��   
AJRedLight#��[ 5=      $
��Left=�5>       	 MaterialDesignIconButton#��[	 5?      .��5@      ' 
 !MaterialDesignThemes.Wpf.PackIcon  #��[ 6&      Kind$
 Lock=�6J   5A      �� 5B      $��42��5C      $��42��5D      $��Right=�5E      $	��Top=�5F      
������ $��Right=�5I      #��[ 5J      #��[	 5K      ��5H      !  Stylet.Xaml.ActionExtension  *CancelActive+.��5L        $ Close=�6&   5M      5N      � 5P      $��0,0,0,8q�6   $	��Top=�6(    � ColumnDefinitions 5Q      �� 5R      $��3* �6+   �� 5S      $��1* �6+   .+�5U      �� $��Bottom=�6!   .+�5V      �� $��请输入产品密钥��6    $
��Bold"�62   �� 5W      $9��3产品密钥从经销商获得，当产品机器码��6    $��12(�6<   5X      �� 5Y      $��1��5Z      $��70��5[      $��70��5\      $��Bottom=�5]      ��5^      �� *QRCode+5_      ]� 5a      $	��Top=�6   .+�5b      �� $��Center=�5c      $
��Left=�5d      ��5e      �� *
HardDiskNo+ �� FallbackValue$	 123���� 5f      $	��8,0q�5g      $��Right=�5h      $��复制��5j      $��Right=�5k      #��[  5l       
 Button NoneStyle#��[
 5m      ��5i        *CopyHardDiskNo+5m      5n      �� 5p      $	��0,8q�5q      $��1q�5s      $ 0��5t      $	��Top=�5u      ��5r      �� *ActiveCodeCorrect+ �� 	Converter# [ # [ ._�5v      �� $��32��5w      $��64��5x      ) 
 #MaterialDesignThemes.Wpf.HintAssist   
IsFloating$ False��5z       �� AcceptsReturn$
 True��5{      $��0q�5|       �� TextWrapping$
 Wrap=�5~      " �� VerticalScrollBarVisibility$ Visible=�5         Hint 5y      �� *ActiveCodeCorrect+# [ $ 	请输入����5}      �� *
ActiveCode+ �� UpdateSourceTrigger$ PropertyChanged=�5      5�      �� 5�      $��Right=�5�      $��Bottom=�5�       �� Orientation$ 
Horizontal=�5�      .+�5�      �� $��0,0,12,0q�5�      
������ #��[  5�      #��[  5�      ��5�        *CancelActive+{�5�      �� *ActiveLoading+  boolReverseConverter# [ .��6n   取消5�      �� 5�      $��1��5�      3 
 -MaterialDesignThemes.Wpf.ButtonProgressAssist   IsIndicatorVisible$
 True��5�         Value$ -1Q�5�      $��	下一步��5�       �� 	IsDefault$
 True��5�          IsIndeterminate  5�      �� *ActiveLoading+��5�        *ConfirmActive+5�      5�      5�         Common.AJControls.AJToast  5�      $��0,12,0,0q�5�      !   Message$! 复制成功!��5�      "   Type$" Success=�5�      #   Visible# 5�      �� *HardDiskNoCopySuccess+$ �� Mode$$ TwoWay=�5�      5�      5�      