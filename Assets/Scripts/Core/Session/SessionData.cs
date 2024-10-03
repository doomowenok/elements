using System.Text;
using Core.Element;
using UnityEngine;

namespace Core.Session
{
    public class SessionData
    {
        public int Level;
        public GridGameElement[][] Elements { get; set; }
        public Vector3[][] Positions { get; set; }

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