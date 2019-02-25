using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TimeSeriesAnalyzer.Model {
    public class TimeSeriesComparatorService : ITimeSeriesComparatorService {
        private const double Eps = 10e-12;
        private Tuple<Point, Point> _firstSegment;
        private Tuple<Point, Point> _secondSegment;

        public IEnumerable<Tuple<Point, Point>> Compare(TimeSeries timeSeries1, TimeSeries timeSeries2) {
            var result = new List<Tuple<Point, Point>>();
            var intersectionPoints = new List<Point>();

            var iter1 = 0;
            var iter2 = 0;

            bool firstGreater;

            while (FindIntersectionIters(timeSeries1, timeSeries2, ref iter1, ref iter2) != -1) {
                intersectionPoints.Add(FindIntersection());

                if (_firstSegment.Item2.X > _secondSegment.Item2.X) iter2++;
                else iter1++;
            }

            var isFirstPointRight = timeSeries1.Points.First().X > timeSeries2.Points.First().X
                ? true
                : false;

            var isLastPointLeft = timeSeries1.Points.Last().X < timeSeries2.Points.Last().X
                ? true
                : false;

            var counter = 0;

            if (Math.Abs(timeSeries1.Points.First().X - timeSeries2.Points.First().X) > Eps)
                firstGreater = IsFirstGreater(isFirstPointRight, timeSeries1, timeSeries2);
            else
                firstGreater = timeSeries1.Points.First().Y > timeSeries2.Points.First().Y;

            if (firstGreater) {
                result.Add(isFirstPointRight
                    ? new Tuple<Point, Point>(timeSeries1.Points.First(), intersectionPoints[counter])
                    : new Tuple<Point, Point>(timeSeries2.Points.First(), intersectionPoints[counter]));

                while (counter + 2 < intersectionPoints.Count) {
                    counter += 2;
                    result.Add(new Tuple<Point, Point>(intersectionPoints[counter - 1], intersectionPoints[counter]));
                }

                if (intersectionPoints.Count % 2 == 0)
                    result.Add(isLastPointLeft
                        ? new Tuple<Point, Point>(intersectionPoints[counter + 1], timeSeries1.Points.Last())
                        : new Tuple<Point, Point>(intersectionPoints[counter + 1], timeSeries2.Points.Last()));
            }
            else {
                do {
                    result.Add(new Tuple<Point, Point>(intersectionPoints[counter], intersectionPoints[counter + 1]));
                    counter += 2;
                } while (counter + 1 < intersectionPoints.Count);

                if (intersectionPoints.Count % 2 == 1)
                    result.Add(isLastPointLeft
                        ? new Tuple<Point, Point>(intersectionPoints[counter], timeSeries1.Points.Last())
                        : new Tuple<Point, Point>(intersectionPoints[counter], timeSeries2.Points.Last()));
            }

            return result;
        }

        private bool IsFirstGreater(bool isFirstPointRight, TimeSeries ts1, TimeSeries ts2) {
            if (isFirstPointRight) {
                var p = ts1.Points.First();
                var i = -1;
                while (ts2.Points[++i].X < p.X) ;

                var dx1 = ts2.Points[i].X - ts2.Points[i - 1].X;
                var dy1 = ts2.Points[i].Y - ts2.Points[i - 1].Y;
                var hypo1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
                var dx2 = ts1.Points.First().X - ts2.Points[i - 1].X;
                var dy2 = ts1.Points.First().Y - ts2.Points[i - 1].Y;
                var hypo2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);

                var cos1 = dx1 / hypo1;
                var cos2 = dx2 / hypo2;
                var sin1 = dy1 / hypo1;
                var sin2 = dy2 / hypo2;

                if (sin1 > 0 && sin2 > 0) return cos2 < cos1;

                if (sin2 > 0 && sin1 < 0) return true;

                if (sin1 > 0 && sin2 < 0) return false;
                return !(cos1 > cos2);
            }
            else {
                var p = ts2.Points.First();
                var i = -1;
                while (ts1.Points[++i].X < p.X) ;

                var dx1 = ts1.Points[i].X - ts1.Points[i - 1].X;
                var dy1 = ts1.Points[i].Y - ts1.Points[i - 1].Y;
                var hypo1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
                var dx2 = ts2.Points.First().X - ts1.Points[i - 1].X;
                var dy2 = ts2.Points.First().Y - ts1.Points[i - 1].Y;
                var hypo2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);

                var cos1 = dx1 / hypo1;
                var cos2 = dx2 / hypo2;
                var sin1 = dy1 / hypo1;
                var sin2 = dy2 / hypo2;

                if (sin1 > 0 && sin2 > 0) return !(cos2 < cos1);

                if (sin2 > 0 && sin1 < 0) return false;

                if (sin1 > 0 && sin2 < 0) return true;
                return cos1 > cos2;
            }
        }

        private int FindIntersectionIters(TimeSeries ts1, TimeSeries ts2, ref int iter1, ref int iter2) {
            while (iter1 + 1 < ts1.Points.Count && iter2 + 1 < ts2.Points.Count) {
                _firstSegment = new Tuple<Point, Point>(ts1.Points[iter1], ts1.Points[iter1 + 1]);
                _secondSegment = new Tuple<Point, Point>(ts2.Points[iter2], ts2.Points[iter2 + 1]);

                if (_firstSegment.Item2.X < _secondSegment.Item1.X) {
                    iter1++;
                    continue;
                }

                if (_firstSegment.Item1.X > _secondSegment.Item2.X) {
                    iter2++;
                    continue;
                }

                if (AreCrossed()) return 0;
                if (_firstSegment.Item2.X > _secondSegment.Item2.X) iter2++;
                else iter1++;
            }

            return -1;
        }

        private Point FindIntersection() {
            var result = new Point();

            var dx1 = _firstSegment.Item2.X - _firstSegment.Item1.X;
            var dy1 = _firstSegment.Item2.Y - _firstSegment.Item1.Y;
            var dx2 = _secondSegment.Item2.X - _secondSegment.Item1.X;
            var dy2 = _secondSegment.Item2.Y - _secondSegment.Item1.Y;
            var x = dy1 * dx2 - dy2 * dx1;
            var y = _secondSegment.Item1.X * _secondSegment.Item2.Y -
                    _secondSegment.Item2.X * _secondSegment.Item1.Y;

            result.X =
                ((_firstSegment.Item1.X * _firstSegment.Item2.Y - _firstSegment.Item2.X * _firstSegment.Item1.Y) * dx2 -
                 y * dx1) / x;
            result.Y = (dy2 * result.X - y) / dx2;

            return result;
        }

        private bool AreCrossed() {
            var mul1 = (_secondSegment.Item2.X - _secondSegment.Item1.X) *
                       (_firstSegment.Item1.Y - _secondSegment.Item1.Y) -
                       (_secondSegment.Item2.Y - _secondSegment.Item1.Y) *
                       (_firstSegment.Item1.X - _secondSegment.Item1.X);

            var mul2 = (_secondSegment.Item2.X - _secondSegment.Item1.X) *
                       (_firstSegment.Item2.Y - _secondSegment.Item1.Y) -
                       (_secondSegment.Item2.Y - _secondSegment.Item1.Y) *
                       (_firstSegment.Item2.X - _secondSegment.Item1.X);

            var mul3 = (_firstSegment.Item2.X - _firstSegment.Item1.X) *
                       (_secondSegment.Item1.Y - _firstSegment.Item1.Y) -
                       (_firstSegment.Item2.Y - _firstSegment.Item1.Y) *
                       (_secondSegment.Item1.X - _firstSegment.Item1.X);

            var mul4 = (_firstSegment.Item2.X - _firstSegment.Item1.X) *
                       (_secondSegment.Item2.Y - _firstSegment.Item1.Y) -
                       (_firstSegment.Item2.Y - _firstSegment.Item1.Y) *
                       (_secondSegment.Item2.X - _firstSegment.Item1.X);


            return mul1 * mul2 < 0 && mul3 * mul4 < 0;
        }
    }
}