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
using _3D_Renderer._GLObjects._UBO;
using Font = _3D_Renderer._Import.Font;

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
                Title = "Game Window",
                NumberOfSamples = 1,
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

        //Temp:
        private FBO sceneFBO;
        private int fboOutputTexture;
        private int cmuSerifHandle;
        private int timesNewRomanHandle;
        private Collection background = new Collection();
        private Collection scene = new Collection();
        private Collection canvas = new Collection();
        private GameObject instanceable;
        private int brickTextureHandle = -1;
        private int brickNormalHandle = -1;

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
            GL.Enable(EnableCap.Multisample);

            defaultMaterial = new UnlitMaterial(Color.Purple);

            //Create perspectivematrix, camera and locks mouse:
            camera = new FreeCamera(70 * MathF.PI / 180f, 0.1f, 1000f, 1 / 800f, 5f);

            //Loads default texture and renderers:
            defaultTextureHandle = TextureLoader.LoadTexture(@"../../../_Assets/"
                + @"_Built-In/_Textures/_Default/WhitePixel.png");
            defaultRenderer = new DefaultRenderer();
            uiRenderer = new UIRenderer();

            cmuSerifHandle = FontLoader.LoadFont(
                @"../../../_Assets/_Built-In/_Fonts/_CMU Serif/cmuserif.fnt",
                @"../../../_Assets/_Built-In/_Fonts/_CMU Serif/cmuserif.png");
            timesNewRomanHandle = FontLoader.LoadFont(
                @"../../../_Assets/_Built-In/_Fonts/_CMU Serif/cmuserif.fnt",
                @"../../../_Assets/_Built-In/_Fonts/_CMU Serif/cmuserif.png");

            brickTextureHandle = TextureLoader.LoadTexture(
                @"../../../_Assets/_Debug/_Textures/_Brickwall/brickTexture.png");
            brickNormalHandle = TextureLoader.LoadTexture(
                @"../../../_Assets/_Debug/_Textures/_Brickwall/brickNormals.png");
            //Populate scene and instantiate RenderStats:
            PopulateScene();
            renderStats = new RenderStats();

            //Loading has finished, show application:
            IsVisible = true;
            CursorState = CursorState.Grabbed;

            UBO.shadowData.shadowColor = new Color4(19, 22, 46, 255);
            UBO.shadowData.minLightStrength = 0.1f;
            UBO.directionalLightData.SetLightColor(0, Color4.White);
            UBO.directionalLightData.SetLightStrength(0, 1);
            UBO.directionalLightData.SetLightCastFromDirection(0, new Vector3(1, 1, 1).Normalized());
            UBO.UpdateUBO();
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
        private GameObject candle;
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
            int textureHandle = TextureLoader.LoadTexture(
                @"../../../_Assets/_Debug/_Textures/color-test.png");
            Mesh suzanneMesh = MeshLoader.Load(@"../../../_Assets/_Debug/_Models/suzanne_normals.obj");
            Mesh secondMesh = MeshGeneration.CubeSphere(12);

            GameObject gameObject = new GameObject();
            DiffuseMaterial material = new DiffuseMaterial(Color4.White, defaultTextureHandle, 
                brickNormalHandle, cubemapTextureHandle);
            gameObject.SetMaterial(material);
            gameObject.SetMesh(suzanneMesh);
            material.useNormalMap = true;
            gameObject.name = "First GameObject!";
            gameObject.transform.position = new Vector3(0, 0, -5f);
            gameObject.transform.scale = Vector3.One;

            temp = gameObject.Clone();
            temp.showBoundingBox = true;
            temp.transform.position = new Vector3(0, 0, -5f);
            gameObject.showBoundingBox = false;
            scene.renderables.Add(temp);

            int colorTextureHandle = TextureLoader.LoadTexture(
                @"../../../_Assets/_Debug/_Textures/colors.png");
            Material colorMaterial = new DiffuseMaterial(Color4.White, colorTextureHandle);
            Mesh thirdMesh = MeshGeneration.Quad();
            GameObject quad = temp.Clone();
            quad.SetMaterial(colorMaterial);
            //quad.SetMesh(thirdMesh);
            quad.showBoundingBox = true;
            quad.transform.position = new Vector3(0, 5f, 0);
            quad.showBoundingBox = false;
            scene.renderables.Add(quad);

            temp2 = temp.Clone();
            temp2.showBoundingBox = true;
            temp2.transform.position = new Vector3(0, 0, 5f);
            temp2.SetMesh(secondMesh);
            scene.renderables.Add(temp2);


            int candleTextureHandle = TextureLoader.LoadTexture(
                @"../../../_Assets/_Debug/_Textures/colors.png");
            candle = new GameObject();
            DiffuseMaterial candleMaterial = new(Color4.White, candleTextureHandle);
            Mesh candleMesh = MeshLoader.Load(
                @"../../../_Assets/_Debug/_Models/candle.obj");
            candle.SetMaterial(candleMaterial);
            candle.SetMesh(candleMesh);
            candleMesh.PermanentlyTransformUVs(Matrix4.CreateScale(new Vector3(1, -1, 1)));
            candle.cull = true;
            scene.renderables.Add(candle);

            instanceable = gameObject.Clone();
            instanceable.SetMesh(secondMesh);
            instanceable.transform.position = Vector3.Zero;
            instanceable.transform.rotation = Vector3.Zero;
            instanceable.transform.scale = Vector3.One;
            Material particleMaterial = new ParticleMaterial(Color4.White);
            instanceable.SetMaterial(particleMaterial);

            Random rand = new Random(1);
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
            instanceable.cull = false;
            //instanceable.GetMesh()!.CreateInstanceVBO(matrices);
            //scene.renderables.Add(instanceable);

            #endregion

            GameObject plane = new GameObject();
            plane.SetMesh(MeshGeneration.Plane(1, 1));
            Material planeMaterial = new DiffuseMaterial(Color.White, 
                brickTextureHandle, brickNormalHandle);
            plane.SetMaterial(planeMaterial);
            plane.transform.scale = Vector3.One * 2f;
            plane.cull = true;
            scene.renderables.Add(plane);

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
            Font timesNewRomanFont = FontLoader.GetFont(timesNewRomanHandle);
            Mesh[] meshes = MeshGeneration.Text(timesNewRomanHandle, "Hello Mister!",
                out float textWidth);
            Material[] materials = new Material[meshes.Length];
            GameObject[] textObjects = new GameObject[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                materials[i] = new UITextMaterial(Color4.White,
                    timesNewRomanFont.textureAtlasHandle[i]);
                textObjects[i] = new GameObject();
                textObjects[i].SetMaterial(materials[i]);
                textObjects[i].SetMesh(meshes[i]);
            }

            //canvas.renderables.Add(textObjects[0]);

            Material cmuFontMaterial = new UITextMaterial(Color4.White,
                    FontLoader.GetFont(cmuSerifHandle).textureAtlasHandle[0]);
            UIElement textObject = new UIElement();
            textObject.SetMaterial(cmuFontMaterial);
            textObject.SetMesh(MeshGeneration.Text(cmuSerifHandle, "Hello World!", 
                out float textWidth2)[0]);
            textObject.transform.scale /= textWidth2 / 2;
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
            //Dispose of all EasyUnload objects:
            EasyUnload[] freezeFrame = EasyUnload.GetInstancedObjects().ToArray();
            foreach (EasyUnload objectToUnload in freezeFrame)
            {
                objectToUnload.Dispose();
                Console.WriteLine($"Unloaded object {objectToUnload.GetType()} : EasyUnload");
            }

            base.OnUnload();
        }

        public double timeSinceStartup = 0;
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            timeSinceStartup += args.Time;
            temp.transform.rotation.Y = (float)timeSinceStartup * 0.45f;
            candle.transform.rotation.Y = (float)timeSinceStartup * 0.6f;
            /*
            temp.transform.rotation.X = (float)timeSinceStartup * 0.4f;
            temp.transform.rotation.Z = (float)timeSinceStartup * 3f;
            */

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Render everything:

            //Background (skybox):
            GL.Disable(EnableCap.DepthTest);
            defaultRenderer.RenderCollection(background, camera,
                camera.GetProjectionMatrix(), camera.RotationMatrix());

            GL.Enable(EnableCap.DepthTest);
            //sceneFBO.BindFramebuffer();
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            defaultRenderer.RenderCollection(scene, camera,
                camera.GetProjectionMatrix(), camera.CameraMatrix());
            //sceneFBO.UnbindFramebuffer();

            //Instanced:
            Random rand = new Random(1);
            Matrix4[] matrices = new Matrix4[instanceCount];
            for (int i = 0; i < instanceCount; i++)
            {
                Transform instanceTransform = new Transform();
                instanceTransform.scale = Vector3.One * (0.05f +
                    MathF.Pow((float)rand.NextDouble(), 1.4f) * 1.95f);

                float theta = (float)rand.NextDouble() * 2 * MathF.PI;
                float height = (float)rand.NextDouble() * 2 - 1f;
                instanceTransform.position = new Vector3(MathF.Cos(theta),
                    height, MathF.Sin(theta)) * (25 + (float)rand.NextDouble() * 5 
                    + 5f * MathF.Sin((float)timeSinceStartup));
                instanceTransform.rotation = new Vector3(0, -theta - MathF.PI / 2, 0);
                matrices[i] = instanceTransform.TransformationMatrix();
            }
            //instanceable.GetMesh()!.UpdateInstancedTransformations(matrices);

            UBO.directionalLightData.SetLightCastFromDirection(0,
                new Vector3(1, 1, 1).Normalized());
            UBO.UpdateUBO();


            //Render UI canvas:
            GL.Disable(EnableCap.DepthTest);
            uiRenderer.RenderCollection(canvas, camera,
                Matrix4.Identity, camera.CameraMatrix());

            float distanceFromCamera = 15f;
            WireframeRenderer.RenderRay(-camera.Position() - camera.Forward() * distanceFromCamera,
                new Vector3(0, MathF.PI, 0), camera.GetProjectionMatrix(),
                camera.CameraMatrix(), Color4.Red);
            WireframeRenderer.RenderRay(-camera.Position() - camera.Forward() * distanceFromCamera,
                new Vector3(0, -MathF.PI / 2, 0), camera.GetProjectionMatrix(),
                camera.CameraMatrix(), Color4.Blue);
            WireframeRenderer.RenderRay(-camera.Position() - camera.Forward() * distanceFromCamera,
                new Vector3(0, 0, MathF.PI / 2), camera.GetProjectionMatrix(),
                camera.CameraMatrix(), Color4.Lime);

            SwapBuffers();

            base.OnRenderFrame(args);
        }
    }
}
