using ThumbnailGenerator;

// See https://aka.ms/new-console-template for more information
Console.WriteLine($"testing started {DateTime.Now},please wait!");
Test();
Console.ReadKey();

static async void Test()
{
	var stlPath = "../../../../../Resource/box.stl";
	var res=File.Exists(stlPath);
	await ThreeDThumbnailGenerator.GenerateAsync(
			stlPath,
			"test.png",
			1024, 1024, System.Drawing.Color.FromArgb(255, 235, 186, 123),
			GeneratorType.Cpu);
	Console.WriteLine($"thumbnail generated {DateTime.Now},you can close it now.");
}