using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SSTMTerminal.Controls
{
    public class PhotoSlideShow
    {
        List<BitmapSource> _images;
        static int _current = -1;

        public PhotoSlideShow() { }

        public void MoveFirst()
        {
            lock (this)
            {
                _current = 0;
            }
        }

        public void MoveNext()
        {
            lock (this)
            {
                ++_current;
                _current %= _images.Count;
            }
        }

        public void MovePrevious()
        {
            lock (this)
            {
                if (_current > 0)
                {
                    --_current;
                }
                else
                {
                    _current = 0;
                }
            }
        }

        public BitmapSource PreviousPhoto
        {
            get
            {
                if (_current > 0)
                {
                    return _images[_current - 1];
                }
                else if (_current == 0 && _images.Count > 0)
                {
                    return _images[_images.Count - 1];
                }

                return null;
            }
        }

        public BitmapSource CurrentPhoto
        {
            get
            {
                if (_current != -1)
                {
                    return _images[_current];
                }
                return null;
            }
        }

        public List<BitmapSource> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
            }
        }

        public BitmapSource NextPhoto
        {
            get
            {
                if (_images != null && _images.Count > 0)
                {
                    return _images[(_current + 1) % _images.Count];
                }
                return null;
            }
        }
    }
}
