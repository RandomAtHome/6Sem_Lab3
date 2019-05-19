using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ViewModel
{
    public interface IUIService
    {
        Chart UIChart { get; }

        bool ConfirmAction(String text, String title);
        void ShowErrorMessage(String text);
        // void ReportError(string message);
    }
    class MainWindowVM : INotifyPropertyChanged
    {
        ModelDataCollectionVM dataView = new ModelDataCollectionVM(new ObservableModelData());
        ModelDataInputVM newModelInputView = new ModelDataInputVM();
        int selectedIndexInList = -1; 

        public static RoutedCommand AddModelCommand = new RoutedCommand("AddModel", typeof(_6Sem_Lab2.MainWindow));
        public static RoutedCommand DrawCommand = new RoutedCommand("Draw", typeof(_6Sem_Lab2.MainWindow));
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IUIService ui = null;

        public ModelDataCollectionVM DataView { get => dataView;
            private set
            {
                dataView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DataView"));
            }
        }

        public int SelectedIndexInList { get => selectedIndexInList; set => selectedIndexInList = value; }

        public MainWindowVM(IUIService ui)
        {
            this.ui = ui;
        }

        private void SaveIfChanged()
        {
            if (DataView.ModelDatas.HasChanged)
            {
                if (ui.ConfirmAction("Do you want to save changes?", "Warning"))
                {
                    CommandSave_Executed(this, null);
                }
            }
        }
        private void CommandNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveIfChanged();
            DataView = new ModelDataCollectionVM(new ObservableModelData());
        }

        private void CommandOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveIfChanged();
            var dg = new Microsoft.Win32.OpenFileDialog();
            if (dg.ShowDialog() == true)
            {
                try
                {
                    Load(dg.FileName);
                }
                catch (Exception)
                {
                    ui.ShowErrorMessage("Failed to open file!");
                }
            }
        }

        private void CommandSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dg = new Microsoft.Win32.SaveFileDialog();
            if (dg.ShowDialog() == true)
            {
                try
                {
                    DataView.ModelDatas.HasChanged = false;
                    Save(dg.FileName);
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Failed to save file!");
                }
            }
        }

        private void CommandDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataView.ModelDatas.Remove_At(SelectedIndexInList);
        }

        private void CommandSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DataView.ModelDatas.HasChanged;
        }

        private void isItemSelected_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
               e.CanExecute = (SelectedIndexInList != -1);
        }

        private void CommandAddModel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataView.ModelDatas.Add_ModelData(new ModelData(int.Parse(newModelInputView.NodeCount), double.Parse(newModelInputView.NodeCount)));
        }

        private void CommandDraw_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataView.Draw(ui.UIChart, DataView.ModelDatas[SelectedIndexInList], DataView.ModelDatas.Farthest(SelectedIndexInList));
        }

        private void CommandAddModel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (pInput == null || nodeCountInput == null)
            {
                e.CanExecute = false;
                return;
            }
            pInput.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            nodeCountInput.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            foreach (FrameworkElement child in newModelStack.Children)
            {
                if (Validation.GetHasError(child))
                {
                    e.CanExecute = false;
                    return;
                }
            }
            e.CanExecute = true;
        }

        private void addDefaults_Click(object sender, RoutedEventArgs e) => DataView.ModelDatas.AddDefaults();

        private void CommandDraw_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (SelectedIndexInList != -1);
            if (e.CanExecute && boundsStack != null)
            {
                foreach (FrameworkElement child in boundsStack.Children)
                {
                    if (Validation.GetHasError(child))
                    {
                        e.CanExecute = false;
                        return;
                    }
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e) => SaveIfChanged();

        public bool Save(string filename)
        {
            FileStream fs = null;
            bool res = false;
            try
            {
                fs = File.Create(filename);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, DataView.ModelDatas);
                res = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            return res;
        }

        public bool Load(string filename)
        {
            bool result = false;
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(filename);
                BinaryFormatter bf = new BinaryFormatter();
                Resources["key_ObsModelData"] = bf.Deserialize(fs) as ObservableModelData;
                DataView.ModelDatas = FindResource("key_ObsModelData") as ObservableModelData;
                DataView.ModelDatas.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => DataView.ModelDatas.HasChanged = true;
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            return result;
        }
    }
}
