   M S B A M L   `   `   `  ����  Project LENA - WPF#    Project_LENA___WPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  
projWindow��5   	    � Title$ Project LENA - WPF��5   	   $	��650��6$   $	��700��61   ���	#FFE5E5E5$	��520��6T    � Icon$ favicon.ico��6c   $��Disabled=�6v   $��Right=�6�   $
��1024��6�    � TaskbarItemInfo 5      *  $System.Windows.Shell.TaskbarItemInfo  5   
   .��5      � .+�5	   
   �� -   5        $��5q�5	      .@�5
      �� -   5         ��Name$ Noise_Generation��5
      $��Noise Generation��60   .��5      � 
������� � ColumnDefinitions 5      �� 5      .+�5      �� $��Gaussian Noise Generation��6   $��
10,10,10,0q�6B   $	��Top=�6V   .��5      �  5      �� 5   "   $
��125* �63   �� 5   "   $
��195* �63   �� 5   "   $
��274* �63    � RowDefinitions 5      �� 5   "   .+�5      �� $��Image:��6$   $
��Left=�65   $��	10,10,0,0q�6P   $	��Top=�6c   $��26��6{   $��46��6�    � ToolTip$q kThe color or grayscale Y Tiff image to be corrupted by Gaussian noise.
Drag and drop function is supported.��6�   �� 5      $��Noise:��6$   $
��Left=�65   $��
10,41,0,10q�6P   $	��Top=�6d   $��26��6|   $��43��6�   $M GThe percentage of Gaussian noise to be applied to the image in decimal.��6�   �� 5      -   5         ��Name$ textBox1��5   &   $��23��68   $��61,13,122,0q�6D   $	��Top=�6Y   $��3��6q   $��2q�6�   	 J� 	AllowDrop	 . 6�   �� 5      -   5        $ textBox2��5   &   $��23��68   $��	61,44,0,0q�6D   $	��Top=�6W   $��2q�6o   �� 5      -   5        $��Load Image. . .��5   %   $��Right=�6?   $��	0,13,10,0q�6[   $	��Top=�6n   $
��10,2q�6�   $��2��6�   $ Load the grayscale image.��6�   �� 5      -   5        
 ��Name$
 	checkBox1��5   '   $!��Add Gaussian noise to image��6:   $��Center=�6`   $��	10,47,0,0q�6}   $	��Top=�6�    d� 	IsChecked$
 TrueY�6�   $��1��6�   $` ZCheck to add the Gaussian noise to the image.
Uncheck to just generate the Gaussian noise.��6�   �� 5      -   5        $
 	checkBox2��5   '   $,��&Generate grayscale noise (color image)��6:   $��Center=�6k   $��3,48,0,0q�6�   $	��Top=�6�   $��2��6�   $� {Check to apply a grayscale only Gausian noise to a color image.
Uncheck to apply a color Gaussian noise to the color image.��6�   5       5!      �� 5"      -	   5        $
 	checkBox3��5"      $��Create grayscale image��62   $
��Left=�6S   $��
10,128,0,0q�6n   $	��Top=�6�   $@ :Convert a color image to a grayscale version of the image.��6�   �� 5#      -
   5        $��Create Gaussian Image��5#      $��Right=�6=   $��
0,124,10,0q�6Y   $	��Top=�6m   $��Center=�6�   $
��10,2q�6�   $4 .Generates the noise based on the size inputed.��6�   �� 5$      $��GroupBox��6   $��10,154,10,0q�61   $	��Top=�6F   $��75��6^   .��5%      � .+�5&      �� $��Color Image:��6$   $
��Left=�6;   $��	10,10,0,0q�6V   $	��Top=�6i   $_ YThe color RGB Tiff image to be reduced to grayscale.
Drag and drop function is supported.��6�   �� 5'      -   5        $ textBox3��5'   &   $��92,13,122,0q�68   $	��Top=�6M   $��23��6e   $��2q�6q   6�   �� 5(      -   5        $��Load Image. . .��5(   %   $��Right=�6?   $��	0,13,10,0q�6[   $	��Top=�6n   $
��10,2q�6�   $C =Load a color TIFF image to be processed to a grayscale image.��6�   5)      5*      �� 5+      -   5        $��Create Grayscale Image��5+      $��Right=�6>   $��
0,240,10,0q�6Z   $	��Top=�6n   $��Center=�6�   $
��10,2q�6�   $$ Generates the grayscale image.��6�   5,      5-      �� 5.      -   5        $ Sample_Generation��5.      $��Sample Generation��61   .��5/      � 
������� 50      �� 51      $
��156* �6+   �� 52      $
��157* �6+   .+�54      �� $��Load Images��6   $��
10,10,10,0q�64   $	��Top=�6H   $��2��6`   $	��100��6t   .��55      � .+�56      �� $��Clean Image:��6$   $
��Left=�6;   $��
10,10,0,10q�6V   $	��Top=�6j   $I CThe clean image to be sampled.
Drag and drop function is supported.��6�   �� 57      $��Noisy Image:��6$   $
��Left=�6;   $��
10,41,0,10q�6V   $	��Top=�6j   $K EThe noisy image to be processed.
Drag and drop function is supported.��6�   �� 58      -   5        $ textBox4��58   &   $��23��68   $��93,13,122,0q�6D   $	��Top=�6Y   $��2q�6q   6�   �� 59      -   5        $ textBox5��59   &   $��23��68   $��93,44,122,0q�6D   $	��Top=�6Y   $��2q�6q   6�   �� 5:      -   5        $��Load Image. . .��5:   %   $��Right=�6?   $��	0,13,10,0q�6[   $	��Top=�6n   $
��10,2q�6�   $# Load a clean grayscale image.��6�   �� 5;      -   5        $��Load Image. . .��5;   %   $��Right=�6?   $��	0,44,10,0q�6[   $	��Top=�6n   $
��10,2q�6�   $# Load a noisy grayscale image.��6�   5<      5=      �� 5>      -   5        $&�� Create Fragment From Images. . .��5>      $��Right=�6H   $��
0,124,10,0q�6d   $	��Top=�6x   $
��10,2q�6�   $��1��6�   {�.  $] WCrop the clean and noisy image inputed to gather samples from specific region of image.��6�   �� 5?      $��Kernel Size��6   $	��100��64   $��
10,154,5,0q�6A   $	��Top=�6U   .��5@      � .+�5A      �� -   5         ��Name$ 	comboBox1��5A   '   $��10,13,150,0q�6:   $	��Top=�6O    �� 
IsEditable . {�.  $= 7Size of the kernel surrounding the pixel being sampled.��6�   .@�5B   "   �� $��3 x 3��6/   �� 5C   "   $��5 x 5��6/   �� 5D   "   $��7 x 7��6/   5E      �� 5F      -   5        $ 	comboBox2��5F   '   $��10,44,150,0q�6:   $	��Top=�6O    . {�.  $&  Size of the patch to be sampled.��6�   .@�5G   "   �� $��11 x 11��6/   �� 5H   "   $��13 x 13��6/   �� 5I   "   $��15 x 15��6/   5J      	� 5K      -   5         	�Name$ radioButton1��5K   *   $��Process Using Pixels��6@   $��Right=�6_   $��	0,16,19,0q�6{   $	��Top=�6�   $v pCreate the samples based on a pixel and surrounding kernel.
If explicitly defined, one dimension may be entered.��6�   	� 5L      -   5        $ radioButton2��5L   *   $��Process Using Patches��6@   $��Right=�6`   $��0,47,8,0q�6|   $	��Top=�6�   $d ^Create the samples based on a patch size.
If explicitly defined, one dimension may be entered.��6�   5M      5N      �� 5O      $��Samples��6   $��
5,154,10,0q�60   $	��Top=�6D   $��1��6\   $��75��6l   .��5P      � .+�5Q      �� -   5        $ 	comboBox3��5Q   '   $��142,13,20,0q�6:   $	��Top=�6O    . $4 .The number of pixels or patches to be sampled.��6y   .@�5R   "   �� $	��200��6/   �� 5S   "   $	��300��6/   �� 5T   "   $	��400��6/   5U      �� 5V      $��Number of Samples:��6$   $
��Left=�6A   $��	10,10,0,0q�6\   $	��Top=�6o   $4 .The number of pixels or patches to be sampled.��6�   5W      5X      �� 5Y      -   5         ��Name$ GenSample_Button��5Y      $��Generate Samples��67   $��Right=�6R   $��
0,240,10,0q�6n   $	��Top=�6�   $
��10,2q�6�   $��1��6�   $&  Start the generation of samples.��6�   5Z      5[      �� 5\      -   5        $ Learning_of_Weights��5\      $��Learning of Weights��63   .��5]      � 
�������$��0q�6*    5^      �� 5_      $
��127* �6+   �� 5`      $
��186* �6+   .+�5b      �� $��Inputs��6   $��
10,10,10,0q�6/   $	��Top=�6C   $	��100��6[   $��2��6h   .��5c      � .+�5d      �� $��Samples:��6$   $
��Left=�67   $��	10,10,0,0q�6R   $	��Top=�6e   $^ XThe samples generated to be used to create weights.
Drag and drop function is supported.��6}   �� 5e      $��Initial Weights:��6$   $
��Left=�6?   $��
10,41,0,10q�6Z   $	��Top=�6n   $� �Use weights that have already been created, or generate random weights.
Drag and drop function is supported if using existing weights.��6�   �� 5f      -   5        $ textBox6��5f   &   $��23��68   $��72,13,133,0q�6D   $	��Top=�6Y   $��2q�6q   6�   �� 5g      -   5        $ textBox7��5g   &   $��23��68   $��237,44,133,0q�6D   $	��Top=�6Z   $��2q�6r   {�.  6�   �� 5h      -   5        $��Load Samples. . .��5h   %   $��Right=�6A   $��	0,13,10,0q�6]   $	��Top=�6p   $
��10,2q�6�   $" Load the samples to be used.��6�   �� 5i      -   5        $ button1��5i   %   $��Load Weights. . .��66   $��Right=�6R   $��	0,44,10,0q�6n   $	��Top=�6�   $
��10,2q�6�   {�.  $ Load existing weights.��6�   �� 5j      -   5        $ 	comboBox4��5j   '   $��
103,44,0,0q�6:   $	��Top=�6N   $
��Left=�6f   $	��120��6�   $M GUse weights that have already been created,
or generate random weights.��6�   .@�5k   "   �� $��Random��6/   �� 5l   "   $��Existing��6/   5m      5n      5o      �� 5p      $��Stopping Criteria��6   $��
10,220,5,0q�6:   $��70��6N   $	��Top=�6Z   .��5q      �  5r      �� 5s   "   .+�5u      �� -    5        $
 	checkBox4��5u   '   $��Angular RMSE��6:   $��Right=�6Q   $	��Top=�6m   $��	0,16,10,0q�6�   $��Right=�6�   �� 5w      -!   5        $ 	comboBox6��5w   '   $��10,13,119,0q�6:   $	��Top=�6O   $4 .The method to be used to reach a final result.��6�   .@�5x   "   �� $	��MAX��6/   �� 5y   "   $��
Error Rate��6/   �� 5z   "   $
��RMSE��6/   �� 5{   "   $	��MSE��6/   5|      5}      5~      �� 5      $��
Thresholds��6   $��
5,220,10,0q�63   $	��Top=�6G   $��1��6_   $��70��6o   .��5�      �  5�      �� 5�   "   $
��169* �63   �� 5�   "   $
��171* �63   .+�5�      �� $��Local Threshold:��6$   $
��Left=�6?   $��	10,10,0,0q�6Z   $	��Top=�6m   $j dThe threshold value to be used to calculate the stopping point
to a given criteria in a local scope.��6�   �� 5�      $��Global Threshold:��6$   $
��Left=�6@   $��	13,10,0,0q�6[   $	��Top=�6n    J� RenderTransformOrigin$ 0.903,0.885&�6�   $��1��6�   $x rThe threshold value to be used to calculate the stopping point
to a given criteria througout the entire algorithm.��6�   �� 5�      -"   5        $ 	textBox12��5�   &   $��23��69   $��111,13,13,0q�6E   $	��Top=�6Z   $��2q�6r   �� 5�      -#   5        $ 	textBox13��5�   &   $��23��69   $��121,13,10,0q�6E   $	��Top=�6Z   $��1��6r   $��2q�6�   5�      5�      �� 5�      -$   5        $��Load Parameters. . .��5�      $
��Left=�6<   $��
10,301,0,0q�6W   $	��Top=�6k   $
��10,2q�6�   $/ )Load existing parameter onto the program.��6�   �� 5�      -%   5        $��Save Parameters. . .��5�      $
��Left=�6<   $��140,301,0,0q�6W   $	��Top=�6l   $
��10,2q�6�   $��2��6�   $$ Save the parameters as a file.��6�   �� 5�      -&   5        $ Test_Button��5�      $
��Test��62   $��Right=�6A   $��
0,301,10,0q�6]   $	��Top=�6q   $
��30,2q�6�   $��1��6�   $3 -Test existing weights by checking their RMSE.��6�   �� 5�      -'   5        $ Learn_Button��5�      $��Learn��63   $��Right=�6C   $��
0,301,99,0q�6_   $	��Top=�6s   $
��30,2q�6�   $��1��6�   $$ Begin the learning of weights.��6�   �� 5�      -(   5        $ Console1��5�      $	��200��60   $��10,335,10,0q�6=    �� TextWrapping$
 Wrap=�6R   $	��Top=�6f   	 .  ��.  ��. " �� VerticalScrollBarVisibility$
 Auto=�6�   $��2��6�   
���   � �� 
IsReadOnly . 
�������$��Consolas)�66  �� 5�      -)   5        $ button4��5�      $$��Generate Image Parameters. . .��6.   $
��Left=�6W   $��
10,549,0,0q�6r   $	��Top=�6�   $
��10,2q�6�   {�.  $C =Create the parameters to be used for the processing of image.��6�   �� 5�      -*   5        $ button3��5�      $��Cancel��6.   $��Right=�6?   $��
0,549,10,0q�6[   $	��Top=�6o   $
��30,2q�6�   $��1��6�   {�.  $$ Abort the learning of weights.��6�   �� 5�      $��Configuration��6   $��10,115,10,0q�66   $	��Top=�6K   $	��100��6c   $��2��6p   .��5�      �  5�      �� 5�   "    5�      �� 5�   "   $
��206* �63   �� 5�   "   $
��199* �63   �� 5�   "   $
��189* �63   .+�5�      �� $��Network Size:��6$   $
��Left=�6<   $��	10,10,0,0q�6W   $	��Top=�6j   $; 5The network array to be used to generate the weights.��6�   �� 5�      $��Output:��6$   $��	10,10,0,0q�66   $	��Top=�6I   $
��Left=�6a   $��1��6|   $W QUsed to determine wether the output will result
in a discrete or continuous form.��6�   �� 5�      -+   5        $ textBox8��5�   &   $��23��68   $��	97,13,1,0q�6D   $	��Top=�6W   $��2q�6o   �� 5�      -,   5        $ textBox9��5�   &   $��23��68   $��107,13,10,0q�6D   $	��Top=�6Y   $��2q�6q   $��2��6}   �� 5�      --   5        $ 	comboBox5��5�   '   $��
65,13,11,0q�6:   $	��Top=�6N   $��1��6f   .@�5�   "   �� $��Discrete��6/   �� 5�   "   $��
Continuous��6/   5�      �� 5�      $��Output Neurons:��6$   $
��Left=�6>   $��3,10,0,0q�6Y   $	��Top=�6k   $��2��6�   $Y SThe number of neurons in the output layer.
Used to calculate the inputs per sample.��6�   �� 5�      $��Samples in Learning:��6$   $
��Left=�6C   $��	10,41,0,0q�6^   $	��Top=�6q   $� }The number of samples to be used to generate the weights.
User may input a lower number of samples than the number existing. ��6�   �� 5�      -.   5        $ 	textBox10��5�   &   $��23��69   $��134,44,15,0q�6E   $	��Top=�6Z   �� 5�      $��Number of Sectors:��6$   $��	10,44,0,0q�6A   $	��Top=�6T   $
��Left=�6l   $��1��6�   $t nThe number of sectors to be processed from the unit circle.
Used for classification in the learning algorithm.��6�   �� 5�      -/   5        $ 	textBox11��5�   &   $��23��69   $��127,44,11,0q�6E   $	��Top=�6Z   $��2q�6r   $��1��6~   5�      5�      d� 5�      -0   5         d�Name$ button2��5�   #   $��Pause��64   $��1��6D   $��Right=�6T   $��0,549,112,0q�6p   $	��Top=�6�   $
��30,2q�6�   {�.  $$ Pause the learning of weights.��6   5�      5�      �� 5�      -1   5        $ Processing_Image��5�      $��Processing Image��60   .��5�      � 
������� 5�      �� 5�      $
��156* �6+   �� 5�      $
��157* �6+   .+�5�      �� $��Load Images��6   $��
10,10,10,0q�64   $	��Top=�6H   $	��100��6`   $��2��6m   .��5�      � .+�5�      �� $��Noisy Image:��6$   $
��Left=�6;   $��
10,10,0,10q�6V   $	��Top=�6j   $J DThe grayscale image with noise.
Drag and drop function is supported.��6�   �� 5�      $��Weights:��6$   $
��Left=�67   $��
10,41,0,10q�6R   $	��Top=�6f   $T NThe file to be used to process the image.
Drag and drop function is supported.��6~   �� 5�      -2   5        $ 	textBox14��5�   &   $��23��69   $��94,13,132,0q�6E   $	��Top=�6Z   $��2q�6r   6�   �� 5�      -3   5        $ 	textBox15��5�   &   $��23��69   $��93,44,133,0q�6E   $	��Top=�6Z   $��2q�6r   6�   �� 5�      -4   5        $��Load Image. . .��5�   %   $��Right=�6?   $��	0,13,10,0q�6[   $	��Top=�6n   $
��10,2q�6�   $	��109��6�   $+ %Load the noisy image to be processed.��6�   �� 5�      -5   5        $��Load Weights. . .��5�   %   $��Right=�6A   $��	0,44,10,0q�6]   $	��Top=�6p   $
��10,2q�6�   ��. $7 1Load the weights used to process the noisy image.��6�   5�      5�      �� 5�      $��
Parameters��6   $��10,144,10,0q�63   $	��Top=�6H   $	��100��6`   $��2��6m   .��5�      �  5�      �� 5�   "   $
��202* �63   �� 5�   "   $	��16* �63   �� 5�   "   $
��191* �63   �� 5�   "   $
��185* �63   .+�5�      �� -6   5         ��Name$ label1��5�   $   $��Method:��64   $
��Left=�6F   $��	10,10,0,0q�6a   $	��Top=�6t   �� 5�      -7   5        $ 	textBox16��5�   &   $��23��69   $��104,13,29,0q�6E   $	��Top=�6Z   $��2��6r   �� 5�      -8   5        $ label2��5�   $   $��Number of Sectors:��64   $
��Left=�6Q   $��3,10,0,0q�6l   $	��Top=�6~   $��2��6�   $��1��6�   �� 5�      -9   5        $ 	textBox17��5�   &   $��23��69   $��36,13,102,0q�6E   $	��Top=�6Z   $��3��6r   �� 5�      -:   5        $ label3��5�   $   $��Step:��64   $
��Left=�6D   $��
186,10,0,0q�6_   $	��Top=�6s   $��2��6�   $��2��6�   �� 5�      -;   5        $ 	textBox18��5�   &   $��23��69   $��	97,44,0,0q�6E   $	��Top=�6X   $��2��6p   �� 5�      -<   5        $ label4��5�   $   $��Network Size:��64   $
��Left=�6L   $��	10,41,0,0q�6g   $	��Top=�6z   �� 5�      -=   5        $ 	comboBox7��5�   '   $��	70,13,0,0q�6:   $	��Top=�6M    . 6e   �� 5�      ->   5        $ label5��5�   $   $��Output Neurons:��64   $
��Left=�6N   $��	17,41,0,0q�6i   $	��Top=�6|   $��2��6�   �� 5�      -?   5        $ 	textBox19��5�   &   $��23��69   $��
121,44,4,0q�6E   $	��Top=�6Y   $��2��6q   5�      5�      �� 5�      -@   5        $��Load Parameters. . .��5�      $
��Left=�6<   $��
10,258,0,0q�6W   $	��Top=�6k   $
��10,2q�6�   $/ )Load existing parameter onto the program.��6�   �� 5�      -A   5        $��Save Parameters. . .��5�      $
��Left=�6<   $��140,258,0,0q�6W   $	��Top=�6l   $
��10,2q�6�   $$ Save the parameters as a file.��6�   �� 5�      -B   5        $ Process_Button��5�      $��Process Image��65   $��Right=�6M   $��
0,258,10,0q�6i   $	��Top=�6}   $
��10,2q�6�    � IsSharedSizeScope . $��1��6�   $( "Start the processing of the image.��6�   	� 5�      -C   5        $ radioButton3��5�   "   $��Process Using Pixels��68   $��Right=�6W   $��
0,124,10,0q�6s   $	��Top=�6�   $8 2Process the image with kernel surrounding a pixel.��6�   	� 5�      -D   5        $ radioButton4��5�   "   $��Process Using Patches��68   $
��Left=�6X   $��
10,124,0,0q�6s   $	��Top=�6�   $��1��6�   $+ %Process the image by group of pixels.��6�   �� 5�      -E   5        $ Console2��5�      $	��200��60   $��10,294,10,0q�6=   $
 Wrap=�6R   $	��Top=�6f   	 .  |�.  ��.  ��. $
 Auto=�6�   $��2��6�   
���   �
�������$��Consolas)�66  �� 5�      -F   5        $ button6��5�      $��Cancel��6.   $��Right=�6?   $��
0,544,10,0q�6[   $	��Top=�6o   $
��30,2q�6�   $��1��6�   {�.  $# Abort the current opperation.��6�   � 5�      -G   5         �Name$ progressBar1��5�   "   $��22��68   $��10,508,10,0q�6D   $	��Top=�6Y   $��2��6q   d� 5�      -H   5        $ button5��5�   #   $��Pause��64   $��1��6D   $��Right=�6T   $��0,544,112,0q�6p   $	��Top=�6�   $
��30,2q�6�   {�.  $# Pause the current opperation.��6  5�      5�      �� 5�      -I   5        $ About��5�      $��About��6%    J� IsHitTestVisible . $y�Hidden=�6L   .��5�      � ���	#FFE5E5E5.+�5�      � $��0q�6   
������� 5�      �� 5�      $
��228* �6/   �� 5�      $	��85* �6/   .+�5�      �� $��Project LENA��6    $
��Left=�67   $��
285,10,0,0q�6R   $	��Top=�6f   
���   �$��Tahoma)�6�   $��30(�6�   $��2��6�   �� 5�      $
��Left=�6    $	��256��6;   $��	14,37,0,0q�6H   $	��Top=�6[   $	��256��6s   $��Lena.bmp��6   $��UniformToFill=�6�   �� 5�      $��Version 0.8.9.6��6    $
��Left=�6:   $��
288,47,0,0q�6U   $	��Top=�6i   $��Tahoma)�6�   �� 5�      $
��Wrap=�6$   $��
285,71,0,0q�68   $	��249��6L   $	��Top=�6Y   $
��Left=�6q    �� Padding$ 5q�6�   $��2��6�   �� 6�   $���This program uses an intelligent approach to image processing using MLMVN, otherwise known as Multilayer feedforward neural network based on multi-valued neurons.��6�   �� 6]  �� 6i  �� 6o  �� 6{  $���The user may create and generate an image suitable to be filtered, generate samples used to create weights used to process images, generate the weights learned from this process, and filter a noisy image from the weights created. ��6  �� 6o  �� 6{  �� 6�  �� 6�  $e�_Additional help can be provided by hovering the cursor over a label of an element of interest. ��6�  �� 6�  �� 6  �� 6  �� 6  $���Developed by Mr. Plumlee and Mr. Del Pino. We would like to thank Dr. Aizenberg and Dr. Laddomada for their support to help us make this program a reality.��6  6�  5�      5�      5�      5�      � 5�   
   $��Right=�6   $��27��6+   $��0,0,10,0q�67   $	��Top=�6I   $��30��6a   .+�5�      �� -J   5        $ 
InfoButton��5�      $��i��6)   $
��Left=�65   $	��Top=�6P   $��Monotype Corsiva)�6h      InfoButtonStyle1#���   6�   $��17.333(�6�   $��18��6�   $��18��6�   
�������$��7,5,0,0q�6  $ About��6  5�      5�      5�      