using System;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Scripting
{
    public static class CustomConvertersFactory
    {
        internal static IConverter GetConverter(Type newType)
        {
            if (newType == typeof(Location))
            {
                return new LocationConverter();
            }

            return null;
        }
    }
}