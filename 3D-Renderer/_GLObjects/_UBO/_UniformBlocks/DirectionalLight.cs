using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects._UBO._UniformBlocks
{
    internal class DirectionalLight
    {
        private Color4 lightColor;
        public Color4 GetLightColor() => lightColor;
        public void SetLightColor(Color4 lightColor) =>
            this.lightColor = lightColor;

        private float lightStrength;
        public float GetLightStrength() => lightStrength;
        public void SetLightStrength(float strength) =>
            lightStrength = strength;

        private Vector3 lightCastFromDirection;
        public Vector3 GetLightCastFromDirection() => lightCastFromDirection;
        public void SetLightCastFromDirection(Vector3 lightCastFromDirection) =>
            this.lightCastFromDirection = lightCastFromDirection;
    }
}
