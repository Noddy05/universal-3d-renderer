using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable
{
    internal class IBO
    {
        private int handle = -1;
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(IBO ibo) => ibo.GetHandle();

        public IBO(int[] indices, BufferUsageHint bufferUsageHint)
        {
            handle = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices,
                bufferUsageHint);

            //Undbind buffer:
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
