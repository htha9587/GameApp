using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameApp.Model
{
	public class Player
	{
		public Texture2D PlayerTexture; // Animation representing player.
		public Vector2 Position; // Represents player position on the left side of the screen.
		public bool Active; // Player state.
		public int Health; // Hit point amount.
		public int Width // Gets width of ship.
		{
			get { return PlayerTexture.Width; }
		}

		public int Height // Gets height of ship.
		{
			get { return PlayerTexture.Height; }
		}

		public void Initialize(Texture2D texture, Vector2 position)
		{
			PlayerTexture = texture;
			//Sets position of the player.
			Position = position;
			// Sets player to be active.
			Active = true;
			//Sets player health.
			Health = 100;
		}

		public void Update()
		{

		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (PlayerTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f,
				SpriteEffects.None, 0f);
		}

	}
}

