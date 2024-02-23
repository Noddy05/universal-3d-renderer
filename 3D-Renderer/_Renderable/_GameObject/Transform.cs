using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable._GameObject
{
    internal class Transform : RenderableTransform
    {
        public Transform()
        {
            scale = Vector3.One;
        }

        public override Matrix4 TransformationMatrix()
            => Matrix4.CreateScale(scale) *
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation)) *
                Matrix4.CreateTranslation(position);
        public override Matrix4 TranslationMatrix()
            => Matrix4.CreateTranslation(position);
        public override Matrix4 RotationMatrix()
            => Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation));
        public override Matrix4 ScalingMatrix()
            => Matrix4.CreateScale(scale);
    }
}
