using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels.MessageBus
{
    class ViewModelMessageBus
    {
        public static ViewModelMessageBus Instance { get; }
        static ViewModelMessageBus()
        {
            Instance = new ViewModelMessageBus();
        }
        private ViewModelMessageBus()
        {
            Task.Run(async () =>
            {
                while(true)
                {
                    await Task.Delay(120000);
                    CleanupListeners();
                }
            });
        }

        System.Collections.Concurrent.ConcurrentDictionary<string, List<IMessageListener>> listeners = 
            new System.Collections.Concurrent.ConcurrentDictionary<string, List<IMessageListener>>();

        public void RegisterListener<T>(MessageListener<T> listener) where T : Message
        {
            if(listeners == null) { throw new ArgumentNullException(nameof(listeners)); }
            string key = typeof(T).ToString();
            listeners.TryAdd(key, new List<IMessageListener>());
            listeners[key].Add(listener);
        }
        public bool SendMessage<T>(T message) where T : Message
        {
            if(message == null) { throw new ArgumentNullException(nameof(message)); }

            string key = typeof(T).ToString();
            if(listeners.TryGetValue(key, out var msgListeners))
            {
                foreach(var l in msgListeners)
                {
                    l.HandleMessage(message);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CleanupListeners()
        {
            LinkedList<string> keysToRemove = null;
            foreach(var g in listeners)
            {
                g.Value.RemoveAll(x => !x.IsValid);
                if(g.Value.Count == 0)
                {
                    if(keysToRemove == null) { keysToRemove = new LinkedList<string>(); }
                    keysToRemove.AddLast(g.Key);
                }
            }
            if(keysToRemove != null)
            {
                foreach (var k in keysToRemove)
                {
                    listeners.Remove(k, out _);
                }
            }
        }
    }

    interface IMessageListener
    {
        bool HandleMessage(Message msg);
        bool IsValid { get; }
    }

    class MessageListener<T> : IMessageListener where T : Message
    {
        WeakReference<Func<T, bool>> Handler;
        public bool HandleMessage(Message msg)
        {
            if(msg is T tMsg)
            {
                return HandleMessage(tMsg);
            }
            return false;
        }
        public virtual bool HandleMessage(T msg)
        {
            if(Handler.TryGetTarget(out var handler))
            {
                return handler(msg);
            }
            return false;
        }
        public bool IsValid => Handler.TryGetTarget(out _);
        public MessageListener(Func<T, bool> handler) => Handler = new WeakReference<Func<T, bool>>(handler);
    }

    abstract class Message { }
}
