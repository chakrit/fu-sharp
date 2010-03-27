
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

            // TOOD: Benchmark this and see if Aggregate would be better?
            //       Making .Aggregate(step2(step1)) work with IWalkPath would be hard
            //       but then it might pays off as faster
            return fu.Void(c => c.WalkPath.InsertNext(steps));
        }

        public static Step Compose(this Step step, Step nextStep)
        { return c => nextStep(step(c)); }

        public static Step Compose(this Step step, params Step[] steps)
        { return fu.Compose(step, fu.Compose(steps)); }

        public static Step Compose(this Step step, IEnumerable<Step> steps)
        { return fu.Compose(step, fu.Compose(steps)); }


        public static Step If(Func<IFuContext, bool> predicate, Step step)
        { return If(predicate, step, fu.Identity); }

        public static Step If<TContext>
            (Func<TContext, bool> predicate, Step step)
            where TContext : IFuContext
        { return If(predicate, step, fu.Identity); }

        public static Step If(Func<IFuContext, bool> predicate, Step trueStep, Step falseStep)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (trueStep == null) throw new ArgumentNullException("trueStep");
            if (falseStep == null) throw new ArgumentNullException("falseStep");

            return c => predicate(c) ? trueStep(c) : falseStep(c);
        }

        public static Step If<TContext>(Func<TContext, bool> predicate, Step trueStep, Step falseStep)
            where TContext : IFuContext
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (trueStep == null) throw new ArgumentNullException("trueStep");
            if (falseStep == null) throw new ArgumentNullException("falseStep");

            return fu.Step<TContext>(c => predicate(c) ? trueStep(c) : falseStep(c));
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
