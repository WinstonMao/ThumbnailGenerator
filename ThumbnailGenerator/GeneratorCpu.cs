using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatterHackers.Agg.Image;
using MatterHackers.Agg.Platform;
using MatterHackers.DataConverters3D;
using MatterHackers.RayTracer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ThumbnailGenerator
{
	public class GeneratorCpu
	{
		/// <summary>
		/// cpu type is slow，i7-2threed,box.stl need 5s
		/// </summary>
		/// <param name="item"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		// Limit to private scope until need returns
		public ImageBuffer GetThumbnail(IObject3D item, int width, int height)
		{
			//if (item == null)
			//{
			//	return DefaultImage;
			//}

			//int estimatedMemorySize = item.EstimatedMemory();
			//if (estimatedMemorySize > MaxFileSizeForThumbnail)
			//{
			//	return DefaultImage;
			//}

			bool forceOrthographic = false;
			//if (estimatedMemorySize > MaxFileSizeForTracing)
			//{
			//	forceOrthographic = true;
			//}

			//bool RenderOrthographic = (forceOrthographic) ? true : UserSettings.Instance.ThumbnailRenderingMode == "orthographic";

			//ORTHOGROPHIC 俯视
			return ThumbnailEngine.Generate(
				item,
				/*RenderOrthographic ? RenderType.ORTHOGROPHIC : */RenderType.RAY_TRACE,
				width,
				height);
		}


		public Image<Rgba32> Get3DThumbnail(IObject3D item, int width, int height)
		{
			ImageBuffer imageBuffer = GetThumbnail(item, width, height);
			var rotateAngle = 0;
			if (imageBuffer != null)
			{
				Image<Rgba32> bitmap = ImageIO.ImageBufferToImage32(imageBuffer);
				switch (rotateAngle)
				{
					case 90:
						//bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
						bitmap.Mutate(x => x.RotateFlip(RotateMode.Rotate90, FlipMode.None));
						break;
					case 180:
						//bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
						bitmap.Mutate(x => x.RotateFlip(RotateMode.Rotate180, FlipMode.None));
						break;
					case 270:
						//bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
						bitmap.Mutate(x => x.RotateFlip(RotateMode.Rotate270, FlipMode.None));
						break;
				}

				return bitmap;
			}
			return null;
		}

		public void SaveThumbnail(IObject3D item, int width, int height, string path)
		{
			ImageBuffer imageBuffer = GetThumbnail(item, width, height);

			if (imageBuffer != null)
			{
				ImageIO.SaveImageData(path, imageBuffer);
			}
		}
	}
}
