﻿using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Shading._Materials
{
    internal class UnlitMaterial : Material
    {
        public Color4 color;
        private int UL_color = -1;

        /// <summary>
        /// Creates a new <see cref="UnlitMaterial"/> of a specific <see cref="Color4"/>.
        /// </summary>
        /// <param name="color"></param>
        public UnlitMaterial(Color4 color)
            : base(new Shader(@"../../../_Assets/_Built-In/_Shaders/_Unlit/unlit.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Unlit/unlit.frag"))
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
