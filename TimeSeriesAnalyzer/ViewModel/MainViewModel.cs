using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using TimeSeriesAnalyzer.Model;

namespace TimeSeriesAnalyzer.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private const int MinTimeSeriesCount = 2;
        private const int MaxTimeSeriesCount = 5;
        private readonly List<TimeSeries> _timeSeries = new List<TimeSeries>();

        private bool _areRangesDifferent;
        private bool _areTimeSeriesCompared;
        private int _firstTimeSeriesIndex;
        private int _secondTimeSeriesIndex;

        private int _timeSeriesCount = MinTimeSeriesCount;

        private double _xMin = 0;
        private double _xMax = 10;
        private double _yMin = -10;
        private double _yMax = 10;
        private int _pointsCount = 20;

        public MainViewModel()
        {
            AreTimeSeriesCompared = false;
            GenerateTimeSeriesCommand = new RelayCommand(GenerateTimeSeries);
            CompareTimeSeriesCommand = new RelayCommand(CompareTimeSeries);
            ShowIntervalsCommand = new RelayCommand(ShowIntervals);
            SaveIntervalsToFileCommand = new RelayCommand(SaveIntervalsToFile);
        }

        public Func<double, string> Formatter => value => value.ToString("0.###");

        public SeriesCollection Series { get; } = new SeriesCollection();
        public SectionsCollection Intervals { get; } = new SectionsCollection();

        public ObservableCollection<Tuple<Point, Point>> СomparisonResult { get; } =
            new ObservableCollection<Tuple<Point, Point>>();

        public ObservableCollection<int> TimeSeriesIndexes { get; } = new ObservableCollection<int>();

        public RelayCommand GenerateTimeSeriesCommand { get; }
        public RelayCommand CompareTimeSeriesCommand { get; }
        public RelayCommand ShowIntervalsCommand { get; }
        public RelayCommand SaveIntervalsToFileCommand { get; }

        public bool AreRangesDifferent
        {
            get => _areRangesDifferent;
            set => Set(ref _areRangesDifferent, value);
        }

        public bool AreTimeSeriesCompared
        {
            get => _areTimeSeriesCompared;
            set => Set(ref _areTimeSeriesCompared, value);
        }

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

        public double XMin
        {
            get => _xMin;
            set => Set(ref _xMin, value);
        }

        public double XMax
        {
            get => _xMax;
            set => Set(ref _xMax, value);
        }

        public double YMin
        {
            get => _yMin;
            set => Set(ref _yMin, value);
        }

        public double YMax
        {
            get => _yMax;
            set => Set(ref _yMax, value);
        }

        public int PointsCount
        {
            get => _pointsCount;
            set => Set(ref _pointsCount, value);
        }

        private void ShowIntervals()
        {
            Intervals.Clear();

            foreach (var interval in СomparisonResult)
                Intervals.Add(new AxisSection
                {
                    Opacity = 0.2,
                    Value = interval.Item1.X,
                    SectionWidth = interval.Item2.X - interval.Item1.X,
                    Fill = Brushes.Coral
                });
        }

        private void SaveIntervalsToFile()
        {
            //TODO SAVETOFILE
            var text = "{\"intervals\" : [";
            foreach (var tuple in СomparisonResult)
                text +=
                    $"[ {{ \"{tuple.Item1.X}\": \"{tuple.Item1.Y}\" }}, {{ \"{tuple.Item2.X}\": \"{tuple.Item2.Y}\" }} ],";
            text = text.Remove(text.Length - 1);
            text += "]}";

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files(*.txt)|*.txt|Json files(*.json)|*.json|All files(*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == false)
                return;

            var filename = saveFileDialog.FileName;
            File.WriteAllText(filename, text);
        }

        private void NewSessionActions()
        {
            AreTimeSeriesCompared = false;
            TimeSeriesIndexes.Clear();
            СomparisonResult.Clear();
            _timeSeries.Clear();
            Intervals.Clear();
            Series.Clear();
            FirstTimeSeriesIndex = 0;
            SecondTimeSeriesIndex = 1;
            RaisePropertyChanged(nameof(FirstTimeSeriesIndex));
            RaisePropertyChanged(nameof(SecondTimeSeriesIndex));
        }

        private void GenerateTimeSeries()
        {
            if (_xMin > _xMax || _yMin > _yMax) {
                ShowErrorMessage("Enter correct values");
                return;
            }

            NewSessionActions();

            var dataService = ServiceLocator.Current.GetInstance<IDataService>();

            for (var i = 0; i < TimeSeriesCount; i++)
            {
                _timeSeries.Add(new TimeSeries(dataService.GetPoints(XMin, XMax, YMin, YMax, PointsCount, _areRangesDifferent)));
                TimeSeriesIndexes.Add(i);
            }

            for (var i = 0; i < _timeSeries.Count; i++)
            { 
                var ts = _timeSeries[i];
                var values = new ChartValues<ObservablePoint>();
                foreach (var p in ts.Points) values.Add(new ObservablePoint(p.X, p.Y));

                var series = new LineSeries
                {
                    Title = i.ToString(),
                    Values = values,
                    PointGeometry = null,
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0
                };
                
                Series.Add(series);
            }
        }

        private void CompareTimeSeries()
        {
            if (_timeSeries.Count < 2)
            {
                ShowErrorMessage("No time series to compare.");
                return;
            }

            if (FirstTimeSeriesIndex == SecondTimeSeriesIndex)
            {
                ShowErrorMessage("Choose different time series to compare.");
                return;
            }

            var comparator = ServiceLocator.Current.GetInstance<ITimeSeriesComparatorService>();
            var result = comparator.Compare(_timeSeries[FirstTimeSeriesIndex], _timeSeries[SecondTimeSeriesIndex]);
            СomparisonResult.Clear();
            foreach (var tuple in result) СomparisonResult.Add(tuple);

            Intervals.Clear();
            AreTimeSeriesCompared = true;
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}