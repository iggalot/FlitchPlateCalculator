using FlitchPlateCalculator.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlitchPlateCalculator.ViewModels
{
    public class FlitchPlateViewModel : BaseVieWModel
    {
        public FlitchPlateModel Model { get; set; }
        public Canvas FPCanvas { get; set; }

        // Minimum extents of the bounding box
        public Point BB_p1_WORLD { get; set; }

        // Minimum extents of the bounding box
        public Point BB_p2_WORLD { get; set; }

        // Minimum extents of the bounding box
        public Point BB_p1_SCREEN { get; set; }

        // Minimum extents of the bounding box
        public Point BB_p2_SCREEN { get; set; }


        /// <summary>
        /// Properties for binding into the model data
        /// </summary>
        public double Area_Untr { get => Model.Area_Untransformed; }
        public double Weight { get => Model.Weight; }
        public double Ix_Untr { get => Model.Ix_Untr; }
        public double Iy_Untr { get => Model.Iy_Untr; }

        public double Centroid_X_Untr { get => Model.Centroid_Untransformed.X; }
        public double Centroid_Y_Untr { get => Model.Centroid_Untransformed.Y; }






        private double CANVAS_SCALE { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        public FlitchPlateViewModel(FlitchPlateModel model, Canvas c)
        {
            Model = model;
            FPCanvas = c;

            // Find drawing parameters
            FindBoundingBox_Untransformed_World();
            ComputeScaleFactor();
            FindBoundingBox_Untransformed_Screen();
        }

        /// <summary>
        /// Draws the plates to the canvas
        /// </summary>
        /// <param name="c"></param>
        public void Draw()
        {
            // clear the canvas
            FPCanvas.Children.Clear();



            // Draw the plates
            foreach (PlateModel p in Model.Plates)
            {
                double width_on_screen = p.Width * CANVAS_SCALE;
                double height_on_screen = p.Height * CANVAS_SCALE;
                double delta_x = p.Centroid.X * CANVAS_SCALE;
                double delta_y = p.Centroid.Y * CANVAS_SCALE;
                double xa1 = FPCanvas.Width * 0.5 + delta_x;
                double ya1 = FPCanvas.Height * 0.5 - delta_y;
                double set_left = xa1 - 0.5 * width_on_screen;
                double set_top = ya1 - 0.5 * height_on_screen;

                Rectangle rect = new Rectangle()
                {
                    Width = width_on_screen,
                    Height = height_on_screen,
                    Fill = GetMaterialColor(p.Material.MaterialType),
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                FPCanvas.Children.Add(rect);
                Canvas.SetTop(rect, set_top);
                Canvas.SetLeft(rect, set_left);
            }

            double default_width1 = 20;
            double width_on_screen1 = default_width1;
            double height_on_screen1 = default_width1;
            double delta_x1 = Centroid_X_Untr * CANVAS_SCALE;
            double delta_y1 = Centroid_Y_Untr * CANVAS_SCALE;
            double xa = FPCanvas.Width * 0.5 + delta_x1;
            double ya = FPCanvas.Height * 0.5 - delta_y1;
            double set_left1 = xa - 0.5 * width_on_screen1;
            double set_top1 = ya - 0.5 * height_on_screen1;

            // Draw the Centroid Point
            Ellipse circle = new Ellipse()
            {
                Width = width_on_screen1,
                Height = height_on_screen1,
                Fill = Brushes.Black,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            FPCanvas.Children.Add(circle);
            Canvas.SetTop(circle, set_top1);
            Canvas.SetLeft(circle, set_left1);


            //// draw the bounding box
            //Line line = new Line();
            //line.Visibility = System.Windows.Visibility.Visible;
            //line.StrokeThickness = 4;
            //line.Stroke = System.Windows.Media.Brushes.Blue;
            //line.X1 = BB_p1_SCREEN.X;
            //line.Y1 = BB_p1_SCREEN.Y;

            //line.X2 = BB_p2_SCREEN.X;
            //line.Y2 = BB_p2_SCREEN.Y;
            //FPCanvas.Children.Add(line);
        }

        /// <summary>
        /// Computes the scale factor for a drawing canvas
        /// </summary>
        /// <param name="c"></param>
        private void ComputeScaleFactor()
        {
            // determine the scale factor
            double scale_x = 0.5 * FPCanvas.Width / (BB_p2_WORLD.X - BB_p1_WORLD.X);
            double scale_y = 0.5 * FPCanvas.Height / (BB_p2_WORLD.Y - BB_p1_WORLD.Y);

            CANVAS_SCALE = scale_x < scale_y ? scale_x : scale_y;
        }

        /// <summary>
        /// Finds the color to draw the material fill
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        private Brush GetMaterialColor(MaterialTypes type)
        {
            switch (type)
            {
                case MaterialTypes.MATERIAL_UNDEFINED:
                    {
                        throw new System.ArgumentException("In GetMaterialColor: " + type.ToString() + " error");
                    }
                case MaterialTypes.MATERIAL_STEEL:
                    {
                        return Brushes.Red;
                    }
                case MaterialTypes.MATERIAL_WOOD_SYP:
                    return Brushes.Blue;
                case MaterialTypes.MATERIAL_WOOD_DF:
                    return Brushes.Blue;
                case MaterialTypes.MATERIAL_WOOD_LVL_E2_0:
                    {
                        return Brushes.Green;
                    }
                default:
                    throw new System.ArgumentException("In GetMaterialColor: " + type.ToString() + " error");
            }
        }

        /// <summary>
        /// Find the extreme limits of the objects in world space
        /// </summary>
        private void FindBoundingBox_Untransformed_World()
        {
            double min_x = 0;
            double min_y = 0;
            double max_x = 0;
            double max_y = 0;

            foreach (PlateModel plate in Model.Plates)
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

        /// <summary>
        /// Find the corresponding limits of the world bounding box on the canvas screen
        /// </summary>
        /// <param name="c"></param>
        private void FindBoundingBox_Untransformed_Screen()
        {
            double p1_x = BB_p1_WORLD.X * CANVAS_SCALE + 0.5 * FPCanvas.Width;
            double p1_y = 0.5 * FPCanvas.Height - BB_p1_WORLD.Y * CANVAS_SCALE;

            double p2_x = BB_p2_WORLD.X * CANVAS_SCALE + 0.5 * FPCanvas.Width;
            double p2_y = 0.5 * FPCanvas.Height - BB_p2_WORLD.Y * CANVAS_SCALE;

            BB_p1_SCREEN = new Point(p1_x, p1_y);
            BB_p2_SCREEN = new Point(p2_x, p2_y);
        }

        public void Update()
        {
            Model.UpdateCalculations();
            
            OnPropertyChanged("Area");
            OnPropertyChanged("Weight");
            OnPropertyChanged("Ix_Untr");
            OnPropertyChanged("Iy_Untr");
            OnPropertyChanged("Centroid_X_Untr");
            OnPropertyChanged("Centroid_Y_Untr");


        }
    }
}
