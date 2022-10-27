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
        public PlateModel(int id, double width, double height, Point point, MaterialTypes mat_type = MaterialTypes.MATERIAL_STEEL)
        {
            Id = id;
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
            if(Material.MaterialType == MaterialTypes.MATERIAL_UNDEFINED)
            {
                return -1000000000;
            }
            if(Material.MaterialType == MaterialTypes.MATERIAL_STEEL)
            {
                return Area * 490 / 144.0;
            } else
            {
                return Area * 45 / 144.0;
            }
        }



        /// <summary>
        /// Compares this plate model to another plate model, returning true is dimensions are the same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsSamePlate(object obj)
        {
            PlateModel model_plate = obj as PlateModel;

            if(model_plate.Weight != this.Weight)
            {
                return false;
            }

            if(((this.Width == model_plate.Width) && (this.Height == model_plate.Height)))
            {
                // continue 
            } else 
            {
                // is it rotated?
                if (((this.Width == model_plate.Height) && (this.Height == model_plate.Width)))
                {
                    // continue
                } else
                {
                    return false;
                }
            }

            if (model_plate.Material.MaterialType != this.Material.MaterialType)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Generic override 
        /// </summary>
        /// <returns></returns>
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
