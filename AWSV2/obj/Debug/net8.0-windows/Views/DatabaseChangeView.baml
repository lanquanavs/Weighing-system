   M S B A M L   `   `   `  ����F  B篮圈AVS1, Version=3.0.0.14, Culture=neutral, PublicKeyToken=null1clr-namespace:AWSV2.ConverterAWSV2.Converter  5      3clr-namespace:AWSV2.ViewModelsAWSV2.ViewModels  5      S OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls 5
      $    AWSV2.Views.DatabaseChangeView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      AControls,http://metro.mahapps.com/winfx/xaml/controls     5      3	converterclr-namespace:AWSV2.Converter     5      C	iconPacks-http://metro.mahapps.com/winfx/xaml/iconpacks     5      0localclr-namespace:AWSV2.ViewModels     5      _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes  5	      [mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions  5
      A	 =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null)s!https://github.com/canton7/Stylet 	 5      $	��480��5      $	��600��5        � AllowsTransparency$
  True��5      6  0MaterialDesignExtensions.Controls.MaterialWindow   BorderBackgroundBrush
 ����    FadeContentIfInactive$ False��5       � 
ResizeMode$ NoResize=�5       � SizeToContent$ Height=�5      $��Microsoft YaHei)�5      $��16(�5      $��Medium"�5         MaterialDesignBody#���   5       � WindowStartupLocation$ CenterScreen=�5       � WindowStyle$
 None=�5      ��5      Z�  � 	Resources 5      /   )AWSV2.ViewModels.DbTypeToVisibleConverter  dbTypeVisibleConvert/  )AWSV2.Converter.DBSelectedToBoolConverter  dbtb��%:   &	       &	        5   
   6)     5   
   6.   5      .��5      �� $��12q�5   	     MaterialDesignPaper#��[ 5    	    �� CornerRadius$ 0��5!   	   	 J� Effect	 5"   
   3  -System.Windows.Media.Effects.DropShadowEffect  5#      
   
BlurRadius$
 18Q�5$         	Direction$	 120Q�5%         Opacity$
 0.42Q�5&         ShadowDepth$ 0Q�5'        CommonWhiteColor   Color# [ 5(      ._�5+   
   �� .+�5,      � $��8q�6   .+�5-      �� $
��Left=�5.      ���#000  MaterialDesignIconButton#��[ 51      ��5/      ! 	 Stylet.Xaml.ActionExtension  *Exit+.��52      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ ChevronLeft=�6"   53      �� 54      $��Center=�55      $��Center=�56      $��24(�57      $
��Bold"�58      $��数据库调整��59      5;      �� 5<      $��Center=�5=      $*��$修改数据库后需要重启软件��5>      $��12(�5?      $
��Bold"�5@      �� 5B      $��48,24q�6   .+�5C      �� $��0,0,0,24q�5D       �� Padding$ 8q�5E        TextBox#��[ 5F      $ 0��5G      ._�5H      �� )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 数据库类型��5I         
IsFloating$ False��5J      .  (MaterialDesignThemes.Wpf.TextFieldAssist   DecorationVisibility$ 	Collapsed=�5K      $��0q�5L       �� DisplayMemberPath$	 Key��5M       �� SelectedValuePath$ Value��5P        MaterialDesignComboBox#��[ 5Q      ��5N      �� *TypeOptions+ �� SelectedValue 5O      �� *
DbCfg.Type+5Q      5R      �� 5U      $��0,0,0,24q�5V      $ 8q�5W      #��[ 5X      $ 0��5Y      y�5Z      �� *
DbCfg.Type+ �� 	Converter# [  �� ConverterParameter$ sqlite��._�5[      ]� .+�5\        $��22��5]      $��22��5^      $
��12,0q�5_      $��Center=�5`      $ Harddisk=�5a      �� 5b      #��[ 6!   $��1,0,0,0q�6T   �� 5c      $��0,0,1,0q�5d      +	  %MaterialDesignThemes.Wpf.ButtonAssist 	  CornerRadius$ 0,0,0,0��5e      
������ $��0q�5g      $��Right=�5i      ��5h        *
LoadDBPath+.��5j        $ DotsHorizontal=�6*   5k      �� 5l      $
��12,0q�5m      $��Center=�5n        PrimaryHueMidBrush   
Foreground# [ 5o      $ 选择路径��5p      $ False��5q      $ 	Collapsed=�5r      $��0q�5s      $��Right=�5t       �� 
IsReadOnly$
 True��5u       	 MaterialDesignTextBox#��[	 5v      ��5w      �� *DbCfg.DbFile+ �� UpdateSourceTrigger$ PropertyChanged=�5y      5{      �� 5~      $��0,0,0,24q�5      $ 8q�5�      #��[ 5�      $ 0��5�      y�5�      �� *
DbCfg.Type+# [ $ mysql|mssql��._�5�      ]� .+�5�        $��22��5�      $��22��5�      $
��12,0q�5�      $��Center=�5�      $ DatabaseCogOutline=�5�      �� 5�      #��[ 6!   $��1,0,0,0q�6T   �� 5�      $
��12,0q�5�      $��Center=�5�      # [ 5�      $ IP地址��5�      $ False��5�      $ 	Collapsed=�5�      $��0q�5�      #��[	 5�      ��5�      �� *DbCfg.Server+$ PropertyChanged=�5�      5�      �� 5�      $��0,0,0,24q�5�      $ 8q�5�      #��[ 5�      $ 0��5�      y�5�      �� *
DbCfg.Type+# [ $ mysql|mssql��._�5�      ]� .+�5�      �� $
��12,0q�5�      $��Center=�5�      # [ 5�      $ 数据库用户名��5�      $ False��5�      $ 	Collapsed=�5�      $��0q�5�      #��[	 5�      ��5�      �� *DbCfg.UserId+$ PropertyChanged=�5�      5�      �� 5�      $��0,0,0,24q�5�      $ 8q�5�      #��[ 5�      $ 0��5�      y�5�      �� *
DbCfg.Type+# [ $ mysql|mssql��._�5�      ]� .+�5�      G� $
��12,0q�5�      $��Center=�5�      # [ 5�      $ 数据库密码��5�      $ False��5�      $ 	Collapsed=�5�      $��0q�5�       #
 MaterialDesignRevealPasswordBox#��[
 5�      0
  *MaterialDesignThemes.Wpf.PasswordBoxAssist 
  Password 5�      �� *DbCfg.Password+$ PropertyChanged=�5�      5�      5�      �� 5�      $��Right=�5�      $��直接点击保存��5�      #��[  5�       ! MaterialDesignFlatLightButton#��[ 5�      ��5�        *TestConnect+{�5�      �� *	Loading+  boolReverseConverter# [ 5�      �� 5�      $��8q�5�       	 AJRed#��[ 5�       �� TextAlignment$ Center=�5�      $
��Wrap=�5�      ��5�      �� *	ErrorInfo+5�      �� 5�      $��0,12,0,0q�5�      3  -MaterialDesignThemes.Wpf.ButtonProgressAssist   IsIndicatorVisible$
 True��5�          Value$  -1Q�5�        PrimaryHueDarkBrush#��[ 5�      $��保存��5�      ! � Cursor$
! Hand��5�      " �� 	IsDefault$
" True��5�      #   IsIndeterminate# 5�      �� *	Loading+��5�        *Save+5�      5�      �� 5�      $
��24,8q�5�      $��Right=�5�      $ �� Orientation$$ 
Horizontal=�5�      .+�5�      �� ��6   �� *
ActiveInfo+�� 5�      $��版本号：��6   �� 5�      ��6   �� *VersionName+5�      5�      5�      5�      