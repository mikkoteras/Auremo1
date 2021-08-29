using Auremo.DataModel;
using Auremo.DataModel.Types;
using Auremo.Network;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auremo.Controls
{
    public partial class PlaybackControlsPanel : UserControl, INotifyPropertyChanged
    {
        // Seek bar complications.
        private bool m_SeekBarDragInProgress = false;
        float? m_SeekBarTimeCode = null;
        bool m_SeekBarTimeCodeIsValid = false;

        public PlaybackControlsPanel()
        {
            InitializeComponent();

            /*
            // This can't be null, but it confuses the Visual Studio Designer.
            if (World.Instance != null)
            {
                World.Instance.ServerStatus.PropertyChanged += OnServerPropertyChanged;
            }
            */
        }

        public float? SeekBarTimeCode
        {
            get => m_SeekBarTimeCode;
            set
            {
                if (m_SeekBarTimeCode != value)
                {
                    m_SeekBarTimeCode = value;
                    NotifyPropertyChanged("SeekBarTimeCode");
                }
            }
        }

        public bool SeekBarTimeCodeIsValid
        {
            get => m_SeekBarTimeCodeIsValid;
            set
            {
                if (m_SeekBarTimeCodeIsValid != value)
                {
                    m_SeekBarTimeCodeIsValid = value;
                    NotifyPropertyChanged("SeekBarTimeCodeIsValid");
                }
            }
        }

        /*
        private void OnServerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Volume")
            {
                // TODO: why isn't the normal data binding working?
                m_VolumeWheel.ServerSideVolume = World.Instance.ServerStatus.Volume;
            }
            else if (e.PropertyName == "Elapsed")
            {
                // We need to set the seek bar value in code-behind, because we
                // need to be able to distinguish seek bar movement initiated
                // by the server v. the user.
                if (!m_SeekBarDragInProgress)
                {
                    float? elapsed = World.Instance.ServerStatus.Elapsed;
                    m_SeekBar.Value = elapsed.GetValueOrDefault(0.0f);
                    SeekBarTimeCode = elapsed;
                    SeekBarTimeCodeIsValid = elapsed != null;
                }
            }
        }
        */

        private void OnSeekBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // TODO: This could probably be accomplished without code behind.
            SeekBarTimeCode = (float)e.NewValue;
        }

        private void OnSeekBarGotMouseCapture(object sender, MouseEventArgs e)
        {
            m_SeekBarDragInProgress = true;
        }

        private void OnSeekBarLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (m_SeekBarDragInProgress)
            {
                SeekTrackToSeekBarPosition();
                m_SeekBarDragInProgress = false;
            }
        }

        private void SeekTrackToSeekBarPosition()
        {
            int? song = World.Instance.ServerStatus.Song;

            if (song != null)
            {
                int pos = (int)m_SeekBar.Value;
                World.Instance.CommandRunner.PushCommand(new Command("seek", song.ToString(), pos.ToString()));
            }
        }

        private void OnToggleRandomClick(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("random", World.Instance.ServerStatus.Random ? "0" : "1"));
        }

        private void OnToggleRepeatClick(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("repeat", World.Instance.ServerStatus.Repeat ? "0" : "1"));
        }

        private void OnToggleSingleClick(object sender, RoutedEventArgs e)
        {
            SingleMode single = World.Instance.ServerStatus.SingleMode;
            World.Instance.CommandRunner.PushCommand(new Command("single", single == SingleMode.True ? "0" : "1"));
        }

        private void OnToggleOneshotClick(object sender, RoutedEventArgs e)
        {
            SingleMode single = World.Instance.ServerStatus.SingleMode;
            World.Instance.CommandRunner.PushCommand(new Command("single", single == SingleMode.Oneshot ? "0" : "oneshot"));
        }

        private void OnToggleConsumeClick(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("consume", World.Instance.ServerStatus.Consume ? "0" : "1"));
        }

        private void OnVolumeUpButtonClicked(object sender, RoutedEventArgs e)
        {
            int vol = World.Instance.ServerStatus.Volume.ClientSideValue;

            if (vol < 100)
            {
                World.Instance.ServerStatus.Volume.ClientSideValue = vol + 1;
            }
        }

        private void OnVolumeDownButtonClicked(object sender, RoutedEventArgs e)
        {
            int vol = World.Instance.ServerStatus.Volume.ClientSideValue;

            if (vol > 0)
            {
                World.Instance.ServerStatus.Volume.ClientSideValue = vol - 1;
            }
        }
        
        public void OnVolumeEditStart(object sender, MouseEventArgs e)
        {
            World.Instance.ServerStatus.Volume.UserIsEditing = true;
        }
        
        public void OnVolumeEditFinish(object sender, MouseEventArgs e)
        {
            World.Instance.ServerStatus.Volume.UserIsEditing = false;
        }
             
        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("previous"));
        }
        
        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("stop"));
        }

        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("play"));
        }

        private void OnPauseClick(object sender, RoutedEventArgs e)
        {

            World.Instance.CommandRunner.PushCommand(new Command("pause", "1"));
        }

        private void OnSkipClick(object sender, RoutedEventArgs e)
        {
            World.Instance.CommandRunner.PushCommand(new Command("next"));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
