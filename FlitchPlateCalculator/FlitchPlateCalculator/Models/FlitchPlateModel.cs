using System;
using System.Collections.Generic;
using System.Windows;

namespace FlitchPlateCalculator.Models
{
    public class FlitchPlateModel
    {
        /// <summary>
        /// Plates of the assembly
        /// </summary>
        public List<PlateModel> Plates { get; set; } = new List<PlateModel>();

        /// <summary>
        /// Centroid point of the composite plate
        /// </summary>
        public Point Centroid_Untransformed { get; set; }

        public double Area_Untransformed { get; set; }

        public double Ix_Untr { get; set; }
        public double Iy_Untr { get; set; }
        
        public double Weight { get; set; }
        public double Sy_Right_Untr { get; set; }
        public double Sy_Left_Untr { get; set; }
        public double Sx_Bot_Untr { get; set; }
        public double Sx_Top_Untr { get; set; }

        public double rx_Untr { get; set; }
        public double ry_Untr { get; set; }

        public double Zx_Untr { get; set; }
        public double Zy_Untr { get; set; }


        // Minimum extents of the bounding box
        public Point BB_p1_WORLD { get; set; }

        // Minimum extents of the bounding box
        public Point BB_p2_WORLD { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public FlitchPlateModel()
        {
            UpdateCalculations();
        }

        /// <summary>
        /// Add a plate to the assembly at a specified location and orientation.
        /// </summary>
        /// <param name="plate"></param>
        /// <param name="angle"></param>
        /// <param name="point"></param>
        public void AddPlate(PlateModel plate)
        {
            Plates.Add(plate);
            UpdateCalculations();
        }

        /// <summary>
        /// Remove a plate from the assembly
        /// </summary>
        /// <param name="id"></param>
        
        public void RemovePlate(int id)
        {
            for (int i = 0; i < Plates.Count; i++)
            {
                if(Plates[i].Id == id)
                {
                    Plates.RemoveAt(i);
                    UpdateCalculations();
                    return;
                }
            }

            MessageBox.Show("Plate #" + id + " is not a member of this flitch assembly.");
            return;
        }

        public void UpdateCalculations()
        {
            CalculateCentroid_Untransformed();
            CalculateTotalArea_Untransformed();
            CalculateInertia_Untransformed();
            CalculateRadiusOfGyration_Untransformed();

            CalculateElasticSectionModulii_Untransformed();
            CalculatePlasticSectionModulii_Untransformed();
            
            CalculateWeight();
        }

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
        /// Calculates the untransformed (gross) moments of inertia for the plate
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

        public void Update()
        {
            UpdateCalculations();
        }

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
            double min_x = 0;
            double min_y = 0;
            double max_x = 0;
            double max_y = 0;

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

            BB_p1_WORLD = new Point(min_x, min_y);
            BB_p2_WORLD = new Point(max_x, max_y);
        }
    }
}
