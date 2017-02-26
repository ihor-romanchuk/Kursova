using PoohMathParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<double> PartitionPoints { get; set; }
        public List<double> ColocationPoints { get; set; }
    }
}
