   M S B A M L   `   `   `  ����A  =Common, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullE/clr-namespace:Common.Utility.AJ;assembly=CommonCommon.Utility.AJ  5      [:clr-namespace:Common.Utility.AJ.Converters;assembly=CommonCommon.Utility.AJ.Converters  5      E/clr-namespace:Common.Converters;assembly=CommonCommon.Converters  5      F B篮圈AVS1, Version=3.0.0.14, Culture=neutral, PublicKeyToken=null1clr-namespace:AWSV2.ConverterAWSV2.Converter 5      )clr-namespace:AWSV2.ViewsAWSV2.Views 5	      S OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls 5      /clr-namespace:AWSV2.ServicesAWSV2.Services 5      /clr-namespace:AWSV2.ControlsAWSV2.Controls 5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      3clr-namespace:AWSV2.ViewModelsAWSV2.ViewModels 5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R	 NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_
 [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a_ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aA =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null7   1AWSV2.Converter.WeighingControlToVisibleConverter #  weighingControlVisibleConverter+   %Common.Converters.BooleanMapConverter  labStateColorConverter 	 AJRed    
FalseValue  AJGreen   	TrueValue   signalTopLightColorConverter 
 AJGray & "signalTopLightStrokeColorConverter  AJBlack ) %signalBottomLightStrokeColorConverter #	 signalBottomLightColorConverter 
 railingImgConverter  labStateTextConverter $  manualWeightBtnsVisibleConverter  System.Windows.Visibility   	Collapsed   Visible $  commonWeightBtnsVisibleConverter  carSumFirstHeaderStyle �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� Property �� Value �� 
TargetType  PrimaryHueDarkBrush �� CornerRadius	 �� Padding
 �� RelativeSource �� Path  MaterialDesignBody  carSumCommonHeaderStyle  carSumLastHeaderStyle4   .Common.Utility.AJ.Converters.RowIndexConverter  rowIndexConverter/  )AWSV2.Converter.DeviceNoticeIconConverter  deviceIconConverter  signalLightToggleBtn  MaterialDesignPaper �� AncestorType �� 	Converter  railingToggleBtn �� DataType)  #AWSV2.ViewModels.WeighFormViewModel  AWSV2.Views.WeighFormView  HomeDefaultDataTemplate � RowDefinitions � ColumnDefinitions �� Orientation �� FallbackValue ��Name!  Stylet.Xaml.ActionExtension)	  #MaterialDesignThemes.Wpf.MenuAssist 	  TopLevelMenuItemHeight'
  !MaterialDesignThemes.Wpf.PackIcon 
  Kind  MaterialDesignMenuItem �� BasedOn �� ElementName � FocusVisualStyle+  %Stylet.Xaml.BoolToVisibilityConverter   Instance ��Name  ThisPageYellowButtonStyle � Loaded  ThisPageButtonStyle.  (MaterialDesignThemes.Wpf.ElevationAssist   	Elevation �� ConverterParameter   InverseInstance  ThisPageOrangeButtonStyle � Cursor  ^� InitialShowDelay! � ToolTip" `� 	Converter# d� 	IsChecked$ �� Mode  boolToVisibleConverter% J� RenderTransformOrigin& �� ScaleX&   System.Windows.Controls.DataGrid' �� AlternationCount(   AutoGenerateColumns)   CanUserAddRows*   CanUserDeleteRows+   CanUserReorderColumns,   CanUserResizeColumns-   CanUserResizeRows.   CanUserSortColumns/   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter0   GridLinesVisibility$1   HorizontalScrollBarVisibility2   
IsReadOnly3   RowHeaderWidth4   Columns0  *System.Windows.Controls.DataGridTextColumn,  &System.Windows.Controls.DataGridColumn5   Header6   HeaderStyle1  +System.Windows.Controls.DataGridBoundColumn7   Binding8   ElementStyle4  .System.Windows.Controls.DataGridTemplateColumn9   MinWidth:   CellTemplate;   Width  Button NoneStyle< �� ResizeBehavior= j� 
AlignmentY> �� ImageSource? j� Stretch  	TrueValue* 
 $Microsoft.Xaml.Behaviors.Interaction@   	Behaviors2  ,AWSV2.Services.ListBoxScrollToBottomBehaviorA   SelectionMode3   -Common.Utility.AJ.DataGridSyncronizerBehaviorB   SyncronizeColumnOrderC   SyncronizeScrollBarD   SyncronizeWidthWithE �� SelectedItemF � ContextMenuG m� ClickH m� CommandParameterI m� CommandJ  NameK   ColumnHeaderHeight�� 5      H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml   	  5      6aj/clr-namespace:Common.Utility.AJ;assembly=Common  5      KajConverters:clr-namespace:Common.Utility.AJ.Converters;assembly=Common  5      CcommonConverter/clr-namespace:Common.Converters;assembly=Common  5      +	converterclr-namespace:AWSV2.Converter  5      3i+http://schemas.microsoft.com/xaml/behaviors 
 5      #localclr-namespace:AWSV2.Views  5	      :md1http://materialdesigninxaml.net/winfx/xaml/themes  5
      YmdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions  5      )s!https://github.com/canton7/Stylet  5      #scclr-namespace:AWSV2.Services  5      #stclr-namespace:AWSV2.Controls  5      1system&clr-namespace:System;assembly=mscorlib  5      %vmclr-namespace:AWSV2.ViewModels  5      %>?  &	        &	      7  7  &	 4     7  7  &	 U     7  &	 z     7  &		 �     7  7  &	
 �     &	 6    &	 �    &	 �    &	 �    7  7  &	 8    7  7  &	 q    7  7  &	 �    &	 �    &	 �    7  7  7  7  7 	 7  &	 �    7 
 (�� 
    5�      *, +)&	 A
    7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7   7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7  7     5      62     5      8    8  5   	     5      8    8  5   	     5      8     5   	   Z�   5!      8     5#   	   Z� 5$   	     5&      8    8  5)   	     5+      $&   /Resources/Img/railing-close.png��5-   	   $% /Resources/Img/railing-open.png��5.   	     50      $  信号传输异常��52   	   $ 信号传输正常��53   	     54      #  Z 56   	   # Z 57   	     58      #  Z 5:   	   # Z 5;   	   �� 5>      "  6+   .�5?   
   ��  � �� 5@      �� 5A      "  6"   .R�5B      �� 8��  $ 6,0,0,0��5E      	 5C      �� 
 �� *Self+$- '(md:DataGridAssist.ColumnHeaderPadding)�._�5F      �� $
��Left=�5G      $��Center=�5H      8�� ��5I      �� 5J      5K      5L      5N      5O      �� 5P      "  6,   .�5Q   
   ��  � �� 5R      �� 5S      "  6"   .R�5T      �� 8��  	 6   �� 
 �� *Self+$- '(md:DataGridAssist.ColumnHeaderPadding)�._�5U      �� $
��Left=�5V      $��Center=�5W      8�� ��5X      �� 5Y      5Z      5[      5]      5^      �� 5_      "  6*   .�5`   
   ��  � �� 5a      �� 5b      "  6"   .R�5c      �� 8��  $ 0,6,0,0��5f      	 5d      �� 
 �� *Self+$- '(md:DataGridAssist.ColumnHeaderPadding)�._�5g      �� $
��Left=�5h      $��Center=�5i      8�� ��5j      �� 5k      5l      5m      5o      5p        5q      6%     5r      6*   �� 5s      " d�6)   .�5t   
   ��  �@�Cursor$
 Hand��6#   �� 5u   
    � �� 5v      �� 5w      .R�5x      �� $��30��5y      $��68��5z      8��  8�� $��1q�5}      $ 6��5~      ._�5      �� $��Center=�6%   .+�5�      A� $��20��5�   !   $��20��5�   !   $	��0,4q�5�   !   ��5�   !   �� 
 ��  ToggleButton$ 	IsChecked�8  ��5�   !   �� 
 ��  ToggleButton$ 	IsChecked�8  A� 5�      $��20��5�   !   $��20��5�   !   $	��0,4q�5�   !   $��1��5�   !   ��5�   !   �� 
 ��  ToggleButton$ 	IsChecked�8  ��5�   !   �� 
 ��  ToggleButton$ 	IsChecked�8  5�   !   5�      5�      5�      5�      5�      �� 5�      " d�6%   .�5�   
   ��  �@�Cursor$
 Hand��6#   �� 5�   
    � ��$ 64��6"   �� 5�   
    � ��$	 100��6#   �� 5�   
    � �� 5�      �� 5�      .R�5�      �� #��z��5�      #��z��5�      ��5�      �� 
 ��  ToggleButton$ 	IsChecked�8   5�      5�      5�      �� 5�      "  6   .R�5�   
     5�      �� 5�      .R�5�   
   � $��1��6    5�      �� 5�      $��4* �6    �� 5�      $��12 �6    �� 5�      $��1* �6    .+�5�      �  5�      �� 5�      $��3* �6'   �� 5�      $��2* �6'   .+�5�      �  5�      �� 5�      $	��188 �6(   �� 5�      $��3* �6(   �� 5�      $��1* �6(   .+�5�      �� $	��180��5�      $��4q�5�      8��  $ 8��5�      $	��Top=�5�      ._�5�      �� .+�5�      ]� $��8q�6(   .+�5�   "   �� $
��Left=�6-   $ 
Horizontal=�6C   .+�5�   &   �� $
��Auto��5�   )   8�� $ 18��5�   )   ._�5�   *   �� $	��6,0q�5�   -   $��Center=�5�   -   $��Center=�5�   -   8�� ��5�   -   �� *
WeighName1+$ 地磅名称��5�   -   5�   '   �� 5�   &   $	 aaa��5�   )   $��20��5�   )   $��6��5�   )   $��12,0,2,0q�5�   )   $
��Left=�5�   )   $��Center=�5�   )   $ 4��5�   )   ��5�   )   �� *StateLabConnected+8  8  5�   )   �� 5�   &   $��Center=�5�   )   
���������5�   )   �� *StateLabConnected+$ 信号状态��8  5�   )   5�   #   �� 5�   "   $��Right=�5�   %   $��Bottom=�5�   %   $��Right=�5�   %   $ 
Horizontal=�5�   %   .+�5�   &   �� $��Btn_SwitchWeightForm��5�   )   $y�	Collapsed=�5�   )   ��5�   )     *SwitchWeightForm+5�   )   o� 5�   &   $	��128��6+   $ 32Q�6:   .@�5�   *   m� ��63   �� *WeightFormTemplateSheetNames+��5�   .   �� 5�   2   $��Center=�6=   $ 
Horizontal=�6X   .+�5�   6   �� $��0,0,8,0q�5�   9   $��Center=�5�   9   8�� ��5�   9   �� *!SelectedWeightFormTemplateSheet+
  5�   6   $��22��5�   9   $��22��5�   9   $��Right=�5�   9   $��Center=�5�   9   8�� $ ChevronDown=�5�   9   5�   3   ��5�   .   �� 5�   2   8  " m�6j   .�5�   6   ��  � �� 6O   �� �� 5�   6    �@m�Command 6P   �� $ Btn_SwitchWeightForm��$ Command��� 5�   6    �@m�CommandParameter 6Y   �� 5�   3   5  +   5  '   �� 5  &   $��20��5  )   $��6��5  )   $	��8,0q�5  )   $ 4��5	  )   ��5  )   �� *
IsStable+8 	 8 
 5	  )   5
  #   5     �� 5     $	��0,8q�5  !   $��Center=�5  !   $��Center=�5  !   $ 
Horizontal=�5  !   .+�5  "   �� $��Center=�5  %   $��79(�5  %   8�� ��5  %   �� *
WeightStr1+5  %   �� 5  "   $��Bottom=�5  %   $��32(�5  %   8�� ��5  %   �� *WeighingUnit+5  %   5     5      5!     �� 5#     $��1��5$     $��4q�5%     $��Stretch=�5&     8�� $ 8��5(     $	��Top=�5)      5*     Z� ._�5+     �� $��4q�5,     $��Stretch=�5-     $��Stretch=�5.     .��5/     �� $|�False��52  !   ��50  !   �� *WeighFormVM+ 51  !   Z� 52  !   53     54     �� 56     $��2��57     $	��120��58     $��4q�59     8�� $ 8��5;     $��Bottom=�5<     y�5=     �� *ManualPZOrMZBtnsVisible+8  ._�5>     � .+�5@     � {�6#   �� *EnableWeighing+y�6H   �� *TwiceWeighing+# Z  5A  "   �� 5B  &   �� 5C  &   �� 5D  &   .+�5F  "   �� $
 btn1��5G  %   $��0��5H  %   $��60��5I  %   $
��20,0q�5J  %   8�� $��18(�5N  %   8�� ��5L  %     *FirstWeighing+��5M  %   �� *
Btn1Text+{�5O  %   �� *EnableWeighing+ 5P  %     *FirstWeighingLoadedCmd+5Q  %   �� 5R  "   $
 btn2��5S  %   $��2��5T  %   $��60��5U  %   $
��20,0q�5V  %   8�� $��18(�5Z  %   8�� ��5X  %     *SecondWeighing+��5Y  %   �� *
Btn2Text+{�5[  %   �� *EnableWeighing+ 5\  %     *SecondWeighingLoadedCmd+5]  %   �� 5^  "   $��3��5_  %   $
��20,0q�5`  %   $	 Dp8=�5a  %   $��称重并存皮��5c  %   $��18(�5d  %   8�� ��5b  %     *WeighWithUpdateCar+{�5e  %   �� *EnableWeighing+y�5g  %   �� *WeighingControl+8  $
 Hand��5h     �� 5i     $
 btn0��5j  !   $��60��5k  !   $
��20,0q�5l  !   8�� 8�� ��5n  !     *OnceWeighing+��5o  !   �� *
Btn3Text+{�5p  !   �� *EnableWeighing+ 5q  !     *OnceWeighingLoadedCmd+y�5s  !   �� *TwiceWeighing+# Z 5u     5v     �� 5x     $��2��5y     $	��120��5z     $��4q�5{     8�� $ 8��5}     $��Bottom=�5~     y�5     �� *ManualPZOrMZBtnsVisible+8  ._�5�     � .+�5�     �  5�  "   �� 5�  &   �� 5�  &   .+�5�  "   �� $��0��5�  %   $��60��5�  %   $
��20,0q�5�  %   8�� $��18(�5�  %   8�� ��5�  %     *FirstWeighing+��5�  %   �� *ManualMZText+{�5�  %   �� *EnableWeighing+5�  %   �� 5�  "   $��1��5�  %   $��60��5�  %   $
��20,0q�5�  %   8�� $��18(�5�  %   8�� ��5�  %     *
SetPZCmd+��5�  %   �� *ManualPZText+{�5�  %   �� *EnableWeighing+5�  %   5�     5�     5�     5�     ]� 5�     $��1��6   .+�5�     �� $	��180��5�     $��4q�5�     8�� $ 8��5�     $	��Top=�5�     ._�5�     �  5�     �� 5�  "   $��1* �60   �� 5�  "   $��4* �60   .+�5�     �� $
��12,4q�6)   $ 
Horizontal=�67   .+�5�  "   �� ��6/   �� *WarningIcons+��5�  &   �� 5�  *   .R�5�  .   �� $ 
Horizontal=�69   5�  +   ��5�  &   �� 5�  *   .R�5�  .   
  $��24��5�  1   $��24��5�  1   $��0,0,12,0q�5�  1   $��Center=�5�  1   $
 Hand��5�  1   $	  600��5�  1   ��5�  1   �� *Online+8   5�  1   �� *Icon+! 5�  2   `� 5�  6   8"   .1�5�  :   �� $ Online�6B   �� 5�  :   $	 Key�6B   5�  7   5�  /   5�  +   5�  #   5�     �� 5�     $��1��5�  !   $	��0,4q�5�  !   $��Center=�5�  !   $��Center=�5�  !   $ 
Horizontal=�5�  !   .+�5�  "   d� $
��12,0q�5�  %   $��Bottom=�5�  %   8��! # 5�  %   �� *SignalLightGreenOne+$$ TwoWay=�5�  %   d� 5�  "   $��24,0,8,0q�5�  %   $��Bottom=�5�  %   $��1��5�  %   8��" ��5�  %     *ToggleGateSwitch+# 5�  %   �� *GateSwitchOne+$$ TwoWay=�5�  %   �� 5�  "   $��80��5�  %   $	��300��5�  %   $��Bottom=�5�  %   8��# 8��$ $��4q�5�  %   ._�5�  &   �� $��/Resources/Img/car.png��6,   y�6L   �� *CarImageVisible+8 % 5�  #   d� 5�  "   $��8,0,24,0q�5�  %   $��Bottom=�5�  %   $��2��5�  %   $% 0.5,0.5&�5�  %   8��& ��5�  %     *ToggleGateSwitch+# 5�  %   �� *GateSwitchTwo+$$ TwoWay=�z�5�  &   �� 5�  *   $& -1Q�69   5�  #   d� 5�  "   $
��12,0q�5�  %   $��Bottom=�5�  %   8��' # 5�  %   �� *SignalLightGreenTwo+$$ TwoWay=�5�  %   5�     5�     5�     � 5      5     �� 5     $
��2.6* �6,   �� 5     $��12 �6,   �� 5     $
��1.6* �6,   .+�5     �� $��4q�5	     8��( $ 8��5     ._�5       $' 2��5  !   $( False��5  !   8��) $) False��5  !   $* False��5  !   $
+ True��5  !   $
, True��5  !   $
- True��5  !   $
. True��5  !   $
/ Auto 5  !   $��18(�5  !   8��* $	0 All=�5  !   $
1 Auto=�5  !   $
2 True��5  !   $3 0Q�5  !   ��5  !   �� *CarSummary.Items+4 5  "     5  &   $5 序号��5!  )   86 + 7 5   )   �� *SortNo+8 5#  *   �� 5$  .   " ��64   .�5%  2   ��  � ��$ Center=�6V   5&  /   5(  '     5)  &   $5 车号��5+  )   86 , 7 5*  )   �� *CarNo+8 5-  *   �� 5.  .   " ��64   .�5/  2   ��  � ��$ Center=�6V   50  /   52  '     53  &   $	9 132Q�6=   86 - 5 54  *   �� 55  .   $ 
Horizontal=�69   .+�56  2   �� 8��. $��车次��6m   �� 57  2   8��/ $��(��6m   �� 58  2   8��0 ��6m   �� 
 �� 
 Window$, &DataContext.CarSummary.TotalUnFinished��� 59  2   8��1 $��/��6m   �� 5:  2   8��2 ��6m   �� 
 �� 
 Window$* $DataContext.CarSummary.TotalFinished��� 5;  2   8��3 $��)��6m   5<  /   : 5>  *   �� 5?  .   .R�5@  2   �� $��Center=�6=   $ 
Horizontal=�6X   .+�5A  6   �� ��6@   �� *
UnFinish+�� 5B  6   $��/��6@   �� 5C  6   ��6@   �� *
Finished+5D  3   5E  /   5G  '     5H  &   $	9 132Q�6=   86 4 5 5I  *   �� 5J  .   $ 
Horizontal=�69   .+�5K  2   �� 8��5 $��	总净重��6m   �� 5L  2   8��6 $��(��6m   �� 5M  2   8��7 ��6m   �� 
 �� 
 Window$$ DataContext.CarSummary.TotalJZ��� 5N  2   8��8 $��)��6m   5O  /   : 5Q  *   �� 5R  .   .R�5S  2   �� $��Center=�6<   ��6W   �� *JZ+5T  /   5V  '     5W  &   $; 3* 5X  )   $5 操作��5Y  )   86 9 : 5[  *   �� 5\  .   .R�5]  2   �� $ 
Horizontal=�6=   .+�5^  6   �� 8��: 8��; 8��< ��5`  9     *CarSummaryReUse+��5a  9   �� .��5c  c   复用5e  7   5r  3   5s  /   5u  '   5x     5y     �� 5|     $��1��5}     $��12��5~     $��Stretch=�5     $	��Top=�5�     $< PreviousAndNext=�5�     ��5�     �� 5�  "   $	= Top=�5�  %   $> /Resources/Img/dian.png��5�  %   $
? None=�5�  %   5�     �� 5�     $��2��5�     $��4q�5�     8��= $ 8��5�     ._�5�     �  5�  "   �� 5�  &   $��1* �67   �� 5�  &   $��8* �67   .+�5�  "   �� $	��0,4q�5�  %   8��> $��0,0,1,0q�5�  %   ._�5�  &   �� $��Center=�61   $��Center=�6N   .+�5�  *   �� $	��0,4q�5�  -   $��0q�5�  -   8��? ��5�  -     *ToggleCarSummaryMode+.��5�  .   
  $��26��5�  1   $��26��5�  1   8��@  5�  1   �� *CarSummary.BtnImage+5�  +   �� 5�  *   $	��0,4q�5�  -   $��0q�5�  -   8��A 8��B ��5�  -     *RefreshCarSummary+.��5�  .   
  $��32��5�  1   $��32��5�  1   8��C $ Refresh=�5�  1   5�  +   5�  '   5�  #   �� 5�  "   $��1��5�  %   $��4q�5�  %   8��D $
��Auto=�5�  %   ��5�  %   �� *
LogContent+@ 5�  &     5�  *   5�  #   5�     5�     5�     5�     5�     �� 5�     $��1��5�     $��12��5�     $��Stretch=�5�     $	��Top=�5�     $< PreviousAndNext=�5�     ��5�     �� 5�     $	= Top=�5�     $> /Resources/Img/dian.png��5�     $
? None=�5�     5�     �� 5�     $��2��5�     $
��Auto��5�     $��4q�5�     $	 4q�5�     8��E $ 8��5�     ._�5�     �  5�     �� 5�     �� 5�     $��60 �6(   .+�5�       $
��Auto��5�     $' 2��5�     $
( True��5�     8��F $) False��5�     $* False��5�     $
+ True��5�     $
, True��5�     $
- True��5�     $
. True��5�     $
/ Auto 5�     $��16(�5�     8��G $	0 All=�5�     $
1 Auto=�5�     $
2 True��5�     $3 0Q�5�     $A Single=�5�     B 5�     �� $ 
DG_Summary��C 5�     �� $ 
DG_Summary��D 5�     �� $ 
DG_Summary����5�     �� *
RecordDT+ 5�       *WeightRecordDataGridLoaded+E 5�     �� *SelectedRecrod+F 5�     �� 5�     .@�5�  "   m� $��补充打印��5   %   G 5�  %     *	Reprint+{�5  %   �� *EnablePrintSelected+m� 5  "   $��修改磅单��5  %   G 5  %     *UpdateSelected+{�5  %   �� *EnableUpdateSelected+m� 5  "   $��手工制单��5  %   G 5  %     *
InsertItem+{�5	  %   �� *EnableInsertItem+m� 5
  "   $��查看图片��5  %   G 5  %     *OpenPicFolder+{�5  %   �� *EnableQueryList+m� 5  "   $��作废记录��5  %   G 5  %     *DeleteSelected+{�5  %   �� *EnableDeleteSelected+m� 5  "   $��筛选��6+   .@�5  &   m� $H 
IsFinish_0��5  )   $��	未完成��5  )   I 5  )     *	QueryList+5  )   m� 5  &   $H 
IsFinish_1��5  )   $��	已完成��5  )   I 5  )     *	QueryList+5  )   m� 5  &   $H 	IsFinish_��5  )   $��全部��5  )   I 5  )     *	QueryList+5  )   5  #   m� 5   "   $��统计��6+   .@�5!  &   m� $H DateRange_1��5#  )   $��当天��5$  )   I 5"  )     *	QueryList+5$  )   m� 5%  &   $H DateRange_7��5'  )   $
��7天��5(  )   I 5&  )     *	QueryList+5(  )   m� 5)  &   $H DateRange_30��5+  )   $��30天��5,  )   I 5*  )     *	QueryList+5,  )   m� 5-  &   $H 
DateRange_��5/  )   $��全部��50  )   I 5.  )     *	QueryList+50  )   51  #   52     54       56     $J 
DG_Summary��57     $��1��58     $
��Auto��59     $��Bottom=�5:     $' 2��5;     $
( True��5<     8��H 
������ $��0q�5?     $) False��5@     $* False��5A     $
+ True��5B     $
, True��5C     $
- True��5D     $
. True��5E     $K 0Q�5F     $
/ Auto 5G     $��16(�5H     8��I $
0 None=�5J     $1 Hidden=�5K     $
2 True��5L     $3 0Q�5N     $A Single=�5O     ��5M     �� *RecordDTSummary+5O     5P     5R     5S     5T     5U     