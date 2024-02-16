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
using System.Security.Cryptography;

namespace _3D_Renderer
{
    internal class Window : GameWindow
    {
        private bool orthographicView = false;

        public Window()
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1920, 1080),
                StartVisible = false,
                Title = "Game Window"
            })
        {

        }

        public int defaultTextureHandle = -1;
        public int textureHandle = -1;
        private Shader shader;
        private Mesh suzanneMesh;
        private Camera camera;
        private Matrix4 perspectiveMatrix;

        private Collection scene = new Collection();
        private Collection canvas = new Collection();
        private Renderer defaultRenderer;
        private UIRenderer uiRenderer;
        public RenderStats renderStats;
        protected override void OnLoad()
        {
            GL.ClearColor(Color.PeachPuff);
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);

            GeneratePerspectiveMatrix();

            suzanneMesh = MeshLoader.Load(@"../../../_Assets/_Debug/suzanne_normals.obj");
            shader = new Shader(@"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_vertex_shader.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_fragment_shader.frag");

            textureHandle = TextureLoader.LoadTexture(@"../../../_Assets/_Debug/color-test.png");
            defaultTextureHandle = TextureLoader.LoadTexture(@"../../../_Assets/_Debug/WhitePixel.png");

            camera = new FreeCamera(1 / 800f, 5f);
            CursorState = CursorState.Grabbed;

            PopulateScene();

            defaultRenderer = new DefaultRenderer();
            uiRenderer = new UIRenderer();
            renderStats = new RenderStats();

            base.OnLoad();
            //Loading has finished, show application:
            IsVisible = true;
        }

        private void PopulateScene()
        {
            //Populating scene:
            GameObject gameObject = new GameObject();
            Material material = new Diffuse(shader, textureHandle);
            gameObject.SetMaterial(material);
            gameObject.mesh = suzanneMesh;
            gameObject.name = "First GameObject!";
            gameObject.transform.position = new Vector3(0, 0, -5f);
            gameObject.transform.size = Vector3.One * 0.5f;

            //Random heads:
            Random rand = new Random();
            for (int i = 0; i < 500; i++)
            {
                gameObject.transform.size = Vector3.One * (0.05f +
                    MathF.Pow((float)rand.NextDouble(), 1.4f) * 1.95f);

                float theta = (float)rand.NextDouble() * 2 * MathF.PI;
                float height = (float)rand.NextDouble() * 2 - 1f;
                gameObject.transform.position = new Vector3(MathF.Cos(theta),
                    height, MathF.Sin(theta)) * (20 + (float)rand.NextDouble());
                gameObject.transform.rotation = new Vector3(0, -theta - MathF.PI / 2, 0);

                scene.renderables.Add(gameObject.Clone());
            }

            //UI:
            Shader uiShader = new Shader(
                @"../../../_Assets/_Built-In/_Shaders/_UI/ui_element.vert",
                @"../../../_Assets/_Built-In/_Shaders/_UI/ui_element.frag"
                );
            Material uiMaterial = new UIMaterial(uiShader, Color4.White, textureHandle);

            UIElement uiElement = new UIElement();
            uiElement.SetMaterial(uiMaterial);
            float scale = 0.3f;
            uiElement.transform.scale *= scale;
            uiElement.transform.position += Vector2.One * (1-scale);
            canvas.renderables.Add(uiElement);
        }

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

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        private double timeSinceStartup = 0;
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            timeSinceStartup += args.Time;

            //Render scene:
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            defaultRenderer.RenderCollection(scene, perspectiveMatrix, camera.CameraMatrix());
            uiRenderer.RenderCollection(canvas, perspectiveMatrix, camera.CameraMatrix());
            SwapBuffers();

            base.OnRenderFrame(args);
        }
    }
}
