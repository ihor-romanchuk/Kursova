using Common.CustomIntegrals;
using PoohMathParser;
using System;
using System.Collections.Generic;

namespace Common
{
    public class IntegralSystemSolver
    {
        private readonly IntegralEquationSolver _integralEquationSolver;

        public IntegralSystemSolver(): this(new IntegralEquationSolver()) { }
        public IntegralSystemSolver(IntegralEquationSolver integralEquationSolver)
        {
            _integralEquationSolver = integralEquationSolver;
        }

        public Dictionary<double, double> CalculateVector(Settings settings)
        {
            settings.PartitionPoints = settings.GeneratePartitionPoints(-1, 1, settings.PartitionsOnCrack);
            settings.PartitionPoints.AddRange(settings.GeneratePartitionPoints(0, 2 * Math.PI, settings.PartitionsOnBound));

            settings.ColocationPoints = settings.GenerateColocationPoints(-1, 1, settings.PartitionsOnCrack);
            settings.ColocationPoints.AddRange(settings.GenerateColocationPoints(0, 2 * Math.PI, settings.PartitionsOnBound));

            var integral_1_1 = new Integral_1_1();
            var integral_1_2 = new Integral_1_2(settings.Radius.Value);
            var integral_2_1 = new Integral_2_1(settings.Radius.Value);
            var integral_2_2 = new Integral_2_2(integral_1_1, settings.Radius.Value);

            Func<Settings, int, int, double> matrixAInit = (_settings, i, j) =>
            {
                ICustomIntegral integral;
                Func<double, double> lambda = p => 1;
                if (i < _settings.PartitionsOnCrack)
                {
                    if (j < _settings.PartitionsOnCrack)
                    {
                        integral = integral_1_1;
                    }
                    else
                    {
                        j++;
                        integral = integral_1_2;
                    }
                    lambda = p => _settings.Lambda.Calculate(p);
                }
                else
                {
                    if (j < _settings.PartitionsOnCrack)
                    {
                        integral = integral_2_1;
                    }
                    else
                    {
                        j++;
                        integral = integral_2_2;
                    }
                }

                int handleTao = i < _settings.PartitionsOnCrack && j < _settings.PartitionsOnCrack && i == j ? 1 : 0;
                return integral.Calculate(_settings.PartitionPoints[j], _settings.PartitionPoints[j + 1], _settings.ColocationPoints[i]) * lambda(_settings.ColocationPoints[i]) + handleTao;
            };

            Func<Settings, int, double> matrixBInit = (_settings, i) =>
            {
                if (i < _settings.PartitionsOnCrack)
                {
                    return _settings.FunctionF.Calculate(_settings.ColocationPoints[i]);
                }

                return _settings.FunctionG.Calculate(_settings.ColocationPoints[i]);
            };

            var result = _integralEquationSolver.Solve(settings, matrixAInit, matrixBInit);

            return result;
        }

        public double Solve(Settings settings, List<double> taoValues, List<double> sigmaValues, double x, double y)
        {
            double result = 0;
            var firstMathExpression = new MathExpression($"ln((sqrt({Math.Pow(y, 2)}+({x}-t)^2))^(0-1))");
            var secondMathExpression = new MathExpression($"a*ln((sqrt(({x}-a*cos(t))^2+({y}-a*sin(t))^2))^(0-1))");
            for (int j = 0; j < settings.PartitionsOnCrack; j++)
            {
                result += GaussMethodForIntegrals.CalculateWithAccuracy(settings.PartitionPoints[j], settings.PartitionPoints[j + 1], firstMathExpression, Constants.Epsilon, new Var("t", 0), new Var("s", settings.ColocationPoints[j])) * taoValues[j];
            }
            for (int j = 0; j < settings.PartitionsOnBound; j++)
            {
                var partitionIndex = j + settings.PartitionsOnCrack + 1;
                result += settings.Radius.Value * GaussMethodForIntegrals.CalculateWithAccuracy(settings.PartitionPoints[partitionIndex], settings.PartitionPoints[partitionIndex + 1], secondMathExpression, Constants.Epsilon, new Var("t", 0), new Var("s", settings.ColocationPoints[j + settings.PartitionsOnCrack]), new Var("a", settings.Radius.Value)) * sigmaValues[j];
            }

            return 1 / (2 * Math.PI) * result;
        }
    }
}
