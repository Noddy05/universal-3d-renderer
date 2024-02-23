using _3D_Renderer._Behaviour;
using _3D_Renderer._Renderable;
using _3D_Renderer._Shading;
using OpenTK.Graphics.OpenGL;

namespace _3D_Renderer._BufferObjects
{
    internal class InstancedVAO : VAO
    {
        //Automatically returns handle when casting to int:
        public static implicit operator int(InstancedVAO vao) => vao.GetHandle();

        public InstancedVAO()
        {
            
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
                //Bind VAO and VBO
                GL.BindVertexArray(this);
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
    }
}
