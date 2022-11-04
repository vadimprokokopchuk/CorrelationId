using MediatR;

namespace CorrelationId
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BaseRequest : IRequest<User>
    {
        public string? CorrelationId { get; set; }
    }

    public class GetUserQuery : BaseRequest
    {
        public GetUserQuery(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }
    }

    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly ILogger<GetUserQueryHandler> _logger;

        public GetUserQueryHandler(ILogger<GetUserQueryHandler> logger)
        {
            _logger = logger;
        }

        public Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogError("CorrelationId: {ID}", request.CorrelationId);
            return Task.FromResult(new User { Id = 1, Name = "Vadim Prokopchuk" });
        }
    }
}
