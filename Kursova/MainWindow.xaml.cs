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
using AutoMapper;

namespace Kursova
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StaticResources _staticResources;
        private readonly IntegralEquationSolver _integralEquationSolver;
        private readonly IMappingEngine _mappingEngine;

        public MainWindowViewModel ViewModel { get; set; }
        public Settings Settings { get; private set; }

        public MainWindow()
        {
            _staticResources = new StaticResources();
            _mappingEngine = Mapper.Engine;
            _integralEquationSolver = new IntegralEquationSolver();

            Settings = _mappingEngine.Mapper.Map<Settings>(_staticResources.DefaultSettings.Value[GetEquationType()]);
            ViewModel = _mappingEngine.Mapper.Map<MainWindowViewModel>(Settings);
            DataContext = ViewModel;

            InitializeComponent();
        }

        private void resultButton_Click(object sender, RoutedEventArgs e)
        {
            _mappingEngine.Mapper.Map(ViewModel, Settings);

            var result = _integralEquationSolver.Solve(Settings, _staticResources.MatrixAInits.Value[GetEquationType()],
                _staticResources.MatrixBInits.Value[GetEquationType()]);

            ResultWindow resultWindow = new ResultWindow(result);
            resultWindow.ShowDialog();
        }
        private void equationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_staticResources?.DefaultSettings != null)
            {
                _mappingEngine.Mapper.Map(_staticResources.DefaultSettings.Value[GetEquationType()], ViewModel);
            }
        }

        private EquationsEnum GetEquationType()
        {
            return (EquationsEnum)(equationComboBox?.SelectedIndex ?? 0);
        }
    }
}