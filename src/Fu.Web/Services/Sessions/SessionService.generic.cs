
using System;

namespace Fu.Services.Sessions
{
    public class SessionService<T> : SessionService, IService<T>
        where T : class
    {
        private Func<ISession, T> _wrapperFunc;


        public SessionService(ISessionIdProvider idProvider, ISessionStore store) :
            base(idProvider, store)
        {
            var generator = new StronglyTypedSessionWrapperGenerator();
            _wrapperFunc = generator.Generate<T>();
        }

        public new T GetServiceObject(IFuContext input)
        {
            return _wrapperFunc(base.GetServiceObject(input));
        }
    }
}
