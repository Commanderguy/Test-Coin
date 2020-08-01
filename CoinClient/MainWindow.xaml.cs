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
using Syncfusion.UI.Xaml.Charts;

namespace CoinClient
{

    public class txStatus
    {
        public static List<txStatus> GetByChain(byte[] address, Blockchain chain)
        {
            List<txStatus> ret = new List<txStatus>();
            double stat = 0;

            bool fTx = false;
            for(int i = 0; i < chain.chain.Count; i++)
            {
                for(int n = 0; n < chain.chain[i].transactions.Count; n++)
                {
                    if(chain.chain[i].transactions[n].receiver == address)
                    {
                        fTx = true;
                        stat += chain.chain[i].transactions[n].value;
                    }else if(chain.chain[i].transactions[n].sender == address)
                    {
                        fTx = true;
                        stat -= chain.chain[i].transactions[n].value;
                    }
                }
                if (Enumerable.SequenceEqual(address, chain.chain[i].miner))
                {
                    fTx = true;
                    stat += CoinFramework.Environment.InitialCoinPerBlock / ((chain.chain[i].block_number / CoinFramework.Environment.diffReducer) + 1);
                }

                if(fTx)
                {
                    txStatus st = new txStatus();
                    st.balance = stat;
                    st.block = i;
                    Console.WriteLine("Added to series");
                }

            }

            return null;
        }

        public double balance;
        public int block;
    }



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


        static string chainFolder = ".CHAIN/";


        FileSystemWatcher watcher = new FileSystemWatcher(chainFolder);


        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            
            if (!Directory.Exists(".CHAIN"))
                Directory.CreateDirectory(".CHAIN");
            bool nChain = false;
            if (File.Exists(chainFolder + ".blockChain"))
                chain = Newtonsoft.Json.JsonConvert.DeserializeObject<Blockchain>(File.ReadAllText(chainFolder + ".blockChain"));
            else
            {
                chain = new Blockchain();
                nChain = true;
            }

            if (File.Exists(chainFolder + ".AccountList"))
            {
                accounts = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, AccountView>>(File.ReadAllText(chainFolder + ".AccountList"));
                current = accounts.First().Value;
                accName = accounts.First().Key;
                
            }
            else
            {
                accounts = new Dictionary<string, AccountView>();
                accounts.Add("DEBUG-Account", new AccountView());
                current = accounts.First().Value;
                accName = accounts.First().Key;
                File.WriteAllText(chainFolder + ".AccountList", Newtonsoft.Json.JsonConvert.SerializeObject(accounts));
            }
            InitializeComponent();
            new Thread(() =>
            {
                if (nChain)
                    ForgeNewChain();
                Save();
            }).Start();
            SendButton.Content = "Send " + (string)Environment.CurrencyType + "s";

            
            AccountAddress.Content = ByteArrToString(current.publicKey, 37);
            onChainUpdated += UpdateBalance;
            onChainUpdated();

            watcher.EnableRaisingEvents = true;
            watcher.Changed += Watcher_Changed;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                chain = Newtonsoft.Json.JsonConvert.DeserializeObject<Blockchain>(File.ReadAllText(chainFolder + ".blockChain"));
            }catch(System.IO.IOException)
            {
                new Thread(() =>
                {
                    for (; ; )
                    {
                        try
                        {
                            chain = Newtonsoft.Json.JsonConvert.DeserializeObject<Blockchain>(File.ReadAllText(chainFolder + ".blockChain"));
                            Dispatcher.Invoke(UpdateBalance);
                            return;
                        }
                        catch (IOException)
                        {
                            continue;
                        }
                    }
                }).Start();
            }
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
            File.WriteAllText(chainFolder + ".blockChain", Newtonsoft.Json.JsonConvert.SerializeObject(chain));
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
            ColumnSeries Series = new ColumnSeries();
            Series.ItemsSource = txStatus.GetByChain(current.publicKey, chain);
            Series.XBindingPath = "block";
            Series.YBindingPath = "balance";
            BalanceChart.Series.Add(Series);
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
            SaveTxToFile(tx);
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
            ContactInterface.Visibility = Visibility.Hidden;
        }

        private void updateCurrent(AccountView v, string cur)
        {
            current = v;
            accName = cur;
        }

        private void Label_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            File.WriteAllText(chainFolder + ".MINERACC", JsonConvert.SerializeObject(new keySafe(current.publicKey, current.privateKey)));

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = new FileInfo("CPU-Miner.exe").FullName;
            info.Arguments = chainFolder;
            Process.Start(info);
        }

        private void Label_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void Label_MouseLeftButtonUp_2(object sender, MouseButtonEventArgs e)
        {
            UpdateBalance();
            SendInterface.Visibility = Visibility.Hidden;
            AccountInterface.Visibility = Visibility.Hidden;
            AccountInterface.end();
            ContactInterface.Visibility = Visibility.Hidden;
        }


        public void SaveTxToFile(Transaction tx)
        {
            List<Transaction> pool;
            if (File.Exists(chainFolder + ".LazyPool"))
                pool = JsonConvert.DeserializeObject<List<Transaction>>(File.ReadAllText(chainFolder + ".LazyPool"));
            else
                pool = new List<Transaction>();
            pool.Add(tx);
            File.WriteAllText(chainFolder + ".LazyPool", JsonConvert.SerializeObject(pool));
        }


        ~MainWindow()
        {
            File.WriteAllText(chainFolder + ".AccountList", Newtonsoft.Json.JsonConvert.SerializeObject(accounts));
        }

        private void Label_MouseLeftButtonUp_3(object sender, MouseButtonEventArgs e)
        {
            SendInterface.Visibility = Visibility.Hidden;
            AccountInterface.Visibility = Visibility.Hidden;
            AccountInterface.end();
            ContactInterface.Visibility = Visibility.Visible;
            ContactInterface.init(onSendTx, chain);
        }

        private void onSendTx(Transaction tx)
        {
            SaveTxToFile(tx);
        }

        private void Label_MouseLeftButtonUp_4(object sender, MouseButtonEventArgs e)
        {
            this.NotImplMessage("Settings");
        }
    }
}
