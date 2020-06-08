using MediatR;
using event_ddd.Domain;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace event_ddd.EventHandlers
{
    public class ValidateWhenOrderStartedDomainEventHandler :
        INotificationHandler<OrderStartedDomainEvent>
    {
        readonly ILogger<ValidateWhenOrderStartedDomainEventHandler> _logger;

        public ValidateWhenOrderStartedDomainEventHandler(
            ILogger<ValidateWhenOrderStartedDomainEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"handling {nameof(OrderStartedDomainEvent)}");
            return Task.Delay(0);
        }
    }
}
