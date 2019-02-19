using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeSeriesAnalyzer.Model
{
    public class MoreThanComparatorService : ITimeSeriesComparatorService
    {
        public IEnumerable<Tuple<Point, Point>> Compare(TimeSeries timeSeries1, TimeSeries timeSeries2)
        {
            return new[]
            {
                new Tuple<Point, Point>(new Point(0,0), new Point(1,1)),
                new Tuple<Point, Point>(new Point(2,2), new Point(4,4)),
                new Tuple<Point, Point>(new Point(2,2), new Point(5,5)),
            };
        }
    }
}
