using _3D_Renderer._Behaviour;
using _3D_Renderer._Renderable;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._BufferObjects
{
    internal class InstancedVBO : VBO
    {
        //Automatically returns handle when casting to int:
        public static implicit operator int(InstancedVBO vbo) => vbo.GetHandle();

        /// <summary>
        /// Creates new <see cref="VBO"/> object.<br/>
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="bufferUsageHint"></param>
        public InstancedVBO(Vertex[] vertices, BufferUsageHint bufferUsageHint)
            :base(vertices, bufferUsageHint)
        {
        }
    }
}
