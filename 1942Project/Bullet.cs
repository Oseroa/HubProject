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
    public class Bullet
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 direction;
        public float speed;

        public Bullet(Texture2D texture, Vector2 position, Vector2 direction, float speed)
        {
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
        }

        public void Update(GameTime gameTime)
        {
            position -= direction * speed;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 offset = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteBatch.Draw(texture, position - offset, Color.White);
        }

    }
}
