using System;
using System.Collections.Generic;
using System.Windows;

namespace TimeSeriesAnalyzer.Model {
    public class RandomDataService : IDataService {
        private readonly Random _rand = new Random();

        public IEnumerable<Point> GetPoints(double xMin, double xMax, double yMin, double yMax, int count,
            bool areIntervalsDifferent) {
            var result = new List<Point>();

            if (areIntervalsDifferent) {
                xMin = (xMax - xMin) * _rand.NextDouble() / 2 + xMin;
                xMax = xMax - (xMax - xMin) * _rand.NextDouble() / 2;
            }

            var yRange = yMax - yMin;
            var xRange = xMax - xMin;
            var step = xRange / count;
            if (areIntervalsDifferent) {
                for (var i = 0; i < count; i++)
                    result.Add(new Point(i * step + _rand.NextDouble() * step / 2 + xMin,
                        _rand.NextDouble() * yRange + yMin));
            }
            else {
                result.Add(new Point(xMin, _rand.NextDouble() * yRange + yMin));

                for (var i = 1; i < count - 1; i++)
                    result.Add(new Point(i * step + _rand.NextDouble() * step / 2 + xMin,
                        _rand.NextDouble() * yRange + yMin));

                result.Add(new Point(xMax, _rand.NextDouble() * yRange + yMin));
            }

            return result;
        }
    }
}