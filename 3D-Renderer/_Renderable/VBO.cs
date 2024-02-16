﻿using _3D_Renderer._Behaviour;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable
{
    internal class VBO : EasyUnload
    {
        private int handle = -1;
        private Vertex[]? vertices;
        public Vertex[]? GetVertices() => vertices;
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(VBO vbo) => vbo.GetHandle();

        /// <summary>
        /// Creates new <see cref="VBO"/> object.<br/>
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="bufferUsageHint"></param>
        public VBO(Vertex[] vertices, BufferUsageHint bufferUsageHint)
        {
            float[] verts = Vertex.VertexToFloatArray(vertices);

            handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts,
                bufferUsageHint);

            //Undbind buffer:
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private bool disposed = false;
        /// <summary>
        /// Disposes the <see cref="VBO"/> object.<br/>
        /// Should be done for all instantiated <see cref="VBO"/>s
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (disposed)
                return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(handle);

            disposed = true;
        }
    }
}
