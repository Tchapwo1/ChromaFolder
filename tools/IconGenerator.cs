using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace FolderColorizer.Tools
{
    public class IconGenerator
    {
        private static readonly int[] Sizes = new int[] { 16, 24, 32, 48, 64, 128, 256 };

        public static void Main(string[] args)
        {
            string outDir = args.Length > 0 ? args[0] : @"D:\Downloads\FolderColorizer\src\Assets";
            Directory.CreateDirectory(outDir);

            // Predefined PRD colors
            GenerateFolderIcon("folder_red.ico", Color.FromArgb(220, 38, 38), outDir);
            GenerateFolderIcon("folder_blue.ico", Color.FromArgb(37, 99, 235), outDir);
            GenerateFolderIcon("folder_green.ico", Color.FromArgb(22, 163, 74), outDir);
            GenerateFolderIcon("folder_yellow.ico", Color.FromArgb(234, 179, 8), outDir);
            GenerateFolderIcon("folder_orange.ico", Color.FromArgb(234, 88, 12), outDir);
            GenerateFolderIcon("folder_purple.ico", Color.FromArgb(147, 51, 234), outDir);

            // App main icon (Blue folder with color dots / badge)
            GenerateAppIcon("app_icon.ico", outDir);

            Console.WriteLine("All 6 color icons + app icon successfully generated in: " + outDir);
        }

        public static void GenerateFolderIcon(string filename, Color primaryColor, string outDir)
        {
            byte[][] pngImages = new byte[Sizes.Length][];

            for (int i = 0; i < Sizes.Length; i++)
            {
                int s = Sizes[i];
                using (Bitmap bmp = RenderFolderBitmap(s, primaryColor, false))
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    pngImages[i] = ms.ToArray();
                }
            }

            WriteIcoFile(Path.Combine(outDir, filename), pngImages);
            Console.WriteLine("Created: " + filename);
        }

        public static void GenerateAppIcon(string filename, string outDir)
        {
            byte[][] pngImages = new byte[Sizes.Length][];

            for (int i = 0; i < Sizes.Length; i++)
            {
                int s = Sizes[i];
                using (Bitmap bmp = RenderFolderBitmap(s, Color.FromArgb(37, 99, 235), true))
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    pngImages[i] = ms.ToArray();
                }
            }

            WriteIcoFile(Path.Combine(outDir, filename), pngImages);
            Console.WriteLine("Created: " + filename);
        }

        private static void WriteIcoFile(string targetPath, byte[][] pngImages)
        {
            using (FileStream fs = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                // ICONDIR header (6 bytes)
                bw.Write((ushort)0); // reserved
                bw.Write((ushort)1); // type 1 = icon
                bw.Write((ushort)Sizes.Length); // count

                int offset = 6 + (16 * Sizes.Length);

                for (int i = 0; i < Sizes.Length; i++)
                {
                    int s = Sizes[i];
                    bw.Write((byte)(s >= 256 ? 0 : s)); // width
                    bw.Write((byte)(s >= 256 ? 0 : s)); // height
                    bw.Write((byte)0); // color count
                    bw.Write((byte)0); // reserved
                    bw.Write((ushort)1); // color planes
                    bw.Write((ushort)32); // bits per pixel
                    bw.Write((uint)pngImages[i].Length); // bytes in res
                    bw.Write((uint)offset); // offset

                    offset += pngImages[i].Length;
                }

                for (int i = 0; i < Sizes.Length; i++)
                {
                    bw.Write(pngImages[i]);
                }
            }
        }

        private static Bitmap RenderFolderBitmap(int size, Color primary, bool isAppIcon)
        {
            Bitmap bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);

                float s = size;
                Color darkShade = MultiplyColor(primary, 0.72f);
                Color lightShade = LightenColor(primary, 0.22f);

                // Dimensions
                float marginX = s * 0.08f;
                float width = s * 0.84f;
                float tabWidth = width * 0.45f;
                float tabHeight = s * 0.16f;
                float bodyY = s * 0.22f;
                float bodyH = s * 0.66f;
                float flapY = s * 0.35f;
                float flapH = s * 0.53f;
                float rad = Math.Max(1.5f, s * 0.06f);

                // 1. Back Tab & Plate
                using (GraphicsPath backPath = new GraphicsPath())
                {
                    // Rounded tab + back rect
                    AddRoundedPath(backPath, marginX, bodyY - tabHeight * 0.5f, tabWidth, tabHeight + 2, rad * 0.8f);
                    AddRoundedPath(backPath, marginX, bodyY, width, bodyH, rad);

                    using (Brush backBrush = new SolidBrush(darkShade))
                    {
                        g.FillPath(backBrush, backPath);
                    }
                }

                // 2. Paper Sheet Peek (inner folder content)
                if (size >= 24)
                {
                    float paperX = marginX + width * 0.08f;
                    float paperW = width * 0.84f;
                    float paperY = bodyY + s * 0.02f;
                    float paperH = flapY - paperY + s * 0.06f;
                    using (GraphicsPath paperPath = new GraphicsPath())
                    {
                        AddRoundedPath(paperPath, paperX, paperY, paperW, paperH, rad * 0.5f);
                        using (Brush paperBrush = new SolidBrush(Color.FromArgb(240, 243, 246)))
                        {
                            g.FillPath(paperBrush, paperPath);
                        }
                    }
                }

                // 3. Drop Shadow behind front flap
                if (size >= 32)
                {
                    using (GraphicsPath shadowPath = new GraphicsPath())
                    {
                        AddRoundedPath(shadowPath, marginX, flapY - 1, width, flapH + 2, rad);
                        using (PathGradientBrush pgb = new PathGradientBrush(shadowPath))
                        {
                            pgb.CenterColor = Color.FromArgb(70, 0, 0, 0);
                            pgb.SurroundColors = new Color[] { Color.Transparent };
                            g.FillPath(pgb, shadowPath);
                        }
                    }
                }

                // 4. Front Flap
                using (GraphicsPath flapPath = new GraphicsPath())
                {
                    AddRoundedPath(flapPath, marginX, flapY, width, flapH, rad);

                    using (LinearGradientBrush flapBrush = new LinearGradientBrush(
                        new PointF(marginX, flapY),
                        new PointF(marginX, flapY + flapH),
                        lightShade,
                        primary))
                    {
                        g.FillPath(flapBrush, flapPath);
                    }

                    // Top rim optical highlight
                    if (size >= 24)
                    {
                        using (Pen rimPen = new Pen(Color.FromArgb(120, 255, 255, 255), Math.Max(1f, s * 0.015f)))
                        {
                            g.DrawLine(rimPen, marginX + rad, flapY + 0.5f, marginX + width - rad, flapY + 0.5f);
                        }
                    }

                    // Outer subtle boundary pen
                    using (Pen borderPen = new Pen(MultiplyColor(primary, 0.85f), Math.Max(1f, s * 0.012f)))
                    {
                        g.DrawPath(borderPen, flapPath);
                    }
                }

                // 5. If App Icon, draw decorative color palette badge on the front flap
                if (isAppIcon && size >= 32)
                {
                    Color[] dots = new Color[]
                    {
                        Color.FromArgb(220, 38, 38),  // Red
                        Color.FromArgb(234, 179, 8),  // Yellow
                        Color.FromArgb(22, 163, 74),  // Green
                        Color.FromArgb(147, 51, 234)  // Purple
                    };

                    float dotR = s * 0.055f;
                    float dotCenterY = flapY + flapH * 0.55f;
                    float startX = marginX + width * 0.5f - (dots.Length * dotR * 1.5f * 0.5f);

                    for (int d = 0; d < dots.Length; d++)
                    {
                        float cx = startX + (d * dotR * 2.3f);
                        using (Brush dotBrush = new SolidBrush(dots[d]))
                        using (Pen dotPen = new Pen(Color.White, Math.Max(1f, s * 0.015f)))
                        {
                            g.FillEllipse(dotBrush, cx - dotR, dotCenterY - dotR, dotR * 2, dotR * 2);
                            g.DrawEllipse(dotPen, cx - dotR, dotCenterY - dotR, dotR * 2, dotR * 2);
                        }
                    }
                }
            }

            return bmp;
        }

        private static void AddRoundedPath(GraphicsPath path, float x, float y, float w, float h, float r)
        {
            if (r <= 0)
            {
                path.AddRectangle(new RectangleF(x, y, w, h));
                return;
            }
            float d = r * 2;
            path.AddArc(x, y, d, d, 180, 90);
            path.AddArc(x + w - d, y, d, d, 270, 90);
            path.AddArc(x + w - d, y + h - d, d, d, 0, 90);
            path.AddArc(x, y + h - d, d, d, 90, 90);
            path.CloseFigure();
        }

        private static Color MultiplyColor(Color c, float factor)
        {
            int r = Math.Min(255, Math.Max(0, (int)(c.R * factor)));
            int g = Math.Min(255, Math.Max(0, (int)(c.G * factor)));
            int b = Math.Min(255, Math.Max(0, (int)(c.B * factor)));
            return Color.FromArgb(c.A, r, g, b);
        }

        private static Color LightenColor(Color c, float factor)
        {
            int r = Math.Min(255, Math.Max(0, c.R + (int)((255 - c.R) * factor)));
            int g = Math.Min(255, Math.Max(0, c.G + (int)((255 - c.G) * factor)));
            int b = Math.Min(255, Math.Max(0, c.B + (int)((255 - c.B) * factor)));
            return Color.FromArgb(c.A, r, g, b);
        }
    }
}
