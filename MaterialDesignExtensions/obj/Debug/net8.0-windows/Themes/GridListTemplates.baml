   M S B A M L   `   `   `  ����W  SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   MaterialDesignGridListItem  �� 
TargetType �� Property �� Value �� 
TargetType � 	Resources  Key �� 
TargetName �� TargetProperty9    3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value	    EasingFunction/   )System.Windows.Media.Animation.CircleEase7   1System.Windows.Media.Animation.EasingFunctionBase
   
EasingMode ��Name ! MaterialDesignFlatButtonClick J� Opacity J� RenderTransformOrigin �� ScaleX J� SnapsToDevicePixels �� Triggers P� Property P� Value N� EnterActions  
OnSelected N� ExitActions  OnUnSelected ]� 
Conditions �� Property �� Value �� 
TargetName  MaterialDesignDivider  MaterialDesignGridList  MaterialDesignBody K� IsItemsHost � Orientation�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml      6W   %�	  &	        7  7  &	 .    7   �� 5      "  ��6/   .�5   
   ��  � ��
 ���� 6'   �� 5   
    � ��$ 0q�6,   �� 5   
    � ��$ 4q�6#   �� 5   
    �@J�SnapsToDevicePixels$
 True��60   �� 5   
    � �� 5	      �� 5
      " ��6"    5      ���� 5      
OnSelected .��5      U� $ SelectedBorder��6<   $ Opacity�6c   .\�5   "      $ 0:0:0.05��67   $ 1Q�6J   	 5   &     5   *   $
 EaseOut=�65   5   #   5      U� 5      $ SelectedBorder��6<   $V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�6c   .\�5   "      $ 0:0:0.1��67   $ 1Q�6I   5      5      �� 5      OnUnSelected .��5      U� $ SelectedBorder��6<   $ Opacity�6c   .\�5   "      $ 0:0:0.05��67   $ 0Q�6J   	 5   &     5   *   $
 EaseOut=�65   5   #   5      U� 5       $ SelectedBorder��6<   $V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�6c   .\�5!   "      $ 0:0:0.1��67   $ 0Q�6I   5"      5#      5$      .R�5%      � 
������ .+�5&      �� $ MouseOverBorder��6!   �� 5'      $ SelectedBorder��5(      #���  5)      $ 0Q�5*      $ 0.5,0.5&�5+      z�5,      V� 5-   "   .~�5.   &   �� $ 0Q�65   �� 5/   &   �� 50   &   T� 51   &   52   #   54      �� 55      $��2q�56      #��z��57      #��z��58      # z 59      5:       5;      P� 5<       �@��
IsSelected$
 True��68    5=      �� 5>   "   8��  62    5@      �� 5A   "   8�� 62   5C      ]� 5D       5E      �� 5F   "    �@��
IsSelected$ False��6B   �� 5G   "    �@J�IsMouseOver$
 True��6C   ./�5I      �� $ MouseOverBorder��6%   # �  6X    � ��5J      5L      5N      5O      �� 5R      "  ��6+   .�5S   
   ��  � ��
 ���� 6'   �� 5T   
    � ��
 ���� 6(   �� 5U   
    � ��$ 0q�6,   �� 5V   
    � ��# �  6'   �� 5W   
    � ��$ Disabled=�6G   �� 5X   
    � ��$
 Auto=�6E   �� 5Y   
    � ��$
 true��6:   �� 5Z   
    �@��PanningMode$
 Both=�65   �� 5[   
    �@��IsFlicksEnabled$ False��63   �� 5\   
    � �� 5]      �� 5^      .R�5_      � $��Center=�5`      $	��Top=�5a      $
 True��5b      $ 
Horizontal=�5c      5d      5f      �� 5g   
    � ��8   6/   5h      5j      