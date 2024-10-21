using System;
using Infrastructure.Installers;
using VContainer;
using VContainer.Unity;

namespace Core.UI.Level
{
    public class LevelUIInstaller : InstallerBase
    {
        public override void Install(IContainerBuilder builder)
        {
            builder.Register<LevelViewModel>(Lifetime.Singleton);
            builder.Register<LevelModel>(Lifetime.Singleton).As<LevelModel, IInitializable, IDisposable>();
        }
    }
}