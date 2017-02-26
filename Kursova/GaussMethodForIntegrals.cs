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
        public static double Calculate(double a, double b, int amountOfPartitions, string mathExpression)
        {
            List<double> Xk = new List<double>() { -0.8611363, -0.3399810, 0.3399810, 0.8611363 };
            List<double> Ck = new List<double>() { 0.3478548, 0.6521452, 0.6521452, 0.3478548 };

            MathExpression function = new MathExpression(mathExpression);
            double result = 0;

            double step = (b - a) / amountOfPartitions;

            for (int i = 0; i < amountOfPartitions; i++)
            {
                double a_new = a + step * i;
                double b_new = a + step * (i + 1);

                double sum = 0;
                for (int j = 0; j < Xk.Count; j++)
                {
                    sum += Ck[j] * function.Calculate((a_new + b_new) / 2.0 + (b_new - a_new) / 2.0 * Xk[j]);
                }

                result += sum * (b_new - a_new) / 2.0;
            }

            return result;
        }
        public static double CalculateWithAccuracy(double a, double b, string mathExpression, double epsilon)
        {
            List<double> Xk = new List<double>() { -0.8611363, -0.3399810, 0.3399810, 0.8611363 };
            List<double> Ck = new List<double>() { 0.3478548, 0.6521452, 0.6521452, 0.3478548 };

            MathExpression function = new MathExpression(mathExpression);
            double result;
            int n = 1;

            double newResult = GaussMethodForIntegrals.Calculate(a, b, n, mathExpression);

            do
            {
                n *= 2;
                result = newResult;
                newResult = GaussMethodForIntegrals.Calculate(a, b, n, mathExpression);                
            }
            while (System.Math.Abs(newResult - result) > epsilon);

            return newResult;
        }
    }
}
