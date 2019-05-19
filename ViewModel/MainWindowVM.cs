using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public interface IConfirmUIService
    {
        bool Confirm(String text);
        // void ReportError(string message);
    }
    class MainWindowVM
    {
        ModelDataCollectionVM dataView = null;
        public static RoutedCommand AddModelCommand = new RoutedCommand("AddModel", typeof(_6Sem_Lab2.MainWindow));
        public static RoutedCommand DrawCommand = new RoutedCommand("Draw", typeof(_6Sem_Lab2.MainWindow));

        public MainWindowVM()
        {
            InitializeComponent();
            dataView = new ModelDataView(FindResource("key_ObsModelData") as ObservableModelData);
            boundsStack.DataContext = dataView;
        }

        private void CommandNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (dataView.modelDatas.HasChanged)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Warning", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    CommandSave_Executed(this, null);
                }
            }
            Resources["key_ObsModelData"] = new ObservableModelData();
            dataView.modelDatas = FindResource("key_ObsModelData") as ObservableModelData;
        }

        private void CommandOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (dataView.modelDatas.HasChanged)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Warning", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
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
                    dataView.modelDatas.HasChanged = false;
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
            dataView.modelDatas.Remove_At(modelsList.SelectedIndex);
        }

        private void CommandSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (dataView != null)
            {
                e.CanExecute = dataView.modelDatas.HasChanged;
            }
        }

        private void isItemSelected_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (modelsList != null)
            {
                e.CanExecute = (modelsList.SelectedIndex != -1);
            }
        }

        private void CommandAddModel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TryFindResource("key_DummyModel") is ModelData dummy_model_ref)
                dataView.modelDatas.Add_ModelData(new ModelData(dummy_model_ref.NodeCount, dummy_model_ref.Parameter));
        }

        private void CommandDraw_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dataView.Draw(chart, dataView.modelDatas[modelsList.SelectedIndex], dataView.modelDatas.Farthest(modelsList.SelectedIndex));
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

        private void addDefaults_Click(object sender, RoutedEventArgs e) => dataView.modelDatas.AddDefaults();

        private void CommandDraw_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (modelsList != null)
            {
                e.CanExecute = (modelsList.SelectedIndex != -1);
            }
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
            if (dataView.modelDatas.HasChanged)
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
                bf.Serialize(fs, dataView.modelDatas);
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
                dataView.modelDatas = FindResource("key_ObsModelData") as ObservableModelData;
                dataView.modelDatas.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => dataView.modelDatas.HasChanged = true;
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
