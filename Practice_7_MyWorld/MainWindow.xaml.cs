using System;
using System.Collections.Generic;
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

namespace Practice_7_MyWorld
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private World world;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            world.StartSimulation();
            StartMenuButton.IsEnabled = false;
            StopMenuButton.IsEnabled = true;
        }
        private void StopClick(object sender, RoutedEventArgs e)
        {
            world.StopSimulation();
            StartMenuButton.IsEnabled = true;
            StopMenuButton.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            world = new World(MainField);
        }

        private void SimulationSpeedClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem button = (MenuItem)sender;
                string str = (string)button.Header;
                int speed = int.Parse(str.Remove(str.Length - 1));
                world.SpeedMultiplier = speed;
            }
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in SpeedSimMenu.Items)
            {
                if (item is MenuItem && sender is MenuItem && item != ((MenuItem)sender))
                {
                    ((MenuItem)item).IsChecked = false;
                }
            }
        }

        private void MonitorExtensionClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem button = (MenuItem)sender;
                string[] str = ((string)button.Header).Split(new char[] { 'x' });
                Height = double.Parse(str[0]);
                Width = double.Parse(str[1]);

                world.StopSimulation();

                MainField.Children.Clear();
                world = new World(MainField);
                

                StartMenuButton.IsEnabled = true;
                StopMenuButton.IsEnabled = false;
            }
        }
    }
}
