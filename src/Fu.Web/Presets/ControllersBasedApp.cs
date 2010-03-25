
using System.Reflection;

using Fu.Steps;

namespace Fu.Presets
{
    public class ControllersBasedApp : WebApp
    {
        public ControllersBasedApp() :
            base(fu.Map.Controllers()) { }

        public ControllersBasedApp(Assembly asm) :
            base(fu.Map.Controllers(asm)) { }
        public ControllersBasedApp(FuSettings settings, Assembly asm) :
            base(settings, fu.Map.Controllers(asm)) { }

        public ControllersBasedApp(params IFuController[] controllers) :
            base(fu.Map.Controllers(controllers)) { }
        public ControllersBasedApp(FuSettings settings, params IFuController[] controllers) :
            base(settings, fu.Map.Controllers(controllers)) { }
    }
}
