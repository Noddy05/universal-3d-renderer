using _3D_Renderer._BufferObjects;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace _3D_Renderer._Renderable
{
    internal class Mesh
    {
        public string name;

        private VAO vao;
        public VAO GetVAO() => vao;
        private IBO ibo;
        public IBO GetIBO() => ibo;
        private int indices = 0;
        public int IndicesCount() => indices;

        private bool isWireframe = false;
        public bool IsWiremesh() => isWireframe;

        /// <summary>
        /// Bind this when rendering mesh
        /// </summary>
        public void Bind(out bool isWireframe) {
            isWireframe = this.isWireframe;
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

        public Mesh CopyAsWireframe()
        {
            Mesh output = new Mesh();
            output.SetVertices(vao.CloneVertices(), BufferUsageHint.StaticCopy);
            int[] indices = ibo.GetIndices();
            int[] newIndices = new int[indices.Length * 2];
            for (int i = 0; i < indices.Length; i += 3)
            {
                newIndices[i * 2    ] = indices[i];
                newIndices[i * 2 + 1] = indices[i + 1];
                newIndices[i * 2 + 2] = indices[i + 1];
                newIndices[i * 2 + 3] = indices[i + 2];
                newIndices[i * 2 + 4] = indices[i + 2];
                newIndices[i * 2 + 5] = indices[i];
            }
            output.name = name;
            output.SetIndices(newIndices, BufferUsageHint.StaticCopy);
            output.isWireframe = true;
            return output;
        }

        public static Mesh operator +(Mesh a, Vector3 offset)
        {
            Mesh merged = new Mesh();
            merged.name = a.name;

            //Cache info:
            Vertex[] aVerts = a.vao.GetVertices();
            int[] aTris = a.ibo.GetIndices();
            Vertex[] newVertices = new Vertex[aVerts.Length];

            for (int i = 0; i < aVerts.Length; i++)
            {
                newVertices[i] = aVerts[i];
                newVertices[i].vertexPosition += offset;
            }
            merged.SetVertices(newVertices, BufferUsageHint.StaticCopy);
            merged.SetIndices(aTris, BufferUsageHint.StaticCopy);

            return merged;
        }
        public static Mesh operator -(Mesh a, Vector3 offset)
        {
            return a + -offset;
        }
        public static Mesh operator +(Mesh a, Mesh b)
        {
            Mesh merged = new Mesh();
            merged.name = a.name;

            //Cache info:
            Vertex[] aVerts = a.vao.GetVertices();
            int[] aTris = a.ibo.GetIndices();
            Vertex[] bVerts = b.vao.GetVertices();
            int[] bTris = b.ibo.GetIndices();
            Vertex[] newVertices = new Vertex[aVerts.Length + bVerts.Length];
            int[] newIndices = new int[aTris.Length + bTris.Length];

            for (int i = 0; i < aVerts.Length; i++)
            {
                newVertices[i] = aVerts[i];
            }
            for (int i = 0; i < bVerts.Length; i++)
            {
                newVertices[i + aVerts.Length] = bVerts[i];
            }
            for (int i = 0; i < aTris.Length; i++)
            {
                newIndices[i] = aTris[i];
            }
            for (int i = 0; i < bTris.Length; i++)
            {
                newIndices[i + aTris.Length] = bTris[i] + aVerts.Length;
            }
            merged.SetVertices(newVertices, BufferUsageHint.StaticCopy);
            merged.SetIndices(newIndices, BufferUsageHint.StaticCopy);

            return merged;
        }

        public Mesh Copy()
        {
            Mesh copy = new Mesh();
            copy.SetVertices(vao.CloneVertices(), BufferUsageHint.StaticCopy);
            copy.SetIndices(ibo.GetIndices(), BufferUsageHint.StaticCopy);
            return copy;
        }
    }
}
