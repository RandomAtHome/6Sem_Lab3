using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    [Serializable]
    public class ModelDataVM : IDataErrorInfo
    {
        private readonly ModelData modelData;
        public double Parameter { get => modelData.Parameter; }
        public int NodeCount { get => modelData.NodeCount; }
        public double[] Nodes => modelData.Nodes;
        public double[] NodeValues => modelData.NodeValues;
        public double[] XinBounds(double x1, double x2) => modelData.XinBounds(x1, x2);
        public double[] YinBounds(double x1, double x2) => modelData.YinBounds(x1, x2);
        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                string message = string.Empty;
                try
                {
                    switch (columnName)
                    {
                        case "Parameter":
                            if (ModelData.PMin > Parameter || Parameter > ModelData.PMax)
                            {
                                message = "P value is offbounds!";
                            }
                            break;
                        case "NodeCount":
                            if (NodeCount < 3)
                            {
                                message = "Node count is less than 3!";
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exc)
                {
                    message = exc.Message;
                }
                return message;
            }
        }

        public ModelDataVM(ModelData modelData)
        {
            this.modelData = modelData;
        }
    }
    public class ModelDataInputVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private string parameter = ModelData.PMin.ToString(), nodeCount = "3";

        public event PropertyChangedEventHandler PropertyChanged;
        public string Parameter
        {
            get => parameter;
            set
            {
                parameter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Parameter"));
            }
        }
        public string NodeCount
        {
            get => nodeCount;
            set
            {
                nodeCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NodeCount"));
            }
        }
        public bool HasErrors
        {
            get
            {
                return this["Parameter"].Length != 0 || this["NodeCount"].Length != 0;
            }
        } 
        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                string message = string.Empty;
                try
                {
                    switch (columnName)
                    {
                        case "Parameter":
                            if (ModelData.PMin > double.Parse(Parameter) || double.Parse(Parameter) > ModelData.PMax)
                            {
                                message = "P value is offbounds!";
                            }
                            break;
                        case "NodeCount":
                            if (int.Parse(NodeCount) < 3)
                            {
                                message = "Node count is less than 3!";
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exc)
                {
                    message = exc.Message;
                }
                return message;
            }
        }

        public ModelDataInputVM() { }
    }
}
