using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
//using System.Drawing;

namespace Parallax_Demo
{
    struct Sand_Particle
    {
        public Vector3 Position;
        public float Acceleration;
        public Vector3 Direction;
    }


    struct VertexTangentSpace : IVertexType
    {
        public static VertexDeclaration vdecl;

        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 Bitangent;
        public Vector2 TextureCoordinate;
        public Vector2 WorldTextureCoord;
        public Vector2 Ratio;

        static VertexTangentSpace()
        {
            VertexElement[] elements = new VertexElement[] {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                    new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
                    new VertexElement(48, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElement(56, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
                    new VertexElement(64, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2)};

        vdecl = new VertexDeclaration(72, elements);
        }

        public VertexDeclaration VertexDeclaration
        {
            get { return vdecl; }
        }
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Footprint_Game : Game
    {
        const float CHILD_FOOT_SIZE = 0.6f;
        const float ADULT_FOOT_SIZE = 1.0f;
        LightDirectionDisplay lightdir;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //VertexPositionNormalTexture[] beach;
        //int[] indicesBeach;
        public Effect beachMaterial;
        public Effect parallaxEffect;
        public Effect runningEffect;
        public Texture2D normalMap;
        public Texture2D leftFoot;
        public Texture2D extract;
        Texture2D sand;
        int footstep = 0;
        FootDecal footprint;
        FootDecal overlap;
        List<FootDecal> steps = new List<FootDecal>();
        SpriteFont font;
        int toggle = 0;
        public int toggle1 = 0;
        Vector3 position;
        Matrix world, view, perspective;
        MouseState last_mouse_state;
        KeyboardState kbs;
        Player character;
        Ground beach;
        RenderTarget2D collisionMap;
        public Effect collision;
        Model sun;
        Model particle;
        Model chunk1;
        Model chunk2;
        Random rand = new Random();
        List<Sand_Particle> chunks = new List<Sand_Particle>();
        float time = 0;
        bool child_right = true;
        bool adult_right = true;
        bool right = true;
        int count = 0;
        bool once = false;

        public Footprint_Game()
        {
            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            //graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
            character = new Player();
            //graphics.ToggleFullScreen();
        }

        public void set_scene()
        {
            //FootDecal foot = new FootDecal(new Vector3(-20, 0, -35), ADULT_FOOT_SIZE, beach, 0, normalMap, this, false, adult_right, 0.15f);
            ////steps.Add(foot);
            //FootDecal f = new DecalOverlap(foot,new Vector3(-21, 0, -35f), ADULT_FOOT_SIZE, beach, (float)Math.PI / 3, normalMap, this, false, adult_right, 0.15f);
            //steps.Add(f);
            add_adult(new Vector3(-6, 0, -8.5f), 3.14f / 8f, 0.1f);
            add_adult(new Vector3(-9, 0, -10), 3.14f / 8f, 0.1f);
            add_adult(new Vector3(-8, 0, -12), 3.14f / 8f, 0.1f);
            add_adult(new Vector3(-11, 0, -13), 3.14f / 6f, 0.1f);
            add_adult(new Vector3(-10, 0, -15), 3.14f / 5f, 0.1f);
            add_adult(new Vector3(-13, 0, -15), 3.14f / 4f, 0.1f);
            add_adult(new Vector3(-13, 0, -18), 3.14f / 4f, 0.1f);
            add_adult(new Vector3(-16, 0, -18), 3.14f / 4f, 0.1f);
            add_adult(new Vector3(-16, 0, -21), 3.14f / 4f, 0.1f);
            add_adult(new Vector3(-7, 0, -6), 3.14f / 8f, 0.1f);
            add_adult(new Vector3(-4, 0, -5f), 3.14f / 4f, 0.25f);
            add_adult(new Vector3(-5, 0, -3.5f), 3.14f / 4f, 0.25f);

            add_child(new Vector3(-25, 0, -14), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-23.5f, 0, -12f), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-21, 0, -12), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-19.5f, 0, -10), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-17, 0, -10), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-15.5f, 0, -8), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-13, 0, -8), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-11.5f, 0, -6), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-9, 0, -6f), 3.14f / 3f, 0.1f);
            add_child(new Vector3(-8f, 0, -3.5f), 3.14f / 3f, 0.2f);
            add_child(new Vector3(-7.5f, 0, -4.5f), 3.14f / 3f, 0.2f);

            add_adult(new Vector3(-2, 0, -3), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(-2, 0, -0.5f), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(1f, 0, -1), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(1, 0, 1.5f), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(4, 0, 2), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(4, 0, 4.5f), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(7, 0, 5), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(7, 0, 7.5f), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(10, 0, 8), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(10, 0, 10.5f), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(13, 0, 11), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(13, 0, 13.5f), 3.14f / 4f, 0.2f);
            add_adult(new Vector3(16, 0, 14), 3.14f / 4f, 0.2f);
        }

        public void add_adult(Vector3 pos, float rotation, float weight)
        {
            FootDecal foot = new FootDecal(pos, ADULT_FOOT_SIZE, beach, rotation, normalMap, this, false, adult_right, weight);
            steps.Add(foot);
            adult_right = !adult_right;
        }

        public void add_child(Vector3 pos, float rotation, float weight)
        {
            FootDecal foot = new FootDecal(pos, CHILD_FOOT_SIZE, beach, rotation, normalMap, this, true, child_right, weight);
            steps.Add(foot);
            child_right = !child_right;
        }

        void ResetNavigation()
        {
            character.reset_Position();
        }

        public Vector3 RandomVector()
        {
            float x = (float)rand.NextDouble();
            float y = (float)rand.NextDouble();
            float z = (float)rand.NextDouble();

            return new Vector3(x, y, z);
        }


        public void add_SandParticles(int size, Vector3 location, int number_of_particles)
        {
            for(int i = 0; i < number_of_particles; i++)
            {
                Sand_Particle sand = new Sand_Particle();
                sand.Acceleration = (float)rand.NextDouble();
                sand.Direction = RandomVector();
                sand.Direction.Normalize();
                sand.Position = RandomVector() * size + location;
                chunks.Add(sand);
            }
        }

        public void draw_SandParticles(Matrix view, Matrix perspective)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                Sand_Particle sand = chunks[i];
                particle.Draw(Matrix.CreateTranslation(sand.Position), view, perspective);
            }
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            ResetNavigation();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            sun = Content.Load<Model>("Sun");
            foreach(ModelMesh mesh in sun.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
            }
            font = Content.Load<SpriteFont>("characters");
            lightdir = new LightDirectionDisplay(graphics);
            beach = new Ground();
            collisionMap = new RenderTarget2D(GraphicsDevice, 1024, 1024, true, SurfaceFormat.Color, DepthFormat.None);
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            beachMaterial = Content.Load<Effect>("BasicLighting");
            //beachMaterial = new BasicEffect(GraphicsDevice);
            //beachMaterial.TextureEnabled = true;
            //http://linolafett.deviantart.com/art/Sand-01-434854377
            sand = Content.Load<Texture2D>("Sand");
            beachMaterial.Parameters["DiffuseTex"].SetValue(sand);

            leftFoot = Content.Load<Texture2D>("LeftParallax");
            parallaxEffect = Content.Load<Effect>("Parallax");
            collision = Content.Load<Effect>("DualOverlap");
            normalMap = Content.Load<Texture2D>("FootPerfect");
            runningEffect = Content.Load<Effect>("RunningEffect");
            extract = Content.Load<Texture2D>("ExtractParallax");
            chunk1 = Content.Load<Model>("chunk1");
            chunk2 = Content.Load<Model>("chunk2");

            //footprint = new FootDecal(new Vector3(10f, 0, 12f), 2,beach, 3.14f, normalMap, this, false);
            //steps.Add(footprint);
            //overlap = new FootDecal(new Vector3(9f, 0, 10f), 1,  beach, 3.14f/4f,normalMap, this, false);
            //steps.Add(overlap);
            set_scene();
            
            foreach (ModelMesh mesh in chunk1.Meshes)
            {
                foreach(ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = beachMaterial;
                }
            }

            foreach (ModelMesh mesh in chunk2.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.Direction = lightdir.Direction;
                }
            }

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if()
            //if (count == 40 && once == false)
            //{

            //    FootDecal f = new FootDecal(character.Position, ADULT_FOOT_SIZE, beach, character.Rotation.Y + (float)Math.PI, normalMap, this, false, right, 0.1f);

            //    right = !right;
            //    count = 0;
            //    //foreach (FootDecal f1 in steps)
            //    //{
            //    //    if (f.Bounds.IntersectsWith(f1.Bounds))
            //    //    {
            //    //        steps.Remove(f1);
            //    //        once = true;
            //    //        f = new DecalOverlap(f1, character.Position, ADULT_FOOT_SIZE, beach, character.Rotation.Y + (float)Math.PI, normalMap, this, false, right, 0.1f);
            //    //        break;
            //    //    }
            //    //}

            //    steps.Add(f);
            //    //FootDecal footprint = new FootDecal(character.Position, beach, normalMap);
            //    //steps.Add(footprint);
            //}
            //else
            //    count++;

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) overlap.Z += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) overlap.Z -= 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) overlap.X += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) overlap.X -= 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) footprint.Rotation += 0.02f;
            //if (Keyboard.GetState().IsKeyDown(Keys.E)) footprint.Rotation -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.E)) once = true;
            footstep = gameTime.TotalGameTime.Seconds;

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            world = Matrix.CreateTranslation(new Vector3(0,0,0));
            
            lightdir.Update(Keyboard.GetState());

            if (Keyboard.GetState().IsKeyDown(Keys.R)) ResetNavigation();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.T) && kbs.IsKeyUp(Keys.T)) toggle = (toggle == 0 ? 1 : 0);
            if (Keyboard.GetState().IsKeyDown(Keys.F) && kbs.IsKeyUp(Keys.F)) toggle1 = (toggle1 == 0 ? 1 : 0);
            kbs = Keyboard.GetState();
            if (IsActive)
            {
                character.Update();
            }
            last_mouse_state = Mouse.GetState();

            view = Matrix.CreateLookAt(character.Position, character.Position + character.Forward, character.Up);
            perspective = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100.0f);
            if (Keyboard.GetState().IsKeyDown(Keys.G)) footprint.Age();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            List<Texture2D> images = new List<Texture2D>();
            GraphicsDevice.Clear(Color.Violet);

            //collision.Parameters["View"].SetValue(view);
            //collision.Parameters["Projection"].SetValue(perspective);
            //collision.Parameters["NormalMap"].SetValue(normalMap);

           // GraphicsDevice.SetRenderTarget(collisionMap);
            //GraphicsDevice.Clear(Color.Transparent);
            GraphicsDevice.SetRenderTarget(collisionMap);
            //GraphicsDevice.Clear(Color.Transparent);
            //foreach (FootDecal f in steps)
            //collision.Parameters["NormalMap"].SetValue(normalMap);
            //collision.Parameters["size"].SetValue(0.25f);
            //collision.Parameters["World"].SetValue(Matrix.Identity);
            //collision.Parameters["SecondWorld"].SetValue(steps[0].World * Matrix.Invert(steps[1].World));
            //collision.Parameters["SecondWorldInverse"].SetValue(Matrix.Invert(steps[0].World * Matrix.Invert(steps[1].World)));
            //collision.Parameters["TopLeft"].SetValue(steps[1].Vertices[0].TextureCoordinate);
            //collision.Parameters["TopRight"].SetValue(steps[1].Vertices[1].TextureCoordinate);
            //collision.Parameters["BottomLeft"].SetValue(steps[1].Vertices[2].TextureCoordinate);
            //collision.Parameters["Length"].SetValue(steps[1].Length_X);
            //collision.Parameters["SecondMap"].SetValue(normalMap);
            //foreach (EffectPass pass in collision.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
            //         PrimitiveType.TriangleList, steps[0].Vertices, 0, steps[0].Vertices.Length, steps[0].ReverseIndices, 0, steps[0].Indices.Length / 3);
            //}
            //GraphicsDevice.SetRenderTarget(null);
            //foreach (FootDecal f in steps)
            //{
            //collision.Parameters["World"].SetValue(f.World * Matrix.Invert(steps[0].World));
            //Matrix m = footprint.World * Matrix.CreateTranslation(0,0,-0.3f);
            //    collision.Parameters["World"].SetValue(footprint.World * Matrix.Invert(footprint.World));
            //collision.Parameters["SecondWorld"].SetValue(footprint.World * Matrix.Invert(m));
            //collision.Parameters["SecondWorldInverse"].SetValue(footprint.World * Matrix.Invert(m));
            ////collision.Parameters["SecondWorld"].SetValue(footprint.World * Matrix.Invert(overlap.World));
            ////collision.Parameters["SecondWorldInverse"].SetValue(Matrix.Invert(footprint.World * Matrix.Invert(overlap.World)));
            //collision.Parameters["TopLeft"].SetValue(overlap.Vertices[0].TextureCoordinate);
            //    collision.Parameters["TopRight"].SetValue(overlap.Vertices[1].TextureCoordinate);
            //    collision.Parameters["BottomLeft"].SetValue(overlap.Vertices[2].TextureCoordinate);
            //    collision.Parameters["Length"].SetValue(overlap.Length_X);
            //    collision.Parameters["Toggle"].SetValue(toggle);
            //    collision.Parameters["SecondMap"].SetValue(hole);
            //  // collision.Parameters["SecondMap"].SetValue(normalMap);
            //foreach (EffectPass pass in collision.CurrentTechnique.Passes)
            //    //foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
            //             PrimitiveType.TriangleList, footprint.Vertices, 0, footprint.Vertices.Length, footprint.ReverseIndices, 0, footprint.Indices.Length / 3);
            //    }

            runningEffect.Parameters["World"].SetValue(Matrix.Identity);
            
            //runningEffect.Parameters["SecondMap"].SetValue(hole);
            runningEffect.Parameters["Extract"].SetValue(extract);
            runningEffect.Parameters["Running"].SetValue(toggle1);

            foreach(FootDecal f in steps)
            {
                if(f.RightFoot)
                    runningEffect.Parameters["NormalMap"].SetValue(normalMap);
                else
                    runningEffect.Parameters["NormalMap"].SetValue(leftFoot);

                images.Add(f.prepareTexture(this));
            }
            

            GraphicsDevice.SetRenderTarget(null);
            //GraphicsDevice.Clear(Color.Transparent);
            //FileStream file = new FileStream("SlumpParallax.png", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //collisionMap.SaveAsPng(file, 512, 512);
            //file.Close();
            //parallaxEffect.Parameters["NormalTex"].SetValue(normalMap);
            beachMaterial.Parameters["World"].SetValue(world);
            beachMaterial.Parameters["View"].SetValue(view);
            beachMaterial.Parameters["Projection"].SetValue(perspective);
            beachMaterial.Parameters["LightDirection"].SetValue(lightdir.Direction);

            foreach (EffectPass pass in beachMaterial.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                     PrimitiveType.TriangleList, beach.Vertices, 0, beach.Vertices.Length, beach.Indices, 0, beach.Indices.Length/3);
            }
            
            parallaxEffect.Parameters["View"].SetValue(view);
            parallaxEffect.Parameters["Projection"].SetValue(perspective);
            parallaxEffect.Parameters["DiffuseTex"].SetValue(sand);
            parallaxEffect.Parameters["LightDirection"].SetValue(lightdir.Direction);
            parallaxEffect.Parameters["CameraPosition"].SetValue(character.Position);
            parallaxEffect.Parameters["Toggle"].SetValue(toggle);
            
            parallaxEffect.Parameters["NormalTex"].SetValue(normalMap);

            DepthStencilState ds_save = GraphicsDevice.DepthStencilState;
            DepthStencilState ds = new DepthStencilState();
            ds.DepthBufferEnable = false;
            ds.StencilEnable = true;
            ds.ReferenceStencil = 0;
            ds.StencilFunction = CompareFunction.Always;
            ds.StencilPass = StencilOperation.Increment;
            GraphicsDevice.DepthStencilState = ds;

            //foreach (FootDecal f in steps)
            //{
            //collision.Parameters["World"].SetValue(f.World);
            //parallaxEffect.Parameters["World"].SetValue(overlap.World);
            //normalMap.SetData<Color>(f.Pixels);

            //foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
            ////foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
            //            PrimitiveType.TriangleList, overlap.Vertices, 0, overlap.Vertices.Length, overlap.Indices, 0, overlap.Indices.Length / 3);
            //}
            BlendState bs_save = GraphicsDevice.BlendState;
            BlendState bs = new BlendState();
            bs.ColorDestinationBlend = Blend.InverseSourceAlpha;
            bs.ColorSourceBlend = Blend.SourceAlpha;
            bs.AlphaSourceBlend = Blend.SourceAlpha;
            bs.AlphaDestinationBlend = Blend.InverseSourceAlpha;
            GraphicsDevice.BlendState = bs;
            for (int i = 0; i < steps.Count; i++)
            {
                Vector2 ratio = new Vector2(steps[i].Length_X / beach.Length_X, -steps[i].Length_Z / beach.Length_Z);
                parallaxEffect.Parameters["Ratio"].SetValue(ratio);
                parallaxEffect.Parameters["World"].SetValue(steps[i].World);
                parallaxEffect.Parameters["Weight"].SetValue(steps[i].Weight);
                //if(toggle == )
                //parallaxEffect.Parameters["NormalTex"].SetValue(normalMap);
                //else
                parallaxEffect.Parameters["NormalTex"].SetValue(images[i]);
                //parallaxEffect.Parameters["NormalTex"].SetValue((Texture2D)collisionMap);
                foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
                         PrimitiveType.TriangleList, steps[i].Vertices, 0, steps[i].Vertices.Length, steps[i].Indices, 0, steps[i].Indices.Length / 3);
                }
               // break;
            }
            GraphicsDevice.BlendState = bs_save;
            //parallaxEffect.Parameters["NormalTex"].SetValue(normalMap);
            GraphicsDevice.DepthStencilState = ds_save;

            lightdir.Draw(graphics, view, perspective);
           // draw_SandParticles(view, perspective);
            sun.Draw(Matrix.CreateScale(0.02f) * Matrix.CreateTranslation(lightdir.Direction * new Vector3(30, -30, -30)), view, perspective);
            base.Draw(gameTime);
           
        }
    }
}
