using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace AssetsDownloadWork.Utils{
	public sealed class HttpWebRequestDownload{
		public string downloadedFilePath = string.Empty;

		private string _defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		private long _totalBytesLength;
		private long _transferredBytes;
		private bool abort;
		
		public HttpWebRequestDownload(){
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		}

		private int _transferredPercents => (int)(100 * this._transferredBytes / this._totalBytesLength);

		public void DownloadFile(string url, string destinationDirectory = default){
			Task.Factory.StartNew(async () => {
				destinationDirectory ??= this._defaultDirectory;

				try{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
					request.Headers.Add("Cache-Control", "no-cache");
					request.Headers.Add("Cache-Control", "no-store");
					request.Headers.Add("Cache-Control", "max-age=1");
					request.Headers.Add("Cache-Control", "s-maxage=1");
					request.Headers.Add("Pragma", "no-cache");
					request.Headers.Add("Expires", "-1");

					if(!Directory.Exists(destinationDirectory)){
						Directory.CreateDirectory(destinationDirectory);
					}

					WebResponse responseAsync = await request.GetResponseAsync();

					using HttpWebResponse response = (HttpWebResponse)responseAsync;
					this._totalBytesLength = response.ContentLength;

					string path = response.Headers["Content-Disposition"];
					string filename;
					if(string.IsNullOrWhiteSpace(path)){
						Uri uri = new Uri(url);
						filename = Path.GetFileName(uri.LocalPath);
					}else{
						ContentDisposition contentDisposition = new ContentDisposition(path);
						filename = contentDisposition.FileName;
					}

					Stream responseStream = response.GetResponseStream();
					FileStream fileStream = File.Create(Path.Combine(destinationDirectory, filename));
					byte[] buffer = new byte[1024 * 1024];
					ProgressEventArgs eventArgs = new ProgressEventArgs(this._totalBytesLength);

					if(responseStream != null){
						int size = await responseStream.ReadAsync(buffer, 0, buffer.Length);
						while(size > 0){
							if(this.abort) break;
							
							fileStream.Write(buffer, 0, size);
							this._transferredBytes += size;

							size = await responseStream.ReadAsync(buffer, 0, buffer.Length);

							eventArgs.UpdateData(this._transferredBytes, this._transferredPercents);
							this.OnDownloadProgressChanged(eventArgs);
						}

						responseStream?.Close();
					}

					this.downloadedFilePath = Path.Combine(destinationDirectory, filename);
					if(!this.abort){
						this.OnDownloadFileCompleted(EventArgs.Empty);
					}
				}catch(Exception e){
					this.OnError($"{e.Message} => {e.InnerException?.Message}");
				}
			});
		}

		public void AbortRequest(){
			this.abort = true;
		}

		// Events
		public event EventHandler<ProgressEventArgs> DownloadProgressChanged;
		public event EventHandler DownloadFileCompleted;
		public event EventHandler<string> Error;

		private void OnDownloadProgressChanged(ProgressEventArgs e){
			this.DownloadProgressChanged?.Invoke(this, e);
		}

		private void OnDownloadFileCompleted(EventArgs e){
			this.DownloadFileCompleted?.Invoke(this, e);
		}

		private void OnError(string errorMessage){
			this.Error?.Invoke(this, errorMessage);
		}

		public class ProgressEventArgs : EventArgs{
			public ProgressEventArgs(long transferredBytes, int transferredPercents, long totalBytesLength){
				this.TransferredBytes = transferredBytes;
				this.TransferredPercents = transferredPercents;
				this.TotalBytesLength = totalBytesLength;
			}

			public ProgressEventArgs(long totalBytesLength){
				this.TotalBytesLength = totalBytesLength;
			}

			public long TransferredBytes{ get; set; }
			public long TotalBytesLength{ get; set; }
			public int TransferredPercents{ get; set; }

			public void UpdateData(long transferredBytes, int transferredPercents){
				this.TransferredBytes = transferredBytes;
				this.TransferredPercents = transferredPercents;
			}
		}
	}
}