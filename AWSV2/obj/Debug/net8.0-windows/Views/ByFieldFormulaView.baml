   M S B A M L   `   `   `  ����S  OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls  5       
篮圈AVS1$   AWSV2.Views.ByFieldFormulaView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  5      [mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions  5      A	 =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null)s!https://github.com/canton7/Stylet 	 5        � Title$  备用字段公式设置��5      $	��480��5	      $	��640��5
         MaterialDesignPaper#��[  5      6   0MaterialDesignExtensions.Controls.MaterialWindow   BorderBackgroundBrush# [  5        MaterialDesignBody   BorderForegroundBrush# [ 5       � ShowInTaskbar$ False��5      $��Microsoft YaHei)�5      $��16(�5      $��Medium"�5      #���  5       � WindowStyle$ 
ToolWindow=�5      .��5      �� $
��Auto=�6   .��5   
   �� .+�5      �� $
��4,12q�6   公式可用字段：5      �� 5      $��30,5,5,5q�6   (&毛重：_mz 皮重：_pz 净重：_jz�� 5   &   (&扣重：_kz 扣率：_kl 实重：_sz�� 5   &   (&备用字段：_by1 , _by2 …… _by55   -   �� 5      $
��4,12q�6   常用函数：5       �� 5!      $��30,5,5,5q�6   !向上取整：Ceiling(5.2) = 6�� 5"   '   向下取整：Floor(5.8) = 5�� 5#   %   +)四舍五入：Round(3.1415926, 2) = 3.145$   2   �� 5%      $
��4,12q�6   $��Center=�6&   �� 5&       �� Command 6   ! 	 Stylet.Xaml.ActionExtension  *ShowHelpChm+.�6=   )'更多函数及示例详见帮助文档6L   5'      �� 5(      $
��4,12q�6   .+�5)      �� )  #MaterialDesignThemes.Wpf.HintAssist   
Background# [  5*         Hint$ 备用字段1公式��5+      ��5,      �� *
By1Formula+5-      ]� 5.      $
��4,12q�6   .+�50      �� $ 备用字段2公式��6   ��67   �� *
By2Formula+51      ]� 52      $
��4,12q�6   .+�54      �� $ 备用字段3公式��6   ��67   �� *
By3Formula+55      ]� 56      $
��4,12q�6   .+�58      �� $ 备用字段4公式��6   ��67   �� *
By4Formula+59      ]� 5:      $
��4,12q�6   .+�5<      �� $ 备用字段5公式��6   ��67   �� *
By5Formula+5=      ]� 5>      $
��4,12q�6   .+�5@      �� $ 备用字段6公式��6   ��67   �� *
By6Formula+5A      ]� 5B      $
��4,12q�6   .+�5D      �� $ 备用字段7公式��6   ��67   �� *
By7Formula+5E      ]� 5F      $
��4,12q�6   .+�5H      �� $ 备用字段81公式��6   ��68   �� *
By8Formula+5I      ]� 5J      $
��4,12q�6   .+�5L      �� $ 备用字段9公式��6   ��67   �� *
By9Formula+5M      ]� 5N      $
��4,12q�6   .+�5P      �� $ 备用字段10公式��6   ��68   �� *By10Formula+5Q      ]� 5R      $
��4,12q�6   .+�5T      �� $ 备用字段11公式��6   ��68   �� *By11Formula+5U      ]� 5V      $
��4,12q�6   .+�5X      �� $ 备用字段12公式��6   ��68   �� *By12Formula+5Y      ]� 5Z      $
��4,12q�6   .+�5\      �� $ 备用字段13公式��6   ��68   �� *By13Formula+5]      ]� 5^      $
��4,12q�6   .+�5`      �� $ 备用字段14公式��6   ��68   �� *By14Formula+5a      ]� 5b      $
��4,12q�6   .+�5d      �� $ 备用字段15公式��6   ��68   �� *By15Formula+5e      ]� 5f      $
��4,12q�6   .+�5h      �� $ 备用字段16公式��6   ��68   �� *By16Formula+5i      ]� 5j      $
��4,12q�6   .+�5l      �� $ 备用字段17公式��6   ��68   �� *By17Formula+5m      ]� 5n      $
��4,12q�6   .+�5p      �� $ 备用字段18公式��6   ��68   �� *By18Formula+5q      ]� 5r      $
��4,12q�6   .+�5t      �� $ 备用字段19公式��6   ��68   �� *By19Formula+5u      ]� 5v      $
��4,12q�6   .+�5x      �� $ 备用字段20公式��6   ��68   �� *By20Formula+5y      �� 5z      $
��4,12q�5{      $��保存��5}      ��5|        *SaveFormula+5}      �� 5~      $
��4,12q�6   ��6&   �� *	StatusBar+ �� FallbackValue$ 	状态栏��5      5�      5�      