﻿
using Fu;
using Fu.Presets;
using Fu.Steps;

namespace RestStyle
{
  class Program
  {
    static void Main(string[] args)
    {
      var step = fu.Map.Controller(new NotesController());

      var app = new WebApp(step);
      app.Start();
    }
  }
}
