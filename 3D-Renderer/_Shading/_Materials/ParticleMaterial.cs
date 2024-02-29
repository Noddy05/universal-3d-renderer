using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Shading._Materials
{
    internal class ParticleMaterial : Material
    {
        public Color4 color;
        private int UL_color = -1;

        /// <summary>
        /// Crates new <see cref="ParticleMaterial"/> of a specified <see cref="Color4"/>.
        /// </summary>
        /// <param name="color"></param>
        public ParticleMaterial(Color4 color)
            : base(new Shader(@"../../../_Assets/_Built-In/_Shaders/_Particle/particle.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Particle/particle.frag"))
        {
            this.color = color;
            UL_color = GL.GetUniformLocation(shader, "color");
        }

        public override void ApplyMaterial()
        {
            base.ApplyMaterial();

            //Apply color:
            GL.Uniform4(UL_color, color);
        }
    }
}
