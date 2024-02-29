using OpenTK.Graphics.OpenGL;

namespace _3D_Renderer._Shading._Materials
{
    internal class CubemapMaterial : Material
    {
        public int textureHandle = -1;
        private int UL_textureSampler = -1;

        /// <summary>
        /// Creates new <see cref="CubemapMaterial"/> with a cubemap texture handle.
        /// </summary>
        /// <param name="textureHandle"></param>
        public CubemapMaterial(int textureHandle)
            : base(new Shader(@"../../../_Assets/_Built-In/_Shaders/_Cubemap/cubemap.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Cubemap/cubemap.frag"))
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
            GL.BindTexture(TextureTarget.TextureCubeMap, textureHandle);
        }
    }
}
