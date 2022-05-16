using System.Windows;
using System.Windows.Controls;
using AssetsDownloadWork.Managers;

namespace AssetsDownloadWork{
    public partial class DownloadPage : Page{
        public DownloadPage(){
            InitializeComponent();
        }

        private void CancelClick(object sender, RoutedEventArgs e){
            throw new System.NotImplementedException();
        }

        private void UpdateClick(object sender, RoutedEventArgs e){
            throw new System.NotImplementedException();
        }

        private void OnUpdateModalClick(object sender, RoutedEventArgs e){
            throw new System.NotImplementedException();
        }

        private void OnCloseModalClick(object sender, RoutedEventArgs e){
            throw new System.NotImplementedException();
        }

        private void TestClick(object sender, RoutedEventArgs e){
            DownloadManager.Instance.SaveToFile();
        }
    }
}