   M S B A M L   `   `   `  ����F  B篮圈AVS1, Version=3.0.0.14, Culture=neutral, PublicKeyToken=null)clr-namespace:AWSV2.ViewsAWSV2.Views  5      S OMaterialDesignExtensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls 5	      ) %System.Windows.Controls.Input.ToolkitpTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls 5      3clr-namespace:AWSV2.ViewModelsAWSV2.ViewModels  5      $    AWSV2.Views.WeighingRecordView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      AControls,http://metro.mahapps.com/winfx/xaml/controls     5      C	iconPacks-http://metro.mahapps.com/winfx/xaml/iconpacks     5      +localclr-namespace:AWSV2.Views     5      _	 [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962a:md1http://materialdesigninxaml.net/winfx/xaml/themes 	 5      [mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions 	 5	      A
 =Stylet, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null)s!https://github.com/canton7/Stylet 
 5
      btoolkitTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit 
 5      'vmclr-namespace:AWSV2.ViewModels 
 5      $	��480��5      $	��640��5         MaterialDesignPaper#��[  5      6  0MaterialDesignExtensions.Controls.MaterialWindow    BorderBackgroundBrush#  [  5        MaterialDesignBody   BorderForegroundBrush# [ 5       � SizeToContent$ Height=�5      $��Microsoft YaHei)�5      $��16(�5      $��Medium"�5      #���  5       � WindowStyle$ 
ToolWindow=�5       � Title 5      �� *Title+ �� FallbackValue$ 新增/修改称重记录�� � 	Resources 5       
 combox- 	 'MaterialDesignThemes.Wpf.ComboBoxAssist   ShowSelectedItem. 	 (MaterialDesignThemes.Wpf.TextFieldAssist   DecorationVisibility	 �� DisplayMemberPath) 	 #MaterialDesignThemes.Wpf.HintAssist
   Hint �� 
IsEditable  boolReverseConverter �� 	Converter �� SelectedValue �� UpdateSourceTrigger �� Text  date �� 
IsReadOnly �� StringFormat  MaterialDesignFlatButton! 
 Stylet.Xaml.ActionExtension' 	 !MaterialDesignThemes.Wpf.PackIcon   Kind 	 input   
SuffixText  inputNumber  decimalToStringConverter/   )AWSV2.ViewModels.FormItemTemplateSelector 	 templateSelector   Combox   Date   InputNumber   TextBox��%�  &	       7  7  &	 m    7  7  7  7  &	 e    7  7  &	 @    7  7  7  &		      7  7  7  7  �� 5   
   .R�5      �� $
��8,12q�5      $
 True��5      $ 	Collapsed=�5       8��  $	 Label��5"      
 5      �� *Label+ 5#      �� *
Editable+{�5$      �� *
Disabled+8  ��5%      �� *	Options+ 5&      �� *SelectedOption+$ PropertyChanged=� 5'      �� *Value+$ PropertyChanged=�5(      �� 5)   
   .R�5*      � $
��8,12q�6   .+�5+      �� 8��  
 5,      �� *Label+ 5.      �� *IsFilterDisplay+8  ��5/      �� *Value+$ PropertyChanged=�$ {0:yyyy-MM-dd HH:mm:ss}���� 50      $��0,0,2,0q�51      $��Right=�52      8�� 8�� ��53        *GenerateNow+��54      �� *Field+{�56      �� *IsFilterDisplay+.��58        $��24��59      $��24��5:      $ ClockTimeFourOutline=�5;      5<      5=      5>      �� 5?   
   .R�5@      �� $
��8,12q�5A      8��  
 5B      �� *Label+ 5C      �� *
SuffixText+ 5E      �� *IsFilterDisplay+8  ��5F      �� *Value+$ PropertyChanged=�5G      �� 5H   
   .R�5I      �� $
��8,12q�5J      8��  
 5K      �� *Label+ 5L      �� *
SuffixText+ 5N      �� *IsFilterDisplay+8  ��5O      �� *Value+$ PropertyChanged=�8  5P        5Q   
   8   8  8  8  5V      5W      .��5X      �  � RowDefinitions 5Y   
   �� 5Z      �� 5[      $��64 �6   .+�5]   
   �� -   5         ��Name$ scrolls��5^      $	��600��5_      $
��Auto=�5`      .��5a      �� #��[	 6   ��6T   �� *DyanmicPoundFieldItems+��5b      �� 5c      .R�5d      �� 5e      5g      5h      �� 5i   
   $��1��5j      $��8q�5k      $��保存��5m      ��5l        *
SaveItem+5m      5n      5p      