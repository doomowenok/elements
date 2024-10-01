using Infrastructure.Boot;
using Infrastructure.FSM.Application;
using Infrastructure.FSM.Factory;
using Services.Scene;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Installers
{
    public sealed class ApplicationInstaller : LifetimeScope
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>();

            builder.Register<StateFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ApplicationStateMachine>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<SceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}