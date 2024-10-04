namespace Core.Utils
{
    public sealed class RenderOrderHelper : IRenderOrderHelper
    {
        private const int BaseRenderOrder = 5;
        
        public int GetRenderOrder(int row, int column)
        {
            return BaseRenderOrder + row + column;
        }
    }
}