using AutoMapper;
using Common;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MasterThesis.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TransformMatrix _transformMatrix;
        private Chart3D _3dChartData;
        private ViewportRect _selectedVieportRect;
        private int _chartModelIndex = -1;
        private int _rectModelIndex = -1;

        private DataTable _dataTable;

        private readonly IMappingEngine _mappingEngine;
        private readonly IntegralSystemSolver _integralSystemSolver;

        public MainWindowViewModel ViewModel { get; set; }
        public Settings Settings { get; private set; }

        public MainWindow()
        {
            _transformMatrix = new TransformMatrix();
            _selectedVieportRect = new ViewportRect();

            _mappingEngine = Mapper.Engine;
            _integralSystemSolver = new IntegralSystemSolver();

            Settings = new Settings();
            ViewModel = new MainWindowViewModel
            {
                FunctionF = "1",
                FunctionG = "1",
                Lambda = 1,
                PartitionsOnCrack = 10,
                PartitionsOnBound = 20,
                MeshSize = 20,
                Radius = 50
            };
            DataContext = ViewModel;

            InitializeComponent();
            InitResultTable();
        }

        private void InitResultTable()
        {
            _dataTable = new DataTable();
            _dataTable.Columns.Add("x");
            _dataTable.Columns.Add("y");
            _dataTable.Columns.Add("u(x,y)");
        }
        private void UpdateResultTable()
        {
            int nVertNo = _3dChartData.GetDataNo();
            _dataTable.Rows.Clear();
            for (int i = 0; i < nVertNo; i++)
            {
                _dataTable.Rows.Add(new TableRow());
                _dataTable.Rows[i][0] = string.Format("{0:0.000}", _3dChartData[i].x);
                _dataTable.Rows[i][1] = string.Format("{0:0.000}", _3dChartData[i].y);
                _dataTable.Rows[i][2] = string.Format("{0:0.0000000}", _3dChartData[i].z);
            }

            dataGrid.ItemsSource = _dataTable.DefaultView;
        }

        private void resultButton_Click(object sender, RoutedEventArgs e)
        {
            _mappingEngine.Mapper.Map(ViewModel, Settings);

            Settings.PartitionPoints = Settings.GeneratePartitionPoints(-1, 1, Settings.PartitionsOnCrack);
            Settings.PartitionPoints.AddRange(Settings.GeneratePartitionPoints(0, 2 * Math.PI, Settings.PartitionsOnBound));

            Settings.ColocationPoints = Settings.GenerateColocationPoints(-1, 1, Settings.PartitionsOnCrack);
            Settings.ColocationPoints.AddRange(Settings.GenerateColocationPoints(0, 2 * Math.PI, Settings.PartitionsOnBound));

            var vector = _integralSystemSolver.CalculateVector(Settings);
            var taoValues = vector.Take(Settings.PartitionsOnCrack).Select(p => p.Value).ToList();
            var sigmaValues = vector.Skip(Settings.PartitionsOnCrack).Select(p => p.Value).ToList();

            TestSurfacePlot(vertex => _integralSystemSolver.Solve(Settings, taoValues, sigmaValues, vertex.x, vertex.y), Settings.MeshSize);
        }

        public void TestSurfacePlot(Func<Vertex3D, double> calculateZ, int nGridNo)
        {
            int nXNo = nGridNo;
            int nYNo = nGridNo;
            // 1. set the surface grid
            _3dChartData = new UniformSurfaceChart3D();
            ((UniformSurfaceChart3D)_3dChartData).SetGrid(nXNo, nYNo, -(float)Settings.Radius.Value, (float)Settings.Radius.Value, -(float)Settings.Radius.Value, (float)Settings.Radius.Value);

            // 2. set surface chart z value
            double xC = _3dChartData.XCenter();
            double yC = _3dChartData.YCenter();
            int nVertNo = _3dChartData.GetDataNo();
            for (int i = 0; i < nVertNo; i++)
            {
                _3dChartData[i].z = (float)calculateZ(_3dChartData[i]);
            }
            UpdateResultTable();
            _3dChartData.GetDataRange();

            // 3. set the surface chart color according to z vaule
            double zMin = _3dChartData.ZMin();
            double zMax = _3dChartData.ZMax();
            for (int i = 0; i < nVertNo; i++)
            {
                Vertex3D vert = _3dChartData[i];
                double h = (vert.z - zMin) / (zMax - zMin);

                Color color = TextureMapping.PseudoColor(h);
                _3dChartData[i].color = color;
            }

            // 4. Get the Mesh3D array from surface chart
            ArrayList meshs = ((UniformSurfaceChart3D)_3dChartData).GetMeshes();

            // 5. display vertex no and triangle no of this surface chart
            UpdateModelSizeInfo(meshs);

            // 6. Set the model display of surface chart
            Model3D model3d = new Model3D();
            Material backMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
            _chartModelIndex = model3d.UpdateModel(meshs, backMaterial, _chartModelIndex, this.mainViewport);

            // 7. set projection matrix, so the data is in the display region
            float xMin = _3dChartData.XMin();
            float xMax = _3dChartData.XMax();
            _transformMatrix.CalculateProjectionMatrix(xMin, xMax, xMin, xMax, zMin, zMax, 0.5);
            TransformChart();
        }

        private void UpdateModelSizeInfo(ArrayList meshs)
        {
            int nMeshNo = meshs.Count;
            int nChartVertNo = 0;
            int nChartTriangelNo = 0;
            for (int i = 0; i < nMeshNo; i++)
            {
                nChartVertNo += ((Mesh3D)meshs[i]).GetVertexNo();
                nChartTriangelNo += ((Mesh3D)meshs[i]).GetTriangleNo();
            }
        }
        // this function is used to rotate, drag and zoom the 3d chart
        private void TransformChart()
        {
            if (_chartModelIndex == -1) return;
            ModelVisual3D visual3d = (ModelVisual3D)(this.mainViewport.Children[_chartModelIndex]);
            if (visual3d.Content == null) return;
            Transform3DGroup group1 = visual3d.Content.Transform as Transform3DGroup;
            group1.Children.Clear();
            group1.Children.Add(new MatrixTransform3D(_transformMatrix.m_totalMatrix));
        }

        public void OnViewportMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point pt = args.GetPosition(mainViewport);
            if (args.ChangedButton == MouseButton.Left)         // rotate or drag 3d model
            {
                _transformMatrix.OnLBtnDown(pt);
            }
            else if (args.ChangedButton == MouseButton.Right)   // select rect
            {
                _selectedVieportRect.OnMouseDown(pt, mainViewport, _rectModelIndex);
            }
        }

        public void OnViewportMouseMove(object sender, System.Windows.Input.MouseEventArgs args)
        {
            Point pt = args.GetPosition(mainViewport);

            if (args.LeftButton == MouseButtonState.Pressed)                // rotate or drag 3d model
            {
                _transformMatrix.OnMouseMove(pt, mainViewport);

                TransformChart();
            }
            else if (args.RightButton == MouseButtonState.Pressed)          // select rect
            {
                _selectedVieportRect.OnMouseMove(pt, mainViewport, _rectModelIndex);
            }
        }

        public void OnViewportMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs args)
        {
            Point pt = args.GetPosition(mainViewport);
            if (args.ChangedButton == MouseButton.Left)
            {
                _transformMatrix.OnLBtnUp();
            }
            else if (args.ChangedButton == MouseButton.Right)
            {
                if (_chartModelIndex == -1) return;
                // 1. get the mesh structure related to the selection rect
                MeshGeometry3D meshGeometry = Model3D.GetGeometry(mainViewport, _chartModelIndex);
                if (meshGeometry == null) return;

                // 2. set selection in 3d chart
                _3dChartData.Select(_selectedVieportRect, _transformMatrix, mainViewport);

                // 3. update selection display
                _3dChartData.HighlightSelection(meshGeometry, Color.FromRgb(200, 200, 200));
            }
        }

        // zoom in 3d display
        public void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs args)
        {
            _transformMatrix.OnKeyDown(args);
            TransformChart();
        }
    }
}
