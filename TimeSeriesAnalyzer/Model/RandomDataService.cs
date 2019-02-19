using System;
using System.Collections.Generic;
using System.Windows;

namespace TimeSeriesAnalyzer.Model
{
    public class RandomDataService : IDataService
    {
        public double XMin { get; }
        public double XMax { get; }
        public double YMin { get; }
        public double YMax { get; }
        public int Count { get; }


        private readonly Random _rand = new Random();

        public RandomDataService(double xMin, double xMax, double yMin, double yMax, int count)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
            Count = count;
        }
        public IEnumerable<Point> GetPoints()
        {
            //var _rand = new Random();
            var res = new List<Point>();
            var yRange = YMax - YMin;
            for (int i = 0; i < Count; i++)
            {
                res.Add(new Point(i, _rand.NextDouble() * yRange + YMin));
            }
            //Thread.Sleep(100);
            return res;
        }
    }
}