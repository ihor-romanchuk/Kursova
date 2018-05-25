using System.ComponentModel;

namespace SimplifiedEquations.WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private double _itervalOfIntegrationLeft;
        private double _intervalOfIntegrationRight;
        private double _itervalOfFunctionLeft;
        private double _intervalOfFunctionRight;
        private int _amountOfPartitions;
        private string _radius;
        private string _functionF;
        private string _functionDistance;
        private string _functionYakobian;

        public double IntervalOfIntegrationLeft
        {
            get { return _itervalOfIntegrationLeft; }
            set
            {
                _itervalOfIntegrationLeft = value;
                NotifyPropertyChanged("IntervalOfIntegrationLeft");
            }
        }
        public double IntervalOfIntegrationRight
        {
            get { return _intervalOfIntegrationRight; }
            set
            {
                _intervalOfIntegrationRight = value;
                NotifyPropertyChanged("IntervalOfIntegrationRight");
            }
        }
        public double IntervalOfFunctionLeft
        {
            get { return _itervalOfFunctionLeft; }
            set
            {
                _itervalOfFunctionLeft = value;
                NotifyPropertyChanged("IntervalOfFunctionLeft");
            }
        }
        public double IntervalOfFunctionRight
        {
            get { return _intervalOfFunctionRight; }
            set
            {
                _intervalOfFunctionRight = value;
                NotifyPropertyChanged("IntervalOfFunctionRight");
            }
        }
        public int AmountOfPartitions
        {
            get { return _amountOfPartitions; }
            set
            {
                _amountOfPartitions = value;
                NotifyPropertyChanged("AmountOfPartitions");
            }
        }
        public string Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                NotifyPropertyChanged("Radius");
            }
        }
        public string FunctionF
        {
            get { return _functionF; }
            set
            {
                _functionF = value;
                NotifyPropertyChanged("FunctionF");
            }
        }
        public string FunctionDistance
        {
            get { return _functionDistance; }
            set
            {
                _functionDistance = value;
                NotifyPropertyChanged("FunctionDistance");
            }
        }
        public string FunctionYakobian
        {
            get { return _functionYakobian; }
            set
            {
                _functionYakobian = value;
                NotifyPropertyChanged("FunctionYakobian");
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
