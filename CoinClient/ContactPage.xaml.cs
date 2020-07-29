using CoinFramework;
using Newtonsoft.Json;
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
    /// Interaktionslogik für ContactPage.xaml
    /// </summary>
    public partial class ContactPage : UserControl
    {
        public ContactPage()
        {
            InitializeComponent();
        }

        AccountView cur;

        public ContactPage(sendTransaction onSendTx, Blockchain chain, AccountView v)
        {
            init(onSendTx, chain);
            cur = v;
        }

        bool _init = false;

        public void init(sendTransaction onSendTx, Blockchain chain)
        {
            _chain = chain;
            if (_init) return;
            _init = true;
            sendTx += onSendTx;
            if (File.Exists(".contacts"))
                contacts = JsonConvert.DeserializeObject<List<Contact>>(File.ReadAllText(".contacts"));
            else
                contacts = new List<Contact>();

            foreach(var x in contacts)
            {
                x.balance = chain.count_funds(x.pubToken);
                ContactItem item = new ContactItem(x.pubToken, x.name, x.balance, OpenSendDialogue);
                item.Margin = new Thickness(20);
                contactList.Children.Add(item);
            }

            Button b = new Button();
            b.Content = "Add contact";
            b.FontSize = 30;
            b.Click += onAddContact;
            b.Margin = new Thickness(30);
            contactList.Children.Add(b);
        }

        Blockchain _chain;

        public void Save()
        {
            File.WriteAllText(".contacts", JsonConvert.SerializeObject(contacts));
        }

        private void onAddContact(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenSendDialogue(byte[] pubToken, string name)
        {
            amountWin.ShowDialogue(name, onGotAmount, pubToken);
        }

        private void onGotAmount(double amount, byte[] pToken)
        {
            if (amount == 0 && pToken == null)
                return;

            Transaction tx = new Transaction(cur.publicKey, pToken, amount, _chain.numTransactions(cur.publicKey), cur.privateKey);
        }

        List<Contact> contacts;

        sendTransaction sendTx;
        public delegate void sendTransaction(Transaction tx);
    }
}
