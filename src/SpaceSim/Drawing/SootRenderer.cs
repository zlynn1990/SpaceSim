using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SpaceSim.Drawing
{
	class SootRenderer : IDisposable
	{
		private Bitmap _baseTexture;
		private Bitmap _sootTexture;
		private Bitmap _sootBuffer;

		public SootRenderer(Bitmap baseTexture, string baseTexturePath)
		{
			_baseTexture = baseTexture;

			string sootTexturePath = baseTexturePath.Replace(".png", "Soot.png");

			if (!File.Exists(sootTexturePath))
			{
				throw new FileNotFoundException("Could not find texture!", sootTexturePath);
			}

			// Initialized 'soot' texture and allocate the drawing buffer
			_sootTexture = new Bitmap(sootTexturePath);
			_sootBuffer = new Bitmap(_sootTexture.Width, _sootTexture.Height);
		}

		public Bitmap GenerateTexture(double sootRatio)
		{
			// Simple case for soot texture
			if (sootRatio > 0.99)
			{
				return _sootTexture;
			}

			// Simple case for base texture
			if (sootRatio < 0.01)
			{
				return _baseTexture;
			}

			// Otherwise blend
			// Build the main texture (a combination of base and soot)
			using (Graphics graphics = RenderUtils.GetContext(false, _sootBuffer))
			{
				graphics.DrawImage(_baseTexture, new Rectangle(0, 0, _sootBuffer.Width, _sootBuffer.Height));

				float[][] matrixAlpha =
				{
					new float[] {1, 0, 0, 0, 0},
					new float[] {0, 1, 0, 0, 0},
					new float[] {0, 0, 1, 0, 0},
					new float[] {0, 0, 0, (float)sootRatio, 0},
					new float[] {0, 0, 0, 0, 1}
				};

				ColorMatrix colorMatrix = new ColorMatrix(matrixAlpha);

				ImageAttributes iaAlphaBlend = new ImageAttributes();
				iaAlphaBlend.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

				graphics.DrawImage(_sootTexture, new Rectangle(0, 0, _sootBuffer.Width, _sootBuffer.Height), 0, 0,
								   _sootBuffer.Width, _sootBuffer.Height, GraphicsUnit.Pixel, iaAlphaBlend);
			}

			return _sootBuffer;
		}

		public void Dispose()
		{
			if (_sootTexture != null)
			{
				_sootTexture.Dispose();

				_sootTexture = null;
			}

			if (_sootBuffer != null)
			{
				_sootBuffer.Dispose();

				_sootBuffer = null;
			}
		}
	}
}
