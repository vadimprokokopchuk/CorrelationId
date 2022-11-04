namespace CorrelationId
{

    public interface IEventBus
    {
        void SendEvent();
    }

    public class EventBus : IEventBus
    {
        private readonly ICorrelationContext _correlationContext;
        private readonly ILogger<EventBus> _logger;

        public EventBus(ICorrelationContext correlationContext, ILogger<EventBus> logger)
        {
            _correlationContext = correlationContext;
            _logger = logger;
        }

        public void SendEvent()
        {
            // EventModel.From(object, correlationId);
            _logger.LogWarning("EventModel.From(object, correlationId): new {{ ..., correlationId: {Id}}}",
                _correlationContext.CorrelationId);
        }
    }
}
