using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
	public class Camera
	{
		public Matrix transform; // Matrix Transform
		public Vector2 pos; // Camera Position
		float rotation = 0;
		public float zoom = 1f;
		Vector2 totalShakeOffset = Vector2.Zero;
		public Vector2 aimOffset = Vector2.Zero;
		Vector2 target = Vector2.Zero;

		public Vector2 Position
		{
			get
			{
				return pos + totalShakeOffset + aimOffset;
			}
			set { pos = value; }
		}

		public Camera(Vector2 newPos)
		{
			pos = newPos;
		}

        Rectangle rectangle = Rectangle.Empty;
        public Rectangle Rectangle { get { return rectangle; } }

		List<Shake> shakes = new List<Shake>();

		Vector2 GetTargetPosition(Vector2 playerPosition, Vector2 mousePos)
		{
			return playerPosition + new Vector2((mousePos.X - playerPosition.X) / 5, (mousePos.Y - playerPosition.Y) / 5);
		}

		/// <summary>
		/// Should be the last thing in your Update function
		/// </summary>
		public void Update(Vector2 playerPosition, Vector2 mousePos)
		{
			target = GetTargetPosition(playerPosition, mousePos);

			if (Math.Abs(pos.X - target.X) < .1f)
				pos.X = target.X;
			else
				pos.X += (target.X - pos.X) * .2f;

			if (Math.Abs(pos.Y - target.Y) < .1f)
				pos.Y = target.Y;
			else
				pos.Y += (target.Y - pos.Y) * .2f;

			totalShakeOffset = Vector2.Zero;
			for (int i = 0; i < shakes.Count; i++)
			{
				if(shakes[i].time > shakes[i].duration)
				{
					shakes.RemoveAt(i);
					i--;
				}
				else
				{
					Vector2 toAdd = shakes[i].GetOffset();
					totalShakeOffset += toAdd;
				}
			}

			

            rectangle.Location = (Position - new Vector2((Game1.ScreenWidth / 2) / zoom + 32, (Game1.ScreenHeight / 2) / zoom + 32)).ToPoint();
            rectangle.Size = new Vector2(Game1.ScreenWidth / zoom + 128, Game1.ScreenHeight / zoom + 128).ToPoint();
        }

		public Matrix get_transformation(GraphicsDevice graphicsDevice)
		{
			transform =       // Thanks to o KB o for this solution
				Matrix.CreateTranslation(new Vector3(-(int)Position.X, -(int)Position.Y, 0)) *
				Matrix.CreateRotationZ(rotation) *
				Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
				Matrix.CreateTranslation(new Vector3(Game1.ScreenWidth * 0.5f, Game1.ScreenHeight * 0.5f, 0));
			return transform;
		}

		public void AddShake(float magnitude, float intensity, int duration)
		{
			Vector2 initialOffset = Vector2.Zero;
			//Vector2 initialOffset = new Vector2((float)Game1.random.NextDouble() * ((2 * (float)Math.PI) / intensity), (float)Game1.random.NextDouble() * ((2 * (float)Math.PI) / intensity));
			shakes.Add(new Shake(magnitude, intensity, initialOffset, duration));
		}

		class Shake
		{
			float magnitude, intensity;
			public int time, duration;
			Vector2 initialOffset;

			public Shake(float magnitude, float intensity, Vector2 initialOffset, int duration)
			{
				this.magnitude = magnitude;
				this.intensity = intensity;
				this.initialOffset = initialOffset;
				this.duration = duration;
				time = 0;
			}

			public Vector2 GetOffset()
			{
				time++;
				float x = time / (float)duration;
				return new Vector2(MathTransformations.Transform(MathTransformations.Type.NormalizedSmoothStop2, x) * magnitude * (float)Math.Sin(intensity * time + initialOffset.X), MathTransformations.Transform(MathTransformations.Type.NormalizedSmoothStop2, x) * magnitude * (float)Math.Sin(intensity * time + initialOffset.Y));
			}

			public override string ToString()
			{
				return time.ToString();
			}
		}
	}
}
