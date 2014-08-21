using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
/* ---------------------- Added Libraries ---------------------- */
using System.Windows.Media.Animation;
using System.Windows.Shell;
using System.Collections.Specialized;
using System.Runtime.InteropServices; // DLLImport
using System.Threading; // CancellationToken
using System.Xml; // Loading xml file parameters
using BitMiracle.LibTiff.Classic; // Use Tiff images
using System.Diagnostics; // Stopwatch
using System.IO; // BinaryReader, open and save files
using System.Numerics; // Incoporates the use of complex numbers

namespace Project_LENA___WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Functions functions;
        MLMVN mlmvn;

        CancellationTokenSource cTokenSource1; // Declare a System.Threading.CancellationTokenSource for the fourth tab.
        PauseTokenSource pTokenSource1; // Declaring a usermade pausetoken for the fourth tab.
        CancellationTokenSource cTokenSource2; // Declare a System.Threading.CancellationTokenSource for the third tab.
        PauseTokenSource pTokenSource2; // Declaring a usermade pausetoken for the third tab.

        public MainWindow()
        {
            functions = new Functions(this);
            mlmvn = new MLMVN(this);
            InitializeComponent();
            this.Height = 230;

            //ensure win32 handle is created
            var handle = new System.Windows.Interop.WindowInteropHelper(this).EnsureHandle();

            //set window background
            var result = SetClassLong(handle, GCL_HBRBACKGROUND, GetSysColorBrush(COLOR_WINDOW));
            
        }

        private bool IsLearing = false;
        private bool IsTesting = false;
        private bool IsProcessing = false;

        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);
        //    switch (m.Msg)
        //    {
        //        case 0x84: //WM_NCHITTEST
        //            var result = (HitTest)m.Result.ToInt32();
        //            if (result == HitTest.Left || result == HitTest.Right)
        //                m.Result = new IntPtr((int)HitTest.Caption);
        //            if (result == HitTest.TopLeft || result == HitTest.TopRight)
        //                m.Result = new IntPtr((int)HitTest.Top);
        //            if (result == HitTest.BottomLeft || result == HitTest.BottomRight)
        //                m.Result = new IntPtr((int)HitTest.Bottom);

        //            break;
        //    }
        //}
        //enum HitTest
        //{
        //    Caption = 2,
        //    Transparent = -1,
        //    Nowhere = 0,
        //    Client = 1,
        //    Left = 10,
        //    Right = 11,
        //    Top = 12,
        //    TopLeft = 13,
        //    TopRight = 14,
        //    Bottom = 15,
        //    BottomLeft = 16,
        //    BottomRight = 17,
        //    Border = 18
        //}

        public static IntPtr SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            //check for x64
            if (IntPtr.Size > 4)
                return SetClassLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetClassLongPtr32(hWnd, nIndex, unchecked((uint)dwNewLong.ToInt32())));
        }

        private const int GCL_HBRBACKGROUND = -10;
        private const int COLOR_WINDOW = 5;

        [DllImport("user32.dll", EntryPoint = "SetClassLong")]
        public static extern uint SetClassLongPtr32(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetClassLongPtr")]
        public static extern IntPtr SetClassLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        static extern IntPtr GetSysColorBrush(int nIndex);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Resources");
            dlg.DefaultExt = ".tif";
            dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                textBox1.Text = dlg.FileName;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                // Make sure events fires only when changing tabs.
                if (e.Source is TabControl)
                {
                    // if the Noise Generation tab is selected.
                    if (Noise_Generation.IsSelected)
                    {
                        if (checkBox3.IsChecked == false) this.AnimateWindowSize(230);
                        else if (checkBox3.IsChecked == true) this.AnimateWindowSize(345);
                    }
                    else if (Sample_Generation.IsSelected) this.AnimateWindowSize(345);
                    else if (Learning_of_Weights.IsSelected)
                    {
                        if (IsLearing == false && IsTesting == false) this.AnimateWindowSize(405);
                        else if (IsLearing == true || IsTesting == true) this.AnimateWindowSize(655);
                    }
                    else if (Processing_Image.IsSelected)
                    {
                        if (radioButton3.IsChecked == true || radioButton4.IsChecked == true) this.AnimateWindowSize(365);
                        else this.AnimateWindowSize(220);
                    }
                    else if (About.IsSelected) this.AnimateWindowSize(400);
                }
            }
        }

        private void lol_Copy15_Click(object sender, RoutedEventArgs e)
        {
            this.AnimateWindowSize(585);
            //ProgressBar1.Foreground = Brushes.Green;
            Process();
        }

        //Create a Delegate that matches 
        //the Signature of the ProgressBar's SetValue method
        private delegate void UpdateProgressBarDelegate(
                System.Windows.DependencyProperty dp, Object value);


        private void Process()
        {
            //Configure the ProgressBar
            progressBar1.Minimum = 0;
            progressBar1.Maximum = short.MaxValue;
            progressBar1.Value = 0;
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            TaskbarItemInfo.ProgressValue = progressBar1.Value/progressBar1.Maximum;

            //Stores the value of the ProgressBar
            double value = 0;

            //Create a new instance of our ProgressBar Delegate that points
            // to the ProgressBar's SetValue method.
            UpdateProgressBarDelegate updatePbDelegate =
                new UpdateProgressBarDelegate(progressBar1.SetValue);

            //Tight Loop: Loop until the ProgressBar.Value reaches the max
            do
            {
                value += 1;
                TaskbarItemInfo.ProgressValue = progressBar1.Value / progressBar1.Maximum;

                /*Update the Value of the ProgressBar:
                    1) Pass the "updatePbDelegate" delegate
                       that points to the progressBar1.SetValue method
                    2) Set the DispatcherPriority to "Background"
                    3) Pass an Object() Array containing the property
                       to update (ProgressBar.ValueProperty) and the new value */
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
            }
            while (progressBar1.Value != progressBar1.Maximum);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            #region Error Checking
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBoxResult result = MessageBox.Show("Please input the image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBoxResult result = MessageBox.Show("Please input the Gaussian noise to add to the image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }
            #endregion        
           
            #region GetTagInfo
            Tiff image = Tiff.Open(textBox1.Text, "r");

            // Obtain basic tag information of the image
            int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
            byte pixel = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
            #endregion

            #region Grayscale Image
            if (pixel == 1)
            {
                double noise = Convert.ToDouble(textBox2.Text);


                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Please input the grayscale image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK)
                    {
                        return;
                    }
                }
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Please enter the noise.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                byte[,] grey = new byte[height, width];
                grey = Functions.Tiff2Array(image, height, width);

                double greysum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        greysum += grey[i, j];
                    }
                }

                double mean = greysum / (height * width); // image mean

                double variancesum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        variancesum += Math.Pow((grey[i, j] - mean), 2);
                    }
                }
                double dispersion;
                dispersion = variancesum / (height * width); // image dispersion

                double standarddev;
                standarddev = Math.Sqrt(dispersion);

                // int width = Convert.ToInt32(textBox1.Text);
                // int height = Convert.ToInt32(textBox2.Text);

                // calls the "createRandomTiff" method
                double[,] y = Functions.createRandomTiff(width, height, mean, standarddev, noise, grey, checkBox1.IsChecked);

                string fileName = textBox18.Text;

                if (checkBox1.IsChecked == true)
                    fileName = System.IO.Path.GetFileNameWithoutExtension(textBox1.Text) + "_Gauss_" + Convert.ToString(noise) + ".tif";
                if (checkBox1.IsChecked == false)
                    fileName = System.IO.Path.GetFileNameWithoutExtension(textBox1.Text) + "_Gauss_" + Convert.ToString(noise) + "_Noise" + ".tif";


                // Create OpenFileDialog 
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".tif";
                dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";
                dlg.FileName = fileName;
                // Assigns the results value when Dialog is opened
                var dlgresult = dlg.ShowDialog();

                // Checks if value is true
                if (dlgresult == true)
                {
                    using (Tiff output = Tiff.Open(dlg.FileName, "w"))
                    {
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        output.SetField(TiffTag.ROWSPERSTRIP, height);
                        output.SetField(TiffTag.XRESOLUTION, 88.0);
                        output.SetField(TiffTag.YRESOLUTION, 88.0);
                        output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                        byte[] im = new byte[width * sizeof(byte /*can be changed depending on the format of the image*/)];

                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                im[j] = Convert.ToByte(y[i, j]);
                            }
                            output.WriteScanline(im, i);
                        }
                        output.WriteDirectory();
                        output.Dispose();
                    }
                }
                
                image.Dispose();
            }
            #endregion

            #region Color Image
            if (pixel == 3)
            {
                double noise = Convert.ToDouble(textBox2.Text);

                //// Obtain basic tag information of the image
                //#region GetTagInfo
                //int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                //int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                //byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                //byte pixel = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                //#endregion

                int test = (int)bits;

                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Please input the color image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK)
                    {
                        return;
                    }
                }
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Please enter the noise.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                int imageSize = height * width * 3;
                byte[] raster = new byte[imageSize];

                byte[] scanline = new byte[image.ScanlineSize()];

                // Read the image into the memory buffer
                byte[,] red = new byte[height, width];
                byte[,] green = new byte[height, width];
                byte[,] blue = new byte[height, width];

                //for (int i = height - 1; i != -1; i--)
                for (int i = 0; height > i; i++)
                {
                    image.ReadScanline(scanline, i); // EVIL BUG HERE
                    for (int j = 0; j < width; j++)
                    {
                        red[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                        green[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                        blue[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct
                    }
                }

                byte[,] RGB = new byte[height, image.ScanlineSize()];

                #region Grayscale Gaussian noise
                if (checkBox2.IsChecked == true)
                {
                    #region Y
                    double[,] y = new double[height, width];
                    byte[,] Y = new byte[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            y[i, j] = (0.299 * red[i, j]) + (0.587 * green[i, j]) + (0.114 * blue[i, j]);

                            Y[i, j] = Convert.ToByte(y[i, j]);
                        }

                    }

                    double ysum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ysum += Y[i, j];
                        }
                    }

                    double mean_Y = ysum / (height * width); // image mean

                    double variancesum_Y = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            variancesum_Y += Math.Pow((Y[i, j] - mean_Y), 2);
                        }
                    }
                    double dispersion_Y;
                    dispersion_Y = variancesum_Y / (height * width); // image dispersion

                    double standarddev_Y;
                    standarddev_Y = Math.Sqrt(dispersion_Y);

                    Random φ = new Random(); // Greek letter phi
                    Random Γ = new Random();// Greek letter gamma

                    // sine and cosine variables for the Box-Muller algorithm
                    double[,] z1 = new double[height, width];
                    double[,] z2 = new double[height, width];

                    // normally distributed variables gathered from Box-Muller algorithm with added image mean and sigma
                    double[,] x1 = new double[height, width];
                    double[,] x2 = new double[height, width];

                    double number; // used to fix bug

                    // applying Gaussian noise to each pixel
                    for (int i = 0; i < height; ++i)
                    {
                        for (int j = 0; j < width; ++j)
                        {
                            // the Box-Muller algorithm
                            z1[i, j] = Math.Cos(2 * Math.PI * φ.NextDouble()) * Math.Sqrt(-2 * Math.Log(Γ.NextDouble()));
                            z2[i, j] = Math.Sin(2 * Math.PI * φ.NextDouble()) * Math.Sqrt(-2 * Math.Log(Γ.NextDouble()));

                            number = φ.NextDouble(); // fixes bug (for some reason)

                            x1[i, j] = mean_Y + z1[i, j] * noise * standarddev_Y;
                            x2[i, j] = mean_Y + z2[i, j] * noise * standarddev_Y;

                            #region Polar form of the Box-Muller algorithm
                            //do
                            //{
                            //    x3[i, j] = 2 * φ.NextDouble() - 1;
                            //    x4[i, j] = 2 * Γ.NextDouble() - 1;
                            //    w[i, j] = x3[i, j] * x3[i, j] + x4[i, j] * x4[i, j];
                            //} while (w[i, j] >= 1);

                            //w[i, j] = Math.Sqrt((-2 * Math.Log(w[i, j])) / w[i, j]);
                            //z1[i, j] = x3[i, j] * w[i, j];
                            //z2[i, j] = x4[i, j] * w[i, j];

                            //x1[i, j] = m + z1[i, j] * noise * σ;
                            //x2[i, j] = m + z2[i, j] * noise * σ;
                            #endregion
                        }
                    }

                    #endregion

                    #region Red

                    double redsum = 0;
                    double[,] r = new double[height, width];

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            redsum += red[i, j];
                        }
                    }

                    double mean_R = redsum / (height * width); // image mean

                    // Applies the Y Gaussian noise to red
                    #region Checkbox status
                    if (checkBox1.IsChecked == true)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    r[i, j] = red[i, j] + x1[i, j] - mean_Y;
                                if (j % 2 == 0)
                                    r[i, j] = red[i, j] + x2[i, j] - mean_Y;

                                if (r[i, j] > 255) r[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (r[i, j] < 0) r[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }

                    else if (checkBox1.IsChecked == false)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    r[i, j] = x1[i, j];
                                if (j % 2 == 0)
                                    r[i, j] = x2[i, j];

                                if (r[i, j] > 255) r[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (r[i, j] < 0) r[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region Green
                    double[,] g = new double[height, width];
                    double greensum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            greensum += green[i, j];
                        }
                    }

                    double mean_G = greensum / (height * width); // image mean

                    // Applies the Y Gaussian noise to green
                    #region Checkbox status
                    if (checkBox1.IsChecked == true)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    g[i, j] = green[i, j] + x1[i, j] - mean_Y;
                                if (j % 2 == 0)
                                    g[i, j] = green[i, j] + x2[i, j] - mean_Y;

                                if (g[i, j] > 255) g[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (g[i, j] < 0) g[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }

                    else if (checkBox1.IsChecked == false)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    g[i, j] = x1[i, j];
                                if (j % 2 == 0)
                                    g[i, j] = x2[i, j];

                                if (g[i, j] > 255) g[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (g[i, j] < 0) g[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region Blue
                    double[,] b = new double[height, width];
                    double bluesum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            bluesum += blue[i, j];
                        }
                    }

                    double mean_B = bluesum / (height * width); // image mean

                    // Applies the Y Gaussian noise to blue
                    #region Checkbox status
                    if (checkBox1.IsChecked == true)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    b[i, j] = blue[i, j] + x1[i, j] - mean_Y;
                                if (j % 2 == 0)
                                    b[i, j] = blue[i, j] + x2[i, j] - mean_Y;

                                if (b[i, j] > 255) b[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (b[i, j] < 0) b[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }

                    else if (checkBox1.IsChecked == false)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    b[i, j] = x1[i, j];
                                if (j % 2 == 0)
                                    b[i, j] = x2[i, j];

                                if (b[i, j] > 255) b[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (b[i, j] < 0) b[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region Merge RGB
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j] = Convert.ToByte(r[i, j]);
                        }
                    }

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j + 1] = Convert.ToByte(g[i, j]);
                        }
                    }
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j + 2] = Convert.ToByte(b[i, j]);
                        }
                    }
                    #endregion
                }
                #endregion

                #region Color Gaussian noise
                if (checkBox2.IsChecked == false)
                {
                    #region Red

                    double redsum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            redsum += red[i, j];
                        }
                    }

                    double mean_R = redsum / (height * width); // image mean

                    double variancesum_R = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            variancesum_R += Math.Pow((red[i, j] - mean_R), 2);
                        }
                    }
                    double dispersion_R;
                    dispersion_R = variancesum_R / (height * width); // image dispersion

                    double standarddev_R;
                    standarddev_R = Math.Sqrt(dispersion_R);

                    // int width = Convert.ToInt32(textBox1.Text);
                    // int height = Convert.ToInt32(textBox2.Text);

                    // calls the "createRandomTiff" method

                    double[,] r = Functions.createRandomTiff(width, height, mean_R, standarddev_R, noise, red, checkBox1.IsChecked);
                    #endregion

                    #region Green
                    double greensum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            greensum += green[i, j];
                        }
                    }

                    double mean_G = greensum / (height * width); // image mean

                    double variancesum_G = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            variancesum_G += Math.Pow((green[i, j] - mean_G), 2);
                        }
                    }
                    double dispersion_G;
                    dispersion_G = variancesum_G / (height * width); // image dispersion

                    double standarddev_G;
                    standarddev_G = Math.Sqrt(dispersion_G);

                    // int width = Convert.ToInt32(textBox1.Text);
                    // int height = Convert.ToInt32(textBox2.Text);

                    // calls the "createRandomTiff" method

                    double[,] g = Functions.createRandomTiff(width, height, mean_G, standarddev_G, noise, green, checkBox1.IsChecked);
                    #endregion

                    #region Blue
                    double bluesum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            bluesum += blue[i, j];
                        }
                    }

                    double mean_B = bluesum / (height * width); // image mean

                    double variancesum_B = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            variancesum_B += Math.Pow((blue[i, j] - mean_B), 2);
                        }
                    }
                    double dispersion_B;
                    dispersion_B = variancesum_B / (height * width); // image dispersion

                    double standarddev_B;
                    standarddev_B = Math.Sqrt(dispersion_B);

                    // int width = Convert.ToInt32(textBox1.Text);
                    // int height = Convert.ToInt32(textBox2.Text);

                    // calls the "createRandomTiff" method

                    double[,] b = Functions.createRandomTiff(width, height, mean_B, standarddev_B, noise, blue, checkBox1.IsChecked);
                    #endregion

                    #region Merge RGB
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j] = Convert.ToByte(r[i, j]);
                        }
                    }

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j + 1] = Convert.ToByte(g[i, j]);
                        }
                    }
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j + 2] = Convert.ToByte(b[i, j]);
                        }
                    }
                    #endregion
                }
                #endregion



                string fileName = textBox18.Text;
                if (checkBox2.IsChecked == true)
                {
                    if (checkBox1.IsChecked == true)
                        fileName = System.IO.Path.GetFileNameWithoutExtension(textBox1.Text) + "_Gauss_Y_" + Convert.ToString(noise) + ".tif";
                    if (checkBox1.IsChecked == false)
                        fileName = System.IO.Path.GetFileNameWithoutExtension(textBox1.Text) + "_Gauss_Y_" + Convert.ToString(noise) + "_Noise" + ".tif";
                }
                if (checkBox2.IsChecked == false)
                {
                    if (checkBox1.IsChecked == true)
                        fileName = System.IO.Path.GetFileNameWithoutExtension(textBox1.Text) + "_Gauss_" + Convert.ToString(noise) + ".tif";
                    if (checkBox1.IsChecked == false)
                        fileName = System.IO.Path.GetFileNameWithoutExtension(textBox1.Text) + "_Gauss_" + Convert.ToString(noise) + "_Noise" + ".tif";
                }
                
                 // Create OpenFileDialog 
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".tif";
                dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";
                dlg.FileName = fileName;
                // Assigns the results value when Dialog is opened
                var dlgresult = dlg.ShowDialog();

                // Checks if value is true
                if (dlgresult == true)
                {
                    using (Tiff output = Tiff.Open(dlg.FileName, "w"))
                    {
                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        //output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        //output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        //output.SetField(TiffTag.ROWSPERSTRIP, height);
                        //output.SetField(TiffTag.XRESOLUTION, 88.0);
                        //output.SetField(TiffTag.YRESOLUTION, 88.0);
                        //output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                        // Write the tiff tags to the file
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        output.SetField(TiffTag.ROWSPERSTRIP, height);
                        output.SetField(TiffTag.XRESOLUTION, 88.0);
                        output.SetField(TiffTag.YRESOLUTION, 88.0);
                        output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        //output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 3);

                        //int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                        //int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                        //byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                        //byte pixel = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();

                        byte[] im = new byte[image.ScanlineSize() * sizeof(byte /*can be changed depending on the format of the image*/)];

                        for (int i = 0; i < height; i++)
                        {

                            for (int j = 0; j < image.ScanlineSize(); j++)
                            {
                                im[j] = RGB[i, j];
                            }
                            output.WriteEncodedStrip(i, im, image.ScanlineSize());
                        }
                        output.WriteDirectory();
                        output.Dispose();
                    }
                }
                image.Dispose();
            }
            #endregion
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Resources");
            dlg.DefaultExt = ".tif";
            dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                textBox3.Text = dlg.FileName;
            }
        }

        private void TextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            // Get data object
            var dataObject = e.Data as DataObject;

            // Check for file list
            if (dataObject.ContainsFileDropList())
            {
                // Clear values
                ((TextBox)sender).Text = string.Empty;

                // Process file names
                StringCollection fileNames = dataObject.GetFileDropList();
                StringBuilder bd = new StringBuilder();
                foreach (var fileName in fileNames)
                {
                    bd.Append(fileName);
                }

                // Set text
                ((TextBox)sender).Text = bd.ToString();
            }


            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //{
            //    // Note that you can have more than one file.
            //    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //    // Assuming you have one file that you care about, pass it off to whatever
            //    // handling code you have defined.
            //    foreach (string fileName in files)
            //    {
            //        ((TextBox)sender).Text = fileName;
            //    }
            //}
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        #region Form Functions

        delegate void SetTextCallback(string text);

         //Textbox for tab 3
        public void SetText1(string text)
        {
            if (Console1.Dispatcher.CheckAccess())
            {
                Console1.AppendText(text);
                Console1.CaretIndex = Console1.Text.Length;
                Console1.ScrollToEnd();
            }
            else
            {
                SetTextCallback d = new SetTextCallback(SetText1);
                this.Dispatcher.Invoke(d, new object[] { text });
                //Console1.Dispatcher.Invoke(d, new object[] { Console1, text });
            }
        }

        // Textbox for tab 4
        public void SetText2(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (Console2.Dispatcher.CheckAccess())
            {
                Console2.AppendText(text);
            }
            else
            {
                SetTextCallback d = new SetTextCallback(SetText2);
                Console2.Dispatcher.Invoke(d, new object[] { text });
            }
        }

        delegate int SetProgressCallback(int value);

        // Progressbar for tab 4
        public double SetProgress1(double value)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (progressBar1.Dispatcher.CheckAccess())
            {
                this.progressBar1.Value += value;
                TaskbarItemInfo.ProgressValue = progressBar1.Value / progressBar1.Maximum;
            }
            else
            {
                SetTextCallback d = new SetTextCallback(SetText2);
                Console2.Dispatcher.Invoke(d, new object[] { progressBar1, value });
            }
            return progressBar1.Value;
        }

        // read parameters from xml file
        public void xmlImageParams(string FileName)
        {
            // Loading from a file
            XmlReader Xml = XmlReader.Create(FileName);

            while (Xml.Read())
            {
                if (Xml.NodeType == XmlNodeType.Element && Xml.Name == "Method")
                {
                    if (Xml.GetAttribute(0) == "Pixel_Parameters")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "Parameters")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.Name == "Number_of_Sectors")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox16.Text = Xml.Value; // Number of sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Input_Layer_Size")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox17.Text = Xml.Value; // Input layer size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Hidden_Layer_Size")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox18.Text = Xml.Value; // Hidden layer size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Kernel")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox7.SelectedIndex = Convert.ToInt32(Xml.Value); // Kernel
                                            }
                                        }
                                        Xml.Read();
                                    }
                                }
                            }
                        }
                    }
                    else if (Xml.GetAttribute(0) == "Patch_Parameters")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "Parameters")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.Name == "Patch_Method")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox7.SelectedIndex = Convert.ToInt32(Xml.Value); // Patch method
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Number_of_Sectors")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox16.Text = Xml.Value; // Number of sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Step")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox17.Text = Xml.Value; // Step
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Network_Size")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox18.Text = Xml.Value; // Network size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Output_Neurons")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox19.Text = Xml.Value; // Output neurons
                                            }
                                        }
                                        Xml.Read();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // proper closure and disposing of the file and memory
            Xml.Close();
            Xml.Dispose();
        }

        public void xmlWeightParams(string FileName)
        {
            // Loading from a file
            XmlReader Xml = XmlReader.Create(FileName);

            while (Xml.Read())
            {
                if (Xml.NodeType == XmlNodeType.Element && Xml.Name == "Method")
                {
                    if (Xml.GetAttribute(0) == "Weight_Parameters")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "Parameters")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.Name == "Size_Of_Network")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox8.Text = Xml.Value; // Size of Network
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Output")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox5.SelectedIndex = Convert.ToInt32(Xml.Value); // Output
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Output_Neurons")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox9.Text = Xml.Value; // Output Neurons
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Samples_in_Learning")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox10.Text = Xml.Value; // Samples in Learning
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Number_Of_Sectors")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox11.Text = Xml.Value; // Number of Sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Stopping_Criteria")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox6.SelectedIndex = Convert.ToInt32(Xml.Value); // Stopping Criteria
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Angular_RMSE")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                checkBox2.IsChecked = Convert.ToBoolean(Xml.Value); // Angular RMSE
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Local_Threshold")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox12.Text = Xml.Value; // Local Threshold
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Global_Threshold")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox13.Text = Xml.Value; // Global Threshold
                                            }
                                        }
                                        Xml.Read();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // proper closure and disposing of the file and memory
            Xml.Close();
            Xml.Dispose();
        }
        #endregion

        private void checkBox3_Checked(object sender, RoutedEventArgs e)
        {
            this.AnimateWindowSize(345);
        }

        private void checkBox3_Unchecked(object sender, RoutedEventArgs e)
        {
            this.AnimateWindowSize(230);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBoxResult result = MessageBox.Show("Please input the color image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            else
            {
                Tiff image = Tiff.Open(textBox3.Text, "r");
                // Obtain basic tag information of the image
                #region GetTagInfo
                int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                #endregion

                int imageSize = height * width * 3;
                int[] raster = new int[imageSize];

                byte[] scanline = new byte[image.ScanlineSize()];

                // Initiallization of RGB values
                int[,] red = new int[height, width];
                int[,] green = new int[height, width];
                int[,] blue = new int[height, width];

                // Initiallization of YUV values
                double[,] Y = new double[height, width];
                double[,] U = new double[height, width];
                double[,] V = new double[height, width];

                // I want the closest result to VEGA
                for (int i = height - 1; i != -1; i--)
                {
                    image.ReadScanline(scanline, i);
                    for (int j = 0; j < width; j++)
                    {
                        red[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                        green[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                        blue[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct


                        // This is what MATLAB uses to convert RGB to YUV - PSNR 26.25002 to VEGA
                        //Y[i, j] = (0.257 * red[i, j]) + (0.504 * green[i, j]) + (0.098 * blue[i, j]) + 16;
                        //U[i, j] = -(0.148 * red[i, j]) - (0.291 * green[i, j]) + (0.439 * blue[i, j]) + 128;
                        //V[i, j] = (0.439 * red[i, j]) - (0.368 * green[i, j]) - (0.071 * blue[i, j]) + 128;

                        // Using bitwise shift operations to convert, no success - PSNR 26.21524
                        //Y[i, j] = ((66 * red[i, j] + 129 * green[i, j] + 25 * blue[i, j] + 128) >> 8) + 16;

                        // CCIR Recommendation 709 - PSNR 30.61359 to VEGA
                        //Y[i, j] = (0.2125 * red[i, j]) + (0.7154 * green[i, j]) + (0.0721 * blue[i, j]);

                        // Conversion from RGB to YUV, as written here:
                        // http://www.eagle.tamut.edu/faculty/igor/MY%20CLASSES/CS-467/Lecture-12.pdf on slide 18
                        // Also part of the CCIR Recommendation 601-1 - PSNR 27.8757 to VEGA
                        //Y[i, j] = (0.299 * red[i, j]) + (0.587 * green[i, j]) + (0.114 * blue[i, j]);
                        Y[i, j] = (0.3 * red[i, j]) + (0.59 * green[i, j]) + (0.11 * blue[i, j]);
                        U[i, j] = -(0.14713 * red[i, j]) - (0.28886 * green[i, j]) + (0.436 * blue[i, j]);
                        V[i, j] = (0.615 * red[i, j]) - (0.51499 * green[i, j]) - (0.10001 * blue[i, j]);

                    }
                }

                #region Merge YUV - (for debugging purposes)

                //byte[,] YUV = new byte[height, image.ScanlineSize()];

                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        YUV[i, 3 * j] = Convert.ToByte(Y[i, j]);
                //    }
                //}

                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        YUV[i, 3 * j + 1] = Convert.ToByte(U[i, j]);
                //    }
                //}
                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        YUV[i, 3 * j + 2] = Convert.ToByte(V[i, j]);
                //    }
                //}
                #endregion

                #region Save Image
                // Create OpenFileDialog 
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".tif";
                dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";
                dlg.FileName = System.IO.Path.GetFileNameWithoutExtension(textBox3.Text) + "_Y" + ".tif";
                // Assigns the results value when Dialog is opened
                var dlgresult = dlg.ShowDialog();

                // Checks if value is true
                if (dlgresult == true)
                {
                    using (Tiff output = Tiff.Open(dlg.FileName, "w"))
                    {
                        // Write the tiff tags to the file
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 1);

                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.BITSPERSAMPLE, bits);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                        //output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        //output.SetField(TiffTag.ROWSPERSTRIP, height);
                        //output.SetField(TiffTag.XRESOLUTION, dpiX);
                        //output.SetField(TiffTag.YRESOLUTION, dpiY);
                        //output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                        byte[] im = new byte[width * sizeof(byte /*can be changed depending on the format of the image)*/)];

                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (Y[i, j] > 255) Y[i, j] = 255;
                                if (Y[i, j] < 0) Y[i, j] = 0;
                                im[j] = Convert.ToByte(Y[i, j]);
                            }
                            output.WriteScanline(im, i);
                        }
                        output.WriteDirectory();
                        output.Dispose();

                        #region Save YUV image - (for debugging purposes)

                        //// Write the tiff tags to the file
                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.SEPARATE);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.YCBCR);
                        //output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        ////output.YCBCRCOEFFICIENTS
                        //  //  output.YCBCRSUBSAMPLING
                        //    //    output.YCBCRPOSITIONING = 

                        //byte[] im = new byte[image.ScanlineSize() * sizeof(byte /*can be changed depending on the format of the image*/)];

                        //for (int i = 0; i < height; i++)
                        //{

                        //    for (int j = 0; j < image.ScanlineSize(); j++)
                        //    {
                        //        im[j] = YUV[i, j];
                        //    }
                        //    output.WriteEncodedStrip(i, im, image.ScanlineSize());
                        //}
                        #endregion
                    }
                    //System.Diagnostics.Process.Start(FileName);                   
                }
                #endregion
                textBox1.Text = dlg.FileName;
                image.Dispose();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Resources");
            dlg.DefaultExt = ".tif";
            dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                textBox4.Text = dlg.FileName;
            }
        }

        private void radioButton3_Checked(object sender, RoutedEventArgs e)
        {
            this.AnimateWindowSize(365);
        }

        private void radioButton4_Checked(object sender, RoutedEventArgs e)
        {
            this.AnimateWindowSize(365);
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".tif";
            dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                textBox5.Text = dlg.FileName;
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Resources");
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text Documents (*.txt)|*.txt|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                textBox6.Text = dlg.FileName;
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".wgt";
            dlg.Filter = "Weights (*.wgt)|*.wgt|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                textBox7.Text = dlg.FileName;
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".tif";
            dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                textBox14.Text = dlg.FileName;
            }
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".tif";
            dlg.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                textBox15.Text = dlg.FileName;
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            GenSample_Button.IsEnabled = false;

            // open the images
            Tiff cleanimage = Tiff.Open(textBox4.Text, "r");
            Tiff noisedimage = Tiff.Open(textBox5.Text, "r");

            #region Error Checking
            // Error Windows when no image entered
            if (cleanimage == null || noisedimage == null)
            {
                GenSample_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Invalid or no image entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            // Error Windows when no radio button checked
            if (radioButton1.IsChecked == false && radioButton2.IsChecked == false)
            {
                GenSample_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("No inplementation checked.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            // Error Windows when no number of samples entered
            if (comboBox3.Text == "")
            {
                GenSample_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Please enter the number of samples.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }
            #endregion

            int sSize = Convert.ToInt32(comboBox3.Text);

            // Obtain basic tag information of the image
            #region GetTagInfo
            int width = cleanimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = cleanimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            byte bits = cleanimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
            byte pixel = cleanimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
            double dpiX = cleanimage.GetField(TiffTag.XRESOLUTION)[0].ToDouble();
            double dpiY = cleanimage.GetField(TiffTag.YRESOLUTION)[0].ToDouble();
            #endregion

            // The clean image
            byte[,] clean = new byte[height, width];
            clean = Functions.Tiff2Array(cleanimage, height, width);

            // The noisy image
            byte[,] noised = new byte[height, width];
            noised = Functions.Tiff2Array(noisedimage, height, width);

            #region Samples using Pixels
            if (radioButton1.IsChecked == true) // Process using pixels
            {
                int kernel = 0;

                if (comboBox1.Text == "")
                {
                    // Error Windows when no number of samples entered
                    GenSample_Button.IsEnabled = true;
                    MessageBoxResult result = MessageBox.Show("No kernel size selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK)
                    {
                        return;
                    }
                }
                else
                {
                    char[] c = comboBox1.Text.ToCharArray(); // seperates compbox elements into an array

                    for (int i = 0; i < c.Length; i++)
                    {
                        if (c[i].ToString() == " " || c[i].ToString() == "x" || c[i].ToString() == "X")
                            break;
                        else
                            kernel = Convert.ToInt32(comboBox1.Text.Substring(0, i + 1));
                    }

                    // ************************  Let the user enter any odd number as size of the pixel
                    if (kernel % 2 != 1)
                    {
                        GenSample_Button.IsEnabled = true;
                        MessageBoxResult result = MessageBox.Show("Please enter an odd number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        if (result == MessageBoxResult.OK)
                        {
                            return;
                        }
                    }
                }

                string fileName = System.IO.Path.GetFileNameWithoutExtension(textBox5.Text) + "_Samples_" + comboBox3.Text + "_Pixels_" + kernel + "x" + kernel + ".txt";

                // Create OpenFileDialog 
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text Documents (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.FileName = fileName;
                // Assigns the results value when Dialog is opened
                var dlgresult = dlg.ShowDialog();

                // Checks if value is true
                if (dlgresult == true)
                {
                    Functions.LearnSet(clean, noised, kernel, sSize, dlg.FileName);
                }
            }
            #endregion

            #region Samples using Patches
            else if (radioButton2.IsChecked == true) // Process using patches
            {
                // combobox values
                int kernel = 0;

                if (comboBox2.Text == "")
                {
                    // Error Windows when no number of samples entered
                    GenSample_Button.IsEnabled = true;
                    MessageBoxResult result = MessageBox.Show("No kernel size selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result == MessageBoxResult.OK)
                    {
                        return;
                    }
                }
                else
                {
                    char[] c = comboBox2.Text.ToCharArray(); // seperates compbox elements into an array

                    for (int i = 0; i < c.Length; i++)
                    {
                        if (c[i].ToString() == " " || c[i].ToString() == "x" || c[i].ToString() == "X")
                            break;
                        else
                            kernel = Convert.ToInt32(comboBox2.Text.Substring(0, i + 1));
                    }

                    // ************************  Let the user enter any odd number as size of the patch
                    if (kernel % 2 != 1)
                    {
                        GenSample_Button.IsEnabled = true;
                        MessageBoxResult result = MessageBox.Show("Please enter an odd number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        if (result == MessageBoxResult.OK)
                        {
                            return;
                        }
                    }
                }


                string fileName = System.IO.Path.GetFileNameWithoutExtension(textBox5.Text) + "_Samples_" + comboBox3.Text + "_Patches_" + kernel + "x" + kernel + ".txt";

                // Create OpenFileDialog 
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                // Set filter for file extension and default file extension                
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text Documents (*.txt)|*.txt|All files (*.*)|*.*";
                dlg.FileName = fileName;
                // Assigns the results value when Dialog is opened
                var dlgresult = dlg.ShowDialog();

                // Checks if value is true
                if (dlgresult == true)
                {
                    Functions.LearnSetPatch(clean, noised, kernel, sSize, dlg.FileName);
                }
            }
            #endregion

            cleanimage.Dispose();
            noisedimage.Dispose();
            GenSample_Button.IsEnabled = true;
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            comboBox1.IsEnabled = true;
            comboBox2.IsEnabled = false;
        }

        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            comboBox1.IsEnabled = false;
            comboBox2.IsEnabled = true;
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"Resources");
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Documents (*.xml)|*.xml|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                xmlWeightParams(dlg.FileName);
            }           
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.FileName = "Weight_Parameters.xml";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Documents (*.xml)|*.xml|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                // defines the xml settings for the file
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true; // allows indentation

                // xml name based on file dialogbox name
                XmlWriter parameters = XmlWriter.Create(dlg.FileName, settings);

                // start the generation of the xml parameters
                parameters.WriteStartDocument();

                // xml parameters
                parameters.WriteStartElement("Method");
                parameters.WriteAttributeString("type", "Weight_Parameters");
                parameters.WriteStartElement("Parameters");
                parameters.WriteElementString("Size_Of_Network", textBox8.Text);
                parameters.WriteElementString("Output", Convert.ToString(comboBox5.SelectedIndex));
                parameters.WriteElementString("Output_Neurons", textBox9.Text);
                parameters.WriteElementString("Samples_in_Learning", textBox10.Text);
                parameters.WriteElementString("Number_Of_Sectors", textBox11.Text);
                parameters.WriteElementString("Stopping_Criteria", Convert.ToString(comboBox6.SelectedIndex));
                parameters.WriteElementString("Angular_RMSE", Convert.ToString(checkBox4.IsChecked));
                parameters.WriteElementString("Local_Threshold", textBox12.Text);
                parameters.WriteElementString("Global_Threshold", textBox13.Text);

                // proper closure and disposing of the file and memory
                parameters.Flush();
                parameters.Close();
                parameters.Dispose();
            }   
        }

        private void comboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox4.SelectedIndex == 1)
            {
                textBox7.IsEnabled = true;
                button1.IsEnabled = true;
            }
            else
            {
                textBox7.IsEnabled = false;
                button1.IsEnabled = false;
            }
        }

        private async void Button_Click_14(object sender, RoutedEventArgs e)
        {
            Learn_Button.IsEnabled = false;
            IsLearing = true;
            #region Error checking
            // Error Windows when no image entered
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Samples file not entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }

            }
            if (comboBox4.SelectedIndex == -1)
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("No weight type selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            if (comboBox4.SelectedIndex == 1 && string.IsNullOrEmpty(textBox7.Text))
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Selected existing weights, but no weights entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            // Error Windows when no radio button checked
            //
            if (string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox9.Text) || string.IsNullOrEmpty(textBox10.Text) || string.IsNullOrEmpty(textBox11.Text) || comboBox5.SelectedIndex == -1)
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Check for empty parameters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            if (comboBox6.SelectedIndex == -1)
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("No stopping criteria algorithm selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }
            if (string.IsNullOrEmpty(textBox12.Text) || string.IsNullOrEmpty(textBox13.Text))
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Check for empty threshold values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }
            #endregion

            #region Variable Initiallization
            string Samples = textBox6.Text;
            string Weights = textBox7.Text;

            // New cancellation token
            cTokenSource2 = new CancellationTokenSource();

            // Create a cancellation token from CancellationTokenSource
            var cToken = cTokenSource2.Token;

            // New pause token
            pTokenSource2 = new PauseTokenSource();

            // Create a pause token from PauseTokenSource
            var pToken = pTokenSource2.Token;

            bool randomWeights = false;
            if (comboBox4.SelectedIndex == 0)
            {
                randomWeights = true;
            }
            if (comboBox4.SelectedIndex == 1)
            {
                randomWeights = false;
            }

            int NumberofSamples = Convert.ToInt32(textBox10.Text);

            double LocalThreshold = Convert.ToDouble(textBox12.Text);
            double GlobalThreshold = Convert.ToDouble(textBox13.Text);

            // convert string array to int
            string[] a = textBox8.Text.Split(',', '.');
            int[] networkSize = new int[4];
            for (int i = 0; i < a.Length; i++)
            {
                networkSize[i] = Convert.ToInt32(a[i]);
            }

            // determine number of samples
            int[] inputsPerSample = new int[a.Length];
            inputsPerSample[0] = networkSize[a.Length - 1] + Convert.ToInt32(textBox9.Text);
            for (int i = 1; i < a.Length; i++)
                inputsPerSample[i] = networkSize[0] + Convert.ToInt32(textBox9.Text);
            // end for

            int NumberofSectors = Convert.ToInt32(textBox11.Text);
            #endregion

            this.comboBox6.IsEnabled = false;
            this.checkBox4.IsEnabled = false;
            this.textBox12.IsEnabled = false;
            this.textBox13.IsEnabled = false;
            this.button2.IsEnabled = true;
            this.button3.IsEnabled = true;
            this.button4.IsEnabled = true;            
            this.AnimateWindowSize(655);

            // Begin processing
            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();
            Title = "Project LENA - WPF (Working)";
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;

            try
            {
                Complex[][,] weights = await Task.Run(() => mlmvn.Learning(Samples, NumberofSamples, Weights, 4, networkSize, inputsPerSample, NumberofSectors, GlobalThreshold, LocalThreshold, randomWeights, cTokenSource2.Token, pTokenSource2.Token));

                string[] imagename = System.IO.Path.GetFileNameWithoutExtension(textBox6.Text).Split('_');

                string fileName = imagename[0] + "_" + imagename[1] + "_" + imagename[2] + "_" + imagename[3] + "_Samples_" + NumberofSamples + "_Network_[" +
                        networkSize[0] + "," + networkSize[1] + "," + networkSize[2] + "," + networkSize[3] + "]" + "_RMSE_" + GlobalThreshold + ".wgt";

                // Stop timing
                stopwatch.Stop();

                // Write result
                SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);

                // Create OpenFileDialog 
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                // Set filter for file extension and default file extension 
                dlg.FileName = fileName;
                dlg.DefaultExt = ".wgt";
                dlg.Filter = "Weights (*.wgt)|*.wgt|All files (*.*)|*.*";

                // Assigns the results value when Dialog is opened
                var result = dlg.ShowDialog();

                // Checks if value is true
                if (result == true)
                {

                    MLMVN.saveMlmvnWeights(dlg.FileName, weights, networkSize);
                }
            }
            catch (OperationCanceledException)
            {
                SetText1("\r\nProgress canceled.\r\n");

                // Stop timing
                stopwatch.Stop();

                // Write result
                SetText2("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine + Environment.NewLine);

                // Set the CancellationTokenSource to null when the work is complete.
                cTokenSource2 = null;
            }
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;

            cTokenSource2 = null;

            this.Learn_Button.IsEnabled = true;
            this.comboBox6.IsEnabled = true;
            this.checkBox4.IsEnabled = true;
            this.textBox12.IsEnabled = true;
            this.textBox13.IsEnabled = true;
            this.button2.IsEnabled = false;
            this.button3.IsEnabled = false;

            Title = "Project LENA - WPF";
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            About.IsSelected = true;
        }

        private void comboBox6_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox6.SelectedIndex == 2) checkBox4.IsEnabled = true;
            else checkBox4.IsEnabled = false;
        }

        private void button2_Checked(object sender, RoutedEventArgs e)
        {
            Title = "Project LENA - WPF (Paused)";
            SetText1("\r\nProcess is paused." + Environment.NewLine);
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            this.button2.Content = "Resume";
            button3.IsEnabled = false;
            pTokenSource2.IsPaused = !pTokenSource2.IsPaused;
        }

        private void button2_Unchecked(object sender, RoutedEventArgs e)
        {
            Title = "Project LENA - WPF (Working)";
            SetText1("Process is resumed." + Environment.NewLine);
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            this.button2.Content = "Pause";
            button3.IsEnabled = true;
            pTokenSource2.IsPaused = !pTokenSource2.IsPaused;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (cTokenSource2 != null)
            {
                cTokenSource2.Cancel();
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.FileName = "Patch_Parameters.xml";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Documents (*.xml)|*.xml|All files (*.*)|*.*";

            // Assigns the results value when Dialog is opened
            var result = dlg.ShowDialog();

            // Checks if value is true
            if (result == true)
            {
                // defines the xml settings for the file
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true; // allows indentation

                // xml name based on file dialogbox name
                XmlWriter parameters = XmlWriter.Create(dlg.FileName, settings);

                // start the generation of the xml parameters
                parameters.WriteStartDocument();

                // xml parameters
                parameters.WriteStartElement("Method");
                parameters.WriteAttributeString("type", "Patch_Parameters");
                parameters.WriteStartElement("Parameters");
                parameters.WriteElementString("Patch_Method", "-1");
                parameters.WriteElementString("Number_of_Sectors", textBox11.Text);
                parameters.WriteElementString("Step", "3");
                parameters.WriteElementString("Network_Size", textBox8.Text);
                parameters.WriteElementString("Output_Neurons", textBox9.Text);

                // proper closure and disposing of the file and memory
                parameters.Flush();
                parameters.Close();
                parameters.Dispose();
            }
        }

        private async void Test_Button_Click(object sender, RoutedEventArgs e)
        {
            Test_Button.IsEnabled = false;
            IsTesting = true;
            #region Error checking
            // Error Windows when no image entered
            if (string.IsNullOrEmpty(textBox6.Text))
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Samples file not entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }

            }

            if (comboBox4.SelectedIndex != 1 && string.IsNullOrEmpty(textBox7.Text))
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Input existing weights for testing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            // Error Windows when no radio button checked
            //
            if (string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox9.Text) || string.IsNullOrEmpty(textBox10.Text) || string.IsNullOrEmpty(textBox11.Text))
            {
                Learn_Button.IsEnabled = true;
                MessageBoxResult result = MessageBox.Show("Check for empty parameters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }
            #endregion

            #region Variable Initiallization
            string Samples = textBox6.Text;
            string Weights = textBox7.Text;

            // convert string array to int
            string[] a = textBox8.Text.Split(',', '.');
            int[] networkSize = new int[4];
            for (int i = 0; i < a.Length; i++)
            {
                networkSize[i] = Convert.ToInt32(a[i]);
            }

            // determine number of samples
            int[] inputsPerSample = new int[a.Length];
            inputsPerSample[0] = networkSize[a.Length - 1] + Convert.ToInt32(textBox9.Text);
            for (int i = 1; i < a.Length; i++)
                inputsPerSample[i] = networkSize[0] + Convert.ToInt32(textBox9.Text);

            int NumberofSamples = Convert.ToInt32(textBox10.Text);
            int NumberofSectors = Convert.ToInt32(textBox11.Text);           
            #endregion

            this.button2.IsEnabled = true;
            this.button3.IsEnabled = true;
            this.button4.IsEnabled = true;
            if (IsLearing == false) this.AnimateWindowSize(620);

            Title = "Project LENA - WPF (Working)";

            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;

            int[,] output = await Task.Run(() => mlmvn.TEST(Samples, NumberofSamples, Weights, 4, networkSize, inputsPerSample, NumberofSectors));

            if (IsLearing == false) TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
            Test_Button.IsEnabled = true;
        }        
    }

    /// <summary>
    /// Class to add resizing animation!
    /// </summary>
    public static class WindowUtilties
    {
        public static void AnimateWindowSize(this Window target, double newHeight)
        {
            var sb = new Storyboard { Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)) };

            //var aniWidth = new DoubleAnimationUsingKeyFrames();
            var aniHeight = new DoubleAnimationUsingKeyFrames();

            //aniWidth.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));
            aniHeight.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200));

            aniHeight.KeyFrames.Add(new EasingDoubleKeyFrame(target.ActualHeight, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 00))));
            aniHeight.KeyFrames.Add(new EasingDoubleKeyFrame(newHeight, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 200))));
            //aniWidth.KeyFrames.Add(new EasingDoubleKeyFrame(target.ActualWidth, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 00))));

            //Storyboard.SetTarget(aniWidth, target);
            //Storyboard.SetTargetProperty(aniWidth, new PropertyPath(Window.WidthProperty));

            Storyboard.SetTarget(aniHeight, target);
            Storyboard.SetTargetProperty(aniHeight, new PropertyPath(Window.HeightProperty));

            //sb.Children.Add(aniWidth);
            sb.Children.Add(aniHeight);

            sb.Begin();
        }
    }
}
