   M S B A M L   `   `   `  ����O  KTransactionGenerator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:TransactionGeneratorTransactionGenerator  5   	   %    TransactionGenerator.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:TransactionGenerator     5   	     � Title$   SimpleTransactionGenerator��5   	   $	��450��6,   $	��800��69   .��5	      � .+�5
      �� -   5         ��Name$ BlockchainBlocks��5
      $
��Left=�6&   $	��340��6A   $��	10,32,0,0q�6N   $	��Top=�6a   $	��229��6y   �� 5      $��Generated Blocks��6
   $
��Left=�6%   $��	52,10,0,0q�6@   $	��Top=�6S   $	��128��6k   $��14(�6w   $
��Bold"�6�   �� 5      -   5        $��UpdateBlocksButton��5      $��Check For New Blocks��6&   $
��Left=�6E   $��
10,387,0,0q�6`   $	��Top=�6t   $	��126��6�   6�   � 5      -   5        $��UpdateBlocksProgress��5       � IsIndeterminate .  $
��Left=�6D   $��10��6_   $��
10,375,0,0q�6k   $	��Top=�6   $	��229��6�   �� 5      -   5         ��Name$ AccountNumberInput��5      $
��Left=�6(   $��19��6C   $��
563,32,0,0q�6O    �� TextWrapping$
 Wrap=�6c   $��Enter Account Number��6w   $	��Top=�6�   $	��130��6�   �� 5      -   5        $��CheckBalanceButton��5      $��Check Balance��6%   $
��Left=�6=   $��
698,32,0,0q�6X   $	��Top=�6l   $��84��6�   $��19��6�   6�   �� 5      -   5        $��BalanceDisplay��5      $
��Left=�6"   $��18��6=   $��
563,56,0,0q�6I   $
 Wrap=�6]   $�� ��6q   $	��Top=�6y   $	��219��6�    �� 
IsReadOnly . 6�   � 5      -   5        $��BalanceProgress��5       .  $
��Left=�6?   $��10��6Z   $��
563,79,0,0q�6f   $	��Top=�6z   $	��219��6�   �� 5      $
��Left=�6
   $	��100��6%   $��
367,10,0,0q�62   $	��Top=�6F   $	��100��6^   $��Capture.PNG��6j   �� 5      $��	TeggyCoin��6
   $
��Left=�6   $��381,104,0,0q�69   $	��Top=�6N   $��71��6f   $
��Bold"�6q   �� 5      $��New Transaction��6
   $
��Left=�6$   $��448,275,0,0q�6?   $	��Top=�6T   $
��Bold"�6l   �� 5      -   5        $��FromWalletInput��5      $
��Left=�6#   $��23��6>   $��448,306,0,0q�6J   $
 Wrap=�6_   $��Enter From Wallet��6s   $	��Top=�6�   $	��156��6�   �� 5      -	   5        $��ToWalletInput��5      $
��Left=�6!   $��23��6<   $��615,306,0,0q�6H   $
 Wrap=�6]   $��Enter To Wallet��6q   $	��Top=�6�   $	��156��6�   �� 5      -
   5        $��AmountInput��5      $
��Left=�6   $��23��6:   $��448,334,0,0q�6F   $
 Wrap=�6[   $��Enter Amount��6o   $	��Top=�6�   $	��323��6�   �� 5      -   5        $��SendTransButton��5      $��
Send Trans��6"   $
��Left=�67   $��448,362,0,0q�6R   $	��Top=�6g   $��75��6   6�   � 5      -   5        $��TransBar��5      $
��Left=�6    $��18��6;   $��528,362,0,0q�6G   $	��Top=�6\   $	��243��6t   5      5      