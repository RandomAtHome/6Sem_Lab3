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
            DataContext = new MainWindowVM(new UI(uiChart));
            newModelStack.DataContext = (DataContext as MainWindowVM).NewModelInputView;
            Closed += (DataContext as MainWindowVM).Window_Closed;
        }
    }
    public class UI : IUIService
    {
        private Chart uiChart = null;
        public UI(Chart chart)
        {
            this.uiChart = chart;
        }

        public Chart UIChart
        {
            get
            {
                return uiChart;
            }
        }

        public bool ConfirmAction(string text, string title)
        {
            return MessageBox.Show(text, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        public string OpenFileDGName()
        {
            var dg = new Microsoft.Win32.OpenFileDialog();
            return dg.ShowDialog() == true ? dg.FileName : string.Empty;
        }

        public string SaveFileDGName()
        {
            var dg = new Microsoft.Win32.SaveFileDialog();
            return dg.ShowDialog() == true ? dg.FileName : string.Empty;
        }

        public void ShowErrorMessage(string text)
        {
            MessageBox.Show(text);
        }
    }
}
