using FlitchPlateCalculator.Models;
using FlitchPlateCalculator.ViewModels;
using System;
using System.Collections.Generic;
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


        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public PlateModel Model { get; set; }

        private bool bPlateControlFinishedLoading = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Qty { get => 1; }

        public MaterialTypes PlateMaterialType { get => Model.Material.MaterialType; }

        public double Centroid_X { get => Model.Centroid.X; }
        public double Centroid_Y { get => Model.Centroid.Y; }

        public double PlateWidth { get => Model.Width; }
        public double PlateHeight { get => Model.Height; }


        public PlateElementControl(PlateModel model)
        {
            InitializeComponent();

            Model = model;


            DataContext = this;
        }

        /// <summary>
        /// Event for when the window finishes loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlateControlLoaded(object sender, RoutedEventArgs e)
        {
            bPlateControlFinishedLoading = true;
            Update(); // run the calculations for the current data set
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

        // expose our event
        public event RoutedEventHandler OnControlModified
        {
            add { AddHandler(OnControlModifiedEvent, value); }
            remove
            {
                RemoveHandler(OnControlModifiedEvent, value);
            }
        }


        private void Button_RotateClick(object sender, RoutedEventArgs e)
        {
            Model.RotatePlate();
            Update();
        }
    }
}
