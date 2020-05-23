using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Quest
{
    /// <summary>
    /// A class sorting out all the drawing for the game, regarding depth etc.
    /// </summary>
    public class DrawBatch
    {
        public static float CalculateDepth(Vector2 position)
        {
            return position.Y / 1024;
        }

        public struct DrawCall
        {
            public string text;
            public SpriteFont font;
            public Texture2D texture;
            public bool useVectorPos, useSourceRectangle;
            public Vector2 position, origin, scale;
            Rectangle rectangle, sourceRectangle;
            public float rotation;
            public Color color;
            public float depth;
            public SpriteEffects spriteEffects;
            public enum Tag
            {
                GameObject,
                Light
            }
            public Tag tag;

            public Rectangle SourceRectangle
            {
                get
                {
                    if (useSourceRectangle)
                    { return sourceRectangle; }
                    else
                        return Rectangle.Empty;
                }
                set
                {
                    if (value != null)
                    {
                        sourceRectangle = value;
                        useSourceRectangle = true;
                    }
                    else
                        useSourceRectangle = false;
                }
            }

            /// <summary>
            /// Decides wether the object drawn is affected by lights or not
            /// 0 - always dark, 1 - affected, 2 - always bright
            /// </summary>
            public short affectedByLight;
            /// <summary>
            /// The amount of light that passes through the object
            /// </summary>
            public float lightBleedThrough;

            public Rectangle Rectangle
            {
                get
                {
                    if (useVectorPos)
                    {
                        return new Rectangle((int)(position.X), (int)(position.Y), (int)(texture.Width * scale.X), (int)(texture.Height * scale.Y));
                    }
                    else return rectangle;
                }
            }

            public Vector2 Position
            {
                get
                {
                    if (useVectorPos)
                        return position;
                    else
                        return rectangle.Location.ToVector2();
                }
                set
                {
                    if (useVectorPos)
                        position = value;
                    else
                        rectangle.Location = value.ToPoint();
                }
            }
            public Point Measurements
            {
                get
                {
                    if (useSourceRectangle)
                        return sourceRectangle.Size;
                    else
                        return new Point(texture.Width, texture.Height);
                }
            }
            public Point FrameOffset
            {
                get
                {
                    if (useSourceRectangle)
                        return sourceRectangle.Location;
                    else
                        return Point.Zero;
                }
            }

            #region Constructors Text
            public DrawCall(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, float scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
            {
                this.tag = tag;
                this.text = text;
                this.font = font;
                this.position = position;
                this.scale = new Vector2(scale);
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.useVectorPos = true;
                this.sourceRectangle = Rectangle.Empty;
                this.rectangle = Rectangle.Empty;
                this.texture = null;
                this.affectedByLight = 0;
                this.lightBleedThrough = 0f;
                useSourceRectangle = false;
            }
            public DrawCall(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
            {
                this.tag = tag;
                this.text = text;
                this.font = font;
                this.position = position;
                this.scale = scale;
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.useVectorPos = true;
                this.sourceRectangle = Rectangle.Empty;
                this.rectangle = Rectangle.Empty;
                this.texture = null;
                this.affectedByLight = 0;
                this.lightBleedThrough = 0f;
                useSourceRectangle = false;
            }
            public DrawCall(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects, short affectedByLight, float lightBleedThrough)
            {
                this.tag = tag;
                this.text = text;
                this.font = font;
                this.position = position;
                this.scale = scale;
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.useVectorPos = true;
                this.sourceRectangle = Rectangle.Empty;
                this.rectangle = Rectangle.Empty;
                this.texture = null;
                this.affectedByLight = affectedByLight;
                this.lightBleedThrough = lightBleedThrough;
                useSourceRectangle = false;

            }
            #endregion
            #region Constructors Texture
            public DrawCall(Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, float scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
            {
                this.affectedByLight = 1;
                this.lightBleedThrough = 0f;
                this.tag = tag;
                this.texture = texture;
                this.useVectorPos = true;
                this.rectangle = Rectangle.Empty;
                this.position = position;
                this.scale = new Vector2(scale);
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                if (sourceRectangle != null)
                {
                    this.useSourceRectangle = true;
                    this.sourceRectangle = sourceRectangle;
                }
                else
                {
                    this.useSourceRectangle = false;
                    this.sourceRectangle = Rectangle.Empty;
                }
                this.text = "";
                this.font = Game1.fontDebug;
            }
            public DrawCall(Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, Vector2 scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects, short affectedByLight, float lightBleedThrough)
            {
                this.affectedByLight = affectedByLight;
                this.lightBleedThrough = lightBleedThrough;
                this.tag = tag;
                this.texture = texture;
                this.useVectorPos = true;
                this.rectangle = Rectangle.Empty;
                this.position = position;
                this.scale = scale;
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.text = "";
                this.font = Game1.fontDebug;
                if (sourceRectangle != null)
                {
                    this.useSourceRectangle = true;
                    this.sourceRectangle = sourceRectangle;
                }
                else
                {
                    this.useSourceRectangle = false;
                    this.sourceRectangle = Rectangle.Empty;
                }
            }
            public DrawCall(Tag tag, Texture2D texture, Vector2 position, float scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
            {
                this.affectedByLight = 1;
                this.lightBleedThrough = 0f;
                this.tag = tag;
                this.texture = texture;
                this.useVectorPos = true;
                this.rectangle = Rectangle.Empty;
                this.position = position;
                this.useSourceRectangle = false;
                this.sourceRectangle = Rectangle.Empty;
                this.scale = new Vector2(scale);
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.text = "";
                this.font = Game1.fontDebug;
            }
            public DrawCall(Tag tag, Texture2D texture, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
            {
                this.affectedByLight = 1;
                this.lightBleedThrough = 0f;
                this.tag = tag;
                this.texture = texture;
                this.useVectorPos = true;
                this.rectangle = Rectangle.Empty;
                this.position = position;
                this.useSourceRectangle = false;
                this.sourceRectangle = Rectangle.Empty;
                this.scale = scale;
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.text = "";
                this.font = Game1.fontDebug;
            }
            public DrawCall(Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
            {
                this.affectedByLight = 1;
                this.lightBleedThrough = 0f;
                this.tag = tag;
                this.texture = texture;
                this.useVectorPos = true;
                this.rectangle = Rectangle.Empty;
                this.position = position;
                if (sourceRectangle != null)
                {
                    this.useSourceRectangle = true;
                    this.sourceRectangle = sourceRectangle;
                }
                else
                {
                    this.useSourceRectangle = false;
                    this.sourceRectangle = Rectangle.Empty;
                }
                this.scale = new Vector2(1);
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.text = "";
                this.font = Game1.fontDebug;
            }
            public DrawCall(Tag tag, Texture2D texture, Rectangle rectangle, Rectangle sourceRectangle, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
            {
                this.affectedByLight = 1;
                this.lightBleedThrough = 0f;
                this.tag = tag;
                this.texture = texture;
                this.useVectorPos = false;
                this.rectangle = rectangle;
                this.position = Vector2.Zero;
                if (sourceRectangle != null)
                {
                    this.useSourceRectangle = true;
                    this.sourceRectangle = sourceRectangle;
                }
                else
                {
                    this.useSourceRectangle = false;
                    this.sourceRectangle = Rectangle.Empty;
                }
                this.scale = new Vector2(1);
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.text = "";
                this.font = Game1.fontDebug;
            }
            public DrawCall(Tag tag, Texture2D texture, Rectangle rectangle, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
            {
                this.affectedByLight = 1;
                this.lightBleedThrough = 0f;
                this.tag = tag;
                this.texture = texture;
                this.useVectorPos = false;
                this.rectangle = rectangle;
                this.position = Vector2.Zero;
                this.useSourceRectangle = false;
                this.sourceRectangle = Rectangle.Empty;
                this.scale = new Vector2(1);
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.text = "";
                this.font = Game1.fontDebug;
            }
            public DrawCall(Tag tag, Texture2D texture, Rectangle rectangle, Rectangle sourceRectangle, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects, short affectedByLight, float lightBleedThrough)
            {
                this.affectedByLight = affectedByLight;
                this.lightBleedThrough = lightBleedThrough;
                this.tag = tag;
                this.texture = texture;
                this.useVectorPos = false;
                this.rectangle = rectangle;
                this.position = Vector2.Zero;
                if (sourceRectangle != null)
                {
                    this.useSourceRectangle = true;
                    this.sourceRectangle = sourceRectangle;
                }
                else
                {
                    this.useSourceRectangle = false;
                    this.sourceRectangle = Rectangle.Empty;
                }
                this.scale = new Vector2(1);
                this.rotation = rotation;
                this.origin = origin;
                this.color = color;
                this.depth = depth;
                this.spriteEffects = spriteEffects;
                this.text = "";
                this.font = Game1.fontDebug;
            }
            #endregion
        }




        BinarySearchList<DrawCall> drawCalls = new BinarySearchList<DrawCall>();
        protected int count = 0;
        public object DrawCallCount { get { return count; }}



        public void Begin()
        {
            drawCalls.Clear();
        }


        /// <summary>
        /// Draws all the contained Drawcalls for game objects to set rendertarget
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawSceneGameObjects(SpriteBatch spriteBatch)
        {
            count = drawCalls.Count;

            for (int i = 0; i < drawCalls.Count; i++)
            {
                if (drawCalls.GetValue(i).tag == DrawCall.Tag.GameObject)
                    ApplyDrawCallToScene(spriteBatch, drawCalls.GetValue(i));
            }
        }

        /// <summary>
        /// Draws all the contained Drawcalls for lights to set rendertarget
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawSceneLights(SpriteBatch spriteBatch)
        {
            DrawCall drawCall;
            for (int i = 0; i < drawCalls.Count; i++)
            {
                if (drawCalls.Count > 2057)
                    i = i;
                if (drawCalls.GetValue(i).tag == DrawCall.Tag.Light)
                {
                    drawCall = drawCalls.GetValue(i);
                    Game1.FXApplyShadowsToLights.Parameters["depth"].SetValue(drawCalls.GetKeyFromIndex(i));
                    Game1.FXApplyShadowsToLights.Parameters["TextureX"].SetValue((drawCall.Position.X - (Game1.camera.Position.X) - drawCall.origin.X * drawCall.scale.X - Game1.ScreenWidth) / Game1.ScreenWidth);
                    Game1.FXApplyShadowsToLights.Parameters["TextureY"].SetValue((drawCall.Position.Y - (Game1.camera.Position.Y) - drawCall.origin.Y * drawCall.scale.Y - Game1.ScreenHeight) / Game1.ScreenHeight);
                    Game1.FXApplyShadowsToLights.Parameters["TextureWidth"].SetValue((float)drawCall.texture.Width * drawCall.scale.X);
                    Game1.FXApplyShadowsToLights.Parameters["TextureHeight"].SetValue((float)drawCall.texture.Height * drawCall.scale.Y);
                    Game1.FXApplyShadowsToLights.Parameters["SpriteTexture"].SetValue(drawCall.texture);
                    Game1.FXApplyShadowsToLights.Parameters["FrameOffsetX"].SetValue((float)drawCall.FrameOffset.X * drawCall.scale.X);
                    Game1.FXApplyShadowsToLights.Parameters["FrameOffsetY"].SetValue((float)drawCall.FrameOffset.Y * drawCall.scale.Y);

                    ApplyDrawCallToScene(spriteBatch, drawCall);
                }
            }
        }
        internal void DrawDepthMask(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < drawCalls.Count; i++)
            {
                if (drawCalls.GetValue(i).tag == DrawCall.Tag.GameObject)
                    ApplyDrawCallDepthToMask(spriteBatch, drawCalls.GetValue(i));
            }
        }



        #region Applying DrawCalls
        /// <summary>
        /// Uses the spriteBatch to draw a given Drawcall onto the active rendertarget. Must be within spriteBatch.begin/end
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to use</param>
        /// <param name="drawCall">The drawcall to draw</param>
        protected void ApplyDrawCallToScene(SpriteBatch spriteBatch, DrawCall drawCall)
        {
            if (drawCall.texture == null)
            {
                //Text
                spriteBatch.DrawString(drawCall.font, drawCall.text, drawCall.position, drawCall.color, drawCall.rotation, drawCall.origin, drawCall.scale, drawCall.spriteEffects, drawCall.depth);
            }
            else
            {
                if (drawCall.useVectorPos)
                {
                    if (drawCall.useSourceRectangle)
                        spriteBatch.Draw(drawCall.texture, drawCall.position, drawCall.SourceRectangle, drawCall.color, drawCall.rotation, drawCall.origin, drawCall.scale, drawCall.spriteEffects, 0f);
                    else
                        spriteBatch.Draw(drawCall.texture, drawCall.position, null, drawCall.color, drawCall.rotation, drawCall.origin, drawCall.scale, drawCall.spriteEffects, 0f);
                }
                else
                {
                    if (drawCall.useSourceRectangle)
                        spriteBatch.Draw(drawCall.texture, drawCall.Rectangle, drawCall.SourceRectangle, drawCall.color, drawCall.rotation, drawCall.origin, drawCall.spriteEffects, 0f);
                    else
                        spriteBatch.Draw(drawCall.texture, drawCall.Rectangle, null, drawCall.color, drawCall.rotation, drawCall.origin, drawCall.spriteEffects, 0f);
                }
            }
        }

        /// <summary>
        /// Uses the spriteBatch to draw a given Drawcall onto the active rendertarget. Must be within spriteBatch.begin/end
        /// Draws the depth of the object as the amount of red color and the rest 0
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to use</param>
        /// <param name="drawCall">The drawcall to draw</param>
        protected void ApplyDrawCallDepthToMask(SpriteBatch spriteBatch, DrawCall drawCall)
        {
            float depth = drawCall.depth;
            float lightBleedThrough = drawCall.lightBleedThrough;

            if (drawCall.affectedByLight == 0)
            {
                depth = 1;
                lightBleedThrough = 1;
            }
            else if (drawCall.affectedByLight == 2)
            {
                depth = 0;
                lightBleedThrough = 0;
            }


            
            if (drawCall.texture == null)
            {
                //Text
                spriteBatch.DrawString(drawCall.font, drawCall.text, drawCall.position, drawCall.color, drawCall.rotation, drawCall.origin, drawCall.scale, drawCall.spriteEffects, drawCall.depth);
            }
            else
            {
                if (drawCall.useVectorPos)
                {
                    if (drawCall.useSourceRectangle)
                        spriteBatch.Draw(drawCall.texture, drawCall.position, drawCall.SourceRectangle, new Color(depth, 1 - lightBleedThrough, 0, 1), drawCall.rotation, drawCall.origin, drawCall.scale, drawCall.spriteEffects, 0f);
                    else
                        spriteBatch.Draw(drawCall.texture, drawCall.position, null, new Color(depth, 1 - lightBleedThrough, 0, 1), drawCall.rotation, drawCall.origin, drawCall.scale, drawCall.spriteEffects, 0f);
                }
                else
                {
                    if (drawCall.useSourceRectangle)
                        spriteBatch.Draw(drawCall.texture, drawCall.Rectangle, drawCall.SourceRectangle, new Color(depth, 1 - lightBleedThrough, 0, 1), drawCall.rotation, drawCall.origin, drawCall.spriteEffects, 0f);
                    else
                        spriteBatch.Draw(drawCall.texture, drawCall.Rectangle, null, new Color(depth, 1 - lightBleedThrough, 0, 1), drawCall.rotation, drawCall.origin, drawCall.spriteEffects, 0f);
                }
            }
        }
        #endregion

        #region DrawFunctions
        public void DrawString(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, Rectangle sourceRectangle, Vector2 scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
        {
            drawCalls.Add(depth, new DrawCall(tag, text, font, position, scale, rotation, origin, color, depth, spriteEffects));
        }
        public void DrawString(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, Rectangle sourceRectangle, Vector2 scale, float rotation, Vector2 origin, Color color, float depth)
        {
            drawCalls.Add(depth, new DrawCall(tag, text, font, position, scale, rotation, origin, color, depth, SpriteEffects.None));
        }
        public void DrawString(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, Rectangle sourceRectangle, float scale, float rotation, Vector2 origin, Color color, float depth)
        {
            drawCalls.Add(depth, new DrawCall(tag, text, font, position, scale, rotation, origin, color, depth, SpriteEffects.None));
        }
        public void DrawString(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, float depth)
        {
            drawCalls.Add(depth, new DrawCall(tag, text, font, position, scale, rotation, origin, color, depth, SpriteEffects.None));
        }
        public void DrawString(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, float scale, float rotation, Vector2 origin, Color color, float depth)
        {
            drawCalls.Add(depth, new DrawCall(tag, text, font, position, scale, rotation, origin, color, depth, SpriteEffects.None));
        }
        public void DrawString(DrawCall.Tag tag, string text, SpriteFont font, Vector2 position, Color color, float depth)
        {
            drawCalls.Add(depth, new DrawCall(tag, text, font, position, 1f, 0f, Vector2.Zero, color, depth, SpriteEffects.None));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="texture"></param>
        /// <param name="rectangle"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <param name="spriteEffects"></param>
        /// <param name="affectedByLight">0 - always dark, 1 - dynamic, 2 - always bright</param>
        /// <param name="lightBleedThrough">The amount of light able to pass through the object</param>
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, Vector2 scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects, short affectedByLight, float lightBleedThrough)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, sourceRectangle, scale, rotation, origin, color, depth, spriteEffects, affectedByLight, lightBleedThrough);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="texture"></param>
        /// <param name="rectangle"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <param name="affectedByLight">0 - always dark, 1 - dynamic, 2 - always bright</param>
        /// <param name="lightBleedThrough">The amount of light able to pass through the object</param>
        public void Draw(DrawCall.Tag tag, Texture2D texture, Rectangle rectangle, Rectangle sourceRectangle, Color color, float depth, short affectedByLight, float lightBleedThrough)
        {
            DrawCall toAdd = new DrawCall(tag, texture, rectangle, sourceRectangle, 0f, Vector2.Zero, color, depth, SpriteEffects.None, affectedByLight, lightBleedThrough);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, Vector2 scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, sourceRectangle, scale, rotation, origin, color, depth, spriteEffects, 0, 0);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, scale, rotation, origin, color, depth, spriteEffects);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, float rotation, Vector2 origin, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, sourceRectangle, rotation, origin, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, Vector2 scale, float rotation, Vector2 origin, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, sourceRectangle, scale, rotation, origin, color, depth, SpriteEffects.None, 0, 0);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, float scale, float rotation, Vector2 origin, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, sourceRectangle, scale, rotation, origin, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Vector2 scale, float rotation, Vector2 origin, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, scale, rotation, origin, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, float scale, float rotation, Vector2 origin, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, scale, rotation, origin, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, 1f, 0f, Vector2.Zero, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Vector2 position, Rectangle sourceRectangle, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, position, sourceRectangle, 0f, 0f, Vector2.Zero, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="texture"></param>
        /// <param name="rectangle"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="color"></param>
        /// <param name="depth"></param>
        /// <param name="spriteEffects"></param>
        /// <param name="affectedByLight">0 - always dark, 1 - dynamic, 2 - always bright</param>
        /// <param name="lightBleedThrough">The amount of light able to pass through the object</param>
        public void Draw(DrawCall.Tag tag, Texture2D texture, Rectangle rectangle, Rectangle sourceRectangle, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects, short affectedByLight, float lightBleedThrough)
        {
            DrawCall toAdd = new DrawCall(tag, texture, rectangle, sourceRectangle, rotation, origin, color, depth, spriteEffects, affectedByLight, lightBleedThrough);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Rectangle rectangle, Rectangle sourceRectangle, float rotation, Vector2 origin, Color color, float depth, SpriteEffects spriteEffects)
        {
            DrawCall toAdd = new DrawCall(tag, texture, rectangle, sourceRectangle, rotation, origin, color, depth, spriteEffects);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Rectangle rectangle, Rectangle sourceRectangle, float rotation, Vector2 origin, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, rectangle, sourceRectangle, rotation, origin, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Rectangle rectangle, float rotation, Vector2 origin, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, rectangle, rotation, origin, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Rectangle rectangle, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, rectangle, 0f, Vector2.Zero, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        public void Draw(DrawCall.Tag tag, Texture2D texture, Rectangle rectangle, Rectangle sourceRectangle, Color color, float depth)
        {
            DrawCall toAdd = new DrawCall(tag, texture, rectangle, sourceRectangle, 0f, Vector2.Zero, color, depth, SpriteEffects.None);
            if (toAdd.Rectangle.Intersects(Game1.camera.Rectangle))
                drawCalls.Add(depth, toAdd);
        }
        #endregion
    }
}
