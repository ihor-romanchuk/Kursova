using PoohMathParser;
using System;

namespace Common.CustomIntegrals
{
    public class Integral_1_2 : ICustomIntegral
    {
        private readonly double _radius;

        public Lazy<MathExpression> MathExpression { get; private set; }

        public Integral_1_2(): this(Constants.Radius) { }
        public Integral_1_2(double radius)
        {
            _radius = radius;

            MathExpression = new Lazy<MathExpression>(() => new MathExpression("ln((sqrt((s-a*cos(t))^2+(a*sin(t))^2))^(0-1))*a"));
        }

        public double Calculate(double from, double to, double colocationPoint)
        {
            return GaussMethodForIntegrals.CalculateWithAccuracy(from, to, MathExpression.Value, Constants.Epsilon, new Var("t", 0), new Var("s", colocationPoint), new Var("a", _radius)) / 2 * Math.PI;
        }
    }
}
