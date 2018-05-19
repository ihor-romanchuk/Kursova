using PoohMathParser;
using System;

namespace Common.CustomIntegrals
{
    public class Integral_2_2 : ICustomIntegral
    {
        private readonly ICustomIntegral _integral_1_1;
        private readonly double _radius;

        public Lazy<MathExpression> MathExpression { get; private set; }
        public Lazy<MathExpression> MathExpressionWithFeature { get; private set; }

        public Integral_2_2(): this(new Integral_1_1(), Constants.Radius) { }
        public Integral_2_2(ICustomIntegral integral_1_1, double radius)
        {
            _integral_1_1 = integral_1_1;
            _radius = radius;

            MathExpression = new Lazy<MathExpression>(() => new MathExpression("ln((sqrt((2*a^2)*(1-cos(t-s))))^(0-1))"));
            MathExpressionWithFeature = new Lazy<MathExpression>(() => new MathExpression("ln((sqrt((2*a^2)*(1-cos(t-s))))^(0-1))-ln((a*abs(t-s))^(0-1))"));
        }

        public double Calculate(double from, double to, double colocationPoint)
        {
            if (colocationPoint <= to && colocationPoint >= from)
            {
                return _integral_1_1.Calculate(from, to, colocationPoint) - Math.Log(_radius) * Math.Abs(to - from) +
                    GaussMethodForIntegrals.CalculateWithAccuracy(from, to, MathExpressionWithFeature.Value, Constants.Epsilon, new Var("t", 0), new Var("s", colocationPoint), new Var("a", _radius)) / 2 * Math.PI;
            }
            else
            {
                return GaussMethodForIntegrals.CalculateWithAccuracy(from, to, MathExpression.Value, Constants.Epsilon, new Var("t", 0), new Var("s", colocationPoint), new Var("a", _radius)) / 2 * Math.PI;
            }
        }
    }
}
