using System;
using System.IO;
using AssetsDownloadWork.Managers;

namespace AssetsDownloadWork.Utils{
	public class AssetFile{
		public bool IAmReady;
		public long FileSize;
		public string FileName;
		public string FilePath;

		private HttpWebRequestDownload hDownload;

		public AssetFile(long _size, string _fileName){
			this.FileSize = _size;
			this.FileName = _fileName;
			this.hDownload = new HttpWebRequestDownload();

			this.VerifyLocation();
		}

		public void VerifyAsset(){
			Console.WriteLine($"Verifying: {this.FileName}...");

			if(this.ValidateExists() && this.ValidateSize()){
				Console.WriteLine($"{this.FileName} was successfully verified !");
				this.IAmReady = true;
				return;
			}

			this.DeleteFile();
			this.DownloadFile();
		}

		public void DownloadFile(){
			// Download File
			// ReVerifyAsset

			this.hDownload.DownloadProgressChanged += this.HDownloadOnDownloadProgressChanged;

			this.hDownload.DownloadFileCompleted += (_, _) => {
				Console.WriteLine("Download finished and saved to: " + this.hDownload.downloadedFilePath);
				this.VerifyAsset();
			};

			this.hDownload.Error += (_, errMessage) => {
				Console.WriteLine("Error has occured !! => " + errMessage);
			};

			this.hDownload.DownloadFile($"http://speedtest.tele2.net/{this.FileName}", Directory.GetCurrentDirectory());
		}

		public void KillDownloadFile(){
			this.hDownload.AbortRequest();
			TimeSlinger.FireTimer(this.DeleteFile, 5f);
		}

		private void HDownloadOnDownloadProgressChanged(object sender, HttpWebRequestDownload.ProgressEventArgs e){
			Console.WriteLine($"[{this.FileName}] Progress: " + e.TransferredBytes + " => " + e.TransferredPercents);
		}

		// File Tests
		public bool ValidateSize(){
			this.VerifyLocation();

			if(!this.ValidateExists()){
				return false;
			}

			return new FileInfo(this.FilePath).Length == this.FileSize;
		}

		public bool ValidateExists(){
			this.VerifyLocation();

			return File.Exists(this.FilePath);
		}

		public void DeleteFile(){
			if(!this.ValidateExists()){
				return;
			}

			TimeSlinger.FireIntervalUntil(() => !File.Exists(this.FilePath), () => {
				try{
					File.Delete(this.FilePath);
				}catch{
					// ignored
				}
			}, null, 1f);
		}

		// Utils
		private void VerifyLocation(){
			// this.FilePath = this.GetFullLongPath($"{GameLogic.Config.GameLocation}/{GameUtils.GameName}_Data/Resources/{this.FileName}");
			this.FilePath = this.GetFullLongPath($"{Directory.GetCurrentDirectory()}/{this.FileName}");
		}

		private string GetFullLongPath(string path){
			if(!path.StartsWith("\\\\?\\")){
				return "\\\\?\\" + Path.GetFullPath(path);
			}

			return path;
		}
	}
}