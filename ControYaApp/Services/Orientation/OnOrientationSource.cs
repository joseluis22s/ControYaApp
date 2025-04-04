using System.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ControYaApp.Services.Orientation
{
    public partial class OnOrientationSource : INotifyPropertyChanged
    {
        private object _defaultValue;
        private object _portraitValue;
        private object _landscapeValue;

        public object DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public object PortraitValue
        {
            get => _portraitValue;
            set
            {
                _portraitValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public object LandscapeValue
        {
            get => _landscapeValue;
            set
            {
                _landscapeValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public object Value => DeviceDisplay.MainDisplayInfo.Orientation switch
        {
            DisplayOrientation.Portrait => PortraitValue ?? DefaultValue,
            DisplayOrientation.Landscape => LandscapeValue ?? DefaultValue,
            _ => DefaultValue
        };

        public OnOrientationSource()
        {
            WeakReferenceMessenger.Default.Register<OnOrientationSource, OnOrientationExtension.OrientationChangedMessage>(this, OnOrientationChanged);
        }

        private void OnOrientationChanged(OnOrientationSource r, OnOrientationExtension.OrientationChangedMessage m)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
