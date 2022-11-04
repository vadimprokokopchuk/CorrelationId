using MediatR;

namespace CorrelationId
{
    public class CorrelationIdPipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICorrelationContext _context;

        public CorrelationIdPipelineBehavior(ICorrelationContext context)
        {
            _context = context;
        }
        
        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is BaseRequest baseRequest)
            {
                baseRequest.CorrelationId = _context.CorrelationId;
            }

            return next();
        }
    }
}
