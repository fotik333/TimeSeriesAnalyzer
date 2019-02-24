using System;
using System.Collections.Generic;
using System.Windows;

namespace TimeSeriesAnalyzer.Model {
    public interface ITimeSeriesComparatorService {
        IEnumerable<Tuple<Point, Point>> Compare(TimeSeries timeSeries1, TimeSeries timeSeries2);
    }
}