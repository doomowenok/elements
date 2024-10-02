using Core.Element;
using UnityEngine;

namespace Core.Session
{
    public class SessionData
    {
        public int Level;
        public GridGameElement[][] Elements { get; set; }
        public Vector3[][] Positions { get; set; }
    }
}