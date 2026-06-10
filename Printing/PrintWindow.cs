using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using ThoughtWorks.QRCode.Codec;

namespace Printing
{
    public class PrintWindow
    {
        private PrintModel printModel;
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        //从图左上角裁剪600*220，裁剪后的图片将显示在destinationRect框中
        RectangleF sourceRect = new RectangleF(0, 0, 1200, 440);
        //裁剪后图片显示框，如果框大于裁剪图片，则裁剪图片将拉伸
        RectangleF destinationRect = new RectangleF(0, 0, 1200, 440);
        private QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
        private  PrintSettingConfig printSettingConfig = null;

        public event EventHandler<PrintModel> DocumentPrintedEvent;

        public PrintWindow(PrintSettingConfig config)
        {
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 5;
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            printSettingConfig = config;
            
            //if (PrintHelper.strType == "HMK-825")
            //{
            //    printDocument.PrinterSettings.PrinterName = "HMK-825";
            //}
        }

        public void Print(PrintModel printModel)
        {
            using (PrintDocument printDocument = new PrintDocument())
            {
                this.printModel = printModel;

                printDocument.PrintController = new StandardPrintController();
                printDocument.PrintPage += new PrintPageEventHandler(OnPrintPage);
				printDocument.EndPrint += OnDocumentEndPrint;

                printDocument.Print();
            }
        }

		private void OnDocumentEndPrint(object sender, PrintEventArgs e)
		{
            DocumentPrintedEvent?.Invoke(this, printModel);
        }

		#region Private Methods

		private void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Point;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            var settings = GetConfiguredPrintSettings(printSettingConfig);
            e.Graphics.ScaleTransform(settings.Scale, settings.Scale);
            e.Graphics.TranslateTransform(settings.LeftMargin, settings.TopMargin, System.Drawing.Drawing2D.MatrixOrder.Prepend);

            ProcessPrintModel(e);
        }

        private void ProcessPrintModel(PrintPageEventArgs e)
        {
            XDocument xdoc = printModel.XDocument;
            foreach (XElement xe in xdoc.Root.Elements())
            {
                if (IsVisible(xe) == false)
                    continue;

                if (xe.Name.LocalName == "Label")
                {
                    DrawString(xe, e);
                }
                else if (xe.Name.LocalName == "Image")
                {
                    DrawImage(xe, e);
                }
                else if (xe.Name.LocalName == "Canvas.Background")
                {
                    DrawBackgroundImage(xe, e);
                }
                else if (xe.Name.LocalName == "TextBlock")
                {
                    DrawTextBlock(xe, e);
                }
            }
        }

        private void DrawTextBlock(XElement xelement, PrintPageEventArgs e)
        {

            String drawString = xelement.Attribute("Text")?.Value;
            if (string.IsNullOrEmpty(drawString))
                return;

            string fontFamily = GetFontFamily(xelement);
            float fontSize = GetNumberValue(xelement, "FontSize");
            FontStyle fontStyle = GetFontStyle(xelement);
            Font drawFont = new Font(fontFamily, fontSize, fontStyle);

            // Create point for upper-left corner of drawing.
            float left = GetNumberValue(xelement, "Canvas.Left");
            float top = GetNumberValue(xelement, "Canvas.Top");
            PointF drawPoint = new PointF(left, top);

            XAttribute xback = xelement.Attribute("Background");
            //换行
            if (drawString.Length > 18)
            {

            }
            if (xback != null) //带背景的字符串
            {
                SizeF stringSize = new SizeF();
                //测量整个字符串宽高
                stringSize = e.Graphics.MeasureString(drawString, drawFont);
                //画背景矩形(黑色画刷)
                e.Graphics.FillRectangle(blackBrush, left, top, stringSize.Width, stringSize.Height);
                //画字符串(白色画刷)
                e.Graphics.DrawString(drawString, drawFont, whiteBrush, drawPoint);
            }
            else //无背景
                e.Graphics.DrawString(drawString, drawFont, blackBrush, drawPoint);
        }

        /// <summary>
        /// 画字符串
        /// </summary>
        /// <param name="xelement"></param>
        /// <param name="e"></param>
        private void DrawString(XElement xelement, PrintPageEventArgs e)
        {
            // xelement.Attribute("Text").Value;
            String drawString = xelement.Value;
            if (string.IsNullOrEmpty(drawString))
                return;

            string fontFamily = GetFontFamily(xelement);
            float fontSize = GetNumberValue(xelement, "FontSize");
            FontStyle fontStyle = GetFontStyle(xelement);
            Font drawFont = new Font(fontFamily, fontSize, fontStyle);

            // Create point for upper-left corner of drawing.
            float left = GetNumberValue(xelement, "Canvas.Left");
            float top = GetNumberValue(xelement, "Canvas.Top");
            PointF drawPoint = new PointF(left, top);

            XAttribute xback = xelement.Attribute("Background");
            if (xback != null) //带背景的字符串
            {
                SizeF stringSize = new SizeF();
                //测量整个字符串宽高
                stringSize = e.Graphics.MeasureString(drawString, drawFont);
                //画背景矩形(黑色画刷)
                e.Graphics.FillRectangle(blackBrush, left, top, stringSize.Width, stringSize.Height);
                //画字符串(白色画刷)
                e.Graphics.DrawString(drawString, drawFont, whiteBrush, drawPoint);
            }
            else //无背景
                e.Graphics.DrawString(drawString, drawFont, blackBrush, drawPoint);
        }

        private void DrawImage(XElement xelement, PrintPageEventArgs e)
        {
            Image bitmap = null;
            bool isQrcode = false;
            string attName = xelement.Attribute("Name").Value;

            string imgName = null;

            if (attName.Contains("two_")) //二维码
            {
                if (!printModel.ImageSetting.ContainsKey(attName))
                    return;
                imgName = printModel.ImageSetting[attName];

                if (imgName.EndsWith(".png") || imgName.EndsWith(".jpg"))
                {
                    MemoryStream stream = new MemoryStream(new WebClient().DownloadData(imgName));
                    bitmap = Image.FromStream(stream);
                }
                else
                {
                    isQrcode = true;
                    bitmap = qrCodeEncoder.Encode(imgName, Encoding.Default);
                }
            }
            else //一般图片
            {
                MemoryStream stream;
                string imageUrl = xelement.Attribute("Source").Value;
				if (attName.StartsWith("Img_areaIcon_"))
				{
					stream = new MemoryStream();
					JpegBitmapEncoder encoder = new JpegBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(new BitmapImage(new Uri(imageUrl))));
					encoder.Save(stream);
				}
				else
				{
					stream = new MemoryStream(new WebClient().DownloadData(imageUrl));
				}
				bitmap = Image.FromStream(stream);
            }

            float left = GetNumberValue(xelement, "Canvas.Left");
            float top = GetNumberValue(xelement, "Canvas.Top");
            float width = GetNumberValue(xelement, "Width");
            float heigh = GetNumberValue(xelement, "Height");

            if (width == 0)
            {
                if (heigh == 0)
                {
                    width = bitmap.Width;
                }
                else
                {
                    width = bitmap.Width * (heigh / bitmap.Height);
                }

            }
            if (heigh == 0)
            {
                if (width == 0)
                {
                    heigh = bitmap.Height;
                }
                else
                {
                    heigh = bitmap.Height * (width / bitmap.Width);
                }
            }

            if (isQrcode && (width != heigh))
            {
                if (heigh > width)
                {
                    top += (heigh - width) / 2;
                    heigh = width;
                }
            }
            e.Graphics.DrawImage(bitmap, left, top, width, heigh);
        }

        private void DrawBackgroundImage(XElement xelement, PrintPageEventArgs e)
        {
            XElement firstNode = xelement.FirstNode as XElement;
            string imageUrl = firstNode.Attribute("ImageSource").Value;
            if(imageUrl == "{x:Null}")
			{
                return;
			}
            MemoryStream stream = new MemoryStream(new WebClient().DownloadData(imageUrl));
            Image image = Image.FromStream(stream);
            //先等比缩放原图 注：因等比，缩放结果有可能不是600 * 220
            image = MakeThumbnail(image, 600, 220);
            //裁剪并绘制图片        
            e.Graphics.DrawImage(image, destinationRect, sourceRect, GraphicsUnit.Pixel);
        }

		#endregion

		#region Utilities

		private (float LeftMargin, float TopMargin, float Scale) GetConfiguredPrintSettings(PrintSettingConfig config)
		{
            if(config == null)
			{
                return (0F, 0F, 1F);
			}

            if (!float.TryParse(config.Scale, out var scale))
			{
                scale = 1F;
			}

            if (float.TryParse(config.LeftMargin, out var left))
			{
                if(float.TryParse(config.TopMargin, out var top))
				{
                    return (left, top, scale);
				}
				else
				{
                    return (left, 0F, scale);
                }
			}
            else
			{
                if (float.TryParse(config.TopMargin, out var top))
                {
                    return (0F, top, scale);
                }
                else
                {
                    return (0F, 0F, scale);
                }
            }
        }

        private System.Drawing.Image MakeThumbnail(Image img, int width, int height)
        {
            int towidth = width; int toheight = height;
            int x = 0; int y = 0; int ow = img.Width;
            int oh = img.Height;

            //按宽缩放
            if (img.Width > towidth)
            {
                toheight = img.Height * width / img.Width;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(img, new System.Drawing.Rectangle(0, 0, towidth, toheight),
            new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            g.Dispose();
            return bitmap;
        }

        private string GetFontFamily(XElement xelement)
        {
            XAttribute xatt = xelement.Attribute("FontFamily");
            if (xatt == null)
                return "微软雅黑";
            else
                return xatt.Value;
        }

        private FontStyle GetFontStyle(XElement xelement)
        {
            FontStyle fontStyle = FontStyle.Regular;
            XAttribute xatt = xelement.Attribute("FontStyle");
            if (xatt != null && xatt.Value == "Italic")
                fontStyle = FontStyle.Italic;

            xatt = xelement.Attribute("FontWeight");
            if (xatt != null && xatt.Value == "Bold")
                fontStyle |= FontStyle.Bold;

            return fontStyle;
        }

        private bool IsVisible(XElement xelement)
        {
            XAttribute xatt = xelement.Attribute("Visibility");
            if (xatt == null)
                return true;

            if (xatt.Value != "Visible")
                return false;

            return true;
        }

        private float GetNumberValue(XElement xelement, string attName)
        {
            XAttribute xatt = xelement.Attribute(attName);
            if (xatt == null)
                return 0;
            else if (xatt.Value == "Auto")
                return 0;
            else
                return float.Parse(xatt.Value);
        }

        #endregion
    }
}
