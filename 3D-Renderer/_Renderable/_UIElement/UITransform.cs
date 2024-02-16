using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable._UIElement
{
    internal class UITransform
    {
        public Vector2 position;
        public Vector3 rotation;
        public Vector2 scale;

        public UITransform()
        {
            scale = Vector2.One;
        }

        public Matrix4 TransformationMatrix()
            => Matrix4.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(
                rotation)) *
                Matrix4.CreateTranslation(new Vector3(position.X, position.Y, -0.2f));
    }
}
