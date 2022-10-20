using FlitchPlateCalculator.Models;
using FlitchPlateCalculator.ViewModels;
using System;
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
        public FlitchPlateViewModel FlitchPlateVM { get; set; }
        public FlitchPlateModel Model { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Model = new FlitchPlateModel();

            DataContext = this;


            // Test case
            PlateModel p1 = new PlateModel(0.5, 2, new Point(-0.75, 0), MaterialTypes.MATERIAL_STEEL);
            Model.AddPlate(p1);

            PlateModel p2 = new PlateModel(0.5, 4, new Point(-0.25, 0), MaterialTypes.MATERIAL_WOOD_LVL_E2_0);
            Model.AddPlate(p2);

            PlateModel p3 = new PlateModel(0.5, 4, new Point(+0.25, 0), MaterialTypes.MATERIAL_WOOD_LVL_E2_0);
            Model.AddPlate(p3);

            PlateModel p4 = new PlateModel(0.5, 2, new Point(+0.75, 0), MaterialTypes.MATERIAL_STEEL);
            Model.AddPlate(p4);

            PlateModel p5 = new PlateModel(4, .25, new Point(0, 2.125), MaterialTypes.MATERIAL_WOOD_SYP);
            Model.AddPlate(p5);

            Model.UpdateCalculations();

            MessageBox.Show(Model.ToString());

            // Create the view model
            FlitchPlateVM = new FlitchPlateViewModel(Model, MainCanvas);
            FlitchPlateVM.Draw();
        }


    }
}
