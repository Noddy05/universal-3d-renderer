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


        private Color4[] lightColor = new Color4[maxDirectionalLights];
        public void SetLightColor(int index, Color4 lightColor) => 
            this.lightColor[index] = lightColor;

        private float[] lightStrength = new float[maxDirectionalLights];
        public void SetLightStrength(int index, float strength) =>
            lightStrength[index] = strength;

        private Vector3[] lightCastFromDirection = new Vector3[maxDirectionalLights];
        public void SetLightCastFromDirection(int index, Vector3 lightCastFromDirection) =>
            this.lightCastFromDirection[index] = lightCastFromDirection;

        public uDirectionalLightData(int index) : base(index)
        {
            
        }

        protected override void UpdateData()
        {
            List<float> data = new List<float>();
            for (int i = 0; i < maxDirectionalLights; i++)
            {
                data.AddRange(VBO.Color4ToColor3FloatArray(lightColor[i]));
                data.Add(lightStrength[i]);
                data.AddRange(VBO.ToFloatArray(lightCastFromDirection[i]));
                data.Add(0); //dummy variable
            }
            dataToBind = data.ToArray();
        }
    }
}
