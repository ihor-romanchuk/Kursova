using PoohMathParser;
using System;
using System.Collections.Generic;

namespace Common
{
    public class Settings
    {
        private List<double> _partitionPoints;
        private List<double> _colocationPoints;

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

        public MathExpression FunctionG { get; set; }
        public int PartitionsOnCrack { get; set; }
        public int PartitionsOnBound { get; set; }
        public int MeshSize { get; set; }
        public double Lambda { get; set; }


        public List<double> PartitionPoints
        {
            get
            {
                if(_partitionPoints == null)
                {
                    _partitionPoints = GeneratePartitionPoints();
                }

                return _partitionPoints;
            }
            set
            {
                _partitionPoints = value;
            }
        }
        public List<double> ColocationPoints
        {
            get
            {
                if (_colocationPoints == null)
                {
                    _colocationPoints = GenerateColocationPoints();
                }

                return _colocationPoints;
            }
            set
            {
                _colocationPoints = value;
            }
        }

        public List<double> GeneratePartitionPoints(double? left = null, double? right = null, int? amountOfPartitions = null)
        {
            var partitionPoints = new List<double>();
            double step = Math.Abs((right ?? IntervalOfIntegration.Item2) - (left ?? IntervalOfIntegration.Item1)) / amountOfPartitions ?? AmountOfPartitions;

            for (int i = 0; i <= (amountOfPartitions ?? AmountOfPartitions); i++)
            {
                partitionPoints.Add((left ?? IntervalOfIntegration.Item1) + i * step);
            }

            return partitionPoints;
        }
        public List<double> GenerateColocationPoints(double? left = null, double? right = null, int? amountOfPartitions = null)
        {
            var colocationPoints = new List<double>();
            List<double> partitionPoints = GeneratePartitionPoints(left, right, amountOfPartitions);

            for (int i = 0; i < partitionPoints.Count - 1; i++)
            {
                colocationPoints.Add((partitionPoints[i] + partitionPoints[i + 1]) / 2.0);
            }

            return colocationPoints;
        }
    }
}
