   M S B A M L   `   `   `  ����G  CSimpleClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:SimpleClientSimpleClient  5   	       SimpleClient.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:SimpleClient     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      � .+�5
      �� -   5        $��FirstNameBox��5
      $
��Left=�6    $��23��6;   $��184,103,0,0q�6G    �� TextWrapping$
 Wrap=�6\   $��	FirstName��6p   $	��Top=�6�   $	��400��6�   {�.  6�   �� 5      -   5        $��LastNameBox��5      $
��Left=�6   $��23��6:   $��184,139,0,0q�6F   $
 Wrap=�6[   $��LastName��6o   $	��Top=�6   $	��400��6�   {�.  6�   �� 5      -   5        $��	AcctNoBox��5      $
��Left=�6   $��23��68   $��184,175,0,0q�6D   $
 Wrap=�6Y   $��AcctNo��6m   $	��Top=�6{   $	��400��6�   {�.  6�   �� 5      -   5        $��PinBox��5      $
��Left=�6   $��23��65   $��184,212,0,0q�6A   $
 Wrap=�6V   $	��Pin��6j   $	��Top=�6u   $	��400��6�   {�.  6�   �� 5      -   5        $��
BalanceBox��5      $
��Left=�6   $��23��69   $��184,248,0,0q�6E   $
 Wrap=�6Z   $��Balance��6n   $	��Top=�6}   $	��400��6�   {�.  6�   �� 5      ���	#FFF4F4F5$
��Left=�6   $	��100��6:   $��
10,314,0,0q�6G   
���   �$	��Top=�6j   $	��772��6�   �� 5      -   5        $��IndexBox��5      $
��Left=�6   $��23��67   $��
17,323,0,0q�6C   $
 Wrap=�6W   $��Enter Index��6k   $	��Top=�6~   $	��119��6�   �� 5      -   5        $��TotalItemsBox��5      $
��Left=�6#   $��
17,351,0,0q�6>   $
��Wrap=�6R   $��Total Items:��6f   $	��Top=�6z   $	��119��6�   $��55��6�   �� 5      -   5        $��GoButton��5      $��Go��6   $
��Left=�6(   $��698,322,0,0q�6C   $	��Top=�6X   $��75��6p   6{   �� 5      $
��Left=�6   $��
184,32,0,0q�6)   $
��Wrap=�6=   $��Database Entry Details��6Q   $	��Top=�6o   $��49��6�   $	��400��6�   $��24(�6�   $
��Bold"�6�    �� TextAlignment$ Center=�6�   �� 5      -	   5        $��ErrorBackground��5      $y�Hidden=�6%   ���	#CCFF0000$
��Left=�6J   $	��399��6e   $��	10,10,0,0q�6r   
���   �$	��Top=�6�   $	��772��6�   $��2��6�   $��2��6�   �� 5      -
   5        $��ErrorOccuredTitle��5      $y�Hidden=�6'   $
��Left=�6;   $��	17,17,0,0q�6V   $
��Wrap=�6i   $��An Error Has Occured��6}   $	��Top=�6�   $��71��6�   $	��756��6�   $
��Bold"�6�   $��48(�6�   $��2��6�   $��2��6�   $ Center=�6  
�������6%  �� 5      -   5        $��ErrorDetailsTitle��5      $y�Hidden=�6'   $
��Left=�6;   $��	17,79,0,0q�6V   $
��Wrap=�6i   $��Error Details��6}   $	��Top=�6�   $��71��6�   $	��756��6�   $
��Bold"�6�   $��22(�6�   $ Center=�6�   
�������6�   �� 5      -   5        $��ErrorDetailsText��5      $y�Hidden=�6'   $
��Left=�6;   $��
17,120,0,0q�6V   $
��Wrap=�6j   $	��Top=�6~   $��71��6�   $	��756��6�   $
��Bold"�6�   
���   �$��Details:��6�   �� 5      -   5        $��ErrorAcceptButton��5      $y�Hidden=�6$   $��Ok��68   $
��Left=�6E   $��649,378,0,0q�6`   $	��Top=�6u   $	��129��6�   $��18(�6�    J� RenderTransformOrigin$ 0.699,0.812&�6�   6�   5      5      