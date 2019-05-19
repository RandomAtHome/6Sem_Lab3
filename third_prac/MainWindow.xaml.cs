using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;

namespace third_prac
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
    public class UI : IUIService
    {
        public Chart UIChart => throw new NotImplementedException();

        public bool ConfirmAction(string text, string title)
        {
            MessageBoxResult res = MessageBox.Show(text, title, MessageBoxButton.YesNo);
            return res == MessageBoxResult.Yes;
        }

        public void ShowErrorMessage(string text)
        {
            MessageBox.Show(text);
        }
    }
}
