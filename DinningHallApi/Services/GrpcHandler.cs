using System;
using System.Threading.Tasks;
using Grpc.Core;
using RestaurantService;

namespace DinningHallApi.Services
{
    public class GrpcHandler : GrpcDinningHallService.GrpcDinningHallServiceBase
    {
        private readonly IOnOrderRetrievedListener _listener;

        public GrpcHandler(IOnOrderRetrievedListener listener)
        {
            _listener = listener;
        }

        public override Task<Empty> SendOrderBack(SendOrderBackRequest request, ServerCallContext context)
        {
            _listener.Listen(Guid.Parse(request.OrderId));
            return Task.FromResult(new Empty());
        }
    }
}