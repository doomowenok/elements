using Core.Balloon;
using Core.Balloon.Factory;
using Core.Boot;
using Core.Element.Factory;
using Core.Grid;
using Core.Input;
using Core.Save;
using Core.Session;
using Core.UI.Level;
using Core.Utils;
using Core.Win;
using Infrastructure.Boot;
using Infrastructure.FSM.Application;
using Infrastructure.FSM.Application.States;
using Infrastructure.FSM.Factory;
using Infrastructure.UI;
using Infrastructure.UI.Factory;
using Services;
using Services.Save;
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
            RegisterEntryPoint(builder);
            RegisterApplicationStates(builder);
            RegisterFactories(builder);
            RegisterProviders(builder);
            RegisterCommonServices(builder);
            RegisterGameplayServices(builder);
            
            CreateInstaller<LevelUIInstaller>(builder);
        }

        private static void RegisterGameplayServices(IContainerBuilder builder)
        {
            builder.Register<Matcher>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<RenderOrderHelper>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<TransformCalculator>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CollisionDetector>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GridElementController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GridRecalculationController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<WinChecker>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameBalloonController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameplayDisposer>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SessionController>(Lifetime.Singleton).AsSelf();
        }

        private static void RegisterCommonServices(IContainerBuilder builder)
        {
            builder.Register<SceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InputSystem>(Lifetime.Singleton).As<ITickable, IInputSystem>();
            builder.Register<UIController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ApplicationStateMachine>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlayerPrefsSaveService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SessionSaver>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameCreator>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private static void RegisterProviders(IContainerBuilder builder)
        {
            builder.Register<ConfigProvider>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private static void RegisterFactories(IContainerBuilder builder)
        {
            builder.Register<StateFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UIFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<BalloonFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GridGameElementFactory>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>();
        }

        private static void RegisterApplicationStates(IContainerBuilder builder)
        {
            builder.Register<BootstrapState>(Lifetime.Singleton);
            builder.Register<LoadLevelState>(Lifetime.Singleton);
            builder.Register<LevelState>(Lifetime.Singleton);
            builder.Register<IncreaseLevelState>(Lifetime.Singleton);
            builder.Register<RestartLevelState>(Lifetime.Singleton);
        }

        private static void CreateInstaller<TInstaller>(IContainerBuilder builder) where TInstaller : InstallerBase, new()
        {
            TInstaller installer = new TInstaller();
            installer.Install(builder);
        }
    }
}