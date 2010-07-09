
namespace StronglyTypedSession
{
  public interface IMySession
  {
    string Username { get; set; }
    int UserID { get; set; }

    // Optional Destroy() methd for destroying the session
    void Destroy();
  }
}
