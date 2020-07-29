using CoinFramework;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using System;
using System.Collections.Generic;
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

namespace CoinClient
{
    /// <summary>
    /// Interaktionslogik für AccountManager.xaml
    /// </summary>
    public partial class AccountManager : UserControl
    {
        public AccountManager()
        {
            InitializeComponent();
        }
        bool active = false;
        public void init(Dictionary<string, AccountView> Accounts, ref AccountView current, string currName, Blockchain chain, string currency, UpdateMainBalance u, UpdateCurrent u2)
        {
            if (active) return;
            up = u;
            up2 = u2;
            _Accounts = Accounts;
            _currency = currency; 
            _chain = chain;
            active = true;
            _curName = currName;
            _current = current;
            new Thread(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    foreach (var x in Accounts)
                    {

                        var card = new AccountCard();
                        if(x.Value == _current)
                            card.HoverStroke.Stroke = Brushes.Red;
                        card.Margin = new Thickness(3);
                        card.init(x.Key, MainWindow.ByteArrToString(x.Value.publicKey), chain.count_funds(x.Value.publicKey), x.Value, currency, _current == x.Value, onUpdate);
                        AccountList.Children.Add(card);
                    }
                    Button b = new Button();
                    b.FontSize = 30;
                    b.Click += B_Click;

                    b.Margin = new Thickness(20);
                    b.Content = "Add account";
                    AccountList.Children.Add(b);
                });
            }).Start();
        }
        Blockchain _chain;
        AccountView _current;
        Dictionary<string, AccountView> _Accounts;
        string _currency;
        string _curName;
        public void onUpdate(AccountView view)
        {
            _current = view;
            end();
            new Thread(() =>
            { 
                Dispatcher.Invoke(() =>
                {
                    foreach (var x in _Accounts)
                    {

                        var card = new AccountCard();
                        if (x.Value == _current)
                            card.HoverStroke.Stroke = Brushes.Red;
                        card.Margin = new Thickness(3);
                        card.init(x.Key, MainWindow.ByteArrToString(x.Value.publicKey), _chain.count_funds(x.Value.publicKey), x.Value, _currency, _current == x.Value, onUpdate);
                        AccountList.Children.Add(card);
                    }
                    Button b = new Button();
                    b.FontSize = 30;

                    b.Click += B_Click;

                    b.Margin = new Thickness(20);
                    b.Content = "Add account";
                    AccountList.Children.Add(b);
                });
            }).Start();
            up2(_current, _curName);
            up();

        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            AccAddWindow.Visibility = Visibility.Visible;
            AccAddWindow.EndFunction += AddFunc;
        }

        private void AddFunc(string name, AccountView v)
        {
            
            try
            {
                _Accounts.Add(name, v);
            }catch(ArgumentException)
            {
                //MessageBox.Show("The accountname already exists in the accountlist.");
            }
            AccAddWindow.Visibility = Visibility.Hidden;
            onUpdate(v);
        }

        public void end()
        {
            active = false;
            AccountList.Children.Clear();
        }


        





        UpdateMainBalance up;
        public delegate void UpdateMainBalance();

        UpdateCurrent up2;
        public delegate void UpdateCurrent(AccountView v, string cur);
    }
}
