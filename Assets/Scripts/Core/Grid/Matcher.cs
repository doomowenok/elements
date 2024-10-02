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
    
    public struct FindMatchJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<ElementType> Elements;
        public NativeList<int2>.ParallelWriter Result;
        public int ElementsInRows;
        public int Rows;

        public void Execute(int index)
        {
            int row = index / ElementsInRows;
            int column = index % ElementsInRows;
            
            if (Elements[index] == ElementType.None) return;

            if (column > 0 && column < ElementsInRows - 1)
            {
                int horizontalLeft = index - 1;
                int horizontalCenter = index;
                int horizontalRight = index + 1;
            
                if (Elements[horizontalCenter] == Elements[horizontalLeft] &&
                    Elements[horizontalCenter] == Elements[horizontalRight])
                {
                    int2 left = new int2(horizontalLeft / ElementsInRows, horizontalLeft % ElementsInRows);
                    int2 center = new int2(horizontalCenter / ElementsInRows, horizontalCenter % ElementsInRows);
                    int2 right = new int2(horizontalRight / ElementsInRows, horizontalRight % ElementsInRows);
                    
                    Result.AddNoResize(left);
                    Result.AddNoResize(center);
                    Result.AddNoResize(right);
                }
            }

            if (row > 0 && row < Rows - 1)
            {
                int verticalDown = index - ElementsInRows;
                int verticalCenter = index;
                int verticalUp = index + ElementsInRows;
                
                if (Elements[verticalCenter] == Elements[verticalDown] &&
                    Elements[verticalCenter] == Elements[verticalUp])
                {
                    int2 down = new int2(verticalDown / ElementsInRows, verticalDown % ElementsInRows);
                    int2 center = new int2(verticalCenter / ElementsInRows, verticalCenter % ElementsInRows);
                    int2 up = new int2(verticalUp / ElementsInRows, verticalUp % ElementsInRows);
                    
                    Result.AddNoResize(down);
                    Result.AddNoResize(center);
                    Result.AddNoResize(up);
                }
            }
        }
    }
}