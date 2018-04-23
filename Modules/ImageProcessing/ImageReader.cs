using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Drawing;

/*
 * 
 * This requires the GDIPlus library in order to work correctly
 * You can install this using this command:
 * sudo apt-get install libgdiplus
 * 
 */
 
namespace GarlicBot.Modules.ImageProcessing
{
    public class ImageReader
    {
        public ImageReader() {
            _bitmap = null;
            _ready = true;
            if(!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
        }

        public async Task<bool> ReadFromUrl(string url, IProgress<string> progress, ImageReadError error) {
            if(!_ready) {
                string errorMsg = "Already working on operation";
                progress.Report(errorMsg);
                error.ErrorReason = errorMsg;
                return false;
            }
            _ready = false;

            Uri uri;
            if(Uri.TryCreate(url, UriKind.Absolute, out uri) && Path.HasExtension(url)) {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                long fileSize = webResponse.ContentLength;
                string extension = Path.GetExtension(url);
                if (fileSize <= (Config.bot.maxFileSize * kilobyteRatio)) { // max file size is in kilobytes, file size is given in bytes
                    WebClient client = new WebClient();
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    long elapsedTimeSinceLastUpdate = 0;

                    client.DownloadProgressChanged += (s, e) => {
                        if(stopwatch.ElapsedMilliseconds - elapsedTimeSinceLastUpdate >= Config.bot.progressUpdateDelay) {
                            elapsedTimeSinceLastUpdate = stopwatch.ElapsedMilliseconds;
                            progress.Report($"Downloading {Math.Round((double)e.BytesReceived / kilobyteRatio)}/{Math.Round((double)fileSize / kilobyteRatio)} KB");
                        }
                    };

                    _fileName = $"{DateTime.Now.ToFileTime()}";
                    //await client.DownloadFileTaskAsync(uri, $"{folder}/{_fileName}{extension}");
                    byte[] data = await client.DownloadDataTaskAsync(uri);
                    MemoryStream ms;
                    try {
                        ms = new MemoryStream(data);
                        _bitmap = new Bitmap(ms);
                        _bitmap.Save($"{folder}/{_fileName}{extension}");
                    }
                    catch(Exception e) {
                        string errorMsg = "File either does not exist or is not an image";
                        progress.Report(errorMsg);
                        error.ErrorReason = errorMsg;
                        return false;
                    }
                    stopwatch.Stop();
                    //_bitmap = (Bitmap)Image.FromFile($"{folder}/{_fileName}{extension}"); // doesn't seem to work on linux
                    progress.Report($"Download complete ({Math.Round((double)fileSize / kilobyteRatio)} KB).");
                }
                else {
                    string errorMsg = $"File is too big! ({Math.Round((double)fileSize / kilobyteRatio)}/{Config.bot.maxFileSize} KB)";
                    progress.Report(errorMsg);
                    error.ErrorReason = errorMsg;
                    _ready = true;
                    return false;
                }
            }
            else {
                string errorMsg = await Utilities.GetAlert("imageDownloadFailed");
                progress.Report(errorMsg);
                error.ErrorReason = errorMsg;
                _ready = true;
                return false;
            }
            
            _ready = true;
            return true;
        }

        private const string folder = "Resources/Images";
        private const long kilobyteRatio = 1024;

        private bool _ready;
        public bool Ready {
            get {
                return _ready;
            }
        }

        private string _fileName;
        public string FileName {
            get {
                return _fileName;
            }
        }

        private Bitmap _bitmap;
        public ref Bitmap Bitmap {
            get {
                return ref _bitmap;
            }
        }
    }

    public class ImageReadError {
        public ImageReadError() {
            _reason = "NULL";
        }

        public ImageReadError(string reason) {
            _reason = reason;
        }

        private string _reason;
        public string ErrorReason {
            set {
                _reason = value;
            }
            get {
                return _reason;
            }
        }
    }
}
