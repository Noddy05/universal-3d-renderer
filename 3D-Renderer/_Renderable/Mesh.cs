using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable
{
    internal class Mesh
    {
        public string name;

        private VAO vao;
        private IBO ibo;
        private int indices = 0;
        public int IndicesCount() => indices;

        /// <summary>
        /// Bind this when rendering mesh
        /// </summary>
        public void Bind() {
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
        }

        public Mesh()
        {
            vao = new VAO();
        }

        public void SetVertices(Vertex[] vertices, BufferUsageHint hint)
        {
            VBO vbo = new VBO(vertices, hint);
            Vertex.BindVAO(vbo, vao);
            GL.BindVertexArray(0);
            vbo.Dispose();
        }
        public void SetIndices(int[] indices, BufferUsageHint hint)
        {
            this.indices = indices.Length;
            if(ibo != null)
            {
                ibo.Dispose();
            }
            ibo = new IBO(indices, hint);
        }
        public void FlipFaces()
        {
            int[] indices = ibo.GetIndices();
            for(int i = 0; i < indices.Length; i += 3)
            {
                int temp = indices[i];
                indices[i] = indices[i + 1];
                indices[i + 1] = temp;
            }
            SetIndices(indices, BufferUsageHint.StaticCopy);
        }
    }
}
