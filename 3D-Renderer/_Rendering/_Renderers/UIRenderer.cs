using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._GameObject;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using _3D_Renderer._Renderable._UIElement;

namespace _3D_Renderer._Rendering._Renderers
{
    /// <summary>
    /// Renders <see cref="UIElement"/>(s)
    /// </summary>
    internal class UIRenderer : Renderer
    {
        public override void RenderCollection(Collection collection, Matrix4 projectionMatrix,
            Matrix4 cameraMatrix)
        {
            foreach(Renderable renderable in collection.renderables)
            {
                int tris = renderable.ApplyRenderable(projectionMatrix, cameraMatrix, 
                    out bool meshIsWireframe);
                //UI elements should never be wireframe
                if (meshIsWireframe)
                {
                    throw new Exception("UIRenderer does not support wireframe meshes!");
                }

                GL.DrawElements(PrimitiveType.Triangles, tris,
                    DrawElementsType.UnsignedInt, 0);

                Program.window.renderStats.NewDrawCall(tris);
            }
        }
    }
}
