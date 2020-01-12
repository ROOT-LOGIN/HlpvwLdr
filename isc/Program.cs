using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

abstract class _ProxyImpl
{
    public abstract string FullName { get; }

    protected readonly object _UO;
    protected readonly Type _TY;
    protected _ProxyImpl(object uo)
    {
        _UO = uo;
        _TY = _UO.GetType();
    }

    protected object GetProperty(string name)
    {
        var pi = _TY.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        return pi.GetValue(this._UO, null);
    }

    protected void SetProperty(string name, object value)
    {
        var pi = _TY.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        pi.SetValue(this._UO, value, null);
    }

    protected void AddEventHandler(string name, Delegate handler)
    {
        var ei = _TY.GetEvent(name, BindingFlags.Public | BindingFlags.Instance);
        var _handler = Delegate.CreateDelegate(ei.EventHandlerType, handler.Target, handler.Method);
        ei.AddEventHandler(this._UO, _handler);
    }

    protected void RemoveEventHandler(string name, Delegate handler)
    {
        var ei = _TY.GetEvent(name, BindingFlags.Public | BindingFlags.Instance);
        var _handler = Delegate.CreateDelegate(ei.EventHandlerType, handler.Target, handler.Method);
        ei.RemoveEventHandler(this._UO, _handler);
    }

    protected object Invoke(string name, object[] argv, Type[] types = null)
    {
        if (types == null) types = Type.EmptyTypes;

        var mi = _TY.GetMethod(name, BindingFlags.Public | BindingFlags.Instance, null, types, null);
        return mi.Invoke(this._UO, argv);
    }
}

namespace test_namepsace
{
    internal delegate int TestEventHandler(object o, string e);

    interface ITestInterface
    {
        string ReadonlyProp { get; }
        string WriteonlyProp { set; }
        string Property { get; set; }

        event TestEventHandler TestEvent;

        object Method();
        object Method(string p1);
        object Method(int p1, bool p2);

        T GenericMethod<T>(object p1);
    }

    class ITestInterfaceImpl : ITestInterface
    {
        public string ReadonlyProp => "ReadonlyProp";

        public string WriteonlyProp { set => Console.WriteLine("WriteonlyProp"); }
        public string Property { get => "get Property"; set => Console.WriteLine("set Property"); }

        public event TestEventHandler TestEvent;

        public T GenericMethod<T>(object p1)
        {
            Console.WriteLine("Callint T GenericMethod<T>(object p1)");
            if (TestEvent != null) TestEvent(p1, null);
            return default(T);
        }

        public object Method()
        {
            Console.WriteLine("Calling object Method()");
            if (TestEvent != null) TestEvent(null, null);
            return null;
        }

        public object Method(string p1)
        {
            Console.WriteLine("Calling object Method(string p1)");
            if (TestEvent != null) TestEvent(null, p1);
            return null;
        }

        public object Method(int p1, bool p2)
        {
            Console.WriteLine("Calling object Method(int p1, bool p2)");
            if (TestEvent != null) TestEvent(p1, p2.ToString());
            return null;
        }
    }
}

namespace isc
{
    interface OutputTarget : IDisposable
    {
        void Write(ulong value);
        void Write(object value);
        void Write(uint value);
        void Write(long value);
        void Write(string value);
        void Write(int value);
        void Write(string format, object arg0);
        void Write(string format, object arg0, object arg1, object arg2, object arg3);
        void Write(decimal value);
        void Write(double value);
        void Write(char[] buffer, int index, int count);
        void Write(char[] buffer);
        void Write(char value);
        void Write(bool value);
        void Write(string format, params object[] arg);
        void Write(string format, object arg0, object arg1, object arg2);
        void Write(string format, object arg0, object arg1);
        void Write(float value);
        void WriteLine(uint value);
        void WriteLine(int value);
        void WriteLine(float value);
        void WriteLine(char value);
        void WriteLine(decimal value);
        void WriteLine(long value);
        void WriteLine(char[] buffer, int index, int count);
        void WriteLine(double value);
        void WriteLine(ulong value);
        void WriteLine(string format, object arg0, object arg1, object arg2, object arg3);
        void WriteLine(string value);
        void WriteLine(string format, object arg0);
        void WriteLine(string format, object arg0, object arg1);
        void WriteLine(string format, object arg0, object arg1, object arg2);
        void WriteLine(string format, params object[] arg);
        void WriteLine(bool value);
        void WriteLine();
        void WriteLine(object value);
        void WriteLine(char[] buffer);
    }

    class FileTarget : OutputTarget
    {
        public static OutputTarget Open() { return ConsoleTarget.Singleton; }
        public static OutputTarget Open(string file) { return new FileTarget(new StreamWriter(file)); }
        public static OutputTarget Open(Stream file) { return new FileTarget(new StreamWriter(file)); }
        public static OutputTarget Open(StreamWriter file) { return new FileTarget(file); }

        readonly StreamWriter _SW;

        public FileTarget(StreamWriter file)
        {
            _SW = file;
        }

        void IDisposable.Dispose()
        {
            if (_SW == null) return;

            _SW.Flush();
            _SW.Close();
            _SW.Dispose();
        }

        sealed class ConsoleTarget : OutputTarget
        {
            void IDisposable.Dispose() { }

            private ConsoleTarget() { }
            public static readonly ConsoleTarget Singleton = new ConsoleTarget();
            public void Write(ulong value) { Console.Write(value); }
            public void Write(object value) { Console.Write(value); }
            public void Write(uint value) { Console.Write(value); }
            public void Write(long value) { Console.Write(value); }
            public void Write(string value) { Console.Write(value); }
            public void Write(int value) { Console.Write(value); }
            public void Write(string format, object arg0) { Console.Write(format, arg0); }
            public void Write(string format, object arg0, object arg1, object arg2, object arg3) { Console.Write(format, arg0, arg1, arg2, arg3); }
            public void Write(decimal value) { Console.Write(value); }
            public void Write(double value) { Console.Write(value); }
            public void Write(char[] buffer, int index, int count) { Console.Write(buffer, index, count); }
            public void Write(char[] buffer) { Console.Write(buffer); }
            public void Write(char value) { Console.Write(value); }
            public void Write(bool value) { Console.Write(value); }
            public void Write(string format, params object[] arg) { Console.Write(format, arg); }
            public void Write(string format, object arg0, object arg1, object arg2) { Console.Write(format, arg0, arg1, arg2); }
            public void Write(string format, object arg0, object arg1) { Console.Write(format, arg0, arg1); }
            public void Write(float value) { Console.Write(value); }
            public void WriteLine(uint value) { Console.WriteLine(value); }
            public void WriteLine(int value) { Console.WriteLine(value); }
            public void WriteLine(float value) { Console.WriteLine(value); }
            public void WriteLine(char value) { Console.WriteLine(value); }
            public void WriteLine(decimal value) { Console.WriteLine(value); }
            public void WriteLine(long value) { Console.WriteLine(value); }
            public void WriteLine(char[] buffer, int index, int count) { Console.WriteLine(buffer, index, count); }
            public void WriteLine(double value) { Console.WriteLine(value); }
            public void WriteLine(ulong value) { Console.WriteLine(value); }
            public void WriteLine(string format, object arg0, object arg1, object arg2, object arg3) { Console.WriteLine(format, arg0, arg1, arg2, arg3); }
            public void WriteLine(string value) { Console.WriteLine(value); }
            public void WriteLine(string format, object arg0) { Console.WriteLine(format, arg0); }
            public void WriteLine(string format, object arg0, object arg1) { Console.WriteLine(format, arg0, arg1); }
            public void WriteLine(string format, object arg0, object arg1, object arg2) { Console.WriteLine(format, arg0, arg1, arg2); }
            public void WriteLine(string format, params object[] arg) { Console.WriteLine(format, arg); }
            public void WriteLine(bool value) { Console.WriteLine(value); }
            public void WriteLine() { Console.WriteLine(); }
            public void WriteLine(object value) { Console.WriteLine(value); }
            public void WriteLine(char[] buffer) { Console.WriteLine(buffer); }
        }

        public void Write(ulong value) { _SW.Write(value); }
        public void Write(object value) {_SW.Write(value); }
        public void Write(uint value) { _SW.Write(value); }
        public void Write(long value) { _SW.Write(value); }
        public void Write(string value) { _SW.Write(value); }
        public void Write(int value) { _SW.Write(value); }
        public void Write(string format, object arg0) { _SW.Write(format, arg0); }
        public void Write(string format, object arg0, object arg1, object arg2, object arg3) { _SW.Write(format, arg0, arg1, arg2, arg3); }
        public void Write(decimal value) { _SW.Write(value); }
        public void Write(double value) { _SW.Write(value); }
        public void Write(char[] buffer, int index, int count) { _SW.Write(buffer, index, count); }
        public void Write(char[] buffer) { _SW.Write(buffer); }
        public void Write(char value) { _SW.Write(value); }
        public void Write(bool value) { _SW.Write(value); }
        public void Write(string format, params object[] arg) { _SW.Write(format, arg); }
        public void Write(string format, object arg0, object arg1, object arg2) { _SW.Write(format, arg0, arg1, arg2); }
        public void Write(string format, object arg0, object arg1) { _SW.Write(format, arg0, arg1); }
        public void Write(float value) { _SW.Write(value); }
        public void WriteLine(uint value) { _SW.WriteLine(value); }
        public void WriteLine(int value) { _SW.WriteLine(value); }
        public void WriteLine(float value) { _SW.WriteLine(value); }
        public void WriteLine(char value) { _SW.WriteLine(value); }
        public void WriteLine(decimal value) { _SW.WriteLine(value); }
        public void WriteLine(long value) { _SW.WriteLine(value); }
        public void WriteLine(char[] buffer, int index, int count) { _SW.WriteLine(buffer, index, count); }
        public void WriteLine(double value) { _SW.WriteLine(value); }
        public void WriteLine(ulong value) { _SW.WriteLine(value); }
        public void WriteLine(string format, object arg0, object arg1, object arg2, object arg3) { _SW.WriteLine(format, arg0, arg1, arg2, arg3); }
        public void WriteLine(string value) { _SW.WriteLine(value); }
        public void WriteLine(string format, object arg0) { _SW.WriteLine(format, arg0); }
        public void WriteLine(string format, object arg0, object arg1) { _SW.WriteLine(format, arg0, arg1); }
        public void WriteLine(string format, object arg0, object arg1, object arg2) { _SW.WriteLine(format, arg0, arg1, arg2); }
        public void WriteLine(string format, params object[] arg) { _SW.WriteLine(format, arg); }
        public void WriteLine(bool value) { _SW.WriteLine(value); }
        public void WriteLine() { _SW.WriteLine(); }
        public void WriteLine(object value) { _SW.WriteLine(value); }
        public void WriteLine(char[] buffer) { _SW.WriteLine(buffer); }
    }

    class Program
    {
        int proxy_handler(object o, string e)
        {
            Console.WriteLine($"TestEvent: {o} {e}");
            return 12345;
        }

        class GenerateArgs
        {
            public bool IsSingleFile = true;
            public string Output;
        }

        static void Main(string[] args)
        {
#if !false
            var impl = new test_namepsace.ITestInterfaceImpl();
            var proxy = new test_namepsace_proxy.ITestInterfaceProxy(impl);
            var pm = new Program();
            proxy.TestEvent += pm.proxy_handler;
            proxy.Method("string-1000");
            proxy.TestEvent -= pm.proxy_handler;
            proxy.Method(-2000, true);

            Console.WriteLine($"{proxy.ReadonlyProp}");

            Console.WriteLine();
            Console.Write("Press any key to continue ...");
            Console.ReadKey();
#else            
            if (args.Length == 0)
            {
                Console.WriteLine("isc <assembly> [/out:target_path] [/single:Y|N]");
                //return;
            }

            string assembly_file = null;
            bool is_single = true;
            string out_dir = null;
            foreach(var arg in args)
            {
                if (arg.StartsWith("/out:", StringComparison.InvariantCultureIgnoreCase))
                {
                    out_dir = Path.GetDirectoryName(Path.GetFullPath(arg.Replace("\"", string.Empty).Substring(5)));
                    continue;
                }

                if (arg.StartsWith("/single:", StringComparison.InvariantCultureIgnoreCase))
                {
                    is_single = !(string.Compare("n", arg.Replace("\"", string.Empty), true) == 0);
                    continue;
                }

                assembly_file = Path.GetFullPath(arg.Replace("\"", string.Empty));
            }
            if (string.IsNullOrEmpty(out_dir)) out_dir = Path.GetTempPath();

            //var file = args[0];
            while (true)
            {
                Console.WriteLine("Type assembly path:");
                if (string.IsNullOrEmpty(assembly_file))
                    assembly_file = Console.ReadLine();

                if (string.IsNullOrEmpty(assembly_file))
                {
                    Console.Write("Do ypu want to exit? ");
                    assembly_file = Console.ReadLine();
                    if (assembly_file == "y" || assembly_file == "Y")
                        return;
                    assembly_file = null;
                    continue;
                }
                if (string.IsNullOrEmpty(assembly_file))
                    continue;

                assembly_file = Path.GetFullPath(assembly_file);
                if (!File.Exists(assembly_file))
                {
                    Console.WriteLine("Assembly is not exist.");
                    assembly_file = null;
                    continue;
                }


                __Generate(assembly_file, true);
            }
#endif

        }

        static string mapTypeName(Type type)
        {
            if (type == typeof(object)) return "object";
            if (type == typeof(string)) return "string";
            if (type == typeof(void)) return "void";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(char)) return "char";
            if (type == typeof(sbyte)) return "sbyte";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(short)) return "short";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(int)) return "int";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(long)) return "long";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(decimal)) return "decimal";

            return type.FullName;
        }

        static void __Generate(string file, bool toConsole)
        {
            OutputTarget Console = FileTarget.Open();

            Assembly assembly = null;
            try
            {
                assembly = Assembly.ReflectionOnlyLoadFrom(file);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsVisible || type.IsGenericType) continue;

                if (type.IsInterface)
                {
                    /*
                    namespace {
                    // public class {}Proxy : _ProxyImpl {
                        public const string FullName = {};
                        readonly object _UO;
                        public {}Proxy(object uo){ this._UO = uo; }

                        public object _f{}
                        {
                            get { return GetField({}); }
                            Set { SetField({}, value); }
                        }

                        public object {}
                        {
                            get { return GetProperty({}, null); }
                            Set { SetProperty({}, value, null); }
                        }

                        public event {}
                        {
                            add { AddEventHandler({}, value); }
                            remove { RemoveEventHandler({}, value); }
                        }
                        
                    }
                    */

                    Console.WriteLine("namespace {0} {{", type.Namespace);
                    Console.WriteLine("class {0}Proxy : _ProxyImpl {{", type.Name);                    
                    Console.WriteLine("// Intrisic");
                    Console.WriteLine("public override string FullName {{ get; }} = \"{0}\";", mapTypeName(type));
                    Console.WriteLine("public {0}Proxy(object uo) : base(uo) {{ }}", type.Name);
                    Console.WriteLine();
                    var props = type.GetProperties();
                    if (props != null)
                    {
                        Console.WriteLine("// Properties");
                        Console.WriteLine();
                        foreach (var prop in props)
                        {
                            /*var ips = prop.GetIndexParameters();
                            string ip_name = null;
                            string ip_value = "null";
                            if (ips == null)
                            {
                                ip_name = $"[{string.Join(", ", ips.Select(p => $"{p.ParameterType.FullName} {p.Name}"))}]";
                                ip_value = $"new object[]{{ {string.Join(",", ips.Select(p => p.Name))} }}";
                            }*/
                            Console.WriteLine("public {1} {0} {{", prop.Name, mapTypeName(prop.PropertyType));
                            if (prop.CanRead)
                            {
                                Console.WriteLine("get {{ return ({1})base.GetProperty(\"{0}\"); }}", prop.Name, mapTypeName(prop.PropertyType));
                            }
                            if (prop.CanWrite)
                            {
                                Console.WriteLine("set {{ base.SetProperty(\"{0}\", value); }}", prop.Name);
                            }
                            Console.WriteLine("}");
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    var events = type.GetEvents();
                    if (events != null)
                    {
                        Console.WriteLine("// Events");
                        Console.WriteLine();

                        foreach (var ent in events)
                        {
                            if (ent.EventHandlerType.IsGenericType)
                            {
                                Console.WriteLine("// {0}", ent.ToString());
                                continue;
                            }

                            if (!ent.EventHandlerType.IsVisible)
                            {
                                var emi = ent.EventHandlerType.GetMethod("Invoke");
                                string ps_name = null;
                                var ps = emi.GetParameters();
                                if (ps != null && ps.Length > 0)
                                {
                                    ps_name = string.Join(", ", ps.Select(p => $"{mapTypeName(p.ParameterType)} {p.Name}"));
                                }
                                Console.WriteLine("public delegate {1} {0}({2});", ent.EventHandlerType.Name, mapTypeName(emi.ReturnType), ps_name);
                            }

                            Console.WriteLine("public event {1} {0} {{", ent.Name, ent.EventHandlerType.IsVisible ? ent.EventHandlerType.FullName : ent.EventHandlerType.Name);
                            Console.WriteLine("add {{ base.AddEventHandler(\"{0}\", value); }}", ent.Name);
                            Console.WriteLine("remove {{ base.RemoveEventHandler(\"{0}\", value); }}", ent.Name);
                            Console.WriteLine("}");
                            Console.WriteLine();
                        }
                        Console.WriteLine();
                    }
                    var methods = type.GetMethods();
                    if (methods != null)
                    {
                        Console.WriteLine("// Methods");
                        Console.WriteLine();

                        foreach (var fun in type.GetMethods())
                        {
                            if (fun.Attributes.HasFlag(MethodAttributes.SpecialName)) continue;
                            if (fun.IsGenericMethod)
                            {
                                Console.WriteLine("// {0}", fun.ToString());
                                continue;
                            }
                            string ps_name = null;
                            var ps_value = "new object[] { }";
                            var ps_type = "null";
                            var ps = fun.GetParameters();
                            if (ps != null && ps.Length > 0)
                            {
                                ps_name = string.Join(", ", ps.Select(p => $"{mapTypeName(p.ParameterType)} {p.Name}"));
                                ps_value = $"new object[] {{ {string.Join(", ", ps.Select(p => p.Name))} }}";
                                ps_type = $"new Type[] {{ {string.Join(", ", ps.Select(p => $"typeof({mapTypeName(p.ParameterType)})"))} }}";
                            }
                            Console.WriteLine("public {1} {0}({2}) {{", fun.Name, mapTypeName(fun.ReturnType), ps_name);
                            if (fun.ReturnType != typeof(void)) Console.Write("return ");
                            Console.WriteLine("base.Invoke(\"{0}\", {1}, {2});", fun.Name, ps_value, ps_type);
                            Console.WriteLine("}");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("}");
                    Console.WriteLine("}");
                }
                else if (type.IsClass)
                {

                }
            }
        }
    }
}

namespace test_namepsace_proxy
{
    class ITestInterfaceProxy : _ProxyImpl
    {
        // Intrisic
        public override string FullName { get; } = "test_namepsace.ITestInterface";
        public ITestInterfaceProxy(object uo) : base(uo) { }

        // Properties

        public string ReadonlyProp
        {
            get { return (string)base.GetProperty("ReadonlyProp"); }
        }

        public string WriteonlyProp
        {
            set { base.SetProperty("WriteonlyProp", value); }
        }

        public string Property
        {
            get { return (string)base.GetProperty("Property"); }
            set { base.SetProperty("Property", value); }
        }


        // Events

        public delegate int TestEventHandler(object o, string e);
        public event TestEventHandler TestEvent
        {
            add { base.AddEventHandler("TestEvent", value); }
            remove { base.RemoveEventHandler("TestEvent", value); }
        }


        // Methods

        public object Method()
        {
            return base.Invoke("Method", new object[] { }, null);
        }
        public object Method(string p1)
        {
            return base.Invoke("Method", new object[] { p1 }, new Type[] { typeof(string) });
        }
        public object Method(int p1, bool p2)
        {
            return base.Invoke("Method", new object[] { p1, p2 }, new Type[] { typeof(int), typeof(bool) });
        }
        // T GenericMethod[T](System.Object)

    }
}
