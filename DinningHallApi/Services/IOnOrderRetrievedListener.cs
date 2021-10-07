using System;
using DinningHallApi.Infrastructure;

namespace DinningHallApi.Services
{
    public interface IOnOrderRetrievedListener
    {
        event EventHandler<OrderRetrievedEventArgs> OnOrderRetrieve;
        public void Listen(Guid orderId);
    }
}