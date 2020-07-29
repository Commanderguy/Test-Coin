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
    /// Interaktionslogik für AmountSelector.xaml
    /// </summary>
    public partial class AmountSelector : UserControl
    {
        public AmountSelector()
        {
            InitializeComponent();
        }

        public void ShowDialogue(string name, sendBackResult retFunc, byte[] tToken)
        {
            this.Visibility = Visibility.Visible;
            res += retFunc;
            _dialogueSafe = tToken;
        }

        byte[] _dialogueSafe;

        sendBackResult res;
        public delegate void sendBackResult(double amount, byte[] token);

        private void amountVal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (amountVal.Text == "") return;

            var n = amountVal.CaretIndex;
            char nChar = amountVal.Text[amountVal.Text.Length - 1];
            if (char.IsDigit(nChar) || nChar == '.' || nChar == ',')
            {
                return;
            }
            amountVal.Text = amountVal.Text.Substring(0, amountVal.Text.Length - 1);
            amountVal.CaretIndex = n;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            res(Convert.ToDouble(amountVal.Text), _dialogueSafe);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            res(0, null);
        }
    }
}
