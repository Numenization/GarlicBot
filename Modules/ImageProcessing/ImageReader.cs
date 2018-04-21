using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Drawing;
//using Discord;

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

        public async Task<bool> ReadFromUrl(string url, IProgress<string> progress) {
            if(!_ready) {
                progress.Report("Already working on operation");
                return false;
            }
            _ready = false;

            Uri uri;
            if(Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri)) {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                long fileSize = webResponse.ContentLength;
                string extension = Path.GetExtension(url);
                if (fileSize <= (Config.bot.maxFileSize * kilobyteRatio)) { // max file size is in kilobytes, file size is given in bytes
                    progress.Report($"File size is {Math.Round((double)fileSize / kilobyteRatio)} KB. Opening download stream...");
                    WebClient client = new WebClient();
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    client.DownloadProgressChanged += (s, e) => {
                        if(stopwatch.ElapsedMilliseconds % 250 == 0)
                            progress.Report($"Downloading {Math.Round((double)e.BytesReceived / kilobyteRatio)}/{Math.Round((double)fileSize / kilobyteRatio)} KB");
                    };

                    string fileName = $"{DateTime.Now.ToFileTime()}";
                    await client.DownloadFileTaskAsync(uri, $"{folder}/{fileName}{extension}");
                    stopwatch.Stop();
                    progress.Report($"Download complete ({Math.Round((double)fileSize / kilobyteRatio)} KB). Opening file for modification...");
                    _bitmap = (Bitmap)System.Drawing.Image.FromFile($"{folder}/{fileName}{extension}");
                    progress.Report("Image opened and ready for writing.");
                }
                else {
                    progress.Report($"File is too big! ({Math.Round((double)fileSize / kilobyteRatio)}/{Config.bot.maxFileSize} KB)");
                    _ready = true;
                    return false;
                }
            }
            else {
                progress.Report($"Could not read URL");
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

        private Bitmap _bitmap;
        public ref Bitmap Bitmap {
            get {
                return ref _bitmap;
            }
        }
    }
}
