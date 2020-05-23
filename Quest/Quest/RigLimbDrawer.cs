using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
    public class RigLimbDrawer
    {
        #region Static
        public static Dictionary<string, Texture2D> sheets = new Dictionary<string, Texture2D>();
        public static Dictionary<string, LimbTexture> limbTextures = new Dictionary<string, LimbTexture>();

        public static void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            LoadSheets(graphics, content);
            InitializeLimbTextures();
        }

        static void LoadSheets(GraphicsDevice graphics, ContentManager content)
        {
            LoadSheet("TestSheet", content);
        }

        static void LoadSheet(string name, ContentManager content)
        {
            sheets.Add(name, content.Load<Texture2D>(@"RigLimbSheets\" + name));
        }

        static void InitializeLimbTextures()
        {
            limbTextures.Add("TestSheet", new LimbTexture("TestSheet", Point.Zero, new Point(64, 16), 16, new Vector2(8, 8), new Vector2(56, 8), false));
        }
        #endregion

        public struct LimbTexture
        {
            Point sheetPosition, measurements;
            int frames;
            string sheet;
            Vector2 origin, endPoint;
            private Vector2 sheetScale;
            public float length, defaultRotation;
            public bool invertSprite;

            public Texture2D Texture
            { get { return sheets[sheet]; } }

            public Vector2 Origin { get { return origin; } }

            public int Width { get { return measurements.X; } }
            public int Height { get { return measurements.Y; } }

            public LimbTexture(string sheet, Point sheetPosition, Point measurements, int frames, Vector2 origin, Vector2 endPoint, bool invertSprite)
            {
                this.sheet = sheet;
                this.sheetPosition = sheetPosition;
                this.measurements = measurements;
                this.frames = frames;
                this.sheetScale = new Vector2(measurements.X / (float)sheets[sheet].Width, measurements.Y / (float)sheets[sheet].Height);
                this.invertSprite = invertSprite;
                //this.origin = origin;
                //this.endPoint = new Vector2(endPoint.X + (measurements.X - endPoint.X) * (1 - sheetScale.X), endPoint.Y * sheetScale.Y);
                this.origin = origin;
                this.endPoint = endPoint;
                length = Vector2.Distance(origin, endPoint);
                defaultRotation = Extensions.GetAngle(endPoint - origin);
            }

            /// <summary>
            /// Returns the proper scale in x-dimension to match length of given line
            /// </summary>
            /// <param name="line"></param>
            /// <returns>X, Y represents the scale and Z the rotation</returns>
            public Vector3 GetScaleAndAngle(Vector4 line)
            {
                float lineDistance = Vector2.Distance(new Vector2(line.X, line.Y), new Vector2(line.Z, line.W));

                float xScale = (lineDistance / (length + origin.X * 2));

                return new Vector3(xScale, 1, Extensions.GetAngle(new Vector2(line.Z, line.W) - new Vector2(line.X, line.Y)) - defaultRotation);
            }
            /// <summary>
            /// Returns the proper scale in x-dimension to match length of given line
            /// </summary>
            /// <param name="a">StartingPos of line</param>
            /// <param name="b">EndPos of line</param>
            /// <returns>X, Y represents the scale and Z the rotation</returns>
            public Vector3 GetScaleAndAngle(Vector2 a, Vector2 b)
            {
                return GetScaleAndAngle(new Vector4(a.X, a.Y, b.X, b.Y));
            }

            /// <summary>
            /// Returns the frame to use for a given angle
            /// </summary>
            /// <returns></returns>
            public int GetIndex(float angle)
            {
                angle.NormalizeRadians();
                return (int)((angle / ((float)Math.PI * 2)) * frames);
            }
            public Rectangle GetSourceRectangle(int index)
            {
                Point tempSheetPosition = sheetPosition;
                if (index >= frames)
                    index -= frames;

                int checkedFrames = 0;
                while (tempSheetPosition.X + (index - checkedFrames) * measurements.X >= sheets[sheet].Width)
                {
                    tempSheetPosition.X = 0;
                    tempSheetPosition.Y += measurements.Y;
                    checkedFrames += (int)((sheets[sheet].Width - tempSheetPosition.X) / measurements.X);
                }

                if (tempSheetPosition.Y >= sheets[sheet].Height)
                    return Rectangle.Empty;
                else
                    tempSheetPosition.X = (index - checkedFrames) * measurements.X;

                return new Rectangle(tempSheetPosition, measurements);
            }
        }


        //Actual class
        string limbTexture;
        public LimbTexture Texture { get { return limbTextures[limbTexture]; } }

        public RigLimbDrawer(string limbTexture)
        {
            this.limbTexture = limbTexture;
        }

        public void Draw(bool drawLines, Vector2 offset, RigLimb rigLimb, DrawBatch drawBatch, float depth, float orientation)
        {
            Vector4 line = rigLimb.Get2DLine(orientation);
            Vector3 scaleAngle = Texture.GetScaleAndAngle(line);
            int index = Texture.GetIndex(GetAngleToDisplay(rigLimb.rotation, orientation));

            Color color = Color.White;
            //if (rigLimb.name == Game1.selectedLimb)
            //{
            //    color = Color.Green;
            //}
            //else
            //    color = Color.White;

            //spriteBatch.Draw(Texture.Texture, new Vector2(line.X + offset.X, line.Y + offset.Y), Texture.GetSourceRectangle(index), color, scaleAngle.Z, new Vector2(8, 8), new Vector2(scaleAngle.X, scaleAngle.Y), SpriteEffects.None, depth);
            if (Game1.rigDrawTextures && rigLimb.drawTexture)
            {
                Rectangle rectangleToDraw = new Rectangle(new Vector2(line.X + offset.X, line.Y + offset.Y).ToPoint(), new Vector2(Texture.Width * scaleAngle.X + Texture.Origin.X * 2, Texture.Height * scaleAngle.Y).ToPoint());

                //float fuckScale = (((rectangleToDraw.Width * 2 - Texture.Origin.X * 2))) / (Texture.Width * 2 + Texture.Origin.X * 2);
                //float fuckOrigin = (Texture.Width / 2) * (1-fuckScale) + ((fuckScale) * Texture.Origin.X);

                float fuckOrigin;
                if (scaleAngle.X < .5f)
                    fuckOrigin = ((Texture.Width / 2f) - (Texture.Width / 4f) * scaleAngle.X) * (1 - scaleAngle.X);// + 8 * (scaleAngle.X);
                else
                    fuckOrigin = Texture.Origin.X + (Texture.Origin.X / 2) * ((1 - scaleAngle.X) * 2);

                if (scaleAngle.X > 1)
                    drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, Texture.Texture, rectangleToDraw, Texture.GetSourceRectangle(index), scaleAngle.Z, new Vector2(Texture.Origin.X / scaleAngle.X, Texture.Origin.Y), color, depth, SpriteEffects.None);
                else
                    drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, Texture.Texture, rectangleToDraw, Texture.GetSourceRectangle(index), scaleAngle.Z, new Vector2(fuckOrigin, Texture.Origin.Y), color, depth, SpriteEffects.None);
            }
            //Game1.DrawLine(spriteBatch, new Vector2(line.X + offset.X, line.Y + offset.Y), new Vector2(line.X + offset.X, line.Y + offset.Y) + Extensions.GetVector2(Texture.Texture.Width * scaleAngle.X, scaleAngle.Z), 2, Color.Lerp(Color.Blue, Color.Transparent, .5f), 0f);

            if (drawLines)
            {
                if (rigLimb.name == Game1.selectedLimb)
                    Game1.DrawLine(drawBatch, new Vector2(line.X + offset.X, line.Y + offset.Y), new Vector2(line.Z + offset.X, line.W + offset.Y), 2, Color.Lerp(Color.Red, Color.Transparent, 0f), depth);
                else
                    Game1.DrawLine(drawBatch, new Vector2(line.X + offset.X, line.Y + offset.Y), new Vector2(line.Z + offset.X, line.W + offset.Y), 2, Color.Lerp(Color.Black, Color.Transparent, 0f), depth);
            }
        }

        float GetAngleToDisplay(Vector3 rotation, float orientation)
        {
            float ret;
            rotation = new Vector3((rotation.X + orientation - (float)Math.PI / 2).NormalizeRadians(), rotation.Y.NormalizeRadians(), rotation.Z.NormalizeRadians());

            if (rotation.Y > (float)Math.PI)
                ret = (rotation.X + rotation.Z) - (float)Math.PI;
            else
                ret = -(rotation.X + rotation.Z);

            if (Texture.invertSprite)
                ret += (float)Math.PI;

            return ret.NormalizeRadians();
        }
    }
}
