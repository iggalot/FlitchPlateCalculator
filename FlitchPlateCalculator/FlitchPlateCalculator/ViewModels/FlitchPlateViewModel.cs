using FlitchPlateCalculator.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlitchPlateCalculator.ViewModels
{
    public class FlitchPlateViewModel : BaseVieWModel
    {
        private string _status_message = "";

        /// <summary>
        /// The flith plate model object for this view model
        /// </summary>
        public FlitchPlateModel Model { get; set; }

        /// <summary>
        /// The canvas this flitch plate view model is associated with
        /// </summary>
        public Canvas FPCanvas { get; set; }

        
        #region Bindings into Model
        /// <summary>
        /// Properties for binding into the model data
        /// </summary>
        public double Area_Untr { get => Model.Area_Untransformed; }
        public double Weight { get => Model.Weight; }
        public double Ix_Untr { get => Model.Ix_Untr; }
        public double Iy_Untr { get => Model.Iy_Untr; }

        public double Zx_Untr { get => Model.Zx_Untr; }
        public double Zy_Untr { get => Model.Zy_Untr; }

        public double rx_Untr { get => Model.rx_Untr; }
        public double ry_Untr { get => Model.ry_Untr; }
        public double Sx_Top_Untr { get => Model.Sx_Top_Untr; }
        public double Sx_Bot_Untr { get => Model.Sx_Bot_Untr; }
        public double Sy_Left_Untr { get => Model.Sy_Left_Untr; }
        public double Sy_Right_Untr { get => Model.Sy_Right_Untr; }
        public double Centroid_X_Untr { get => Model.Centroid_Untransformed.X; }
        public double Centroid_Y_Untr { get => Model.Centroid_Untransformed.Y; }
        public string StatusMessage
        {
            get => _status_message;
            set
            {
                _status_message = value;
                OnPropertyChanged("StatusMessage");
            }
        }




        // Drawing limits
        public double HOR_TOP { get => Math.Round(Model.BB_p2_WORLD.Y, 3); }
        public double HOR_BOTTOM { get => Math.Round(Model.BB_p1_WORLD.Y, 3); }
        public double VER_LEFT { get => Math.Round(Model.BB_p1_WORLD.X, 3); }
        public double VER_RIGHT { get => Math.Round(Model.BB_p2_WORLD.X, 3); }

        #endregion

        public Point BB_p1_SCREEN { get; set; }

        // Minimum extents of the bounding box
        public Point BB_p2_SCREEN { get; set; }

        // Canvas scale
        private double CANVAS_SCALE { get; set; }

        #region Constructors
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="c"></param>
        public FlitchPlateViewModel(Canvas c)
        {
            Model = new FlitchPlateModel();
            FPCanvas = c;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        public FlitchPlateViewModel(FlitchPlateModel model, Canvas c)
        {
            Model = model;
            FPCanvas = c;
        }
        #endregion


        /// <summary>
        /// Draws the plates to the canvas
        /// </summary>
        /// <param name="c"></param>
        protected void Draw()
        {
            // clear the canvas
            FPCanvas.Children.Clear();

            // draw mid window lines
            // vertical mid window line
            Line line = new Line();
            line.Visibility = System.Windows.Visibility.Visible;
            line.StrokeThickness = 4;
            line.Stroke = System.Windows.Media.Brushes.DarkGray;
            line.X1 = FPCanvas.Width * 0.5;
            line.Y1 = 0;

            line.X2 = FPCanvas.Width * 0.5;
            line.Y2 = FPCanvas.Height;
            FPCanvas.Children.Add(line);

            // horizontal mid window line
            line = new Line();
            line.Visibility = System.Windows.Visibility.Visible;
            line.StrokeThickness = 4;
            line.Stroke = System.Windows.Media.Brushes.DarkGray;
            line.X1 = 0;
            line.Y1 = FPCanvas.Height * 0.5;

            line.X2 = FPCanvas.Width;
            line.Y2 = FPCanvas.Height * 0.5;
            FPCanvas.Children.Add(line);

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
                    StrokeThickness = 1,
                    Opacity = 0.90
                };

                FPCanvas.Children.Add(rect);
                Canvas.SetTop(rect, set_top);
                Canvas.SetLeft(rect, set_left);
            }

            // Draw composite section centroid
            DrawCentroidMarker(8);

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

        protected void DrawCentroidMarker(double width)
        {
            double default_width1 = width;
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
                Fill = Brushes.Transparent,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            FPCanvas.Children.Add(circle);
            Canvas.SetTop(circle, set_top1);
            Canvas.SetLeft(circle, set_left1);
        }

        /// <summary>
        /// Computes the scale factor for a drawing canvas
        /// </summary>
        /// <param name="c"></param>
        protected void ComputeScaleFactor()
        {
            double scale_x = 0;
            double scale_y = 0;
            // determine the scale factor
            if ((Model.BB_p2_WORLD.X - Model.BB_p1_WORLD.X) == 0)
            {
                scale_x = 1;
            } else
            {
                scale_x = 0.5 * FPCanvas.Width / (Model.BB_p2_WORLD.X - Model.BB_p1_WORLD.X);
            }

            if ((Model.BB_p2_WORLD.Y - Model.BB_p1_WORLD.Y) == 0)
            {
                scale_y = 1;
            } else
            {
                scale_y = 0.5 * FPCanvas.Height / (Model.BB_p2_WORLD.Y - Model.BB_p1_WORLD.Y);
            }

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
                    return Brushes.Cyan;
                case MaterialTypes.MATERIAL_STEEL:
                    return Brushes.Red;
                case MaterialTypes.MATERIAL_WOOD_SYP:
                    return Brushes.Blue;
                case MaterialTypes.MATERIAL_WOOD_DF:
                    return Brushes.Blue;
                case MaterialTypes.MATERIAL_WOOD_LVL_E2_0:
                    return Brushes.Green;
                default:
                    throw new System.ArgumentException("In GetMaterialColor: " + type.ToString() + " error");
            }
        }


        /// <summary>
        /// Find the corresponding limits of the world bounding box on the canvas screen
        /// </summary>
        /// <param name="c"></param>
        protected void FindBoundingBox_Untransformed_Screen()
        {
            double p1_x = Model.BB_p1_WORLD.X * CANVAS_SCALE + 0.5 * FPCanvas.Width;
            double p1_y = 0.5 * FPCanvas.Height - Model.BB_p1_WORLD.Y * CANVAS_SCALE;

            double p2_x = Model.BB_p2_WORLD.X * CANVAS_SCALE + 0.5 * FPCanvas.Width;
            double p2_y = 0.5 * FPCanvas.Height - Model.BB_p2_WORLD.Y * CANVAS_SCALE;

            BB_p1_SCREEN = new Point(p1_x, p1_y);
            BB_p2_SCREEN = new Point(p2_x, p2_y);
        }

        public void Update()
        {
            // If we have plates, update the calculations for the model
            if (Model.Plates.Count > 0)
            {
                // update the model calculations
                Model.UpdateCalculations();

                // update the view factors for the view model
                // Find drawing parameters
                Model.FindBoundingBox_Untransformed_World();
                ComputeScaleFactor();
                FindBoundingBox_Untransformed_Screen();
            }

            // Update the calculation display
            OnPropertyChanged("Area");
            OnPropertyChanged("Weight");
            OnPropertyChanged("Ix_Untr");
            OnPropertyChanged("Iy_Untr");
            OnPropertyChanged("Centroid_X_Untr");
            OnPropertyChanged("Centroid_Y_Untr");
            OnPropertyChanged("Zx_Untr");
            OnPropertyChanged("Zy_Untr");
            OnPropertyChanged("rx_Untr");
            OnPropertyChanged("ry_Untr");
            OnPropertyChanged("Sx_Left_Untr");
            OnPropertyChanged("Sx_Right_Untr");
            OnPropertyChanged("Sy_Top_Untr");
            OnPropertyChanged("Sy_Right_Untr");
            OnPropertyChanged("HOR_TOP");
            OnPropertyChanged("HOR_BOTTOM");
            OnPropertyChanged("VER_LEFT");
            OnPropertyChanged("VER_RIGHT");
            OnPropertyChanged("StatusMessage");

            // redraw the canvas
            Draw();
        }
    }
}
