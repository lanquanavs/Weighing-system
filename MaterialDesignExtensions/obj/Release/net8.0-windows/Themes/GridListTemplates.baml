   M S B A M L   `   `   `  ���� W  SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   MaterialDesignGridListItem  �� 
TargetType �� Property �� Value �� 
TargetType � 	Resources  Key �� 
TargetName �� TargetProperty9    3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value	    EasingFunction/   )System.Windows.Media.Animation.CircleEase7   1System.Windows.Media.Animation.EasingFunctionBase
   
EasingMode ��Name ! MaterialDesignFlatButtonClick J� Opacity J� RenderTransformOrigin �� ScaleX J� SnapsToDevicePixels �� Triggers P� Property P� Value N� EnterActions  
OnSelected N� ExitActions  OnUnSelected ]� 
Conditions �� Property �� Value �� 
TargetName  MaterialDesignDivider  MaterialDesignGridList  MaterialDesignBody K� IsItemsHost � Orientation�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml      %�  &	        7  7  &	     7   �� "  ��.���  � ��
 ���� ��  � ��$ 0q���  � ��$ 4q���  �@J�SnapsToDevicePixels$
 True����  � �� �� " �� ���� 
OnSelected .��U� $ SelectedBorder��$ Opacity�.\�   $ 0:0:0.05��$ 1Q�	   $
 EaseOut=�U� $ SelectedBorder��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\�   $ 0:0:0.1��$ 1Q��� OnUnSelected .��U� $ SelectedBorder��$ Opacity�.\�   $ 0:0:0.05��$ 0Q�	   $
 EaseOut=�U� $ SelectedBorder��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\�   $ 0:0:0.1��$ 0Q�.R�� 
������ .+��� $ MouseOverBorder���� $ SelectedBorder��#���  $ 0Q�$ 0.5,0.5&�z�V� .~��� $ 0Q��� �� T� �� $��2q�#��z��#��z��# z  P�  �@��
IsSelected$
 True�� �� 8��   �� 8�� ]�  ��  �@��
IsSelected$ False����  �@J�IsMouseOver$
 True��./��� $ MouseOverBorder��# �   � ���� "  ��.���  � ��
 ���� ��  � ��
 ���� ��  � ��$ 0q���  � ��# �  ��  � ��$ Disabled=���  � ��$
 Auto=���  � ��$
 true����  �@��PanningMode$
 Both=���  �@��IsFlicksEnabled$ False����  � �� �� .R�� $��Center=�$	��Top=�$
 True��$ 
Horizontal=���  � ��8   