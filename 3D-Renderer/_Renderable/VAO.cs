using OpenTK.Graphics.OpenGL;

namespace _3D_Renderer._Renderable
{
    internal class VAO
    {
        private int handle = -1;
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(VAO vao) => vao.GetHandle();

        public VAO()
        {
            handle = GL.GenVertexArray();
        }

        public void Bind(int vboHandle, int index, int size, 
            int stride, int offset)
        {
            GL.BindVertexArray(handle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride * sizeof(float),
                offset * sizeof(float));
            GL.EnableVertexAttribArray(index);

            //Unbind VAO and VBO
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
