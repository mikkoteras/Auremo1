using Auremo.DataModel.ConfigurationObjects;
using Auremo.DataModel.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Auremo.DataModel.DataStores

{
    public class Configuration : INotifyPropertyChanged
    {
        #region Options

        [XmlRoot]
        public class Options : INotifyPropertyChanged
        {
            public Options()
            {
            }

            public Options(Options rhs)
            {
                PrimaryServer = new MpdServer(rhs.PrimaryServer);
                AdditionalServers = new List<MpdServer>();
                
                foreach (MpdServer elem in rhs.AdditionalServers)
                {
                    AdditionalServers.Add(new MpdServer(elem));
                }

                VolumeControlType = rhs.VolumeControlType;
            }

            public MpdServer PrimaryServer
            {
                get
                {
                    return m_PrimaryServer;
                }
                set
                {
                    if (m_PrimaryServer != value)
                    {
                        m_PrimaryServer = value;
                        NotifyPropertyChanged("PrimaryServer");
                    }
                }
            }

            public List<MpdServer> AdditionalServers
            {
                get
                {
                    return m_AdditionalServers;
                }
                set
                {
                    if (m_AdditionalServers != value)
                    {
                        m_AdditionalServers = value;
                        NotifyPropertyChanged("AdditionalServers");
                    }
                }
            }

            public VolumeControlType VolumeControlType
            {
                get => m_VolumeControlType;
                set
                {
                    if (m_VolumeControlType != value)
                    {
                        m_VolumeControlType = value;
                        NotifyPropertyChanged("VolumeControlType");
                        NotifyPropertyChanged("VolumeControlType");
                        NotifyPropertyChanged("VolumeControlTypeIsNone");
                        NotifyPropertyChanged("VolumeControlTypeIsWheel");
                        NotifyPropertyChanged("VolumeControlTypeIsSlider");
                        NotifyPropertyChanged("VolumeControlTypeIsButtons");
                    }
                }
            }
        
            public bool VolumeControlTypeIsNone => VolumeControlType == VolumeControlType.None;
            public bool VolumeControlTypeIsWheel => VolumeControlType == VolumeControlType.Wheel;
            public bool VolumeControlTypeIsSlider => VolumeControlType == VolumeControlType.Slider;
            public bool VolumeControlTypeIsButtons => VolumeControlType == VolumeControlType.Buttons;

            private MpdServer m_PrimaryServer = new MpdServer();
            private List<MpdServer> m_AdditionalServers = new List<MpdServer>();
            private VolumeControlType m_VolumeControlType = VolumeControlType.Wheel;

            #region INotifyPropertyChanged implementation

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string info)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }

            #endregion
        }

        #endregion

        #region API

        public Configuration()
        {
            SavedOptions = new Options();
            LoadSavedOptions();
            ActiveOptions = new Options(SavedOptions);
            DisplayedOptions = new Options(SavedOptions);
        }

        public Options ActiveOptions
        {
            get => m_ActiveOptions;
            set
            {
                if (m_ActiveOptions != value)
                {
                    m_ActiveOptions = value;
                    NotifyPropertyChanged("ActiveOptions");
                }
            }
        }

        public Options DisplayedOptions
        {
            get => m_DisplayedOptions;
            set
            {
                if (m_DisplayedOptions != value)
                {
                    m_DisplayedOptions = value;
                    NotifyPropertyChanged("DisplayedOptions");
                }
            }
        }

        private Options SavedOptions
        {
            get => m_SavedOptions;
            set
            {
                if (m_SavedOptions != value)
                {
                    m_SavedOptions = value;
                    NotifyPropertyChanged("SavedOptions");
                }
            }
        }

        public void ApplyDisplayedOptions()
        {
            ActiveOptions = new Options(DisplayedOptions);
        }

        public void SaveDisplayedOptions()
        {
            SavedOptions = new Options(DisplayedOptions);
            ActiveOptions = new Options(DisplayedOptions);
            SaveActiveOptions();
        }

        public void RevertToSavedOptions()
        {
            ActiveOptions = new Options(SavedOptions);
            DisplayedOptions = new Options(SavedOptions);
        }

        private Options m_ActiveOptions;
        private Options m_DisplayedOptions;
        private Options m_SavedOptions;

        #endregion

        #region Save/load implementation

        private static readonly string m_ConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Auremo";
        private static readonly string m_ConfigFile = m_ConfigDir + @"\config.xml";

        private void LoadSavedOptions()
        {
            try
            {
                using (FileStream input = new FileStream(m_ConfigFile, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Options));
                    SavedOptions = serializer.Deserialize(input) as Options;
                }
            }
            catch (Exception e)
            {
                World.Instance.Log.LogMessage($"Cannot open file {m_ConfigFile}: {e.Message}");
                World.Instance.Log.LogMessage("Continuing with default settings. You can change them, but saving them may fail.");
            }
        }

        private void SaveActiveOptions()
        {
            try
            {
                if (!Directory.Exists(m_ConfigDir))
                {
                    Directory.CreateDirectory(m_ConfigDir);
                }

                using (FileStream output = new FileStream(m_ConfigFile, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Options));
                    serializer.Serialize(output, ActiveOptions);
                    XmlWriter writer = new XmlTextWriter(output, Encoding.UTF8);
                }
            }
            catch (Exception e)
            {
                World.Instance.Log.LogMessage("Could not to save options: " + e.Message);
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
}
