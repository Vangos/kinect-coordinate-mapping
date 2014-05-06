using Microsoft.Kinect;
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

namespace KinectCoordinateMapping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CameraMode _mode = CameraMode.Color;

        KinectSensor _sensor;
        Skeleton[] _bodies = new Skeleton[6];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.KinectSensors.Where(s => s.Status == KinectStatus.Connected).FirstOrDefault();

            if (_sensor != null)
            {
                _sensor.ColorStream.Enable();
                _sensor.DepthStream.Enable();
                _sensor.SkeletonStream.Enable();

                _sensor.AllFramesReady += Sensor_AllFramesReady;

                _sensor.Start();
            }
        }

        void Sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // Color
            using (var frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Color)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Depth
            using (var frame = e.OpenDepthImageFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Depth)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Body
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    frame.CopySkeletonDataTo(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // COORDINATE MAPPING
                            foreach (Joint joint in body.Joints)
                            {
                                // 3D coordinates in meters
                                SkeletonPoint skeletonPoint = joint.Position;

                                // 2D coordinates in pixels
                                Point point = new Point();

                                if (_mode == CameraMode.Color)
                                {
                                    // Skeleton-to-Color mapping
                                    ColorImagePoint colorPoint = _sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);

                                    point.X = colorPoint.X;
                                    point.Y = colorPoint.Y;
                                }
                                else if (_mode == CameraMode.Depth) // Remember to change the Image and Canvas size to 320x240.
                                {
                                    // Skeleton-to-Depth mapping
                                    DepthImagePoint depthPoint = _sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution320x240Fps30);

                                    point.X = depthPoint.X;
                                    point.Y = depthPoint.Y;
                                }

                                // DRAWING...
                                Ellipse ellipse = new Ellipse
                                {
                                    Fill = Brushes.LightBlue,
                                    Width = 20,
                                    Height = 20
                                };

                                Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                                Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

                                canvas.Children.Add(ellipse);
                            }
                        }
                    }
                }
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_sensor != null)
            {
                _sensor.Stop();
            }
        }
    }

    enum CameraMode
    {
        Color,
        Depth
    }
}
