using _3D_Renderer._Behaviour;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects
{
    /// <summary>
    /// UBO can store up to 65536 bytes, corresponding to 16384 floats or 1024 matrices.
    /// </summary>
    internal static class UBO
    {
        private static int handle = -1;

        static UBO()
        {
            handle = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.UniformBuffer, handle);
            //Allocate memory:
            //A matrix fills 16 bytes:
            GL.BufferData(BufferTarget.UniformBuffer, 16 * 1000 * sizeof(float), IntPtr.Zero, 
                BufferUsageHint.DynamicRead);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, handle,
                0, 16 * 1000 * sizeof(float));

            //Undbind buffer:
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        private static bool disposed = false;
        /// <summary>
        /// Disposes the <see cref="UBO"/> object.<br/>
        /// </summary>
        public static void Dispose()
        {
            if (disposed)
                return;

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.DeleteBuffer(handle);

            disposed = true;
        }
    }
}
