﻿using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class ModelDataVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private ModelData modelData;
        public event PropertyChangedEventHandler PropertyChanged;
        public double Parameter { get => modelData.Parameter; }
        public int NodeCount { get => modelData.NodeCount; }
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
