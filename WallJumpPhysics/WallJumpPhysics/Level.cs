using Microsoft.Xna.Framework;

using PolyOne.Scenes;
using PolyOne.Engine;
using PolyOne.Utility;

using PolyOne.LevelProcessor;

namespace WallJumpPhysics
{
    public enum GameTags
    {
        Player = 1,
        Solid = 2
    }

    public class Level : Scene
    {
        Player player;

        LevelTilesSolid tiles;

        public LevelTiler Tile { get; private set; }
        LevelData levelData = new LevelData();
        bool[,] collisionInfo;


        public override void LoadContent()
        {
            base.LoadContent();

            Tile = new LevelTiler();

            levelData = Engine.Instance.Content.Load<LevelData>("Map");
            Tile.LoadContent(levelData);

            collisionInfo = LevelTiler.TileConverison(Tile.CollisionLayer, 2);
            tiles = new LevelTilesSolid(collisionInfo);
            this.Add(tiles);

            player = new Player(new Vector2(200, 200));
            this.Add(player);
            player.Added(this);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            Engine.Begin(Resolution.GetScaleMatrix);
            Tile.DrawBackground();
            base.Draw();
            Engine.End();
        }
    }
}
