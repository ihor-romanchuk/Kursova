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
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using System.Collections.ObjectModel;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Data;

namespace Kursova
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        private Dictionary<double, double> data;
        private DataTable dataTable;

        public ResultWindow()
        {
            InitializeComponent();
        }

        public ResultWindow(List<double> points, List<double> values)
        {
            InitializeComponent();

            this.data = new Dictionary<double,double>();
            for (int i = 0; i < points.Count; i++)
            {
                this.data.Add(points[i], values[i]);
            }
        }
        public ResultWindow(Dictionary<double, double> data)
        {
            InitializeComponent();

            this.data = data;
        }

        private void Initialize()
        {
            this.dataTable = new DataTable();
            this.dataTable.Columns.Add("t");
            this.dataTable.Columns.Add("τ(t)");

            for (int i = 0; i < this.data.Count; i++)
            {
                this.dataTable.Rows.Add(new TableRow());
                this.dataTable.Rows[i][0] = string.Format("{0:0.000}", this.data.ElementAt(i).Key);
                this.dataTable.Rows[i][1] = string.Format("{0:0.0000000}", this.data.ElementAt(i).Value);
            }

            this.dataGrid.ItemsSource = this.dataTable.DefaultView;

            List<Point> dataPoints = new List<Point>();

            foreach (var item in this.data)
            {
                dataPoints.Add(new Point(item.Key, item.Value));
            }

            ClearLines();

            EnumerableDataSource<Point> eds = new EnumerableDataSource<Point>(dataPoints);
            eds.SetXMapping(p => p.X);
            eds.SetYMapping(p => p.Y);
            IPointDataSource ipds = eds;

            LineGraph line = new LineGraph(ipds);
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

        private void resultWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Initialize();
        }
    }
}
