using System;
using System.Windows;
using Autofac;
using Genesis.Events;
using Genesis.Logging;
using Genesis.UI;

namespace Genesis
{
    public class Bootstrapper
    {
        public ILifetimeScope Scope { get; set; }

        public void Initialize(Action<ContainerBuilder> registration, string[] features)
        {
            Scope = createContainer(registration).Resolve<IShellContainerFactory>().CreateScope(features);
        }

        public void Run(Func<ILifetimeScope, DependencyObject> createShell, Action<ILifetimeScope> featureInitialize)
        {
            if (null == Scope)
            {
                throw new NullReferenceException("The environment is not yet initialized, please call Initialize first.");
            }

            if (null != createShell)
            {
                Zone.SetZoneManager(createShell(Scope), Scope.Resolve<IZoneManager>());
            }

            if (null != featureInitialize)
            {
                featureInitialize(Scope);
            }
        }

        private static IContainer createContainer(Action<ContainerBuilder> registrations)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<GenesisLogger>().As<ILogger>().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<Zone>().As<IZone>().SingleInstance();
            builder.RegisterType<DefaultViewLocator>().As<IViewLocator>().SingleInstance();
            builder.RegisterType<DefaultZoneManager>().As<IZoneManager>().SingleInstance();
            builder.RegisterType<ShellContainerFactory>().As<IShellContainerFactory>().SingleInstance();

            registrations(builder);
            var container = builder.Build();
            var updater = new ContainerBuilder();
            updater.RegisterInstance(container).As<IContainer>();
            updater.Update(container);
            return container;
        }
    }
}
