using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;



// Define the paths for the input and output images
string inputImagePath = Path.Combine("example", "example.jpg");
string outputImagePath = Path.Combine("example", "out.jpg");

ImagePreprocessor imageProcessor = new ImagePreprocessor();
// imageProcessor.LoadImageByPath(inputImagePath);
// imageProcessor.Resize();
// imageProcessor.DrawRectanglesAndLabels();
// imageProcessor.SaveImage(outputImagePath);

// Test loading from stream
Console.WriteLine("\nTesting loading from stream:");
using (FileStream fileStream = new FileStream(inputImagePath, FileMode.Open, FileAccess.Read))
{
    imageProcessor.LoadImage(fileStream);
    imageProcessor.DrawRectanglesAndLabels();
    imageProcessor.Resize();
}
imageProcessor.SaveImage(outputImagePath);

Console.WriteLine("Image saved as out.jpg in the example folder.");