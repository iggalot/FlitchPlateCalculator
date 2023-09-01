using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using static FlitchPlateCalculator.Utilities.GeometryHelpers;

namespace FlitchPlateCalculator.Models
{
    public class OverlappedPlate
    {
        public int p1_id;
        public int p2_id;

        public OverlappedPlate(int first, int second)
        {
            p1_id = first;
            p2_id = second;
        }
    }

    public class FlitchPlateModel
    {
        // Dimensional extents of the bounding box
        public Point BB_p1_WORLD { get; set; }

        // Minimum extents of the bounding box
        public Point BB_p2_WORLD { get; set; }
        public bool IsValidModel { get; set; }

        public ObservableCollection<OverlappedPlate> ocOverlappedPlate = new ObservableCollection<OverlappedPlate>();

        #region Properties
        /// <summary>
        /// Plates of the assembly
        /// </summary>
        public List<PlateModel> Plates { get; set; } = new List<PlateModel>();

        /// <summary>
        /// Centroid point of the composite plate
        /// </summary>
        public Point Centroid_Untransformed { get; set; }

        /// <summary>
        /// Untransformed composite area
        /// </summary>
        public double Area_Untransformed { get; set; }

        /// <summary>
        /// Untransformed composite moment of inertia
        /// </summary>
        public double Ix_Untr { get; set; }
        public double Iy_Untr { get; set; }
        
        /// <summary>
        /// Weight
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Elastic Section Modulii
        /// </summary>
        public double Sy_Right_Untr { get; set; }
        public double Sy_Left_Untr { get; set; }
        public double Sx_Bot_Untr { get; set; }
        public double Sx_Top_Untr { get; set; }

        /// <summary>
        /// Radii of gyration
        /// </summary>
        public double rx_Untr { get; set; }
        public double ry_Untr { get; set; }

        /// <summary>
        /// Plastic Section Modulii
        /// </summary>
        public double Zx_Untr { get; set; }
        public double Zy_Untr { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// default constructor
        /// </summary>
        public FlitchPlateModel()
        {
            //UpdateCalculations();
        }
        #endregion

        /// <summary>
        /// Main function to call to update all of the calculations
        /// </summary>
        public void UpdateCalculations()
        {
            ValidateModel();
            FindBoundingBox_Untransformed_World();
            CalculateCentroid_Untransformed();
            CalculateTotalArea_Untransformed();
            CalculateInertia_Untransformed();
            CalculateRadiusOfGyration_Untransformed();
            CalculateElasticSectionModulii_Untransformed();
            CalculatePlasticSectionModulii_Untransformed();
            CalculateWeight();
        }

        public void ValidateModel()
        {
            if (ValidateMaterialType() && ValidateOverlap())
            {
                IsValidModel = true;
            } else
            {
                IsValidModel = false;
            }
        }

        /// <summary>
        /// Checks the model plates for overlap condition
        /// </summary>
        public bool ValidateOverlap()
        {
            ocOverlappedPlate.Clear();

            // Check for overlapping plates
            for (int i = 0; i < Plates.Count; i++)
            {
                // is plate 'i' x-corner between plate 'j' x-corner?
                double i_x_left = Math.Round(Plates[i].Centroid.X - Plates[i].Width * 0.5, 3);
                double i_x_right = Math.Round(Plates[i].Centroid.X + Plates[i].Width * 0.5, 3);
                double i_y_top = Math.Round(Plates[i].Centroid.Y + Plates[i].Height * 0.5, 3);
                double i_y_bot = Math.Round(Plates[i].Centroid.Y - Plates[i].Height * 0.5, 3);

                for (int j = 0; j < Plates.Count; j++)
                {
                    // are the two plates the same?  If so skip and continue searching
                    if (i == j)
                    {
                        continue;
                    }

                    double j_x_left = Math.Round(Plates[j].Centroid.X - Plates[j].Width * 0.5, 3);
                    double j_x_right = Math.Round(Plates[j].Centroid.X + Plates[j].Width * 0.5, 3);
                    double j_y_top = Math.Round(Plates[j].Centroid.Y + Plates[j].Height * 0.5, 3);
                    double j_y_bot = Math.Round(Plates[j].Centroid.Y - Plates[j].Height * 0.5, 3);

                    // if any of the edge lines intersect then they overlap
                    //top A top B

                    bool rectangles_overlap = false;

                    IntersectPointData intersect_point1 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_top, j_x_left, j_y_top, j_x_right, j_y_top);
                    if (intersect_point1.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }
                    //top A right B
                    IntersectPointData intersect_point2 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_top, j_x_right, j_y_top, j_x_right, j_y_bot);
                    if (intersect_point2.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //top A bot B
                    IntersectPointData intersect_point3 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_top, j_x_left, j_y_bot, j_x_right, j_y_bot);
                    if (intersect_point3.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }
                    //top A left B
                    IntersectPointData intersect_point4 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_top, j_x_left, j_y_bot, j_x_left, j_y_bot);
                    if (intersect_point4.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //right A top B
                    IntersectPointData intersect_point5 = FindPointOfIntersectLines_2D(i_x_right, i_y_top, i_x_right, i_y_bot, j_x_left, j_y_top, j_x_right, j_y_top);
                    if (intersect_point5.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }
                    //right A right B
                    IntersectPointData intersect_point6 = FindPointOfIntersectLines_2D(i_x_right, i_y_top, i_x_right, i_y_bot, j_x_right, j_y_top, j_x_right, j_y_bot);
                    if (intersect_point6.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }
                    //right A bot B
                    IntersectPointData intersect_point7 = FindPointOfIntersectLines_2D(i_x_right, i_y_top, i_x_right, i_y_bot, j_x_left, j_y_bot, j_x_right, j_y_bot);
                    if (intersect_point7.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }
                    //right A left B
                    IntersectPointData intersect_point8 = FindPointOfIntersectLines_2D(i_x_right, i_y_top, i_x_right, i_y_bot, j_x_left, j_y_bot, j_x_left, j_y_bot);
                    if (intersect_point8.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //bot A top B
                    IntersectPointData intersect_point9 = FindPointOfIntersectLines_2D(i_x_left, i_y_bot, i_x_right, i_y_bot, j_x_left, j_y_top, j_x_right, j_y_top);
                    if (intersect_point9.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //bot A right B
                    IntersectPointData intersect_point10 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_bot, j_x_right, j_y_top, j_x_right, j_y_bot);
                    if (intersect_point10.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //bot A bot B
                    IntersectPointData intersect_point11 = FindPointOfIntersectLines_2D(i_x_left, i_y_bot, i_x_right, i_y_bot, j_x_left, j_y_bot, j_x_right, j_y_bot);
                    if (intersect_point11.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //bot A left B
                    IntersectPointData intersect_point12 = FindPointOfIntersectLines_2D(i_x_left, i_y_bot, i_x_right, i_y_bot, j_x_left, j_y_bot, j_x_left, j_y_bot);
                    if (intersect_point12.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //left A top B
                    IntersectPointData intersect_point13 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_left, i_y_bot, j_x_left, j_y_top, j_x_right, j_y_top);
                    if (intersect_point13.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //left A right B
                    IntersectPointData intersect_point14 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_left, i_y_bot, j_x_right, j_y_top, j_x_right, j_y_bot);
                    if (intersect_point14.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //left A bot B
                    IntersectPointData intersect_point15 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_left, i_y_bot, j_x_left, j_y_bot, j_x_right, j_y_bot);
                    if (intersect_point15.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    //left A left B
                    IntersectPointData intersect_point16 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_left, i_y_bot, j_x_left, j_y_bot, j_x_left, j_y_bot);
                    if (intersect_point16.isWithinSegment == true)
                    {
                        rectangles_overlap = true;
                    }

                    // if rectangles dont have intersecting edges, need to check case that j plate is completely inside i plate
                    if (j_x_left >= i_x_left && j_x_right <= i_x_right && j_y_top <= i_y_top && j_y_bot >= i_y_bot)
                    {
                        rectangles_overlap = true;
                    }

                    // add the plates to our list of overlap
                    if (rectangles_overlap is true)
                    {
                        bool already_exists = false;
                        // search if the overlapped plates are not already in the collection
                        foreach(var item in ocOverlappedPlate)
                        {
                            if((item.p1_id == Plates[i].Id && item.p2_id == Plates[j].Id) ||
                                (item.p1_id == Plates[j].Id && item.p2_id == Plates[i].Id))
                            {
                                already_exists = true;
                                break;
                            }
                        }

                        if (already_exists == false)
                        {
                            OverlappedPlate ov_plate = new OverlappedPlate(Plates[i].Id, Plates[j].Id);
                            ocOverlappedPlate.Add(ov_plate);
                        }
                    }
                }
            }

            return (ocOverlappedPlate.Count == 0) ? true : false;
        }

        public bool ValidateMaterialType()
        {
            // Does the model have any undefined materials?
            foreach (PlateModel plate in Plates)
            {
                if (plate.Material.MaterialType == MaterialTypes.MATERIAL_UNDEFINED)
                {
                    return false;
                }
            }

            return true;
        }

        #region Plate Manipulations Methods
        /// <summary>
        /// Add a plate to the assembly at a specified location and orientation.
        /// </summary>
        /// <param name="plate"></param>
        /// <param name="angle"></param>
        /// <param name="point"></param>
        public void AddPlate(PlateModel plate)
        {
            if(plate != null)
            {
                Plates.Add(plate);
                UpdateCalculations();
            }
        }

        public void CreatePlateCopy(PlateModel plate)
        {
            if (plate != null)
            {
                AddPlate(plate);
                UpdateCalculations();
            }
        }


        /// <summary>
        /// Remove a plate from the assembly
        /// </summary>
        /// <param name="id"></param>
        public void RemovePlate(PlateModel plate)
        {
            if (plate != null)
            {
                for (int i = 0; i < Plates.Count; i++)
                {
                    if (Plates[i].Id == plate.Id)
                    {
                        Plates.RemoveAt(i);
                        UpdateCalculations();
                        return;
                    }
                }

                MessageBox.Show("Plate #" + plate.Id + " is not a member of this flitch assembly.");
                return;
            }
        }

        /// <summary>
        /// Rotates the specified plate in the assembly by swapping the height and width dimension
        /// </summary>
        /// <param name="plate"></param>
        public void RotatePlate(PlateModel plate)
        {
            if (plate != null)
            {
                for (int i = 0; i < Plates.Count; i++)
                {
                    if (Plates[i].Id == plate.Id)
                    {
                        double temp;
                        temp = Plates[i].Width;
                        Plates[i].Width = Plates[i].Height;
                        Plates[i].Height = temp;

                        UpdateCalculations();
                        return;
                    }
                }

                MessageBox.Show("Plate #" + plate.Id + " is not a member of this flitch assembly.");
                return;
            }
        }
        #endregion

        #region Engineering Calculation Methods
        private void CalculateWeight()
        {
            double wt = 0;
            for (int i = 0; i < Plates.Count; i++)
            {
                wt += Plates[i].Weight;
            }

            Weight = wt;

            return;
        }

        /// <summary>
        /// Calculates the untransformed area of the gross cross section
        /// </summary>
        private void CalculateTotalArea_Untransformed()
        {
            double ai = 0;
            for (int i = 0; i < Plates.Count; i++)
            {
                ai += Plates[i].Area;
            }

            Area_Untransformed = ai;

            return;
        }

        /// <summary>
        /// Calculates the untransformed moment of inertia
        /// </summary>
        private void CalculateCentroid_Untransformed()
        {
            double ai = 0;
            double aixi = 0;
            double aiyi = 0;
            for (int i = 0; i < Plates.Count; i++)
            {
                ai += Plates[i].Area;
                aixi += Plates[i].Area * Plates[i].Centroid.X;
                aiyi += Plates[i].Area * Plates[i].Centroid.Y;
            }

            Centroid_Untransformed = new Point(aixi / ai, aiyi / ai);

            return;
        }

        /// <summary>
        /// Calculates the untransformed (gross) moments of inertia for the assembly
        /// </summary>
        private void CalculateInertia_Untransformed()
        {
            double ix_untr = 0;
            double iy_untr = 0;
            for (int i = 0; i < Plates.Count; i++)
            {
                ix_untr += Plates[i].ParallelX_Inertia(Centroid_Untransformed);
                iy_untr += Plates[i].ParallelY_Inertia(Centroid_Untransformed);
            }

            Ix_Untr = ix_untr;
            Iy_Untr = iy_untr;

            return;
        }

        /// <summary>
        /// Calculates the untransformed (gross) radii of gyration for the assembly
        /// </summary>
        private void CalculateRadiusOfGyration_Untransformed()
        {
            if(Area_Untransformed == 0)
            {
                rx_Untr = -1;
                ry_Untr = -1;
            }
            else
            {
                rx_Untr = Math.Sqrt(Ix_Untr / Area_Untransformed);
                ry_Untr = Math.Sqrt(Iy_Untr / Area_Untransformed);
            }
        }

        /// <summary>
        /// Calculates the untransformed (gross) elastic section modulii for the assembly
        /// </summary>
        private void CalculateElasticSectionModulii_Untransformed()
        {
            double top = Math.Abs(BB_p2_WORLD.Y - Centroid_Untransformed.Y);
            double bot = Math.Abs(BB_p1_WORLD.Y - Centroid_Untransformed.Y);
            double left = Math.Abs(BB_p1_WORLD.X - Centroid_Untransformed.X);
            double right = Math.Abs(BB_p1_WORLD.X - Centroid_Untransformed.X);

            Sx_Top_Untr = Ix_Untr / top;
            Sx_Bot_Untr = Ix_Untr / bot;
            Sy_Left_Untr = Iy_Untr / left;
            Sy_Right_Untr = Iy_Untr / right;
        }

        /// <summary>
        /// Calculates the untransformed (gross) plastic section modulii for the assembly
        /// </summary>
        private void CalculatePlasticSectionModulii_Untransformed()
        {
            double top = Math.Abs(BB_p2_WORLD.Y - Centroid_Untransformed.Y);
            double bot = Math.Abs(BB_p1_WORLD.Y - Centroid_Untransformed.Y);
            double left = Math.Abs(BB_p1_WORLD.X - Centroid_Untransformed.X);
            double right = Math.Abs(BB_p1_WORLD.X - Centroid_Untransformed.X);

            double x_total = 0;

            // Plastic Section Modulus about Horizontal Centroid axis
            foreach (PlateModel plate in Plates)
            {
                // plate fully above or below centroid
                if ((((plate.Centroid.Y + plate.Height * 0.5) > Centroid_Untransformed.Y) &&
                    ((plate.Centroid.Y - plate.Height * 0.5) > Centroid_Untransformed.Y))
                    ||
                    (((plate.Centroid.Y + plate.Height * 0.5) < Centroid_Untransformed.Y) &&
                    ((plate.Centroid.Y - plate.Height * 0.5) < Centroid_Untransformed.Y)))

                {
                    x_total += Math.Abs(plate.Area * (plate.Centroid.Y - Centroid_Untransformed.Y));
                }

                // plate split by centroid so need to calculate the two pieces above
                else
                {
                    double height_above = Math.Abs(((plate.Centroid.Y + plate.Height * 0.5) - Centroid_Untransformed.Y));
                    double centroid_above = 0.5 * height_above;
                    double area_above = height_above * plate.Width;

                    double height_below = Math.Abs((Centroid_Untransformed.Y - (plate.Centroid.Y - plate.Height * 0.5)));
                    double centroid_below = 0.5 * height_below;
                    double area_below = height_below * plate.Width;

                    x_total += area_above * centroid_above;
                    x_total += area_below * centroid_below;
                }
            }

            Zx_Untr = x_total;

            double y_total = 0;
            // Plastic Section Modulus about Vertical Centroid axis
            foreach (PlateModel plate in Plates)
            {
                // plate fully left or right centroid
                if ((((plate.Centroid.X + plate.Width * 0.5) > Centroid_Untransformed.X) &&
                    ((plate.Centroid.X - plate.Width * 0.5) > Centroid_Untransformed.X))
                    ||
                    (((plate.Centroid.X + plate.Width * 0.5) < Centroid_Untransformed.X) &&
                    ((plate.Centroid.X - plate.Width * 0.5) < Centroid_Untransformed.X)))

                {
                    y_total += Math.Abs(plate.Area * (plate.Centroid.X - Centroid_Untransformed.X));
                }

                // plate split by centroid so need to calculate the two pieces above
                else
                {
                    double width_right = Math.Abs(((plate.Centroid.X + plate.Width * 0.5) - Centroid_Untransformed.X));
                    double centroid_right = 0.5 * width_right;
                    double area_right = width_right * plate.Height;

                    double width_left = Math.Abs((Centroid_Untransformed.X - (plate.Centroid.X - plate.Width * 0.5)));
                    double centroid_left = 0.5 * width_left;
                    double area_left = width_left * plate.Height;

                    y_total += area_right * centroid_right;
                    y_total += area_left * centroid_left;
                }
            }
            Zy_Untr = y_total;

        }

        #endregion

        public override string ToString()
        {
            string str = "";
            str += "\nUntransformed Centroid -- X: " + Centroid_Untransformed.X.ToString() + " in.   Y: " + Centroid_Untransformed.Y.ToString() + " in.";
            str += "\nWt per ft: " + Weight.ToString() + " plf";
            str += "\nA_untr: " + Area_Untransformed.ToString() + " in^2";
            str += "\nIx_untr: " + Ix_Untr.ToString() + " in^4";
            str += "\nIy_untr: " + Iy_Untr.ToString() + " in^4";
            str += "\nrx_untr: " + rx_Untr.ToString() + " in^4";
            str += "\nry_untr: " + ry_Untr.ToString() + " in^4";
            str += "\nSx_top_untr: " + Sx_Top_Untr.ToString() + " in^4";
            str += "\nSx_bot_untr: " + Sx_Bot_Untr.ToString() + " in^4";
            str += "\nSy_left_untr: " + Sy_Left_Untr.ToString() + " in^4";
            str += "\nSy_right_untr: " + Sy_Right_Untr.ToString() + " in^4";

            return str;
        }

        /// <summary>
        /// Find the extreme limits of the objects in world space
        /// </summary>
        public void FindBoundingBox_Untransformed_World()
        {
            double min_x=0;
            double min_y=0;
            double max_x=0;
            double max_y=0;

            // Large values to ensure that the first one is the smallest.
            if (Plates.Count > 0)
            {
                // min values set large
                min_x = 1000000;
                min_y = 1000000;

                // max values set small
                max_x = -1000000;
                max_y = -1000000;

                foreach (PlateModel plate in Plates)
                {
                    if (plate.Centroid.X - 0.5 * plate.Width < min_x)
                    {
                        min_x = plate.Centroid.X - 0.5 * plate.Width;
                    }

                    if (plate.Centroid.Y - 0.5 * plate.Height < min_y)
                    {
                        min_y = plate.Centroid.Y - 0.5 * plate.Height;
                    }

                    if (plate.Centroid.X + 0.5 * plate.Width > max_x)
                    {
                        max_x = plate.Centroid.X + 0.5 * plate.Width;
                    }

                    if (plate.Centroid.Y + 0.5 * plate.Height > max_y)
                    {
                        max_y = plate.Centroid.Y + 0.5 * plate.Height;
                    }
                }
            }

            BB_p1_WORLD = new Point(min_x, min_y);
            BB_p2_WORLD = new Point(max_x, max_y);
        }
    }
}
