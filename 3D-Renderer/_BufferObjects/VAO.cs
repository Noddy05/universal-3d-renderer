using _3D_Renderer._Behaviour;
using _3D_Renderer._Renderable;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._BufferObjects
{
    internal class VAO : EasyUnload
    {
        private int handle = -1;
        protected Vertex[] vertices;
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
            vertices = verts;

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

        public void ModifyPointer(VBO vbo, int index, int size,
            int stride, int offset, bool copyNewVertices)
        {
            if (copyNewVertices)
            {
                Vertex[]? verts = vbo.GetVertices();
                if (verts == null)
                    throw new Exception("Trying to bind VBO to VAO failed! (VBO.GetVertices() returned null)");
                vertices = verts;
            }

            //Bind VAO and VBO
            GL.BindVertexArray(handle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            //Modify pointer:
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false,
                stride * sizeof(float), offset * sizeof(float));

            //Unbind VAO and VBO
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /// <summary>
        /// Returns the new vertices
        /// </summary>
        /// <param name="transformationMatrix"></param>
        /// <param name="bufferUsageHint"></param>
        /// <returns></returns>
        public Vertex[] ModifyVertices(Matrix4 transformationMatrix, BufferUsageHint bufferUsageHint)
        {
            Vertex[] newVerts = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                newVerts[i] = new Vertex((new Vector4(vertices[i].vertexPosition, 1) 
                    * transformationMatrix).Xyz,
                    vertices[i].vertexNormal, vertices[i].textureCoordinate);
            }
            VBO vbo = new VBO(newVerts, bufferUsageHint);
            ModifyPointer(vbo, 0, 3, 8, 0, false);
            GL.BindVertexArray(0);
            vbo.Dispose();

            return newVerts;
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
