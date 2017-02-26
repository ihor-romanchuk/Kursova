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

        public Settings Settings { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _staticResources = new StaticResources();
            _integralEquationSolver = new IntegralEquationSolver();
        }

        private void Update()
        {
            Settings = new Settings
            {
                IntervalOfIntegration = new Tuple<double, double>(ToDoubleIgnoreCase(intervalTFromTextBox.Text), ToDoubleIgnoreCase(intervalTToTextBox.Text)),
                IntervalOfFunction = new Tuple<double, double>(ToDoubleIgnoreCase(intervalSFromTextBox.Text), ToDoubleIgnoreCase(intervalSToTextBox.Text)),
                AmountOfPartitions = Convert.ToInt32(amountOfPartitionsTextBox.Text),
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
