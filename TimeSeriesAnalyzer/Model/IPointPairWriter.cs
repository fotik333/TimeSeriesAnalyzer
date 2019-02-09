using System;
using System.Collections.Generic;
using System.Windows;

namespace TimeSeriesAnalyzer.Model
{
    public interface IPointPairWriter
    {
        void Write(IEnumerable<Tuple<Point, Point>> pointPairs);
    }
}