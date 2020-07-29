using CoinFramework;
using DigitalSignatureAlgo;
using Microsoft.WindowsAPICodePack.Shell.Interop;
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
    /// Interaktionslogik für AddAccount.xaml
    /// </summary>
    public partial class AddAccount : UserControl
    {
        public AddAccount()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            AccountView v = new AccountView();
            EndFunction(nameBox.Text, v);
            nameBoxLogin.Text = "";
        }

        public SendBackAccount EndFunction;
        public delegate void SendBackAccount(string name, AccountView v);

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            AccountView view = new AccountView();
            try
            {
                KeyPair pair = new KeyPair(StringToByteArray(PubKeyBox.Text), StringToByteArray(PrivKeyBox.Text));
                view.keys = pair;
            }catch(Exception)
            {
                MessageBox.Show("The public key or private key is in the wrong format or corrupted");
            }
            
            EndFunction(nameBoxLogin.Text, view);
            PubKeyBox.Text = "";
            PrivKeyBox.Text = "";
            nameBoxLogin.Text = "";
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
