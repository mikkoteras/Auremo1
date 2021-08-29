using Auremo.DataModel;
using Auremo.DataModel.Types;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Auremo.Controls
{
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            VolumeControlChoices = new ObservableCollection<VolumeControlType>()
            {
                VolumeControlType.None,
                VolumeControlType.Wheel,
                VolumeControlType.Slider,
                VolumeControlType.Buttons
            };

            InitializeComponent();
        }

        public IList<VolumeControlType> VolumeControlChoices
        {
            get;
            private set;
        }

        private void OnRevertClick(object sender, System.Windows.RoutedEventArgs e)
        {
            World.Instance.Configuration.RevertToSavedOptions();
        }

        private void OnApplyClick(object sender, System.Windows.RoutedEventArgs e)
        {
            World.Instance.Configuration.ApplyDisplayedOptions();
        }

        private void OnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            World.Instance.Configuration.SaveDisplayedOptions();
        }
    }
}
