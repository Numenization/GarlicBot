using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GarlicBot.Modules.ImageProcessing
{
    /// <summary>
    /// Class containing methods for altering images
    /// </summary>
    public class ImageProcessor
    {
        /// <summary>
        /// Class containing methods for altering images
        /// </summary>
        public ImageProcessor(ImageReader reader) {
            _working = false;
            _progress = 0;
            _progressDecimals = 1;
            _timeBetweenUpdates = Config.bot.progressUpdateDelay;
            _reader = reader;
        }

        public ImageProcessor(ImageReader reader, ImageProcessorSettings settings) {
            _working = false;
            _progress = 0;
            _progressDecimals = settings.ProgressDecimals;
            _timeBetweenUpdates = settings.UpdateTick;
            _reader = reader;
        }

        /// <summary>
        /// Randomly flips pixels along the X-Axis {O(n/2)}
        /// </summary>
        /// <param name="progress">Keeps track of the processing progress</param>
        /// <returns></returns>
        public async Task<string> Scramble(IProgress<double> progress) {
            if(_reader.Bitmap != null && _reader.Ready && !_working) {
                //do stuff with image
                Bitmap bitmap = _reader.Bitmap;
                Stopwatch stopwatch = new Stopwatch();
                Random rand = new Random();
                string newFileName = $"Resources/Images/{_reader.FileName}_Scrambled.jpg";
                long totalPixels = bitmap.Width * (bitmap.Height / 2);
                long lastUpdate = 0;
                long pixelsProcessed = 0;

                await Utilities.LogAsync("Processing image...", Discord.LogSeverity.Info);
                stopwatch.Start();
                _working = true;

                for (int i = 0; i < bitmap.Width; i++) {
                    for (int j = 0; j < bitmap.Height / 2; j++) {
                        if (rand.Next() % 2 == 1) {
                            var temp = bitmap.GetPixel(i, j);
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

                bitmap.Save(newFileName, ImageFormat.Jpeg);
                stopwatch.Stop();
                await Utilities.LogAsync($"Processing image complete. ({(double)stopwatch.ElapsedMilliseconds / 1000} s)", Discord.LogSeverity.Verbose);
                _working = false;
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

        /// <summary>
        /// Status representing whether the processor is working or not
        /// </summary>
        public bool Working {
            get {
                return _working;
            }
        }
        /// <summary>
        /// Gets the current processing progress
        /// </summary>
        public double Progress {
            get {
                return _progress;
            }
        }
        /// <summary>
        /// The time between updates
        /// </summary>
        public long UpdateTick {
            set {
                _timeBetweenUpdates = value;
            }
            get {
                return _timeBetweenUpdates;
            }
        }
        /// <summary>
        /// The number of decimal places for the progress reports
        /// </summary>
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
