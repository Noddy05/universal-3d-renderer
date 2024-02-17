using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Shading._Materials
{
    internal class UIText : Material
    {
        public int textureHandle = -1;
        public Color4 color;
        private int UL_textureSampler = -1;
        private int UL_color = -1;
        private int UL_aspectRatio = -1;

        public UIText(Color4 color, int textureHandle) 
            : base(new Shader(
                @"../../../_Assets/_Built-In/_Shaders/_UI/_Text/text.vert",
                @"../../../_Assets/_Built-In/_Shaders/_UI/_Text/text.frag"))
        {
            this.color = color;
            this.textureHandle = textureHandle;
            UL_color = GL.GetUniformLocation(shader, "color");
            UL_textureSampler = GL.GetUniformLocation(shader, "textureSampler");
            UL_aspectRatio = GL.GetUniformLocation(shader, "aspectRatio");
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
            GL.Uniform1(UL_aspectRatio, Program.window.Size.X / (float)Program.window.Size.Y);
        }
    }
}
