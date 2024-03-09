using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Shading._Materials
{
    internal class OutlineMaterial : Material
    {
        public Color4 color;
        public float outlineThickness = 1.0f;
        private int UL_color = -1;
        private int UL_outlineThickness = -1;

        /// <summary>
        /// Creates a new <see cref="OutlineMaterial"/> of a specific <see cref="Color4"/>.
        /// </summary>
        /// <param name="color"></param>
        public OutlineMaterial(Color4 color)
            : base(new Shader(@"../../../_Assets/_Debug/_Shaders/_Outline/outline.vert",
                @"../../../_Assets/_Debug/_Shaders/_Outline/outline.frag"))
        {
            this.color = color;
            outlineThickness = 1.0f;
            UL_color = GL.GetUniformLocation(shader, "color");
            UL_outlineThickness = GL.GetUniformLocation(shader, "outlineThickness");
        }

        public override void ApplyMaterial()
        {
            base.ApplyMaterial();

            //Apply color:
            GL.Uniform4(UL_color, color);
            GL.Uniform1(UL_outlineThickness, outlineThickness);
        }
    }
}
