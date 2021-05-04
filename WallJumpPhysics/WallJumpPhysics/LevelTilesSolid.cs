using PolyOne.Collision;
using PolyOne.Utility;


namespace WallJumpPhysics
{
    public class LevelTilesSolid : Solid
    {
        public Grid Grid { get; private set; }

        public LevelTilesSolid(bool[,] solidData)
        {
            this.Active = false;
            this.Collider = (this.Grid = new Grid(TileInformation.TileWidth, TileInformation.TileHeight, solidData));
        }
    }
}
