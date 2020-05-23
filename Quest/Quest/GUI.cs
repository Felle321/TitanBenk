using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public static class GUI
    {
        public static Texture2D tex2d_elementSheet, tex2d_HUDProgressbarSheet, tex2d_HUDHotBar;
        public static Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        static List<string> scenesToDraw = new List<string>();

        public static void StartDrawScene(string scene)
        {
            if (!scenesToDraw.Contains(scene))
                scenesToDraw.Add(scene);
        }

        public static void StopDrawScene(string scene)
        {
            if (scenesToDraw.Contains(scene))
                scenesToDraw.Remove(scene);
        }

        internal static void LoadContent(ContentManager content)
        {
            tex2d_elementSheet = content.Load<Texture2D>("GUI\\elementSheet");
            tex2d_HUDProgressbarSheet = content.Load<Texture2D>("GUI\\HUDProgressBarSheet");
            tex2d_HUDHotBar = content.Load<Texture2D>("GUI\\hotBarBG");

            scenes.Add("HUD", new Scene.HUD());
            scenes.Add("Inventory", new Scene.Inventory());
            scenes.Add("PauseMenu", new Scene.PauseMenu());

            StartDrawScene("HUD");
        }

        public static void DrawHotbar(SpriteBatch spriteBatch, Vector2 position, float spacing, float scale)
        {
            Color color;


            Point pos = position.ToPoint();
            Point size = new Point((int)(scale * Ability.iconSize));
            for (int i = 0; i < Game1.Player.hotBar.Length; i++)
            {
                if (Game1.Player.hotBarInputs[i])
                    color = Color.Red;
                else
                    color = Color.White;
                if (i > 0)
                    pos.X = (int)position.X + (int)(i * (size.X + spacing));
                Game1.Player.abilityHandler.DrawIcon(Game1.Player.hotBar[i], spriteBatch, new Rectangle(pos, size), color);
            }
        }

        internal static void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < scenesToDraw.Count; i++)
            {
                scenes[scenesToDraw[i]].Draw(spriteBatch);
            }
        }



        public abstract class Element
        {
            public Rectangle drawRectangle;
            public Color color;

            public Element(Rectangle drawRectangle, Color color)
            {
                this.drawRectangle = drawRectangle;
                this.color = color;
            }

            public virtual void Draw(SpriteBatch spriteBatch)
            {

            }
        }

        public class TextureElement : Element
        {
            public Texture2D texture;
            public TextureElement(Texture2D texture, Rectangle drawRectangle, Color color) : base(drawRectangle, color)
            {
                this.texture = texture;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(texture, drawRectangle, color);
            }
        }

        public class ProgressBar : Element
        {
            public float progress;
            public Texture2D texture;
            /// <summary>
            /// Determines wether the sprite stretches or 
            /// </summary>
            public bool stretch;
            public bool reversed;

            public Rectangle GetSourceRectangleOffsets()
            {
                Rectangle ret = new Rectangle(0, 0, texture.Width, texture.Height);

                int delta = (int)(ret.Width * (1 - progress));

                if (!stretch)
                {
                    ret.Width = ret.Width - delta;

                    if (reversed)
                        ret.X = delta;
                }

                return ret;
            }

            public Rectangle GetRectangleToDraw(Rectangle originalRectangle)
            {
                Rectangle ret = originalRectangle;
                int delta = (int)(ret.Width * (1 - progress));
                if (reversed)
                    ret.X += delta;

                ret.Width = ret.Width - delta;

                return ret;
            }

            public ProgressBar(Rectangle drawRectangle, Color color, float progress, Texture2D texture, bool stretch, bool reversed) : base(drawRectangle, color)
            {
                this.progress = progress;
                this.texture = texture;
                this.stretch = stretch;
                this.reversed = reversed;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                Rectangle sourceOffsets = GetSourceRectangleOffsets();
                spriteBatch.Draw(texture, GetRectangleToDraw(drawRectangle), sourceOffsets, color);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tag"></param>
            /// <param name="drawBatch"></param>
            /// <param name="depth"></param>
            /// <param name="affectedByLight">0 - always dark, 1 - dynamic, 2 - always bright</param>
            /// <param name="lightBleedThrough"></param>
            internal void Draw(DrawBatch.DrawCall.Tag tag, DrawBatch drawBatch, float depth, short affectedByLight, float lightBleedThrough)
            {
                Rectangle sourceOffsets = GetSourceRectangleOffsets();
                drawBatch.Draw(tag, texture, GetRectangleToDraw(drawRectangle), sourceOffsets, color, depth, affectedByLight, lightBleedThrough);
            }
        }
        public class ProgressBarAnimated : Element
        {
            public float progress;
            public AnimatedSprite sprite;
            /// <summary>
            /// Determines wether the sprite stretches or 
            /// </summary>
            public bool stretch;
            public bool reversed;

            public Rectangle GetSourceRectangleOffsets()
            {
                Rectangle ret = new Rectangle(0, 0, sprite.Measurements.X, sprite.Measurements.Y);

                int delta = (int)(ret.Width * (1 - progress));

                if (!stretch)
                {
                    ret.Width = ret.Width - delta;

                    if (reversed)
                        ret.X = delta;
                }

                return ret;
            }

            public Rectangle GetRectangleToDraw(Rectangle originalRectangle)
            {
                Rectangle ret = originalRectangle;
                int delta = (int)(ret.Width * (1 - progress));
                if (reversed)
                    ret.X += delta;

                ret.Width = ret.Width - delta;

                return ret;
            }

            public ProgressBarAnimated(Rectangle drawRectangle, Color color, float progress, AnimatedSprite sprite, bool stretch, bool reversed) : base(drawRectangle, color)
            {
                this.progress = progress;
                this.sprite = sprite;
                this.stretch = stretch;
                this.reversed = reversed;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                Rectangle sourceOffsets = GetSourceRectangleOffsets();
                sprite.Draw(spriteBatch, GetRectangleToDraw(drawRectangle), sourceOffsets.Location, sourceOffsets.Size, color);
            }
        }



        public abstract class Scene
        {
            Dictionary<string, Element> elements = new Dictionary<string, Element>();

            public virtual void Draw(SpriteBatch spriteBatch)
            {
                foreach (var item in elements)
                {
                    item.Value.Draw(spriteBatch);
                }
            }

            public class HUD : Scene
            {
                public HUD() : base()
                {
                    elements.Add("ProgressBarHP", new ProgressBarAnimated(new Rectangle(266, 1011, 288, 64), Color.White, 1f, new AnimatedSprite(tex2d_HUDProgressbarSheet, new Point(288, 64), 12, .2f, true), false, false));
                    elements.Add("ProgressBarMana", new ProgressBarAnimated(new Rectangle(1366, 1011, 288, 32), Color.White, 1f, new AnimatedSprite(tex2d_HUDProgressbarSheet, new Point(288, 32), 12, .2f, true, new Point(0, 256)), false, false));
                    elements.Add("ProgressBarEnergy", new ProgressBarAnimated(new Rectangle(1366, 1043, 288, 32), Color.White, 1f, new AnimatedSprite(tex2d_HUDProgressbarSheet, new Point(288, 32), 12, .2f, true, new Point(0, 384)), false, false));
                    elements.Add("ProgressBarExp", new ProgressBar(new Rectangle(320, 993, 1280, 12), Color.GreenYellow, 1f, Game1.pixel, true, false));
                    elements.Add("HotBarBG", new TextureElement(tex2d_HUDHotBar, new Rectangle(256, 988, tex2d_HUDHotBar.Width, tex2d_HUDHotBar.Height), Color.White));
                }

                public override void Draw(SpriteBatch spriteBatch)
                {
                    ((ProgressBarAnimated)elements["ProgressBarHP"]).progress = ((Player)Game1.agents[0]).GetCurrentHealth() / ((Player)Game1.agents[0]).characterStats.GetValue(CharacterStats.ValueType.Health);
                    ((ProgressBarAnimated)elements["ProgressBarMana"]).progress = ((Player)Game1.agents[0]).characterStats.currentMana / ((Player)Game1.agents[0]).characterStats.GetValue(CharacterStats.ValueType.Mana);
                    ((ProgressBarAnimated)elements["ProgressBarEnergy"]).progress = ((Player)Game1.agents[0]).characterStats.currentEnergy / ((Player)Game1.agents[0]).characterStats.GetValue(CharacterStats.ValueType.Energy);
                    ((ProgressBar)elements["ProgressBarExp"]).progress = ((Player)Game1.agents[0]).EXP / ((Player)Game1.agents[0]).requiredExp;
                    base.Draw(spriteBatch);
                    DrawHotbar(spriteBatch, new Vector2(576, 1011), 16, 1f);
                }
            }
            public class Inventory : Scene
            {

            }
            public class PauseMenu : Scene
            {

            }
        }
    }
}
