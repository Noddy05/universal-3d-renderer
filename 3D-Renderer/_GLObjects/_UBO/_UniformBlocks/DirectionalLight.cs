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
        private Color4 color;
        public Color4 GetLightColor() => color;
        public void SetLightColor(Color4 lightColor) =>
            color = lightColor;

        private float strength;
        public float GetLightStrength() => strength;
        public void SetLightStrength(float strength) =>
            this.strength = strength;

        private Vector3 castFromDirection;
        public Vector3 GetLightCastFromDirection() => castFromDirection;
        public void SetLightCastFromDirection(Vector3 lightCastFromDirection) =>
            castFromDirection = lightCastFromDirection;

        /// <summary>
        /// Calculates a rotation matrix that transforms any vector into the light-direction space.
        /// <br></br>Based on: https://www.geogebra.org/m/ytgwmcmw
        /// </summary>
        /// <param name="tolerance">When checking if the castdirection is direcly up or down,
        /// how close to 1 does the dot product between the direction and the y-axis have to be
        /// to be considered aligned.</param>
        /// <returns></returns>
        public Matrix4 CalculateRotationMatrix(float tolerance = 0.98f)
        {
            Matrix4 mat4 = new Matrix4();
            Vector3 primaryAxis;
            Vector3 secondaryAxis;
            Vector3 forward = castFromDirection;
            mat4.Column2 = new Vector4(forward, 0);
            mat4.Column3 = new Vector4(0, 0, 0, 1);

            //First check if light is pointing direction up or down
            float dot = Vector3.Dot(castFromDirection, Vector3.UnitY);
            if(MathF.Abs(dot) >= tolerance) //Adding a little bit of tolerance
            {
                primaryAxis = new Vector3(1, 0, 0) * CustomSignFunction(dot);
                secondaryAxis = new Vector3(0, 0, 1) * CustomSignFunction(dot);
                mat4.Column0 = new Vector4(primaryAxis, 0);
                mat4.Column1 = new Vector4(secondaryAxis, 0);
            }

            float primaryZ = forward.X /
                MathF.Sqrt(MathF.Pow(forward.X, 2) + MathF.Pow(forward.Z, 2));
            primaryAxis = new Vector3(CustomSignFunction(forward.Z) * 
                MathF.Sqrt(1 - primaryZ * primaryZ), 0,  -primaryZ);
            secondaryAxis = Vector3.Cross(primaryAxis, forward);
            if(secondaryAxis.Y < 0)
            {
                secondaryAxis *= -1;
                primaryAxis *= -1;
            }
            mat4.Column0 = new Vector4(primaryAxis, 0);
            mat4.Column1 = new Vector4(secondaryAxis, 0);

            return mat4;
        }

        /// <summary>
        /// This function retrieves the signed value of a number, except its 1 when the number is 0
        /// </summary>
        /// <returns></returns>
        private float CustomSignFunction(float x)
        {
            return x >= 0 ? 1 : -1;
        }
    }
}
