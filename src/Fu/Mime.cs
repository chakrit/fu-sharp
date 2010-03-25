
using System.IO;

using NetMime = System.Net.Mime.MediaTypeNames;

namespace Fu
{
    // TODO: See System.Net.Mime, re-use stuff from there
    public static class Mime
    {
        public const string TextHtml = NetMime.Text.Html;
        public const string TextXml = NetMime.Text.Xml;
        public const string TextPlain = NetMime.Text.Plain;
        public const string TextRich = NetMime.Text.RichText;
        public const string TextCss = "text/css";
        public const string TextJavaScript = "text/javascript";

        public const string AppJson = "application/json";
        public const string AppOctetStream = NetMime.Application.Octet;
        public const string AppPdf = NetMime.Application.Pdf;
        public const string AppSoap = NetMime.Application.Soap;
        public const string AppZip = NetMime.Application.Zip;

        public const string ImagePng = "image/png";
        public const string ImageJpeg = NetMime.Image.Jpeg;
        public const string ImageGif = NetMime.Image.Gif;
        public const string ImageTiff = NetMime.Image.Tiff;
        public const string ImageBmp = "image/bmp";
        public const string ImageIco = "image/vnd.microsoft.icon"; // per IANA records


        public static string FromFilename(string filename)
        { return FromExtension(Path.GetExtension(filename)); }

        public static string FromExtension(string fileExt)
        {
            fileExt = fileExt.ToLower();
            if (fileExt.StartsWith("."))
                fileExt = fileExt.Substring(1);

            switch (fileExt.ToLower())
            {
                case "htm":
                case "html":
                case "xhtml":
                case "asp":
                case "aspx":
                case "php":
                    return TextHtml;

                case "jpeg":
                case "jpg":
                    return ImageJpeg;

                case "gif": return ImageGif;
                case "png": return ImagePng;
                case "bmp": return ImageBmp;
                case "ico": return ImageIco;
                case "tiff": return ImageTiff;

                case "xml": return TextXml;
                case "txt": return TextPlain;
                case "css": return TextCss;
                case "js": return TextJavaScript;

                case "pdf": return AppPdf;
                case "zip": return AppZip;
                case "json": return AppJson;

                case "exe":
                case "msi":
                case "dll":
                default:
                    return AppOctetStream;

            }
        }
    }
}
