using System;
using System.Collections.Generic;

namespace Core.Session
{
    [Serializable]
    public sealed class SessionSaveData
    {
        public int Level;
        public int Rows;
        public int Columns;
        public List<int> Elements;
    }
}