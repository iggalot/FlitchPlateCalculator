using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlitchPlateCalculator.Utilities
{
    public class GeometryHelpers
    {
        /// <summary>
        /// Class object for data relating to finding intersection points of lines with polylines.
        /// Mostly used for passing back full data.
        /// </summary>
        public class IntersectPointData
        {
            public Point intPoint;
            public bool isParallel;
            public bool isWithinSegment;
            public string logMessage = "";
        }

        /// <summary>
        /// Determines if lines are parallel
        /// </summary>
        /// <param name="l1_sx">l1 start x</param>
        /// <param name="l1_sy">l1 start_y</param>
        /// <param name="l1_ex">l1 end x</param>
        /// <param name="l1_ey">l1 end y</param>
        /// <param name="l2_sx">l2 start x</param>
        /// <param name="l2_sy">l2 start y</param>
        /// <param name="l2_ex">l2 end x</param>
        /// <param name="l2_ey">l2 end y</param>
        /// <returns></returns>
        public static bool LinesAreParallel(double l1_sx, double l1_sy, double l1_ex, double l1_ey, double l2_sx, double l2_sy, double l2_ex, double l2_ey)
        {
            double A1 = l1_ey - l1_sy;
            double A2 = l2_ey - l2_sy;
            double B1 = l1_sx - l1_ex;
            double B2 = l2_sx - l2_ex;
            double C1 = A1 * l1_sx + B1 * l1_sy;
            double C2 = A2 * l2_sx + B2 * l2_sy;

            double det = A1 * B2 - A2 * B1;
            return det == 0;
        }

        /// <summary>
        /// Find the location where two line segements intersect
        /// </summary>
        /// <param name="l1">autocad line object #1</param>
        /// <param name="l2">autocad line objtxt #2</param>
        /// <param name="withinSegment">The coordinate must be within the line segments</param>
        /// <param name="areParallel">returns if the lines are parallel. This needs to be checked everytime as the intersection point defaults to a really large value otherwise</param>
        /// <returns></returns>
        public static IntersectPointData FindPointOfIntersectLines_2D(double l1_sx, double l1_sy, double l1_ex, double l1_ey, double l2_sx, double l2_sy, double l2_ex, double l2_ey)
        {
            double tol = 0.001;  // a tolerance fudge factor since autocad is having issues with rounding at the 9th and 10th decimal place
            double A1 = l1_ey - l1_sy;
            double A2 = l2_ey - l2_sy;
            double B1 = l1_sx - l1_ex;
            double B2 = l2_sx - l2_ex;
            double C1 = A1 * l1_sx + B1 * l1_sy;
            double C2 = A2 * l2_sx + B2 * l2_sy;

            // compute the determinant
            double det = A1 * B2 - A2 * B1;

            double intX, intY;

            IntersectPointData intPtData = new IntersectPointData();
            intPtData.isParallel = LinesAreParallel(l1_sx, l1_sy, l1_ex, l1_ey, l2_sx, l2_sy, l2_ex, l2_ey);

            if (intPtData.isParallel is true)
            {
                // Lines are parallel, but are they the same line?
                intX = double.MaxValue;
                intY = double.MaxValue;
                intPtData.isWithinSegment = false; // cant intersect if the lines are parallel
                //MessageBox.Show("segment is parallel");
                //MessageBox.Show("A1: " + A1 + "\n" + "  B1: " + B1 + "\n" + "  C1: " + C1 + "\n" +
                //    "A2: " + A2 + "\n" + "  B2: " + B2 + "\n" + "  C2: " + C2 + "\n" +
                //    "delta: " + delta);
            }
            else
            {
                intX = (B2 * C1 - B1 * C2) / det;
                intY = (A1 * C2 - A2 * C1) / det;

                intPtData.isWithinSegment = true;
                string msg = "";
                //// Check that the intersection point is between the endpoints of both lines assuming it isnt
                if (((Math.Min(l1_sx, l1_ex) - tol <= intX) && (Math.Max(l1_sx, l1_ex) + tol >= intX)) is false)
                {
                    intPtData.isWithinSegment = false;
                    msg += "line 1 X - failed";
                }
                else if (((Math.Min(l2_sx, l2_ex) - tol <= intX) && (Math.Max(l2_sx, l2_ex) + tol >= intX)) is false)
                {
                    intPtData.isWithinSegment = false;
                    msg += "line 2 X - failed";

                }
                else if (((Math.Min(l1_sy, l1_ey) - tol <= intY) && (Math.Max(l1_sy, l1_ey) + tol >= intY)) is false)
                {
                    intPtData.isWithinSegment = false;
                    msg += "line 3 X - failed";

                }
                else if (((Math.Min(l2_sy, l2_ey) - tol <= intY) && (Math.Max(l2_sy, l2_ey) + tol >= intY)) is false)
                {
                    intPtData.isWithinSegment = false;
                    msg += "line 4 X - failed";

                }
                else
                {
                    intPtData.isWithinSegment = true;
                    msg += "intersection point is within line segment limits";

                }
            }

            intPtData.intPoint = new Point(intX, intY);

            return intPtData;
        }
    }
}
