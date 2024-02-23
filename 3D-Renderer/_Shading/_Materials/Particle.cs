using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Shading._Materials
{
    internal class Particle : Material
    {
        public Color4 color;
        private int UL_color = -1;
        private int[] UL_transformationMatrices;
        public Matrix4[] transformations;

        public Particle(Color4 color)
            : base(new Shader(@"../../../_Assets/_Built-In/_Shaders/_Particle/particle.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Particle/particle.frag"))
        {
            this.color = color;
            UL_color = GL.GetUniformLocation(shader, "color");

            //The shader currently only supports 1000 instances
            UL_transformationMatrices = new int[200];
            transformations = new Matrix4[UL_transformationMatrices.Length];
            for (int i = 0; i < UL_transformationMatrices.Length; i++)
            {
                UL_transformationMatrices[i] = GL.GetUniformLocation(shader, 
                    $"transformationMatrix[{i}]");
            }
        }

        public override void ApplyMaterial()
        {
            base.ApplyMaterial();

            //Apply color:
            GL.Uniform4(UL_color, color);
            for (int i = 0; i < UL_transformationMatrices.Length; i++)
            {
                GL.UniformMatrix4(UL_transformationMatrices[i], false, 
                    ref transformations[i]);
            }
        }
    }
}
