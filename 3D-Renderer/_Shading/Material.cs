using _3D_Renderer._Behaviour;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Shading
{
    internal class Material
    {
        public Shader shader;

        public Material(Shader shader)
        {
            this.shader = shader;
        }

        public virtual void ApplyMaterial()
        {
            GL.UseProgram(shader);
        }
    }
}
