using AutoMapper;
using PoohMathParser;
using System;
using System.Diagnostics;
using System.Windows;

namespace Kursova
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            if(!Debugger.IsAttached)
            { 
                Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            }

            RegisterAutoMapper();
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void RegisterAutoMapper()
        {
            Mapper.Initialize(mapper =>
            {
                mapper.CreateMap<Settings, Settings>();
                mapper.CreateMap<MainWindowViewModel, Settings>()
                .ForMember(dest => dest.IntervalOfIntegration, opts => opts.MapFrom(src => new Tuple<double, double>(src.IntervalOfIntegrationLeft, src.IntervalOfIntegrationRight)))
                .ForMember(dest => dest.IntervalOfFunction, opts => opts.MapFrom(src => new Tuple<double, double>(src.IntervalOfFunctionLeft, src.IntervalOfFunctionRight)))
                .ForMember(dest => dest.Radius, opts => opts.MapFrom(src => !string.IsNullOrEmpty(src.Radius) ? Convert.ToDouble(src.Radius) : (double?)null))
                .ForMember(dest => dest.FunctionDistance, opts => opts.MapFrom(src => new MathExpression(src.FunctionDistance)))
                .ForMember(dest => dest.FunctionF, opts => opts.MapFrom(src => new MathExpression(src.FunctionF)))
                .ForMember(dest => dest.FunctionYakobian, opts => opts.MapFrom(src => new MathExpression(src.FunctionYakobian)));

                mapper.CreateMap<Settings, MainWindowViewModel>()
                    .ForMember(dest => dest.IntervalOfIntegrationLeft, opts => opts.MapFrom(src => src.IntervalOfIntegration.Item1))
                    .ForMember(dest => dest.IntervalOfIntegrationRight, opts => opts.MapFrom(src => src.IntervalOfIntegration.Item2))
                    .ForMember(dest => dest.IntervalOfFunctionLeft, opts => opts.MapFrom(src => src.IntervalOfFunction.Item1))
                    .ForMember(dest => dest.IntervalOfFunctionRight, opts => opts.MapFrom(src => src.IntervalOfFunction.Item2));
            });
        }
    }
}
