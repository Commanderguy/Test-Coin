using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoinFramework;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace CoinClient
{

    public struct keySafe
    {
        public byte[] publicKey;
        public byte[] privateKey;
        public keySafe(byte[] pubKey, byte[] privKey)
        {
            publicKey = pubKey;
            privateKey = privKey;
        }
    }

    /// <summary>
    /// Show a side panel and the balance of the account
    /// </summary>
    public partial class MainWindow : Window
    {
        private void NotImplMessage(string feature)
        {
            System.Windows.Forms.MessageBox.Show($"The '{feature}' feature is not implemented yet", "WIP", MessageBoxButtons.OK);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            bool nChain = false;
            if (File.Exists(".blockChain"))
                chain = Newtonsoft.Json.JsonConvert.DeserializeObject<Blockchain>(File.ReadAllText(".blockChain"));
            else
            {
                chain = new Blockchain();
                nChain = true;
            }

            if (File.Exists(".AccountList"))
            {
                accounts = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, AccountView>>(File.ReadAllText(".AccountList"));
                current = accounts.First().Value;
                accName = accounts.First().Key;
                
            }
            else
            {
                accounts = new Dictionary<string, AccountView>();
                accounts.Add("DEBUG-Account", new AccountView());
                current = accounts.First().Value;
                accName = accounts.First().Key;
                File.WriteAllText(".AccountList", Newtonsoft.Json.JsonConvert.SerializeObject(accounts));
            }

            if(nChain)
                ForgeNewChain();
            Save();
            InitializeComponent();
            SendButton.Content = "Send " + (string)Environment.CurrencyType + "s";

            
            AccountAddress.Content = ByteArrToString(current.publicKey, 37);
            onChainUpdated += UpdateBalance;
            onChainUpdated();
        }


        /// <summary>
        /// Get environment variables from the Json file.
        /// </summary>
        static dynamic Environment = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("env.json"));



        AccountView current;
        string accName;

        Dictionary<string, AccountView> accounts;


        public void ForgeNewChain()
        {
            Block b = new CoinFramework.Block();
            b.prev_hash = null;
            b.block_number = 0;
            b.miner = current.publicKey;
            b.calculateNonce(current.publicKey, chain);
            chain.AddBlock(b);
        }


        public void Save()
        {
            File.WriteAllText(".blockChain", Newtonsoft.Json.JsonConvert.SerializeObject(chain));
        }

        /// <summary>
        /// Turn a byte array into a string in hex format.
        /// </summary>
        /// <param name="arr">The byte array</param>
        /// <param name="cPerLiner">Characters per line</param>
        /// <returns>The array as a string</returns>
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


        /// <summary>
        /// Turn a byte array into a string in hex format.
        /// </summary>
        /// <param name="arr">The byte array</param>
        /// <returns>The array as a string</returns>
        public static string ByteArrToString(byte[] arr)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                builder.Append(arr[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public void UpdateBalance()
        {
            Balance.Content = chain.count_funds(current.publicKey).ToString() + (string)Environment.CurrencyType;
        }


        Blockchain chain;


        // Delegates
        public delegate void ChainChangedEvent();
        public ChainChangedEvent onChainUpdated;


        //Events

        private void AccountAddress_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Clipboard.SetText(ByteArrToString(current.publicKey));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendInterface.init(ReceiveTransaction, chain, current.privateKey, current.publicKey, (string)Environment.CurrencyType);
            SendInterface.Visibility = Visibility.Visible;
        }


        public void ReceiveTransaction(Transaction tx)
        {
            SendInterface.Visibility = Visibility.Hidden;
        }

        private void SavePublicToken_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(ByteArrToString(current.publicKey));
        }

        private void SavePrivateToken_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(ByteArrToString(current.privateKey));
        }

        [STAThread]
        private void SaveAccToFile_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                    File.WriteAllText(dialog.FileName + "/" + accName + ".acc", Newtonsoft.Json.JsonConvert.SerializeObject(new keySafe(current.publicKey, current.privateKey)));

            }
        }

        private void InspAcc_Click(object sender, RoutedEventArgs e)
        {
            this.NotImplMessage("Inspect account");
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AccountInterface.Visibility = Visibility.Visible;
            AccountInterface.init(accounts, ref current, accName, chain, (string)Environment.CurrencyType, UpdateBalance, updateCurrent);
            AccountInterface.AccAddWindow.Visibility = Visibility.Hidden;
        }

        private void updateCurrent(AccountView v, string cur)
        {
            current = v;
            accName = cur;
        }

        private void Label_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            this.NotImplMessage("GPU-Miner");
        }

        private void Label_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.NotImplMessage("Settings");
        }

        private void Label_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            SendInterface.Visibility = Visibility.Hidden;
            AccountInterface.Visibility = Visibility.Hidden;
            AccountInterface.end();
        }


        ~MainWindow()
        {
            File.WriteAllText(".AccountList", Newtonsoft.Json.JsonConvert.SerializeObject(accounts));
        }

        private void Label_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
