using Auremo.DataModel;
using Auremo.Network;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Auremo.Controls
{
    public partial class ServerControlView : UserControl
    {
        public ServerControlView()
        {
            InitializeComponent();
        }

        private void OnOutputClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Output output)
            {
                World.Instance.CommandRunner.PushCommand(new Command("toggleoutput", output.Id.ToString()));
            }
        }

        private void OnMixRampThresholdStartEdit(object sender, MouseEventArgs e)
        {
            World.Instance.ServerStatus.MixRampThreshold.UserIsEditing = true;
        }

        private void OnMixRampThresholdFinishEdit(object sender, MouseEventArgs e)
        {
            World.Instance.ServerStatus.MixRampThreshold.UserIsEditing = false;
        }

        private void OnMixRampDelayStartEdit(object sender, MouseEventArgs e)
        {
            World.Instance.ServerStatus.MixRampDelay.UserIsEditing = true;
        }

        private void OnMixRampDelayFinishEdit(object sender, MouseEventArgs e)
        {
            World.Instance.ServerStatus.MixRampDelay.UserIsEditing = false;
        }

        private void OnCrossfadeStartEdit(object sender, MouseEventArgs e)
        {
            World.Instance.ServerStatus.Xfade.UserIsEditing = true;
        }

        private void OnCrossfadeFinishEdit(object sender, MouseEventArgs e)
        {
            World.Instance.ServerStatus.Xfade.UserIsEditing = false;
        }
    }
}
