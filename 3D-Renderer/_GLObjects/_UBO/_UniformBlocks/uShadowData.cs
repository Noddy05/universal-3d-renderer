using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects._UBO._UniformBlocks
{
    internal class uShadowData : UniformBlock
    {
        public Color4 shadowColor;
        public float minLightStrength;

        public uShadowData(int index) : base(index)
        {
            
        }

        protected override void UpdateData()
        {
            List<float> data = [.. VBO.Color4ToColor3FloatArray(shadowColor),
                minLightStrength];
            dataToBind = data.ToArray();
        }
    }
}
