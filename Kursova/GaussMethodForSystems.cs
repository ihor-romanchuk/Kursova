using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursova
{
    public static class GaussMethodForSystems
    {
        public static List<double> Calculate(List<List<double>> A_jk, List<double> F_j)
        {
            //A*X=B
            List<double> matrixX = new List<double>();

            List<List<double>> matrixA = CloneList(A_jk);
            List<double> matrixB = new List<double>(F_j);

            //Прямий хід
            for (int i = 0; i < matrixB.Count; i++)
            {
                if(matrixA[i][i] == 0)
                {
                    int l = i + 1;
                    while(l < matrixA.Count && matrixA[l][i] == 0)
                    {
                        l++;
                    }

                    if(l == matrixA.Count)
                    {
                        continue;
                    }
                    else
                    {
                        List<double> temp = matrixA[i];
                        matrixA[i] = matrixA[l];
                        matrixA[l] = temp;

                        double temp1 = matrixB[i];
                        matrixB[i] = matrixB[l];
                        matrixB[l] = temp1;
                    }
                }

                double matrixA_i_i = matrixA[i][i];
                for (int k = i; k < matrixB.Count; k++)
                {
                    matrixA[i][k] /= matrixA_i_i;
                }
                matrixB[i] /= matrixA_i_i;

                for (int j = 1; j < matrixB.Count - i; j++)
                {
                    double matrixA_iPlusj_i = matrixA[i + j][i];
                    for (int k = 0; k < matrixB.Count - i; k++)
                    {
                        matrixA[i + j][i + k] -= matrixA[i][i + k] * matrixA_iPlusj_i;
                    }

                    matrixB[i + j] -= matrixB[i] * matrixA_iPlusj_i;
                }
            }

            //Зворотній хід
            for (int i = matrixA.Count - 1; i >= 0 ; i--)
            {
                double sumToSubtract = 0;

                for (int j = 0; j < matrixA.Count - 1 - i; j++)
                {
                    sumToSubtract += matrixA[i][matrixA.Count - 1 - j] * matrixX[j];
                }

                matrixX.Add((matrixB[i] - sumToSubtract));
            }

            matrixX.Reverse();

            return matrixX;
        }
        private static List<List<double>> CloneList(List<List<double>> listToClone)
        {
            List<List<double>> newList = new List<List<double>>();

            foreach (List<double> item in listToClone)
            {
                newList.Add(new List<double>(item));
            }

            return newList;
        }
    }
}
