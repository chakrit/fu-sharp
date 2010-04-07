
namespace Fu.Steps
{
  public static partial class Map
  {
    public static Continuation Method(this IMapSteps _, string method)
    {
      return _.Method(method, fu.Http.MethodNotAllowed());
    }

    // TODO: What happens with lowercase HTTP methods?
    //       What would be the proper behavior in such cases?
    public static Continuation Method(this IMapSteps _, string method, Continuation on405)
    {
      on405 = on405 ?? fu.Http.MethodNotAllowed();
      method = method.ToUpper();

      return step => ctx =>
        (ctx.Request.HttpMethod == method ? step : on405(step))(ctx);
    }


    // TOOD: Move this to another file
    #region GET POST PUT DELETE overloads

    public static Continuation Get(this IMapSteps _)
    { return _.Get(null); }

    public static Continuation Get(this IMapSteps _, Continuation on405)
    { return _.Method("GET", null); }

    public static Continuation Post(this IMapSteps _)
    { return _.Post(null); }

    public static Continuation Post(this IMapSteps _, Continuation on405)
    { return _.Method("POST", on405); }

    public static Continuation Put(this IMapSteps _)
    { return _.Put(null); }

    public static Continuation Put(this IMapSteps _, Continuation on405)
    { return _.Method("PUT", on405); }

    public static Continuation Delete(this IMapSteps _)
    { return _.Delete(null); }

    public static Continuation Delete(this IMapSteps _, Continuation on405)
    { return _.Method("DELETE", on405); }

    #endregion


    public static Continuation GetPost(this IMapSteps _,
      Continuation get, Continuation post)
    { return _.GetPost(get, post, null); }

    public static Continuation GetPost(this IMapSteps _,
      Continuation get, Continuation post, Continuation on405)
    {
      return _.Get(_.Put(on405));
    }


    public static Continuation Methods(this IMapSteps _,
      Continuation get, Continuation put, Continuation post, Continuation delete)
    { return _.Methods(get, put, post, delete, null); }

    public static Continuation Methods(this IMapSteps _,
      Continuation get, Continuation put, Continuation post, Continuation delete,
      Continuation on405)
    {
      return _.Get(_.Post(_.Put(_.Delete(on405))));
    }
  }
}
