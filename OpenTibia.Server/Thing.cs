using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server
{
    public abstract class Thing : IThing
    {
        public event OnThingStateChange OnLocationChanged;
        public event OnThingStateChange OnThingAdded;
        public event OnThingStateChange OnThingRemoved;

        protected ITile _tile;
        protected Location _location;

        public const ushort CreatureThingId = 0x63;

        public abstract ushort ThingId { get; }

        public abstract byte Count { get; }

        public abstract string InspectionText { get; }

        public abstract string CloseInspectionText { get; }

        public abstract bool CanBeMoved { get; }

        public Location Location
        {
            get { return _location; }
            protected set
            {
                var oldValue = _location;
                _location = value;

                if (oldValue != _location)
                {
                    OnLocationChanged?.Invoke();
                }
            }
        }

        public ITile Tile
        {
            get { return _tile; }
            set
            {
                if (value != null)
                {
                    Location = value.Location;
                }

                _tile = value;
            }
        }

        public void Added()
        {
            OnThingAdded?.Invoke();
        }

        public void Removed()
        {
            OnThingRemoved?.Invoke();
        }
    }
}