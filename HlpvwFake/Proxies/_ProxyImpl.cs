using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HlpvwFake.Proxies
{
    abstract class _ProxyImpl
    {
        public abstract string FullName { get; }

        public readonly object _UO;
        public readonly Type _TY;

        protected _ProxyImpl(object uo)
        {
            _UO = uo;
            _TY = _UO.GetType();
        }

        public T SlowCastTo<T>() where T : _ProxyImpl
        {
            return (T)Activator.CreateInstance(typeof(T), this._UO);
        }

        public object GetProperty(string name)
        {
            var pi = _TY.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            return pi.GetValue(this._UO, null);
        }

        public void SetProperty(string name, object value)
        {
            var pi = _TY.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            pi.SetValue(this._UO, value, null);
        }

        public void AddEventHandler(string name, Delegate handler)
        {
            var ei = _TY.GetEvent(name, BindingFlags.Public | BindingFlags.Instance);
            var _handler = Delegate.CreateDelegate(ei.EventHandlerType, handler.Target, handler.Method);
            ei.AddEventHandler(this._UO, _handler);
        }

        public void RemoveEventHandler(string name, Delegate handler)
        {
            var ei = _TY.GetEvent(name, BindingFlags.Public | BindingFlags.Instance);
            var _handler = Delegate.CreateDelegate(ei.EventHandlerType, handler.Target, handler.Method);
            ei.RemoveEventHandler(this._UO, _handler);
        }

        public object Invoke(string name, object[] argv, Type[] types = null)
        {
            if (types == null) types = Type.EmptyTypes;

            var mi = _TY.GetMethod(name, BindingFlags.Public | BindingFlags.Instance, null, types, null);
            return mi.Invoke(this._UO, argv);
        }
    }
}
