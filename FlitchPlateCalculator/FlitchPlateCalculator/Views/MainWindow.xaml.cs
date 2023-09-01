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
        /// <summary>
        /// Sets the current id and updates the numbering
        /// </summary>
        /// <returns></returns>
        public int GetNextId()
        {
            int test_number = 0;
            while (true)
            {
                bool match_found = false;
                for (int i = 0; i < FlitchPlateVM.Model.Plates.Count; i++)
                {
                    if (FlitchPlateVM.Model.Plates[i].Id == test_number)
                    {
                        // found a match with this id number
                        match_found = true;
                        continue;
                    } 
                }

                if(match_found)
                {
                    test_number++;
                    continue;
                } else
                {
                    return test_number;
                }
            }
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
            //LoadW8x31Test();
            //LoadMultiMaterialTest();

            // Run the first time
            OnUserCreate();

            // Update the model values
            OnUserUpdate();

            // Sets the Data context for the data bindings
            DataContext = FlitchPlateVM;
        }

        /// <summary>
        /// Function that fires only once when the application is created.
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
            // Update the Flitch Plate View Model
            FlitchPlateVM.Update();

            // Updates the status message and highlighting to be displayed
            UpdateStatusMessage();

            // If the FlitchPlate model is not valid, hide the results stack panel
            if (FlitchPlateVM.Model.IsValidModel is true)
            {
                spResults.Visibility = Visibility.Visible;
            }
            else
            {
                spResults.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Updates the status message and performs several checks
        /// </summary>
        private void UpdateStatusMessage()
        {
            bool bHasErrors = false;
            string str = "Assembly has " + FlitchPlateVM.Model.Plates.Count + " plates";

            // changed the background of the canvas
            if (FlitchPlateVM.Model.ValidateMaterialType() == false)
            {
                str += "\nOne or more materials are undefined";
                bHasErrors = true;
            }

            // check the overlap of the model
            FlitchPlateVM.Model.ValidateOverlap();
            if (FlitchPlateVM.Model.ocOverlappedPlate.Count > 0)
            {
                for (int i = 0; i < FlitchPlateVM.Model.ocOverlappedPlate.Count; i++)
                {
                    str += "\nPlates " + FlitchPlateVM.Model.ocOverlappedPlate[i].p1_id.ToString() + " and " + FlitchPlateVM.Model.ocOverlappedPlate[i].p2_id.ToString() + " are overlapping";
                    bHasErrors = true;
                }
            }

            // change the UI colors
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

            // update the status message
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
                ((PlateElementControl)uc).Update();

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

            FlitchPlateVM.Update();

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
        }

        /// <summary>
        /// Event to update a plate model after it has been edited in the plate control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePlateModel(object sender, RoutedEventArgs e)
        {
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

                    // and set up the events to handle recalculations
                    AddPlateControlEvents(pce);

                    break;
                }
            }

            // recreate the view model
            RecreateViewModel(sender, e);
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

            // create a copy of this plate
            PlateModel new_model = new PlateModel(GetNextId(), copy_model.Width, copy_model.Height, copy_model.Centroid, copy_model.Material.MaterialType);

            // Create the new control
            UserControl uc = new PlateElementControl(new_model);

            // Add the events
            AddPlateControlEvents(uc);

            // Add to the UI stackpanel
            spSteelControls.Children.Add(uc);

            // recreate the view model and render it
            RecreateViewModel(sender, e);

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
                    FlitchPlateVM.Update();
                }
            }

            ((PlateElementControl)sender).Update();

            //// recreate the view model and render it
            RecreateViewModel(sender, e);
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

            // Update the UI
            OnUserUpdate();
        }

        #endregion



        protected void LoadW8x31Test()
        {
            FlitchPlateModel model = new FlitchPlateModel();

            PlateModel p1 = new PlateModel(GetNextId(), 8, 0.435, new Point(0, +3.7825), MaterialTypes.MATERIAL_STEEL) ;
            FlitchPlateVM.Model.AddPlate(p1);

            PlateModel p2 = new PlateModel(GetNextId(), 8, 0.435, new Point(0, -3.7825), MaterialTypes.MATERIAL_STEEL);
            FlitchPlateVM.Model.AddPlate(p2);

            PlateModel p3 = new PlateModel(GetNextId(), 0.285, 7.12, new Point(0, 0), MaterialTypes.MATERIAL_STEEL);
            FlitchPlateVM.Model.AddPlate(p3);
        }

        /// <summary>
        /// Test case for multiple materials
        /// </summary>
        /// <returns></returns>
        protected void LoadMultiMaterialTest()
        {
            FlitchPlateModel model = new FlitchPlateModel();

            PlateModel p1 = new PlateModel(GetNextId(), 0.5, 2, new Point(-0.76, 0), MaterialTypes.MATERIAL_STEEL);
            FlitchPlateVM.Model.AddPlate(p1);

            PlateModel p2 = new PlateModel(GetNextId(), 0.5, 4, new Point(-0.255, 0), MaterialTypes.MATERIAL_WOOD_LVL_E2_0);
            FlitchPlateVM.Model.AddPlate(p2);

            PlateModel p3 = new PlateModel(GetNextId(), 0.5, 4, new Point(+0.255, 0), MaterialTypes.MATERIAL_WOOD_LVL_E2_0);
            FlitchPlateVM.Model.AddPlate(p3);

            PlateModel p4 = new PlateModel(GetNextId(), 0.5, 2, new Point(+0.76, 0), MaterialTypes.MATERIAL_STEEL);
            FlitchPlateVM.Model.AddPlate(p4);

            PlateModel p5 = new PlateModel(GetNextId(), 4, .25, new Point(0, 2.13), MaterialTypes.MATERIAL_WOOD_SYP);
            FlitchPlateVM.Model.AddPlate(p5);

            OnUserUpdate();
        }


    }
}
