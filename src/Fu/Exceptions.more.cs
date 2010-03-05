
using System;

namespace Fu.Exceptions
{
    public class MismatchedContextTypeException : Exception
    {
        public Type SourceType { get; private set; }
        public Type TargetType { get; private set; }

        public MismatchedContextTypeException(Type src, Type target) :
            base(string.Format(
                "Context from previous step is of type {0} but the next step " +
                "expects a {1}.", src.FullName, target.FullName))
        {
            SourceType = src;
            TargetType = target;
        }
    }

    public class InvalidServiceTypeException : Exception
    {
        public Type RequestedType { get; private set; }

        // TODO: Add list of services that has service object of type requestedType
        //       but cannot provide a sevice object because of incompatibility
        //       e.g. missing multipart support in FormDataParser, for example
        public InvalidServiceTypeException(Type requestedType) :
            base(string.Format(
                "Cannot provide a service object of type {0}, either because there is " +
                "no applicable or compatible services right now or you have not added " +
                "any services that provides the requested service object type.",
                requestedType.Name))
        {
            RequestedType = requestedType;
        }
    }
}
