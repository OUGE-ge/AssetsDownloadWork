using System.Windows;
using AssetsDownloadWork.Managers;

namespace AssetsDownloadWork{
    public partial class MainWindow{
        public MainWindow(){
            this.InitializeComponent();

            new TimeSlinger();
            new DownloadManager();

            this.MyFrame.Content = new DownloadPage();

            this.Init();
        }

        private void Init(){
            
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();
        private void Minimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
    }
}