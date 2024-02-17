using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Shading._Materials
{
    internal class UnlitAlphaClipping : Material
    {
        public int textureHandle = -1;
        public Color4 color;
        private int UL_textureSampler = -1;
        private int UL_color = -1;

        public UnlitAlphaClipping(Color4 color, int textureHandle)
            : base(
                new Shader(@"../../../_Assets/_Built-In/_Shaders/_UnlitAlphaClipping/unlitalphaclip.vert",
                @"../../../_Assets/_Built-In/_Shaders/_UnlitAlphaClipping/unlitalphaclip.frag"))
        {
            this.color = color;
            this.textureHandle = textureHandle;
            UL_color = GL.GetUniformLocation(shader, "color");
            UL_textureSampler = GL.GetUniformLocation(shader, "textureSampler");
        }

        public override void ApplyMaterial()
        {
            base.ApplyMaterial();

            //Bind texture handle:
            GL.Uniform1(UL_textureSampler, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            //Apply color:
            GL.Uniform4(UL_color, color);
        }
    }
}
