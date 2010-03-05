
using Fu.Contexts;
using Fu.Exceptions;

namespace Fu
{
    // Lambda conversion helpers
    // TODO: Add error handling for potential cast failure
    public static partial class fu
    {
        public static Step Step<TIn, TOut>(Step<TIn, TOut> step)
            where TIn : IFuContext
            where TOut : IFuContext
        {
            return ctx => step(fu.ConvertTo<TIn>(ctx));
        }

        public static Step Step<TIn>(Step<TIn> step)
            where TIn : IFuContext
        {
            return ctx => step(fu.ConvertTo<TIn>(ctx));
        }


        public static Step Returns<TIn, TOut>(Returns<TIn, TOut> resultStep)
            where TIn : IFuContext
            where TOut : IFuContext
        {
            return ctx => resultStep(fu.ConvertTo<TIn>(ctx));
        }

        public static Step Returns<TOut>(Returns<TOut> resultStep)
            where TOut : IFuContext
        {
            return ctx => resultStep(ctx);
        }


        public static Step Void(Void voidStep)
        {
            return ctx => { voidStep(ctx); return ctx; };
        }

        public static Step Void<TIn>(Void<TIn> voidStep)
            where TIn : IFuContext
        {
            return ctx => { voidStep(fu.ConvertTo<TIn>(ctx)); return ctx; };
        }


        public static Step Results(ResultStep resultStep)
        {
            return ctx =>
            {
                var result = resultStep(ctx);
                return result == null ? ctx :
                    new ResultContext(ctx, result);
            };
        }

        public static Step Results<TIn>(ResultStep<TIn> resultStep)
            where TIn : IFuContext
        {
            return ctx =>
            {
                var result = resultStep(fu.ConvertTo<TIn>(ctx));
                return result == null ? ctx :
                    new ResultContext(ctx, result);
            };
        }


        public static T ConvertTo<T>(IFuContext c)
            where T : IFuContext
        {
            if (c is T) return (T)c;

            // TODO: Add more information to the exception
            //       e.g. what steps were executing
            throw new MismatchedContextTypeException(
                c.GetType(), typeof(T));
        }
    }
}
