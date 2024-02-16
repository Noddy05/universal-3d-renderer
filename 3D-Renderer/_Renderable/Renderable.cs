using _3D_Renderer._Behaviour;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable
{
    internal class Renderable : ActiveObject
    {
        /// <summary>
        /// Prepares the renderable for the renderer.
        /// </summary>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual int ApplyRenderable(Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            throw new NotImplementedException();
        }
    }
}
