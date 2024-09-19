using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

public class ImagePreprocessor
{
    public Image<Bgr, byte>? Image { get; private set; }

    public void LoadImage(Stream stream)
    {
        Bitmap bitmap = new Bitmap(stream);
        Image = bitmap.ToImage<Bgr, byte>();
        // rotate image to original..
        Image = Image.Rotate(90, new Bgr(Color.White), false);
    }

    public void LoadImageByPath(string imagePath)
    {
        Image = new Image<Bgr, byte>(imagePath);
    }

    public void DrawRectanglesAndLabels()
    {
        if (Image == null)
        {
            throw new InvalidOperationException("Load an image before processing.");
        }

        // Get image dimensions
        int width = Image.Width;
        int height = Image.Height;

        // Calculate rectangle dimensions
        int rectWidth = width / 3;
        int rectHeight = height / 3;

        // Define colors for rectangles
        Bgr[] colors = new Bgr[]
        {
            new Bgr(Color.Red), new Bgr(Color.Green), new Bgr(Color.Blue),
            new Bgr(Color.Yellow), new Bgr(Color.Cyan), new Bgr(Color.Magenta),
            new Bgr(Color.Orange), new Bgr(Color.Pink), new Bgr(Color.Purple)
        };

        // Define labels for rectangles
        string[] labels = new string[]
        {
            "Top-Left", "Top-Middle", "Top-Right",
            "Middle-Left", "Middle-Middle", "Middle-Right",
            "Bottom-Left", "Bottom-Middle", "Bottom-Right"
        };

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Rectangle rect = new Rectangle(j * rectWidth, i * rectHeight, rectWidth, rectHeight);
                Image.Draw(rect, colors[i * 3 + j], 5);

                Point textPoint = new Point(j * rectWidth + 10, i * rectHeight + 30);

                // Add a filled black rectangle behind the text
                int baseline = 0;
                Size textSize = CvInvoke.GetTextSize(labels[i * 3 + j], FontFace.HersheySimplex, 0.8, 2, ref baseline);
                Rectangle textBackgroundRect = new Rectangle(textPoint.X - 5, textPoint.Y - textSize.Height - 5, textSize.Width + 10, textSize.Height + 10);
                Image.Draw(textBackgroundRect, new Bgr(Color.Black), -1); // -1 thickness means filled

                Image.Draw(labels[i * 3 + j], textPoint, FontFace.HersheySimplex, 0.8, new Bgr(Color.White), 2);
            }
        }
    }

    public void Resize()
    {
        if (Image == null)
        {
            throw new InvalidOperationException("Load an image before processing.");
        }

        int width = 400;
        float aspectRatio = (float)Image.Width / Image.Height;
        int height = (int)(width / aspectRatio);

        Image = Image.Resize(width, height, Inter.Linear);
    }

    public void SaveImage(string outputImagePath)
    {
        if (Image == null)
        {
            throw new InvalidOperationException("Load an image before saving.");
        }

        Image.Save(outputImagePath);
    }
}
