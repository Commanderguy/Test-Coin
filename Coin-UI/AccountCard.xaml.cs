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
using Test_Coin;

namespace Coin_UI
{
    /// <summary>
    /// Interaktionslogik für AccountCard.xaml
    /// </summary>
    public partial class AccountCard : UserControl
    {
        public AccountCard()
        {
            InitializeComponent();
        }
        Blockchain _chain;
        public void init(string name, byte[] _publicKey, byte[] _privateKey, Blockchain chain)
        {
            _chain = chain;
            NAME.Content = name;
            pubKey = _publicKey;
            string s = "";

            s = BitConverter.ToString(pubKey);


            pub_key.Content = s;
            privKey = _privateKey;
            Balance.Content = Convert.ToString(chain.count_funds(_publicKey));
        }

        public byte[] pubKey;
        public byte[] privKey;

        public void update() 
        {
            Balance.Content = Convert.ToString(_chain.count_funds(pubKey));
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Clipboard.SetText(ByteArrayToString(pubKey));
        }
    }
}
