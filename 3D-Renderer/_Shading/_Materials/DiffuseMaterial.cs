using _3D_Renderer._Geometry;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Shading._Materials
{
    internal class DiffuseMaterial : Material
    {
        public int textureHandle = -1;
        public int reflectionMap = -1;
        public float cubemapReflectivity = 0.5f;
        public float cubemapRefractivity = 0.5f;
        public float reflectivity = 1;
        public float specularHighlightDamper = 15;
        public Color4 color;
        private int UL_color = -1;
        private int UL_clippingPlane = -1;
        private int UL_textureSampler = -1;
        private int UL_reflectionSampler = -1;
        private int UL_cubemapReflectivity  = -1;
        private int UL_cubemapRefractivity  = -1;
        private int UL_reflectivity         = -1;
        private int UL_specularHighlightDamper = -1;
        private Plane clippingPlane;

        /// <summary>
        /// Creates new <see cref="DiffuseMaterial"/> that responds to light, shadows, cubemaps...
        /// </summary>
        /// <param name="textureHandle"></param>
        /// <param name="color"></param>
        /// <param name="reflectionCubemapTexture"></param>
        public DiffuseMaterial(int textureHandle, Color4 color, int reflectionCubemapTexture)
            : base(new Shader(@"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_vertex_shader.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_fragment_shader.frag"))
        {
            clippingPlane = new Plane(Vector3.Zero, Vector3.Zero);

            this.color = color;
            this.textureHandle = textureHandle;
            reflectionMap = reflectionCubemapTexture;
            UL_color = GL.GetUniformLocation(shader, "color");
            UL_clippingPlane = GL.GetUniformLocation(shader, "clippingPlane");
            UL_textureSampler = GL.GetUniformLocation(shader, "textureSampler");
            UL_reflectionSampler = GL.GetUniformLocation(shader, "reflectionSampler");
            UL_cubemapReflectivity  = GL.GetUniformLocation(shader, "cubemapReflectivity");
            UL_cubemapRefractivity  = GL.GetUniformLocation(shader, "cubemapRefractivity");
            UL_reflectivity            = GL.GetUniformLocation(shader, "reflectivity");
            UL_specularHighlightDamper = GL.GetUniformLocation(shader, "specularHighlightDamper");
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
            GL.BindTexture(TextureTarget.TextureCubeMap, reflectionMap);

            GL.Uniform4(UL_color, color);
            GL.Uniform1(UL_cubemapReflectivity, cubemapReflectivity);
            GL.Uniform1(UL_cubemapRefractivity, cubemapRefractivity);
            GL.Uniform1(UL_reflectivity, reflectivity);
            GL.Uniform1(UL_specularHighlightDamper, specularHighlightDamper);

            clippingPlane.ApplyAsClippingPlane(UL_clippingPlane);
        }
    }
}
