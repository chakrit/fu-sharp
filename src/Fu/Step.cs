
using System.IO;

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


    public delegate TOut Returns<TIn, TOut>(TIn input)
        where TIn : IFuContext
        where TOut : IFuContext;

    public delegate TOut Returns<TOut>(IFuContext input)
        where TOut : IFuContext;


    public delegate void Void(IFuContext input);
    public delegate void Void<TIn>(TIn input)
        where TIn : IFuContext;


    // Results framework support
    public delegate IResult ResultStep(IFuContext input);
    public delegate IResult ResultStep<TIn>(TIn input)
        where TIn : IFuContext;


    // Filter support
    public delegate TFilter FilterStep<TFilter>(IFuContext context, TFilter input);
    public delegate TFilter FilterStep<TIn, TFilter>(TIn context, TFilter input)
        where TIn : IFuContext;

}
