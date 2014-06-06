
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Inedo.Iconator;

namespace _2icon
{
  internal class Program
  {
    [STAThread]
    private static int Main(string[] args)
    {
      Console.Out.WriteLine("2icon.exe --output:file.ico image...");
      if (args.Length <= 1)
      {
        Console.Out.WriteLine("Invalid arguments");
        return 1;
      }

      var dest = args[0];

      if (!dest.StartsWith("--output:"))
      {
        Console.Out.WriteLine("Are you mixing input and output? Output file name must start with `--output:`");
        return 1;
      }
      dest = dest.Substring("--output:".Length);

      if (File.Exists(dest)) File.Delete(dest);

      var images = new List<Bitmap>();
      foreach (var arg in args.Skip(1))
      {

        var argIcon = arg + ".ico";

        var img = new Bitmap(Image.FromFile(arg));
        img.Save(argIcon, ImageFormat.Icon);

        images.Add(img);
      }

      var iif = new IconFile();
      foreach (var img in images.OrderBy(x => x.Width))
      {
        iif.Images.Add(img.CreateBitmapSourceFromBitmap());
      }
     
      iif.Save(dest);
      return 0;
    }
  }

  public static class Imaging
  {
    public static BitmapSource CreateBitmapSourceFromBitmap(this Bitmap bitmap)
    {
      if (bitmap == null)
        throw new ArgumentNullException("bitmap");

      return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
          bitmap.GetHbitmap(),
          IntPtr.Zero,
          Int32Rect.Empty,
          BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));
    }
  }
}
