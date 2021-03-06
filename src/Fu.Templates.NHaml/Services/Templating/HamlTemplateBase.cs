﻿
using System;
using System.Collections.Generic;

using Fu.Results;

using NHaml;

namespace Fu.Services.Templating
{
  public class HamlTemplateBase : Template
  {
    private IDictionary<string, Stack<Action>> _blocks;


    public HamlResultBase Result { get; set; }
    public IFuContext Context { get; set; }


    protected void Block(string name, Action yield)
    {
      _blocks = _blocks ?? new Dictionary<string, Stack<Action>>(5);

      Stack<Action> stack;

      if (!_blocks.TryGetValue(name, out stack))
        _blocks[name] = stack = new Stack<Action>(5);

      stack.Push(yield);
    }

    protected void RenderBlock(string name)
    {
      if (_blocks == null) return;

      Stack<Action> stack;

      if (!_blocks.TryGetValue(name, out stack) || stack.Count == 0)
        return;

      stack.Pop()();
    }
  }
}
