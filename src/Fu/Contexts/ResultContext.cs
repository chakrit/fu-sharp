
using Fu.Results;

namespace Fu.Contexts
{
    public class ResultContext : FuContext, IResultContext
    {
        public IResult Result { get; private set; }

        public ResultContext(IFuContext context, IResult result) :
            base(context)
        {
            Result = result;
        }
    }
}
