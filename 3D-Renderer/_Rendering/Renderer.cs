using _3D_Renderer._Camera;
using _3D_Renderer._Geometry;
using _3D_Renderer._Renderable;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Rendering
{
    internal class Renderer
    {
        protected Window window;
        public Renderer() {
            window = Program.GetWindow();
        }

        /// <summary>
        /// Renders a collection of <see cref="Renderable"></see>s
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void RenderCollection(string collection, Camera camera,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders a single <see cref="Renderable"></see>
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void RenderObject(Renderable renderable, Camera camera,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            throw new NotImplementedException();
        }
    }
}
