using _3D_Renderer._Behaviour;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects._FBO
{
    internal class FBO : EasyUnload
    {
        protected int framebuffer;
        protected int depthBuffer;
        protected int depthTexture;
        protected int texture;
        protected Vector2i size;

        public int GetFramebufferHandle() => framebuffer;
        public int GetDepthbufferHandle() => depthBuffer;
        public int GetDepthTextureHandle() => depthTexture;
        public int GetTextureHandle() => texture;
        public Vector2i GetSize() => size;

        public FBO(Vector2i size)
        {
            this.size = size;
            CreateFramebuffer();
        }

        public virtual void SetSize(Vector2i size) => this.size = size;

        public virtual void BindFramebuffer()
        {
            BindFramebuffer(framebuffer, size.X, size.Y);
        }

        public virtual void UnbindFramebuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Window? gameWindow = Program.GetWindow();
            if (gameWindow == null)
                throw new Exception("No window to render to!");

            GL.Viewport(0, 0, gameWindow.Size.X, gameWindow.Size.Y);
        }

        protected virtual void BindFramebuffer(int frameBuffer, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.Viewport(0, 0, width, height);
        }

        protected virtual void CreateFramebuffer()
        {
            int framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            this.framebuffer = framebuffer;
        }

        public virtual void CreateTextureAttachment()
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, size.X, size.Y, 0,
                PixelFormat.Rgb, PixelType.UnsignedByte, nint.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture, 0);
            this.texture = texture;
        }

        public virtual void CreateDepthTextureAttachment()
        {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, size.X, size.Y, 0,
                PixelFormat.DepthComponent, PixelType.UnsignedByte, nint.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, texture, 0);
            depthTexture = texture;
        }

        public virtual void CreateDepthBufferAttachment()
        {
            int depthBuffer = GL.GenBuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, size.X, size.Y);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, depthBuffer);
            this.depthBuffer = depthBuffer;
        }


        private bool disposed = false;
        /// <summary>
        /// Disposes the <see cref="FBO"/> object.<br/>
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (disposed)
                return;

            GL.DeleteFramebuffer(framebuffer);
            GL.DeleteRenderbuffer(depthBuffer);
            GL.DeleteTexture(texture);
            GL.DeleteTexture(depthTexture);

            disposed = true;
        }
    }
}
