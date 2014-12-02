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
using System.Windows.Shapes;
/* ---------------------- Added Libraries ---------------------- */
using BitMiracle.LibTiff.Classic; // Use Tiff images
using System.Collections.Specialized; // String Collection
using System.IO; // Memory Stream

namespace Project_LENA___WPF
{
    /// <summary>
    /// Interaction logic for Fragments.xaml
    /// </summary>
    public partial class Fragments : Window
    {
        string cleanimage;
        string noisyimage;
        private double Percentage;

        public Fragments(string clean, string noisy)
        {
            InitializeComponent();

            cleanimage = clean;
            noisyimage = noisy;

            // read bytes of an image
            byte[] buffer = File.ReadAllBytes(clean);

            // create a memory streams out of the bytes read
            MemoryStream ms = new MemoryStream(buffer);
            var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = ms;
                imageSource.EndInit();

            ImageBrush brush = new ImageBrush();
            brush.ImageSource = imageSource;

            //open a tiff stored in the memory stream!
            Canvas1.Background = brush;

            label1.Content = System.IO.Path.GetFileName(clean);

            // read bytes of an image
            byte[] buffer2 = File.ReadAllBytes(noisy);

            // create a memory streams out of the bytes read
            MemoryStream ms2 = new MemoryStream(buffer2);
            var imageSource2 = new BitmapImage();
                imageSource2.BeginInit();
                imageSource2.StreamSource = ms2;
                imageSource2.EndInit();

            ImageBrush brush2 = new ImageBrush();
            brush2.ImageSource = imageSource2;

            //open a tiff stored in the memory stream!
            Canvas1.Background = brush2;

            label1.Content = System.IO.Path.GetFileName(noisy);


            int WindowWidth = Convert.ToInt32(Canvas1.Width + Canvas2.Width + 40);
            int WindowHeight = Convert.ToInt32(Canvas1.Height + 138);

            //if (this.MinimumSize.Width < maxwidth && this.MinimumSize.Height < maxheight)
            this.Width = WindowWidth;
            this.Height = WindowHeight;
            //else this.MaximumSize = this.MinimumSize;

            //Canvas1.Width = (this.Width) / 2 - 20;
            //Canvas1.Height = this.Height - 12 - 126;
            //Canvas2.Width = (this.Width) / 2 - 20;
            //Canvas2.Height = this.Height - 12 - 126;
            //Canvas2.Left = (Canvas1.Width + 12);
            //label2.Left = panel2.Left;

            //comboBox1.Left = 44;
            //comboBox1.Top = this.Height - 61;

            //string[] a = comboBox1.Text.Split(' ');
            //Percentage = Convert.ToDouble(a[0]) / 100;   
        }
    }
}
