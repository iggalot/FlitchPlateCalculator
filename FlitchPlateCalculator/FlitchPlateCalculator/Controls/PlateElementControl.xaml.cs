using FlitchPlateCalculator.Models;
using FlitchPlateCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlitchPlateCalculator.Controls
{
    /// <summary>
    /// Interaction logic for PlateElementControl.xaml
    /// </summary>
    public partial class PlateElementControl : UserControl, INotifyPropertyChanged
    {
        private static int _id = 0;

        public int ID { get; set; }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // the current model information
        public PlateModel Model { get; set; }

        // stores the OldModel information
        public PlateModel OldModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Qty { get => 1; }

        public MaterialTypes PlateMaterialType { get => Model.Material.MaterialType; }

        public double Centroid_X { get => Model.Centroid.X; }
        public double Centroid_Y { get => Model.Centroid.Y; }

        public double PlateWidth { get => Model.Width; }
        public double PlateHeight { get => Model.Height; }


        ObservableCollection<string> ocSteelGrades = new ObservableCollection<string> { "A36", "A992" };
        ObservableCollection<string> ocSteelThickness = new ObservableCollection<string> { "PL 1/8", "PL 3/16", "PL 1/4", "PL 5/16", "PL 3/8", "PL 7/16", "PL 1/2", "PL 5/8", "PL 3/4", "PL 7/8", "PL 1", "PL 1-1/4", "PL 1-1/2", "PL 2" };
        ObservableCollection<string> ocWoodGrades = new ObservableCollection<string> { "SYP #2", "DF", "LVL E2.0" };
        ObservableCollection<string> ocWoodNominalSize = new ObservableCollection<string> { "2x4", "2x6", "2x8", "2x10", "2x12" };
        ObservableCollection<string> ocWoodLVLSize = new ObservableCollection<string> { "9.5", "11.875", "14", "16", "18.75", "23.875" };

        ObservableCollection<int> ocSteelQty = new ObservableCollection<int> { 1, 2, 3, 4 };
        ObservableCollection<int> ocWoodQty = new ObservableCollection<int> { 1, 2, 3, 4 };

        public PlateElementControl(PlateModel model)
        {
            // set and ID marker for the control
            ID = _id;
            _id++;

            InitializeComponent();

            Model = model;
            Model.Id = ID;

            OldModel = model;
            OldModel.Id = ID;

            DataContext = this;

            OnCreate();
        }

        /// <summary>
        /// Event for when the window finishes loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlateControlLoaded(object sender, RoutedEventArgs e)
        {
            Update(); // run the calculations for the current data set
        }

        /// <summary>
        /// Creates the UI and sets up the combo boxes
        /// </summary>
        public void OnCreate()
        {
            cmbGrade.ItemsSource = Enum.GetValues(typeof(MaterialTypes));
            cmbGrade.SelectedItem = cmbGrade.Items[(int)Model.Material.MaterialType];
            cmbGrade.FontSize = 12;
            cmbGrade.FontWeight = FontWeights.Bold;
            cmbGrade.HorizontalAlignment = HorizontalAlignment.Center;
            cmbGrade.VerticalAlignment = VerticalAlignment.Top;

            ApplyUserWarnings();
        }

        private void ApplyUserWarnings()
        {
            if(Model.Material.MaterialType == MaterialTypes.MATERIAL_UNDEFINED)
            {
                spMainPlate.Background = Brushes.LightSalmon;
                spMainPlate.Opacity = 0.75;
            } else
            {
                spMainPlate.Background = Brushes.AntiqueWhite;
                spMainPlate.Opacity = 0.9;
            }
        }

        /// <summary>
        /// Read the form data, parse it, and create a new model for the data.
        /// </summary>
        public void Update()
        {
            OnPropertyChanged("Qty");
            OnPropertyChanged("Centroid_X");
            OnPropertyChanged("Centroid_Y");
            OnPropertyChanged("PlateMaterialType");
            OnPropertyChanged("PlateWidth");
            OnPropertyChanged("PlateHeight");

            // Raise the routed event to notify the parent that something has changed
            RaiseEvent(new RoutedEventArgs(PlateElementControl.OnControlModifiedEvent));
        }


        // create an event to notify the parent that something has changed
        public static readonly RoutedEvent OnControlModifiedEvent = EventManager.RegisterRoutedEvent("UpdateModelRequired", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlateElementControl));
        public static readonly RoutedEvent OnRemovePlateControlEvent = EventManager.RegisterRoutedEvent("DeletePlateControl", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlateElementControl));
        public static readonly RoutedEvent OnPlateModelChangedEvent = EventManager.RegisterRoutedEvent("PlateModelChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlateElementControl));
        public static readonly RoutedEvent OnCopyPlateModelEvent = EventManager.RegisterRoutedEvent("CopyPlateControl", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PlateElementControl));


        // expose our event
        public event RoutedEventHandler OnRemovePlateControl
        {
            add { AddHandler(OnRemovePlateControlEvent, value); }
            remove
            {
                RemoveHandler(OnRemovePlateControlEvent, value);
            }
        }


        // expose our event
        public event RoutedEventHandler OnControlModified
        {
            add { AddHandler(OnControlModifiedEvent, value); }
            remove
            {
                RemoveHandler(OnControlModifiedEvent, value);
            }
        }

        // expose our event
        public event RoutedEventHandler OnPlateModelChanged
        {
            add { AddHandler(OnPlateModelChangedEvent, value); }
            remove
            {
                RemoveHandler(OnPlateModelChangedEvent, value);
            }
        }

        // expose our event
        public event RoutedEventHandler OnCopyPlateModel
        {
            add { AddHandler(OnCopyPlateModelEvent, value); }
            remove
            {
                RemoveHandler(OnCopyPlateModelEvent, value);
            }
        }


        private void Button_RotateClick(object sender, RoutedEventArgs e)
        {
            Model.RotatePlate();
            Update();
        }

        private void Button_DeletePlateClick(object sender, RoutedEventArgs e)
        {
            // Raise the event to signal the plate removal
            RaiseEvent(new RoutedEventArgs(PlateElementControl.OnRemovePlateControlEvent));
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            btnSave.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;
            btnEdit.Visibility = Visibility.Visible;
            btnRotate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnCopy.IsEnabled = true;


            tblk_Grade.Visibility = Visibility.Visible;
            cmbGrade.Visibility = Visibility.Collapsed;

            // Toggle the fields to edit mode
            tblk_Width.Visibility = Visibility.Visible;
            tb_Width.Visibility = Visibility.Collapsed;

            tblk_Height.Visibility = Visibility.Visible;
            tb_Height.Visibility = Visibility.Collapsed;

            tblk_Xi.Visibility = Visibility.Visible;
            tb_Xi.Visibility = Visibility.Collapsed;

            tblk_Yi.Visibility = Visibility.Visible;
            tb_Yi.Visibility = Visibility.Collapsed;

            // reload the current settings
            Update();
        }


        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            // Raise the event to copy the control
            RaiseEvent(new RoutedEventArgs(PlateElementControl.OnCopyPlateModelEvent));



        }
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            spMainPlate.Opacity = 1.0;

            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;

            btnEdit.Visibility = Visibility.Collapsed;
            btnRotate.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnCopy.IsEnabled = false;


            tblk_Grade.Visibility = Visibility.Collapsed;
            cmbGrade.Visibility = Visibility.Visible;

            // Toggle the fields to edit mode
            tblk_Width.Visibility = Visibility.Collapsed;
            tb_Width.Visibility = Visibility.Visible;

            tblk_Height.Visibility = Visibility.Collapsed;
            tb_Height.Visibility = Visibility.Visible;

            tblk_Xi.Visibility = Visibility.Collapsed;
            tb_Xi.Visibility = Visibility.Visible;

            tblk_Yi.Visibility = Visibility.Collapsed;
            tb_Yi.Visibility = Visibility.Visible;


        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            PlateModel model;
            if(ValidateInput(out model) is true)
            {
                btnSave.Visibility = Visibility.Collapsed;
                btnCancel.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Visible;
                btnRotate.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnCopy.IsEnabled = true;


                tblk_Grade.Visibility = Visibility.Visible;
                cmbGrade.Visibility = Visibility.Collapsed;

                // Toggle the fields to edit mode
                tblk_Width.Visibility = Visibility.Visible;
                tb_Width.Visibility = Visibility.Collapsed;

                tblk_Height.Visibility = Visibility.Visible;
                tb_Height.Visibility = Visibility.Collapsed;

                tblk_Xi.Visibility = Visibility.Visible;
                tb_Xi.Visibility = Visibility.Collapsed;

                tblk_Yi.Visibility = Visibility.Visible;
                tb_Yi.Visibility = Visibility.Collapsed;

                OldModel = Model;
                Model = model;

                RaiseEvent(new RoutedEventArgs(PlateElementControl.OnPlateModelChangedEvent));

            };

            Update();
        }

        /// <summary>
        /// Validates user input and returns a PlateModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool ValidateInput(out PlateModel model)
        {
            bool isValid = true;

            double width = 0, height = 0, xi = 0, yi = 0;
            MaterialTypes material_type = MaterialTypes.MATERIAL_UNDEFINED;

            double result;
            if(double.TryParse(tb_Width.Text, out result) is false)
            {
                isValid = false;                
            } else
            {
                width = result;
            }

            if (double.TryParse(tb_Height.Text, out result) is false)
            {
                isValid = false;
            }
            else
            {
                height = result;
            }

            if (double.TryParse(tb_Xi.Text, out result) is false)
            {
                isValid = false;
            }
            else
            {
                xi = result;
            }

            if (double.TryParse(tb_Yi.Text, out result) is false)
            {
                isValid = false;
            }
            else
            {
                yi = result;
            }

            MaterialTypes mat_result;
            if(Enum.TryParse<MaterialTypes>(cmbGrade.SelectedItem.ToString(), out mat_result) is false)
            {
                isValid = false;
            }
            else
            {
                material_type = mat_result;
            }

            if(isValid is true)
            {
                model = new PlateModel(width, height, new Point(xi, yi), material_type);
            } else
            {
                model = Model;
            }

            return isValid;
        }
    }
}
