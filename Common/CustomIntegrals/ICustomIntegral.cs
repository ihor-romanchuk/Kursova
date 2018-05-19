namespace Common.CustomIntegrals
{
    /// <summary>
    /// Represents each of four integrals in integral equation system.
    /// Tao(x) + Lambda * Int_1_1(x) + Lambda * Int_1_2(x) = f(x); x in S
    /// Int_2_1(x) + Int_2_2(x) = g(x); x in Sigma
    /// </summary>
    public interface ICustomIntegral
    {
        double Calculate(double from, double to, double colocationPoint);
    }
}
