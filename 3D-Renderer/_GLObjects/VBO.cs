using _3D_Renderer._Behaviour;
using _3D_Renderer._Renderable;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace _3D_Renderer._GLObjects
{
    internal class VBO : EasyUnload
    {
        private int handle = -1;
        /*
        private Vertex[] vertices = [];
        public Vertex[] GetVertices() => vertices;
        */
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(VBO vbo) => vbo.GetHandle();

        /// <summary>
        /// Automatically converts <see cref="Vertex"/> array to <see cref="float"/> array
        /// and creates new <see cref="VBO"/> object.
        /// </summary>
        /// <param name="matrices"></param>
        /// <param name="bufferUsageHint"></param>
        public VBO(Vertex[] vertices, BufferUsageHint bufferUsageHint)
            : this(VerticesToFloatArray(vertices), bufferUsageHint) { }
        /// <summary>
        /// Automatically converts <see cref="Matrix4"/> array to <see cref="float"/> array
        /// and creates new <see cref="VBO"/> object.
        /// </summary>
        /// <param name="matrices"></param>
        /// <param name="bufferUsageHint"></param>
        public VBO(Matrix4[] matrices, BufferUsageHint bufferUsageHint)
            : this(MatricesToFloatArray(matrices), bufferUsageHint) { }

        /// <summary>
        /// Creates new <see cref="VBO"/> object.<br/>
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="bufferUsageHint"></param>
        public VBO(float[] data, BufferUsageHint bufferUsageHint)
        {
            handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data,
                bufferUsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        /// <summary>
        /// Creates new <see cref="VBO"/> object.<br/>
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="bufferUsageHint"></param>
        public VBO(int bytesOfMemoryToAllocate, BufferUsageHint bufferUsageHint)
        {
            handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, bytesOfMemoryToAllocate, 0,
                bufferUsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetBufferSubData(Vertex[] vertices, int offset)
            => SetBufferSubData(VerticesToFloatArray(vertices), offset);
        public void SetBufferSubData(Matrix4[] matrices, int offset)
            => SetBufferSubData(MatricesToFloatArray(matrices), offset);
        public void SetBufferSubData(float[] data, int offset)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, offset, data.Length * sizeof(float) * 8, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        /*
        public VBO(Matrix4[] matrices, BufferUsageHint bufferUsageHint)
        {
            float[] matricesAsFloats = new float[16 * matrices.Length];
            for(int i = 0; i < matrices.Length; i++)
            {
                float[] matrixAsFloats = MatrixToFloatArray(matrices[i]);
                for (int j = 0; j < 16; j++)
                {
                    matricesAsFloats[i * 16 + j] = matrixAsFloats[j];
                }
            }

            handle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferData(BufferTarget.ArrayBuffer, matricesAsFloats.Length * sizeof(float),
                matricesAsFloats, bufferUsageHint);

            //Unbind buffer:
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        */

        /*
        /// <summary>
        /// This is extremely slow at high number of vertices.
        /// </summary>
        /// <param name="transformation"></param>
        /// <returns>Transformed Vertices</returns>
        public Vertex[] TransformVertices(Matrix4 transformation)
        {
            Vertex[] transformedVertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                transformedVertices[i] =
                    new Vertex(vertices[i].vertexPosition,
                    vertices[i].vertexNormal,
                    vertices[i].textureCoordinate);

                transformedVertices[i].vertexPosition = (new Vector4(vertices[i].vertexPosition, 1)
                    * transformation).Xyz;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, transformedVertices.Length * sizeof(float) * 8,
                Vertex.VertexToFloatArray(transformedVertices));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return transformedVertices;
        }
        /// <summary>
        /// This is extremely slow at high number of vertices.
        /// </summary>
        /// <param name="transformation"></param>
        public void PermanentlyTransformVertices(Matrix4 transformation)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].vertexPosition = (new Vector4(vertices[i].vertexPosition, 1)
                    * transformation).Xyz;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, vertices.Length * sizeof(float) * 8,
                Vertex.VertexToFloatArray(vertices));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        */

        public static float[] MatricesToFloatArray(Matrix4[] matrix)
        {
            List<float> matricesAsFloats = new List<float>();
            for(int i = 0; i < matrix.Length; i++) {
                matricesAsFloats.AddRange(MatrixToFloatArray(matrix[i]));
            }

            return matricesAsFloats.ToArray();
        }
        public static float[] MatrixToFloatArray(Matrix4 matrix)
        {
            Vector4[] vectors = [
                matrix.Row0,
                matrix.Row1,
                matrix.Row2,
                matrix.Row3,
            ];
            float[] matrixAsFloats = new float[16];
            for (int i = 0; i < vectors.Length; i++)
            {
                matrixAsFloats[i * 4    ] = vectors[i].X;
                matrixAsFloats[i * 4 + 1] = vectors[i].Y;
                matrixAsFloats[i * 4 + 2] = vectors[i].Z;
                matrixAsFloats[i * 4 + 3] = vectors[i].W;
            }
            return matrixAsFloats;
        }
        public static float[] VerticesToFloatArray(Vertex[] vertices)
        {
            List<float> vertexList = new List<float>();
            foreach (Vertex vertex in vertices)
            {
                vertexList.AddRange(vertex.ToArray());
            }

            return vertexList.ToArray();
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
