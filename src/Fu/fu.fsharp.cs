
using Microsoft.FSharp.Core;

// damn the Units !
// FSAction = void()
using FSAction = Microsoft.FSharp.Core.FSharpFunc<Fu.IFuContext, Microsoft.FSharp.Core.Unit>;

// FSContinuation == FSharpFunc<FSAction, FSAction>
using FSContinuation = Microsoft.FSharp.Core.FSharpFunc<Microsoft.FSharp.Core.FSharpFunc<Fu.IFuContext, Microsoft.FSharp.Core.Unit>, Microsoft.FSharp.Core.FSharpFunc<Fu.IFuContext, Microsoft.FSharp.Core.Unit>>;

namespace Fu
{
  // fu.fsharp.cs - Provides utilities for interoperating with F# apps
  public static partial class fu
  {
    public static Continuation FromFSharp(FSContinuation cont)
    {
      return step => ctx => cont.Invoke(fu.ToFSharp(step)).Invoke(ctx);
    }

    public static FuAction FromFSharp(FSAction act)
    {
      return ctx => act.Invoke(ctx);
    }


    public static FSContinuation ToFSharp(Continuation cont)
    {
      return FuncConvert.ToFSharpFunc<FSAction, FSAction>(next =>
      {
        var nextAct = fu.FromFSharp(next);
        nextAct = cont(nextAct);

        return fu.ToFSharp(nextAct);
      });
    }

    public static FSAction ToFSharp(FuAction act)
    {
      return FuncConvert.ToFSharpFunc<IFuContext>(c_ => act(c_));
    }
  }
}
