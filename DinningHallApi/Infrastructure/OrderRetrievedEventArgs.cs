using System;

namespace DinningHallApi.Infrastructure
{
    public class OrderRetrievedEventArgs : EventArgs
    {
        public Guid OrderId { get; }

        public OrderRetrievedEventArgs(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}