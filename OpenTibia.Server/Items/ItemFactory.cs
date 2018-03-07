using System;
using System.Collections.Generic;
using System.IO;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Server.Items
{
    public static class ItemFactory
    {
        public static object InitLock = new object();
        public static Dictionary<ushort, ItemType> ItemsCatalog { get; private set; }
        
        public static void Initialize()
        {
            if (ItemsCatalog != null)
            {
                return;
            }

            lock (InitLock)
            {
                if (ItemsCatalog == null)
                {
                    ItemsCatalog = Game.Instance.ItemLoader.Load(ServerConfiguration.ObjectsFileName);
                }
            }
        }

        public static Item Create(ushort typeId)
        {
            if (ItemsCatalog == null)
            {
                Initialize();

                if (ItemsCatalog == null)
                {
                    throw new InvalidOperationException("Failed to initialize ItemsCatalog.");
                }
            }

            if (typeId < 100 || !ItemsCatalog.ContainsKey(typeId))
            {
                return null;
                //throw new ArgumentException("Invalid type.", nameof(typeId));
            }

            if (ItemsCatalog[typeId].Flags.Contains(ItemFlag.Container) || ItemsCatalog[typeId].Flags.Contains(ItemFlag.Chest))
            {
                return new Container(ItemsCatalog[typeId]);
            }

            return new Item(ItemsCatalog[typeId]);
        }
    }
}
