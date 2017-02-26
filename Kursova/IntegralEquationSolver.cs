using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PoohMathParser;
using System;
using System.Collections.Generic;

namespace Kursova
{
    public class IntegralEquationSolver
    {
        /// <summary>
        /// A*X=B . Returns X.
        /// </summary>
        /// <param name="rightPart"></param>
        /// <returns></returns>
        public Dictionary<double, double> Solve(Settings settings, Func<Settings, int, int, double> matrixAInit,
            Func<Settings, int, double> vectorBInit)
        {
            Matrix<double> matrixA = Matrix.Build.Dense(settings.ColocationPoints.Count, settings.ColocationPoints.Count, (i, j) => matrixAInit(settings, i, j));
            Vector<double> vectorB = Vector.Build.Dense(settings.ColocationPoints.Count, i => vectorBInit(settings, i));
            Vector<double> vectorX = matrixA.Solve(vectorB);

            Dictionary<double, double> result = new Dictionary<double, double>();
            for (int i = 0; i < settings.ColocationPoints.Count; i++)
            {
                result.Add(settings.ColocationPoints[i], vectorX[i]);
            }
            return result;
        }
    }
}
