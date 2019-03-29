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
        private ViewModelMessageBus() { }

        Dictionary<string, List<IMessageListener>> listeners = new Dictionary<string, List<IMessageListener>>();

        public void RegisterListener<T>(MessageListener<T> listener) where T : Message
        {
            if(listeners == null) { throw new ArgumentNullException(nameof(listeners)); }

            string key = typeof(T).ToString();
            if(!listeners.ContainsKey(key))
            {
                listeners.Add(key, new List<IMessageListener>());
            }
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
            LinkedList<string> keysToRemove = new LinkedList<string>();
            foreach(var g in listeners)
            {
                g.Value.RemoveAll(x => !x.IsValid);
                if(g.Value.Count == 0)
                {
                    keysToRemove.AddLast(g.Key);
                }
            }
            foreach(var k in keysToRemove)
            {
                listeners.Remove(k);
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
