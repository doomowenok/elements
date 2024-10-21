using System.Collections.Generic;
using System.Text;
using Core.Element;
using Extensions.Property;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Session
{
    public class SessionController
    {
        public INotifyProperty<int> Level = new NotifyProperty<int>();
        public GridGameElement[][] Elements { get; set; }
        public Vector3[][] Positions { get; set; }

        public void FillElements(IEnumerable<GridGameElement> elements)
        {
            foreach (GridGameElement element in elements)
            {
                int2 index = element.GridIndex;
                Elements[index.x][index.y] = element;
            }
        }

        public void ClearElements()
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                for (int j = 0; j < Elements[i].Length; j++)
                {
                    Elements[i][j] = null;
                }
            }
        }

        public bool IsElementsEmpty()
        {
            for (int i = 0; i < Elements[0].Length; i++)
            {
                if (Elements[0][i] != null)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            
            builder.Append("\n");

            for (int i = Elements.Length - 1; i >= 0; i--)
            {
                string elements = string.Empty;
                
                for (int j = 0; j < Elements[i].Length; j++)
                {
                    int elementType = Elements[i][j] == null ? (int)ElementType.None : (int)Elements[i][j].Type;
                    elements = string.Concat(elements, elementType + " ");
                }
                
                builder.Append(elements);
                builder.Append("\n");
            }
            
            return builder.ToString();
        }
    }
}