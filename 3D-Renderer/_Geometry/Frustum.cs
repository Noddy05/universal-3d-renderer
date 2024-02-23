using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._GameObject;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Geometry
{
    internal class Frustum
    {
        //Should be the inverse camera matrix
        public Matrix4 frustumRotationMatrix = Matrix4.Identity;
        private Plane[] clippingPlanes;
        private GameObject frustumMesh;
        public float fov, aspectRatio, nearDist, farDist;

        //https://cgvr.cs.uni-bremen.de/teaching/cg_literatur/lighthouse3d_view_frustum_culling/index.html
        public Frustum(float fovRadians, float aspectRatio, float nearDistance, float farDistance)
        {
            fov = fovRadians;
            this.aspectRatio = aspectRatio;
            nearDist = nearDistance;
            farDist = farDistance;
            float sideAngle = MathF.Atan(MathF.Tan(fov/2) * aspectRatio);
            float cosSideAngle = MathF.Cos(sideAngle);
            float sinSideAngle = MathF.Sin(sideAngle);
            float cosTopBottomAngle = MathF.Cos(fov / 2f);
            float sinTopBottomAngle = MathF.Sin(fov / 2f);

            //Generate clipping planes:
            clippingPlanes = new Plane[6];
            clippingPlanes[0] = new Plane(-Vector3.UnitZ, new Vector3(0, 0, -nearDistance));
            clippingPlanes[1] = new Plane( Vector3.UnitZ, new Vector3(0, 0, -farDistance));
            clippingPlanes[2] = new Plane(new Vector3(cosSideAngle, 0, -sinSideAngle),
                Vector3.Zero);
            clippingPlanes[3] = new Plane(new Vector3(-cosSideAngle, 0, -sinSideAngle),
                Vector3.Zero);
            clippingPlanes[4] = new Plane(new Vector3(0, cosTopBottomAngle,
                -sinTopBottomAngle), Vector3.Zero);
            clippingPlanes[5] = new Plane(new Vector3(0, -cosTopBottomAngle,
                -sinTopBottomAngle), Vector3.Zero);
        }

        //Checks for intersection between the frustum and a specified sphere
        public bool Intersects(Vector3 position, float radius)
        {
            Vector3 modifiedPosition = (new Vector4(position, 1) * frustumRotationMatrix).Xyz;

            for (int i = 0; i < clippingPlanes.Length; i++)
            {
                Plane plane = clippingPlanes[i];
                if (plane.SignedDistance(modifiedPosition) < -radius)
                {
                    return false;
                }
            }
            return true;
        }

        //Checks for intersection between the frustum and a specified box. Simply checks for each
        //clipping plane if all points are outside. If any point is inside it returns true.
        public bool NaiveIntersects(Vector3 position, Vector3 size)
        {
            Vector3 modifiedPosition = (new Vector4(position, 1) * frustumRotationMatrix).Xyz;
            Vector3[] vertices = VerticesOfABox(modifiedPosition, size);

            for (int i = 0; i < clippingPlanes.Length; i++)
            {
                Plane plane = clippingPlanes[i];
                int pointsOutside = 0;
                for(int j = 0; j < vertices.Length; j++)
                {
                    if (plane.SignedDistance(vertices[j]) < 0)
                    {
                        pointsOutside++;
                    }
                }
                if (pointsOutside == vertices.Length)
                    return false;
            }
            return true;
        }

        private Vector3[] VerticesOfABox(Vector3 position, Vector3 size)
        {
            Vector3[] vertices = [
                position + size,
                position + size * new Vector3(1,   1, -1),
                position + size * new Vector3(1,  -1,  1),
                position + size * new Vector3(1,  -1, -1),
                position + size * new Vector3(-1,  1,  1),
                position + size * new Vector3(-1,  1, -1),
                position + size * new Vector3(-1, -1,  1),
                position + size * new Vector3(-1, -1, -1),
            ];

            return vertices;
        }
    }
}
