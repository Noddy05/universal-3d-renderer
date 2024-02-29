using OpenTK.Graphics.OpenGL;

namespace _3D_Renderer._Shading
{
    internal class Material
    {
        public Shader shader;

        /// <summary>
        /// Creates a new material from a specified <see cref="Shader"/>.
        /// </summary>
        /// <param name="shader"></param>
        public Material(Shader shader)
        {
            this.shader = shader;
        }

        /// <summary>
        /// Binds <see cref="Shader"/> and others (uniforms...) depends on the <see cref="Material"/>.
        /// </summary>
        public virtual void ApplyMaterial()
        {
            GL.UseProgram(shader);
        }
    }
}
