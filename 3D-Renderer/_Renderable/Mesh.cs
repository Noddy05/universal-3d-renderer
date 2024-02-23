using _3D_Renderer._BufferObjects;
using _3D_Renderer._Renderable._GameObject;
using _3D_Renderer._Renderable._UIElement;
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

        private float boundingRadius;
        public float GetBoundingRadius() => boundingRadius;

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

        public void DisposeIBO()
        {
            ibo.Dispose();
        }
        public void DisposeVAO()
        {
            vao.Dispose();
        }

        public void SetVertices(Vertex[] vertices, BufferUsageHint hint)
        {
            VBO vbo = new VBO(vertices, hint);
            Vertex.BindVAO(vbo, vao);
            GL.BindVertexArray(0);
            vbo.Dispose();
            boundingRadius = CalculateBoundingRadius(vertices);
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

        #region Modification
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

        #region Wireframe
        public Mesh CopyAsWireframe()
        {
            Mesh output = new Mesh();
            output.SetVertices(vao.CloneVertices(), BufferUsageHint.StaticCopy);
            int[] indices = ibo.GetIndices();
            int[] newIndices = new int[indices.Length * 2];
            for (int i = 0; i < indices.Length; i += 3)
            {
                newIndices[i * 2] = indices[i];
                newIndices[i * 2 + 1] = indices[i + 1];
                newIndices[i * 2 + 2] = indices[i + 1];
                newIndices[i * 2 + 3] = indices[i + 2];
                newIndices[i * 2 + 4] = indices[i + 2];
                newIndices[i * 2 + 5] = indices[i];
            }
            output.name = name;
            output.SetIndices(newIndices, BufferUsageHint.StaticCopy);
            return output;
        }
        public Mesh ConvertToWireframe()
        {
            VAO oldVAO = vao;
            IBO oldIBO = ibo;
            SetVertices(vao.CloneVertices(), BufferUsageHint.StaticCopy);
            oldVAO.Dispose();

            int[] indices = ibo.GetIndices();
            int[] newIndices = new int[indices.Length * 2];
            for (int i = 0; i < indices.Length; i += 3)
            {
                newIndices[i * 2] = indices[i];
                newIndices[i * 2 + 1] = indices[i + 1];
                newIndices[i * 2 + 2] = indices[i + 1];
                newIndices[i * 2 + 3] = indices[i + 2];
                newIndices[i * 2 + 4] = indices[i + 2];
                newIndices[i * 2 + 5] = indices[i];
            }
            SetIndices(newIndices, BufferUsageHint.StaticCopy);
            oldIBO.Dispose();
            return this;
        }
        #endregion
        #endregion

        #region Operators
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
        #endregion

        public Mesh Copy()
        {
            Mesh copy = new Mesh();
            copy.SetVertices(vao.CloneVertices(), BufferUsageHint.StaticCopy);
            copy.SetIndices(ibo.GetIndices(), BufferUsageHint.StaticCopy);
            return copy;
        }

        public void ApplyTransformation(Matrix4 transformation)
        {
            Vertex[] vertices = vao.CloneVertices();
            foreach(Vertex v in vertices)
            {
                v.vertexPosition = (transformation * new Vector4(v.vertexPosition, 0)).Xyz;
            }
            SetVertices(vertices, BufferUsageHint.StaticCopy);
            SetIndices(ibo.GetIndices(), BufferUsageHint.StaticCopy);
        }

        public void UpdateBounding(Vertex[] vertices)
        {
            CalculateBoundingBox(vertices);
            boundingRadius = CalculateBoundingRadius(vertices);
        }

        private float CalculateBoundingRadius(Vertex[] vertices)
        {
            float maxRadiusSquared = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                float radius = vertices[i].vertexPosition.LengthSquared;
                if (radius > maxRadiusSquared)
                {
                    maxRadiusSquared = radius;
                }
            }

            return MathF.Sqrt(maxRadiusSquared);
        }

        private (Vector3 center, Vector3 size) CalculateBoundingBox(Matrix4 rotationMatrix,
            Vertex[] vertices)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 position = (new Vector4(vertices[i].vertexPosition, 0)
                    * rotationMatrix).Xyz;

                if (position.X > maxX)
                    maxX = position.X;
                if (position.Y > maxY)
                    maxY = position.Y;
                if (position.Z > maxZ)
                    maxZ = position.Z;

                if (position.X < minX)
                    minX = position.X;
                if (position.Y < minY)
                    minY = position.Y;
                if (position.Z < minZ)
                    minZ = position.Z;
            }

            return (new Vector3(minX + maxX, minY + maxY, minZ + maxZ) / 2f,
                new Vector3(maxX - minX, maxY - minY, maxZ - minZ) / 2f);
        }
        private (Vector3 center, Vector3 size) CalculateBoundingBox(Vertex[] vertices)
        {
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 position = vertices[i].vertexPosition;

                if (position.X > maxX)
                    maxX = position.X;
                if (position.Y > maxY)
                    maxY = position.Y;
                if (position.Z > maxZ)
                    maxZ = position.Z;

                if (position.X < minX)
                    minX = position.X;
                if (position.Y < minY)
                    minY = position.Y;
                if (position.Z < minZ)
                    minZ = position.Z;
            }

            return (new Vector3(minX + maxX, minY + maxY, minZ + maxZ) / 2f,
                new Vector3(maxX - minX, maxY - minY, maxZ - minZ) / 2f);
        }

        public (Vector3 center, Vector3 size) CalculateBoundingBox(Matrix4 rotationMatrix)
        {
            return CalculateBoundingBox(rotationMatrix, vao.GetVertices());
        }
        public (Vector3 center, Vector3 size) CalculateBoundingBox()
        {
            return CalculateBoundingBox(vao.GetVertices());
        }
    }
}
