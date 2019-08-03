using System.Numerics;
using Uchu.World.Collections;

namespace Uchu.World.Parsers
{
    public class SpawnerWaypoint : IPathWaypoint
    {
        public Vector3 Position { get; set; }
        public LegoDataDictionary Config { get; set; }

        public Quaternion Rotation { get; set; }
    }
}