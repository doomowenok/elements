using VContainer;

namespace Infrastructure.Installers
{
    public abstract class InstallerBase
    {
        public abstract void Install(IContainerBuilder builder);
    }
}