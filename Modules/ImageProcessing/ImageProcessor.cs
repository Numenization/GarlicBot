using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GarlicBot.Modules.ImageProcessing
{
    public class ImageProcessor
    {
        public ImageProcessor(ImageReader reader) {
            _working = false;
            _progress = 0;
            _progressDecimals = 1;
            _timeBetweenUpdates = 500;
            _reader = reader;
        }

        public ImageProcessor(ImageReader reader, ImageProcessorSettings settings) {
            _working = false;
            _progress = 0;
            _progressDecimals = settings.ProgressDecimals;
            _timeBetweenUpdates = settings.UpdateTick;
            _reader = reader;
        }

        public async Task<string> Scramble(IProgress<double> progress) {
            if(_reader.Bitmap != null && _reader.Ready) {
                //do stuff with image
                await Utilities.Log("Processing image...", Discord.LogSeverity.Info);
                Bitmap bitmap = _reader.Bitmap;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Random rand = new Random();
                long totalPixels = bitmap.Width * (bitmap.Height / 2);
                long lastUpdate = 0;
                long pixelsProcessed = 0;

                for (int i = 0; i < bitmap.Width; i++) {
                    for (int j = 0; j < bitmap.Height / 2; j++) {
                        if (rand.Next() % 2 == 1) {
                            Color temp = bitmap.GetPixel(i, j);
                            bitmap.SetPixel(i, j, bitmap.GetPixel(i, (bitmap.Height - 1) - j));
                            bitmap.SetPixel(i, (bitmap.Height - 1) - j, temp);
                        }
                        pixelsProcessed++;
                        if (stopwatch.ElapsedMilliseconds - lastUpdate >= _timeBetweenUpdates) {
                            lastUpdate = stopwatch.ElapsedMilliseconds;
                            progress.Report(Math.Round(((double)pixelsProcessed / totalPixels) * 100, _progressDecimals));
                        }
                    }
                }

                string newFileName = $"Resources/Images/{_reader.FileName}_Scrambled.jpg";
                bitmap.Save(newFileName, ImageFormat.Jpeg);
                stopwatch.Stop();
                await Utilities.Log($"Done ({(double)stopwatch.ElapsedMilliseconds / 1000} s)", Discord.LogSeverity.Verbose);
                return newFileName;
            }
            else {
                throw new Exception("Trying to modify image from invalid reader.");
            }
        }

        private double _progress;
        private bool _working;
        private ImageReader _reader;
        private long _timeBetweenUpdates;
        private int _progressDecimals;

        public bool Working {
            get {
                return _working;
            }
        }
        public double Progress {
            get {
                return _progress;
            }
        }
        public long UpdateTick {
            set {
                _timeBetweenUpdates = value;
            }
            get {
                return _timeBetweenUpdates;
            }
        }
        public int ProgressDecimals {
            set {
                _progressDecimals = value;
            }
            get {
                return _progressDecimals;
            }
        }
    }

    public class ImageProcessorSettings {
        public ImageProcessorSettings(long updateTick = 500, int progressDecimals = 1) {
            UpdateTick = updateTick;
            ProgressDecimals = progressDecimals;
        }

        public long UpdateTick;
        public int ProgressDecimals;
    }
}
