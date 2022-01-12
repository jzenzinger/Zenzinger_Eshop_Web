using System;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Zenzinger_Eshop_Web.Models.Extensions
{
    public static class CreatePdfExtension
    {
        public static void CreatePdf(string order, int orderPrice, string userName, string userEmail, string billAddress, string productInfo)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Console.WriteLine("Generating PDF...");
            
            // Create a new PDF document
            var document = new PdfDocument();
            document.Info.Title = "Invoice - Order number: " + order;

            // Create an empty page
            var page = document.AddPage();

            // Get an XGraphics object for drawing
            var gfx = XGraphics.FromPdfPage(page);

            // Create a font
            var font = new XFont("Arial", 20, XFontStyle.BoldItalic);

            // Draw the text
            gfx.DrawString(productInfo, font, XBrushes.Black,
                new XRect(0, 0, page.Width, page.Height),
                XStringFormats.Center);

            // Save the document...
            var filename = order + ".pdf";
            // Sent filename by email
            // Call static function in SendEmailExtension
            document.Save(filename);
            Console.WriteLine("PDF Generated!");
        }
    }
}