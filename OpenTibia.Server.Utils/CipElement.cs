using System.Collections.Generic;

namespace OpenTibia.Utilities
{
    public class CipElement
    {
        public CipElement(int data)
        {
            Data = data;
            Attributes = new List<CipAttribute>();
        }

        public int Data { get; set; }
        public IList<CipAttribute> Attributes { get; set; }
    }
}
