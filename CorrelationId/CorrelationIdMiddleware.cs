namespace CorrelationId
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // scoped service: live while live HttpContext for one request
            var correlationContext = context.RequestServices.GetRequiredService<ICorrelationContext>();
            var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
           
            if (!string.IsNullOrEmpty(correlationId))
            {
                correlationContext.UseCorrelationId(correlationId);
            }
            else
            {
                correlationId = correlationContext.CorrelationId;
            }

            if (context.Response.Headers.ContainsKey("X-Correlation-ID"))
            {
                context.Response.Headers["X-Correlation-ID"] = correlationId;
            }
            else
            {
                context.Response.Headers.Add("X-Correlation-ID", correlationId);
            }

            await _next(context);
        }
    }
}
