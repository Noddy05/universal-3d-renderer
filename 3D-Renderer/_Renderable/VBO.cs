using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable
{
    internal class VBO
    {
        private int handle = -1;
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(VBO vbo) => vbo.GetHandle();

        public VBO(float[] vertices, BufferUsageHint bufferUsageHint)
        {
            handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
                bufferUsageHint);

            //Undbind buffer:
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
