
namespace Fu.Services.Sessions
{
    public interface ISessionIdProvider
    {
        // in bits
        int KeyLength { get; }

        string CreateId(IFuContext c);
        string GetId(IFuContext c);

        void DeleteId(IFuContext c);
    }
}
