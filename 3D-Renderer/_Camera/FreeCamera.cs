using _3D_Renderer._Behaviour;
using _3D_Renderer._Geometry;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Camera
{
    internal class FreeCamera : Camera
    {
        private float cameraSensitivity;
        private float moveSpeed;
        //FOV is in radians
        private float fov, nearPlane, farPlane;
        public float GetFOV() => fov;

        private Frustum cameraFrustum;
        public Frustum CameraFrustum() => cameraFrustum;

        /// <summary>
        /// This should be called everytime the window is resized, this updates the perspective matrix
        /// to fit with the new aspect ratio.
        /// </summary>
        public override void GenerateProjectionMatrix()
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(70f / 180f * MathF.PI,
                (float)window.Size.X / window.Size.Y, nearPlane, farPlane);
            CreateFrustum();
        }

        public FreeCamera(float fovRadians, float nearPlane, float farPlane, 
            float cameraSensitivity, float moveSpeed)
        {
            fov = fovRadians;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            this.cameraSensitivity = cameraSensitivity;
            this.moveSpeed = moveSpeed;
            window.MouseMove += OnMouseMove;
            CreateFrustum();
            GenerateProjectionMatrix();
        }
        public FreeCamera(float fovRadians, float nearPlane, float farPlane,
            float cameraSensitivity, float moveSpeed, 
            Vector3 startPosition, Vector3 startRotation)
            : base(startPosition, startRotation)
        {
            fov = fovRadians;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            this.cameraSensitivity = cameraSensitivity;
            this.moveSpeed = moveSpeed;
            window.MouseMove += OnMouseMove;
            CreateFrustum();
            GenerateProjectionMatrix();
        }

        private void CreateFrustum()
        {
            cameraFrustum = new Frustum(fov, window.Size.X / (float)window.Size.Y, nearPlane, farPlane);
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
            rotation += new Vector3(e.DeltaY, e.DeltaX, 0) * cameraSensitivity;
            rotation.X = MathF.Max(MathF.Min(rotation.X, MathF.PI / 2), -MathF.PI / 2);
        }

        public override void Update(FrameEventArgs args)
        {
            Vector2 input = Input.Movement();
            float y = 0;
            float currentMoveSpeed = moveSpeed;
            if (Input.GetKey(Keys.E))
            {
                y += (float)args.Time;
            }
            if (Input.GetKey(Keys.Q))
            {
                y -= (float)args.Time;
            }
            if (Input.GetKey(Keys.LeftShift))
            {
                currentMoveSpeed *= 4;
            }

            Matrix4 rotationMatrix = Matrix4.Invert(RotationMatrix());
            Vector3 newPosition = (new Vector4(new Vector3(-input.X, 0, input.Y), 0)
                * rotationMatrix).Xyz;
            if (newPosition.LengthSquared > 0)
                position += newPosition.Normalized() * (float)args.Time * currentMoveSpeed;
            position -= (new Vector4(0, y, 0, 0) * rotationMatrix).Xyz * currentMoveSpeed;
        }
    }
}
