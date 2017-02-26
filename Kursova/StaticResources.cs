using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoohMathParser;

namespace Kursova
{
    public class StaticResources
    {
        public Lazy<Dictionary<EquationsEnum, Settings>> DefaultSettings { get; private set; }
        public Lazy<Dictionary<EquationsEnum, Func<Settings, int, int, double>>> MatrixAInits { get; private set; }
        public Lazy<Dictionary<EquationsEnum, Func<Settings, int, double>>> MatrixBInits { get; private set; }

        public StaticResources()
        {
            DefaultSettings = new Lazy<Dictionary<EquationsEnum, Settings>>(() => GetDefaultSettings());
            MatrixAInits = new Lazy<Dictionary<EquationsEnum, Func<Settings, int, int, double>>>(() => GetMatrixAInits());
            MatrixBInits = new Lazy<Dictionary<EquationsEnum, Func<Settings, int, double>>>(() => GetMatrixBInits());
        }

        private Dictionary<EquationsEnum, Settings> GetDefaultSettings()
        {
            var result = new Dictionary<EquationsEnum, Settings>();

            result.Add(EquationsEnum.Equation_3_2, new Settings
            {
                AmountOfPartitions = 10,
                IntervalOfFunction = new Tuple<double, double>(-1, 1),
                IntervalOfIntegration = new Tuple<double, double>(-1, 1),
                FunctionDistance = new MathExpression("abs(t-s)"),
                FunctionF = new MathExpression("1"),
                FunctionYakobian = new MathExpression("1"),
                Variables = new List<string> { "t", "s" }
            });

            result.Add(EquationsEnum.Equation_3_3, new Settings
            {
                AmountOfPartitions = 10,
                IntervalOfFunction = new Tuple<double, double>(0, 2 * Math.PI),
                IntervalOfIntegration = new Tuple<double, double>(0, 2 * Math.PI),
                FunctionDistance = new MathExpression("((2*a^2)*(1-cos(t-s)))^(1/2)"),
                FunctionF = new MathExpression("1"),
                FunctionYakobian = new MathExpression("a"),
                Radius = 9,
                Variables = new List<string> { "t", "s" }
            });

            //result.Add(EquationsEnum.Equation_3_4, new Settings
            //{
            //    AmountOfPartitions = 10,
            //    IntervalOfFunction = new Tuple<double, double>(0, 2 * Math.PI),
            //    IntervalOfIntegration = new Tuple<double, double>(-1, 1),
            //    FunctionDistance = new MathExpression("todo"),
            //    FunctionF = new MathExpression("1"),
            //    FunctionYakobian = new MathExpression("a"),
            //    Radius = 9,
            //    Variables = new List<string> { "t", "s" }
            //});

            //result.Add(EquationsEnum.Equation_3_5, new Settings
            //{
            //    AmountOfPartitions = 10,
            //    IntervalOfFunction = new Tuple<double, double>(-1, 1),
            //    IntervalOfIntegration = new Tuple<double, double>(0, 2 * Math.PI),
            //    FunctionDistance = new MathExpression("todo"),
            //    FunctionF = new MathExpression("1"),
            //    FunctionYakobian = new MathExpression("a"),
            //    Radius = 9,
            //    Variables = new List<string> { "t", "s" }
            //});

            return result;
        }
        private Dictionary<EquationsEnum, Func<Settings, int, int, double>> GetMatrixAInits()
        {
            var result = new Dictionary<EquationsEnum, Func<Settings, int, int, double>>();

            result.Add(EquationsEnum.Equation_3_2, (settings, i, j) =>
            {
                j++;
                if (settings.PartitionPoints[j - 1] > settings.ColocationPoints[i])
                {
                    return -((settings.PartitionPoints[j] - settings.ColocationPoints[i]) * Math.Log(settings.PartitionPoints[j] - settings.ColocationPoints[i]) - (settings.PartitionPoints[j - 1] - settings.ColocationPoints[i]) * Math.Log(settings.PartitionPoints[j - 1] - settings.ColocationPoints[i]) - (settings.PartitionPoints[j] - settings.PartitionPoints[j - 1]));
                }
                else
                {
                    if (settings.PartitionPoints[j] < settings.ColocationPoints[i])
                    {
                        return -(-(-settings.PartitionPoints[j] + settings.ColocationPoints[i]) * Math.Log(-settings.PartitionPoints[j] + settings.ColocationPoints[i]) + (-settings.PartitionPoints[j - 1] + settings.ColocationPoints[i]) * Math.Log(-settings.PartitionPoints[j - 1] + settings.ColocationPoints[i]) - (settings.PartitionPoints[j] - settings.PartitionPoints[j - 1]));
                    }
                    else
                    {
                        return -((-settings.PartitionPoints[j - 1] + settings.ColocationPoints[i]) * Math.Log(-settings.PartitionPoints[j - 1] + settings.ColocationPoints[i]) + (settings.PartitionPoints[j] - settings.ColocationPoints[i]) * Math.Log(settings.PartitionPoints[j] - settings.ColocationPoints[i]) - (settings.PartitionPoints[j] - settings.PartitionPoints[j - 1]));
                    }
                }
            });

            result.Add(EquationsEnum.Equation_3_3, (settings, i, j) =>
            {
                j++;
                if (i == (j - 1))
                {
                    return (2 * FredholmEquationFirstOrder.Calculate_Aij(settings.PartitionPoints, settings.ColocationPoints, i, j) - Math.Log(settings.Radius.Value) *
                        Math.Abs(settings.IntervalOfIntegration.Item2 - settings.IntervalOfIntegration.Item1) / settings.AmountOfPartitions +
                        GaussMethodForIntegrals.CalculateWithAccuracy(settings.PartitionPoints[j - 1], settings.PartitionPoints[j],
                        string.Format("ln(1/(2*{1}*(1-cos(x-{0}))))-ln(1/({1}*(x-{0})^2))", settings.ColocationPoints[i], settings.Radius.Value), 0.001));
                }
                else
                {
                    return (GaussMethodForIntegrals.CalculateWithAccuracy(settings.PartitionPoints[j - 1], settings.PartitionPoints[j],
                        string.Format("ln(1/(2*{1}*(1-cos(x-{0}))))", settings.ColocationPoints[i], settings.Radius.Value), 0.001));
                }
            });

            result.Add(EquationsEnum.Equation_3_4, (settings, i, j) => 1);

            result.Add(EquationsEnum.Equation_3_5, (settings, i, j) => 1);

            return result;
        }
        private Dictionary<EquationsEnum, Func<Settings, int, double>> GetMatrixBInits()
        {
            var result = new Dictionary<EquationsEnum, Func<Settings, int, double>>();

            result.Add(EquationsEnum.Equation_3_2, (settings, i) => settings.FunctionF.Calculate(settings.ColocationPoints[i]));
            result.Add(EquationsEnum.Equation_3_3, (settings, i) => settings.FunctionF.Calculate(settings.ColocationPoints[i]));
            result.Add(EquationsEnum.Equation_3_4, (settings, i) => settings.FunctionF.Calculate(settings.ColocationPoints[i]));
            result.Add(EquationsEnum.Equation_3_5, (settings, i) => settings.FunctionF.Calculate(settings.ColocationPoints[i]));

            return result;
        }
    }
}
