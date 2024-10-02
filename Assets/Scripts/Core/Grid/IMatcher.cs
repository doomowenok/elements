using System.Collections.Generic;
using Core.Element;
using Unity.Mathematics;

namespace Core.Grid
{
    public interface IMatcher
    {
        List<int2> FindMatches(GridGameElement[][] grid);
    }
}