using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Data.Interfaces
{
    public delegate void OnThingStateChange();

    public interface IThing
    {
        event OnThingStateChange OnLocationChanged;
        event OnThingStateChange OnThingAdded;
        event OnThingStateChange OnThingRemoved;

        ushort ThingId { get; }

        byte Count { get; }

        Location Location { get; }

        ITile Tile { get; }

        string InspectionText { get; }

        string CloseInspectionText { get; }

        bool CanBeMoved { get; }

        void Added();
        void Removed();
    }
}
