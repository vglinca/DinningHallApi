using System;
using System.Collections.Generic;
using System.Linq;

namespace DinningHallApi.Models
{
    public class Table
    {
        public Guid Id { get; }
        public TableStatus Status { get; set; }
        public Order Order { get; private set; }
        public Waiter Waiter { get; private set; }

        public Table()
        {
            Id = Guid.NewGuid();
            Status = TableStatus.Free;
            Waiter = null;
        }

        public Order CreateOrder(IList<MenuItem> menuItems, Waiter waiter)
        {
            var numOfMenuItemsSelected = (new Random()).Next(1, menuItems.Count + 1);
            var order = new Order
            {
                MenuItems = menuItems.Take(numOfMenuItemsSelected).ToList(),
                WaiterId = waiter.Id
            };
            order.SetPriority();

            Order = order;
            Status = TableStatus.WaitingForAnOrder;

            return order;
        }

        public void AssignAWaiter(Waiter waiter)
        {
            if (Waiter != null)
            {
                throw new ArgumentException("A waiter has already been assigned to this table.");
            }

            Waiter = waiter;
        }
    }
}