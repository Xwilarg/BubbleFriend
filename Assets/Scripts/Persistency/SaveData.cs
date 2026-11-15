using Sketch.Persistency;
using System;
using System.Collections.Generic;

namespace BubbleFriend.Persistency
{
    public class SaveData : ISaveData
    {
        public DateTime LastCheck { set; get; } = DateTime.MinValue;
        public Dictionary<Need, int> Needs { set; get; } = null;
    }
}
