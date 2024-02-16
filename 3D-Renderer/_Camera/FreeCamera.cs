using _3D_Renderer._Behaviour;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Camera
{
    internal class FreeCamera : Camera
    {
        private float cameraSensitivity;
        private float moveSpeed;

        public FreeCamera(float cameraSensitivity, float moveSpeed)
        {
            this.cameraSensitivity = cameraSensitivity;
            this.moveSpeed = moveSpeed;
            Program.window.MouseMove += OnMouseMove;
        }
        public FreeCamera(float cameraSensitivity, float moveSpeed, 
            Vector3 startPosition, Vector3 startRotation)
            : base(startPosition, startRotation)
        {
            this.cameraSensitivity = cameraSensitivity;
            this.moveSpeed = moveSpeed;
            Program.window.MouseMove += OnMouseMove;
        }

        public void OnMouseMove(MouseMoveEventArgs e)
        {
            rotation += new Vector3(e.DeltaY, e.DeltaX, 0) * cameraSensitivity;
        }

        public override void Update(FrameEventArgs args)
        {
            Vector2 input = Input.Movement();
            float y = 0;
            if (Input.GetKey(Keys.E))
            {
                y += (float)args.Time;
            }
            if (Input.GetKey(Keys.Q))
            {
                y -= (float)args.Time;
            }

            Matrix4 rotationMatrix = Matrix4.Invert(RotationMatrix());
            Vector3 newPosition = (new Vector4(new Vector3(-input.X, 0, input.Y), 0)
                * rotationMatrix).Xyz;
            if (newPosition.LengthSquared > 0)
                position += newPosition.Normalized() * (float)args.Time * moveSpeed;
            position -= (new Vector4(0, y, 0, 0) * rotationMatrix).Xyz * moveSpeed;
        }
    }
}
