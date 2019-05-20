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
    }
    public class MainWindowVM : INotifyPropertyChanged
    {
        ModelDataCollectionVM dataView = new ModelDataCollectionVM(new ObservableModelData());
        public ModelDataCollectionVM DataView
        {
            get => dataView;
            private set
            {
                dataView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DataView"));
            }
        }
        ModelDataInputVM newModelInputView = new ModelDataInputVM();
        public ModelDataInputVM NewModelInputView { get => newModelInputView;
            set
            {
                newModelInputView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewModelInputView"));
            }
        }
        int selectedIndexInList = -1;
        public int SelectedIndexInList { get => selectedIndexInList; set => selectedIndexInList = value; }

        public ICommand AddModelCommand { get; private set; }
        public ICommand DrawCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand AddDefaultsCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IUIService ui = null;

        public MainWindowVM(IUIService ui)
        {
            this.ui = ui;
            DrawCommand = new RelayCommand(
                _ => !DataView.HasErrors && (SelectedIndexInList != -1),
                _ => DataView.Draw(ui.UIChart, DataView.ModelDatas[SelectedIndexInList], DataView.ModelDatas.Farthest(SelectedIndexInList))
            );
            AddModelCommand = new RelayCommand(
                _ => !NewModelInputView.HasErrors,
                _ => CommandAddModel_Executed(_)
            );
            NewCommand = new RelayCommand(
                _ => true,
                _ => CommandNew_Executed(_)
            );
            OpenCommand = new RelayCommand(
                _ => true,
                _ => CommandOpen_Executed(_)
            );
            SaveCommand = new RelayCommand(
                _ => DataView.ModelDatas.HasChanged,
                _ => CommandSave_Executed(_)
            );
            DeleteCommand = new RelayCommand(
                _ => SelectedIndexInList != -1,
                _ => DataView.ModelDatas.Remove_At(SelectedIndexInList)
            );
            AddDefaultsCommand = new RelayCommand(
                _ => true,
                _ => DataView.ModelDatas.AddDefaults()
            );
        }

        private void SaveIfChanged()
        {
            if (DataView.ModelDatas.HasChanged)
            {
                if (ui.ConfirmAction("Do you want to save changes?", "Warning"))
                {
                    CommandSave_Executed(this);
                }
            }
        }
        private void CommandNew_Executed(object sender)
        {
            SaveIfChanged();
            DataView = new ModelDataCollectionVM(new ObservableModelData());
        }

        private void CommandOpen_Executed(object sender)
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

        private void CommandSave_Executed(object sender)
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

        private void CommandAddModel_Executed(object sender)
        {
            try
            {
                DataView.ModelDatas.Add_ModelData(new ModelDataVM(
                    new ModelData(int.Parse(NewModelInputView.NodeCount), double.Parse(NewModelInputView.Parameter))
                ));
            } catch (Exception ex) {
                ui.ShowErrorMessage("Exception: " + ex.Message);
            }
        }

        public void Window_Closed(object sender, EventArgs e) => SaveIfChanged();

        private bool Save(string filename)
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

        private bool Load(string filename)
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
