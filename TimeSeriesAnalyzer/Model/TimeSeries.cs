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

        public IEnumerable<Tuple<Point, Point>> Compare(TimeSeries timeSeries, ITimeSeriesComparatorService comparator)
        {
            return comparator.Compare(this, timeSeries);
        }
    }
}