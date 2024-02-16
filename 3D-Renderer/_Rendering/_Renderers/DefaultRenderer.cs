using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._GameObject;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Rendering._Renderers
{
    internal class DefaultRenderer : Renderer
    {
        public override void RenderCollection(Collection collection, Matrix4 projectionMatrix,
            Matrix4 cameraMatrix)
        {
            foreach(GameObject gameObject in collection.renderables)
            {
                int tris = gameObject.ApplyRenderable(projectionMatrix, cameraMatrix);
                GL.DrawElements(PrimitiveType.Triangles, tris, 
                    DrawElementsType.UnsignedInt, 0);

                Program.window.renderStats.RenderCall(tris);
            }
        }
    }
}
