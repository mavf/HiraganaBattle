using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Particles.Background;

namespace SilverlightXna
{
    public partial class MainPage : PhoneApplicationPage
    {
        ContentManager contentManager;
        GameTimer timer;
        SpriteBatch spriteBatch;
        // For rendering the XAML onto a texture
        UIElementRenderer elementRenderer;
        private BackgroundParticleSystem particleSystem;

        private bool isActive = true;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            //Create a timer for this page
            timer = new GameTimer();
            //timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.UpdateInterval = TimeSpan.FromSeconds(1.0 / 40.0);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            ////// Use the LayoutUpdate event to know when the page layout 
            ////// has completed so that we can create the UIElementRenderer.
            LayoutUpdated += new EventHandler(GamePage_LayoutUpdated);

            
        }

        // Simple button Click event handler to take us to the second page
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //timer.Stop();
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            particleSystem = (Application.Current as App).BackgroundParticleSystem;

            base.OnNavigatedTo(e);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                while (NavigationService.CanGoBack)
                {
                    NavigationService.RemoveBackEntry();
                }
            }

            // Start the timer
            timer.Start();

            isActive = true;
        }


        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            isActive = false;

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                elementRenderer.Dispose();

                elementRenderer = null;
            }

            base.OnNavigatedFrom(e);
        }

        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            if (isActive)
            {

                // Draw the Silverlight UI into the texture
                //if (elementRenderer != null)
                //    elementRenderer.Render();
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
                SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

                particleSystem.Draw(spriteBatch, 1, Vector2.Zero);

                spriteBatch.End();

                spriteBatch.Begin();

                // Using the texture from the UIElementRenderer, 
                // draw the Silverlight controls to the screen.
                spriteBatch.Draw(elementRenderer.Texture, Vector2.Zero, Microsoft.Xna.Framework.Color.White);

                spriteBatch.End();
            }
        }

        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            if (isActive)
                particleSystem.Update(e.ElapsedTime.Milliseconds / 2000f);
        }

        void GamePage_LayoutUpdated(object sender, EventArgs e)
        {
            if (isActive)
            {
                CreateUiElementRenderer();
            }
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

        private void About_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void HowTo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/HowTo.xaml", UriKind.Relative));
        }
    }
}