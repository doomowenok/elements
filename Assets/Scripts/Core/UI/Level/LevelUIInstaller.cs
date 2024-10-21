using Infrastructure.Installers;
using VContainer;

namespace Core.UI.Level
{
    public class LevelUIInstaller : InstallerBase
    {
        public override void Install(IContainerBuilder builder)
        {
            builder.Register<LevelViewModel>(Lifetime.Singleton);
            builder.Register<LevelModel>(Lifetime.Singleton);
        }
    }
}