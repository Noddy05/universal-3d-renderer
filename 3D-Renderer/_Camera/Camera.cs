using _3D_Renderer._Behaviour;
using _3D_Renderer._Geometry;
using _3D_Renderer._Saves;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Camera
{
    internal abstract class Camera : ActiveObject
    {
        protected Matrix4 projectionMatrix = Matrix4.Identity;
        public Matrix4 GetProjectionMatrix() => projectionMatrix;

        protected Window window;

        /// <summary>
        /// This should be called everytime the window is resized, this updates the perspective matrix
        /// to fit with the new aspect ratio.
        /// </summary>
        public virtual void GenerateProjectionMatrix() => new NotImplementedException();

        [SaveOnClose]
        protected Vector3 position;
        public Vector3 Position() => position;
        [SaveOnClose]
        protected Vector3 rotation;
        public Vector3 Rotation() => rotation;

        public Camera(Vector3 position, Vector3 rotation)
        {
            this.position = position;
            this.rotation = rotation;
            window = Program.GetWindow();
        }
        public Camera()
        {
            window = Program.GetWindow();
        }

        public Matrix4 CameraMatrix() =>
            Matrix4.CreateTranslation(position)
            * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation));
        public Matrix4 RotationMatrix() =>
            Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotation));
        public Matrix4 PositionMatrix() =>
            Matrix4.CreateTranslation(position);

        public Vector3 Up() => (Vector4.UnitY * CameraMatrix().Inverted()).Xyz;
        public Vector3 Down() => -Up();
        public Vector3 Forward() => (Vector4.UnitZ * CameraMatrix().Inverted()).Xyz;
        public Vector3 Backward() => -Forward();
        public Vector3 Right() => -(Vector4.UnitX * CameraMatrix().Inverted()).Xyz;
        public Vector3 Left() => -Right();
    }
}
