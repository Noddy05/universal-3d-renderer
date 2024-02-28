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
using _3D_Renderer._GLObjects;

namespace _3D_Renderer
{
    /* To clean up:
     * InstanceMethods are temporary classes, should be rewritten
     * UBO should be rewritten to not lose generality
     * Cleanup in how vao modifications are done 
    */

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
            CenterWindow();
        }

        public RenderStats renderStats;
        private int defaultTextureHandle = -1;
        public int GetDefaultTextureHandle() => defaultTextureHandle;
        private Camera camera;
        private Material defaultMaterial;
        public Material GetDefaultMaterial() => defaultMaterial;
        //Renderers
        private Renderer defaultRenderer;
        private UIRenderer uiRenderer;
        private InstancedRenderer instancedRenderer;

        //Temp:
        private FBO sceneFBO;
        private int fboOutputTexture;
        private int cmuSerifHandle;
        private Collection background = new Collection();
        private Collection scene = new Collection();
        private Collection canvas = new Collection();
        private GameObject instanceable;
        private Matrix4[] matrices;

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
            base.OnLoad();

            //Clear color and backface culling:
            GL.ClearColor(Color.PeachPuff);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.ClipDistance0);

            defaultMaterial = new UnlitMaterial(Color.Purple);

            //Create perspectivematrix, camera and locks mouse:
            camera = new FreeCamera(70 * MathF.PI / 180f, 0.1f, 1000f, 1 / 800f, 5f);

            //Loads default texture and renderers:
            defaultTextureHandle = TextureLoader.LoadTexture(@"../../../_Assets/_Debug/WhitePixel.png");
            defaultRenderer = new DefaultRenderer();
            uiRenderer = new UIRenderer();
            instancedRenderer = new InstancedRenderer();

            cmuSerifHandle = FontLoader.LoadFont(
                @"../../../_Assets/_Built-In/_Fonts/_CMU Serif/cmuserif.png",
                @"../../../_Assets/_Built-In/_Fonts/_CMU Serif/cmuserif.fnt");

            //Populate scene and instantiate RenderStats:
            PopulateScene();
            renderStats = new RenderStats();

            Random rand = new Random();
            matrices = new Matrix4[1000];
            for (int i = 0; i < matrices.Length; i++)
            {
                Transform instanceTransform = new Transform();

                instanceTransform.scale = Vector3.One * (0.05f +
                    MathF.Pow((float)rand.NextDouble(), 1.4f) * 1.95f);

                float theta = (float)rand.NextDouble() * 2 * MathF.PI;
                float height = (float)rand.NextDouble() * 2 - 1f;
                instanceTransform.position = new Vector3(MathF.Cos(theta),
                    height, MathF.Sin(theta)) * (25 + (float)rand.NextDouble() * 5);
                instanceTransform.rotation = new Vector3(0, -theta - MathF.PI / 2, 0);
                matrices[i] = instanceTransform.TransformationMatrix();
            }

            //Loading has finished, show application:
            IsVisible = true;
            CursorState = CursorState.Grabbed;
        }

        /// <summary>
        /// This fills the <see cref="Collection"/> "scene" with randomly positioned and scaled 
        /// monkey heads facing the user.<br></br>
        /// Then generates the cubemap that will function as the skybox and adds it to the "background" 
        /// <see cref="Collection"/>.<br></br>
        /// Finally it adds a textured UI-element to the top-right corner which will be added to the
        /// "canvas" <see cref="Collection"/>.
        /// </summary>

        private GameObject temp;
        private GameObject temp2;
        private int instanceCount = 5000;
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
            cubemap.cull = false;
            background.renderables.Add(cubemap);
            #endregion

            #region GameObjects
            //Populating scene:
            int textureHandle = TextureLoader.LoadTexture(@"../../../_Assets/_Debug/color-test.png");
            Mesh suzanneMesh = MeshLoader.Load(@"../../../_Assets/_Debug/suzanne.obj");
            Mesh secondMesh = MeshLoader.Load(@"../../../_Assets/_Debug/suzanne.obj");

            GameObject gameObject = new GameObject();
            Material material = new Diffuse(textureHandle, Color4.White, cubemapTextureHandle);
            gameObject.SetMaterial(material);
            gameObject.SetMesh(suzanneMesh);
            gameObject.name = "First GameObject!";
            gameObject.transform.position = new Vector3(0, 0, -5f);
            gameObject.transform.scale = Vector3.One;

            temp = gameObject.Clone();
            temp.showBoundingBox = true;
            //scene.renderables.Add(temp);
            gameObject.showBoundingBox = false;

            temp2 = temp.Clone();
            temp2.showBoundingBox = true;
            temp2.transform.position = new Vector3(0, 0, 5f);
            scene.renderables.Add(temp2);

            //Random heads:
            Random rand = new Random();
            for (int i = 0; i < 500; i++)
            {
                gameObject.transform.scale = Vector3.One * (0.05f +
                    MathF.Pow((float)rand.NextDouble(), 1.4f) * 1.95f);

                float theta = (float)rand.NextDouble() * 2 * MathF.PI;
                float height = (float)rand.NextDouble() * 2 - 1f;
                gameObject.transform.position = new Vector3(MathF.Cos(theta),
                    height, MathF.Sin(theta)) * (25 + (float)rand.NextDouble() * 5);
                gameObject.transform.rotation = new Vector3(0, -theta - MathF.PI / 2, 0);

                //scene.renderables.Add(gameObject.Clone());
            }

            instanceable = gameObject.Clone();
            instanceable.SetMesh(secondMesh);
            instanceable.transform.position = Vector3.Zero;
            instanceable.transform.rotation = Vector3.Zero;
            instanceable.transform.scale = Vector3.One;
            InstanceMethod2 particleMaterial = new InstanceMethod2(Color4.White);
            instanceable.SetMaterial(particleMaterial);
            Matrix4[] matrices = new Matrix4[instanceCount];
            for(int i = 0; i < instanceCount; i++)
            {
                Transform instanceTransform = new Transform();
                instanceTransform.scale = Vector3.One * (0.05f +
                    MathF.Pow((float)rand.NextDouble(), 1.4f) * 1.95f);

                float theta = (float)rand.NextDouble() * 2 * MathF.PI;
                float height = (float)rand.NextDouble() * 2 - 1f;
                instanceTransform.position = new Vector3(MathF.Cos(theta),
                    height, MathF.Sin(theta)) * (25 + (float)rand.NextDouble() * 5);
                instanceTransform.rotation = new Vector3(0, -theta - MathF.PI / 2, 0);
                matrices[i] = instanceTransform.TransformationMatrix();
            }
            instanceable.GetMesh()!.CreateInstanceVBO(matrices);

            #endregion

            #region UI
            //UI:
            Material uiMaterial = new UIMaterial(Color4.White, sceneFBO.GetTextureHandle());

            UIElement uiElement = new UIElement();
            uiElement.SetMaterial(uiMaterial);
            float scale = 0.3f;
            uiElement.transform.scale *= scale;
            uiElement.transform.position += Vector3.One * (1 - scale);
            //canvas.renderables.Add(uiElement);
            #endregion

            #region Text
            //Text
            Material cmuFontMaterial = new UIText(Color4.White,
                FontLoader.GetFont(cmuSerifHandle).textureAtlasHandle);
            UIElement textObject = new UIElement();
            textObject.SetMaterial(cmuFontMaterial);
            textObject.SetMesh(MeshGeneration.Text(cmuSerifHandle, "Hello World!", out float textWidth));
            textObject.transform.scale /= textWidth / 2;
            //canvas.renderables.Add(textObject);
            #endregion
        }


        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            camera.GenerateProjectionMatrix();
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
                objectToUnload.Dispose();
                Console.WriteLine($"Unloaded object {objectToUnload.GetType()} : EasyUnload");
            }

            //Dispose UBO:
            UBO.Dispose();

            base.OnUnload();
        }

        public double timeSinceStartup = 0;
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            timeSinceStartup += args.Time;
            temp.transform.rotation.Y = (float)timeSinceStartup;
            temp.transform.rotation.X = (float)timeSinceStartup * 0.4f;
            temp.transform.rotation.Z = (float)timeSinceStartup * 3f;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Render scene:


            //Background (skybox):
            GL.Disable(EnableCap.DepthTest);
            defaultRenderer.RenderCollection(background, camera,
                camera.GetProjectionMatrix(), camera.RotationMatrix());

            GL.Enable(EnableCap.DepthTest);
            //sceneFBO.BindFramebuffer();
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Transform transform = new Transform();
            transform.position = new Vector3(0, 2 * MathF.Sin((float)timeSinceStartup / 5f), 0);
            transform.scale = Vector3.One;

            //This takes a long time:
            //400 frames without this line, 120 without calculating bounds, 100 when calculating bounds
            //temp2.GetMesh()!.ApplyTransformation(transform.TransformationMatrix(), true);

            defaultRenderer.RenderCollection(scene, camera,
                camera.GetProjectionMatrix(), camera.CameraMatrix());
            //sceneFBO.UnbindFramebuffer();

            //Instanced:

            /*
            Matrix4[] newMatrices = new Matrix4[matrices.Length];
            for (int i = 0; i < matrices.Length; i++)
            {
                newMatrices[i] = matrices[i]
                    * Matrix4.CreateTranslation(0, MathF.Sin((float)timeSinceStartup) * i, 0);
            }
            UBO.Update(newMatrices);
            */
            instancedRenderer.RenderInstancedObject(instanceable, instanceCount, 
                camera.GetProjectionMatrix(), camera.CameraMatrix());

            //Render UI canvas:
            GL.Disable(EnableCap.DepthTest);
            uiRenderer.RenderCollection(canvas, camera,
                Matrix4.Identity, camera.CameraMatrix());

            SwapBuffers();

            base.OnRenderFrame(args);
        }
    }
}
