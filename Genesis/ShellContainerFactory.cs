using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Genesis.DynamicProxy;

namespace Genesis
{
    public class ShellContainerFactory : IShellContainerFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public ShellContainerFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public ILifetimeScope CreateScope(string[] features)
        {
            //get dependencies from features.
            Assembly[] assemblies = (from feature in features select Assembly.Load(feature)).ToArray();
            var filterTypes = (from assembly in assemblies select assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && !t.IsNotPublic)).ToArray();

            return _lifetimeScope.BeginLifetimeScope(delegate(ContainerBuilder builder)
            {
                foreach (var assemblyTypes in filterTypes)
                {
                    //register the viewmodel and the manager for all type.
                    foreach (Type extensionType in from t in assemblyTypes where typeof(IDependency).IsAssignableFrom(t) select t)
                    {
                        IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration;

                        if (typeof(IService).IsAssignableFrom(extensionType))
                        {
                           registration = this.RegisterType(builder, extensionType).EnableInterfaceInterceptors().InstancePerLifetimeScope();
                        }
                        else
                        {
                            registration = this.RegisterType(builder, extensionType).InstancePerLifetimeScope();
                        }

                        foreach (Type type in from itf in extensionType.GetInterfaces() where typeof(IDependency).IsAssignableFrom(itf) select itf)
                        {
                            registration = registration.As(new Type[] { type });
                            registration = registration.InstancePerDependency();

                            if (typeof(IViewModel).IsAssignableFrom(type) )
                            {
                                builder.RegisterType(extensionType);
                            }

                            if (typeof(IServiceWithoutInterceptor).IsAssignableFrom(type))
                            {
                                builder.RegisterType(extensionType);
                            }
                            
                            if (typeof(IService).IsAssignableFrom(type))
                            {
                                builder.RegisterType(extensionType).EnableInterfaceInterceptors();
                            }

                            if (typeof(IEntityDao).IsAssignableFrom(type))
                            {
                                builder.RegisterType(extensionType);
                            }

                            if (typeof(IPrintManager).IsAssignableFrom(type))
                            {
                                builder.RegisterType(extensionType);
                            }
                        }
                    }
                }
            });
        }

        private IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(ContainerBuilder builder, Type type)
        {
            return builder.RegisterType(type).WithProperty<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>("Type", type).WithMetadata("Type", type);
        }
    }
}
