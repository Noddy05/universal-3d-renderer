using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace _3D_Renderer._Renderable
{
    internal abstract class RenderableTransform
    {
        public Vector3 position, rotation, scale;

        private Vector3 prevPosition, prevRotation, prevScale;
        public bool PositionChangedSinceLastCheck()
        {
            if (prevPosition == position)
                return false;

            prevPosition = position;
            return true;
        }
        public bool RotationChangedSinceLastCheck()
        {
            if (prevRotation == rotation)
                return false;

            prevRotation = rotation;
            return true;
        }
        public bool ScaleChangedSinceLastCheck()
        {
            if (prevScale == scale)
                return false;

            prevScale = scale;
            return true;
        }

        public RenderableTransform()
        {
            position = Vector3.Zero;
            rotation = Vector3.Zero;
            scale = Vector3.One;
            prevPosition = position;
            prevRotation = rotation;
            prevScale = scale;
        }

        public virtual Matrix4 TransformationMatrix()
        {
            return Matrix4.Identity;
        }
        public virtual Matrix4 TranslationMatrix()
        {
            return Matrix4.Identity;
        }
        public virtual Matrix4 RotationMatrix()
        {
            return Matrix4.Identity;
        }
        public virtual Matrix4 ScalingMatrix()
        {
            return Matrix4.Identity;
        }
    }
}
