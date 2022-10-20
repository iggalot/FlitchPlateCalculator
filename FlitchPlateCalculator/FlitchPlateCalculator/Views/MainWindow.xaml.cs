using FlitchPlateCalculator.Controls;
using FlitchPlateCalculator.Models;
using FlitchPlateCalculator.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FlitchPlateCalculator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool bWindowFinishedLoading = false;
        private bool bFirstLoad = true;

        public FlitchPlateViewModel FlitchPlateVM { get; set; }
        public FlitchPlateModel Model { get; set; } = new FlitchPlateModel();

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

            // Load test model
            //Model = W8x31Test();
            Model = MultiMaterialTest();

            tb_UIStatusBar.Text = Model.ToString();

            OnUserCreate();

            DataContext = FlitchPlateVM;

            OnUserUpdate();
        }

        /// <summary>
        /// Function that first only once when the application is created.
        /// </summary>
        private void OnUserCreate()
        {
            CreateUI(this);
            bFirstLoad = true;
        }

        /// <summary>
        /// Event for when the window finishes loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            bWindowFinishedLoading = true;
            Update(); // run the calculations for the current data set
            bFirstLoad = false;
        }

        /// <summary>
        /// Utility function that draws every time an update is needed.
        /// </summary>
        private void OnUserUpdate()
        {
            if (bWindowFinishedLoading || Model.Plates.Count > 0)
            {
                Update();
            }
        }

        /// <summary>
        /// Read the form data, parse it, and create a new model for the data.
        /// </summary>
        public void Update()
        {
            bool modelIsValid = true;

            if(modelIsValid is true)
            {
                FlitchPlateVM = new FlitchPlateViewModel(Model, MainCanvas);
                FlitchPlateVM.Update();
                Model.UpdateCalculations();
                DataContext = FlitchPlateVM;
            }

            //FlitchPlateVM.Update();

            FlitchPlateVM.Draw();
        }

        /// <summary>
        /// Setup the view model for this window
        /// </summary>
        /// <param name="window"></param>
        private void CreateUI(MainWindow window)
        {
            // Create the steel grade drop down box
            foreach (var item in ocSteelGrades)
            {
                ComboBoxItem cbi1 = new ComboBoxItem();
                cbi1.Content = item.ToString();
                window.cmbSteelGrade.Items.Add(cbi1);
            }

            // Set the default to the first item
            window.cmbSteelGrade.SelectedItem = window.cmbSteelGrade.Items[1];
            window.cmbSteelGrade.FontSize = 16;
            window.cmbSteelGrade.FontWeight = FontWeights.Bold;
            window.cmbSteelGrade.HorizontalAlignment = HorizontalAlignment.Center;
            window.cmbSteelGrade.VerticalAlignment = VerticalAlignment.Top;

            // Create the steel thickness drop down box
            foreach (var item in ocSteelQty)
            {
                ComboBoxItem cbi1 = new ComboBoxItem();
                cbi1.Content = item.ToString();
                window.cmbSteelQty.Items.Add(cbi1);
            }

            // Set the default to the first item
            window.cmbSteelQty.SelectedItem = window.cmbSteelQty.Items[0];
            window.cmbSteelQty.FontSize = 16;
            window.cmbSteelQty.FontWeight = FontWeights.Bold;
            window.cmbSteelQty.HorizontalAlignment = HorizontalAlignment.Center;
            window.cmbSteelQty.VerticalAlignment = VerticalAlignment.Top;


            // Create the steel thickness drop down box
            foreach (var item in ocSteelThickness)
            {
                ComboBoxItem cbi1 = new ComboBoxItem();
                cbi1.Content = item.ToString();
                window.cmbSteelThickness.Items.Add(cbi1);
            }

            // Set the default to the first item
            window.cmbSteelThickness.SelectedItem = window.cmbSteelThickness.Items[2];
            window.cmbSteelThickness.FontSize = 16;
            window.cmbSteelThickness.FontWeight = FontWeights.Bold;
            window.cmbSteelThickness.HorizontalAlignment = HorizontalAlignment.Center;
            window.cmbSteelThickness.VerticalAlignment = VerticalAlignment.Top;





            // Add controls to the spSteelControl panel
            for (int i = 0; i < Model.Plates.Count; i++)
            {
                UserControl uc = new PlateElementControl(Model.Plates[i]);
                spSteelControls.Children.Add(uc);

                // and set up the events to handle recalculations
                ((PlateElementControl)uc).OnControlModified += RecreateModel;
            }
            











            // Create the wood grade drop down box
            foreach (var item in ocWoodGrades)
            {
                ComboBoxItem cbi1 = new ComboBoxItem();
                cbi1.Content = item.ToString();
                window.cmbWoodGrade.Items.Add(cbi1);
            }

            // Set the default to the first item
            window.cmbWoodGrade.SelectedItem = window.cmbWoodGrade.Items[2];
            window.cmbWoodGrade.FontSize = 16;
            window.cmbWoodGrade.FontWeight = FontWeights.Bold;
            window.cmbWoodGrade.HorizontalAlignment = HorizontalAlignment.Center;
            window.cmbWoodGrade.VerticalAlignment = VerticalAlignment.Top;

            // Create the steel thickness drop down box
            foreach (var item in ocWoodQty)
            {
                ComboBoxItem cbi1 = new ComboBoxItem();
                cbi1.Content = item.ToString();
                window.cmbWoodQty.Items.Add(cbi1);
            }

            // Set the default to the first item
            window.cmbWoodQty.SelectedItem = window.cmbWoodQty.Items[1];
            window.cmbWoodQty.FontSize = 16;
            window.cmbWoodQty.FontWeight = FontWeights.Bold;
            window.cmbWoodQty.HorizontalAlignment = HorizontalAlignment.Center;
            window.cmbWoodQty.VerticalAlignment = VerticalAlignment.Top;


            //// Create the yield stress drop down
            //foreach (var item in ocSteelYieldStrength)
            //{
            //    ComboBoxItem cbi2 = new ComboBoxItem();
            //    cbi2.Content = item.ToString();
            //    window.cmbSteelYieldStrength.Items.Add(cbi2);
            //}

            //// Set the yield strength default to 60000
            //window.cmbSteelYieldStrength.SelectedItem = window.cmbSteelYieldStrength.Items[1];
            //window.cmbSteelYieldStrength.FontSize = 15;
            //window.cmbSteelYieldStrength.FontWeight = FontWeights.Bold;
            //window.cmbSteelYieldStrength.HorizontalAlignment = HorizontalAlignment.Center;
            //window.cmbSteelYieldStrength.VerticalAlignment = VerticalAlignment.Top;

            //// Create the concrete compressive stress drop down
            //foreach (var item in ocConcreteCompStrength)
            //{
            //    ComboBoxItem cbi3 = new ComboBoxItem();
            //    cbi3.Content = item.ToString();
            //    window.cmbConcreteCompStrength.Items.Add(cbi3);
            //}

            //// Set the concrete compressive strength default to 3000
            //window.cmbConcreteCompStrength.SelectedItem = window.cmbConcreteCompStrength.Items[5];
            //window.cmbConcreteCompStrength.FontSize = 15;
            //window.cmbConcreteCompStrength.FontWeight = FontWeights.Bold;
            //window.cmbConcreteCompStrength.HorizontalAlignment = HorizontalAlignment.Center;
            //window.cmbConcreteCompStrength.VerticalAlignment = VerticalAlignment.Top;

            //// Create the epoxy drop down
            //ComboBoxItem cbi4 = new ComboBoxItem() { Content = "YES" };
            //window.cmbEpoxy.Items.Add(cbi4);
            //cbi4 = new ComboBoxItem() { Content = "NO" };
            //window.cmbEpoxy.Items.Add(cbi4);
            //window.cmbEpoxy.SelectedItem = window.cmbEpoxy.Items[0];

            //// Create the top bars drop down
            //ComboBoxItem cbi5 = new ComboBoxItem() { Content = "YES" };
            //window.cmbTopBars.Items.Add(cbi5);
            //cbi5 = new ComboBoxItem() { Content = "NO" };
            //window.cmbTopBars.Items.Add(cbi5);
            //window.cmbTopBars.SelectedItem = window.cmbTopBars.Items[0];

            //// Create the lightweight drop down
            //ComboBoxItem cbi6 = new ComboBoxItem() { Content = "YES" };
            //window.cmbLightweightConcrete.Items.Add(cbi6);
            //cbi6 = new ComboBoxItem() { Content = "NO" };
            //window.cmbLightweightConcrete.Items.Add(cbi6);
            //window.cmbLightweightConcrete.SelectedItem = window.cmbLightweightConcrete.Items[1];

            //// Create the has minimum transverse reinforcement drop down
            //ComboBoxItem cbi7 = new ComboBoxItem() { Content = "YES" };
            //window.cmbHasMinTransverseReinf.Items.Add(cbi7);
            //cbi7 = new ComboBoxItem() { Content = "NO" };
            //window.cmbHasMinTransverseReinf.Items.Add(cbi7);
            //window.cmbHasMinTransverseReinf.SelectedItem = window.cmbHasMinTransverseReinf.Items[1];

            //// Create the develop bar type (straight, standard hook, ties, etc)
            //foreach (var item in ocDevelopmentLengthTypes)
            //{
            //    ComboBoxItem cbi8 = new ComboBoxItem();
            //    cbi8.Content = item.ToString();
            //    window.cmbDevelopmentBarType.Items.Add(cbi8);
            //}
            //// Set the default bar type to the first item (straight bars)
            ////window.cmbDevelopmentBarType.SelectedItem = window.cmbDevelopmentBarType.Items[0];
            //window.cmbDevelopmentBarType.FontSize = 15;

            //// Create the terminates in column drop down
            //ComboBoxItem cbi9 = new ComboBoxItem() { Content = "YES" };
            //window.cmbTerminatesInColumn.Items.Add(cbi9);
            //cbi9 = new ComboBoxItem() { Content = "NO" };
            //window.cmbTerminatesInColumn.Items.Add(cbi9);
            //window.cmbTerminatesInColumn.SelectedItem = window.cmbTerminatesInColumn.Items[1];

        }

        /// <summary>
        /// Event function that reloads the model when changes are made to the plate model user  control elements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RecreateModel(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("There are " + spSteelControls.Children.Count + "child elements at this time.");

            // delete the plates list
            Model.Plates.Clear();
            Model = new FlitchPlateModel();

            // search the loaded controls and recreate the models for each
            foreach(UserControl uc in spSteelControls.Children)
            {
                // retrieve the model data
                PlateModel model = ((PlateElementControl)uc).Model;
                Model.AddPlate(model);
            }

            // Notify the system of the changes
            OnUserUpdate();
        }

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





        private FlitchPlateModel W8x31Test()
        {
            FlitchPlateModel model = new FlitchPlateModel();

            PlateModel p1 = new PlateModel(8, 0.435, new Point(0, +3.7825), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p1);

            PlateModel p2 = new PlateModel(8, 0.435, new Point(0, -3.7825), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p2);

            PlateModel p3 = new PlateModel(0.285, 7.13, new Point(0, 0), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p3);

            return model;
        }

        /// <summary>
        /// Test case for multiple materials
        /// </summary>
        /// <returns></returns>
        public FlitchPlateModel MultiMaterialTest()
        {
            FlitchPlateModel model = new FlitchPlateModel();

            PlateModel p1 = new PlateModel(0.5, 2, new Point(-0.75, 0), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p1);

            PlateModel p2 = new PlateModel(0.5, 4, new Point(-0.25, 0), MaterialTypes.MATERIAL_WOOD_LVL_E2_0);
            model.AddPlate(p2);

            PlateModel p3 = new PlateModel(0.5, 4, new Point(+0.25, 0), MaterialTypes.MATERIAL_WOOD_LVL_E2_0);
            model.AddPlate(p3);

            PlateModel p4 = new PlateModel(0.5, 2, new Point(+0.75, 0), MaterialTypes.MATERIAL_STEEL);
            model.AddPlate(p4);

            PlateModel p5 = new PlateModel(4, .25, new Point(0, 2.125), MaterialTypes.MATERIAL_WOOD_SYP);
            model.AddPlate(p5);

            return model;
        }


    }
}
