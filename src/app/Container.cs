

///
/// http://www.kenegozi.com/Blog/2008/01/17/its-my-turn-to-build-an-ioc-container-in-15-minutes-and-33-lines.aspx
/// http://ayende.com/Blog/archive/2007/10/20/Building-an-IoC-container-in-15-lines-of-code.aspx
///

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeSoda.Impression {

	public delegate object Creator(Container container);

	public interface IContainer
	{
		IDictionary<string, object> Configuration { get; }
		T GetConfiguration<T>(string name);
		IContainer Register<TContract>(Creator creator);
		IContainer Register<TContract>(TContract implementation);
		IContainer Register<TContract, TImplementation>();
		TContract Resolve<TContract>();
		object Resolve(Type contract);
	}

	public partial class Container : IContainer, IDisposable
	{

		private IDictionary<Type, object> instances = null;
		private IDictionary<Type, Creator> creators = null;
		private IDictionary<Type, Type> types = null;

		private IDictionary<string, object> configuration = null;

		public Container() {
			instances = new Dictionary<Type, object>();
			creators = new Dictionary<Type, Creator>();
			types = new Dictionary<Type, Type>();
			configuration = new Dictionary<string, object>();
		}

		#region Configuration

		public IDictionary<string, object> Configuration {
			get { return configuration; }
		}

		public T GetConfiguration<T>(string name) {
			return (T)configuration[name];
		}

		#endregion configuration


		#region Register

		/// <summary>
		/// Register a Creator delegate to create the object
		/// </summary>
		/// <typeparam name="TContract">The type of object to be created</typeparam>
		/// <param name="creator">The delegate to be called to create the Object of type T</param>
		/// <returns>IContainer for stringing Register calls together</returns>
		public IContainer Register<TContract>(Creator creator) {
			creators.Add(typeof(TContract), creator);
			return this;
		}

		/// <summary>
		/// Register a singleton instance to be return when resolving TContract
		/// </summary>
		/// <typeparam name="TContract">The type of the instance to be registered</typeparam>
		/// <param name="instance">The actual instance of TContract to register</param>
		/// <returns>IContainer for stringing Register calls together</returns>
		public IContainer Register<TContract>(TContract instance) {
			instances.Add(typeof(TContract), instance);
			return this;
		}

		/// <summary>
		/// Register an implementation of a contract to be instantiated each time it is resolved
		/// </summary>
		/// <typeparam name="TContract">The type of object to be created</typeparam>
		/// <typeparam name="TImplementation">The actual type of TContract to be instantiated</typeparam>
		/// <returns>IContainer for stringing Register calls together</returns>
		public IContainer Register<TContract, TImplementation>() {
			types[typeof(TContract)] = typeof(TImplementation);
			return this;
		}

		#endregion register


		#region Resolve

		/// <summary>
		/// Looks up an object within the container, based on the design time Type
		/// </summary>
		/// <typeparam name="TContract">The contract to lookup</typeparam>
		/// <returns>An instance of TContract or null if not found</returns>
		public TContract Resolve<TContract>() {
			return (TContract)Resolve(typeof(TContract));
		}

		/// <summary>
		/// Looks up an object within the container based on the runtime name of the Type
		/// </summary>
		/// <param name="contractTypeName">The namer of the contract to lookup</param>
		/// <returns>An instance of TContract or null if not found</returns>
		public object Resolve(string contractTypeName) {
			Type contract = Type.GetType(contractTypeName, true);
			return Resolve(contract);
		}

		/// <summary>
		/// Looks up an object within the container, based on the runtime Type
		/// </summary>
		/// <param name="contract"></param>
		/// <returns>An instance of TContract or null if not found</returns>
		public object Resolve(Type contract) {
			// see if we have a specific instance available
			object obj = instances.ContainsKey(contract) ? instances[contract] : null;
			if (obj != null) {
				return obj;
			}

			// see if we have a creator
			Creator creator = creators.ContainsKey(contract) ? creators[contract] : null;
			if (creator != null) {
				return creator(this);
			}

			// no creator, see if we can create object using activator/constructor
			Type implementation = types.ContainsKey(contract) ? types[contract] : null;

			// no creator, no implementation, is it a concrete class ?
			if (implementation == null & !contract.IsInterface && !contract.IsAbstract) {
				// try just use the class itself
				implementation = contract;
			}

			if (implementation != null) {
				ConstructorInfo constructor = implementation.GetConstructors()[0];

				ParameterInfo[] constructorParameters = constructor.GetParameters();

				if (constructorParameters.Length == 0)
					return Activator.CreateInstance(implementation);

				List<object> parameters = new List<object>(constructorParameters.Length);

				foreach (ParameterInfo parameterInfo in constructorParameters)
					parameters.Add(Resolve(parameterInfo.ParameterType));

				return constructor.Invoke(parameters.ToArray());
			}

			// no creator, no implementation, return null
			return null;

		}

		#endregion Resolve

		#region IDisposable Members

		public void Dispose() {
			foreach (object instance in instances) {
				if (instance is IDisposable) {
					((IDisposable)instance).Dispose();
				}
			}
		}

		#endregion
	}
}

//public interface IFileSystemAdapter { }
//public class FileSystemAdapter : IFileSystemAdapter { }
//public interface IBuildDirectoryStructureService { }
//public class BuildDirectoryStructureService : IBuildDirectoryStructureService
//{
//    IFileSystemAdapter fileSystemAdapter;
//    public BuildDirectoryStructureService(IFileSystemAdapter fileSystemAdapter)
//    {
//        this.fileSystemAdapter = fileSystemAdapter;
//    }
//}


//IoC.Register<IFileSystemAdapter, FileSystemAdapter>();
//IoC.Register<IBuildDirectoryStructureService, BuildDirectoryStructureService>(); 
//IBuildDirectoryStructureService service = IoC.Resolve<IBuildDirectoryStructureService>();



////////////////////////////////////////////////////////////////////////////////////

//public class DemoContainer
//{
//    public delegate object Creator(DemoContainer container);

//    private readonly Dictionary<string, object> configuration
//                   = new Dictionary<string, object>();
//    private readonly Dictionary<Type, Creator> typeToCreator
//                   = new Dictionary<Type, Creator>();

//    public Dictionary<string, object> Configuration
//    {
//        get { return configuration; }
//    }

//    public void Register<T>(Creator creator)
//    {
//        typeToCreator.Add(typeof(T), creator);
//    }

//    public T Create<T>()
//    {
//        return (T)typeToCreator[typeof(T)](this);
//    }

//    public T GetConfiguration<T>(string name)
//    {
//        return (T)configuration[name];
//    }
//}



//foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
//    if (typeof(IController).IsAssignableFrom(t))
//        builder.Register(t).FactoryScoped();

// make container implement iDisposable

// 3 types of register levels
// singleton - at most only ever 1 instance in the container, on container disposing - dispose any singletons which implement idisposable

// factory - A factory-scoped component will be disposed along with the container in which it was requested
// builder.Register(c => new MyClass()).FactoryScoped();
// var a = container.Resolve<MyClass>();
// var b = container.Resolve<MyClass>();
// Assert.AreNotSame(a, b);


/*
var a = container.Resolve<MyClass>();
var b = container.Resolve<MyClass>();
Assert.AreSame(a, b);

var inner = container.CreateInnerContainer();
var c = inner.Resolve<MyClass>();
Assert.AreNotSame(a, c);
*/







/*
public class DemoContainer
{
	public delegate object Creator(DemoContainer container);

	private readonly Dictionary<string, object> configuration 
                   = new Dictionary<string, object>();
	private readonly Dictionary<Type, Creator> typeToCreator 
                   = new Dictionary<Type, Creator>();

	public Dictionary<string, object> Configuration
	{
		get { return configuration; }
	}

	public void Register<T>(Creator creator)
	{
		typeToCreator.Add(typeof(T),creator);
	}

	public T Create<T>()
	{
		return (T) typeToCreator[typeof (T)](this);
	}

	public T GetConfiguration<T>(string name)
	{
		return (T) configuration[name];
	}
}

DemoContainer container = new DemoContainer();
//registering dependecies
container.Register<IRepository>(delegate
{
	return new NHibernateRepository();
});
container.Configuration["email.sender.port"] = 1234;
container.Register<IEmailSender>(delegate
{
	return new SmtpEmailSender(container.GetConfiguration<int>("email.sender.port"));
});
container.Register<LoginController>(delegate
{
	return new LoginController(
		container.Create<IRepository>(),
		container.Create<IEmailSender>());
});

//using the container
Console.WriteLine(
	container.Create<LoginController>().EmailSender.Port
	);

 */