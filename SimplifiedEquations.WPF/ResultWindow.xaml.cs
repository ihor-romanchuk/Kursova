using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SimplifiedEquations.WPF
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        private Dictionary<double, double> _data;
        private DataTable _dataTable;

        public ResultWindow()
        {
            InitializeComponent();
        }

        public ResultWindow(List<double> points, List<double> values)
        {
            InitializeComponent();

            _data = new Dictionary<double,double>();
            for (int i = 0; i < points.Count; i++)
            {
                _data.Add(points[i], values[i]);
            }
        }

        public ResultWindow(Dictionary<double, double> data)
        {
            InitializeComponent();

            _data = data;
        }


        private void resultWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void Initialize()
        {
            _dataTable = new DataTable();
            _dataTable.Columns.Add("x");
            _dataTable.Columns.Add("f(x)");

            List<Point> dataPoints = new List<Point>();
            for (int i = 0; i < _data.Count; i++)
            {
                _dataTable.Rows.Add(new TableRow());
                _dataTable.Rows[i][0] = string.Format("{0:0.000}", _data.ElementAt(i).Key);
                _dataTable.Rows[i][1] = string.Format("{0:0.0000000}", _data.ElementAt(i).Value);

                dataPoints.Add(new Point(_data.ElementAt(i).Key, _data.ElementAt(i).Value));
            }

            dataGrid.ItemsSource = _dataTable.DefaultView;
            ClearLines();

            EnumerableDataSource<Point> eds = new EnumerableDataSource<Point>(dataPoints);
            eds.SetXMapping(p => p.X);
            eds.SetYMapping(p => p.Y);

            LineGraph line = new LineGraph(eds);
            line.LinePen = new Pen(Brushes.Black, 2);
            plotter.Children.Add(line);
            plotter.FitToView();
        }

        private void ClearLines()
        {
            var lgc = new Collection<IPlotterElement>();
            foreach (var x in plotter.Children)
            {
                if (x is LineGraph || x is ElementMarkerPointsGraph)
                    lgc.Add(x);
            }

            foreach (var x in lgc)
            {
                plotter.Children.Remove(x);
            }
        }
    }
}
