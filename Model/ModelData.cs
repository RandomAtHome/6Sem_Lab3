using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Model
{
    [Serializable]
    public class ModelData
    {
        private static double pMin = -10.0;
        private static double pMax = 10.0;
        void F(int n, double p)
        {
            NodeValues[n] = Math.Sin(p * Math.PI * Nodes[n]);
            //NodeValues[n] = Nodes[n] * Nodes[n] * p;
        }

        public int NodeCount { get; private set; }
        public double Parameter { get; private set; }
        public double[] Nodes { get; private set; }
        public double[] NodeValues { get; private set; }
        public static double PMin { get => pMin; private set => pMin = value; }
        public static double PMax { get => pMax; private set => pMax = value; }

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
