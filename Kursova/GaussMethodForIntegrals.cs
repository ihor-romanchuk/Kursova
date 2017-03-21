using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoohMathParser;

namespace Kursova
{
    public static class GaussMethodForIntegrals
    {
        public static double Calculate(double a, double b, int amountOfPartitions, MathExpression function, params Var[] variables)
        {
            List<double> Xk = new List<double>() { -0.8611363, -0.3399810, 0.3399810, 0.8611363 };
            List<double> Ck = new List<double>() { 0.3478548, 0.6521452, 0.6521452, 0.3478548 };

            double result = 0;

            double step = (b - a) / amountOfPartitions;

            for (int i = 0; i < amountOfPartitions; i++)
            {
                double a_new = a + step * i;
                double b_new = a + step * (i + 1);

                double sum = 0;
                for (int j = 0; j < Xk.Count; j++)
                {
                    variables[0].Value = (a_new + b_new) / 2.0 + (b_new - a_new) / 2.0 * Xk[j];
                    sum += Ck[j] * function.Calculate(variables);
                }

                result += sum * (b_new - a_new) / 2.0;
            }

            return result;
        }

        public static double CalculateWithAccuracy(double a, double b, MathExpression function, double epsilon, params Var[] variables)
        {
            List<double> Xk = new List<double>() { -0.8611363, -0.3399810, 0.3399810, 0.8611363 };
            List<double> Ck = new List<double>() { 0.3478548, 0.6521452, 0.6521452, 0.3478548 };

            double result;
            int n = 1;

            double newResult = Calculate(a, b, n, function, variables);

            do
            {
                n *= 2;
                result = newResult;
                newResult = Calculate(a, b, n, function, variables);
            }
            while (System.Math.Abs(newResult - result) > epsilon);

            return newResult;
        }
    }
}
