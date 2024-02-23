using _3D_Renderer._Behaviour;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._BufferObjects
{
    internal class UBO : EasyUnload
    {
        private int handle = -1;
        public int GetHandle() => handle;

        //Automatically returns handle when casting to int:
        public static implicit operator int(UBO ubo) => ubo.GetHandle();

        public UBO(Matrix4[] matrices, BufferUsageHint hint)
        {
            handle = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.UniformBuffer, handle);
            //Allocate memory:
            //A matrix fills 16 bytes:
            GL.BufferData(BufferTarget.UniformBuffer, 16 * 1000 * sizeof(float), IntPtr.Zero, hint);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, handle,
                0, 16 * 1000 * sizeof(float));
            for(int i = 0; i < matrices.Length; i++)
            {
                GL.BufferSubData(BufferTarget.UniformBuffer, 16 * i * sizeof(float), 
                    16 * sizeof(float), ref matrices[i]);
            }

            //Undbind buffer:
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void Update(Matrix4[] matrices)
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, handle);
            for (int i = 0; i < matrices.Length; i++)
            {
                GL.BufferSubData(BufferTarget.UniformBuffer, 16 * i * sizeof(float),
                    16 * sizeof(float), ref matrices[i]);
            }
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        private bool disposed = false;
        /// <summary>
        /// Disposes the <see cref="UBO"/> object.<br/>
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (disposed)
                return;

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            GL.DeleteBuffer(this);

            disposed = true;
        }
    }
}
