using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameApp.View;
namespace GameApp.Model
{
	public class Player
	{
		
		public Animation PlayerAnimation;
		public Vector2 Position; // Represents player position on the left side of the screen.
		public bool Active; // Player state.
		public int Health; // Hit point amount.
		public int Width // Gets width of ship.
		//Animation representing player.

		{
			get { return PlayerAnimation.FrameWidth; }
		}

		public int Height // Gets height of ship.
		{
			get { return PlayerAnimation.FrameHeight; }
		}

		// Initialize the player
		public void Initialize(Animation animation, Vector2 position)
		{
			PlayerAnimation = animation;

			// Set the starting position of the player around the middle of the screen and to the back
			Position = position;

			// Set the player to be active
			Active = true;

			// Set the player health
			Health = 100;
		}

		public void Update(GameTime gameTime)
		{
			PlayerAnimation.Position = Position;
			PlayerAnimation.Update (gameTime);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			PlayerAnimation.Draw(spriteBatch);
		}

	}
}

