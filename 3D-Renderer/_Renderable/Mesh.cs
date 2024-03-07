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
using System.Reflection.Metadata;
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

        private int instances = 1;
        public int InstanceCount() => instances;
        private int maxInstances = 1;
        /// <summary>
        /// This returns the number of instances allowed in the current <see cref="VBO"/>.
        /// If you want more you must make a new <see cref="VBO"/>.
        /// </summary>
        /// <returns></returns>
        public int MaxInstancesAllowed() => instances;
        public void SetInstanceCount(int count)
        {
            if (count < 1)
            {
                throw new Exception("Mesh must have atleast 1 instance!");
            }
            if (count > maxInstances)
            {
                throw new Exception("Mesh VBO only allows for a maximum of"
                    + $"{maxInstances}");
            }
            instances = count;
        }

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
            UpdateBoundingRadius(vertices);
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
            instances = transformations.Length;
            maxInstances = instances;
            if(instanceVBO != null)
            {
                instanceVBO.Dispose();
            }
            instanceVBO = new VBO(transformations, BufferUsageHint.StreamDraw);
            vao.BindInstanceData(instanceVBO);
        }

        public void UpdateInstancedTransformations(Matrix4[] transformations)
        {
            if (instanceVBO == null)
            {
                throw new Exception("This mesh is has no instanceVBO!");
            }
            if (transformations.Length > instances)
            {
                throw new Exception("Too many transformations matrices for VBO!"
                    + $"Only {instances} transformations allowed.");
            }
            instanceVBO.SetBufferSubData(transformations, 0);
        }

        #region Modification
        public void FlipFaces()
        {
            int[] indices = ibo.GetIndices();
            for (int i = 0; i < indices.Length; i += 3)
            {
                int temp = indices[i + 2];
                indices[i + 2] = indices[i + 1];
                indices[i + 1] = temp;
            }
            SetIndices(indices, BufferUsageHint.StaticDraw);
        }
        public void FlipUVs()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 texCoords = vertices[i].textureCoordinate;
                vertices[i].textureCoordinate = new Vector2(1-texCoords.X, 1-texCoords.Y);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertices.Length * sizeof(float) * 8,
                Vertex.VertexToFloatArray(vertices));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public Mesh CopyAsWireframe()
        {
            Mesh output = new Mesh();
            Vertex[] verticesCopied = new Vertex[vertices.Length];
            Array.Copy(vertices, verticesCopied, vertices.Length);
            output.SetVertices(vertices, BufferUsageHint.StaticDraw);
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
            output.SetIndices(newIndices, BufferUsageHint.StaticDraw);
            return output;
        }
        #endregion

        /// <summary>
        /// Transforms vertices in the <see cref="VBO"/> only. 
        /// This means that the vertices in the mesh is not affected by this change
        /// </summary>
        /// <param name="transformation"></param>
        /// <returns>Transformed Vertices</returns>
        public Vertex[] TransformVertices(Matrix4 transformation)
        {
            Vertex[] transformedVertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                transformedVertices[i] = vertices[i];
                transformedVertices[i].vertexPosition = (new Vector4(vertices[i].vertexPosition, 1)
                    * transformation).Xyz;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, transformedVertices.Length * sizeof(float) * 8,
                Vertex.VertexToFloatArray(transformedVertices));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return transformedVertices;
        }

        /// <summary>
        /// Transforms vertices in the <see cref="VBO"/> and the mesh vertices. 
        /// </summary>
        /// <param name="transformation"></param>
        /// <returns>Transformed Vertices</returns>
        public void PermanentlyTransformVertices(Matrix4 transformation)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].vertexPosition = (new Vector4(vertices[i].vertexPosition, 1)
                    * transformation).Xyz;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertices.Length * sizeof(float) * 8,
                Vertex.VertexToFloatArray(vertices));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        public void PermanentlyTransformNormals(Matrix4 transformation)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].vertexNormal = (new Vector4(vertices[i].vertexNormal, 1)
                    * transformation).Xyz;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertices.Length * sizeof(float) * 8,
                Vertex.VertexToFloatArray(vertices));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

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
            merged.SetVertices(newVertices, BufferUsageHint.StaticDraw);
            merged.SetIndices(aTris, BufferUsageHint.StaticDraw);

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
            int[] aTris;
            if (a.ibo != null)
                aTris = a.ibo.GetIndices();
            else 
                aTris = [];

            Vertex[] bVerts = b.vertices;
            int[] bTris;
            if (b.ibo != null)
                bTris = b.ibo.GetIndices();
            else
                bTris = [];

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
            merged.SetVertices(newVertices, BufferUsageHint.StaticDraw);
            merged.SetIndices(newIndices, BufferUsageHint.StaticDraw);

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
            copy.SetVertices(verticesCopied, BufferUsageHint.StaticDraw);
            copy.SetIndices(ibo.GetIndices(), BufferUsageHint.StaticDraw);
            copy.name = name;
            return copy;
        }

        #region Bounding box
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

        public (Vector3 center, Vector3 size) CalculateBoundingBox(Matrix4 rotationMatrix,
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
            SetVertices(verticesCopied, BufferUsageHint.StaticDraw);
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
            SetIndices(newIndices, BufferUsageHint.StaticDraw);
            oldIBO.Dispose();
            return this;
        }

        #endregion
    }
}
