using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auremo.Controls
{
    public partial class VolumeWheel : UserControl
    {
        private const double AngleAtMute = 7.0 * Math.PI / 6.0;
        private const double AngleDeltaPerVolumeTick = 8.0 * Math.PI / 6.0 / 100.0;
        private const double HaloCenterX = 60.0;
        private const double HaloCenterY = 60.0;
        private const double HaloRadius = 53.0;
        private static readonly double ArcStartX = HaloCenterX + HaloRadius * Math.Cos(AngleAtMute);
        private static readonly double ArcStartY = HaloCenterY - HaloRadius * Math.Sin(AngleAtMute);

        public static readonly DependencyProperty ServerSideVolumeProperty =
            DependencyProperty.Register("ServerSideVolume", typeof(int), typeof(VolumeWheel),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnServerSideVolumeChanged)));
        public static readonly DependencyProperty ClientSideVolumeProperty =
            DependencyProperty.Register("ClientSideVolume", typeof(int), typeof(VolumeWheel),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnClientSideVolumeChanged)));
        private static readonly DependencyProperty HaloArcEndProperty =
            DependencyProperty.Register("HaloArcEnd", typeof(Point), typeof(VolumeWheel), new PropertyMetadata(new Point(ArcStartX, ArcStartY)));
        private static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(VolumeWheel), new PropertyMetadata(false));
        private static readonly DependencyProperty TickRotationProperty =
            DependencyProperty.Register("TickRotation", typeof(double), typeof(VolumeWheel), new PropertyMetadata(-120.0));

        // TODO: there fire events with MouseEventArgs args, but I have no idea
        // if they are correctly constructed. The arguments are not used either,
        // so they are kind of pointess. If someone is more in the know than I
        // am, fixing these would be excellent.
        public event EventHandler<MouseEventArgs> UserStartedEdit;
        public event EventHandler<MouseEventArgs> UserFinishedEdit;

        public VolumeWheel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Volume as acquired from server status (0..100), 0 if missing.
        /// </summary>
        public int ServerSideVolume
        {
            get => (int)GetValue(ServerSideVolumeProperty);
            set => SetValue(ServerSideVolumeProperty, value);
        }

        /// <summary>
        /// Volume as set by user, or the server is the user is not editing the value (0..100).
        /// </summary>
        public int ClientSideVolume
        {
            get => (int)GetValue(ClientSideVolumeProperty);
            set => SetValue(ClientSideVolumeProperty, value);
        }

        public Point HaloArcEnd
        {
            get => (Point)GetValue(HaloArcEndProperty);
            set => SetValue(HaloArcEndProperty, value);
        }

        public bool IsLargeArc
        {
            get => (bool)GetValue(IsLargeArcProperty);
            set => SetValue(IsLargeArcProperty, value);
        }

        public double TickRotation
        {
            get => (double)GetValue(TickRotationProperty);
        }

        public static Point HaloCenter => new Point(60.0, 60.0);

        public Size HaloRadiusSize => new Size(HaloRadius, HaloRadius);

        public Point HaloArcStart => new Point(ArcStartX, ArcStartY);

        private static void OnServerSideVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int oldVol = (int)e.OldValue;
            int newVol = (int)e.NewValue;

            if (newVol != oldVol)
            {
                d.SetValue(IsLargeArcProperty, newVol >= 75); // The arc is exactly a half-circle at vol=75%.
                double angle = VolumeToAngle(newVol);
                d.SetValue(HaloArcEndProperty, AngleToArcEndPoint(angle));
            }
        }

        private static void OnClientSideVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int oldVol = (int)e.OldValue;
            int newVol = (int)e.NewValue;

            if (newVol != oldVol)
            {
                d.SetValue(TickRotationProperty, -120 + newVol * 2.4);
            }
        }


        #region Drag logic

        private bool m_Dragging = false;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Dragging = true;
            UserStartedEdit?.Invoke(sender, new MouseEventArgs(e.MouseDevice, e.Timestamp));
            UpdateVolume(sender, e);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Note that UpdateVolume() may call EndDrag() too (it then returns false).
            if (m_Dragging && UpdateVolume(sender, e))
            {
                EndDrag(sender, e);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            EndDrag(sender, e);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_Dragging)
            {
                UpdateVolume(sender, e);
            }
        }

        private void EndDrag(object sender, MouseEventArgs e)
        {
            m_Dragging = false;
            UserFinishedEdit?.Invoke(sender, e);
        }

        private bool UpdateVolume(object sender, MouseEventArgs e)
        {
            double angle = PointToAngle(e.GetPosition(m_CoordinateReferencePoint));

            if (double.IsNaN(angle))
            {
                EndDrag(sender, e);
                return false;
            }
            else
            {
                ClientSideVolume = AngleToVolume(angle);
                return true;
            }
        }

        #endregion

        #region Math

        private static double PointToAngle(Point p)
        {
            // Don't consider the very center of the wheel as
            // things get twitchy.
            if (p.X * p.X + p.Y * p.Y <= 4.0)
            {
                return double.NaN;
            }

            return Math.Atan2(-p.Y, p.X);
        }

        private static double VolumeToAngle(int volume)
        {
            return AngleAtMute - volume * AngleDeltaPerVolumeTick;
        }

        private static Point AngleToArcEndPoint(double angle)
        {
            return new Point(HaloCenter.X + HaloRadius * Math.Cos(angle),
                             HaloCenter.Y - HaloRadius * Math.Sin(angle));
        }

        private int AngleToVolume(double angle)
        {
            // TODO: A mathemagician could probably tidy this up quite a bit.
            double cwAngleFromStraightDown = (7.0 * Math.PI / 2.0 - angle) % (2.0 * Math.PI);

            if (cwAngleFromStraightDown < Math.PI / 3.0)
            {
                return 0; // Counterclockwise from "8 o'clock"
            }
            else if (cwAngleFromStraightDown > 5.0 * Math.PI / 3.0)
            {
                return 100; // Clockwise from "4 o'clock"
            }
            else
            {
                double cwAngleFromMute = (2.0 * Math.PI - angle + AngleAtMute) % (2.0 * Math.PI);
                double volumeTicks = cwAngleFromMute / AngleDeltaPerVolumeTick;
                int rounded = (int)Math.Round(volumeTicks, MidpointRounding.AwayFromZero);
                int clamped = Math.Min(Math.Max(0, rounded), 100);

                return clamped;
            }
        }

        #endregion
    }
}
