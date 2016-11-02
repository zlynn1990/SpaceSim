using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceSim.Drawing
{
    class SpriteSheet : IDisposable
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        private Bitmap _sheet;

        private int _spriteWidth;
        private int _spriteHeight;

        public SpriteSheet(string source, int rows, int cols)
        {
            Rows = rows;
            Cols = cols;

            _sheet = new Bitmap(source);

            _spriteWidth = _sheet.Width/cols;
            _spriteHeight = _sheet.Height/rows;
        }

        public void Draw(int spriteIndex, Graphics graphics, RectangleF screenBounds)
        {
            int spriteX = spriteIndex / Rows;
            int spriteY = spriteIndex - spriteX * Rows;

            var source = new RectangleF(spriteX * _spriteWidth, spriteY * _spriteHeight, _spriteWidth, _spriteHeight);

            graphics.DrawImage(_sheet, screenBounds, source, GraphicsUnit.Pixel);
        }

        public void Dispose()
        {
            if (_sheet != null)
            {
                _sheet.Dispose();
                _sheet = null;
            }
        }
    }
}
