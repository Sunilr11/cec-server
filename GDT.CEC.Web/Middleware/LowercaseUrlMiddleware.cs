namespace GDT.CEC.Web.Middleware
{
    public class LowercaseUrlMiddleware
    {
        private readonly RequestDelegate _next;

        public LowercaseUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.Path = new PathString(context.Request.Path.Value.ToLowerInvariant());
            context.Request.PathBase = new PathString(context.Request.PathBase.Value.ToLowerInvariant());

            await _next(context);
        }
    }
}
