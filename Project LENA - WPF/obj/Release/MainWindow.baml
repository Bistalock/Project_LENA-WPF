   M S B A M L   `   `   `  ����   Project LENA - WPF#    Project_LENA___WPF.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     �Name$  
projWindow�� � Title$ Project LENA - WPF��$	��650��$	��700�����	#FFE5E5E5$	��530�� � Icon$ favicon.ico��$��Disabled=�$��Right=�$
��1024�� � TaskbarItemInfo *  $System.Windows.Shell.TaskbarItemInfo  .��� .+��� -   $��5q�.@��� -    ��Name$ Noise_Generation��$��Noise Generation��.��� 
������� � ColumnDefinitions �� .+��� $��Gaussian Noise Generation��$��
10,10,10,0q�$	��Top=�.���  �� $
��125* ��� $
��195* ��� $
��274* � � RowDefinitions �� .+��� $��Image:��$
��Left=�$��	10,10,0,0q�$	��Top=�$��26��$��46�� � ToolTip$q kThe color or grayscale Y Tiff image to be corrupted by Gaussian noise.
Drag and drop function is supported.���� $��Noise:��$
��Left=�$��
10,41,0,10q�$	��Top=�$��26��$��43��$M GThe percentage of Gaussian noise to be applied to the image in decimal.���� -    ��Name$ textBox1��$��23��$��61,13,122,0q�$	��Top=�$��3��$��2q�	 J� 	AllowDrop	 . �� -   $ textBox2��$��23��$��	61,44,0,0q�$	��Top=�$��2q��� -   $��Load Image. . .��$��Right=�$��	0,13,10,0q�$	��Top=�$
��10,2q�$��2��$ Load the grayscale image.���� -   
 ��Name$
 	checkBox1��$!��Add Gaussian noise to image��$��Center=�$��	10,47,0,0q�$	��Top=� d� 	IsChecked$
 TrueY�$��1��$` ZCheck to add the Gaussian noise to the image.
Uncheck to just generate the Gaussian noise.���� -   $
 	checkBox2��$,��&Generate grayscale noise (color image)��$��Center=�$��3,48,0,0q�$	��Top=�$��2��$� {Check to apply a grayscale only Gausian noise to a color image.
Uncheck to apply a color Gaussian noise to the color image.���� -	   $
 	checkBox3��$��Create grayscale image��$
��Left=�$��
10,128,0,0q�$	��Top=�$@ :Convert a color image to a grayscale version of the image.���� -
   $��Create Gaussian Image��$��Right=�$��
0,124,10,0q�$	��Top=�$��Center=�$
��10,2q�$4 .Generates the noise based on the size inputed.���� $��GroupBox��$��10,154,10,0q�$	��Top=�$��75��.��� .+��� $��Color Image:��$
��Left=�$��	10,10,0,0q�$	��Top=�$_ YThe color RGB Tiff image to be reduced to grayscale.
Drag and drop function is supported.���� -   $ textBox3��$��92,13,122,0q�$	��Top=�$��23��$��2q��� -   $��Load Image. . .��$��Right=�$��	0,13,10,0q�$	��Top=�$
��10,2q�$C =Load a color TIFF image to be processed to a grayscale image.���� -   $��Create Grayscale Image��$��Right=�$��
0,240,10,0q�$	��Top=�$��Center=�$
��10,2q�$$ Generates the grayscale image.���� -   $ Sample_Generation��$��Sample Generation��.��� 
������� �� $
��156* ��� $
��157* �.+��� $��Load Images��$��
10,10,10,0q�$	��Top=�$��2��$	��100��.��� .+��� $��Clean Image:��$
��Left=�$��
10,10,0,10q�$	��Top=�$I CThe clean image to be sampled.
Drag and drop function is supported.���� $��Noisy Image:��$
��Left=�$��
10,41,0,10q�$	��Top=�$K EThe noisy image to be processed.
Drag and drop function is supported.���� -   $ textBox4��$��23��$��93,13,122,0q�$	��Top=�$��2q��� -   $ textBox5��$��23��$��93,44,122,0q�$	��Top=�$��2q��� -   $��Load Image. . .��$��Right=�$��	0,13,10,0q�$	��Top=�$
��10,2q�$# Load a clean grayscale image.���� -   $��Load Image. . .��$��Right=�$��	0,44,10,0q�$	��Top=�$
��10,2q�$# Load a noisy grayscale image.���� -   $&�� Create Fragment From Images. . .��$��Right=�$��
0,124,10,0q�$	��Top=�$
��10,2q�$��1��$] WCrop the clean and noisy image inputed to gather samples from specific region of image.���� $��Kernel Size��$	��100��$��
10,154,5,0q�$	��Top=�.��� .+��� -    ��Name$ 	comboBox1��$��10,13,150,0q�$	��Top=� �� 
IsEditable . {�.  $= 7Size of the kernel surrounding the pixel being sampled.��.@��� $��3 x 3���� $��5 x 5���� $��7 x 7���� -   $ 	comboBox2��$��10,44,150,0q�$	��Top=� . {�.  $&  Size of the patch to be sampled.��.@��� $��11 x 11���� $��13 x 13���� $��15 x 15��	� -    	�Name$ radioButton1��$��Process Using Pixels��$��Right=�$��	0,16,19,0q�$	��Top=�$v pCreate the samples based on a pixel and surrounding kernel.
If explicitly defined, one dimension may be entered.��	� -   $ radioButton2��$��Process Using Patches��$��Right=�$��0,47,8,0q�$	��Top=�$d ^Create the samples based on a patch size.
If explicitly defined, one dimension may be entered.���� $��Samples��$��
5,154,10,0q�$	��Top=�$��1��$��75��.��� .+��� -   $ 	comboBox3��$��142,13,20,0q�$	��Top=� . $4 .The number of pixels or patches to be sampled.��.@��� $	��200���� $	��300���� $	��400���� $��Number of Samples:��$
��Left=�$��	10,10,0,0q�$	��Top=�$4 .The number of pixels or patches to be sampled.���� -    ��Name$ GenSample_Button��$��Generate Samples��$��Right=�$��
0,240,10,0q�$	��Top=�$
��10,2q�$��1��$&  Start the generation of samples.���� -   $ Learning_of_Weights��$��Learning of Weights��.��� 
�������$��0q� �� $
��127* ��� $
��186* �.+��� $��Inputs��$��
10,10,10,0q�$	��Top=�$	��100��$��2��.��� .+��� $��Samples:��$
��Left=�$��	10,10,0,0q�$	��Top=�$^ XThe samples generated to be used to create weights.
Drag and drop function is supported.���� $��Initial Weights:��$
��Left=�$��
10,41,0,10q�$	��Top=�$� �Use weights that have already been created, or generate random weights.
Drag and drop function is supported if using existing weights.���� -   $ textBox6��$��23��$��72,13,133,0q�$	��Top=�$��2q��� -   $ textBox7��$��23��$��237,44,133,0q�$	��Top=�$��2q�{�.  �� -   $��Load Samples. . .��$��Right=�$��	0,13,10,0q�$	��Top=�$
��10,2q�$" Load the samples to be used.���� -   $ button1��$��Load Weights. . .��$��Right=�$��	0,44,10,0q�$	��Top=�$
��10,2q�{�.  $ Load existing weights.���� -   $ 	comboBox4��$��
103,44,0,0q�$	��Top=�$
��Left=�$	��120��$M GUse weights that have already been created,
or generate random weights.��.@��� $��Random���� $��Existing���� $��Stopping Criteria��$��
10,220,5,0q�$��70��$	��Top=�.���  �� .+��� -    $
 	checkBox4��$��Angular RMSE��$��Right=�$	��Top=�$��	0,16,10,0q�$��Right=��� -!   $ 	comboBox6��$��10,13,119,0q�$	��Top=�$4 .The method to be used to reach a final result.��.@��� $	��MAX���� $��
Error Rate���� $
��RMSE���� $	��MSE���� $��
Thresholds��$��
5,220,10,0q�$	��Top=�$��1��$��70��.���  �� $
��169* ��� $
��171* �.+��� $��Local Threshold:��$
��Left=�$��	10,10,0,0q�$	��Top=�$j dThe threshold value to be used to calculate the stopping point
to a given criteria in a local scope.���� $��Global Threshold:��$
��Left=�$��	13,10,0,0q�$	��Top=� J� RenderTransformOrigin$ 0.903,0.885&�$��1��$x rThe threshold value to be used to calculate the stopping point
to a given criteria througout the entire algorithm.���� -"   $ 	textBox12��$��23��$��111,13,13,0q�$	��Top=�$��2q��� -#   $ 	textBox13��$��23��$��121,13,10,0q�$	��Top=�$��1��$��2q��� -$   $��Load Parameters. . .��$
��Left=�$��
10,301,0,0q�$	��Top=�$
��10,2q�$/ )Load existing parameter onto the program.���� -%   $��Save Parameters. . .��$
��Left=�$��140,301,0,0q�$	��Top=�$
��10,2q�$��2��$$ Save the parameters as a file.���� -&   $ Test_Button��$
��Test��$��Right=�$��
0,301,10,0q�$	��Top=�$
��30,2q�$��1��$3 -Test existing weights by checking their RMSE.���� -'   $ Learn_Button��$��Learn��$��Right=�$��
0,301,99,0q�$	��Top=�$
��30,2q�$��1��$$ Begin the learning of weights.���� -(   $ Console1��$	��200��$��10,335,10,0q� �� TextWrapping$
 Wrap=�$	��Top=�	 .  ��.  ��. " �� VerticalScrollBarVisibility$
 Auto=�$��2��
���   � �� 
IsReadOnly . 
�������$��Consolas)��� -)   $ button4��$$��Generate Image Parameters. . .��$
��Left=�$��
10,549,0,0q�$	��Top=�$
��10,2q�{�.  $C =Create the parameters to be used for the processing of image.���� -*   $ button3��$��Cancel��$��Right=�$��
0,549,10,0q�$	��Top=�$
��30,2q�$��1��{�.  $$ Abort the learning of weights.���� $��Configuration��$��10,115,10,0q�$	��Top=�$	��100��$��2��.���  ��  �� $
��206* ��� $
��199* ��� $
��189* �.+��� $��Network Size:��$
��Left=�$��	10,10,0,0q�$	��Top=�$; 5The network array to be used to generate the weights.���� $��Output:��$��	10,10,0,0q�$	��Top=�$
��Left=�$��1��$W QUsed to determine wether the output will result
in a discrete or continuous form.���� -+   $ textBox8��$��23��$��	97,13,1,0q�$	��Top=�$��2q��� -,   $ textBox9��$��23��$��107,13,10,0q�$	��Top=�$��2q�$��2���� --   $ 	comboBox5��$��
65,13,11,0q�$	��Top=�$��1��.@��� $��Discrete���� $��
Continuous���� $��Output Neurons:��$
��Left=�$��3,10,0,0q�$	��Top=�$��2��$Y SThe number of neurons in the output layer.
Used to calculate the inputs per sample.���� $��Samples in Learning:��$
��Left=�$��	10,41,0,0q�$	��Top=�$� }The number of samples to be used to generate the weights.
User may input a lower number of samples than the number existing. ���� -.   $ 	textBox10��$��23��$��134,44,15,0q�$	��Top=��� $��Number of Sectors:��$��	10,44,0,0q�$	��Top=�$
��Left=�$��1��$t nThe number of sectors to be processed from the unit circle.
Used for classification in the learning algorithm.���� -/   $ 	textBox11��$��23��$��127,44,11,0q�$	��Top=�$��2q�$��1��d� -0    d�Name$ button2��$��Pause��$��1��$��Right=�$��0,549,112,0q�$	��Top=�$
��30,2q�{�.  $$ Pause the learning of weights.���� -1   $ Processing_Image��$��Processing Image��.��� 
������� �� $
��156* ��� $
��157* �.+��� $��	Load Data��$��
10,10,10,0q�$	��Top=�$	��100��$��2��.��� .+��� $��Noisy Image:��$
��Left=�$��
10,10,0,10q�$	��Top=�$J DThe grayscale image with noise.
Drag and drop function is supported.���� $��Weights:��$
��Left=�$��
10,41,0,10q�$	��Top=�$T NThe file to be used to process the image.
Drag and drop function is supported.���� -2   $ 	textBox14��$��23��$��94,13,132,0q�$	��Top=�$��2q��� -3   $ 	textBox15��$��23��$��71,44,133,0q�$	��Top=�$��2q��� -4   $��Load Image. . .��$��Right=�$��	0,13,10,0q�$	��Top=�$
��10,2q�$	��109��$+ %Load the noisy image to be processed.���� -5   $��Load Weights. . .��$��Right=�$��	0,44,10,0q�$	��Top=�$
��10,2q���. $7 1Load the weights used to process the noisy image.���� $��
Parameters��$��10,144,10,0q�$	��Top=�$	��100��$��2��.���  �� $
��202* ��� $	��16* ��� $
��191* ��� $
��185* �.+��� -6    ��Name$ label1��$��Method:��$
��Left=�$��	10,10,0,0q�$	��Top=��� -7   $ 	textBox16��$��23��$��104,13,29,0q�$	��Top=�$��2���� -8   $ label2��$��Number of Sectors:��$
��Left=�$��3,10,0,0q�$	��Top=�$��2��$��1���� -9   $ 	textBox17��$��23��$��36,13,102,0q�$	��Top=�$��3���� -:   $ label3��$��Step:��$
��Left=�$��
186,10,0,0q�$	��Top=�$��2��$��2���� -;   $ 	textBox18��$��23��$��	97,44,0,0q�$	��Top=�$��2���� -<   $ label4��$��Network Size:��$
��Left=�$��	10,41,0,0q�$	��Top=��� -=   $ 	comboBox7��$��	70,13,0,0q�$	��Top=� . �� ->   $ label5��$��Output Neurons:��$
��Left=�$��	17,41,0,0q�$	��Top=�$��2���� -?   $ 	textBox19��$��23��$��
121,44,4,0q�$	��Top=�$��2���� -@   $��Load Parameters. . .��$
��Left=�$��
10,258,0,0q�$	��Top=�$
��10,2q�$/ )Load existing parameter onto the program.���� -A   $��Save Parameters. . .��$
��Left=�$��140,258,0,0q�$	��Top=�$
��10,2q�$$ Save the parameters as a file.���� -B   $ Process_Button��$��Process Image��$��Right=�$��
0,258,10,0q�$	��Top=�$
��10,2q� � IsSharedSizeScope . $��1��$( "Start the processing of the image.��	� -C   $ radioButton3��$��Process Using Pixels��$��Right=�$��
0,124,10,0q�$	��Top=�$8 2Process the image with kernel surrounding a pixel.��	� -D   $ radioButton4��$��Process Using Patches��$
��Left=�$��
10,124,0,0q�$	��Top=�$��1��$+ %Process the image by group of pixels.���� -E   $ Console2��$	��200��$��10,294,10,0q�$
 Wrap=�$	��Top=�	 .  |�.  ��.  ��. $
 Auto=�$��2��
���   �
�������$��Consolas)��� -F   $ button6��$��Cancel��$��Right=�$��
0,544,10,0q�$	��Top=�$
��30,2q�$��1��{�.  $# Abort the current opperation.��� -G    �Name$ progressBar1��$��22��$��10,508,10,0q�$	��Top=�$��2��d� -H   $ button5��$��Pause��$��1��$��Right=�$��0,544,112,0q�$	��Top=�$
��30,2q�{�.  $# Pause the current opperation.���� -I   $ Gather_Statistics��$��Gather Statistics��.��� 
�������.+��� $��Load Images��$��
10,10,10,0q�$	��Top=�$	��100��.��� .+��� $��First Image:��$
��Left=�$��
10,10,0,10q�$	��Top=�$K EThe first image to be compaired.
Drag and drop function is supported.���� $��Second Image:��$
��Left=�$��
10,41,0,10q�$	��Top=�$K EThe second image to be compared.
Drag and drop function is supported.���� -J   $ 	textBox20��$��23��$��86,13,122,0q�$	��Top=�$��2q��� -K   $ 	textBox21��$��23��$��103,44,122,0q�$	��Top=�$��2q��� -L   $��Load Image. . .��$��Right=�$��	0,13,10,0q�$	��Top=�$
��10,2q�$# Load a clean grayscale image.���� -M   $��Load Image. . .��$��Right=�$��	0,44,10,0q�$	��Top=�$
��10,2q�$# Load a noisy grayscale image.���� -N   $ Statistics_Button��$"��Gather Image Statistics. . .��$��Right=�$��
0,124,10,0q�$	��Top=�$
��10,2q� . $( "Start the processing of the image.���� -O   $ About��$��About�� J� IsHitTestVisible . $y�Hidden=�.��� ���	#FFE5E5E5.+�� $��0q�
������� �� $
��228* ��� $	��85* �.+��� $��Project LENA��$
��Left=�$��
285,10,0,0q�$	��Top=�
���   �$��Tahoma)�$��30(�$��2���� $
��Left=�$	��256��$��	14,37,0,0q�$	��Top=�$	��256��$��Lena.bmp��$��UniformToFill=��� $��Version 0.9.0.0��$
��Left=�$��
288,47,0,0q�$	��Top=�$��Tahoma)��� $
��Wrap=�$��
285,71,0,0q�$	��249��$	��Top=�$
��Left=� �� Padding$ 5q�$��2���� $���This program uses an intelligent approach to image processing using MLMVN, otherwise known as Multilayer feedforward neural network based on multi-valued neurons.���� �� �� �� $���The user may create and generate an image suitable to be filtered, generate samples used to create weights used to process images, generate the weights learned from this process, and filter a noisy image from the weights created. ���� �� �� �� $e�_Additional help can be provided by hovering the cursor over a label of an element of interest. ���� �� �� �� $���Developed by Mr. Plumlee and Mr. Del Pino. We would like to thank Dr. Aizenberg and Dr. Laddomada for their support to help us make this program a reality.��� $��Right=�$��27��$��0,0,10,0q�$	��Top=�$��30��.+��� -P   $ 
InfoButton��$��i��$
��Left=�$	��Top=�$��Monotype Corsiva)�   InfoButtonStyle1#���   $��17.333(�$��18��$��18��
�������$��7,5,0,0q�$ About��