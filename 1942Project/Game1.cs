#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace _1942Project
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum EGameState
        {
            MENU,
            GAME,
            NOLIVES,
            CREDITS
        }

        EGameState gameState = EGameState.MENU;
        //480x800 resolution
        Texture2D player1Texture;
        Texture2D bulletTexture;
        Texture2D enemyTexture;
        Texture2D bossTexture;
        Texture2D menuBackgroundTexture;
        Texture2D gameBackgroundTexture;
        Texture2D gameBackgroundTextureSecondary;
        Texture2D creditsBackgroundTexture;
        Texture2D noLivesBackgroundTexture;
        Texture2D winnerBackgroundTexture;
        Texture2D lifeSymbol;
        Texture2D powerupTexture;
        Texture2D powerupstrongTexture;

        SoundEffect enemyDieSound;
        SoundEffect gameOverSound;
        SoundEffect laserSound;
        SoundEffect playerDieSound;
        SoundEffect winnerSound;

        Vector2 creditsBackgroundPosition = new Vector2(0, 800);
        Vector2 secondaryCreditsBackgroundPosition = new Vector2(0, 0);
        Vector2 initialGameBackgroundPosition = new Vector2(0, 0);
        Vector2 secondaryGameBackgroundPosition = new Vector2(0, 800);

        SpriteFont basicFont;

        Player player1;
        GameBackground initialBackground;
        GameBackground secondaryBackground;
        GameBackground creditsBackground;
        GameBackground secondaryCreditsBackground;

        public List<Bullet> bullets = new List<Bullet>();
        List<Enemy> enemies = new List<Enemy>();
        List<Powerup> powerups = new List<Powerup>();
        List<Powerup> powerupsstrong = new List<Powerup>();


        Random rand = new Random();

        float enemySpawnTime = 3.0f;
        float enemySpawnCooldownTime = 0.0f;

        float powerupSpawnTime = 15.0f;
        float powerupSpawnCooldownTime = 0.0f;

        float powerupstrongSpawnTime = 50.0f;
        float powerupstrongSpawnCooldownTime = 0.0f;

        int player1Lives = 5;
        int player1Score = 0;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            player1Texture = Content.Load<Texture2D>("player1lvl1.png");
            bulletTexture = Content.Load<Texture2D>("bullet.png");
            enemyTexture = Content.Load<Texture2D>("enemy1.png");
            bossTexture = Content.Load<Texture2D>("boss.png");
            menuBackgroundTexture = Content.Load<Texture2D>("BetterGraphics/MenuBackground.png");
            gameBackgroundTexture = Content.Load<Texture2D>("dodgybackground.png");
            gameBackgroundTextureSecondary = Content.Load<Texture2D>("dodgybackgroundwords.png");
            creditsBackgroundTexture = Content.Load<Texture2D>("BetterGraphics/CreditsBackground.png");
            noLivesBackgroundTexture = Content.Load<Texture2D>("BetterGraphics/GameOverBackground.png");
            winnerBackgroundTexture = Content.Load<Texture2D>("BetterGraphics/WinnerBackground.png");
            lifeSymbol = Content.Load<Texture2D>("dodgyheart.png");
            basicFont = Content.Load<SpriteFont>("BasicFont");
            powerupTexture = Content.Load<Texture2D>("dodgypowerup.png");
            powerupstrongTexture = Content.Load<Texture2D>("dodgypowerupstrong.png");

            enemyDieSound = Content.Load<SoundEffect>("Audio/EnemyDie.wav");
            gameOverSound = Content.Load<SoundEffect>("Audio/GameOver.wav");
            laserSound = Content.Load<SoundEffect>("Audio/LaserSound.wav");
            playerDieSound = Content.Load<SoundEffect>("Audio/PlayerDie.wav");
            winnerSound = Content.Load<SoundEffect>("Audio/Winner.wav");

            player1 = new Player(player1Texture, new Vector2(200, 300), this);

            initialBackground = new GameBackground(gameBackgroundTexture, initialGameBackgroundPosition);
            secondaryBackground = new GameBackground(gameBackgroundTextureSecondary, secondaryGameBackgroundPosition);
            creditsBackground = new GameBackground(creditsBackgroundTexture, creditsBackgroundPosition);
            secondaryCreditsBackground = new GameBackground(creditsBackgroundTexture, secondaryCreditsBackgroundPosition);


            player1.upKey = Keys.W;
            player1.downKey = Keys.S;
            player1.leftKey = Keys.A;
            player1.rightKey = Keys.D;
            player1.shootKey = Keys.Space;

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            switch (gameState)
            {
                case EGameState.MENU: UpdateMenu(gameTime); break;
                case EGameState.GAME: UpdateGame(gameTime); break;
                case EGameState.NOLIVES: UpdateNoLives(gameTime); break;
                case EGameState.CREDITS: UpdateCredits(gameTime); break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            switch (gameState)
            {
                case EGameState.MENU: DrawMenu(gameTime); break;
                case EGameState.GAME: DrawGame(gameTime); break;
                case EGameState.NOLIVES: DrawNoLives(gameTime); break;
                case EGameState.CREDITS: DrawCredits(gameTime); break;

            }

            base.Draw(gameTime);
        }

        public void Shoot(Vector2 position, Vector2 direction, float speed)
        {
            Bullet b = new Bullet(bulletTexture, position, direction, speed);
            bullets.Add(b);
            laserSound.Play();
        }

        public void SpawnEnemy()
        {
            float randomX = rand.Next(0, graphics.PreferredBackBufferWidth);
            float randomY = rand.Next(graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferHeight + 100);
            Enemy e = new Enemy(enemyTexture, new Vector2(randomX, randomY), 5f);
            enemies.Add(e);
        }

        public void SpawnPowerup()
        {
            float randomX = rand.Next(0, graphics.PreferredBackBufferWidth);
            float randomY = rand.Next(graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferHeight + 100);
            Powerup e = new Powerup(powerupTexture, new Vector2(randomX, randomY));
            powerups.Add(e);
        }

        public void SpawnPowerupStrong()
        {
            float randomX = rand.Next(0, graphics.PreferredBackBufferWidth);
            float randomY = rand.Next(graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferHeight + 100);
            Powerup e = new Powerup(powerupstrongTexture, new Vector2(randomX, randomY));
            powerupsstrong.Add(e);
        }

        public void CheckCollisions()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    float distance = (bullets[i].position - enemies[j].position).Length();
                    if (distance < (bulletTexture.Height / 2.0f + enemyTexture.Height / 2.0f))
                    {
                        bullets.RemoveAt(i);
                        enemies.RemoveAt(j);
                        player1Score += 100;
                        enemyDieSound.Play();

                        break;
                    }
                }
            }
            for (int j = enemies.Count - 1; j >= 0; j--)
            {
                float distance = (enemies[j].position - player1.position).Length();
                if (distance < (enemyTexture.Height /2.0f + player1Texture.Height / 2.0f))
                {
                    player1Lives -= 1;
                    playerDieSound.Play();
                    player1.position = new Vector2(240, 100 - player1Texture.Height);
                    enemies.RemoveAt(j);
                }
            }
            for (int k = powerups.Count - 1; k >= 0; k--)
            {
                float distance = (powerups[k].position - player1.position).Length();
                if (distance < (powerupTexture.Height / 2.0f + player1Texture.Height / 2.0f))
                {
                    player1Score += 50;
                    powerups.RemoveAt(k);
                }
            }
            for (int k = powerupsstrong.Count - 1; k >= 0; k--)
            {
                float distance = (powerupsstrong[k].position - player1.position).Length();
                if (distance < (powerupstrongTexture.Height / 2.0f + player1Texture.Height / 2.0f))
                {
                    player1Score += 500;
                    powerupsstrong.RemoveAt(k);
                }
            }

        }

        public void Reset()
        {
            gameState = EGameState.GAME;
            player1.position = new Vector2(240, 100 - player1Texture.Height);

            player1Lives = 5;
            player1Score = 0;
        }

        //after voids
        public void UpdateMenu(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Reset();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                gameState = EGameState.CREDITS;
            }
        }

        public void DrawMenu(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(menuBackgroundTexture, new Vector2(0, 0), Color.White);
            spriteBatch.End();
        }

        public void UpdateGame(GameTime gameTime)
        {
            //-----State Switches-----------------------
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                gameState = EGameState.MENU;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                Reset();
            }
            if (player1Lives <= 0 || player1Score >= 25000)
            {
                gameState = EGameState.NOLIVES;

                for (int i = bullets.Count - 1; i >= 0; i--)
                {
                    bullets.RemoveAt(i);
                }

                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    enemies.RemoveAt(i);
                }

                for (int i = powerups.Count - 1; i >= 0; i--)
                {
                    powerups.RemoveAt(i);
                }

                for (int i = powerupsstrong.Count - 1; i >= 0; i--)
                {
                    powerupsstrong.RemoveAt(i);
                }
                if (player1Lives < 25000)
                {
                    gameOverSound.Play();
                }
                if (player1Lives >= 25000)
                {
                    winnerSound.Play();
                }
            }
            //------------------------------------------

            player1.Update(gameTime);
            initialBackground.Update(gameTime);
            secondaryBackground.Update(gameTime);
            CheckCollisions();

            //------Spawns--------------------------------------------------------------
            enemySpawnCooldownTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (enemySpawnCooldownTime < 0.0f)
            {
                SpawnEnemy();
                if (player1Score < 1000)
                {
                    enemySpawnCooldownTime = enemySpawnTime;
                }
                if (player1Score >= 1000 && player1Score < 2000)
                {
                    enemySpawnCooldownTime = enemySpawnTime - 1;
                }
                if (player1Score >= 2000 && player1Score < 3500)
                {
                    enemySpawnCooldownTime = enemySpawnTime - 2;
                }
                if (player1Score >= 3500 && player1Score < 10000)
                {
                    enemySpawnCooldownTime = enemySpawnTime - 2.5f;
                }
                if (player1Score >= 10000)
                {
                    enemySpawnCooldownTime = enemySpawnTime - 2.75f;
                }
            }

            powerupSpawnCooldownTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (powerupSpawnCooldownTime < 0.0f)
            {
                SpawnPowerup();
                powerupSpawnCooldownTime = powerupSpawnTime;
            }

            powerupstrongSpawnCooldownTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (powerupstrongSpawnCooldownTime < 0.0f)
            {
                SpawnPowerupStrong();
                powerupstrongSpawnCooldownTime = powerupstrongSpawnTime;
            }

            //-------------------------------------------------------------------------

            //here i; how long to loop for; -1 when reach bottom of list

            //----Removing Items Off Screen-----------------------------------------
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(gameTime);

                if (bullets[i].position.X < 0 ||
                    bullets[i].position.X > graphics.PreferredBackBufferWidth ||
                    bullets[i].position.Y < 0 ||
                    bullets[i].position.Y > graphics.PreferredBackBufferHeight ||
                    Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    bullets.RemoveAt(i);
                }
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].position.Y < 0 ||
                    Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    enemies.RemoveAt(i);
                    player1Score -= 10;
                }
            }

            for (int i = powerups.Count - 1; i >= 0; i--)
            {
                powerups[i].Update(gameTime);

                if (powerups[i].position.Y < 0 ||
                    Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    powerups.RemoveAt(i);
                }
            }

            for (int i = powerupsstrong.Count - 1; i >= 0; i--)
            {
                powerupsstrong[i].Update(gameTime);

                if (powerupsstrong[i].position.Y < 0 ||
                    Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    powerupsstrong.RemoveAt(i);
                }
            }

            //----------------------------------------------
        }

        public void DrawGame(GameTime gameTime)
        {
            spriteBatch.Begin();
            initialBackground.Draw(gameTime, spriteBatch);
            secondaryBackground.Draw(gameTime, spriteBatch);

            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Draw(gameTime, spriteBatch);
            }

            for (int i = 0; i < powerups.Count; i++)
            {
                powerups[i].Draw(gameTime, spriteBatch);
            }

            for (int i = 0; i < powerupsstrong.Count; i++)
            {
                powerupsstrong[i].Draw(gameTime, spriteBatch);
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(gameTime, spriteBatch);
            }

            player1.Draw(spriteBatch);

            //drawlives----------------------------------------------------------
            if (player1Lives >= 2) spriteBatch.Draw(lifeSymbol, new Vector2(475 - lifeSymbol.Width, 5), Color.White);
            if (player1Lives >= 3) spriteBatch.Draw(lifeSymbol, new Vector2(455 - lifeSymbol.Width, 5), Color.White);
            if (player1Lives >= 4) spriteBatch.Draw(lifeSymbol, new Vector2(435 - lifeSymbol.Width, 5), Color.White);
            if (player1Lives >= 5) spriteBatch.Draw(lifeSymbol, new Vector2(415 - lifeSymbol.Width, 5), Color.White);
            //--------------------------------------------------------------------

            spriteBatch.DrawString(basicFont, "Score: " + player1Score.ToString(), new Vector2(5, 5), Color.White);

            spriteBatch.End();

        }

        public void UpdateNoLives(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                gameState = EGameState.MENU;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                gameState = EGameState.CREDITS;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                Reset();
            }

        }

        public void DrawNoLives(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (player1Score < 25000)
            {
                spriteBatch.Draw(noLivesBackgroundTexture, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(basicFont, "Final Score: " + player1Score.ToString(), new Vector2(185, 365), Color.White);
            }
            if (player1Score >= 25000)
            {
                spriteBatch.Draw(winnerBackgroundTexture, new Vector2(0, 0), Color.White);
            }
            spriteBatch.End();
        }

        public void UpdateCredits(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                gameState = EGameState.MENU;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Reset();
            }

            creditsBackground.Update(gameTime);
            secondaryCreditsBackground.Update(gameTime);
        }

        public void DrawCredits(GameTime gameTime)
        {
            spriteBatch.Begin();
            creditsBackground.Draw(gameTime, spriteBatch);
            secondaryCreditsBackground.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }
    }
}
