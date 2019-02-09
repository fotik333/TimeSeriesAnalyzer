using System;
using System.Collections.Generic;
using System.Windows;

namespace TimeSeriesAnalyzer.Model
{
    public class FileWriter : IPointPairWriter
    {
        public void Write(IEnumerable<Tuple<Point, Point>> pointPairs)
        {
            throw new NotImplementedException();
        }
    }
}