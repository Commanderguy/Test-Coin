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

namespace CoinClient
{
    /// <summary>
    /// Interaktionslogik für ContactItem.xaml
    /// </summary>
    public partial class ContactItem : UserControl
    {
        public ContactItem()
        {
            InitializeComponent();
        }

        public ContactItem(byte[] byteToken, string name, double balance, SendDelegate dSend)
        {
            InitializeComponent();
            AccName.Content = name;
            AccToken.Content = MainWindow.ByteArrToString(byteToken);
            Balance.Content = balance.ToString();
            onSending += onSending;
            _pubToken = byteToken;
            _name = name;
        }
        byte[] _pubToken;
        string _name;
        SendDelegate onSending;
        public delegate void SendDelegate(byte[] pubToken, string name);

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((string)AccToken.Content);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            onSending(_pubToken, _name);
        }
    }
}
