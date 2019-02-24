using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TimeSeriesAnalyzer.Model {
    public class TimeSeries {
        public TimeSeries(IEnumerable<Point> points) {
            Points = points.ToList();
        }

        public List<Point> Points { get; }
    }
}