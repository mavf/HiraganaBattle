using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLib.Background
{
    public class FlowingBackground : IBackground
    {
        private readonly Texture2D texture;
        readonly int horizontalTileCount;
        readonly int verticalTileCount;
        Vector2 startCoord;
        private float speed;
        public Rectangle _previousCameraRectangle;

        public FlowingBackground(Texture2D texture, float speed, int environmentWidth, int environmentHeight)
        {
            this.texture = texture;
            horizontalTileCount = (int)(Math.Round((double)environmentWidth / this.texture.Width) + 1);
            verticalTileCount = (int)(Math.Round((double)environmentHeight / this.texture.Height) + 1);

            startCoord = new Vector2(0, 0);
            this.speed = speed;
        }


        public void Update(Rectangle screenRectangle)
        {
            //var dif = game.CameraService.CameraRectangle.X - _previousCameraRectangle.X;
            //_previousCameraRectangle = game.CameraService.CameraRectangle;

            startCoord.X = startCoord.X + (this.speed) - 0;
            if ((-startCoord.X >= texture.Width) || (startCoord.X >= texture.Width))
                startCoord.X = 0;



            startCoord.Y = startCoord.Y + (0.1f);
            if ((-startCoord.Y >= texture.Height) || (startCoord.Y >= texture.Height))
                startCoord.Y = 0;

            //startCoord.Y = ((game.CameraService.CameraRectangle.Y / texture.Height) * texture.Height) - game.CameraService.CameraRectangle.Y; 
            //startCoord.Y = 0;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = -1; i < verticalTileCount; i++)
            {
                for (int j = -1; j < horizontalTileCount; j++)
                {
                    spriteBatch.Draw(texture,
                        new Rectangle(
                            (int)startCoord.X + (j * texture.Width),
                            (int)startCoord.Y + (i * texture.Height),
                            texture.Width, texture.Height),
                            Color.White);
                }
            }
        }
    }
}
