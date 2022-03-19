using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
	public class ThreeDThumbnailGenerator
	{
		public static async Task<Object3DInfo> GenerateAsync(
			string srcPath,
			string targetPath,
			int width,
			int height,
			System.Drawing.Color color,
			GeneratorType type)
		{
			if (File.Exists(targetPath))
				File.Delete(targetPath);
			switch (type)
			{
				case GeneratorType.Cpu:
					return await Task.Run(() =>
					{
						GeneratorCpu generator = new();
						var ob = Object3D.Load(srcPath, CancellationToken.None);
                        if (ob != null&& ob.Mesh != null)
						{
							var box = ob.GetAxisAlignedBoundingBox();
						    ob.Color = new MatterHackers.Agg.Color(color.R, color.G, color.B);
						    generator.SaveThumbnail(ob, width, height, targetPath);
						    return new Object3DInfo
						    {
								Dimension = new[] { (float)box.XSize, (float)box.YSize, (float)box.ZSize }
					        };
						}						
						return null;							
					});	

				case GeneratorType.OpenGL:
					break;
				default:
					break;
			}
			return null;
		}
	}

	public enum GeneratorType
	{
		Cpu,
		OpenGL
	}

	/// <summary>
	/// reserve
	/// </summary>
	public class Object3DInfo
    {
		public float[] Dimension { get; set; }
		//public string HashValue { get; set; }
	}
}
