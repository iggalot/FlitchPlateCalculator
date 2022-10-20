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

            return str;
        }
    }
}
