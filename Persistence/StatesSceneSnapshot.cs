using System;
using System.Collections.Generic;

namespace Amaryllis.Persistence
{
    [Serializable]
    public class StatesSceneSnapshot
    {
        public string Version = "1";
        public string CreatedUtc;
        public List<StatesObjectSnapshot> States = new List<StatesObjectSnapshot>();
    }
}
