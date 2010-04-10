
using System.Linq;

using Fu.Contexts;
using Fu.Results;

namespace Fu
{
  // functional-style utilities
  public static partial class fu
  {
    public static Continuation Compose(Continuation[] src, params Continuation[] conts)
    { return fu.Compose(fu.Compose(src), fu.Compose(conts)); }

    public static Continuation Compose(params Continuation[] conts)
    {
      var reverse = conts.Reverse().ToArray();
      return step => reverse.Aggregate(step, (s, cont) => cont(s));
    }


    public static Continuation If(Reduce<bool> predicate, Continuation trueStep)
    {
      return fu.If(predicate, trueStep, fu.Identity);
    }

    public static Continuation If(Reduce<bool> predicate,
      Continuation trueCont, Continuation falseCont)
    {
      return step => ctx =>
      {
        if (predicate(ctx))
          trueCont(step)(ctx);
        else
          falseCont(step)(ctx);
      };
    }


    public static Continuation Then(this Continuation cont, Continuation then)
    {
      return step => ctx => cont(then(step))(ctx);
    }

    public static Continuation Then(this Continuation cont, params Continuation[] conts)
    {
      return cont.Then(fu.Compose(conts));
    }


    public static Continuation Action(FuAction act)
    {
      return step => ctx => { act(ctx); step(ctx); };
    }


    public static Continuation Results(IResult result)
    {
      return Results(c => result);
    }

    public static Continuation Results(Reduce<IResult> resultReducer)
    {
      return step => ctx =>
      {
        var result = resultReducer(ctx);
        var newCtx = new ResultContext(ctx, result);

        step(newCtx);
      };
    }
  }
}
