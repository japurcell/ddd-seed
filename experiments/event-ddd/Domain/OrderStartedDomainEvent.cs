using MediatR;

namespace event_ddd.Domain
{
    public class OrderStartedDomainEvent : INotification
    {
        public Order Order { get; }

        public OrderStartedDomainEvent(Order order)
        {
            Order = order;
        }
    }
}
