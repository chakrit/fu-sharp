
using System.IO;

namespace Fu
{
    // TODO: See System.Net.Mime, re-use stuff from there
    public static class Mime
    {
        public const string TextHtml = "text/html";
        public const string TextXml = "text/xml";
        public const string TextPlain = "text/plain";
        public const string TextCss = "text/css";
        public const string TextJavaScript = "text/javascript";

        public const string AppJson = "application/json";
        public const string AppOctetStream = "application/octet-stream";

        public const string ImagePng = "image/png";
        public const string ImageJpeg = "image/jpeg";
        public const string ImageGif = "image/gif";
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
                    return TextHtml;

                case "jpeg":
                case "jpg":
                    return ImageJpeg;

                case "gif": return ImageGif;
                case "png": return ImagePng;
                case "bmp": return ImageBmp;
                case "ico": return ImageIco;

                case "xml": return TextXml;
                case "txt": return TextPlain;
                case "css": return TextCss;
                case "js": return TextJavaScript;

                default: return AppOctetStream;

            }
        }
    }
}
