using _3D_Renderer._Behaviour;
using _3D_Renderer._Renderable;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            : this(ToFloatArray(vertices), bufferUsageHint) { }
        /// <summary>
        /// Automatically converts <see cref="Matrix4"/> array to <see cref="float"/> array
        /// and creates new <see cref="VBO"/> object.
        /// </summary>
        /// <param name="matrices"></param>
        /// <param name="bufferUsageHint"></param>
        public VBO(Matrix4[] matrices, BufferUsageHint bufferUsageHint)
            : this(ToFloatArray(matrices), bufferUsageHint) { }

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
            => SetBufferSubData(ToFloatArray(vertices), offset);
        public void SetBufferSubData(Matrix4[] matrices, int offset)
            => SetBufferSubData(ToFloatArray(matrices), offset);
        public void SetBufferSubData(float[] data, int offset)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, offset, data.Length * sizeof(float), data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public static float[] ToFloatArray(Matrix4[] matrix)
        {
            List<float> matricesAsFloats = new List<float>();
            for(int i = 0; i < matrix.Length; i++) {
                matricesAsFloats.AddRange(ToFloatArray(matrix[i]));
            }

            return matricesAsFloats.ToArray();
        }
        public static float[] ToFloatArray(Matrix4 matrix)
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
        public static float[] ToFloatArray(Vertex[] vertices)
        {
            List<float> vertexList = new List<float>();
            foreach (Vertex vertex in vertices)
            {
                vertexList.AddRange(vertex.ToArray());
            }

            return vertexList.ToArray();
        }
        public static float[] ToFloatArray(Vector3 vector)
        {
            return [vector.X, vector.Y, vector.Z];
        }
        public static float[] ToFloatArray(Color4 color)
        {
            return [color.R, color.G, color.B, color.A];
        }
        public static float[] Color4ToColor3FloatArray(Color4 color)
        {
            return [color.R, color.G, color.B];
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
