using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects._UBO._UniformBlocks
{
    /// <summary>
    /// Supports 16 directional lights
    /// </summary>
    internal class uDirectionalLightData : UniformBlock
    {
        private const int maxDirectionalLights = 16;

        private DirectionalLight[] directionalLights 
            = new DirectionalLight[maxDirectionalLights];
        public DirectionalLight GetDirectionalLight(int index)
            => directionalLights[index];

        public uDirectionalLightData(int index) : base(index)
        {
            for(int i = 0; i < maxDirectionalLights; i++)
            {
                directionalLights[i] = new DirectionalLight();
            }
        }

        protected override void UpdateData()
        {
            List<float> data = new List<float>();
            for (int i = 0; i < maxDirectionalLights; i++)
            {
                data.AddRange(VBO.Color4ToColor3FloatArray(directionalLights[i].GetLightColor()));
                data.Add(directionalLights[i].GetLightStrength());
                data.AddRange(VBO.ToFloatArray(directionalLights[i].GetLightCastFromDirection()));
                data.Add(0); //dummy variable
            }
            dataToBind = data.ToArray();
        }
    }
}
