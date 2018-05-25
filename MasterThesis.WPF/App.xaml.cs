using AutoMapper;
using Common;
using PoohMathParser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MasterThesis.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            if (!Debugger.IsAttached)
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
                .ForMember(dest => dest.Radius, opts => opts.MapFrom(src => src.Radius))
                .ForMember(dest => dest.Lambda, opts => opts.MapFrom(src => new MathExpression(src.Lambda)))
                .ForMember(dest => dest.FunctionF, opts => opts.MapFrom(src => new MathExpression(src.FunctionF)))
                .ForMember(dest => dest.FunctionG, opts => opts.MapFrom(src => new MathExpression(src.FunctionG)))
                .ForMember(dest => dest.PartitionsOnCrack, opts => opts.MapFrom(src => src.PartitionsOnCrack))
                .ForMember(dest => dest.PartitionsOnBound, opts => opts.MapFrom(src => src.PartitionsOnBound));

                mapper.CreateMap<Settings, MainWindowViewModel>();
            });
        }
    }
}
