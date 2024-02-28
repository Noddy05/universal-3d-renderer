using _3D_Renderer._GLObjects;
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


        private VBO vbo;
        public VBO GetVBO() => vbo;
        private VAO vao;
        public VAO GetVAO() => vao;
        private IBO ibo;
        public IBO GetIBO() => ibo;
        private VBO instanceVBO;
        public VBO GetInstanceVBO() => instanceVBO;


        private Vertex[] vertices = [];
        public Vertex[] GetVertices() => vertices;
        public void SetVertices(Vertex[] vertices, BufferUsageHint hint)
        {
            this.vertices = vertices;
            if(vbo != null)
            {
                vbo.Dispose();
            }
            vbo = new VBO(vertices, hint);
            vao.BindVertexData(vbo);
            GL.BindVertexArray(0);
            UpdateBounding(vertices);
        }

        private int[] indices = [];
        public int[] GetIndices() => indices;
        public void SetIndices(int[] indices, BufferUsageHint hint)
        {
            this.indices = indices;
            if (ibo != null)
            {
                ibo.Dispose();
            }
            ibo = new IBO(indices, hint);
        }

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
        public Mesh(Matrix4[] transformations) : this()
        {
            CreateInstanceVBO(transformations);
        }

        public void CreateInstanceVBO(Matrix4[] transformations)
        {
            instanceVBO = new VBO(transformations, BufferUsageHint.StreamDraw);
            vao.BindInstanceData(instanceVBO);
        }

        public void UpdateInstancedTransformations(Matrix4[] transformations)
        {
            if(instanceVBO == null)
            {
                throw new Exception("This mesh is has no instanceVBO!");
            }
            instanceVBO.SetBufferSubData(transformations, 0);
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

        public Mesh CopyAsWireframe()
        {
            Mesh output = new Mesh();
            Vertex[] verticesCopied = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopied, vertices.Length);
            output.SetVertices(vertices, BufferUsageHint.StaticCopy);
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
            return output;
        }
        #endregion

        #region Operators
        public static Mesh operator +(Mesh a, Vector3 offset)
        {
            Mesh merged = new Mesh();
            merged.name = a.name;

            //Cache info:
            Vertex[] aVerts = a.vertices;
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
            Vertex[] aVerts = a.vertices;
            int[] aTris = a.ibo.GetIndices();
            Vertex[] bVerts = b.vertices;
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

        /// <summary>
        /// Creates a copy of the original <see cref="Mesh"/>. The vertices will be a deep copy
        /// whereas the indices will be a shallow copy of the original.
        /// This copy will have a deep copy of the 
        /// <see cref="IBO"/>, <see cref="VAO"/> and the <see cref="VBO"/>.
        /// </summary>
        /// <returns></returns>
        public Mesh Copy()
        {
            //I'm making a deep copy of the vertices as I intend for the user to modify
            //these vertices later, however I dont want to affect the original vertices
            //of the original mesh. For the case of indices I create a shallow copy
            //as I don't expect the user to modify the indices during runtime
            // - atleast not without creating a new IBO with SetIndices(...).
            Mesh copy = new Mesh();
            Vertex[] verticesCopied = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopied, vertices.Length);
            copy.SetVertices(verticesCopied, BufferUsageHint.StaticCopy);
            copy.SetIndices(ibo.GetIndices(), BufferUsageHint.StaticCopy);
            copy.name = name;
            return copy;
        }

        #region Vertex Transformations
        /*
        public void ApplyTransformation(Matrix4 transformation, bool updateBoundingBox)
        {
            Vertex[] transformedVertices = vbo.TransformVertices(transformation);
            if (!updateBoundingBox)
                return;

            Vertex[] transformedVerticesCloned = new Vertex[transformedVertices.Length];
            Array.Copy(transformedVertices, transformedVerticesCloned, transformedVertices.Length);
            UpdateBounding(transformedVerticesCloned);
        }
        public void PermanentlyApplyTransformation(Matrix4 transformation)
        {
            vbo.PermanentlyTransformVertices(transformation);
            vertices = new Vertex[vbo.GetVertices().Length];
            Array.Copy(vbo.GetVertices(), vertices, vbo.GetVertices().Length);
            UpdateBounding(vertices);
        }
        */
        #endregion

        #region Bounding box

        protected (Vector3 center, Vector3 size) boundingBox;
        public (Vector3 center, Vector3 size) GetBoundingBox() => boundingBox;

        public void UpdateBounding(Vertex[] vertices)
        {
            boundingRadius = CalculateBoundingRadius(vertices);
            boundingBox = CalculateBoundingBox(Matrix4.Identity, vertices);
        }
        public void UpdateBoundingBox(Vertex[] vertices, Matrix4 rotation)
        {
            boundingBox = CalculateBoundingBox(rotation, vertices);
        }
        public void UpdateBoundingRadius(Vertex[] vertices)
        {
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
                Vector3 position = (new Vector4(vertices[i].vertexPosition, 1)
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
        #endregion

        #region Modifications
        public Mesh ConvertToWireframe()
        {
            VAO oldVAO = vao;
            IBO oldIBO = ibo;
            Vertex[] verticesCopied = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopied, vertices.Length);
            SetVertices(verticesCopied, BufferUsageHint.StaticCopy);
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
    }
}
