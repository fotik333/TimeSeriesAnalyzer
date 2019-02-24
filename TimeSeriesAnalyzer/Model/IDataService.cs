using System.Collections.Generic;
using System.Windows;

namespace TimeSeriesAnalyzer.Model {
    public interface IDataService {
        IEnumerable<Point> GetPoints(double xMin, double xMax, double yMin, double yMax, int count,
            bool areIntervalsDifferent);
    }
}