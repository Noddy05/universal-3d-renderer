using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Geometry
{
    internal class Plane
    {
        private Vector3 normal;
        private Vector3 pointInPlane;
        private float d;

        public Plane(Vector3 normal, Vector3 pointInPlane)
        {
            this.pointInPlane = pointInPlane;
            SetNormal(normal);
        }

        public void SetNormal(Vector3 normal)
        {
            this.normal = normal.Normalized();
            d = -Vector3.Dot(pointInPlane, this.normal);
        }
        public void SetPointInPlane(Vector3 pointInPlane)
        {
            this.pointInPlane = pointInPlane;
            d = -Vector3.Dot(pointInPlane, normal);
        }
        public void ApplyAsClippingPlane(int UL_clippingPlaneLocation)
        {
            Vector4 planeEquation = new Vector4(normal, d);
            GL.Uniform4(UL_clippingPlaneLocation, planeEquation);
        }

        //https://mathinsight.org/distance_point_plane
        //This requires the normal to be normalized
        public float SignedDistance(Vector3 point)
            => Vector3.Dot(normal, point) + d;
    }
}
