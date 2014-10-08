using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;


namespace _1942Project
{
    class Powerup
    {
        public Texture2D texture;
        public Vector2 position;

        public Powerup(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public void Update(GameTime gameTime)
        {
            position.Y -= 8f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 offset = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteBatch.Draw(texture, position - offset, Color.White);
        }

    }
}
