using System.Collections.Generic;
using System.Linq;
using Core.Element;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Core.Grid
{
    public class Matcher : IMatcher
    {
        public List<int2> FindMatches(GridGameElement[][] grid)
        {
            int elementsInRows = grid[0].Length;
            int length = grid.Length * elementsInRows;
            
            NativeArray<ElementType> elements = new NativeArray<ElementType>(length, Allocator.TempJob);
            NativeList<int2> result = new NativeList<int2>(length * 2, Allocator.TempJob);

            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if(grid[i][j] == null) continue;
                    
                    elements[i * elementsInRows + j] = grid[i][j].Type;
                }
            }

            FindMatchJob job = new FindMatchJob()
            {
                Elements = elements,
                Result = result.AsParallelWriter(),
                ElementsInRows = elementsInRows,
                Rows = grid.Length
            };

            JobHandle handle = job.Schedule(length, 64);
            handle.Complete();

            List<int2> resultWithDuplicates = new List<int2>(result.Length);

            for (int i = 0; i < result.Length; i++)
            {
                resultWithDuplicates.Add(result[i]);
            }
            
            List<int2> finalResult = resultWithDuplicates.Distinct().ToList();

            elements.Dispose();
            result.Dispose();

            return finalResult;
        }
    }
}