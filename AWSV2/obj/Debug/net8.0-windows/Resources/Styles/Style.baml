   M S B A M L   `   `   `  ����S  OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls  5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a !  MaterialDesignOutlinedTextBox  �� BasedOn �� 
TargetType �� Property �� Value.   (MaterialDesignThemes.Wpf.TextFieldAssist)  #MaterialDesignThemes.Wpf.HintAssist  MaterialDesignBody " MaterialDesignOutlinedComboBox + 'MaterialDesignOutlinedRevealPasswordBox  MaterialDesignRaisedButton+  %MaterialDesignThemes.Wpf.ButtonAssist.  (MaterialDesignThemes.Wpf.ElevationAssist  PrimaryHueDarkBrush3  -MaterialDesignThemes.Wpf.ButtonProgressAssist  MaterialDesignPaper  MaterialDesignCheckBox-  'MaterialDesignThemes.Wpf.CheckBoxAssist �� 
TargetType �Name J� SnapsToDevicePixels � ColumnDefinitions ��Name	 �� CornerRadius
 �� RelativeSource �� Path'  !MaterialDesignThemes.Wpf.PackIcon  Name J� Opacity ��Name �� Triggers ^� 
Conditions �� Binding �� Value P� Property P� Value  OptionMarkFocusVisual �� 
TargetName &	 "MaterialDesignValidationErrorBrush )
 %MaterialDesignValidationErrorTemplate  MaterialDesignRadioButton � 	Resources  Key �� 
TargetName �� TargetProperty9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value F�Name A�Name �� Left
 �� Top J� RenderTransformOrigin  :� RoutedEvent 	 Click " MaterialDesignCheckBoxDisabled  ThisPageButtonStyle! � Cursor" J� Effect3  -System.Windows.Media.Effects.DropShadowEffect#   
BlurRadius$   	Direction%   Opacity&   ShadowDepth  CommonWhiteColor'   Color( ��Name  ThisPageYellowButtonStyle  ThisPageOrangeButtonStyle  Button NoneStyle ! MaterialDesignFlatLightButton  Label A  Label B�� 5      H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      :md1http://materialdesigninxaml.net/winfx/xaml/themes  5      YmdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions  5      %'%  '��       7   7  '�� �     7  7  'G� �    7  7  '�� �    7  7  7  7  7  '�� �    7  7  7  7  7 
 '	� �    7  7  7  7  7  7 
 &	     7  7  7  &	 �!    7  &	 "    7  &	 $"    7  &	 E"    &	 W#    �� 5      8    " ��6E   .�5	   
   ��  � ��$ 8q�6$   �� 5
   
    �@  TextFieldCornerRadius$ 0��6E   �� 5   
    �@ 
Background
 ���� 65   �� 5   
    � ��8  6(   5      �� 5      8    " ��6F   .�5   
   ��  � ��$ 8q�6$   �� 5   
    �@  TextFieldCornerRadius$ 0��6E   �� 5   
    �@ 
Background
 ���� 65   �� 5   
    � ��8  6(   5      �� 5      8    " G�6O   .�5   
   ��  � ��$ 8q�6$   �� 5   
    �@  TextFieldCornerRadius$ 0��6E   �� 5   
    �@ 
Background
 ���� 65   �� 5   
    � ��8  6(   5      �� 5      8    " ��6B   .�5   
   ��  �@ CornerRadius$ 0��69   �� 5   
    � ��$ 42��6#   �� 5    
    �@ 	Elevation$	 Dp0=�69   �� 5!   
    � ��8  6'   �� 5"   
    � ��8  6'   �� 5#   
    � �� 6(   Z� �� 5$   
    �@ IndicatorBackground8  6H   �� 5%   
    �@ IndicatorForeground8  6H   5&      �� 5(      8    " ��6>   .�5)   
   ��  �@ CheckBoxSize$ 14Q�6;   �� 5*   
    � ��8  6'   �� 5+   
    � ��8  6'   �� 5,   
    � ��$	 0.5q�6,   �� 5-   
    � �� 5.      �� 5/      " ��6"   .R�50      � $ templateRoot��51      
������ $
 True��53       54      �� 55      $
��Auto �6/   �� 56      $��* �6/   .+�58      �� $ bd_chx��59      #��z��5<      #��z��5=      #��z��5>      $	 2��5?      $��LeftToRight=�5@      ��5:      �� 
 �� *TemplatedParent+$&  (md:CheckBoxAssist.CheckBoxSize)���5;      �� 
 �� *TemplatedParent+$&  (md:CheckBoxAssist.CheckBoxSize)�._�5A        $ Graphic��5B   !   $��14��5C   !   $��14��5D   !   $��Center=�5E   !   $��Center=�5F   !   #��z��5G   !   $ 0Q�5H   !   5I      �� 5J      $ contentPresenter��5K      $��1��5L      #��z��5M      #��z��5N      #��z��5O      $|�False��5P      $
��True��5Q      # z 5R      5S       5T      ^� 5U       5V      �� 5W   "    6,   �� 
 �� *Self+$ 	IsPressed� true6u   �� 5X   "    6,   �� 
 �� *Self+$" (md:RippleAssist.IsDisabled)�	 false6�   5]      P� 5^       � ��$
 true��68   . �5_      �� 8   �@�FocusVisualStyle6A   �� 5`       � ��$ 4,-1,0,0q�68   5a      P� 5b       � ��$
 true��67   P� 5c       �@d�	IsChecked$
 trueY�67   . �5d      �� $ Graphic��6%    �@ Kind$ Check=�6J   �� 5f      $ Graphic��6%    �@J�Opacity$ 1Q�6M   �� 5g      $ Graphic��6%    6P   �� 
 �� *TemplatedParent+$ 
Background� � ��5h      P� 5i       � {�$ false��67   . �5j      ��  �@J�Opacity$
 0.56Q�68   �� 5k      $ Graphic��6%    �@J�Opacity$	 0.6Q�6M   �� 5l      $ bd_chx��6%    �@J�Opacity$	 0.6Q�6L   5m      P� 5n       67   Z�  �@d�	IsChecked. �5o      �� $ Graphic��6%    6P   �� 
 �� *TemplatedParent+$ 
Background� � ���� 5p      $ Graphic��6%    �@ Kind$ Square=�6J   �� 5q      $ Graphic��6%    �@J�Opacity$ 1Q�6M   5r      P� 5s       �@=�HasError$
 true��6A   . �5t      �� $ Graphic��6%   # � 	 6P    � ���� 5u      $ bd_chx��6%   # � 	 6P    � ���� 5v      $ contentPresenter��6%   # � 	 6a    � ��5w      5y      5{      �� 5|   
    �@=�ErrorTemplate8  65   �� 5}   
    � ��$ Center=�65   5~      �� 5�      8    " 	�6A   .�5�   
   ��  � ��8  6'   �� 5�   
    � ��8  6'   �� 5�   
    � �� 5�      �� 5�      " 	�6"    5�      ���� 5�      	Click .��5�      U� $ InteractionEllipse��6<   $ (FrameworkElement.Width)�6g   .\�5�   "     $ 0��67   $ 0Q�6C     5�   "   $ 0:0:0.3��67   $ 48Q�6I   5�      U� 5�      $ InteractionEllipse��6<   $ (FrameworkElement.Height)�6g   .\�5�   "     $ 0��67   $ 0Q�6C     5�   "   $ 0:0:0.3��67   $ 48Q�6I   5�      U� 5�      $ InteractionEllipse��6<   $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�6g   .\�5�   "     $ 0��67   $ 0Q�6C     5�   "   $ 0:0:0.3��67   $	 -24Q�6I   5�      U� 5�      $ InteractionEllipse��6<   $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)�6g   .\�5�   "     $ 0��67   $ 0Q�6C     5�   "   $ 0:0:0.3��67   $	 -24Q�6I   5�      U� 5�      $ InteractionEllipse��6<   $ (UIElement.Opacity)�6g   .\�5�   "     $ 0��67   $	 0.3Q�6C     5�   "   $ 0:0:0.3��67   $ 0Q�6I   5�      5�      5�      .R�5�      � $ templateRoot��5�      
������ $
 True��5�       5�      �� 5�      $
��Auto �6/   �� 5�      $��* �6/   .+�5�      (� $��18��5�      $��18��5�      #��z��5�      .��5�      �� $��24��6%   $��24��60   .+�5�   "   F� $ Graphic��5�   %   ����� �  -1� 	=  � � z  z � �  	= � z  z � -1 � � z  z � �  -1� z  z � � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� 	#��z��5�   %   A� 5�   "   $ InteractionEllipse��5�   %   $ 12��5�   %   $ 12��5�   %   $��0��5�   %   $��0��5�   %   #��z��5�   %   $ 0Q�5�   %   $ 0.5,0.5&�5�   %   z�5�   &   V� 5�   *   .~�5�   .   �� �� 5�   .   �� 5�   .   T� 5�   .   5�   +   5�   #   5�      5�      �� 5�      $ contentPresenter��5�      $��1��5�      #��z��5�      #��z��5�      #��z��5�      $|�False��5�      $
��True��5�      # z 5�      5�       5�      :� 5�      $  ButtonBase.Click��6'   .[�5�      �� 8�� 6.   5�      P� 5�       � ��$
 true��68   . �5�      �� 8   �@�FocusVisualStyle6A   �� 5�       � ��$ 4,-1,0,0q�68   5�      P� 5�       � {�$ false��67   . �5�      ��  �@J�Opacity$
 0.26Q�68   �� 5�      $ Graphic��6%   # �  6J    � ��5�      P� 5�       � ��$
 true��67   P� 5�       �@d�	IsChecked$
 trueY�67   . �5�      �� $ Graphic��6%    � ��� �� �  -1� 	=  � � z  z � �  	= � z  z � -1 � � z  z � �  -1� z  z � � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� � � ��j ���j  �  @KL @KL � � @f @KL @KL �@f �  @KL @KL � � ��j  @KL @KL 	6J   �� 5�      $ Graphic��6%    6J   �� 
 �� *TemplatedParent+$ 
Background� � ��5�      P� 5�       67   Z�  �@d�	IsChecked. �5�      �� $ Graphic��6%    � ��� �� �  -1� 	=  � � z  z � �  	= � z  z � -1 � � z  z � �  -1� z  z � � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� � � ��j ���j  �  @KL @KL � � @f @KL @KL �@f �  @KL @KL � � ��j  @KL @KL 	6J   �� 5�      $ Graphic��6%    �@J�Opacity$
 0.56Q�6M   5�      P� 5�       �@=�HasError$
 true��6A   . �5�      �� $ Graphic��6%   # � 	 6J    � ���� 5�      $ contentPresenter��6%   # � 	 6a    � ��5�      5�      5�      �� 5�   
    �@=�ErrorTemplate8  65   �� 5�   
    � ��$ Center=�65   5�      �� 5�      " ��6(   .�5�   
   ��  � ��$ 16(�6%   �� 5�   
    � ��$ 48��6#   �� 5�   
    � ��$	 132��6%   �� 5�   
    � ��$	 0.5q�6,   �� 5�   
    � ��# �  6'   �� 5�   
    � ��# �  6'   �� 5�   
    � ��# �  6(   �� 5�   
    � �� 5�      �� 5�      " ��6"   .R�5�      � .+�5�      �� $ border��5      #��z��5     #��z��5     #��z��5     $	 0��5     $
! Hand��5     " 5       5  "   $# 8Q�5  %   $$ 0Q�5	  %   $% .4Q�5
  %   $& 0Q�5  %   8'   5  %   5     �� 5     $( 
buttonText��5     $��Center=�5     $��Center=�5     #��z��5     #��z��5     5      5     P� 5      �@J�IsMouseOver$
 true��69   . �5     �� $ border��6%    �@J�Effect 5  "     5  &   $# 12Q�5  )   $$ 0Q�5  )   $% .6Q�5  )   $& 0Q�5  )   8'  5   )   5"     5#     P� 5$      � {�$ False��67   . �5%     �� $ border��6%   8   � ��6O   �� 5&     $ 
buttonText��6%   # �  6S    � ���� 5'      � ��$	 0.5q�6@   �� 5(      �@J�Opacity$
 0.38Q�68   5)     5+     5-     5/     �� 50     8    " ��53  	   �� 55     8    " ��58  	   �� 5:     8    " ��5=  	   �� 5@     " ��6   .�5A  
   ��  � �� �#F5F7F96'   �� 5B  
    � ��$ Center=�65   �� 5C  
    � ��$ 45��6#   �� 5D  
    � ��$ 16(�6%   �� 5E  
    � �� �#2222226'   �� 5F  
    � ��$ 12,0,0,0q�6$   5G     �� 5H     " ��6   .�5I  
   ��  � ��$ Center=�65   �� 5J  
    � ��$ 45��6#   �� 5K  
    � ��$ 18(�6%   �� 5L  
    � �� �#6666666'   �� 5M  
    � ��$ 12,0,0,0q�6$   5N     5S     