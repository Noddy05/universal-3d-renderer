using _3D_Renderer._Geometry;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Shading._Materials
{
    internal class DiffuseMaterial : Material
    {
        //Material parameters:
        //Textures:
        public int textureHandle = -1;
        public int normalMapHandle = -1;
        public int reflectionMapHandle = -1;
        public int shadowMapHandle = -1;
        public bool useNormalMap = false;

        public float cubemapReflectivity = 0f;
        public float cubemapRefractivity = 0f;
        public float reflectivity = 1;
        public float specularHighlightDamper = 15;
        public Color4 color;

        //Uniforms locations:
        private int UL_color                   = -1;
        private int UL_clippingPlane           = -1;
        private int UL_cubemapReflectivity     = -1;
        private int UL_cubemapRefractivity     = -1;
        private int UL_reflectivity            = -1;
        private int UL_useNormalMap            = -1;
        private int UL_specularHighlightDamper = -1;
        private int UL_textureSampler          = -1;
        private int UL_reflectionSampler       = -1;
        private int UL_normalSampler           = -1;
        private int UL_shadowSampler        = -1;

        //Might delete:
        private Plane clippingPlane;

        /// <summary>
        /// Creates new <see cref="DiffuseMaterial"/> that responds to light, shadows, cubemaps...
        /// </summary>
        /// <param name="textureHandle"></param>
        /// <param name="color"></param>
        /// <param name="reflectionCubemapTexture"></param>
        public DiffuseMaterial(Color4 color, bool instanced = false)
            : base(new Shader(instanced ? 
                @"../../../_Assets/_Built-In/_Shaders/_Diffuse/instanced.vert"
                : @"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_vertex_shader.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_fragment_shader.frag", 
                (@"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_geometry_shader.geom", 
                ShaderType.GeometryShader)))
        {
            clippingPlane = new Plane(Vector3.Zero, Vector3.Zero);

            useNormalMap = false;
            this.color = color;
            textureHandle = Program.GetWindow().GetDefaultTextureHandle();

            UL_color = GL.GetUniformLocation(shader, "color");
            UL_clippingPlane = GL.GetUniformLocation(shader, "clippingPlane");
            UL_cubemapReflectivity = GL.GetUniformLocation(shader, "cubemapReflectivity");
            UL_cubemapRefractivity = GL.GetUniformLocation(shader, "cubemapRefractivity");
            UL_reflectivity = GL.GetUniformLocation(shader, "reflectivity");
            UL_specularHighlightDamper = GL.GetUniformLocation(shader, "specularHighlightDamper");
            UL_useNormalMap = GL.GetUniformLocation(shader, "useNormalMap");
            UL_textureSampler = GL.GetUniformLocation(shader, "textureSampler");
            UL_reflectionSampler = GL.GetUniformLocation(shader, "reflectionSampler");
            UL_normalSampler = GL.GetUniformLocation(shader, "normalSampler");
            UL_shadowSampler = GL.GetUniformLocation(shader, "shadowSampler");
        }
        public DiffuseMaterial(Color4 color, int textureHandle, bool instanced = false)
            : this(color, instanced)
        {
            this.textureHandle = textureHandle;
        }

        public DiffuseMaterial(Color4 color, int textureHandle, int normalMapHandle, 
            bool instanced = false)
            : this(color, textureHandle, instanced)
        {
            this.normalMapHandle = normalMapHandle;
            useNormalMap = true;
        }
        public DiffuseMaterial(Color4 color, int textureHandle, 
            int normalMapHandle, int reflectionCubemapTexture, bool instanced = false)
            : this(color, textureHandle, normalMapHandle, instanced)
        {
            reflectionMapHandle = reflectionCubemapTexture;
            cubemapReflectivity = 0.5f;
            cubemapRefractivity = 0.5f;
        }

        public override void ApplyMaterial()
        {
            /*
            clippingPlane.SetPointInPlane(Vector3.UnitY 
                * MathF.Sin((float)Program.window.timeSinceStartup));
                * */
            clippingPlane.SetNormal(Vector3.Zero); //Disables clipping plane

            base.ApplyMaterial();

            //Bind texture handle:
            GL.Uniform1(UL_textureSampler, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            //Bind cubemap:
            GL.Uniform1(UL_reflectionSampler, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, reflectionMapHandle);

            //Bind normalmap:
            GL.Uniform1(UL_normalSampler, 2);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, normalMapHandle);

            //Bind normalmap:
            GL.Uniform1(UL_shadowSampler, 3);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, shadowMapHandle);

            //Uniforms:
            GL.Uniform4(UL_color, color);
            GL.Uniform1(UL_cubemapReflectivity, cubemapReflectivity);
            GL.Uniform1(UL_cubemapRefractivity, cubemapRefractivity);
            GL.Uniform1(UL_reflectivity, reflectivity);
            GL.Uniform1(UL_specularHighlightDamper, specularHighlightDamper);
            GL.Uniform1(UL_useNormalMap, useNormalMap ? 1f : 0f);

            clippingPlane.ApplyAsClippingPlane(UL_clippingPlane);
        }
    }
}
