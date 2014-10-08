#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion


namespace _1942Project
{
    public class Player
    {
        public Texture2D graphic;
        public Vector2 position;

        //---Keys---------------
        public Keys upKey;
        public Keys downKey;
        public Keys leftKey;
        public Keys rightKey;
        public Keys shootKey;
        //----------------------
        public Game1 game;

        public float bulletSpawnTime = 0.25f;
        public float bulletSpawnCooldownTime = 0.0f;

        //soundeffect
        //soundeffectinstance


        public Player(Texture2D texture, Vector2 pos, Game1 game)
        {
            graphic = texture; 
            position = pos;
            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            bulletSpawnCooldownTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(upKey)) position.Y -= 5f;
            if (Keyboard.GetState().IsKeyDown(downKey)) position.Y += 5f;
            if (Keyboard.GetState().IsKeyDown(leftKey)) position.X -= 5f;
            if (Keyboard.GetState().IsKeyDown(rightKey)) position.X += 5f;
            if (Keyboard.GetState().IsKeyDown(shootKey) && game.bullets.Count <= 3)
            {
                if (bulletSpawnCooldownTime < 0.0f)
                {
                    game.Shoot(new Vector2(position.X, position.Y + graphic.Height), new Vector2(0, -1), 10);
                    bulletSpawnCooldownTime = bulletSpawnTime;
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 offset = new Vector2(graphic.Width / 2, 0);
            spriteBatch.Draw(graphic, position - offset, Color.White);
        }

    }
}
