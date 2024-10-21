using Core.Element;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Core.Grid
{
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