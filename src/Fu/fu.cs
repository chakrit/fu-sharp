
using Fu.Contexts;
using Fu.Exceptions;
using Fu.Results;

namespace Fu
{
  // Lambda conversion helpers
  // TODO: Add error handling for potential cast failure
  public static partial class fu
  {
    public static readonly FuAction End = c => { };


    public static Step Step<TIn, TOut>(Step<TIn, TOut> step)
      where TIn : IFuContext
      where TOut : IFuContext
    {
      return ctx =>
      {
        FuTrace.Step(step);
        return step(fu.Cast<TIn>(ctx));
      };
    }

    public static Step Step<TIn>(Step<TIn> step)
      where TIn : IFuContext
    {
      return ctx =>
      {
        FuTrace.Step(step);
        return step(fu.Cast<TIn>(ctx));
      };
    }


    public static Step Void(Void voidStep)
    {
      return ctx =>
      {
        FuTrace.Step(voidStep);
        voidStep(ctx);
        return ctx;
      };
    }

    public static Step Void<TIn>(Void<TIn> voidStep)
      where TIn : IFuContext
    {
      return ctx =>
      {
        FuTrace.Step(voidStep);
        voidStep(fu.Cast<TIn>(ctx));
        return ctx;
      };
    }


    public static Step Results(Reduce<IResult> resultStep)
    {
      return ctx =>
      {
        // TODO: Should this return IResultContext consistently instead of null?
        FuTrace.Step(resultStep);
        var result = resultStep(ctx);
        return result == null ? ctx :
          new ResultContext(ctx, result);
      };
    }

    public static Step Results<TIn>(Reduce<TIn, IResult> resultStep)
      where TIn : IFuContext
    {
      return ctx =>
      {
        FuTrace.Step(resultStep);
        var result = resultStep(fu.Cast<TIn>(ctx));
        return result == null ? ctx :
          new ResultContext(ctx, result);
      };
    }


    public static T Cast<T>(IFuContext c)
        where T : IFuContext
    {
      if (c is T)
        return (T)c;

      // TODO: Add more information to the exception
      //       e.g. what steps were executing
      throw new MismatchedContextTypeException(
        c.GetType(), typeof(T));
    }
  }
}
