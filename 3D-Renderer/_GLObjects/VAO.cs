using _3D_Renderer._Behaviour;
using _3D_Renderer._Debug;
using _3D_Renderer._Renderable;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;

namespace _3D_Renderer._GLObjects
{
    internal class VAO : EasyUnload
    {
        private int handle = -1;
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(VAO vao) => vao.GetHandle();

        public VAO()
        {
            handle = GL.GenVertexArray();
        }

        public void BindVertexData(VBO vbo)
        {
            //For each vertex data struct:
            //Position:
            SetPointer(vbo, 0, 3, 8, 0, true, false);

            //Normal:
            SetPointer(vbo, 1, 3, 8, 3, false, false);

            //TexCoordinate
            SetPointer(vbo, 2, 2, 8, 6, false, true);
        }
        public void BindInstanceData(VBO instancedVBO)
        {
            //For each vertex data struct:
            //Position:
            SetPointer(instancedVBO, 3, 4, 16, 0,  true, false);
            SetPointer(instancedVBO, 4, 4, 16, 4,  false, false);
            SetPointer(instancedVBO, 5, 4, 16, 8,  false, false);
            SetPointer(instancedVBO, 6, 4, 16, 12, false, false);

            GL.VertexAttribDivisor(3, 1);
            GL.VertexAttribDivisor(4, 1);
            GL.VertexAttribDivisor(5, 1);
            GL.VertexAttribDivisor(6, 1);

            //Unbind VAO & VBO:
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetPointer(VBO vbo, int index, int size,
            int stride, int offset, bool bind, bool unbind)
        {
            if (bind)
            {
                GL.BindVertexArray(handle);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo!);
            }

            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false,
                stride * sizeof(float), offset * sizeof(float));
            GL.EnableVertexAttribArray(index);

            if (unbind)
            {
                //Unbind VAO and VBO
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="transformationMatrix"></param>
        /// <param name="bufferUsageHint"></param>
        /// <returns></returns>
        public void ModifyVertices(Matrix4 transformationMatrix)
        {
            //vbo!.TransformVertices(transformationMatrix);
        }

        private bool disposed = false;
        /// <summary>
        /// Disposes the <see cref="VAO"/> object.<br/>
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (disposed)
                return;

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(this);

            disposed = true;
        }
    }
}
