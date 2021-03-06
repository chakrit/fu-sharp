﻿
using Fu.Contexts;
using Fu.Exceptions;
using Fu.Results;
using Microsoft.FSharp.Core;

namespace Fu
{
  // Lambda conversion helpers
  // TODO: Add error handling for potential cast failure
  public static partial class fu
  {
    public static readonly Continuation Identity = step => ctx => step(ctx);

    public static readonly Continuation End = step => EndAct;

    // TODO: rename this to Plug
    public static readonly FuAction EndAct = ctx => { /* no-op */ };


    // Continuation utilities
    public static void Execute(this Continuation cont, IFuContext context)
    {
      cont(fu.EndAct)(context);
    }
  }
}
