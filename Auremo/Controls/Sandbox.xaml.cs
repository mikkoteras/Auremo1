using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace Auremo.Controls
{
    public partial class Sandbox : UserControl, INotifyPropertyChanged
    {
        public Sandbox()
        {
            //CreateBrushedMetalBrush();
            InitializeComponent();
        }

        private void CreateBrushedMetalBrush()
        {
            Bitmap bitmap = new Bitmap(2560, 256);
            Random random = new Random(0);

            /*
            for (int y = 0; y < bitmap.Height; ++y)
            {
                int x = 0;

                while (x < bitmap.Width)
                {
                    int w = random.Next() % 512 + 256;
                    byte rgb = (byte)(random.Next() % 32 + 200);

                    while (x < bitmap.Width && w >= 0)
                    {
                        bitmap.SetPixel(x, y, Color.FromArgb(0xFF, rgb, rgb, rgb));
                        ++x;
                        --w;
                    }
                }
            }

            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    int r = bitmap.GetPixel(x, y).R;
                    byte rgb = (byte)(r - 10 + random.Next() % 21);
                    bitmap.SetPixel(x, y, Color.FromArgb(0xFF, rgb, 0, 0));
                }
            }
            */

            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    byte rgb = (byte)(random.Next() % 64 + 100);
                    bitmap.SetPixel(x, y, Color.FromArgb(0x7F, rgb, rgb, rgb));
                }
            }

            Tile = new WriteableBitmap(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()));
        }

        public BitmapSource Tile
        {
            get;
            set;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion

        private void OnVolumeUpButtonClicked(object sender, RoutedEventArgs e)
        {

        }
        private void OnVolumeDownButtonClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
