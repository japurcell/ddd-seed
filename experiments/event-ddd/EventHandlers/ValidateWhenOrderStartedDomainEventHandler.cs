using MediatR;
using event_ddd.Domain;
using System.Threading.Tasks;
using System.Threading;

namespace event_ddd.EventHandlers
{
    public class ValidateWhenOrderStartedDomainEventHandler :
        INotificationHandler<OrderStartedDomainEvent>
    {
        public Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }
    }
}
