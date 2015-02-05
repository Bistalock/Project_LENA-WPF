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

namespace Project_LENA___WPF
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        public Statistics(double RMSE, double PSNR)
        {           
            InitializeComponent();
            standardDevLabel.Content = RMSE;
            PSNRLabel.Content = PSNR;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }
    }
}
