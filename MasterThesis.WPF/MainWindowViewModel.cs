using System.ComponentModel;

namespace MasterThesis.WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private double _radius;
        private string _lambda;
        private string _functionF;
        private string _functionG;
        private int _partitionsOnCrack;
        private int _partitionsOnBound;
        private int _meshSize;

        public double Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                NotifyPropertyChanged(nameof(Radius));
            }
        }
        public string Lambda
        {
            get { return _lambda; }
            set
            {
                _lambda = value;
                NotifyPropertyChanged(nameof(Lambda));
            }
        }        
        public string FunctionF
        {
            get { return _functionF; }
            set
            {
                _functionF = value;
                NotifyPropertyChanged(nameof(FunctionF));
            }
        }
        public string FunctionG
        {
            get { return _functionG; }
            set
            {
                _functionG = value;
                NotifyPropertyChanged(nameof(FunctionG));
            }
        }
        public int PartitionsOnCrack
        {
            get { return _partitionsOnCrack; }
            set
            {
                _partitionsOnCrack = value;
                NotifyPropertyChanged(nameof(PartitionsOnCrack));
            }
        }
        public int PartitionsOnBound
        {
            get { return _partitionsOnBound; }
            set
            {
                _partitionsOnBound = value;
                NotifyPropertyChanged(nameof(PartitionsOnBound));
            }
        }

        public int MeshSize
        {
            get { return _meshSize; }
            set
            {
                _meshSize = value;
                NotifyPropertyChanged(nameof(MeshSize));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
