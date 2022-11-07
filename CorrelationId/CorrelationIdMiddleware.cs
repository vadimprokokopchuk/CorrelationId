namespace CorrelationId
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string XCorrelationIdHeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationContext = context.RequestServices.GetRequiredService<ICorrelationContext>(); // scoped service
            var xCorrelationId = context.Request.Headers[XCorrelationIdHeaderName].ToString();
           
            if (!string.IsNullOrEmpty(xCorrelationId))
            {
                correlationContext.UseCorrelationId(xCorrelationId);
            }
            
            var correlationId = correlationContext.CorrelationId;
            
            if (context.Response.Headers.ContainsKey(XCorrelationIdHeaderName))
            {
                context.Response.Headers[XCorrelationIdHeaderName] = correlationId;
            }
            else
            {
                context.Response.Headers.Add(XCorrelationIdHeaderName, correlationId);
            }

            await _next(context);
        }
    }
}
