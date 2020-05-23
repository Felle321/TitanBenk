using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace Quest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DrawBatch drawBatch;
        public static Texture2D pixel, fader;
        public static SpriteFont fontDebug;
        public static Random random = new Random();

        public static List<Agent> agents = new List<Agent>();
        public static List<GameObject> gameObjects = new List<GameObject>();
        public static List<DamageZone> damageZones = new List<DamageZone>();
        public static List<EffectObject> effects = new List<EffectObject>();
        public static Terrain terrain = new Terrain();

        public static bool rigDrawTextures = false;
        public static string selectedLimb = "Neck";

        public static Camera camera = new Camera(Vector2.Zero);

        public static Player Player
        {
            get { return (Player)agents[0]; }
        }

        //TempRenderTest
        bool renderShadows = true;
        RenderTarget2D renderTargetToDraw;

        #region Rendering
        RenderTarget2D RTGameWorld, RTLights, RTFinal, RTDepthMask;
        public static float globalBrightness = 0.3f;
        public static Effect FXDepthMask, FXApplyShadowsToLights;
        #endregion

        #region ScreenMeasurements
        protected static int screenWidth = 1920;
        protected static int screenHeight = 1080;

        protected static int displayWidth = 1280;
        protected static int displayHeight = 720;

        public static int ScreenWidth
        {
            get
            {
                return screenWidth;
            }
        }
        public static int ScreenHeight
        {
            get
            {
                return screenHeight;
            }
        }

        public void SetScreenWidth(int width)
        {
            graphics.PreferredBackBufferWidth = width;
            screenWidth = width;
        }
        public void SetScreenHeight(int height)
        {
            graphics.PreferredBackBufferHeight = height;
            screenHeight = height;
        }
        #endregion

        #region Input
        public static KeyboardState keyboardState, keyboardStatePrev;
        public static MouseState mouseState, mouseStatePrev;
        public static Vector2 mousePosInWorld;
        public static Point mouseTile;
        #endregion

        #region NextID
        private static int nextIDAgent = 0;
        private static int nextIDGameObject = 0;
        private static int nextIDDamageZone = 0;
        public static int NextIDAgent { get { nextIDAgent++; return nextIDAgent-1; } }
        public static int NextIDGameObject { get { nextIDGameObject++; return nextIDGameObject-1; } }
        public static int NextIDDamageZone { get { nextIDDamageZone++; return nextIDDamageZone-1; } }
        #endregion

        #region Efficiency
        int performanceAverageReset;

        DateTime performanceDateTime = DateTime.Now;
        DateTime performanceDateTimeTotal = DateTime.Now;

        float millisecondBudget = 1000 / 60f;

        float millisecondsPerFrameAverage = 0f;
        float[] millisecondsLastFrames = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        float millisecondsPerFrameAverageDraw = 0f;
        float[] millisecondsLastFramesDraw = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        float millisecondsPerFrameAverageUpdate = 0f;
        float[] millisecondsLastFramesUpdate = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = displayWidth;
            graphics.PreferredBackBufferHeight = displayHeight;
        }

        protected override void Initialize()
        {
            camera.zoom = 2.5f;
            Controls.Initialize();

            RTGameWorld = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight);
            RTLights = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight);
            RTFinal = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight);
            RTDepthMask = new RenderTarget2D(GraphicsDevice, screenWidth, screenHeight);
            renderTargetToDraw = RTDepthMask;

            Ability.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            drawBatch = new DrawBatch();
            pixel = Content.Load<Texture2D>("pixel");
            fader = Content.Load<Texture2D>("Fader");
            fontDebug = Content.Load<SpriteFont>("debug");
            Terrain.LoadContent(Content);
            Projectile.LoadContent(Content);
            EffectObject.LoadContent(Content);
            Particle.LoadContent(Content);
            Ability.LoadContent(Content);
            GUI.LoadContent(Content);
            Item.LoadContent(Content);
            FXDepthMask = Content.Load<Effect>("Shaders\\FXDepthMask");
            FXApplyShadowsToLights = Content.Load<Effect>("Shaders\\FXApplyShadowsToLight");

            Rig.LoadContent(Content);
            RigAnimation.LoadContent(Content);
            RigLimbDrawer.LoadContent(GraphicsDevice, Content);

            agents.Add(new Player(new Vector2(100)));
            agents.Add(new Agent(new Vector2(40), "Könet", Agent.Tag.Enemy, CharacterStats.Default));
            agents.Add(new Agent(new Vector2(60), "Gypsit", Agent.Tag.Enemy, CharacterStats.Default));

            agents[1].collider.SetActiveAreaCircle(0, 3.14f);

            TileObject.LoadContent(Content);

            for (int i = 0; i < 100; i++)
            {
                terrain.TryCreateTileObject(new Tree(new Point(random.Next(terrain.size.X), random.Next(terrain.size.Y))));
            }
        }

        protected override void UnloadContent()
        {

        }

        #region PerformanceUpdates
        public void PerformanceUpdateMS()
        {
            if (performanceAverageReset >= millisecondsLastFrames.Length)
                performanceAverageReset = 0;
            else
                performanceAverageReset++;

            if(performanceAverageReset == 0)
                millisecondsPerFrameAverage = 0;
            for (int i = millisecondsLastFrames.Length - 1; i > 0; i--)
            {
                millisecondsLastFrames[i] = millisecondsLastFrames[i - 1];
                if (performanceAverageReset == 0)
                    millisecondsPerFrameAverage += millisecondsLastFrames[i];
            }


            millisecondsLastFrames[0] = millisecondsLastFramesDraw[0] + millisecondsLastFramesUpdate[0];
            if (performanceAverageReset == 0)
                millisecondsPerFrameAverage += millisecondsLastFrames[0];

            if (performanceAverageReset == 0)
                millisecondsPerFrameAverage /= (float)millisecondsLastFrames.Length;
        }
        public void PerformanceUpdateMSUpdate()
        {
            if (performanceAverageReset == 0)
                millisecondsPerFrameAverageUpdate = 0;

            for (int i = millisecondsLastFramesUpdate.Length - 1; i > 0; i--)
            {
                millisecondsLastFramesUpdate[i] = millisecondsLastFramesUpdate[i - 1];
                if (performanceAverageReset == 0)
                    millisecondsPerFrameAverageUpdate += millisecondsLastFramesUpdate[i];
            }

            millisecondsLastFramesUpdate[0] = (float)(DateTime.Now - performanceDateTime).TotalMilliseconds;
            if (performanceAverageReset == 0)
                millisecondsPerFrameAverageUpdate += millisecondsLastFramesUpdate[0];

            if (performanceAverageReset == 0)
                millisecondsPerFrameAverageUpdate /= millisecondsLastFramesUpdate.Length;
        }
        public void PerformanceUpdateMSDraw()
        {
            if (performanceAverageReset == 0)
                millisecondsPerFrameAverageDraw = 0;

            for (int i = millisecondsLastFramesDraw.Length - 1; i > 0; i--)
            {
                millisecondsLastFramesDraw[i] = millisecondsLastFramesDraw[i - 1];
                if (performanceAverageReset == 0)
                    millisecondsPerFrameAverageDraw += millisecondsLastFramesDraw[i];
            }

            millisecondsLastFramesDraw[0] = (float)(DateTime.Now - performanceDateTime).TotalMilliseconds;
            if (performanceAverageReset == 0)
                millisecondsPerFrameAverageDraw += millisecondsLastFramesDraw[0];

            if (performanceAverageReset == 0)
                millisecondsPerFrameAverageDraw /= millisecondsLastFramesDraw.Length;
        }
        #endregion

        protected override void Update(GameTime gameTime)
        {
            performanceDateTime = DateTime.Now;
            performanceDateTimeTotal = DateTime.Now;

            keyboardStatePrev = keyboardState;
            keyboardState = Keyboard.GetState();
            mouseStatePrev = mouseState;
            mouseState = Mouse.GetState();
            drawBatch.Begin();
            camera.Update(agents[0].PositionXY + agents[0].origin.ToVector2(), mousePosInWorld);

            if (KeyDown(Keys.H))
            {
                gameObjects.Add(new DroppedItem(new Vector3(mousePosInWorld.X, mousePosInWorld.Y, 16), new Vector3(random.Next(10) - 5, random.Next(10) - 5, (float)(random.NextDouble() * 4)), Item.GetItem(Item.ItemID.Turd, 1)));
            }
            if (KeyDown(Keys.J))
            {
                gameObjects.Add(new ExpOrb(new Vector3(mousePosInWorld.X, mousePosInWorld.Y, 16), new Vector3(random.Next(10) - 5, random.Next(10) - 5, (float)(random.NextDouble() * 4)), 50));
            }
            if (KeyDown(Keys.L))
            {
                gameObjects.Add(new Particle.Ember(new Vector3(mousePosInWorld.X, mousePosInWorld.Y, 16), new Vector3(random.Next(10) - 5, random.Next(10) - 5, (float)(random.NextDouble() * 4)), random.Next(50) + 40, Color.Gold, (float)random.NextDouble() + .5f));
            }

            if (KeyPressed(Keys.X))
                switch (SelectionHandler.CurrentTarget)
                {
                    case SelectionHandler.Target.Tile:
                        SelectionHandler.CurrentTarget = SelectionHandler.Target.GameObject;
                        break;
                    case SelectionHandler.Target.GameObject:
                        SelectionHandler.CurrentTarget = SelectionHandler.Target.Tile;
                        break;
                    default:
                        break;
                }

            if (KeyPressed(Keys.E))
                agents.Add(new Agent(mousePosInWorld, "Gypsit", Agent.Tag.Enemy, CharacterStats.Default));

            //if (KeyPressed(Keys.D1))
            //    gameObjects.Add(new Projectile.Fireball(agents[0].position + new Vector3(0,0,8), Extensions.GetVector2(new Vector2(mousePosInWorld.X - agents[0].position.X, mousePosInWorld.Y - agents[0].position.Y).GetAngle(), 6f).ToVector3(0), 0, Agent.Tag.Player));
            
            //if (KeyPressed(Keys.D2))
            //    gameObjects.Add(new Projectile.BouncingBall(agents[0].position + new Vector3(0, 0, 8), Extensions.GetVector2(new Vector2(mousePosInWorld.X - agents[0].position.X, mousePosInWorld.Y - agents[0].position.Y).GetAngle(), 4f).ToVector3(4), 0, Agent.Tag.Player));

            mousePosInWorld = new Vector2(((mouseState.X - displayWidth / 2) * ((float)screenWidth / displayWidth) / camera.zoom + camera.pos.X), ((mouseState.Y - displayHeight / 2) * ((float)screenHeight / displayHeight)) / camera.zoom + camera.pos.Y);
            mouseTile = WorldPosToTile(mousePosInWorld.ToPoint());

            if (KeyPressed(Keys.Q))
            {
                if (KeyDown(Keys.LeftShift) && KeyDown(Keys.LeftControl))
                {
                    for (int i = 0; i < random.Next(100); i++)
                    {
                        SpawnExplosionEffect(new Vector3(mousePosInWorld.X + random.Next(200) - 100, mousePosInWorld.Y + random.Next(200) - 100, 8), 5f);
                    }
                    for (int i = 0; i < random.Next(100); i++)
                    {
                        SpawnExplosionEffect(new Vector3(mousePosInWorld.X + random.Next(200) - 100, mousePosInWorld.Y + random.Next(300) - 150, 8), 1.2f);
                    }
                }
                else if( KeyDown(Keys.LeftShift))
                {
                    for (int i = 0; i < random.Next(100); i++)
                    {
                        SpawnExplosionEffect(new Vector3(mousePosInWorld.X + random.Next(200) - 100, mousePosInWorld.Y + random.Next(400) - 200, 8), 1.2f);
                    }
                }
                else if (KeyDown(Keys.LeftControl))
                {
                    for (int i = 0; i < random.Next(50); i++)
                    {
                        SpawnExplosionEffect(new Vector3(mousePosInWorld.X + random.Next(400) - 200, mousePosInWorld.Y + random.Next(400) - 200, 8), 1f);
                    }
                }
                else
                    SpawnExplosionEffect(new Vector3(mousePosInWorld.X, mousePosInWorld.Y, 8), 1f);
            }

            if (KeyPressed(Keys.K))
                renderShadows = !renderShadows;

            if (KeyPressed(Keys.Y))
                renderTargetToDraw = RTFinal;
            if (KeyPressed(Keys.U))
                renderTargetToDraw = RTLights;
            if (KeyPressed(Keys.I))
                renderTargetToDraw = RTDepthMask;
            if (KeyPressed(Keys.O))
                renderTargetToDraw = RTGameWorld;

            SelectionHandler.Update();

            if (Game1.MouseButtonPressedRight())
            {
                if (SelectionHandler.tag == GameObject.Type.Agent)
                {
                    int index = GetAgentIndex(SelectionHandler.selectedID);
                    if (index >= 0)
                        //agents[index].WalkTowardsTile(new Point((int)(mousePosInWorld.X / World.tileSize), (int)(mousePosInWorld.Y / World.tileSize)));
                        agents[index].PathFind(terrain, mouseTile);
                }
            }

            for (int i = 0; i < damageZones.Count; i++)
            {
                if(damageZones[i].lifeSpan > 0)
                    damageZones[i].Update();
                else
                {
                    damageZones.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (!gameObjects[i].remove)
                {
                    gameObjects[i].Update();
                }
                else
                {
                    gameObjects.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].GetCurrentHealth() > 0)
                {
                    if (SelectionHandler.TrySelect && agents[i].SelectionHitBox.Contains(mousePosInWorld))
                    {
                        SelectionHandler.SelectGameObject(agents[i].ID, agents[i].type);
                    }
                    agents[i].Update();

                    for (int j = 0; j < damageZones.Count; j++)
                    {
                        if (!damageZones[j].IsTagIgnored(agents[i].tag) && damageZones[j].collider.CollisionCheck(agents[i].collider, false))
                        {
                            agents[i].TakeDamage(damageZones[j].TakeDamage(agents[i].ID));
                        }
                    }
                }
                else
                {
                    agents[i].Die();
                    agents.RemoveAt(i);
                    i--;
                }
            }

            base.Update(gameTime);
            PerformanceUpdateMSUpdate();
        }


        protected override void Draw(GameTime gameTime)
        {
            performanceDateTime = DateTime.Now;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawGame();

            DrawDepthMask();

            DrawLightScene();

            ApplyRTLightsToFinal();

            GraphicsDevice.SetRenderTarget(RTFinal);

            spriteBatch.Begin();
            DrawHUD();
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(renderTargetToDraw, new Rectangle(0, 0, displayWidth, displayHeight), Color.White);
            spriteBatch.End();

            drawBatch.Begin();

            base.Draw(gameTime);
            PerformanceUpdateMSDraw();
            PerformanceUpdateMS();
        }

        #region DrawFunctions
        private void DrawGame()
        {
            //Draw stuff here
            terrain.Draw(drawBatch);


            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].Draw(drawBatch);
                if (agents[i].state == Agent.State.Walking)
                {
                    for (int j = 0; j < agents[i].path.instructions.Length; j++)
                    {
                        drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, new Rectangle(agents[i].path.instructions[j].X * 16, agents[i].path.instructions[j].Y * 16, World.tileSize, World.tileSize), Color.Lerp(Color.Black, Color.Transparent, .5f), 1f);
                    }
                }
            }

            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Draw(drawBatch);
            }

            List<Point> intersectingTiles = new List<Point>();
            Point p = new Point(-1);
            if (MouseButtonDownLeft(false))
            {
                /*
                if (terrain.RayCastObstacle(SelectionHandler.selectionPoint.ToVector2(), SelectionHandler.selectionPointSecond.ToVector2(), out p, out intersectingTiles))
                {
                    DrawLine(drawBatch, SelectionHandler.selectionPoint.ToVector2(), SelectionHandler.selectionPointSecond.ToVector2(), 2, Color.Red, 1f);
                    drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, new Rectangle(p.X * World.tileSize, p.Y * World.tileSize, World.tileSize, World.tileSize), Color.Lerp(Color.Red, Color.Transparent, .5f), 1f);
                }
                else
                    DrawLine(drawBatch, SelectionHandler.selectionPoint.ToVector2(), SelectionHandler.selectionPointSecond.ToVector2(), 2, Color.Green, 1f);

                for (int i = 0; i < intersectingTiles.Count; i++)
                {
                    drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, new Rectangle(intersectingTiles[i].X * World.tileSize, intersectingTiles[i].Y * World.tileSize, World.tileSize, World.tileSize), Color.Lerp(Color.Blue, Color.Transparent, .5f), 1f);
                }
                */
            }

            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, new Rectangle((int)mousePosInWorld.X - 2, (int)mousePosInWorld.Y - 2, 4, 4), Color.Green, 1f);

            if (SelectionHandler.CurrentTarget == SelectionHandler.Target.Tile)
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, new Rectangle(mouseTile.X * World.tileSize, mouseTile.Y * World.tileSize, World.tileSize, World.tileSize), Color.Lerp(Color.Lime, Color.Transparent, .5f), 1f);

            if (SelectionHandler.CurrentTarget == SelectionHandler.Target.Tile)
                for (int i = 0; i < SelectionHandler.TileRectangle.Width; i++)
                {
                    for (int j = 0; j < SelectionHandler.TileRectangle.Height; j++)
                    {
                        if ((SelectionHandler.TileRectangle.X + i) * World.tileSize > 0 && (SelectionHandler.TileRectangle.Y + i) * World.tileSize > 0)
                            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, new Rectangle((SelectionHandler.TileRectangle.X + i) * World.tileSize, (SelectionHandler.TileRectangle.Y + j) * World.tileSize, World.tileSize, World.tileSize), Color.Lerp(Color.Purple, Color.Transparent, .5f), 1f);
                    }
                }
            else
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, SelectionHandler.SelectionRectangle, Color.Lerp(Color.Olive, Color.Transparent, .5f), 1f);


            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].remove)
                {
                    effects.RemoveAt(i);
                    i--;
                }
                else
                    effects[i].Draw(drawBatch);
            }


            DrawLights();

            DrawRectangle(drawBatch, camera.Rectangle, 2f, Color.OrangeRed, 1f);

            //Draw GameWorld to the scene and rendertarget
            GraphicsDevice.SetRenderTarget(RTGameWorld);
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(GraphicsDevice));
            drawBatch.DrawSceneGameObjects(spriteBatch);
            spriteBatch.End();
        }
        private void DrawDepthMask()
        {
            GraphicsDevice.SetRenderTarget(RTDepthMask);
            GraphicsDevice.Clear(new Color(0, 1f, 0, 1f));

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, FXDepthMask, camera.get_transformation(GraphicsDevice));
            drawBatch.DrawDepthMask(spriteBatch);
            spriteBatch.End();
        }
        private void DrawLights()
        {
            drawBatch.Draw(DrawBatch.DrawCall.Tag.Light, fader, new Vector2(150, 100), Color.Orange, DrawBatch.CalculateDepth(new Vector2(150, 100)));
            drawBatch.Draw(DrawBatch.DrawCall.Tag.Light, fader, new Vector2(570, 290), Color.Cyan, DrawBatch.CalculateDepth(new Vector2(570, 290)));
            drawBatch.Draw(DrawBatch.DrawCall.Tag.Light, fader, new Vector2(344, 320), Color.Purple, DrawBatch.CalculateDepth(new Vector2(344, 320)));

            if(KeyDown(Keys.F))
                drawBatch.Draw(DrawBatch.DrawCall.Tag.Light, fader, mousePosInWorld, new Vector2(2, 2), 0f, new Vector2(128), Color.White, DrawBatch.CalculateDepth(mousePosInWorld));
        }
        private void DrawLightScene()
        {
            GraphicsDevice.SetRenderTarget(RTLights);
            GraphicsDevice.Clear(Color.Lerp(Color.Black, Color.White, globalBrightness));
            if (renderShadows)
            {
                FXApplyShadowsToLights.Parameters["Zoom"].SetValue(camera.zoom);
                FXApplyShadowsToLights.Parameters["CameraX"].SetValue(camera.Position.X);
                FXApplyShadowsToLights.Parameters["CameraY"].SetValue(camera.Position.Y);
                FXApplyShadowsToLights.Parameters["Mask"].SetValue((Texture2D)RTDepthMask);
                FXApplyShadowsToLights.Parameters["MaskWidth"].SetValue((float)RTDepthMask.Width);
                FXApplyShadowsToLights.Parameters["MaskHeight"].SetValue((float)RTDepthMask.Height);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, FXApplyShadowsToLights, camera.get_transformation(GraphicsDevice));
            }
            else
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.get_transformation(GraphicsDevice));
            drawBatch.DrawSceneLights(spriteBatch);
            spriteBatch.End();
        }
        private void ApplyRTLightsToFinal()
        {
            GraphicsDevice.SetRenderTarget(RTFinal);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(RTGameWorld, Vector2.Zero, Color.White);
            spriteBatch.End();

            BlendState blendState = new BlendState();
            blendState.AlphaDestinationBlend = Blend.SourceColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;
            blendState.AlphaSourceBlend = Blend.Zero;
            blendState.ColorSourceBlend = Blend.Zero;


            spriteBatch.Begin(SpriteSortMode.Immediate, blendState, null, null, null, null, null);
            spriteBatch.Draw(RTLights, Vector2.Zero, Color.Lerp(Color.White, Color.Transparent, 0f));
            spriteBatch.End();
        }
        private void DrawHUD()
        {
            //Draw HUD here
            spriteBatch.DrawString(fontDebug, "Selected: " + SelectionHandler.selectedTile.ToString(), new Vector2(20, 40), Color.White);
            spriteBatch.DrawString(fontDebug, "Rect: " + SelectionHandler.TileRectangle.ToString(), new Vector2(20, 80), Color.White);
            spriteBatch.DrawString(fontDebug, "Second: " + SelectionHandler.selectedTileSecond.ToString(), new Vector2(20, 120), Color.White);


            spriteBatch.DrawString(fontDebug, "Select Target: " + SelectionHandler.CurrentTarget.ToString(), new Vector2(20, 140), Color.White);
            spriteBatch.DrawString(fontDebug, "SelectionRect: " + SelectionHandler.SelectionRectangle.ToString(), new Vector2(20, 180), Color.White);
            spriteBatch.DrawString(fontDebug, "Selected: " + SelectionHandler.selectionPoint.ToString(), new Vector2(20, 200), Color.White);
            spriteBatch.DrawString(fontDebug, "Second: " + SelectionHandler.selectionPointSecond.ToString(), new Vector2(20, 220), Color.White);
            spriteBatch.DrawString(fontDebug, "MouseTile: " + mouseTile.ToString(), new Vector2(20, 260), Color.White);
            spriteBatch.DrawString(fontDebug, "MousePosInWrld: " + mousePosInWorld.ToString(), new Vector2(20, 280), Color.White);
            spriteBatch.DrawString(fontDebug, "Cam.pos: " + camera.pos.ToString(), new Vector2(20, 320), Color.White);
            spriteBatch.DrawString(fontDebug, "Cam.Position: " + camera.Position.ToString(), new Vector2(20, 340), Color.White);

            spriteBatch.DrawString(fontDebug, "MouseDepth: " + DrawBatch.CalculateDepth(mousePosInWorld).ToString(), new Vector2(20, 380), Color.Red);
            spriteBatch.DrawString(fontDebug, "Drawcalls: " + drawBatch.DrawCallCount.ToString(), new Vector2(20, 400), Color.Red);

            spriteBatch.DrawString(fontDebug, "Average Time per Frame: " + millisecondsPerFrameAverage, new Vector2(20, 450), Color.Red);

            spriteBatch.DrawString(fontDebug, "Average Time per Frame (Upd): " + millisecondsPerFrameAverageUpdate, new Vector2(20, 480), Color.Red);
            spriteBatch.DrawString(fontDebug, "Average Time per Frame (Drw): " + millisecondsPerFrameAverageDraw, new Vector2(20, 500), Color.Red);

            spriteBatch.DrawString(fontDebug, "Time Budget Total: " + millisecondBudget, new Vector2(20, 550), Color.Red);
            spriteBatch.DrawString(fontDebug, "Time Budget Left: " + (millisecondBudget - millisecondsPerFrameAverage), new Vector2(20, 580), Color.Red);

            spriteBatch.DrawString(fontDebug, "AbilityHandler: ", new Vector2(400, 450), Color.Red);
            int i = 0;
            foreach (var item in ((Player)agents[0]).abilityHandler.executers)
            {
                spriteBatch.DrawString(fontDebug, i.ToString() + ": " + item.Value.type + ", " + item.Value.Ready, new Vector2(400, 480), Color.Red);
                i++;
            }
            spriteBatch.DrawString(fontDebug, "Time Budget Left: " + (millisecondBudget - millisecondsPerFrameAverage), new Vector2(20, 580), Color.Red);

            //if (agents.Count > 1)
            //    drawBatch.DrawString(fontDebug, "Colliding Circles: " + agents[0].collider.CollisionCheck(agents[1].collider, false), new Vector2(20, 320), Color.White);

            GUI.Draw(spriteBatch);

            spriteBatch.Draw(pixel, new Rectangle((int)(mouseState.X * ((float)screenWidth / displayWidth)) - 2, (int)(mouseState.Y * ((float)screenHeight / displayHeight)) - 2, 4, 4), Color.Brown);
        }
        #endregion

        #region DrawLine
        public static void DrawLine(DrawBatch drawBatch, Vector2 v1, Vector2 v2, float thickness, Color color, float depth)
        {
            Vector2 delta = new Vector2(v2.X - v1.X, v2.Y - v1.Y); drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, v1, new Vector2(delta.Length(), thickness), Extensions.GetAngle(delta), Vector2.Zero, color, depth);
        }
        public static void DrawLine(DrawBatch drawBatch, Vector4 v, float thickness, Color color,float depth)
        {
            Vector2 delta = new Vector2(v.Z - v.X, v.W - v.Y);
            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, pixel, new Vector2(v.X, v.Y), new Vector2(delta.Length(), thickness), Extensions.GetAngle(delta), Vector2.Zero, color, depth);
        }
        public static void DrawRectangle(DrawBatch drawBatch, Rectangle rect, float thickness, Color color, float depth)
        {
            Game1.DrawLine(drawBatch, new Vector4(rect.X, rect.Y, rect.X + rect.Width, rect.Y), thickness, color, depth);
            Game1.DrawLine(drawBatch, new Vector4(rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height), thickness, color, depth);
            Game1.DrawLine(drawBatch, new Vector4(rect.X + rect.Width, rect.Y + rect.Height, rect.X, rect.Y + rect.Height), thickness, color, depth);
            Game1.DrawLine(drawBatch, new Vector4(rect.X, rect.Y + rect.Height, rect.X, rect.Y), thickness, color, depth);
        }
        #endregion

        public static void SpawnExplosionEffect(Vector3 position, float scale)
        {
            effects.Add(new EffectObject.Explosion(new Vector3(position.X, position.Y, position.Z), new Vector2(scale)));
            camera.AddShake(scale, 1f,(int)(scale * 10));
        }

        public static Point WorldPosToTile(Point position)
        {
            if (position.X < 0)
                position.X -= World.tileSize;
            if (position.Y < 0)
                position.Y -= World.tileSize;

            return new Point((int)position.X - (int)(position.X % World.tileSize), (int)position.Y - (int)(position.Y % World.tileSize)).ScaleRet(1 / (float)World.tileSize);
        }

        /// <summary>
        /// Finds the index of GameObject with given ID. -1 if not found
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetGameObjectIndex(int ID)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].ID == ID)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of Agent with given ID. -1 if not found
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetAgentIndex(int ID)
        {
            if (ID < 0)
                return -1;

            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].ID == ID)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of Agent with given ID. -1 if not found
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static int GetDamageZoneIndex(int ID)
        {
            if (ID < 0)
                return -1;

            for (int i = 0; i < damageZones.Count; i++)
            {
                if (damageZones[i].ID == ID)
                {
                    return i;
                }
            }

            return -1;
        }

        #region Mouse and Keyboard
        public static bool KeyPressed(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && keyboardStatePrev.IsKeyUp(key));
        }
        public static bool KeyReleased(Keys key)
        {
            return (keyboardState.IsKeyUp(key) && keyboardStatePrev.IsKeyDown(key));
        }
        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }
        public static bool KeyUp(Keys key)
        {
            return keyboardState.IsKeyUp(key);
        }
        public static bool MouseButtonPressedLeft()
        {
            return (mouseState.LeftButton == ButtonState.Pressed && mouseStatePrev.LeftButton == ButtonState.Released);
        }
        public static bool MouseButtonReleasedLeft()
        {
            return (mouseState.LeftButton == ButtonState.Pressed && mouseStatePrev.LeftButton == ButtonState.Released);
        }
        public static bool MouseButtonPressedRight()
        {
            return (mouseState.RightButton == ButtonState.Pressed && mouseStatePrev.RightButton == ButtonState.Released);
        }
        public static bool MouseButtonReleasedRight()
        {
            return (mouseState.RightButton == ButtonState.Pressed && mouseStatePrev.RightButton == ButtonState.Released);
        }
        public static bool MouseButtonDownLeft(bool prev)
        {
            if (prev)
                return mouseStatePrev.LeftButton == ButtonState.Pressed;
            else
                return mouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool MouseButtonUpLeft(bool prev)
        {
            if (prev)
                return mouseStatePrev.LeftButton == ButtonState.Released;
            else
                return mouseState.LeftButton == ButtonState.Released;
        }
        public static bool MouseButtonDownRight(bool prev)
        {
            if (prev)
                return mouseStatePrev.RightButton == ButtonState.Pressed;
            else
                return mouseState.RightButton == ButtonState.Pressed;
        }
        public static bool MouseButtonUpRight(bool prev)
        {
            if (prev)
                return mouseStatePrev.RightButton == ButtonState.Released;
            else
                return mouseState.RightButton == ButtonState.Released;
        }
        #endregion
    }
}
