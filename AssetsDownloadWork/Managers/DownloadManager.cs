using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AssetsDownloadWork.Managers{
    public class DownloadManager{
        public static DownloadManager Instance;

        private List<AssetFile> stableAssets = new List<AssetFile>{
            new AssetFile{ FileName = "100MB.zip", FilePath = "/", FileSize = 104857600L },
            new AssetFile{ FileName = "10MB.zip", FilePath = "/", FileSize = 10485760L }
        };

        // Verify files from current downloaded build
        // TODO: Check size, avaiability of all files from current downloaded build, read data from saved JSON
        public void VerifyFiles(){
            
        }

        // Change Game Build, delete all old files, download all the new, verify all new
        public void ChangeGameBuild(){
            
        }

        // Download all new files to TempFolder, verify all new, delete all old files, move from TempFolder to desired folders
        public void ManualUpdate(){
            
        }

        // Cancel All Downloads, Delete All Temp Files
        public void CancelDownload(){
            
        }
        
        // Delete Old Files from old build - find them in desired in-game folders 
        // TODO: Find what files they are, probably some JSON file
        private void DeleteOldFiles(){
            
        }

        // Download files to TempFolder
        private void DownloadFiles(){
            
        }

        // Save current build files locations
        public void SaveToFile(){
            ConfigFile configFile = new ConfigFile{
                ActiveBuildVersion = new Version(1, 0, 0),
                ActiveBuildType = "Stable",
                AssetFiles = new List<AssetFile>{
                    new AssetFile{ FileName = "100MB.zip", FilePath = "/", FileSize = 104857600L },
                    new AssetFile{ FileName = "10MB.zip", FilePath = "/", FileSize = 10485760L }
                }
            };

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "AssetsConfig.cfg"), JsonConvert.SerializeObject(configFile));
        }
        
        // Logic
        public DownloadManager(){
            DownloadManager.Instance = this;
        }

        ~DownloadManager(){
            DownloadManager.Instance = null;
        }
    }

    public class ConfigFile{
        public Version ActiveBuildVersion;
        public string ActiveBuildType;
        public List<AssetFile> AssetFiles;
    }
    
    public class AssetFile{
        public string FileName;
        public string FilePath;
        public long FileSize;
    }
}