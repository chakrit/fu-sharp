
using System;

using Fu.Contexts;
using Fu.Results;

namespace Fu.Steps
{
    public static class Compress
    {
        public static Step ByExtension(this ICompressSteps _,
            string extension, Func<string, string> compressor)
        {
            return fu.If<IResultContext>(
                c => c.Request.Url.AbsolutePath.EndsWith(extension),
                _.With(compressor));
        }

        public static Step With(this ICompressSteps _,
            Func<string, string> compressor)
        {
            return fu.Results<IResultContext>(c =>
                new CompressedResult(c.Result, compressor));
        }
    }
}
