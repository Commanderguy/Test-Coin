using CoinFramework;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaktionslogik für RequestSendAdress.xaml
    /// </summary>
    public partial class RequestSendAdress : UserControl
    {
        public RequestSendAdress()
        {
            InitializeComponent();
            
        }

        public void init(onFinish finishFunc, Blockchain chain, byte[] privKey, byte[] pubKey, string currency)
        {
            privateKey = privKey;
            publicKey = pubKey;
            finished += finishFunc;
            
            SndLabel.Content = "Send " + currency;
            c = chain;
        }

        Blockchain c;

        byte[] privateKey;
        byte[] publicKey;

        onFinish finished;

        private void ConfirmTx_Click(object sender, RoutedEventArgs e)
        {
            Transaction tx = new Transaction(publicKey, StringToByteArray(Rec.Text), Convert.ToDouble(Amount.Text), c.numTransactions(publicKey), privateKey);
            Console.WriteLine("Added tran saction with num = " + c.numTransactions(publicKey));
            finished(tx);
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }


        public delegate void onFinish(Transaction tx);

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Amount.Text == "") return;

            var n = Amount.CaretIndex;
            char nChar = Amount.Text[Amount.Text.Length - 1];
            if(char.IsDigit(nChar) || nChar == '.' || nChar == ',')
            {
                return;
            }
            Amount.Text = Amount.Text.Substring(0, Amount.Text.Length - 1);
            Amount.CaretIndex = n;
        }

        private void cncl_Click(object sender, RoutedEventArgs e)
        {
            finished(null);
        }
    }
}
