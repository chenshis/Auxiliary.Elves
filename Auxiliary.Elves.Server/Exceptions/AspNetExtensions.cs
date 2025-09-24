namespace Auxiliary.Elves.Server.Exceptions
{
    public static class AspNetExtensions
    {
        public static void UseException(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
