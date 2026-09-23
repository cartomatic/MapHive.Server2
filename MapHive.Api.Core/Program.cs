namespace MapHive.Core.Api
{

    public class Program
    {
        public static int Main(string[] args)
        {
            return MapHive.Core.Api.WebHostUtils.BuildAndRunWebHost<Startup>(args);
        }
    }
}
