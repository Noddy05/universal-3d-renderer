using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects._FBO
{
    internal class ShadowMapFBO : FBO
    {
        public ShadowMapFBO(Vector2i size) : base(size)
        {
            CreateDepthTextureAttachment();
            UnbindFramebuffer();
        }

        protected override void CreateFramebuffer()
        {
            int framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            this.framebuffer = framebuffer;
        }
        protected override void BindFramebuffer(int frameBuffer, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.Viewport(0, 0, width, height);
        }

        public override void CreateDepthTextureAttachment()
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, 
                size.X, size.Y, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, nint.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
                (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                (int)TextureMinFilter.Linear);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, 
                FramebufferAttachment.DepthAttachment, texture, 0);
            depthTexture = texture;
        }
    }
}
