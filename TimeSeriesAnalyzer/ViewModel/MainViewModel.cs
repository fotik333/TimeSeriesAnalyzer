using System.Collections;
using System.Collections.Generic;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using TimeSeriesAnalyzer.Model;

namespace TimeSeriesAnalyzer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public SeriesCollection Series { get; } = new SeriesCollection();

        public MainViewModel(IDataService dataService)
        {
            IEnumerable<int> col = new List<int>();
            


            TimeSeries[] timeSeries =
            {
                new TimeSeries(dataService.GetData()),
                new TimeSeries(dataService.GetData())
            };

            foreach (var ts in timeSeries)
            {
                var values = new ChartValues<ObservablePoint>();
                foreach (var p in ts.Points)
                {
                    values.Add(new ObservablePoint(p.X, p.Y));
                }

                var series = new LineSeries
                {
                    Values = values,
                    //Fill = Brushes.Transparent,
                    //StrokeThickness = .5,
                    PointGeometry = null,
                    LineSmoothness = 0
                };
                Series.Add(series);
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}