using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Drawing;
using Discord;

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

        public async Task ReadFromUrl(string url) {
            _ready = false;
            HttpClient client = new HttpClient();
            Uri uri;
            if(Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri)) {
                Stream stream = await client.GetStreamAsync(uri);
                if(stream != null) {
                    _bitmap = (Bitmap)System.Drawing.Image.FromStream(stream);
                    _bitmap.Save($"{folder}/{DateTime.Now.ToFileTime()}.bmp");
                }
            }
            else {

            }
            _ready = true;
        }

        private const string folder = "Resources/Images";

        private bool _ready;
        public bool Ready {
            get {
                return _ready;
            }
        }

        private Bitmap _bitmap;
        public Bitmap Bitmap {
            get {
                return _bitmap;
            }
        }
    }
}
