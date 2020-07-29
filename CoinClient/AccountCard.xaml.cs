using CoinFramework;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoinClient
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

        public void init(string name, string token, double balance, AccountView nView, string currency, bool selected, onUpdateDelegate onUp)
        {
            onUpdate += onUp;
            AccName.Content = name;
            AccToken.Content = token;
            Balance.Content = "Balance: " + balance.ToString() + currency;
            view = nView;
            _selected = selected;
        }

        bool _selected = false;

        SolidColorBrush GetBrush { get => _selected ? Brushes.Red : Brushes.AliceBlue; }

        public AccountView view;

        
        private void Grid_MouseEnter(object sender, MouseEventArgs e) => HoverStroke.BeginAnimation(Rectangle.StrokeProperty, new BrushAnimation {To = Brushes.Black, Duration = TimeSpan.FromMilliseconds(300) });
        private void Grid_MouseLeave(object sender, MouseEventArgs e) => HoverStroke.BeginAnimation(Rectangle.StrokeProperty, new BrushAnimation { To = GetBrush, Duration = TimeSpan.FromMilliseconds(300) });

        private void HoverStroke_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        onUpdateDelegate onUpdate;

        public delegate void onUpdateDelegate(AccountView newCurrent);

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            onUpdate(view);
        }

        private void PubKeyCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(MainWindow.ByteArrToString(view.publicKey));
        }

        private void PrivKeyCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(MainWindow.ByteArrToString(view.privateKey));
        }
    }
}
