using _3D_Renderer._Behaviour;
using _3D_Renderer._GLObjects._UBO._UniformBlocks;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._GLObjects._UBO
{
    internal abstract class UniformBlock : EasyUnload
    {
        protected float[] dataToBind = [];
        private int handle;
        private int index;

        public UniformBlock(int index)
        {
            this.index = index;
            handle = GL.GenBuffer();
        }

        public void BindData()
        {
            UpdateData();

            //Bind buffer
            GL.BindBuffer(BufferTarget.UniformBuffer, handle);

            //Upload data:
            GL.BufferData(BufferTarget.UniformBuffer, dataToBind.Length * sizeof(float), dataToBind,
                BufferUsageHint.DynamicDraw);
            GL.BindBufferRange(BufferRangeTarget.UniformBuffer, index, handle,
                0, dataToBind.Length * sizeof(float));

            //Undbind buffer:
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        protected virtual void UpdateData()
        {

        }

        private bool disposed = false;
        /// <summary>
        /// Disposes the <see cref="VAO"/> object.<br/>
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (disposed)
                return;

            GL.BindVertexArray(0);
            GL.DeleteBuffer(handle);

            disposed = true;
        }
    }
}
