using Serilog.Core;
using Serilog.Events;

namespace CorrelationId
{
    public class SerilogCorrelationIdEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SerilogCorrelationIdEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null)
            {
                var correlationId = httpContext.Response.Headers["X-Correlation-ID"].ToString();
                if (!string.IsNullOrEmpty(correlationId))
                {
                    var property = propertyFactory.CreateProperty("XCorrelationId", correlationId);
                    
                    // It will add correlationId to each log as a property
                    logEvent.AddPropertyIfAbsent(property);
                }
            }
        }
    }
}
