using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DinningHallApi.Models
{
    public class Waiter
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public Thread Thread { get; set; }
        public IList<Guid> TableIds { get; set; } = new List<Guid>();
        public Order Order { get; set; }

        public Waiter()
        {
            Id = Guid.NewGuid();
        }
    }
}