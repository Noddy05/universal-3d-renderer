using _3D_Renderer._Behaviour;
using _3D_Renderer._Renderable;
using _3D_Renderer._Shading;
using OpenTK.Graphics.OpenGL;

namespace _3D_Renderer._BufferObjects
{
    internal class VAO : EasyUnload
    {
        private int handle = -1;
        private Vertex[] vertices;
        public Vertex[] GetVertices() => vertices;
        public Vertex[] CloneVertices() => VBO.DuplicateVertices(vertices);
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(VAO vao) => vao.GetHandle();

        public VAO()
        {
            handle = GL.GenVertexArray();
        }

        public void Bind(VBO vbo, int index, int size,
            int stride, int offset, bool bind, bool unbind)
        {
            Vertex[]? verts = vbo.GetVertices();
            if (verts == null)
                throw new Exception("Trying to bind VBO to VAO failed! (VBO.GetVertices() returned null)");
            vertices = VBO.DuplicateVertices(verts!);

            if (bind)
            {
                GL.BindVertexArray(handle);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
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
