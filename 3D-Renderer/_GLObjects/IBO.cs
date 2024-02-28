using _3D_Renderer._Behaviour;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects
{
    internal class IBO : EasyUnload
    {
        private int handle = -1;
        private int[] indices;
        public int[] GetIndices() => indices;
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(IBO ibo) => ibo.GetHandle();

        public IBO(int[] indices, BufferUsageHint bufferUsageHint)
        {
            this.indices = indices;
            handle = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices,
                bufferUsageHint);

            //Undbind buffer:
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private bool disposed = false;
        /// <summary>
        /// Disposes the <see cref="IBO"/> object.<br/>
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (disposed)
                return;

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(this);

            disposed = true;
        }
    }
}
