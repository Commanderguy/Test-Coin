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
using CoinFramework;

namespace CoinClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AccountView v1 = new AccountView();
            AccountAddress.Content = ByteArrToString(v1.publicKey, 37);
        }





        static string ByteArrToString(byte[] arr, int cPerLiner)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                builder.Append(arr[i].ToString("x2"));
                if ((i + 2) % cPerLiner == 0) builder.Append("\n");
            }
            return builder.ToString();
        }
    }
}
