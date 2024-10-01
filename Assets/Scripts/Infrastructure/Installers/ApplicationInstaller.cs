using Core;
using Infrastructure.Boot;
using Infrastructure.FSM.Application;
using Infrastructure.FSM.Application.States;
using Infrastructure.FSM.Factory;
using Services;
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

            builder.Register<BootstrapState>(Lifetime.Singleton);
            builder.Register<LoadLevelState>(Lifetime.Singleton);
            builder.Register<LevelState>(Lifetime.Singleton);

            builder.Register<StateFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ApplicationStateMachine>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.Register<SceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ConfigProvider>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<GameCreator>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}