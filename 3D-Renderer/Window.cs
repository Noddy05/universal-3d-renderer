using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using _3D_Renderer._Shading;
using _3D_Renderer._Import;
using _3D_Renderer._Camera;
using _3D_Renderer._Rendering._Renderers;
using _3D_Renderer._Renderable._GameObject;
using _3D_Renderer._Shading._Materials;
using _3D_Renderer._Debug;
using _3D_Renderer._Renderable._UIElement;
using _3D_Renderer._Generation;
using _3D_Renderer._Renderable._Cubemap;
using _3D_Renderer._Behaviour;
using _3D_Renderer._GLObjects._UBO;
using Font = _3D_Renderer._Import.Font;
using _3D_Renderer._SceneHierarchy;
using _3D_Renderer._Saves;
using _3D_Renderer._Editor;
using _3D_Renderer._GLObjects._FBO;
using _3D_Renderer._Renderable._Mesh;

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

        [SaveOnClose]
        private FreeCamera testCamera;

        private FreeCamera editorCamera;
        public FreeCamera GetEditorCamera() => editorCamera;
        private Material defaultMaterial;
        public Material GetDefaultMaterial() => defaultMaterial;
        //Renderers
        private DefaultRenderer defaultRenderer;
        private ShadowMapRenderer shadowRenderer;
        private UIRenderer uiRenderer;
        private EditorUI editorUI;

        //Temp:
        private FBO shadowMapFBO;
        private int fboOutputTexture;
        private int cmuSerifHandle;
        private int timesNewRomanHandle;
        private GameObject instanceable;
        private int brickTextureHandle = -1;
        private int brickNormalHandle = -1;
        private OutlineMaterial outlineMaterial;
        private GameObject temp;
        private GameObject temp2;
        private GameObject candle;
        private int instanceCount = 500;

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
            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

            EditorMemory.SetLatestSaveLocation(@"../../../_Saves/_LatestSave");

            defaultMaterial = new UnlitMaterial(Color.Purple);

            //Create perspectivematrix, camera and locks mouse:
            editorCamera = new FreeCamera(70 * MathF.PI / 180f, 0.1f, 1000f, 1 / 800f, 5f);

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

            //Add collections:
            SceneHierarchy.NewCollection("Background");
            SceneHierarchy.NewCollection("Canvas");

            //Populate scene and instantiate RenderStats:
            renderStats = new RenderStats();


            //Loading has finished, show application:
            IsVisible = true;
            CursorState = CursorState.Grabbed;

            UBO.shadowData.shadowColor = new Color4(19, 22, 46, 255);
            UBO.shadowData.minLightStrength = 0.1f;
            UBO.directionalLightData.GetDirectionalLight(0).SetLightColor(Color4.White);
            UBO.directionalLightData.GetDirectionalLight(0).SetLightStrength(1);
            UBO.directionalLightData.GetDirectionalLight(0)
                .SetLightCastFromDirection(new Vector3(1, 1, 1).Normalized());
            UBO.UpdateUBO();

            testCamera = new FreeCamera(10, 0.1f, 1000, 0, 0);
            SaveFileParser.AddEncoder(typeof(FreeCamera), (object obj) =>
            {
                FreeCamera freeCamera = (FreeCamera)obj;
                return SaveFileParser.GetEncoder(typeof(Vector3))(freeCamera.Up());
            });
            SaveFileParser.AddDecoder(typeof(FreeCamera), (string data) =>
            {
                return SaveFileParser.GetDecoder(typeof(Vector3));
            });

            EditorMemory.AttachObject("EditorCamera", editorCamera);
            EditorMemory.AttachObject("WindowState", this);


            editorUI = new EditorUI(editorCamera);

            shadowMapFBO = new ShadowMapFBO(new Vector2i((int)(Size.X * 0.3f), (int)(Size.Y * 0.3f)));
            shadowRenderer = new ShadowMapRenderer();

            PopulateScene();
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
            outlineMaterial = new OutlineMaterial(Color4.Red);

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
            SceneHierarchy.AddRenderable("Background", cubemap);
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
            material.shadowMapHandle = shadowMapFBO.GetTextureHandle();
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
            SceneHierarchy.AddRenderable("Scene", temp);

            int colorTextureHandle = TextureLoader.LoadTexture(
                @"../../../_Assets/_Debug/_Textures/colors.png");
            Material colorMaterial = new DiffuseMaterial(Color4.White, colorTextureHandle);
            Mesh thirdMesh = MeshGeneration.Circle(15);
            GameObject quad = temp.Clone();
            quad.SetMaterial(colorMaterial);
            quad.SetMesh(thirdMesh);
            quad.showBoundingBox = true;
            quad.transform.position = new Vector3(0, 5f, 0);
            quad.showBoundingBox = false;
            SceneHierarchy.AddRenderable("Scene", quad);

            temp2 = temp.Clone();
            temp2.showBoundingBox = true;
            temp2.transform.position = new Vector3(0, 0, 5f);
            temp2.SetMesh(secondMesh);
            SceneHierarchy.AddRenderable("Scene", temp2);


            int candleTextureHandle = TextureLoader.LoadTexture(
                @"../../../_Assets/_Debug/_Textures/colors.png");
            candle = new GameObject();
            DiffuseMaterial candleMaterial = new(Color4.White, candleTextureHandle, true);
            Mesh candleMesh = MeshLoader.Load(
                @"../../../_Assets/_Debug/_Models/candle.obj");
            candle.SetMaterial(candleMaterial);
            candle.SetMesh(candleMesh);
            candleMesh.PermanentlyTransformUVs(Matrix4.CreateScale(new Vector3(1, -1, 1)));
            candle.cull = false;
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
            candle.GetMesh()!.CreateInstanceVBO(matrices);
            SceneHierarchy.AddRenderable("Scene", candle);

            instanceable = gameObject.Clone();
            instanceable.SetMesh(secondMesh);
            instanceable.transform.position = Vector3.Zero;
            instanceable.transform.rotation = Vector3.Zero;
            instanceable.transform.scale = Vector3.One;
            Material particleMaterial = new ParticleMaterial(Color4.White);
            instanceable.SetMaterial(particleMaterial);


            #endregion

            GameObject plane = new GameObject();
            plane.SetMesh(MeshGeneration.Plane(1, 1));
            Material planeMaterial = new DiffuseMaterial(Color.White, 
                brickTextureHandle, brickNormalHandle);
            plane.SetMaterial(planeMaterial);
            plane.transform.scale = Vector3.One * 2f;
            plane.cull = true;
            SceneHierarchy.AddRenderable("Scene", plane);

            #region UI
            //UI:
            Material uiMaterial = new UIMaterial(Color4.White, shadowMapFBO.GetDepthTextureHandle());

            UIElement uiElement = new UIElement();
            uiElement.SetMaterial(uiMaterial);
            float scale = 0.3f;
            uiElement.transform.scale *= scale;
            uiElement.transform.position += Vector3.One * (1 - scale);
            SceneHierarchy.AddRenderable("Canvas", uiElement);
            #endregion

            #region Text
            //Text
            Font timesNewRomanFont = FontLoader.GetFont(timesNewRomanHandle);
            Mesh[] meshes = MeshGeneration.Text(timesNewRomanHandle, "In-Game text!",
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

            textObjects[0].transform.position -= Vector3.UnitY * 0.8f + Vector3.UnitX * 0.7f;
            textObjects[0].transform.scale /= 2f;
            SceneHierarchy.AddRenderable("Canvas", textObjects[0]);

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
            editorCamera.GenerateProjectionMatrix();
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

        [SaveOnClose]
        public double timeSinceStartup = 0;
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            timeSinceStartup += args.Time;
            //temp.transform.rotation.Y = (float)timeSinceStartup * 0.45f;
            //candle.transform.rotation.Y = (float)timeSinceStartup * 0.6f;

            //Render everything:
            DefaultRender();
            shadowMapFBO.BindFramebuffer();
            LightRender();
            shadowMapFBO.UnbindFramebuffer();
            /*
            //Background (skybox):
            GL.Disable(EnableCap.DepthTest);
            defaultRenderer.RenderCollection("Background", editorCamera,
                editorCamera.GetProjectionMatrix(), editorCamera.RotationMatrix());

            GL.Enable(EnableCap.DepthTest);

            defaultRenderer.RenderCollection("Scene", editorCamera,
                editorCamera.GetProjectionMatrix(), editorCamera.CameraMatrix());
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            */

            //Instanced:
            Random rand = new Random(1);
            Matrix4[] matrices = new Matrix4[instanceCount];
            for (int i = 0; i < candle.GetMesh()!.InstanceCount(); i++)
            {
                Transform instanceTransform = new Transform();
                instanceTransform.scale = Vector3.One * (0.05f +
                    MathF.Pow((float)rand.NextDouble(), 1.4f) * 1.95f);

                float theta = (float)rand.NextDouble() * 2 * MathF.PI;
                float height = (float)rand.NextDouble() * 2 - 1f;
                instanceTransform.position = new Vector3(MathF.Cos(theta) 
                    * (1.2f + 0.2f * MathF.Sin((float)timeSinceStartup)),
                    height, MathF.Sin(theta) * (1.2f + 0.2f * MathF.Sin((float)timeSinceStartup))) 
                    * (25 + (float)rand.NextDouble() * 5);
                instanceTransform.rotation = new Vector3(0, -theta - MathF.PI / 2, 0);
                matrices[i] = instanceTransform.TransformationMatrix();
            }
            matrices[0] = Matrix4.Identity;
            matrices[0] *= Matrix4.CreateScale(3);
            candle.GetMesh()!.UpdateInstancedTransformations(matrices);

            UBO.directionalLightData.GetDirectionalLight(0).SetLightCastFromDirection
                (new Vector3(MathF.Sin((float)timeSinceStartup), 0, 
                MathF.Cos((float)timeSinceStartup)).Normalized());
            UBO.UpdateUBO();


            //Render UI canvas:
            GL.Disable(EnableCap.DepthTest);
            uiRenderer.RenderCollection("Canvas", editorCamera,
                Matrix4.Identity, editorCamera.CameraMatrix());

            editorUI.DrawOrientation();

            SwapBuffers();

            base.OnRenderFrame(args);
        }

        private void DefaultRender()
        {
            defaultRenderer.gizmosEnabled = true;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.DepthTest);
            defaultRenderer.RenderCollection("Background", editorCamera,
                editorCamera.GetProjectionMatrix(), editorCamera.RotationMatrix());

            GL.Enable(EnableCap.DepthTest);

            defaultRenderer.RenderCollection("Scene", editorCamera,
                editorCamera.GetProjectionMatrix(), editorCamera.CameraMatrix());
        }
        private void LightRender()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shadowRenderer.RenderCollection("Scene", UBO.directionalLightData.GetDirectionalLight(0));
        }
    }
}
