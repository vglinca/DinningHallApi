using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using DinningHallApi.Consts;
using DinningHallApi.Infrastructure;
using DinningHallApi.Models;
using Microsoft.Extensions.Hosting;

namespace DinningHallApi.Services
{
    public class DinningHallService : IHostedService
    {
        private readonly IList<Table> _tables;
        private readonly IList<MenuItem> _menu;
        private readonly IList<Waiter> _waiters;
        private readonly Queue<Guid> _readyOrders;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly object _lock = new();
        private readonly SemaphoreSlim _semaphore;
        private readonly IOnOrderRetrievedListener _onOrderRetrievedListener;
        
        public DinningHallService(IHttpClientFactory httpClientFactory, IOnOrderRetrievedListener onOrderRetrievedListener)
        {
            _httpClientFactory = httpClientFactory;
            _onOrderRetrievedListener = onOrderRetrievedListener;
            _tables = new List<Table>();
            _menu = new List<MenuItem>();
            _waiters = new List<Waiter>();
            _readyOrders = new Queue<Guid>();
            _semaphore = new SemaphoreSlim(1);
            GenerateMenu();
            CreateTables();
            CreateWaiters();

            _onOrderRetrievedListener.OnOrderRetrieve += OnOrderRetrieved;
        }

        private void OnOrderRetrieved(object? sender, OrderRetrievedEventArgs e)
        {
            lock (_lock)
            {
                Console.WriteLine($"Retrieved order with ID: '{e.OrderId}");
                _readyOrders.Enqueue(e.OrderId);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("-->Starting the Dinning Hall Service...");
            foreach (var waiter in _waiters)
            {
                Console.WriteLine($"Starting thread for waiter {waiter.Id}");
                var thread = new Thread(Run) {Name = $"TH_{waiter.Id}"};
                thread.Start(waiter);
            }
            
            return Task.CompletedTask;
        }

        private async void Run(object waiterParam)
        {
            if (waiterParam is not Waiter waiter)
            {
                throw new ArgumentNullException(nameof(waiter));
            }

            Console.WriteLine($"Waiter with ID: '{waiter.Id} is here.");
            while (_tables.Any(t => t.Status != TableStatus.OrderFinished))
            {
                Console.WriteLine($"--> Thread '{Thread.CurrentThread.Name}' is about to pick up a table");
                //pick up a table
                Table table = null;
                lock (_lock)
                {
                    Thread.Sleep(2000);
                    table = _tables.FirstOrDefault(t => t.Status == TableStatus.ReadyForAnOrder);
                }

                //if table is null then it's the end
                if (table is null)
                {
                    break;
                }

                //make an order
                waiter.TableIds.Add(table.Id);
                var order = table.CreateOrder(_menu, waiter);

                try
                {
                    //deliver order to kitchen
                    var httpClient = _httpClientFactory.CreateClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5002/api/orders/{order.Id}");
                    await httpClient.SendAsync(request);
                }
                catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
                {
                    Console.WriteLine(e);
                    throw;
                }


                Console.WriteLine($"--> Thread '{Thread.CurrentThread.Name}' is simulating a request to kitchen to deliver order with ID: '{order.Id}': http://localhost:5002/api/make-order/{order.Id}");

                // subscribe on response from kitchen

                //deliver order to table
                table.Status = TableStatus.OrderFinished;

                //change table status to 'finished'
            }

            Console.WriteLine("No more tables");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private void CreateTables()
        {
            var rnd = new Random();
            var tablesCount = rnd.Next(5, 9);
            for (var i = 0; i < tablesCount; i++)
            {
                var table = new Table() {Status = TableStatus.ReadyForAnOrder};
                _tables.Add(table);
            }
        }
        
        private void CreateWaiters()
        {
            var rnd = new Random();
            var waitersCount = rnd.Next(1, 5);
            for (var i = 0; i < 3; i++)
            {
                var waiter = new Waiter() {Name = $"Waiter #{i + 1}"};
                _waiters.Add(waiter);
            }
        }

        private void GenerateMenu()
        {
            var rnd = new Random();
            for (var i = 1; i <= FoodItems.Food.Count; ++i)
            {
                var complexity = rnd.Next(1, 6);
                var menuItem = new MenuItem
                {
                    Name = FoodItems.Food[i],
                    Complexity = complexity,
                    CookingApparatus = i % 2 == 0 ? CookingApparatus.Oven : CookingApparatus.Stove,
                    PreparationTime = complexity * 5
                };

                _menu.Add(menuItem);
            }
        }
    }
}