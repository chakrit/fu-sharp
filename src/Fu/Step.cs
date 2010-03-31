
using Fu.Results;

namespace Fu
{
  // NOTE: Some delegate is intended to function exactly the same
  //       they're provided for convenience only


  // Main building block
  public delegate IFuContext Step(IFuContext input);


  // Variations to provide FuContext derivation on inputs/outputs
  public delegate TOut Step<TIn, TOut>(TIn input)
    where TIn : IFuContext
    where TOut : IFuContext;

  public delegate IFuContext Step<TIn>(TIn input)
    where TIn : IFuContext;


  public delegate void Void(IFuContext input);
  public delegate void Void<TIn>(TIn input)
    where TIn : IFuContext;


  // Utility delegates
  public delegate TFilter Filter<TFilter>(IFuContext context, TFilter input);
  public delegate TFilter Filter<TIn, TFilter>(TIn context, TFilter input)
    where TIn : IFuContext;

  public delegate TResult Reduce<TResult>(IFuContext context);
  public delegate TResult Reduce<TIn, TResult>(TIn context)
    where TIn : IFuContext;

}
