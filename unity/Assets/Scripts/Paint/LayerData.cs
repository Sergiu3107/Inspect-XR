using System.Collections.Generic;
using UnityEngine;

namespace Paint
{
    [System.Serializable]
    public class LayerData
    {
        public long id;
        public long createdById;
        public long modelId;
        public long sessionId;
        public string data;
        public string texture;
        public Color color;
    }
}