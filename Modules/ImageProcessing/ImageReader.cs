﻿using System;
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
    /// <summary>
    /// Downloads images from URLs to be used for image processing
    /// </summary>
    public class ImageReader
    {
        /// <summary>
        /// Downloads images from URLs to be used for image processing
        /// </summary>
        public ImageReader() {
            _bitmap = null;
            _ready = true;
            if(!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
        }

        /// <summary>
        /// Downloads an image from a given URL
        /// </summary>
        /// <param name="url">An absolute URL pointing to an image</param>
        /// <param name="progress">Keeps track of the download progress</param>
        /// <param name="error">If the task returns false, error information will be passed here</param>
        /// <returns></returns>
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
                    MemoryStream ms;
                    byte[] data;
                    long elapsedTimeSinceLastUpdate = 0;

                    stopwatch.Start();

                    client.DownloadProgressChanged += (s, e) => {
                        if(stopwatch.ElapsedMilliseconds - elapsedTimeSinceLastUpdate >= Config.bot.progressUpdateDelay) {
                            elapsedTimeSinceLastUpdate = stopwatch.ElapsedMilliseconds;
                            progress.Report($"Downloading {Math.Round((double)e.BytesReceived / kilobyteRatio)}/{Math.Round((double)fileSize / kilobyteRatio)} KB");
                        }
                    };

                    _fileName = $"{DateTime.Now.ToFileTime()}";
                    data = await client.DownloadDataTaskAsync(uri);
                    progress.Report($"Download complete ({Math.Round((double)fileSize / kilobyteRatio)} KB).");

                    try {
                        progress.Report($"Reading image data...");
                        ms = new MemoryStream(data);
                        _bitmap = new Bitmap(ms);
                        _bitmap.Save($"{folder}/{_fileName}{extension}");
                        progress.Report($"Image reading complete. ({(double)stopwatch.ElapsedMilliseconds / 1000} s)");
                    }
                    catch(Exception e) {
                        string errorMsg = $"{e.HResult} ({e.Message})";
                        progress.Report(errorMsg);
                        error.ErrorReason = errorMsg;
                        return false;
                    }

                    stopwatch.Stop();
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
        /// <summary>
        /// Status representing whether the reader is ready to download another file
        /// </summary>
        public bool Ready {
            get {
                return _ready;
            }
        }

        private string _fileName;
        /// <summary>
        /// The filename of the currently stored bitmap
        /// </summary>
        public string FileName {
            get {
                return _fileName;
            }
        }

        private Bitmap _bitmap;
        /// <summary>
        /// The currently stored bitmap in the image reader
        /// </summary>
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
        /// <summary>
        /// The error returned by an ImageReader
        /// </summary>
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
