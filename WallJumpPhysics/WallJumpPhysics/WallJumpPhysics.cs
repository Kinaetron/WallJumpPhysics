using Microsoft.Xna.Framework;

using PolyOne.Engine;
using PolyOne.Utility;


namespace WallJumpPhysics
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class WallJumpPhysics : Engine
    {
        Level level = new Level();
        
            
        public WallJumpPhysics()
            :base(640, 360, "WallJumpPhysics", 2.0f, false)
        { }


        protected override void Initialize()
        {
            base.Initialize();
            TileInformation.TileDiemensions(16, 16);
            level.LoadContent();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            level.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            level.Draw();
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (WallJumpPhysics game = new WallJumpPhysics())
            {
                game.Run();
            }
        }
    }
}
