using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using GameApp.Model;
using GameApp.View;
namespace GameApp.Controller
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class FirstGame : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		private Player player; //Represents player.
		//Keyboard states to determine pressing.
		KeyboardState currentKeyboardState;
		//KeyboardState previousKeyboardState;
		//Gamepad states to determine pressing.
		GamePadState currentGamePadState;
		GamePadState previousGamePadState;
		// Player movement speed.
		float playerMoveSpeed;
		// Image used to display the static background
		Texture2D mainBackground;
		Texture2D explosionTexture;
		List<Animation> explosions;
		// Parallaxing Layers
		ParallaxingBackground bgLayer1;
		ParallaxingBackground bgLayer2;
		// Enemies
		Texture2D enemyTexture;
		List<Enemy> enemies;
		// The sound that is played when a laser is fired
		SoundEffect laserSound;
		//SoundEffect bombSound;
		//SoundEffect popSound;

		// The sound used when the player or an enemy dies
		SoundEffect explosionSound;

		// The music played during gameplay
		Song gameplayMusic;
		//Number that holds the player score
		int score;
		// The font used to display UI elements
		SpriteFont font;
		// The rate at which the enemies appear
		TimeSpan enemySpawnTime;
		TimeSpan previousSpawnTime;

		// A random number generator
		Random random;
		Texture2D projectileTexture;
		Texture2D bombTexture;
		Texture2D puddingTexture;
		List<Projectile> projectiles;
		List<puddingPop> puddingPops;
		List<bomb> bombs;

		// The rate of fire of the player laser
		TimeSpan fireTime;
		TimeSpan previousFireTime;

		public FirstGame ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
            

			//Initializes player class.
			player = new Player();
			//Sets constant move speed.
			playerMoveSpeed = 8.0f;
			//Enable FreeDrag gesture.
			bgLayer1 = new ParallaxingBackground();
			bgLayer2 = new ParallaxingBackground();
			// Initialize the enemies list
			enemies = new List<Enemy> ();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator
			random = new Random();
			projectiles = new List<Projectile>();
			bombs = new List<bomb> ();
			puddingPops = new List<puddingPop>();

			// Set the laser to fire every quarter second
			fireTime = TimeSpan.FromSeconds(.15f);
			explosions = new List<Animation>();
			score = 0;

			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/shipAnimation");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

			Vector2 playerPosition = new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
				+ GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation, playerPosition);
			// Load the parallaxing background
			bgLayer1.Initialize(Content, "Texture/bgLayer1", GraphicsDevice.Viewport.Width, -1);
			bgLayer2.Initialize(Content, "Texture/bgLayer2", GraphicsDevice.Viewport.Width, -2);
			enemyTexture = Content.Load<Texture2D>("Animation/mineAnimation");
			mainBackground = Content.Load<Texture2D>("Texture/mainbackground");
			projectileTexture = Content.Load<Texture2D>("Texture/laser");
			explosionTexture = Content.Load<Texture2D>("Animation/explosion");
			puddingTexture = Content.Load<Texture2D> ("Texture/puddingPop");
			bombTexture = Content.Load<Texture2D> ("Texture/bomb");
			// Load the music
			gameplayMusic = Content.Load<Song>("Sound/gameMusic");

			// Load the laser and explosion sound effect
			laserSound = Content.Load<SoundEffect>("Sound/laserFire");
			//bombSound = Content.Load<SoundEffect> ("Sound/laserFire");
			//popSound = Content.Load<SoundEffect> ("Sound/laserFire");
			explosionSound = Content.Load<SoundEffect>("Sound/explosion");

			// Start the music right away
			font = Content.Load<SpriteFont>("Font/gameFont");
			PlayMusic(gameplayMusic);

			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			#endif
            
			//Saves previous keyboard state and gamepad to determine user presses.
			previousGamePadState = currentGamePadState;
			//previousKeyboardState = currentKeyboardState;
            // Reads the current state and stores it.
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);
			//previousKeyboardState = Keyboard.GetState();
			previousGamePadState = GamePad.GetState(PlayerIndex.One);
				//Updates player.
				UpdatePlayer(gameTime);

			// Update the parallaxing background
			bgLayer1.Update();
			bgLayer2.Update();
			// Update the enemies
			UpdateEnemies(gameTime);
			// Update the collision
			UpdateCollision();
			UpdateBombs ();
			UpdatePuddingPop ();
			UpdateProjectiles ();
			UpdateExplosions(gameTime);
			UpdatePop ();
			UpdateBomb ();



			base.Update (gameTime);
		}

		private void UpdatePlayer(GameTime gameTime)
		{
			player.Update (gameTime);
			//Get thumbstick controls.
			player.Position.X += currentGamePadState.ThumbSticks.Left.X *playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y *playerMoveSpeed;
			player.Position.X += previousGamePadState.ThumbSticks.Left.X *playerMoveSpeed;
			player.Position.Y -= previousGamePadState.ThumbSticks.Left.Y *playerMoveSpeed;

			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) ||
				currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) ||
				currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) ||
				currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) ||
				currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}
			// reset score if player health goes to zero
			if (player.Health <= 0)
			{
				player.Health = 100;
				score = 0;
			}

			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X, 0,GraphicsDevice.Viewport.Width - player.Width);
			player.Position.Y = MathHelper.Clamp(player.Position.Y, 0,GraphicsDevice.Viewport.Height - player.Height);
			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime && currentKeyboardState.IsKeyDown(Keys.K))
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddProjectile(player.Position + new Vector2(player.Width / 2, 0));
				// Play the laser sound
				laserSound.Play();
			}
			if (gameTime.TotalGameTime - previousFireTime > fireTime && currentKeyboardState.IsKeyDown(Keys.Space))
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddBomb(player.Position + new Vector2(player.Width / 2, 0));
				// Play the laser sound
				laserSound.Play();
			}
			if (gameTime.TotalGameTime - previousFireTime > fireTime && currentKeyboardState.IsKeyDown(Keys.Enter))
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddPudding(player.Position + new Vector2(player.Width / 2, 0));
				// Play the laser sound
				laserSound.Play();
			}
		}


		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
			//Drawing starts. 
			spriteBatch.Begin();
			// Draws player.
			spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

			// Draw the moving background
			bgLayer1.Draw(spriteBatch);
			bgLayer2.Draw(spriteBatch);
			// Draw the Enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw(spriteBatch);
			}
			player.Draw(spriteBatch);
			// Stops drawing.
			for (int i = 0; i < projectiles.Count; i++)
			{
				projectiles[i].Draw(spriteBatch);
			}
			for (int i = 0; i < puddingPops.Count; i++)
			{
				puddingPops[i].Draw(spriteBatch);
			}
			for (int i = 0; i < bombs.Count; i++)
			{
				bombs[i].Draw(spriteBatch);
			}
			for (int i = 0; i < explosions.Count; i++)
			{
				explosions[i].Draw(spriteBatch);
			}
			// Draw the score
			spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
			// Draw the player health
			spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
			// Draw the Projectiles

			spriteBatch.End();

            
			//TODO: Add your drawing co		de here
            
			base.Draw (gameTime);
		}
		private void AddEnemy()
		{ 
			// Create the animation object
			Animation enemyAnimation = new Animation();

			// Initialize the animation with the correct animation information
			enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30,Color.White, 1f, true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height -100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(enemyAnimation, position); 

			// Add the enemy to the active enemies list
			enemies.Add(enemy);
		}
		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for (int i = projectiles.Count - 1; i >= 0; i--) 
			{
				projectiles[i].Update();

				if (projectiles[i].Active == false)
				{
					projectiles.RemoveAt(i);
				} 
			}
		}
		private void UpdatePop()
		{
			// Update the Projectiles
			for (int i = puddingPops.Count - 1; i >= 0; i--) 
			{
				puddingPops[i].Update();

				if (puddingPops[i].Active == false)
				{
					puddingPops.RemoveAt(i);
				} 
			}
		}
		private void UpdateBomb()
		{
			// Update the Projectiles
			for (int i = bombs.Count - 1; i >= 0; i--) 
			{
				bombs[i].Update();

				if (bombs[i].Active == false)
				{
					bombs.RemoveAt(i);
				} 
			}
		}
		private void UpdateCollision()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)player.Position.X,
				(int)player.Position.Y,
				player.Width,
				player.Height);
			
			// Do the collision between the player and the enemies
			for (int i = 0; i <enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X,
					(int)enemies[i].Position.Y,
					enemies[i].Width,
					enemies[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false; 
				}


				}
			// Projectile vs Enemy Collision
			for (int i = 0; i < projectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++) 
				{

					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle ((int)projectiles [i].Position.X -
						projectiles [i].Width / 2, (int)projectiles [i].Position.Y -
						projectiles [i].Height / 2, projectiles [i].Width, projectiles [i].Height);

					rectangle2 = new Rectangle ((int)enemies [j].Position.X - enemies [j].Width / 2,
						(int)enemies [j].Position.Y - enemies [j].Height / 2,
						enemies [j].Width, enemies [j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects (rectangle2)) 
					{
						enemies [j].Health -= projectiles [i].Damage;
						projectiles [i].Active = false;
					}
				}
			}


		}
		private void UpdatePuddingPop()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)player.Position.X,
				(int)player.Position.Y,
				player.Width,
				player.Height);
			// Projectile vs Enemy Collision
			for (int i = 0; i < puddingPops.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++) 
				{

					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle ((int)puddingPops [i].Position.X -
						puddingPops [i].Width / 2, (int)puddingPops [i].Position.Y -
						puddingPops [i].Height / 2, puddingPops [i].Width, puddingPops [i].Height);

					rectangle2 = new Rectangle ((int)enemies [j].Position.X - enemies [j].Width / 2,
						(int)enemies [j].Position.Y - enemies [j].Height / 2,
						enemies [j].Width, enemies [j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects (rectangle2)) 
					{


						enemies [j].Health -= puddingPops [i].Damage;
						puddingPops [i].Active = false;
					}
				}
			}
			// Do the collision between the player and the enemies
			for (int i = 0; i <enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X,
					(int)enemies[i].Position.Y,
					enemies[i].Width,
					enemies[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false; 
				}


			}


		}
		private void UpdateBombs()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;


			// Projectile vs Enemy Collision
			for (int i = 0; i < bombs.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++) 
				{

					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle ((int)bombs [i].Position.X -
						bombs [i].Width / 2, (int)bombs [i].Position.Y -
						bombs [i].Height / 2, bombs [i].Width, bombs [i].Height);

					rectangle2 = new Rectangle ((int)enemies [j].Position.X - enemies [j].Width / 2,
						(int)enemies [j].Position.Y - enemies [j].Height / 2,
						enemies [j].Width, enemies [j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects (rectangle2)) 
					{
   						enemies [j].Health -= bombs [i].Damage;
						bombs [i].Active = false;
					}
				}
			}



		}
		private void AddExplosion(Vector2 position)
		{
			Animation explosion = new Animation();
			explosion.Initialize(explosionTexture,position, 134, 134, 12, 45, Color.White, 1f,false);
			explosions.Add(explosion);
		}
		private void PlayMusic(Song song)
		{
			// Due to the way the MediaPlayer plays music,
			// we have to catch the exception. Music will play when the game is not tethered
			try
			{
				// Play the music
				MediaPlayer.Play(song);

				// Loop the currently playing song
				MediaPlayer.IsRepeating = true;
			}
			catch { }
		}
		private void UpdateExplosions(GameTime gameTime)
		{
			for (int i = explosions.Count - 1; i >= 0; i--)
			{
				explosions[i].Update(gameTime);
				if (explosions[i].Active == false)
				{
					explosions.RemoveAt(i);
				}
			}
		}
		private void AddProjectile(Vector2 position)
		{
			Projectile projectile = new Projectile(); 
			projectile.Initialize(GraphicsDevice.Viewport, projectileTexture,position); 
			projectiles.Add(projectile);
		}
		private void AddBomb(Vector2 position)
		{
			bomb bomb = new bomb(); 
			bomb.Initialize(GraphicsDevice.Viewport, bombTexture,position); 
			bombs.Add(bomb);
		}
		private void AddPudding(Vector2 position)
		{
			puddingPop puddingPop = new puddingPop(); 
			puddingPop.Initialize(GraphicsDevice.Viewport, puddingTexture,position); 
			puddingPops.Add(puddingPop);
		}
		private void UpdateEnemies(GameTime gameTime)
		{
			
				// Spawn a new enemy enemy every 1.5 seconds
				if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime) {
					previousSpawnTime = gameTime.TotalGameTime;

					// Add an Enemy
					AddEnemy ();

				}

			// Update the Enemies
			for (int i = enemies.Count - 1; i >= 0; i--) 
			{
				enemies[i].Update(gameTime);

				if (enemies[i].Active == false)
				{
					// If not active and health <= 0
					if (enemies[i].Health <= 0)
					{
						// Add an explosion
						AddExplosion(enemies[i].Position);
					}
					enemies.RemoveAt(i);
				} 
			}

		}
	}
}