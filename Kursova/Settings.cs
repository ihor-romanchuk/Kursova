using PoohMathParser;
using System;
using System.Collections.Generic;

namespace Kursova
{
    public class Settings
    {
        /// <summary>
        /// Bounds of t.
        /// </summary>
        public Tuple<double, double> IntervalOfIntegration { get; set; }

        /// <summary>
        /// Bounds of s.
        /// </summary>
        public Tuple<double, double> IntervalOfFunction { get; set; }

        public int AmountOfPartitions { get; set; }
        public double? Radius { get; set; }
        public MathExpression FunctionF { get; set; }
        public MathExpression FunctionDistance { get; set; }
        public MathExpression FunctionYakobian { get; set; }
        public List<string> Variables { get; set; }
        public List<double> PartitionPoints
        {
            get
            {
                var partitionPoints = new List<double>();
                double step = Math.Abs(IntervalOfIntegration.Item2 - IntervalOfIntegration.Item1) / AmountOfPartitions;

                for (int i = 0; i <= AmountOfPartitions; i++)
                {
                    partitionPoints.Add(IntervalOfIntegration.Item1 + i * step);
                }

                return partitionPoints;
            }
        }
        public List<double> ColocationPoints
        {
            get
            {
                var colocationPoints = new List<double>();
                List<double> partitionPoints = PartitionPoints;

                for (int i = 0; i < partitionPoints.Count - 1; i++)
                {
                    colocationPoints.Add((partitionPoints[i] + partitionPoints[i + 1]) / 2.0);
                }

                return colocationPoints;
            }
        }
    }
}
