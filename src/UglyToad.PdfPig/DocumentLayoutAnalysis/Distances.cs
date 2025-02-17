﻿using System;
using System.Collections.Generic;
using System.Linq;
using UglyToad.PdfPig.Geometry;

namespace UglyToad.PdfPig.DocumentLayoutAnalysis
{
    /// <summary>
    /// Contains helpful tools for distance measures.
    /// </summary>
    public static class Distances
    {
        /// <summary>
        /// The Euclidean distance is the "ordinary" straight-line distance between two points.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        public static double Euclidean(PdfPoint point1, PdfPoint point2)
        {
            double dx = (double)(point1.X - point2.X);
            double dy = (double)(point1.Y - point2.Y);
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// The weighted Euclidean distance.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="wX">The weight of the X coordinates. Default is 1.</param>
        /// <param name="wY">The weight of the Y coordinates. Default is 1.</param>
        public static double WeightedEuclidean(PdfPoint point1, PdfPoint point2, double wX = 1.0, double wY = 1.0)
        {
            double dx = (double)(point1.X - point2.X);
            double dy = (double)(point1.Y - point2.Y);
            return Math.Sqrt(wX * dx * dx + wY * dy * dy);
        }

        /// <summary>
        /// The Manhattan distance between two points is the sum of the absolute differences of their Cartesian coordinates.
        /// <para>Also known as rectilinear distance, L1 distance, L1 norm, snake distance, city block distance, taxicab metric.</para>
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        public static double Manhattan(PdfPoint point1, PdfPoint point2)
        {
            return (double)(Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y));
        }

        /// <summary>
        /// The angle in degrees between the horizontal axis and the line between two points.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns></returns>
        public static double Angle(PdfPoint point1, PdfPoint point2)
        {
            return Math.Atan2((float)(point2.Y - point1.Y), (float)(point2.X - point1.X)) * 180.0 / Math.PI;
        }

        /// <summary>
        /// The absolute distance between the Y coordinates of two points.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns></returns>
        public static double Vertical(PdfPoint point1, PdfPoint point2)
        {
            return Math.Abs((double)(point2.Y - point1.Y));
        }

        /// <summary>
        /// The absolute distance between the X coordinates of two points.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <returns></returns>
        public static double Horizontal(PdfPoint point1, PdfPoint point2)
        {
            return Math.Abs((double)(point2.X - point1.X));
        }

        /// <summary>
        /// Find the index of the nearest point, excluding itself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The reference point, for which to find the nearest neighbour.</param>
        /// <param name="candidates">The list of neighbours candidates.</param>
        /// <param name="candidatesPoint"></param>
        /// <param name="pivotPoint"></param>
        /// <param name="distanceMeasure">The distance measure to use.</param>
        /// <param name="distance">The distance between reference point, and its nearest neighbour.</param>
        /// <returns></returns>
        internal static int FindIndexNearest<T>(this T element, IReadOnlyList<T> candidates,
            Func<T, PdfPoint> candidatesPoint, Func<T, PdfPoint> pivotPoint,
            Func<PdfPoint, PdfPoint, double> distanceMeasure, out double distance)
        {
            if (candidates == null || candidates.Count == 0)
            {
                throw new ArgumentException("Distances.FindIndexNearest(): The list of neighbours candidates is either null or empty.", "points");
            }

            if (distanceMeasure == null)
            {
                throw new ArgumentException("Distances.FindIndexNearest(): The distance measure must not be null.", "distanceMeasure");
            }

            distance = double.MaxValue;
            int closestPointIndex = -1;
            var candidatesPoints = candidates.Select(candidatesPoint).ToList();
            var pivot = pivotPoint(element);

            for (var i = 0; i < candidates.Count; i++)
            {
                double currentDistance = distanceMeasure(candidatesPoints[i], pivot);
                if (currentDistance < distance && !candidates[i].Equals(element))
                {
                    distance = currentDistance;
                    closestPointIndex = i;
                }
            }

            return closestPointIndex;
        }

        /// <summary>
        /// Find the index of the nearest line, excluding itself.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The reference line, for which to find the nearest neighbour.</param>
        /// <param name="candidates">The list of neighbours candidates.</param>
        /// <param name="candidatesLine"></param>
        /// <param name="pivotLine"></param>
        /// <param name="distanceMeasure">The distance measure between two lines to use.</param>
        /// <param name="distance">The distance between reference line, and its nearest neighbour.</param>
        internal static int FindIndexNearest<T>(this T element, IReadOnlyList<T> candidates,
            Func<T, PdfLine> candidatesLine, Func<T, PdfLine> pivotLine,
            Func<PdfLine, PdfLine, double> distanceMeasure, out double distance)
        {
            if (candidates == null || candidates.Count == 0)
            {
                throw new ArgumentException("Distances.FindIndexNearest(): The list of neighbours candidates is either null or empty.", "lines");
            }

            if (distanceMeasure == null)
            {
                throw new ArgumentException("Distances.FindIndexNearest(): The distance measure must not be null.", "distanceMeasure");
            }

            distance = double.MaxValue;
            int closestLineIndex = -1;
            var candidatesLines = candidates.Select(candidatesLine).ToList();
            var pivot = pivotLine(element);

            for (var i = 0; i < candidates.Count; i++)
            {
                double currentDistance = distanceMeasure(candidatesLines[i], pivot);
                if (currentDistance < distance && !candidates[i].Equals(element))
                {
                    distance = currentDistance;
                    closestLineIndex = i;
                }
            }

            return closestLineIndex;
        }
    }
}
