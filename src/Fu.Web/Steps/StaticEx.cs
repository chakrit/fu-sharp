
using Fu.Results;

namespace Fu.Steps
{
  public static class StaticEx
  {
    public static Continuation Json(this IStaticSteps _, object obj)
    {
      return _.Json(ctx => obj);
    }

    public static Continuation Json(this IStaticSteps _, Reduce<object> objReducer)
    {
      return step => ctx => step(JsonResult.From(ctx, objReducer(ctx)));
    }
  }
}
