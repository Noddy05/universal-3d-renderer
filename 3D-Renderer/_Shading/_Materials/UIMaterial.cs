using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Shading._Materials
{
    internal class UIMaterial : Material
    {
        public int textureHandle = -1;
        public Color4 color;
        private int UL_textureSampler = -1;
        private int UL_color = -1;

        public UIMaterial(Color4 color, int textureHandle) 
            : base(new Shader(
                @"../../../_Assets/_Built-In/_Shaders/_UI/_Textured/ui_element.vert",
                @"../../../_Assets/_Built-In/_Shaders/_UI/_Textured/ui_element.frag"))
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
