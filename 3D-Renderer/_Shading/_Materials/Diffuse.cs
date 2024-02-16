using OpenTK.Graphics.OpenGL;
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
        private int UL_textureSampler = -1;

        public Diffuse(Shader shader, int textureHandle) 
            : base(shader)
        {
            this.textureHandle = textureHandle;
            UL_textureSampler = GL.GetUniformLocation(shader, "textureSampler");
        }

        public override void ApplyMaterial()
        {
            base.ApplyMaterial();

            //Bind texture handle:
            GL.Uniform1(UL_textureSampler, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            //Bind normal map:
            /*
            GL.Uniform1(UL_normalSampler, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, normalMapHandle);
            */
        }
    }
}
