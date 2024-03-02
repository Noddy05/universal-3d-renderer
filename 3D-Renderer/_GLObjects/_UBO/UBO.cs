using _3D_Renderer._Behaviour;
using _3D_Renderer._GLObjects._UBO._UniformBlocks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects._UBO
{
    /// <summary>
    /// UBO can store up to 65536 bytes, corresponding to 16384 floats or 1024 matrices.
    /// </summary>
    internal static class UBO
    {
        public static uShadowData shadowData = new uShadowData(0);
        public static uDirectionalLightData directionalLightData = new uDirectionalLightData(1);

        static UBO()
        {
            UpdateUBO();
        }

        public static void UpdateUBO()
        {
            shadowData.BindData();
            directionalLightData.BindData();
        }
    }
}
