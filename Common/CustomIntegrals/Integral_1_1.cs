using System;

namespace Common.CustomIntegrals
{
    public class Integral_1_1 : ICustomIntegral
    {
        public double Calculate(double from, double to, double colocationPoint)
        {
            double result;
            if (from > colocationPoint)
            {
                result = -((to - colocationPoint) * Math.Log(to - colocationPoint) - (from - colocationPoint) * Math.Log(from - colocationPoint) - (to - from));
            }
            else
            {
                if (to < colocationPoint)
                {
                    result = -(-(-to + colocationPoint) * Math.Log(-to + colocationPoint) + (-from + colocationPoint) * Math.Log(-from + colocationPoint) - (to - from));
                }
                else
                {
                    result = -((-from + colocationPoint) * Math.Log(-from + colocationPoint) + (to - colocationPoint) * Math.Log(to - colocationPoint) - (to - from));
                }
            }

            result = 1 / (2 * Math.PI) * result;
            return result;
        }
    }
}
