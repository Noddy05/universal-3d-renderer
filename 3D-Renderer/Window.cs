using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using _3D_Renderer._Shading;
using _3D_Renderer._Renderable;
using _3D_Renderer._Import;
using _3D_Renderer._Camera;
using _3D_Renderer._Rendering;
using _3D_Renderer._Rendering._Renderers;
using _3D_Renderer._Renderable._GameObject;
using _3D_Renderer._Shading._Materials;
using _3D_Renderer._Statistics;
using _3D_Renderer._Renderable._UIElement;
using _3D_Renderer._Generation;
using _3D_Renderer._Renderable._Cubemap;
using _3D_Renderer._Behaviour;
using _3D_Renderer._BufferObjects;

namespace _3D_Renderer
{
    /// <summary>
    /// The window will contain all the visual information the user will receive
    /// </summary>
    internal class Window : GameWindow
    {
        private bool orthographicView = false;

        /// <summary>
        /// Instantiates a new window. Default resolution: 1920, 1080
        /// </summary>
        public Window()
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1920, 1080),
                StartVisible = false,
                Title = "Game Window"
            })
        {

        }
        public RenderStats renderStats;

        public int defaultTextureHandle = -1;
        private Camera camera;
        private Matrix4 perspectiveMatrix;

        //For rendering
        private Collection background = new Collection();
        private Collection scene = new Collection();
        private Collection canvas = new Collection();
        private Renderer defaultRenderer;
        private UIRenderer uiRenderer;
        private int cmuSerifHandle;
        private FBO sceneFBO;
        private int fboOutputTexture;

        /// <summary>
        /// This is called whenever <see cref="Window"/>.Run() is called.<br></br>
        /// It first sets the clear color (Default background color).<br></br>
        /// Then it enables backface culling.<br></br>
        /// Generates perspective matrix and camera object (Also locks mouse to allow
        /// for camera rotation with the mouse).<br></br>
        /// It then loads the default texture and instantiates renderers.<br></br>
        /// Populates scene and instantiates new <see cref="RenderStats"/> object.<br></br>
        /// Finally it makes the window visible to user.
        /// </summary>
        protected override void OnLoad()
        {
            //Clear color and backface culling:
            GL.ClearColor(Color.PeachPuff);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.ClipDistance0);

            //Create perspectivematrix, camera and locks mouse:
            GeneratePerspectiveMatrix();
            camera = new FreeCamera(1 / 800f, 5f);
            CursorState = CursorState.Grabbed;

            //Loads default texture and renderers:
            defaultTextureHandle = TextureLoader.LoadTexture(@"../../../_Assets/_Debug/WhitePixel.png");
            defaultRenderer = new DefaultRenderer();
            uiRenderer = new UIRenderer();

            cmuSerifHandle = FontLoader.LoadFont(
                @"../../../_Assets/_Built-In/_Fonts/_CMU Serif/cmuserif.png",
                @"../../../_Assets/_Built-In/_Fonts/_CMU Serif/cmuserif.fnt");

            //Populate scene and instantiate RenderStats:
            PopulateScene();
            renderStats = new RenderStats();


            base.OnLoad();
            //Loading has finished, show application:
            IsVisible = true;
        }

        /// <summary>
        /// This fills the <see cref="Collection"/> "scene" with randomly positioned and scaled 
        /// monkey heads facing the user.<br></br>
        /// Then generates the cubemap that will function as the skybox and adds it to the "background" 
        /// <see cref="Collection"/>.<br></br>
        /// Finally it adds a textured UI-element to the top-right corner which will be added to the
        /// "canvas" <see cref="Collection"/>.
        /// </summary>
        private void PopulateScene()
        {
            sceneFBO = new FBO(new Vector2i((int)(Size.X * 0.3f), (int)(Size.Y * 0.3f)));
            sceneFBO.CreateTextureAttachment();
            sceneFBO.CreateDepthTextureAttachment();
            sceneFBO.UnbindFramebuffer();

            #region Skybox
            //Skybox:
            Cubemap cubemap = new Cubemap();
            int cubemapTextureHandle = TextureLoader.LoadCubemap([
                @"../../../_Assets/_Built-In/_Skybox/right.png",
                @"../../../_Assets/_Built-In/_Skybox/left.png",
                @"../../../_Assets/_Built-In/_Skybox/top.png",
                @"../../../_Assets/_Built-In/_Skybox/bottom.png",
                @"../../../_Assets/_Built-In/_Skybox/front.png",
                @"../../../_Assets/_Built-In/_Skybox/back.png",
            ]);
            Material skybox = new CubemapMaterial(cubemapTextureHandle);
            cubemap.SetMaterial(skybox);
            background.renderables.Add(cubemap);
            #endregion

            #region Monkey Heads
            //Populating scene:
            int textureHandle = TextureLoader.LoadTexture(@"../../../_Assets/_Debug/color-test.png");
            Mesh suzanneMesh = MeshLoader.Load(@"../../../_Assets/_Debug/suzanne_normals.obj");

            GameObject gameObject = new GameObject();
            Material material = new Diffuse(textureHandle, cubemapTextureHandle);
            gameObject.SetMaterial(material);
            gameObject.mesh = suzanneMesh;
            gameObject.name = "First GameObject!";
            gameObject.transform.position = new Vector3(0, 0, -5f);
            gameObject.transform.scale = Vector3.One * 0.5f;

            //Random heads:
            Random rand = new Random();
            for (int i = 0; i < 5000; i++)
            {
                gameObject.transform.scale = Vector3.One * (0.05f +
                    MathF.Pow((float)rand.NextDouble(), 1.4f) * 1.95f);

                float theta = (float)rand.NextDouble() * 2 * MathF.PI;
                float height = (float)rand.NextDouble() * 2 - 1f;
                gameObject.transform.position = new Vector3(MathF.Cos(theta),
                    height, MathF.Sin(theta)) * (25 + (float)rand.NextDouble() * 5);
                gameObject.transform.rotation = new Vector3(0, -theta - MathF.PI / 2, 0);

                scene.renderables.Add(gameObject.Clone());
            }
            #endregion

            #region UI
            //UI:
            Material uiMaterial = new UIMaterial(Color4.White, sceneFBO.GetTextureHandle());

            UIElement uiElement = new UIElement();
            uiElement.SetMaterial(uiMaterial);
            float scale = 0.3f;
            uiElement.transform.scale *= scale;
            uiElement.transform.position += Vector2.One * (1-scale);
            //canvas.renderables.Add(uiElement);
            #endregion

            #region Text
            //Text
            Material cmuFontMaterial = new UIText(Color4.White,
                FontLoader.GetFont(cmuSerifHandle).textureAtlasHandle);
            UIElement textObject = new UIElement();
            textObject.SetMaterial(cmuFontMaterial);
            textObject.mesh = MeshGeneration.Text(cmuSerifHandle, "Hello World!", out float textWidth);
            textObject.transform.scale /= textWidth / 2;
            //canvas.renderables.Add(textObject);
            #endregion
        }

        /// <summary>
        /// This should be called everytime the window is resized, this updates the perspective matrix
        /// to fit with the new aspect ratio.
        /// </summary>
        private void GeneratePerspectiveMatrix()
        {
            if (!orthographicView)
            {
                perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(70f / 180f * MathF.PI,
                    (float)Program.window!.Size.X / Program.window!.Size.Y, 0.1f, 1000f);
            }
            else
            {
                perspectiveMatrix = Matrix4.CreateOrthographic(
                    4f, 4f * Program.window!.Size.Y / Program.window!.Size.X, 0.1f, 1000f);
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            GeneratePerspectiveMatrix();
            base.OnResize(e);
        }

        /// <summary>
        /// This is called when the window unloads. This is where all the post-program cleanup should 
        /// take place. It automatically disposes of all <see cref="EasyUnload"/> objects.
        /// </summary>
        protected override void OnUnload()
        {
            //Cleanup:
            //Dispose shaders, VAO, IBO

            //Dispose of all EasyUnload objects:
            EasyUnload[] freezeFrame = EasyUnload.GetInstancedObjects().ToArray();
            foreach (EasyUnload objectToUnload in freezeFrame)
            {
                if (objectToUnload.DisposeObject())
                    Console.WriteLine($"Unloaded object {objectToUnload.GetType()} : EasyUnload");
            }

            base.OnUnload();
        }

        public double timeSinceStartup = 0;
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            timeSinceStartup += args.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Render scene:
            GL.Disable(EnableCap.DepthTest);
            defaultRenderer.RenderCollection(background, perspectiveMatrix, camera.RotationMatrix());

            GL.Enable(EnableCap.DepthTest);
            //sceneFBO.BindFramebuffer();
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            defaultRenderer.RenderCollection(scene, perspectiveMatrix, camera.CameraMatrix());
            //sceneFBO.UnbindFramebuffer();

            //Render UI canvas:
            GL.Disable(EnableCap.DepthTest);
            uiRenderer.RenderCollection(canvas, perspectiveMatrix, Matrix4.Identity);

            SwapBuffers();

            base.OnRenderFrame(args);
        }
    }
}
