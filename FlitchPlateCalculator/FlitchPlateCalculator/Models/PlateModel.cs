using System.Windows;

namespace FlitchPlateCalculator.Models
{
    public class PlateModel
    {
        // Id of the plate
        public int Id { get; set; }

        // MAterial of the plate
        public MaterialModel Material { get; set; }

        // Height of plate -- default vertical
        public double Height { get; set; }

        // Width of the plate -- default horizontal
        public double Width { get; set; }

        // centroid location
        public Point Centroid { get; set; }

        // Weight / ft
        public double Weight { get => ComputeWeight(); }

        /// <summary>
        /// Computes area of the plate
        /// </summary>
        public double Area { get => Height * Width; }

        /// <summary>
        /// Computes moment of inertia about the horizontal (x-) centroidal axis
        /// </summary>
        public double Ixo { get => Width * Height * Height * Height / 12.0; }

        /// <summary>
        /// Computes moment of inertia about the vertical (y-) centroidal axis
        /// </summary>
        public double Iyo { get => Height * Width * Width * Width / 12.0; }

        public string InertiaToString { 
            get => "Ixo: " + Ixo.ToString() + " in^4     Iyo: " + Iyo.ToString() +" in^4";
        }

        /// <summary>
        /// Plate constructor
        /// </summary>
        /// <param name="width">Horizonal dimension</param>
        /// <param name="height">Vertical dimension</param>
        /// <param name="point">Centroid position</param>
        public PlateModel(double width, double height, Point point, MaterialTypes mat_type = MaterialTypes.MATERIAL_STEEL)
        {
            if (mat_type == MaterialTypes.MATERIAL_UNDEFINED)
                return;

            Material = new MaterialModel(mat_type);

            Width = width;
            Height = height;
            Centroid = point;
        }

        /// <summary>
        /// Returns the X- moment of inertial about an axis through an arbitrary point.
        /// </summary>
        /// <param name="point">Point of the parallel axis origin</param>
        /// <returns></returns>
        public double ParallelX_Inertia(Point point)
        {
            return (Ixo + Area * (Centroid.Y - point.Y) * (Centroid.Y - point.Y));
        }

        /// <summary>
        /// Returns the X- moment of inertial about an axis through an arbitrary point.
        /// </summary>
        /// <param name="point">Point of the parallel axis origin</param>
        /// <returns></returns>
        public double ParallelY_Inertia(Point point)
        {
            return (Iyo + Area * (Centroid.X - point.X) * (Centroid.X - point.X));
        }

        private double ComputeWeight()
        {
            // steel
            if(Material.MaterialType == MaterialTypes.MATERIAL_STEEL)
            {
                return Area * 490 / 144.0;
            } else
            {
                return Area * 45 / 144.0;
            }
        }

        public override string ToString()
        {
            string str = "";
            str += "\nCentroid -- X: " + Centroid.X.ToString() + " in.   Y: " + Centroid.Y.ToString() + " in.";
            str += "\nWt per ft: " + Weight.ToString() + " plf";
            str += "\nArea: " + Area.ToString() + " in^2";
            str += "\nIxo: " + Ixo.ToString() + " in^4";
            str += "\nIyo: " + Iyo.ToString() + " in^4";

            return str;
        }

    }
}
