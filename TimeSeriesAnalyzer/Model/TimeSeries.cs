using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TimeSeriesAnalyzer.Model
{
    public class TimeSeries
    {
        public List<Point> Points { get; }

        public TimeSeries(IEnumerable<Point> points)
        {
            Points = points.ToList();
        }

        public IEnumerable<Tuple<double, double>> FindGreaterThan(TimeSeries timeSeries)
        {
            var res = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(1, 10), new Tuple<double, double>(2, 20), new Tuple<double, double>(3, 30)
            };
            return res;
        }
    }
}