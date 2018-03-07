using System.Collections.Generic;
using System.IO;

namespace OpenTibia.Utilities
{
	public class BytePair
	{
		public byte Key { get; set; }
		public byte Value { get; set; }
		
		public BytePair(byte b1, byte b2)
		{
			Key = b1;
			Value = b2;
		}
		
		public override string ToString()
		{
			return Key + " :: " + Value; 
		}
			
		public static byte[] Serialize(List<BytePair> list)
		{
            var data = new MemoryStream();
			
            data.WriteByte((byte)list.Count);
            
            foreach(var pair in list)
            {
            	data.WriteByte(pair.Key);
            	data.WriteByte(pair.Value);
            }
            
            return data.ToArray();
		}
		
		public static List<BytePair> Unserialize(byte[] dataArray)
		{
			var data = new MemoryStream(dataArray);
			
			var list = new List<BytePair>();
			
			var count = data.ReadByte();
			
			for(var i = 0; i < count; i++)
			{
				var key = (byte)data.ReadByte();
				var val = (byte)data.ReadByte();
				
				var pair = new BytePair(key, val);
				
				list.Add(pair);
			}
			
			return list;
		}
	
	}
}
