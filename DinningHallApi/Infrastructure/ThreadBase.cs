using System.Threading;

namespace DinningHallApi.Infrastructure
{
    public abstract class ThreadBase
    {
        private readonly Thread _thread;

        protected ThreadBase() => _thread = new Thread(Run);

        public void Start() => _thread.Start();
        public void Join() => _thread.Join();
        public bool IsAlive() => _thread.IsAlive;

        public abstract void Run();
    }
}