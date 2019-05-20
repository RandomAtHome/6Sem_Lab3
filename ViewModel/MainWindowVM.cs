using Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;

namespace ViewModel
{
    public interface IUIService
    {
        Chart UIChart { get; }

        bool ConfirmAction(string text, string title);

        string SaveFileDGName();
        string OpenFileDGName();
        void ShowErrorMessage(string text);
        // void ReportError(string message);
    }
    class MainWindowVM : INotifyPropertyChanged
    {
        ModelDataCollectionVM dataView = new ModelDataCollectionVM(new ObservableModelData());
        ModelDataInputVM newModelInputView = new ModelDataInputVM();
        int selectedIndexInList = -1;

        public ICommand AddModelCommand { get; private set; }
        public ICommand DrawCommand { get; private set; }

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
            DrawCommand = new RelayCommand(
                _ => CommandDraw_CanExecute(_),
                _ => CommandDraw_Executed(_)
            );
            AddModelCommand = new RelayCommand(
                _ => CommandAddModel_CanExecute(_),
                _ => CommandAddModel_Executed(_)
            );
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
            string FileName = ui.OpenFileDGName();
            if (FileName.Length != 0)
            {
                try
                {
                    Load(FileName);
                }
                catch (Exception)
                {
                    ui.ShowErrorMessage("Failed to open file!");
                }
            }
        }

        private void CommandSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string FileName = ui.SaveFileDGName();
            if (FileName.Length!= 0)
            {
                try
                {
                    DataView.ModelDatas.HasChanged = false;
                    Save(FileName);
                }
                catch (Exception)
                {
                    ui.ShowErrorMessage("Failed to save file!");
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

        private void CommandAddModel_Executed(object sender)
        {
            DataView.ModelDatas.Add_ModelData(new ModelDataVM(
                new ModelData(int.Parse(newModelInputView.NodeCount), double.Parse(newModelInputView.NodeCount))
            ));
        }

        private void CommandDraw_Executed(object sender)
        {
            DataView.Draw(ui.UIChart, DataView.ModelDatas[SelectedIndexInList], DataView.ModelDatas.Farthest(SelectedIndexInList));
        }

        private bool CommandAddModel_CanExecute(object sender)
        {
            pInput.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            nodeCountInput.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            foreach (FrameworkElement child in newModelStack.Children)
            {
                if (Validation.GetHasError(child))
                {
                    return false;
                }
            }
            return true;
        }

        private void addDefaults_Click(object sender, RoutedEventArgs e) => DataView.ModelDatas.AddDefaults();

        private bool CommandDraw_CanExecute(object sender)
        {
            bool result = (SelectedIndexInList != -1);
            if (result && boundsStack != null)
            {
                foreach (FrameworkElement child in boundsStack.Children)
                {
                    if (Validation.GetHasError(child))
                    {
                        return false;
                    }
                }
            }
            return result;
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
                ui.ShowErrorMessage("Exception: " + ex.Message);
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
                DataView = new ModelDataCollectionVM(bf.Deserialize(fs) as ObservableModelData);
                DataView.ModelDatas.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => DataView.ModelDatas.HasChanged = true;
                result = true;
            }
            catch (Exception ex)
            {
                ui.ShowErrorMessage("Exception: " + ex.Message);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            return result;
        }
    }
}
