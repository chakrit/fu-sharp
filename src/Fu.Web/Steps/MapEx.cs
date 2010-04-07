
using System;
using System.Linq;
using System.Reflection;

namespace Fu.Steps
{
  public static partial class MapEx
  {
    public static Continuation Controller<TController>(this IMapSteps _)
      where TController : IFuController
    {
      return buildStep(buildController(typeof(TController)));
    }

    public static Continuation Controller(this IMapSteps _, Type controllerType)
    {
      if (!typeof(IFuController).IsAssignableFrom(controllerType))
        throw new ArgumentException(
            "controllerType must inherits from Fu.Steps.Use.Controller", "controllerType");

      return buildStep(buildController(controllerType));
    }

    public static Continuation Controller<TController>(this IMapSteps _, TController controller)
      where TController : IFuController
    {
      controller.Initialize();
      return buildStep(controller);
    }


    // TODO: What's the right assembly to use if not GetEntryAssembly?
    public static Continuation Controllers(this IMapSteps _)
    { return _.Controllers(Assembly.GetEntryAssembly()); }

    public static Continuation Controllers(this IMapSteps _, Assembly controllersAsm)
    {
      var controllerType = typeof(IFuController);

      // find all the defined controllers in the assembly
      var controllers = controllersAsm
        .GetTypes()
        .Where(t => !(t.IsAbstract || t.IsInterface))
        .Where(t => controllerType.IsAssignableFrom(t))
        .Select(t => buildController(t))
        .Cast<IFuController>()
        .ToArray();

      return _.Controllers(controllers);
    }

    public static Continuation Controllers(this IMapSteps _, params IFuController[] controllers)
    {
      Array.ForEach(controllers, c => c.Initialize());
      return buildStep(controllers);
    }


    private static IFuController buildController(Type controllerType)
    {
      var controller = (IFuController)Activator.CreateInstance(controllerType);
      controller.Initialize();

      return controller;
    }

    private static Continuation buildStep(params IFuController[] controllers)
    {
      var mappings = controllers
        .SelectMany(c => c.Mappings)
        .ToDictionary(kv => kv.Key, kv => kv.Value);

      return fu.Map.Urls(mappings);
    }
  }
}
