using System;
using DinningHallApi.Infrastructure;

namespace DinningHallApi.Services
{
    public class OnOrderRetrievedListener : IOnOrderRetrievedListener
    {
        public event EventHandler<OrderRetrievedEventArgs> OnOrderRetrieve;
         
        public void Listen(Guid orderId)
        {
            OnOrderRetrieve?.Invoke(this, new OrderRetrievedEventArgs(orderId));
        }
    }
}