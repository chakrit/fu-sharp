﻿
namespace Fu.Services
{
    public interface IService
    {
        void BeginWalk(IFuContext input);
        void EndWalk(IFuContext input);
    }

    public interface IService<T> : IService
    {
        bool CanGetServiceObject(IFuContext input);
        T GetServiceObject(IFuContext input);
    }
}
