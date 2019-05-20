using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using Model;

namespace ViewModel
{
    [Serializable]
    public class ModelDataCollectionVM : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableModelData modelDatas = null;
        private double _x1 = 0.0;
        private double _x2 = 1.0;

        public double X1 { get => _x1;
            set
            {
                _x1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X1"));
            }
        }
        public double X2 { get => _x2;
            set
            {
                _x2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X2"));
            }
        }

        public string Error => throw new System.NotImplementedException();

        public ObservableModelData ModelDatas { get => modelDatas;
            private set
            {
                modelDatas = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ModelDatas"));
            }
        }

        public string this[string columnName]
        {
            get
            {
                string message = string.Empty;
                switch (columnName)
                {
                    case "X1":
                        if (X1 < 0.0)
                        {
                            message = "Left bound is less than 0.0!";
                        }
                        if (X1 >= X2)
                        {
                            message = "Left bound is bigger than right bound!";
                        }
                        break;
                    case "X2":
                        if (X2 > 1.0)
                        {
                            message = "Right bound is bigger than 1.0!";
                        }
                        if (X2 <= X1)
                        {
                            message = "Right bound if less than left bound!";
                        }
                        break;
                    default:
                        break;
                }
                return message;
            }
        }
        public bool HasErrors
        {
            get
            {
                return this["X1"].Length != 0 || this["X2"].Length != 0;
            }
        }
        public ModelDataCollectionVM(ObservableModelData modelDatas)
        {
            this.ModelDatas = modelDatas;
        }

        public void Draw(Chart chart, ModelDataVM md1, ModelDataVM md2)
        {
            chart.ChartAreas["notScaledArea"].AxisX.LabelStyle.Format = "F3";
            chart.ChartAreas["notScaledArea"].AxisY.LabelStyle.Format = "F3";
            chart.ChartAreas["scaledArea"].AxisX.LabelStyle.Format = "F3";
            chart.ChartAreas["scaledArea"].AxisY.LabelStyle.Format = "F3";
            Series series1 = chart.Series["md1NotScaled"];
            Series series2 = chart.Series["md2NotScaled"];
            series1.Points.DataBindXY(md1.Nodes, md1.NodeValues);
            series2.Points.DataBindXY(md2.Nodes, md2.NodeValues);
            series1.LegendText = "p = " + md1.Parameter.ToString();
            series2.LegendText = "p = " + md2.Parameter.ToString();
            Series series3 = chart.Series["md1Scaled"];
            Series series4 = chart.Series["md2Scaled"];
            series3.Points.DataBindXY(md1.XinBounds(X1, X2), md1.YinBounds(X1, X2));
            series4.Points.DataBindXY(md2.XinBounds(X1, X2), md2.YinBounds(X1, X2));
            for (int i = 0; i < series3.Points.Count; i++)
            {
                series3.Points[i].ToolTip =
                    "p = " + md1.Parameter.ToString() +
                    "\ny = " + series3.Points[i].YValues[0].ToString("F3");
            }
            for (int i = 0; i < series4.Points.Count; i++)
            {
                series4.Points[i].ToolTip =
                    "p = " + md2.Parameter.ToString() +
                    "\ny = " + series4.Points[i].YValues[0].ToString("F3");
            }
        }
    }

    [Serializable]
    public class ObservableModelData : ObservableCollection<ModelDataVM>
    {
        private bool _hasChanged;

        public ObservableModelData()
        {
            CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => HasChanged = true;
        }
        public bool HasChanged
        {
            get => _hasChanged;
            set => _hasChanged = value;
        }

        public void Add_ModelData(ModelDataVM modelDataVM) => Add(modelDataVM);

        public void Remove_At(int index) => RemoveAt(index);

        public void AddDefaults()
        {
            Add_ModelData(new ModelDataVM(new ModelData(10, -2)));
            Add_ModelData(new ModelDataVM(new ModelData(3, 2)));
            Add_ModelData(new ModelDataVM(new ModelData(24, 3)));
            Add_ModelData(new ModelDataVM(new ModelData(24, 5)));
            Add_ModelData(new ModelDataVM(new ModelData(50, 0)));
            Add_ModelData(new ModelDataVM(new ModelData(100, 5)));
            Add_ModelData(new ModelDataVM(new ModelData(24, -6)));
        }

        public ModelDataVM Farthest(int index) => (from item in Items
                                                 orderby Math.Abs(item.Parameter - Items[index].Parameter) descending
                                                 select item).First();

        public override string ToString()
        {
            string result = "Total items: " + Count + "\n";
            result += "Parameters of Data:\n";
            foreach (var item in Items)
            {
                result += item.Parameter + "\n";
            }
            return result;
        }
    }
}
