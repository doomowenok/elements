using System;
using System.Collections.Generic;

namespace Core.Session
{
    [Serializable]
    public sealed class SessionSaveData
    {
        public int Level;
        public List<int> Elements;
    }
}