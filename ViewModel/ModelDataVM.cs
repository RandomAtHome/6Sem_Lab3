using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    class ModelDataInputVM : IDataErrorInfo
    {
        private string parameter = ModelData.PMin.ToString(), nodeCount = "3";
        public string Error => throw new NotImplementedException();

        public string Parameter { get => parameter; set => parameter = value; }
        public string NodeCount { get => nodeCount; set => nodeCount = value; }

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
    }
}
