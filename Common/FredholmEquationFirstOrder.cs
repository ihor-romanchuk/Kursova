using System;
using System.Collections.Generic;
using PoohMathParser;

namespace Common
{
    public static class FredholmEquationFirstOrder
    {
        public static Dictionary<double, double> Calculate(double intervalLeft, double intervalRight, double amountOfPartitions, string func)
        {
            List<double> partitionPoints = new List<double>();
            List<double> colocationPoints = new List<double>();

            double step = Math.Abs(intervalRight - intervalLeft) / amountOfPartitions;

            for (int i = 0; i <= amountOfPartitions; i++)
            {
                partitionPoints.Add(intervalLeft + i * step);
            }

            for (int i = 0; i < partitionPoints.Count - 1; i++)
            {
                colocationPoints.Add((partitionPoints[i] + partitionPoints[i + 1]) / 2.0);
            }

            List<List<double>> A = Calculate_A(partitionPoints, colocationPoints);
            List<double> F_j = Calculate_Fj(func, colocationPoints);
            List<double> C_k = GaussMethodForSystems.Calculate(A, F_j);

            Dictionary<double, double> result = new Dictionary<double, double>();

            for (int i = 0; i < colocationPoints.Count; i++)
            {
                result.Add(colocationPoints[i], C_k[i]);
            }

            return result;
        }

        public static double Calculate_Aij(List<double> partitionPoints, List<double> colocationPoints, int i, int j)
        {
            if (partitionPoints[j - 1] > colocationPoints[i])
            {
                return -((partitionPoints[j] - colocationPoints[i]) * Math.Log(partitionPoints[j] - colocationPoints[i]) - (partitionPoints[j - 1] - colocationPoints[i]) * Math.Log(partitionPoints[j - 1] - colocationPoints[i]) - (partitionPoints[j] - partitionPoints[j - 1]));
            }
            else
            {
                if (partitionPoints[j] < colocationPoints[i])
                {
                    return -(-(-partitionPoints[j] + colocationPoints[i]) * Math.Log(-partitionPoints[j] + colocationPoints[i]) + (-partitionPoints[j - 1] + colocationPoints[i]) * Math.Log(-partitionPoints[j - 1] + colocationPoints[i]) - (partitionPoints[j] - partitionPoints[j - 1]));
                }
                else
                {
                    return -((-partitionPoints[j - 1] + colocationPoints[i]) * Math.Log(-partitionPoints[j - 1] + colocationPoints[i]) + (partitionPoints[j] - colocationPoints[i]) * Math.Log(partitionPoints[j] - colocationPoints[i]) - (partitionPoints[j] - partitionPoints[j - 1]));
                }
            }
        }
        public static List<List<double>> Calculate_A(List<double> partitionPoints, List<double> colocationPoints)
        {
            List<List<double>> result = new List<List<double>>();

            for (int i = 0; i < colocationPoints.Count; i++)
            {
                result.Add(new List<double>());

                for (int j = 1; j < partitionPoints.Count; j++)
                {
                    result[i].Add(Calculate_Aij(partitionPoints, colocationPoints, i, j));
                }
            }

            return result;
        }

        public static List<double> Calculate_Fj(string func, List<double> points)
        {
            List<double> result = new List<double>();

            MathExpression function = new MathExpression(func);

            for (int i = 0; i < points.Count; i++)
            {
                result.Add(function.Calculate(points[i]));
            }

            return result;
        }
    }
}
