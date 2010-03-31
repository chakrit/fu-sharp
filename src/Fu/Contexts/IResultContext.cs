
using Fu.Results;

namespace Fu.Contexts
{
  public interface IResultContext : IFuContext
  {
    IResult Result { get; }
  }
}
