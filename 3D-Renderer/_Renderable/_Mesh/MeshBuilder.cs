using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _3D_Renderer._GLObjects;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Renderable._Mesh
{
    /// <summary>
    /// This is an object that will allow for faster mesh modification, that can later
    /// be converted into an actual mesh.
    /// </summary>
    internal class MeshBuilder
    {
        public string name = "";

        protected Vertex[] vertices = [];
        public Vertex[] GetVertices() => vertices;
        public void SetVertices(Vertex[] vertices) => this.vertices = vertices;

        protected int[] indices = [];
        public int[] GetIndices() => indices;
        public void SetIndices(int[] indices) => this.indices = indices;

        #region Modification

        #region Flipping

        public void FlipFacesUVsNormals()
        {
            FlipFaces();
            FlipUVs();
            PermanentlyTransformNormals(Matrix4.CreateScale(-1));
        }
        public void FlipFaces()
        {
            for (int i = 0; i < indices.Length; i += 3)
            {
                int temp = indices[i + 2];
                indices[i + 2] = indices[i + 1];
                indices[i + 1] = temp;
            }
        }
        public void FlipUVs()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 texCoords = vertices[i].textureCoordinate;
                vertices[i].textureCoordinate = new Vector2(1 - texCoords.X, 1 - texCoords.Y);
            }
        }
        #endregion

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
        }
        public void PermanentlyTransformNormals(Matrix4 transformation)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].vertexNormal = (new Vector4(vertices[i].vertexNormal, 1)
                    * transformation).Xyz;
            }
        }
        public void PermanentlyTransformUVs(Matrix4 transformation)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].textureCoordinate = (new Vector4(
                    vertices[i].textureCoordinate.X, vertices[i].textureCoordinate.Y, 1, 1)
                * transformation).Xy;
            }
        }
        #endregion

        #region Operators
        public static MeshBuilder operator +(MeshBuilder a, Vector3 offset)
        {
            a.PermanentlyTransformVertices(Matrix4.CreateTranslation(offset));
            return a;
        }
        public static MeshBuilder operator -(MeshBuilder a, Vector3 offset)
        {
            a.PermanentlyTransformVertices(Matrix4.CreateTranslation(-offset));
            return a;
        }

        public static MeshBuilder operator +(MeshBuilder a, MeshBuilder b)
        {
            //Cache info:
            Vertex[] aVerts = a.vertices;
            int[] aTris = a.indices;

            for (int i = 0; i < aVerts.Length; i++)
            {
                a.vertices[i] = aVerts[i];
            }
            for (int i = 0; i < b.vertices.Length; i++)
            {
                a.vertices[i + aVerts.Length] = b.vertices[i];
            }
            for (int i = 0; i < aTris.Length; i++)
            {
                a.indices[i] = aTris[i];
            }
            for (int i = 0; i < b.indices.Length; i++)
            {
                a.indices[i + aTris.Length] = b.indices[i] + aVerts.Length;
            }

            return a;
        }
        #endregion

        public Mesh Build()
        {
            Mesh outputMesh = new Mesh();
            outputMesh.SetVertices(vertices, BufferUsageHint.StaticDraw);
            outputMesh.SetIndices(indices, BufferUsageHint.StaticDraw);
            outputMesh.name = name;

            return outputMesh;
        }
    }
}
