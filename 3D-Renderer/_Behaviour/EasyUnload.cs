using _3D_Renderer._Renderable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Behaviour
{
    internal abstract class EasyUnload : IDisposable
    {
        private static List<EasyUnload> instancedObjects = new List<EasyUnload>();
        public bool DisposeObject()
        {
            if (!instancedObjects.Contains(this))
                return false;

            Dispose();
            instancedObjects.Remove(this);
            return true;
        }

        public static List<EasyUnload> GetInstancedObjects() => instancedObjects;

        public EasyUnload()
        {
            instancedObjects.Add(this);
        }

        public virtual void Dispose()
        {
            if (instancedObjects.Contains(this))
                instancedObjects.Remove(this);
        }

        //Dispose template:
        /*

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
            GL.DeleteVertexArray(this);

            disposed = true;
        }

        */
    }
}
