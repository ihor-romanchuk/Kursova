using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PoohMathParser;

namespace Kursova
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StaticResources _staticResources;
        private readonly IntegralEquationSolver _integralEquationSolver;

        private double intervalLeft;
        private double intervalRight;
        private int amountOfPartitions;
        private string function;
        private double radius;
        private List<double> _partitionPoints;
        private List<double> _colocationPoints;

        public Settings Settings { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _staticResources = new StaticResources();
            _integralEquationSolver = new IntegralEquationSolver();
        }

        private void Update_1()
        {
            this.intervalLeft = Convert.ToDouble(this.textboxIntervalLeft.Text);
            this.intervalRight = Convert.ToDouble(this.textboxIntervalRight.Text);
            this.amountOfPartitions = Convert.ToInt32(this.textBoxNumberOfPartitions.Text);
            this.function = this.textBoxFunction.Text;
        }
        private void Update_2()
        {
            this.intervalLeft = Convert.ToDouble(this.textboxIntervalLeft2.Text);
            this.intervalRight = Convert.ToDouble(this.textboxIntervalRight2.Text);
            this.amountOfPartitions = Convert.ToInt32(this.textBoxNumberOfPartitions2.Text);
            this.function = this.textBoxFunction2.Text;
            this.radius = Convert.ToDouble(this.textBoxRadius.Text);
            this._partitionPoints = new List<double>();
            this._colocationPoints = new List<double>();

            double step = Math.Abs(this.intervalRight - this.intervalLeft) / this.amountOfPartitions;

            for (int i = 0; i <= this.amountOfPartitions; i++)
            {
                this._partitionPoints.Add(this.intervalLeft + i * step);
            }

            for (int i = 0; i < this._partitionPoints.Count - 1; i++)
            {
                this._colocationPoints.Add((this._partitionPoints[i] + this._partitionPoints[i + 1]) / 2.0);
            }
        }

        private void buttonResult_Click(object sender, RoutedEventArgs e)
        {
            this.Update_1();

            ResultWindow resultWindow = new ResultWindow(FredholmEquationFirstOrder.Calculate(this.intervalLeft, this.intervalRight, this.amountOfPartitions, this.function));
            resultWindow.ShowDialog();
        }
        private void buttonResult_2_Click(object sender, RoutedEventArgs e)
        {
            this.Update_2();

            List<List<double>> A = Calculate_A_2(this._partitionPoints, this._colocationPoints);
            List<double> F_j = FredholmEquationFirstOrder.Calculate_Fj(this.function, this._colocationPoints);
            List<double> C_k = GaussMethodForSystems.Calculate(A, F_j);

            ResultWindow resultWindow = new ResultWindow(this._colocationPoints, C_k);
            resultWindow.ShowDialog();
        }

        private List<List<double>> Calculate_A_2(List<double> partitionPoints, List<double> colocationPoints)
        {
            this.Update_2();

            List<List<double>> result = new List<List<double>>();

            for (int i = 0; i < colocationPoints.Count; i++)
            {
                result.Add(new List<double>());

                for (int j = 1; j < partitionPoints.Count; j++)
                {
                    if (i == (j - 1))
                    {
                        result[i].Add(2 * FredholmEquationFirstOrder.Calculate_Aij(partitionPoints, colocationPoints, i, j) - Math.Log(this.radius) *
                            Math.Abs(this.intervalRight - this.intervalLeft) / this.amountOfPartitions +
                            GaussMethodForIntegrals.CalculateWithAccuracy(partitionPoints[j - 1], partitionPoints[j],
                            string.Format("ln(1/(2*{1}*(1-cos(x-{0}))))-ln(1/({1}*(x-{0})^2))", this._colocationPoints[i], this.radius), 0.001));
                    }
                    else
                    {
                        result[i].Add(GaussMethodForIntegrals.CalculateWithAccuracy(partitionPoints[j - 1], partitionPoints[j], 
                            string.Format("ln(1/(2*{1}*(1-cos(x-{0}))))", this._colocationPoints[i], this.radius), 0.001));
                    }
                }
            }

            return result;
        }

        private void Update()
        {
            Settings = new Settings
            {
                IntervalOfIntegration = new Tuple<double, double>(ToDoubleIgnoreCase(intervalTFromTextBox.Text), ToDoubleIgnoreCase(intervalTToTextBox.Text)),
                IntervalOfFunction = new Tuple<double, double>(ToDoubleIgnoreCase(intervalSFromTextBox.Text), ToDoubleIgnoreCase(intervalSToTextBox.Text)),
                AmountOfPartitions = Convert.ToInt32(textBoxNumberOfPartitions.Text),
                Radius = !string.IsNullOrEmpty(radiusTextBox.Text) ? Convert.ToDouble(radiusTextBox.Text) : (double?)null,
                FunctionF = new MathExpression(functionFTextBox.Text),
                FunctionDistance = new MathExpression(distanceFunctionTextBox.Text),
                FunctionYakobian = new MathExpression(yakobianFunctionTextBox.Text)
            };

            var partitionPoints = new List<double>();
            var colocationPoints = new List<double>();

            double step = Math.Abs(Settings.IntervalOfIntegration.Item2 - Settings.IntervalOfIntegration.Item1) / Settings.AmountOfPartitions;

            for (int i = 0; i <= Settings.AmountOfPartitions; i++)
            {
                partitionPoints.Add(Settings.IntervalOfIntegration.Item1 + i * step);
            }

            for (int i = 0; i < partitionPoints.Count - 1; i++)
            {
                colocationPoints.Add((partitionPoints[i] + partitionPoints[i + 1]) / 2.0);
            }

            Settings.PartitionPoints = partitionPoints;
            Settings.ColocationPoints = colocationPoints;
        }

        public double ToDoubleIgnoreCase(string input)
        {
            //return Convert.ToDouble(new MathExpression(input.ToLower()).Calculate());
            return Convert.ToDouble(input);
        }

        

        private void resultButton_Click(object sender, RoutedEventArgs e)
        {
            Update();

            var result = _integralEquationSolver.Solve(Settings, _staticResources.MatrixAInits.Value[GetEquationType()],
                _staticResources.MatrixBInits.Value[GetEquationType()]);

            ResultWindow resultWindow = new ResultWindow(result);
            resultWindow.ShowDialog();
        }

        private void UpdateSettingsView(Settings settings)
        {
            intervalTFromTextBox.Text = settings.IntervalOfIntegration.Item1.ToString();
            intervalTToTextBox.Text = settings.IntervalOfIntegration.Item2.ToString();

            intervalSFromTextBox.Text = settings.IntervalOfFunction.Item1.ToString();
            intervalSToTextBox.Text = settings.IntervalOfFunction.Item2.ToString();

            radiusTextBox.Text = settings.Radius.ToString();
            functionFTextBox.Text = settings.FunctionF.ToString();
            distanceFunctionTextBox.Text = settings.FunctionDistance.ToString();
            yakobianFunctionTextBox.Text = settings.FunctionYakobian.ToString();
            amountOfPartitionsTextBox.Text = settings.AmountOfPartitions.ToString();
        }
        private void equationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_staticResources?.DefaultSettings != null)
            { 
                UpdateSettingsView(_staticResources.DefaultSettings.Value[GetEquationType()]);
            }
        }
        private EquationsEnum GetEquationType()
        {
            return (EquationsEnum)equationComboBox.SelectedIndex;
        }
    }
}
