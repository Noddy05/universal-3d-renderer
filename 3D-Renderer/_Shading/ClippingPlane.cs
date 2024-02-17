using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Shading
{
    internal class ClippingPlane
    {
        private Vector3 normal;
        private Vector3 pointInPlane;
        private float d;

        public ClippingPlane(Vector3 normal, Vector3 pointInPlane)
        {
            this.pointInPlane = pointInPlane;
            SetNormal(normal);
        }

        public void SetNormal(Vector3 normal)
        {
            this.normal = normal;
            d = -Vector3.Dot(pointInPlane, normal);
        }
        public void SetPointInPlane(Vector3 pointInPlane)
        {
            this.pointInPlane = pointInPlane;
            d = -Vector3.Dot(pointInPlane, normal);
        }
        public void ApplyClippingPlane(int UL_clippingPlaneLocation)
        {
            Vector4 planeEquation = new Vector4(normal, d);
            GL.Uniform4(UL_clippingPlaneLocation, planeEquation);
        }
    }
}
