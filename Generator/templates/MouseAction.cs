using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using XleModel;

namespace WindowsGame3
{
    public class MouseAction : GameComponent
    {
        MouseState currentState;
        MouseState prevState;
        Camera camera;
        int halfWidth;
        int halfHeight;

        public MouseAction(Game game, Camera camera)
            : base(game)
        {
            prevState = new MouseState();
            this.camera = camera;
        }

        public override void  Update(GameTime gameTime)
        {
 	        base.Update(gameTime);
            halfWidth = Game.Window.ClientBounds.Width / 2;
            halfHeight = Game.Window.ClientBounds.Height / 2;
            currentState = Mouse.GetState();
            if (currentState.RightButton == ButtonState.Pressed)// && prevState.RightButton == ButtonState.Released)
            {
                Mouse.SetPosition(halfWidth, halfHeight);
            }

            if (currentState.RightButton == ButtonState.Pressed && prevState.RightButton == ButtonState.Pressed && (currentState.X != halfWidth || currentState.Y != halfHeight))
            {
                camera.Rotate((currentState.Y - halfHeight) / 10f, -(currentState.X - halfWidth) / 10f, 0);
            }

            prevState = currentState;
        }
    }
}
