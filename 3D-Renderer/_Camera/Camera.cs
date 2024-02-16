using _3D_Renderer._Behaviour;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Camera
{
    internal class Camera : ActiveObject
    {
        protected Vector3 position;
        public Vector3 Position() => position;
        protected Vector3 rotation;
        public Vector3 Rotation() => rotation;

        public Camera(Vector3 position, Vector3 rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
        public Camera()
        {
        }

        public Matrix4 CameraMatrix() =>
            Matrix4.CreateTranslation(position)
            * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation));
        public Matrix4 RotationMatrix() => 
            Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation));
    }
}
