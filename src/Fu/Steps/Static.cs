
using System.IO;

using Fu.Results;
using Fu.Services;

namespace Fu.Steps
{
    public static partial class Static
    {
        public static Step Text(this IStaticSteps _, string text)
        { return ctx => StringResult.From(ctx, text); }

        // TODO: Convert this to *not* use overloads to allow better
        //       composition
        // TODO: Add automatic caching, pre-execute the result
        //       or maybe wraps it in a memorizing step
        public static Step File(this IStaticSteps _, string filename)
        { return _.File(Mime.FromFilename(filename), filename); }

        public static Step File(this IStaticSteps _, string contentType, string filename)
        { return fu.Results(c => new FileResult(filename) { MediaType = contentType }); }


        // TODO: Remove the automatic wrapping of "/" to urlPrefixes
        //       Maybe implements a proper appPath/virtualpath mapping system?
        public static Step Folder(this IStaticSteps _, string folder)
        { return _.Folder(string.IsNullOrEmpty(folder) ? "/" : "/" + folder + "/", folder, null); }

        public static Step Folder(this IStaticSteps _, string folder, Step step404)
        { return _.Folder(string.IsNullOrEmpty(folder) ? "/" : "/" + folder + "/", folder, step404); }

        public static Step Folder(this IStaticSteps _, string urlPrefix, string folder)
        { return _.Folder(urlPrefix, folder, null); }

        public static Step Folder(this IStaticSteps _,
            string urlPrefix, string folder, Step step404)
        {
            step404 = step404 ?? fu.Http.NotFound();

            // TODO: Detect Urls-mapped context and if it is,
            //       use that as the basis for appPath instead.
            return c =>
            {
                var rawUrl = c.Request.Url.AbsolutePath;
                var folderPath = c.ResolvePath(folder);

                if (!rawUrl.StartsWith(urlPrefix))
                    return step404(c);

                var url = rawUrl.Substring(urlPrefix.Length);
                var filePath = Path.Combine(folderPath, url);

                if (!System.IO.File.Exists(filePath))
                    return step404(c);

                return FileResult.From(c, filePath);
            };
        }
    }
}
