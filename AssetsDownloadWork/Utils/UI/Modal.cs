using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace AssetsDownloadWork.Utils.UI{
    public class Modal : ContentControl{
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Modal), new PropertyMetadata(false));

        static Modal(){
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(typeof(Modal)));
            BackgroundProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(CreateDefaultBackground()));
        }

        public bool IsOpen{
            get => (bool)this.GetValue(IsOpenProperty);
            set => this.SetValue(IsOpenProperty, value);
        }

        private static object CreateDefaultBackground(){
            return new SolidColorBrush(Colors.Black){
                Opacity = 0.3
            };
        }
    }
}