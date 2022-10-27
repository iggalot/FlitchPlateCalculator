using FlitchPlateCalculator.Controls;
using FlitchPlateCalculator.Models;
using FlitchPlateCalculator.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static FlitchPlateCalculator.Utilities.GeometryHelpers;

namespace FlitchPlateCalculator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int _id = 0;

        /// <summary>
        /// The current id value
        /// </summary>
        public int CurrentId
        {
            get => _id;
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Sets the current id and updates the numbering
        /// </summary>
        /// <returns></returns>
        public int GetNextId()
        {
            _id++;
            CurrentId = _id;
            return CurrentId; ;
        }

        private bool bWindowFinishedLoading = false;

        public FlitchPlateViewModel FlitchPlateVM { get; set; }

        ObservableCollection<string> ocSteelGrades = new ObservableCollection<string> { "A36", "A992" };
        ObservableCollection<string> ocSteelThickness = new ObservableCollection<string> { "PL 1/8", "PL 3/16", "PL 1/4", "PL 5/16", "PL 3/8", "PL 7/16", "PL 1/2", "PL 5/8", "PL 3/4", "PL 7/8", "PL 1", "PL 1-1/4", "PL 1-1/2", "PL 2" };
        ObservableCollection<string> ocWoodGrades = new ObservableCollection<string> { "SYP #2", "DF", "LVL E2.0" };
        ObservableCollection<string> ocWoodNominalSize = new ObservableCollection<string> { "2x4", "2x6", "2x8", "2x10", "2x12" };
        ObservableCollection<string> ocWoodLVLSize = new ObservableCollection<string> { "9.5", "11.875", "14", "16", "18.75","23.875" };

        ObservableCollection<int> ocSteelQty = new ObservableCollection<int> { 1, 2, 3, 4 };
        ObservableCollection<int> ocWoodQty = new ObservableCollection<int> { 1, 2, 3, 4 };

        public MainWindow()
        {
            InitializeComponent();

            // Create the view model and set the DataContext
            FlitchPlateVM = new FlitchPlateViewModel(MainCanvas);

            // Load test models
            //FlitchPlateVM.Model = LoadW8x31Test();
            //FlitchPlateVM.Model = LoadMultiMaterialTest();

            // Run the first time
            OnUserCreate();

            // Update the model values
            OnUserUpdate();

            // Sets the Data context for the data bindings
            DataContext = FlitchPlateVM;
        }

        /// <summary>
        /// Function that first only once when the application is created.
        /// </summary>
        private void OnUserCreate()
        {
            CreateUI(this);
        }

        /// <summary>
        /// Event for when the window finishes its first loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            bWindowFinishedLoading = true;
        }

        /// <summary>
        /// Utility function that draws every time an update is needed.
        /// </summary>
        private void OnUserUpdate()
        {
            // Updates the status message
            UpdateStatusMessage();

            // Update the Flitch Plate View Model
            FlitchPlateVM.Update();
        }

        /// <summary>
        /// Updates the status message and performs several checks
        /// </summary>
        private void UpdateStatusMessage()
        {
            string str = "Assembly has " + FlitchPlateVM.Model.Plates.Count + " plates";
            bool bHasErrors = false;

            // Does the model have any undefined materials?
            bool materials_defined = true;
            foreach (PlateModel plate in FlitchPlateVM.Model.Plates)
            {
                if (plate.Material.MaterialType == MaterialTypes.MATERIAL_UNDEFINED)
                {
                    materials_defined = false;
                    bHasErrors = true;
                }
            }

            // changed the background of the canvas
            if (materials_defined == false)
            {
                str += "\nOne or more materials are undefined";
            }

            //// Check for overlapping plates
            //for (int i = 0; i < FlitchPlateVM.Model.Plates.Count; i++)
            //{
            //    // is plate 'i' x-corner between plate 'j' x-corner?
            //    double i_x_left = Math.Round(FlitchPlateVM.Model.Plates[i].Centroid.X - FlitchPlateVM.Model.Plates[i].Width * 0.5, 3);
            //    double i_x_right = Math.Round(FlitchPlateVM.Model.Plates[i].Centroid.X + FlitchPlateVM.Model.Plates[i].Width * 0.5, 3);
            //    double i_y_top = Math.Round(FlitchPlateVM.Model.Plates[i].Centroid.Y + FlitchPlateVM.Model.Plates[i].Height * 0.5, 3);
            //    double i_y_bot = Math.Round(FlitchPlateVM.Model.Plates[i].Centroid.Y - FlitchPlateVM.Model.Plates[i].Height * 0.5, 3);

            //    for (int j = 0; j < FlitchPlateVM.Model.Plates.Count; j++)
            //    {
            //        // are the two plates the same?  If so skip and continue searching
            //        if (i == j)
            //        {
            //            continue;
            //        }

            //        double j_x_left = Math.Round(FlitchPlateVM.Model.Plates[j].Centroid.X - FlitchPlateVM.Model.Plates[j].Width * 0.5, 3);
            //        double j_x_right = Math.Round(FlitchPlateVM.Model.Plates[j].Centroid.X + FlitchPlateVM.Model.Plates[j].Width * 0.5, 3);
            //        double j_y_top = Math.Round(FlitchPlateVM.Model.Plates[j].Centroid.Y + FlitchPlateVM.Model.Plates[j].Height * 0.5, 3);
            //        double j_y_bot = Math.Round(FlitchPlateVM.Model.Plates[j].Centroid.Y - FlitchPlateVM.Model.Plates[j].Height * 0.5, 3);

            //        // if any of the edge lines intersect then they overlap
            //        //top A top B

            //        bool rectangles_overlap = false;

            //        IntersectPointData intersect_point1 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_top, j_x_left, j_y_top, j_x_right, j_y_top);
            //        if(intersect_point1.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }
            //        //top A right B
            //        IntersectPointData intersect_point2 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_top, j_x_right, j_y_top, j_x_right, j_y_bot);
            //        if (intersect_point2.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //top A bot B
            //        IntersectPointData intersect_point3 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_top, j_x_left, j_y_bot, j_x_right, j_y_bot);
            //        if (intersect_point3.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }
            //        //top A left B
            //        IntersectPointData intersect_point4 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_top, j_x_left, j_y_bot, j_x_left, j_y_bot);
            //        if (intersect_point4.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //right A top B
            //        IntersectPointData intersect_point5 = FindPointOfIntersectLines_2D(i_x_right, i_y_top, i_x_right, i_y_bot, j_x_left, j_y_top, j_x_right, j_y_top);
            //        if (intersect_point5.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }
            //        //right A right B
            //        IntersectPointData intersect_point6 = FindPointOfIntersectLines_2D(i_x_right, i_y_top, i_x_right, i_y_bot, j_x_right, j_y_top, j_x_right, j_y_bot);
            //        if (intersect_point6.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }
            //        //right A bot B
            //        IntersectPointData intersect_point7 = FindPointOfIntersectLines_2D(i_x_right, i_y_top, i_x_right, i_y_bot, j_x_left, j_y_bot, j_x_right, j_y_bot);
            //        if (intersect_point7.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }
            //        //right A left B
            //        IntersectPointData intersect_point8 = FindPointOfIntersectLines_2D(i_x_right, i_y_top, i_x_right, i_y_bot, j_x_left, j_y_bot, j_x_left, j_y_bot);
            //        if (intersect_point8.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //bot A top B
            //        IntersectPointData intersect_point9 = FindPointOfIntersectLines_2D(i_x_left, i_y_bot, i_x_right, i_y_bot, j_x_left, j_y_top, j_x_right, j_y_top);
            //        if (intersect_point9.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //bot A right B
            //        IntersectPointData intersect_point10 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_right, i_y_bot, j_x_right, j_y_top, j_x_right, j_y_bot);
            //        if (intersect_point10.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //bot A bot B
            //        IntersectPointData intersect_point11 = FindPointOfIntersectLines_2D(i_x_left, i_y_bot, i_x_right, i_y_bot, j_x_left, j_y_bot, j_x_right, j_y_bot);
            //        if (intersect_point11.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //bot A left B
            //        IntersectPointData intersect_point12 = FindPointOfIntersectLines_2D(i_x_left, i_y_bot, i_x_right, i_y_bot, j_x_left, j_y_bot, j_x_left, j_y_bot);
            //        if (intersect_point12.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //left A top B
            //        IntersectPointData intersect_point13 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_left, i_y_bot, j_x_left, j_y_top, j_x_right, j_y_top);
            //        if (intersect_point13.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //left A right B
            //        IntersectPointData intersect_point14 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_left, i_y_bot, j_x_right, j_y_top, j_x_right, j_y_bot);
            //        if (intersect_point14.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //left A bot B
            //        IntersectPointData intersect_point15 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_left, i_y_bot, j_x_left, j_y_bot, j_x_right, j_y_bot);
            //        if (intersect_point15.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        //left A left B
            //        IntersectPointData intersect_point16 = FindPointOfIntersectLines_2D(i_x_left, i_y_top, i_x_left, i_y_bot, j_x_left, j_y_bot, j_x_left, j_y_bot);
            //        if (intersect_point16.isWithinSegment == true)
            //        {
            //            rectangles_overlap = true;
            //        }

            //        // if rectangles dont have intersecting edges, need to check case that j plate is completely inside i plate
            //        if(j_x_left >= i_x_left && j_x_right <= i_x_right && j_y_top <= i_y_top && j_y_bot >= i_y_bot )
            //        {
            //            rectangles_overlap = true;
            //        }

            //        // the corner of i is inside the region of j, so it's overlapping
            //        if (rectangles_overlap is true)
            //        {
            //            str += "\nPlates " + FlitchPlateVM.Model.Plates[i].Id.ToString() + " and " + FlitchPlateVM.Model.Plates[j].Id.ToString() + " are overlapping";
            //            bHasErrors = true;
            //            continue;
            //        }
            //    }
            //}


            if (bHasErrors is true){
                tblk_StatusNotes.Foreground = Brushes.Red;
                MainCanvas.Background = Brushes.LightSalmon;
                MainCanvas.Opacity = 0.75;
            } else
            {
                tblk_StatusNotes.Foreground = Brushes.Black;
                MainCanvas.Background = Brushes.LightGray;
                MainCanvas.Opacity = 1.0;
            }

            FlitchPlateVM.StatusMessage = str;
        }

        // Add our events for the plate control
        protected void AddPlateControlEvents(UserControl uc)
        {
            // and set up the events to handle recalculations
            ((PlateElementControl)uc).OnControlModified += RecreateViewModel;
            ((PlateElementControl)uc).OnRemovePlateControl += RemovePlateControl;
            ((PlateElementControl)uc).OnPlateModelChanged += UpdatePlateModel;
            ((PlateElementControl)uc).OnCopyPlateModel += CopyPlateModel;
            ((PlateElementControl)uc).OnRotatePlateModel += RotatePlateModel;

        }

        /// <summary>
        /// Create the UI from the view model
        /// </summary>
        /// <param name="window"></param>
        private void CreateUI(MainWindow window)
        {
            // Add plate element controls to the spSteelControl panel
            // first clear the stack panel for the plate element controls
            spSteelControls.Children.Clear();

            // then recreate each control from the current view model
            for (int i = 0; i < FlitchPlateVM.Model.Plates.Count; i++)
            {
                // create the control
                UserControl uc = new PlateElementControl(FlitchPlateVM.Model.Plates[i]);
                spSteelControls.Children.Add(uc);

                // add the plate element control events
                AddPlateControlEvents(uc);
            }
        }

        #region Routed Events from Plate Element Control

        /// <summary>
        /// Event function that reloads the model when changes are made to the plate model user  control elements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecreateViewModel(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("There are " + spSteelControls.Children.Count + "child elements at this time.");

            // delete the plates list
            FlitchPlateVM.Model.Plates.Clear();
            FlitchPlateVM.Model = new FlitchPlateModel();

            // search the loaded controls and recreate the models for each
            foreach(UserControl uc in spSteelControls.Children)
            {
                // retrieve the model data
                PlateModel model = ((PlateElementControl)uc).Model;
                FlitchPlateVM.Model.AddPlate(model);

            }

            // Notify the system of the changes
            OnUserUpdate();
        }

        /// <summary>
        /// Remove the plate user control from the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemovePlateControl(object sender, RoutedEventArgs e)
        {
            // fetch the model to be removed
            PlateModel remove_model = ((PlateElementControl)sender).Model;

            foreach(UserControl uc in spSteelControls.Children)
            {
                PlateModel test_model = ((PlateElementControl)uc).Model;

                // search for the control in the list and check if it has the same model information
                if (test_model.Equals(remove_model))
                {
                    spSteelControls.Children.Remove(uc);
                    break;
                }
            }

            // recreate the view model
            RecreateViewModel(sender, e);

            OnUserUpdate();
        }

        /// <summary>
        /// Event to update a plate model after it has been edited in the plate control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePlateModel(object sender, RoutedEventArgs e)
        {
            //// get the old model
            //PlateModel remove_model = ((PlateElementControl)sender).OldModel;

            // store the current model from the control
            PlateModel store_model = ((PlateElementControl)sender).Model;

            // search for the old model 
            for (int i = 0; i < spSteelControls.Children.Count; i++)
            {
                PlateModel uc_model = ((PlateElementControl)spSteelControls.Children[i]).Model;
                if (uc_model.Equals(store_model))
                {
                    // get the control and overwrite the model
                    spSteelControls.Children.RemoveAt(i);

                    // create the new control and update its model ID to be the same as before
                    PlateElementControl pce = new PlateElementControl(store_model);
                    spSteelControls.Children.Insert(i, pce);
                    pce.Model.Id = store_model.Id;
                    pce.OldModel.Id = pce.Model.Id;  // need to update the model ID numbers on the old and new models now.

                    // and set up the events to handle recalculations
                    AddPlateControlEvents(pce);

                    break;
                }
            }

            // recreate the view model
            RecreateViewModel(sender, e);

            OnUserUpdate();

        }

        /// <summary>
        /// Event to update a plate model after it has been edited in the plate control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyPlateModel(object sender, RoutedEventArgs e)
        {
            // get the model
            PlateModel copy_model = ((PlateElementControl)sender).Model;

            UserControl uc = new PlateElementControl(copy_model);
            ((PlateElementControl)uc).Model.Id = GetNextId();

            AddPlateControlEvents(uc);
            spSteelControls.Children.Add(uc);

            // recreate the view model and render it
            RecreateViewModel(sender, e);

            OnUserUpdate();
        }

        /// <summary>
        /// Event to update a plate model after it has been edited in the plate control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotatePlateModel(object sender, RoutedEventArgs e)
        {
            // get the model
            PlateModel control_model = ((PlateElementControl)sender).Model;

            for (int i = 0; i < FlitchPlateVM.Model.Plates.Count; i++)
            {
                if (control_model.Equals(FlitchPlateVM.Model.Plates[i]))
                {
                    FlitchPlateVM.Model.RotatePlate(FlitchPlateVM.Model.Plates[i]);
                }
            }

            //// recreate the view model and render it
            //RecreateViewModel(sender, e);

            OnUserUpdate();

        }

        #endregion

        #region Main Window UI Events

        /// <summary>
        /// Event that triggers when a combo box selection has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bWindowFinishedLoading)
            {
                OnUserUpdate();
                return;
            }
        }

        private void Button_NewPlateClick(object sender, RoutedEventArgs e)
        {
            // create a new arbitrary plate
            PlateModel new_model = new PlateModel(GetNextId(), 10, 1, new Point(0, 0), MaterialTypes.MATERIAL_UNDEFINED);
            FlitchPlateVM.Model.AddPlate(new_model);

            // add the control to the layout
            UserControl uc = new PlateElementControl(new_model);
            AddPlateControlEvents(uc);
            spSteelControls.Children.Add(uc);

            OnUserUpdate();
        }

        #endregion



        protected FlitchPlateModel LoadW8x31Test()
        {
            FlitchPlateModel model = new FlitchPlateModel();

            PlateModel p1 = new PlateModel(GetNextId(), 8, 0.435, new Point(0, +3.7825), MaterialTypes.MATERIAL_STEEL) ;
            model.AddPlate(p1);

            PlateModel p2 = new PlateModel(GetNextId(), 8, 0.435, new Point(0, -3.7825), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p2);

            PlateModel p3 = new PlateModel(GetNextId(), 0.285, 7.12, new Point(0, 0), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p3);

            return model;
        }

        /// <summary>
        /// Test case for multiple materials
        /// </summary>
        /// <returns></returns>
        protected FlitchPlateModel LoadMultiMaterialTest()
        {
            FlitchPlateModel model = new FlitchPlateModel();

            PlateModel p1 = new PlateModel(GetNextId(), 0.5, 2, new Point(-0.75, 0), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p1);

            PlateModel p2 = new PlateModel(GetNextId(), 0.5, 4, new Point(-0.25, 0), MaterialTypes.MATERIAL_WOOD_LVL_E2_0);
            model.AddPlate(p2);

            PlateModel p3 = new PlateModel(GetNextId(), 0.5, 4, new Point(+0.25, 0), MaterialTypes.MATERIAL_WOOD_LVL_E2_0);
            model.AddPlate(p3);

            PlateModel p4 = new PlateModel(GetNextId(), 0.5, 2, new Point(+0.75, 0), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p4);

            PlateModel p5 = new PlateModel(GetNextId(), 4, .25, new Point(0, 2.124), MaterialTypes.MATERIAL_WOOD_SYP);
            model.AddPlate(p5);

            return model;
        }


    }
}
