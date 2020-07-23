using BlockChainNode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Media.Animation;
using Test_Coin;
using DigitalSignatureAlgo;

namespace Coin_UI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists("Blockchain.TST"))
                chain = new Blockchain();
            else
                chain = Newtonsoft.Json.JsonConvert.DeserializeObject<Blockchain>(File.ReadAllText("Blockchain.TST"));
            dB = new NodeDB(".nodes");
            node = new Node(chain, dB);
            th = new Thread(dB.Lookup);
            th.Start(chain);
            
        }
        AccountCard current = null;
        Thread th;

        Blockchain chain;
        NodeDB dB;
        Node node;
        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation(0.75, TimeSpan.FromMilliseconds(400));
            blackFill.BeginAnimation(Rectangle.OpacityProperty, da);
            blackFill.Visibility = Visibility.Visible;
            WhiteFill.Visibility = Visibility.Visible;
            _visi_inText.Visibility = Visibility.Visible;
            _visi_nameLabel.Visibility = Visibility.Visible;
            _visi_titleLabel.Visibility = Visibility.Visible;
            subButton.Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            File.WriteAllText("Blockchain.TST", Newtonsoft.Json.JsonConvert.SerializeObject(chain));
            throw new Exception();
            node.stopThread();
            Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
            node.stop = true;
            System.Environment.FailFast("Application ended, force ending all threads");
            String process = Process.GetCurrentProcess().ProcessName;
            Process.Start("cmd.exe", "/c taskkill /F /IM " + process + ".exe /T");
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation(0, TimeSpan.FromMilliseconds(400));
            blackFill.BeginAnimation(Rectangle.OpacityProperty, da);
            blackFill.Visibility = Visibility.Hidden;
            WhiteFill.Visibility = Visibility.Hidden;
            _visi_inText.Visibility = Visibility.Hidden;
            _visi_nameLabel.Visibility = Visibility.Hidden;
            _visi_titleLabel.Visibility = Visibility.Hidden;
            subButton.Visibility = Visibility.Hidden;

            AccountCard nCard = new AccountCard();
            var k = ECDSA.GenerateKeys();
            nCard.init(_visi_inText.Text, k.pubKey, k.privKey, chain);
            AccountList.Children.Add(nCard);
            current = (AccountCard)AccountList.Children[AccountList.Children.Count - 1];
        }


        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            
            Transaction tx = new Transaction(current.pubKey, StringToByteArray(GetRecSAddr.Text), Convert.ToDouble(ValueBox.Text), 0, current.privKey);
            dB.pool.addTx(tx);
            tx.SendTransaction(dB.KnownNodes);
            
        }

        




        private void ValueBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Test_Coin.Block b = dB.pool.ForgeBlock();
            statBar.Text = "Size of pool: " + b.transactions.Count;
            Dispatcher.Invoke(() => {
                UpBar.Content = "Started calculating nonce";
                });
            if (chain.chain.Count == 0)
            {
                b.block_number = 0;
                b.calculateNonce(current.pubKey);
            }else
            {
                b.prev_hash = chain.chain[chain.chain.Count - 1].hash;
                b.block_number = chain.chain.Count;
                b.calculateNonce(current.pubKey);
            }
            Dispatcher.Invoke(() => {
                UpBar.Content = "Found coin, adding to chain";
                Balance.Content = 
                chain.count_funds(current.pubKey).ToString();
            });
            chain.AddBlock(b);

            
        }
    }
}
