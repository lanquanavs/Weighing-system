   M S B A M L   `   `   `  ����   MaterialDesignExtensionswQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls  W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089_ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a !  MaterialDesignOutlinedTextBox  �� BasedOn �� 
TargetType �� Property �� Value.   (MaterialDesignThemes.Wpf.TextFieldAssist)  #MaterialDesignThemes.Wpf.HintAssist  MaterialDesignBody " MaterialDesignOutlinedComboBox + 'MaterialDesignOutlinedRevealPasswordBox  MaterialDesignRaisedButton+  %MaterialDesignThemes.Wpf.ButtonAssist.  (MaterialDesignThemes.Wpf.ElevationAssist  PrimaryHueDarkBrush3  -MaterialDesignThemes.Wpf.ButtonProgressAssist  MaterialDesignPaper  MaterialDesignCheckBox-  'MaterialDesignThemes.Wpf.CheckBoxAssist �� 
TargetType �Name J� SnapsToDevicePixels � ColumnDefinitions ��Name	 �� CornerRadius
 �� RelativeSource �� Path'  !MaterialDesignThemes.Wpf.PackIcon  Name J� Opacity ��Name �� Triggers ^� 
Conditions �� Binding �� Value P� Property P� Value  OptionMarkFocusVisual �� 
TargetName &	 "MaterialDesignValidationErrorBrush )
 %MaterialDesignValidationErrorTemplate  MaterialDesignRadioButton � 	Resources  Key �� 
TargetName �� TargetProperty9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value F�Name A�Name �� Left
 �� Top J� RenderTransformOrigin  :� RoutedEvent 	 Click " MaterialDesignCheckBoxDisabled  ThisPageButtonStyle! � Cursor" J� Effect3  -System.Windows.Media.Effects.DropShadowEffect#   
BlurRadius$   	Direction%   Opacity&   ShadowDepth  CommonWhiteColor'   Color( ��Name  ThisPageYellowButtonStyle  ThisPageOrangeButtonStyle  Button NoneStyle ! MaterialDesignFlatLightButton  Label A  Label B�� H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml     :md1http://materialdesigninxaml.net/winfx/xaml/themes  YmdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions  %�  '��       7   7  '�� �     7  7  'G�     7  7  '�� �    7  7  7  7  7  '�� �    7  7  7  7  7 
 '	� �    7  7  7  7  7  7 
 &	     7  7  7  &	 �    7  &	 �    7  &	 �    7  &	 �    &	 v    �� 8    " ��.���  � ��$ 8q���  �@  TextFieldCornerRadius$ 0����  �@ 
Background
 ���� ��  � ��8  �� 8    " ��.���  � ��$ 8q���  �@  TextFieldCornerRadius$ 0����  �@ 
Background
 ���� ��  � ��8  �� 8    " G�.���  � ��$ 8q���  �@  TextFieldCornerRadius$ 0����  �@ 
Background
 ���� ��  � ��8  �� 8    " ��.���  �@ CornerRadius$ 0����  � ��$ 42����  �@ 	Elevation$	 Dp0=���  � ��8  ��  � ��8  ��  � �� Z� ��  �@ IndicatorBackground8  ��  �@ IndicatorForeground8  �� 8    " ��.���  �@ CheckBoxSize$ 14Q���  � ��8  ��  � ��8  ��  � ��$	 0.5q���  � �� �� " ��.R�� $ templateRoot��
������ $
 True�� �� $
��Auto ��� $��* �.+��� $ bd_chx��#��z��#��z��#��z��$	 2��$��LeftToRight=����� 
 �� *TemplatedParent+$&  (md:CheckBoxAssist.CheckBoxSize)����� 
 �� *TemplatedParent+$&  (md:CheckBoxAssist.CheckBoxSize)�._�  $ Graphic��$��14��$��14��$��Center=�$��Center=�#��z��$ 0Q��� $ contentPresenter��$��1��#��z��#��z��#��z��$|�False��$
��True��# z  ^�  ��  �� 
 �� *Self+$ 	IsPressed� true��  �� 
 �� *Self+$" (md:RippleAssist.IsDisabled)�	 falseP�  � ��$
 true��. ��� 8   �@�FocusVisualStyle��  � ��$ 4,-1,0,0q�P�  � ��$
 true��P�  �@d�	IsChecked$
 trueY�. ��� $ Graphic�� �@ Kind$ Check=��� $ Graphic�� �@J�Opacity$ 1Q��� $ Graphic�� �� 
 �� *TemplatedParent+$ 
Background� � ��P�  � {�$ false��. ���  �@J�Opacity$
 0.56Q��� $ Graphic�� �@J�Opacity$	 0.6Q��� $ bd_chx�� �@J�Opacity$	 0.6Q�P�  Z�  �@d�	IsChecked. ��� $ Graphic�� �� 
 �� *TemplatedParent+$ 
Background� � ���� $ Graphic�� �@ Kind$ Square=��� $ Graphic�� �@J�Opacity$ 1Q�P�  �@=�HasError$
 true��. ��� $ Graphic��# � 	  � ���� $ bd_chx��# � 	  � ���� $ contentPresenter��# � 	  � ����  �@=�ErrorTemplate8  ��  � ��$ Center=��� 8    " 	�.���  � ��8  ��  � ��8  ��  � �� �� " 	� ���� 	Click .��U� $ InteractionEllipse��$ (FrameworkElement.Width)�.\�  $ 0��$ 0Q�  $ 0:0:0.3��$ 48Q�U� $ InteractionEllipse��$ (FrameworkElement.Height)�.\�  $ 0��$ 0Q�  $ 0:0:0.3��$ 48Q�U� $ InteractionEllipse��$U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�.\�  $ 0��$ 0Q�  $ 0:0:0.3��$	 -24Q�U� $ InteractionEllipse��$U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)�.\�  $ 0��$ 0Q�  $ 0:0:0.3��$	 -24Q�U� $ InteractionEllipse��$ (UIElement.Opacity)�.\�  $ 0��$	 0.3Q�  $ 0:0:0.3��$ 0Q�.R�� $ templateRoot��
������ $
 True�� �� $
��Auto ��� $��* �.+�(� $��18��$��18��#��z��.���� $��24��$��24��.+�F� $ Graphic������� �  -1� 	=  � � z  z � �  	= � z  z � -1 � � z  z � �  -1� z  z � � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� 	#��z��A� $ InteractionEllipse��$ 12��$ 12��$��0��$��0��#��z��$ 0Q�$ 0.5,0.5&�z�V� .~��� �� �� T� �� $ contentPresenter��$��1��#��z��#��z��#��z��$|�False��$
��True��# z  :� $  ButtonBase.Click��.[��� 8�� P�  � ��$
 true��. ��� 8   �@�FocusVisualStyle��  � ��$ 4,-1,0,0q�P�  � {�$ false��. ���  �@J�Opacity$
 0.26Q��� $ Graphic��# �   � ��P�  � ��$
 true��P�  �@d�	IsChecked$
 trueY�. ��� $ Graphic�� � ��� �� �  -1� 	=  � � z  z � �  	= � z  z � -1 � � z  z � �  -1� z  z � � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� � � ��j ���j  �  @KL @KL � � @f @KL @KL �@f �  @KL @KL � � ��j  @KL @KL 	�� $ Graphic�� �� 
 �� *TemplatedParent+$ 
Background� � ��P�  Z�  �@d�	IsChecked. ��� $ Graphic�� � ��� �� �  -1� 	=  � � z  z � �  	= � z  z � -1 � � z  z � �  -1� z  z � � �� ׀�  �  ��� ��� � � ��O ��� ��� ׀�O �  ��� ��� � � ��  ��� ��� � � ��j ���j  �  @KL @KL � � @f @KL @KL �@f �  @KL @KL � � ��j  @KL @KL 	�� $ Graphic�� �@J�Opacity$
 0.56Q�P�  �@=�HasError$
 true��. ��� $ Graphic��# � 	  � ���� $ contentPresenter��# � 	  � ����  �@=�ErrorTemplate8  ��  � ��$ Center=��� " ��.���  � ��$ 16(���  � ��$ 48����  � ��$	 132����  � ��$	 0.5q���  � ��# �  ��  � ��# �  ��  � ��# �  ��  � �� �� " ��.R�� .+��� $ border��#��z��#��z��#��z��$	 0��$
! Hand��"   $# 8Q�$$ 0Q�$% .4Q�$& 0Q�8'   �� $( 
buttonText��$��Center=�$��Center=�#��z��#��z�� P�  �@J�IsMouseOver$
 true��. ��� $ border�� �@J�Effect   $# 12Q�$$ 0Q�$% .6Q�$& 0Q�8'  P�  � {�$ False��. ��� $ border��8   � ���� $ 
buttonText��# �   � ����  � ��$	 0.5q���  �@J�Opacity$
 0.38Q��� 8    " ���� 8    " ���� 8    " ���� " ��.���  � �� �#F5F7F9��  � ��$ Center=���  � ��$ 45����  � ��$ 16(���  � �� �#222222��  � ��$ 12,0,0,0q��� " ��.���  � ��$ Center=���  � ��$ 45����  � ��$ 18(���  � �� �#666666��  � ��$ 12,0,0,0q�