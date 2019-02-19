using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Practices.ServiceLocation;
using TimeSeriesAnalyzer.Model;

namespace TimeSeriesAnalyzer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const int MinTimeSeriesCount = 2;
        private const int MaxTimeSeriesCount = 5;

        private int _timeSeriesCount = MinTimeSeriesCount;
        private readonly List<TimeSeries> _timeSeries = new List<TimeSeries>();
        private int _firstTimeSeriesIndex;
        private int _secondTimeSeriesIndex;

        public SeriesCollection Series { get; } = new SeriesCollection();
        public ObservableCollection<Tuple<Point, Point>> СomparisonResult { get; } = new ObservableCollection<Tuple<Point, Point>>();

        public RelayCommand GenerateTimeSeriesCommand { get; }
        public RelayCommand CompareTimeSeriesCommand { get; }

        public ObservableCollection<int> TimeSeriesIndexes { get; } = new ObservableCollection<int>();

        public int FirstTimeSeriesIndex
        {
            get => _firstTimeSeriesIndex;
            set => Set(ref _firstTimeSeriesIndex, value);
        }

        public int SecondTimeSeriesIndex
        {
            get => _secondTimeSeriesIndex;
            set => Set(ref _secondTimeSeriesIndex, value);
        }

        public int TimeSeriesCount
        {
            get => _timeSeriesCount;
            set
            {
                if (value < MinTimeSeriesCount || value > MaxTimeSeriesCount)
                    throw new ArgumentException();
                Set(ref _timeSeriesCount, value);
            }
        }

        public MainViewModel()
        {
            GenerateTimeSeriesCommand = new RelayCommand(GenerateTimeSeries);
            CompareTimeSeriesCommand = new RelayCommand(CompareTimeSeries, CanCompareTimeSeries);
        }

        private void GenerateTimeSeries()
        {
            //IEnumerable<int> col = new List<int>();
            var dataService = ServiceLocator.Current.GetInstance<IDataService>();

            _timeSeries.Clear();
            for (int i = 0; i < TimeSeriesCount; i++)
            {
                _timeSeries.Add(new TimeSeries(dataService.GetPoints()));
            }

            Series.Clear();
            for (var i = 0; i < _timeSeries.Count; i++)
            {
                var ts = _timeSeries[i];
                var values = new ChartValues<ObservablePoint>();
                foreach (var p in ts.Points)
                {
                    values.Add(new ObservablePoint(p.X, p.Y));
                }

                var series = new LineSeries
                {
                    Title = i.ToString(),
                    Values = values,
                    //Fill = Brushes.Transparent,
                    //StrokeThickness = .5,
                    PointGeometry = null,
                    LineSmoothness = 0
                };
                Series.Add(series);
            }

            TimeSeriesIndexes.Clear();
            for (int i = 0; i < TimeSeriesCount; i++)
            {
                TimeSeriesIndexes.Add(i);
            }
            FirstTimeSeriesIndex = 0;
            SecondTimeSeriesIndex = 1;

            CommandManager.InvalidateRequerySuggested();
        }

        private void CompareTimeSeries()
        {
            if (_timeSeries.Count < 2)
            {
                ShowErrorMessage("Нет временных рядов для сравнения.");
                return;
            }
            if (FirstTimeSeriesIndex == SecondTimeSeriesIndex)
            {
                ShowErrorMessage("Выберете разные временные ряды для сравнения.");
                return;
            }




            var comparator = ServiceLocator.Current.GetInstance<ITimeSeriesComparatorService>();
            var result = comparator.Compare(_timeSeries[FirstTimeSeriesIndex], _timeSeries[SecondTimeSeriesIndex]);
            СomparisonResult.Clear();
            foreach (var tuple in result)
            {
                СomparisonResult.Add(tuple);
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool CanCompareTimeSeries()
        {
            //todo не работает
            //return _timeSeries.Count >= 2;
            return true;
        }




        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}