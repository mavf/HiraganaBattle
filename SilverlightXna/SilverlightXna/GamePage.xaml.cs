using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Entities;
using GameLib.Background;
using Microsoft.Xna.Framework.Input;
using GameLib.Helpers.Control;
using System.Diagnostics;
using GameLib.Helpers;
using Particles.Background;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Phone.Shell;

namespace SilverlightXna
{
    public partial class GamePage : PhoneApplicationPage
    {
        ContentManager contentManager;
        GameTimer timer;
        SpriteBatch spriteBatch;

        private Texture2D playerTexture, laserTexture, starTexture, explosion, gameOverTexture;
        private SpriteFont font;
        int centerX, playerYposition;
        private Vector2 centerPosition;

        //private IBackground background;
        //private Song music;
        private List<Enemy> enemies = new List<Enemy>(6);
        private List<LaserBullet> bullets = new List<LaserBullet>(4);
        private Player player;

        KeysQueue keysQueue = new KeysQueue();

        private EndGameScore endGameScore;
        private BackgroundParticleSystem particleSystem;
        private EnemyHelper enemyHelper;
        // For rendering the XAML onto a texture
        UIElementRenderer elementRenderer;

        private int refreshRate;
        private bool isGameOver = false;

        int score = 0;
        int wave = 0;

        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            //timer.UpdateInterval = TimeSpan.FromTicks(333333);
            //timer.UpdateInterval = TimeSpan.FromTicks(166667);
            timer.UpdateInterval = TimeSpan.FromSeconds(1.0 / 40.0);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            // Use the LayoutUpdate event to know when the page layout 
            // has completed so that we can create the UIElementRenderer.
            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);

            (Application.Current as App).AppDeactivated += new EventHandler(GamePage_AppDeactivated);
        }

        void GamePage_AppDeactivated(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State["timer"] = timer;
            PhoneApplicationService.Current.State["enemies"] = enemies;

            PhoneApplicationService.Current.State["isGameOver"] = isGameOver;
            PhoneApplicationService.Current.State["score"] = score;
            PhoneApplicationService.Current.State["wave"] = wave;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // TODO: use this.content to load your game content here
            playerTexture = contentManager.Load<Texture2D>("player");
            laserTexture = contentManager.Load<Texture2D>("laser");
            starTexture = contentManager.Load<Texture2D>("star");
            font = contentManager.Load<SpriteFont>("font");
            explosion = contentManager.Load<Texture2D>("explosion0");
            gameOverTexture = contentManager.Load<Texture2D>("gameover");
            
            //background = new FlowingBackground(this.contentManager.Load<Texture2D>("sky1"), 0.15f, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height);

            centerX = SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Width / 2;
            playerYposition = SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - 50;
            centerPosition = new Vector2(centerX, SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height / 2);
            player = new Player(playerTexture, centerX, playerYposition, explosion);

            enemyHelper = new EnemyHelper(explosion);
            endGameScore = new EndGameScore(new Vector2(centerPosition.X, 380), font, gameOverTexture);


            if (e.NavigationMode == NavigationMode.Back)
            {
                elementRenderer = new UIElementRenderer(this, 800, 480);
                isGameOver = (bool)PhoneApplicationService.Current.State["isGameOver"];

                if (particleSystem == null)
                {
                    particleSystem = (Application.Current as App).BackgroundParticleSystem;

                    LinkedList<Particle> particles = (LinkedList<Particle>)PhoneApplicationService.Current.State["particles"];

                    foreach (var item in particles)
                    {
                        item.Parent = particleSystem.EmitterList.FirstOrDefault();
                    }

                    var emitter = particleSystem.EmitterList.FirstOrDefault();

                    if (emitter != null)
                    {
                        emitter.ActiveParticles = particles;
                    }
                }

                if (enemies == null || PhoneApplicationService.Current.State.ContainsKey("enemies"))
                {
                    enemies = (List<Enemy>)PhoneApplicationService.Current.State["enemies"];

                    enemies = (from en in enemies
                              select new Enemy(font, en, explosion)).ToList();
                }

                //if (bullets == null || PhoneApplicationService.Current.State.ContainsKey("bullets"))
                //{
                //    bullets = (List<LaserBullet>)PhoneApplicationService.Current.State["bullets"];
                //}

                if (PhoneApplicationService.Current.State.ContainsKey("score"))
                    score = (int)PhoneApplicationService.Current.State["score"];

                if (PhoneApplicationService.Current.State.ContainsKey("wave"))
                    wave = (int)PhoneApplicationService.Current.State["wave"];
            }
            else
            {
                isGameOver = false;
                particleSystem = (Application.Current as App).BackgroundParticleSystem;
            }

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);

            elementRenderer.Dispose();
            elementRenderer = null;
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: Add your update logic here
            refreshRate = (int)(1 / ((double)e.ElapsedTime.Milliseconds/1000d));

            if (isGameOver == false)
            {

                #region key control
                var eq = enemies.Where(x => x.FirstCharacter.Equals(keysQueue.Text, StringComparison.OrdinalIgnoreCase) && !String.IsNullOrEmpty(x.FirstCharacter)).FirstOrDefault();
                if (eq != null)
                {
                    Debug.WriteLine(String.Format("  shoot {0}   keys {1}", eq.FirstCharacter, keysQueue.Count.ToString()));

                    Shoot(eq);
                    keysQueue.Clear();
                }
                else
                {
                    var eqp = enemies.Where(x => x.Text.StartsWith(keysQueue.Text, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                    if (eqp != null)
                    {
                        ;
                    }
                    else
                    {
                        keysQueue.Clear();
                    }
                }
                #endregion

                CreateEnemy();

                LaserBullet tmpBullet = null;
                foreach (var laserBullet in bullets)
                {
                    if (laserBullet != null)
                    {
                        laserBullet.Update();

                        if (laserBullet.x < 0 || laserBullet.y < 0)
                            //laserBullet = null;
                            tmpBullet = laserBullet;
                    }
                }

                bullets.Remove(tmpBullet);

                #region collisions
                Enemy tmp = null;
                foreach (var item in enemies)
                {
                    item.Update();

                    if (item.position.Y > SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height)
                        tmp = item;

                    if (item.ShouldBeRemoved)
                        tmp = item;

                    foreach (var laserBullet in bullets)
                    {
                        if (laserBullet != null && laserBullet.Bounds.Intersects(item.Bounds))
                        {
                            //remove letter form item
                            item.Hit();
                            tmpBullet = laserBullet;
                            score += 20;
                        }
                    }
                    bullets.Remove(tmpBullet);

                    if (item.Bounds.Intersects(player.Bounds) && item.IsExploding == false)
                    {
                        player.Explode();
                        item.Explode();

                        playExplodeSound();
                    }
                }
                if (tmp != null)
                {
                    enemies.Remove(tmp);
                    tmp = null;
                }
                #endregion
                #region end game
                var enemyToExplode = enemies.Where(x => x.IsExploding == false && x.CenteryY > SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Height - 40).ToList();
                if (enemyToExplode.Count > 0)
                {
                    foreach (var item in enemyToExplode)
                    {
                        item.Explode();
                        playExplodeSound();
                    }
                }

                if (player.IsExploded || enemies.Any(x => x.IsExploded))
                {
                    //remove all game over
                    isGameOver = true;
                    enemies.Clear();
                }
                #endregion
            }
            else
            {
                endGameScore.Update();

                var touchCollection = TouchPanel.GetState();
                if (touchCollection.Count > 0)
                {
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
                    else
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }

            player.Update();
            particleSystem.Update(e.ElapsedTime.Milliseconds / 2000f);

            //background.Update(SharedGraphicsDeviceManager.Current.GraphicsDevice.Viewport.Bounds);
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            try
            {
                if (elementRenderer != null)
                {
                    // Draw the Silverlight UI into the texture
                    elementRenderer.Render();
                }
            }
            catch (ObjectDisposedException)
            {
                CreateUiElementRenderer();
            }


            //SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.CornflowerBlue);
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            particleSystem.Draw(spriteBatch, 1, Vector2.Zero);

            spriteBatch.End();

            spriteBatch.Begin();

            if (isGameOver == false)
            {
                foreach (var laserBullet in bullets)
                {
                    laserBullet.Draw(spriteBatch);
                }

                foreach (var item in enemies)
                {
                    item.Draw(spriteBatch);
                }

                player.Draw(spriteBatch);

                // Using the texture from the UIElementRenderer, 
                // draw the Silverlight controls to the screen.
                spriteBatch.Draw(elementRenderer.Texture, Vector2.Zero, Color.White);

                Vector2 scorePosition = new Vector2(600, playerYposition + 20);
                spriteBatch.DrawString(font, score.ToString(), scorePosition, Color.Gray);
                scorePosition.Y += 2;
                spriteBatch.DrawString(font, score.ToString(), scorePosition, Color.Red);
            }
            else
            {
                Vector2 scorePosition = new Vector2(centerX - font.MeasureString(score.ToString()).X/2, 260);
                spriteBatch.DrawString(font, score.ToString(), scorePosition, Color.Red);
                scorePosition.Y += 2;
                scorePosition.X += 1;
                spriteBatch.DrawString(font, score.ToString(), scorePosition, Color.WhiteSmoke);

                endGameScore.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        private void Shoot(Enemy enemy)
        {
            if (enemies.Count > 0 && enemy.position.Y < playerYposition)
            {
                var laserBullet = new LaserBullet(laserTexture, centerX, playerYposition);

                float a = (playerYposition - (enemy.position.Y + enemy.Height)) / (centerX - enemy.CenterX);
                laserBullet.Alpha = a;

                var aAngle = MathHelper.ToDegrees((float)Math.Atan(a));
                //var x = 180 - aAngle;
                var angle = 180 - 90 - (180 - aAngle);
                laserBullet.Angle = MathHelper.ToRadians(angle);

                var b = (enemy.position.Y + enemy.Height) - ((enemy.CenterX) * a);
                laserBullet.Bparam = b;

                float v = ((Math.Abs(centerX - enemy.CenterX)) / ((float)refreshRate));
                if (v == 0)
                    ;
                laserBullet.Vs = v;

                bullets.Add(laserBullet);

                enemy.Shoot();
                //Debug.WriteLine(String.Format("     vSpeed: {0}", v.ToString()));
            }
        }

        private void CreateEnemy()
        {
            if (enemies.Count == 0)
            {
                if (wave > 10 && wave % 5 == 0)
                {
                    enemies.Add(enemyHelper.CreateEnemy(font, EnemyType.FastSingle));
                    enemies.Add(enemyHelper.CreateEnemy(font, EnemyType.FastSingle));
                    enemies.Add(enemyHelper.CreateEnemy(font, EnemyType.FastSingle));
                    enemies.Add(enemyHelper.CreateEnemy(font, EnemyType.FastSingle));
                }
                else
                {
                    if (wave < 10)
                    {
                        enemies.Add(enemyHelper.CreateEnemy(font));
                        enemies.Add(enemyHelper.CreateEnemy(font));
                    }
                    if (wave > 10)
                    {
                        enemies.Add(enemyHelper.CreateEnemy(font));
                        enemies.Add(enemyHelper.CreateEnemy(font));
                        enemies.Add(enemyHelper.CreateEnemy(font, EnemyType.FastSingle));
                    }
                }
                
                //Debug.WriteLine("Creating enemy");
                wave += 1;
            }
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            CreateUiElementRenderer();
        }

        private void CreateUiElementRenderer()
        {
            // Create the UIElementRenderer to draw the XAML page to a texture.

            // Check for 0 because when we navigate away the LayoutUpdate event
            // is raised but ActualWidth and ActualHeight will be 0 in that case.
            if ((ActualWidth > 0.0D) && (ActualHeight > 0.0D))
            {
                SharedGraphicsDeviceManager.Current.PreferredBackBufferWidth = (int)ActualWidth;
                SharedGraphicsDeviceManager.Current.PreferredBackBufferHeight = (int)ActualHeight;
            }

            if (null == elementRenderer)
            {
                elementRenderer = new UIElementRenderer(this, 800, 480);
            }
        }

        private void ButtonInput_Click(object sender, RoutedEventArgs e)
        {
            string key = (e.OriginalSource as Button).CommandParameter.ToString();

            keysQueue.Add(key);
        }

        private void playExplodeSound()
        {

        }

    }
}