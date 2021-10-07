using System;
using System.Collections.Generic;
using System.Linq;
using DinningHallApi.Extensions;

namespace DinningHallApi.Models
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid WaiterId { get; set; }
        public int Priority { get; private set; }
        public IList<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public double MaxWaitTime => MenuItems.Max(x => x.PreparationTime).MultiplyBy(1.3);

        public Order()
        {
            Id = Guid.NewGuid();
        }

        public void SetPriority() => Priority = (new Random()).Next(1, 6);
    }
}