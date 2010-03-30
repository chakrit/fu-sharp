
using System;
using System.Linq;
using System.Reflection;

namespace Fu.Steps
{
    public static partial class MapEx
    {
        public static Step Controller<TController>(this IMapSteps _)
            where TController : IFuController
        {
            return buildStep(buildController(typeof(TController)));
        }

        public static Step Controller(this IMapSteps _, Type controllerType)
        {
            if (!typeof(IFuController).IsAssignableFrom(controllerType))
                throw new ArgumentException(
                    "controllerType must inherits from Fu.Steps.Use.Controller", "controllerType");

            return buildStep(buildController(controllerType));
        }

        public static Step Controller<TController>(this IMapSteps _, TController controller)
            where TController : IFuController
        {
            controller.Initialize();
            return buildStep(controller);
        }


        // TODO: What's the right assembly to use if not GetEntryAssembly?
        public static Step Controllers(this IMapSteps _)
        { return _.Controllers(Assembly.GetEntryAssembly()); }

        public static Step Controllers(this IMapSteps _, Assembly controllersAsm)
        {
            var controllerType = typeof(IFuController);

            // find all the defined controllers in the assembly
            var controllers = controllersAsm.GetTypes()
                .Where(t => !(t.IsAbstract || t.IsInterface))
                .Where(t => controllerType.IsAssignableFrom(t))
                .Select(t => buildController(t))
                .Cast<IFuController>()
                .ToArray();

            return _.Controllers(controllers);
        }

        public static Step Controllers(this IMapSteps _, params IFuController[] controllers)
        { return buildStep(controllers); }


        private static IFuController buildController(Type controllerType)
        {
            var controller = (IFuController)Activator.CreateInstance(controllerType);
            controller.Initialize();

            return controller;
        }

        private static Step buildStep(params IFuController[] controllers)
        {
            var allMappings = controllers
                .Select(c => new
                {
                    // pre-process presteps and poststeps for each controller
                    PreStep = fu.Compose(c.PreSteps),
                    PostStep = fu.Compose(c.PostSteps),

                    Mappings = c.Mappings
                })
                .SelectMany(c => c.Mappings, (c, m) => new
                {
                    // then append them to each mapped step
                    PreStep = c.PreStep,
                    PostStep = c.PostStep,

                    Key = m.Key,
                    Step = fu.Compose(c.PreStep, m.Value, c.PostStep),
                })
                .ToDictionary(x => x.Key, x => x.Step);

            return fu.Map.Urls(allMappings);
        }
    }
}
