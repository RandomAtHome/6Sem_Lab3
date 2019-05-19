using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public interface IConfirmUIService
    {
        bool ConfirmAction(String text, String title);
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

        private readonly IConfirmUIService ui;

        public ModelDataCollectionVM DataView { get => dataView;
            private set
            {
                dataView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DataView"));
            }
        }

        public int SelectedIndexInList { get => selectedIndexInList; set => selectedIndexInList = value; }

        public MainWindowVM()
        {}

        private void CommandNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataView.ModelDatas.HasChanged)
            {
                if (ui.ConfirmAction("Do you want to save changes?", "Warning"))
                {
                    CommandSave_Executed(this, null);
                }
            }
            DataView = new ModelDataCollectionVM(new ObservableModelData());
        }

        private void CommandOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataView.ModelDatas.HasChanged)
            {
                if (ui.ConfirmAction("Do you want to save changes?", "Warning"))
                {
                    CommandSave_Executed(this, null);
                }
            }
            var dg = new Microsoft.Win32.OpenFileDialog();
            if (dg.ShowDialog() == true)
            {
                try
                {
                    Load(dg.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to open file!");
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
            DataView.ModelDatas.Remove_At(modelsList.SelectedIndex);
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
            DataView.Draw(chart, DataView.ModelDatas[modelsList.SelectedIndex], DataView.ModelDatas.Farthest(modelsList.SelectedIndex));
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

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DataView.ModelDatas.HasChanged)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Warning", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    CommandSave_Executed(this, null);
                }
            }
        }

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
