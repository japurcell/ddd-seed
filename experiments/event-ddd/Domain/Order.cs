using System;
using MediatR;

namespace event_ddd.Domain
{
    public class Order : Psns.DDD.Entity
    {
        public DateTime OrderDate { get; private set; }

        private Order() { }

        public Order(DateTime? orderDate = default)
        {
            OrderDate = orderDate ?? DateTime.UtcNow;

            AddDomainEvent(new OrderStartedDomainEvent(this));
        }
    }
}
