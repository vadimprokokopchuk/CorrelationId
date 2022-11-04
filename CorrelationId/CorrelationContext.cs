namespace CorrelationId
{
    public interface ICorrelationContext
    {
        string CorrelationId { get; }
        void UseCorrelationId(string correlationId);
    }

    public class CorrelationContext : ICorrelationContext
    {
        private string? _correlationId;

        public string CorrelationId => _correlationId ??= Guid.NewGuid().ToString("D");

        public void UseCorrelationId(string correlationId)
        {
            if (!string.IsNullOrEmpty(_correlationId))
            {
                throw new ApplicationException("DeveloperException: CorrelationID already generated and value has been used in other place.");
            }

            _correlationId = correlationId;
        }
    }
}
