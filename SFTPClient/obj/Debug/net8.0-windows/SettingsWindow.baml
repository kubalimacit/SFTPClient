   M S B A M L   `   `   `  ����T  PMahApps.Metro, Version=2.0.0.0, Culture=neutral, PublicKeyToken=51482d6f650b2b3fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  5       
SFTPClient   SFTPClient.SettingsWindow   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      6mah,http://metro.mahapps.com/winfx/xaml/controls   5      JControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro   5        � Title$  Settings��5         ThemeColorBrush(   "MahApps.Metro.Controls.MetroWindow   WindowTitleBrush# �   6#    � WindowStartupLocation$ CenterScreen=�6X   $	��400��6}   $	��600��6�    � 
ResizeMode$ NoResize=�6�   .��5      �� .@�5
   
   �� ��5      �� 5      .R�5      �� $��General��6    $��20(�6/    � Cursor$
 Hand��6=   #���   6K   5      ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � �� �� Value
 �����63   �� 5       � ��# �   63    �� Triggers 5      P� 5      	 P� Property	 �@��
IsSelected
 P� Value$

 True��68   . �5      ��  � ��# �   6;   �� 5       � ��
 �����6;   5      5      .��5      �� $��10q�6   $  ��6%   .+�5      �� -   5         ��Name$! SearchBoxVisibilityCheckBox��5        TextColorBrush#���  6@   $
 Hand��6n   $��Show Filter��6|    d� 	IsChecked$
 TrueY�6�   $��5q�6  �� 5      -   5        $ ShowBreadcrumbCheckBox��5      #���  6;   $
 Hand��6i   $��Show Breadcrumb��6w   $
 TrueY�6�   $��5q�6�   �� 5       -   5        $! ShowSessionComboBoxCheckBox��5       #���  6@   $��5q�6n   $
 Hand��6y   $��Show Sessions & Save��5!      $
 TrueY�6:   5#      �� 5%       �� Orientation$ 
Horizontal=�6   $
��10,5q�66   .+�5&      �� $��	Language:��6   #���  60   $��Center=�6^   $��0,0,10,0q�6y   �� 5'      -   5         ��Name$ LanguageComboBox��5'      #���  69   $	��150��6g   $
 Hand��6s   .@�5(      �� $��English��6'   
 � Tag$ en-US��69    �� 
IsSelected$
 True��6E   �� 5)      $��Türkçe��6'   $ tr-TR��69   ${�False��6E   �� 5*      $��Spanish��6'   $ sp��6:   ${�False��6C   �� 5+      $��Arabic��6'   $ ar��69   ${�False��6B   5,      5-      5.      5/      �� 52   
   ��53      �� 54      .R�55      �� $��Theme��6    $��20(�6-   $
 Hand��6;   #���   6I   56      ��58      �� 59      " ��6   .�5:      ��  � ��
 �����63   �� 5;       � ��# �   63    5<      P� 5=      	 �@��
IsSelected$

 True��68   . �5>      ��  � ��# �   6;   �� 5?       � ��
 �����6;   5@      5B      .��5D      �� $��10q�6   .+�5E      �� $��Select Theme Color:��6   $
��Bold"�67   $��16(�6I   #���  6W   $��0,10,0,0q�6�   (   "MahApps.Metro.Controls.ColorPicker  5F      -   5          Name$ ThemeColorPicker��5F   '   $
 Hand��6A   $
��Bold"�6O   $��14(�6a   6o   �� 5H      $��Select Text Color:��6   $
��Bold"�66   $��16(�6H   #���  6V   $��0,10,0,0q�6�     5I      -   5        $ TextColorPicker��5I   '   $
 Hand��6@   $
��Bold"�6N   $��14(�6`   6n   5J      5K      �� 5N   
   ��5O      �� 5P      .R�5Q      �� $��Sessions��6    $��20(�60   $
 Hand��6>   #���   6L   5R      ��5T      �� 5U      " ��6   .�5V      ��  � ��
 �����63   �� 5W       � ��# �   63    5X      P� 5Y      	 �@��
IsSelected$

 True��68   . �5Z      ��  � ��# �   6;   �� 5[       � ��
 �����6;   5\      5^      .��5`      �� .+�5a      �� $��Manage saved sessions:��6   $
��Bold"�6:   $��16(�6L   $��10q�6Z   #���  6f   �� 5c      $��2q�6   #���   6-    �� CornerRadius$ 3��6]   $��10q�6n   ._�5d      �� -   5         ��Name$ SessionListBox��5d      $
 Hand��66   $��5q�6D   #���   6O   ��5e      �� 5f      .R�5g   "   �� $��1q�6)   #���   6=   $ 1��6m    �� Padding$ 5q�6~   $��5q�6�   ._�5h   &   �� $ 
Horizontal=�61   .+�5i   *   �� $��14(�6E   $��Center=�6S   $	��5,0q�6n   #���  6{   ��64   �� 6{   5j   '   5k   #   5l      5n      5o      �� 5q      -   5         ��Name$ DeleteSessionButton��5q      $
 Hand��66   $��Delete Selected Session��6D   $��10q�6�   $	��200��6�   $��Center=�6�     MahApps.Styles.Button#���  5r   	   
�������#���   6L   5s      5u      5v      5w      