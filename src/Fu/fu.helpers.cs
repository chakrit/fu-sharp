
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fu
{
    // functional-style utilities
    public static partial class fu
    {
        public static Step Compose(params Step[] steps)
        { return fu.Compose((IEnumerable<Step>)steps); }

        public static Step Compose(IEnumerable<Step> steps)
        {
            if (steps == null || steps.Count() == 0)
                return fu.Identity;

            return steps
                .Aggregate((step1, step2) => c => step2(step1(c)));
        }


        public static Step If(Func<IFuContext, bool> condition, Step step)
        {
            if (condition == null) throw new ArgumentNullException("condition");
            if (step == null) throw new ArgumentNullException("step");

            return c => condition(c) ? step(c) : c;
        }

        public static Step If<TContext>
            (Func<TContext, bool> predicate, Step step)
            where TContext : IFuContext
        {
            if (predicate == null) throw new ArgumentNullException("condition");
            if (step == null) throw new ArgumentNullException("step");

            return fu.Step<TContext>(c => predicate(c) ? step(c) : c);
        }


        // TODO: is this called "Bind"? I think it's something different
        //       but I can't find what its' called... maybe "partial eval" ?
        public static Func<TArg2, Step> BindL<TArg1, TArg2>(TArg1 value, Func<TArg1, TArg2, Step> stepFunc)
        {
            return arg2 => stepFunc(value, arg2);
        }

        public static Func<TArg1, Step> BindR<TArg1, TArg2>(TArg2 value, Func<TArg1, TArg2, Step> stepFunc)
        {
            return arg1 => stepFunc(arg1, value);
        }


        // would curry-l curry-r be useful?

    }
}
