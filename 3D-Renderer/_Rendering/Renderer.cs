using _3D_Renderer._Renderable;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Rendering
{
    internal class Renderer
    {
        /// <summary>
        /// Renders a collection of <see cref="Renderable"></see>(s)
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void RenderCollection(Collection collection, Matrix4 projectionMatrix,
            Matrix4 cameraMatrix)
        {
            throw new NotImplementedException();
        }
    }
}
