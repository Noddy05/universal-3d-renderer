using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Shading._Materials
{
    internal class Diffuse : Material
    {
        public int textureHandle = -1;
        public int reflectionMap = -1;
        public float cubemapReflectivity = 0.5f;
        public float cubemapRefractivity = 0.5f;
        public float reflectivity = 1;
        public float specularHighlightDamper = 15;
        private int UL_clippingPlane = -1;
        private int UL_textureSampler = -1;
        private int UL_reflectionSampler = -1;
        private int UL_cubemapReflectivity  = -1;
        private int UL_cubemapRefractivity  = -1;
        private int UL_reflectivity         = -1;
        private int UL_specularHighlightDamper = -1;
        private ClippingPlane clippingPlane;

        public Diffuse(int textureHandle, int reflectionCubemapTexture)
            : base(new Shader(@"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_vertex_shader.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Diffuse/debug_fragment_shader.frag"))
        {
            clippingPlane = new ClippingPlane(Vector3.UnitY, Vector3.Zero);

            this.textureHandle = textureHandle;
            reflectionMap = reflectionCubemapTexture;
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
            clippingPlane.SetPointInPlane(Vector3.UnitY 
                * MathF.Sin((float)Program.window.timeSinceStartup));

            base.ApplyMaterial();

            //Bind texture handle:
            GL.Uniform1(UL_textureSampler, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            //Bind cubemap:
            GL.Uniform1(UL_reflectionSampler, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, reflectionMap);

            GL.Uniform1(UL_cubemapReflectivity, cubemapReflectivity);
            GL.Uniform1(UL_cubemapRefractivity, cubemapRefractivity);
            GL.Uniform1(UL_reflectivity, reflectivity);
            GL.Uniform1(UL_specularHighlightDamper, specularHighlightDamper);

            clippingPlane.ApplyClippingPlane(UL_clippingPlane);
        }
    }
}
