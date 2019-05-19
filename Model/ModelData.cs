using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Model
{
    [Serializable]
    public class ModelData : IDataErrorInfo
    {
        static double pMin = -10.0;
        static double pMax = 10.0;
        void F(int n, double p)
        {
            NodeValues[n] = Math.Sin(p * Math.PI * Nodes[n]);
            //NodeValues[n] = Nodes[n] * Nodes[n] * p;
        }

        public int NodeCount { get; private set; }
        public double Parameter { get; private set; }
        public double[] Nodes { get; private set; }
        public double[] NodeValues { get; private set; }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string message = string.Empty;
                switch (columnName)
                {
                    case "Parameter":
                        if (pMin > Parameter || Parameter > pMax)
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
                return message;
            }
        }

        public ModelData(int node_count, double p)
        {
            NodeCount = node_count;
            Parameter = p;
            Nodes = new double[node_count];
            NodeValues = new double[node_count];
            double step = 1.0 / (node_count - 1);
            for (int i = 0; i < NodeCount; i++)
            {
                Nodes[i] = step * i;
                F(i, p);
            }
        }
        public ModelData()
        {
            Parameter = pMin;
        }

        public double[] XinBounds(double x1, double x2) => (from node in Nodes
                                                            where x1 <= node && node <= x2
                                                            select node).ToArray();
        public double[] YinBounds(double x1, double x2)
        {
            List<double> answ = new List<double>();
            for (int i = 0; i < NodeCount; i++)
            {
                if (Nodes[i] > x2)
                {
                    break;
                }
                if (Nodes[i] < x1)
                {
                    continue;
                }
                answ.Add(NodeValues[i]);
            }
            return answ.ToArray();
        }
    }
}
