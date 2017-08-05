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
    public class Footprint_Game : Microsoft.Xna.Framework.Game
    {
        LightDirectionDisplay lightdir;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //VertexPositionNormalTexture[] beach;
        //int[] indicesBeach;
        public Effect beachMaterial;
        public Effect parallaxEffect;
        public Effect runningEffect;
        public Texture2D normalMap;
        public Texture2D hole;
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
        Effect collision;
        Model sun;
        Model particle;
        Model chunk1;
        Model chunk2;
        Random rand = new Random();
        List<Sand_Particle> chunks = new List<Sand_Particle>();
        float time = 0;


        public Footprint_Game()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.ToggleFullScreen();
            Content.RootDirectory = "Content";
            character = new Player();
            
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
            collisionMap = new RenderTarget2D(GraphicsDevice, 512, 512, true, SurfaceFormat.Color, DepthFormat.None);
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            beachMaterial = Content.Load<Effect>("BasicLighting");
            //beachMaterial = new BasicEffect(GraphicsDevice);
            //beachMaterial.TextureEnabled = true;
            //http://linolafett.deviantart.com/art/Sand-01-434854377
            sand = Content.Load<Texture2D>("Sand");
            beachMaterial.Parameters["DiffuseTex"].SetValue(sand);

            hole = Content.Load<Texture2D>("lumpParallax3");
            parallaxEffect = Content.Load<Effect>("Parallax");
            collision = Content.Load<Effect>("DualOverlap");
            normalMap = Content.Load<Texture2D>("FootPerfect");
            runningEffect = Content.Load<Effect>("RunningEffect");
            extract = Content.Load<Texture2D>("ExtractParallax");
            chunk1 = Content.Load<Model>("chunk1");
            chunk2 = Content.Load<Model>("chunk2");

            footprint = new FootDecal(new Vector3(10f, 0, 12f), beach, 3.14f, normalMap, this);
            steps.Add(footprint);
            overlap = new FootDecal(new Vector3(9f, 0, 10f),  beach, 3.14f/4f,normalMap, this);
            steps.Add(overlap);

            
            foreach (ModelMesh mesh in chunk1.Meshes)
            {
                foreach(ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = beachMaterial;
                }
                //foreach (BasicEffect effect in mesh.Effects)
                //{
                //    effect.EnableDefaultLighting();
                //    effect.DirectionalLight0.Direction = lightdir.Direction;
                //}
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
            //if (2 == gameTime.TotalGameTime.Seconds)
            //{

            //    FootDecal f = new FootDecal(character.Position, beach, character.Rotation, normalMap);
            //    steps.Add(f);
            //    //FootDecal footprint = new FootDecal(character.Position, beach, normalMap);
            //    //steps.Add(footprint);
            //}

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) overlap.Z += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) overlap.Z -= 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) overlap.X += 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) overlap.X -= 0.05f;
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) footprint.Rotation += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.E)) footprint.Rotation -= 0.02f;
            footstep = gameTime.TotalGameTime.Seconds;

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            world = Matrix.CreateTranslation(new Vector3(0,0,0));
            //world = world * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds);
            
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

            collision.Parameters["View"].SetValue(view);
            collision.Parameters["Projection"].SetValue(perspective);
            collision.Parameters["NormalMap"].SetValue(normalMap);

            GraphicsDevice.SetRenderTarget(collisionMap);
            GraphicsDevice.Clear(Color.Transparent);
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
            runningEffect.Parameters["NormalMap"].SetValue(normalMap);
            runningEffect.Parameters["SecondMap"].SetValue(hole);
            runningEffect.Parameters["Extract"].SetValue(extract);
            runningEffect.Parameters["Running"].SetValue(toggle1);
            images.Add(footprint.prepareTexture(this));

            GraphicsDevice.SetRenderTarget(null);
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
            Vector2 ratio = new Vector2(footprint.Length_X / beach.Length_X, -footprint.Length_Z / beach.Length_Z);
            parallaxEffect.Parameters["View"].SetValue(view);
            parallaxEffect.Parameters["Projection"].SetValue(perspective);
            parallaxEffect.Parameters["DiffuseTex"].SetValue(sand);
            parallaxEffect.Parameters["LightDirection"].SetValue(lightdir.Direction);
            parallaxEffect.Parameters["CameraPosition"].SetValue(character.Position);
            parallaxEffect.Parameters["Toggle"].SetValue(toggle);
            parallaxEffect.Parameters["Ratio"].SetValue(ratio);
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
            parallaxEffect.Parameters["World"].SetValue(overlap.World);
            //normalMap.SetData<Color>(f.Pixels);

            //foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
            ////foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
            //            PrimitiveType.TriangleList, overlap.Vertices, 0, overlap.Vertices.Length, overlap.Indices, 0, overlap.Indices.Length / 3);
            //}


            parallaxEffect.Parameters["World"].SetValue(footprint.World);
            //normalMap.SetData<Color>(f.Pixels);
            //parallaxEffect.Parameters["NormalTex"].SetValue(normalMap);
            parallaxEffect.Parameters["NormalTex"].SetValue(images[0]);
            //parallaxEffect.Parameters["NormalTex"].SetValue((Texture2D)collisionMap);
            foreach (EffectPass pass in parallaxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexTangentSpace>(
                     PrimitiveType.TriangleList, footprint.Vertices, 0, footprint.Vertices.Length, footprint.Indices, 0, footprint.Indices.Length / 3);
            }
            //parallaxEffect.Parameters["NormalTex"].SetValue(normalMap);
            GraphicsDevice.DepthStencilState = ds_save;

            lightdir.Draw(graphics, view, perspective);
           // draw_SandParticles(view, perspective);
            sun.Draw(Matrix.CreateScale(0.02f) * Matrix.CreateTranslation(lightdir.Direction * new Vector3(30, -30, -30)), view, perspective);
            base.Draw(gameTime);

            //foreach(ModelMesh mesh in chunk1.Meshes)
            //{
            //    foreach(ModelMeshPart part in mesh.MeshParts)
            //    {
            //        part.Effect = beachMaterial;
            //        beachMaterial.Parameters["World"].SetValue(Matrix.CreateScale(0.2f) * Matrix.CreateRotationX(-(float)Math.PI / 2) * Matrix.CreateRotationY(-(float)Math.PI / 1.2f) * footprint.World * Matrix.CreateTranslation(new Vector3(1f, 0f, 1f)));
            //        beachMaterial.Parameters["View"].SetValue(view);
            //        beachMaterial.Parameters["Projection"].SetValue(perspective);
            //        beachMaterial.Parameters["LightDirection"].SetValue(lightdir.Direction);
            //    }
            //    //chunk1.Draw(footprint.World * Matrix.CreateTranslation(new Vector3(1f, 0, 1f)), view, perspective);
            //    mesh.Draw();
            //}
           
        }
    }
}
