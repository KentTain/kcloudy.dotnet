using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Util;

namespace KC.Service.Events
{
    public static class ServiceEvents
    { 
        [ThreadStatic] // so that each thread has its own callbacks
        private static List<IHandler> _handlers;
    
        // public static IContainer Container { get; set; } 
    
        // Registers a callback for the given domain event
        public static void Register(IHandler handler)
        {
            if (_handlers == null)
            {
                _handlers = new List<IHandler>();
            }

            //LogUtil.LogInfo("------------Register ServiceEvents handler.");

            _handlers.Add(handler);
        }
    
        // Clears callbacks passed to Register on the current thread
        public static void ClearCallbacks()
        {
            _handlers = null;
        }

        public static void Raise()
        {
            ////if (Container != null)
            ////{
            ////    foreach (Handles<T> handler in Container.ResolveAll<Handles<T>>())
            ////    {
            ////        handler.Handle(args);
            ////    }
            ////}

            //LogUtil.LogInfo("------------Raise ServiceEvents handler. count = " + (_handlers != null ? _handlers.Count.ToString() : "null"));
            if (_handlers != null && _handlers.Any())
            {
                foreach (var handler in _handlers.ToList())
                {
                    var isSuccess = handler.Handle();
                    if (isSuccess)
                        _handlers.Remove(handler);
                }
            }
        }
    }
}
