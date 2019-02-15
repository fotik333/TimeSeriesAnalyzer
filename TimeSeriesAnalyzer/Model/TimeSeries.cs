using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TimeSeriesAnalyzer.Model
{

    public interface IDataService
    {
        IEnumerable<Point> GetPoints();
    }

    public class RandomDataService : IDataService
    {
        public IEnumerable<Point> GetPoints()
        {
            throw new NotImplementedException();
        }
    }

    public interface ITimeSeriesComparatorService
    {
        IEnumerable<Tuple<Point, Point>> Compare(TimeSeries timeSeries1, TimeSeries timeSeries2);
    }

    public class MoreThanComparatorService : ITimeSeriesComparatorService
    {
        public IEnumerable<Tuple<Point, Point>> Compare(TimeSeries timeSeries1, TimeSeries timeSeries2)
        {
            throw new NotImplementedException();
        }
    }

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