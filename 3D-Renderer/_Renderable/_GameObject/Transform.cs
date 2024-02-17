using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable._GameObject
{
    internal class Transform
    {
        public Vector3 position, rotation, scale;

        public Transform()
        {
            scale = Vector3.One;
        }

        public Matrix4 TransformationMatrix()
            => Matrix4.CreateScale(scale) *
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation)) *
                Matrix4.CreateTranslation(position);
        public Matrix4 RotationMatrix()
            => Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation));
    }
}
