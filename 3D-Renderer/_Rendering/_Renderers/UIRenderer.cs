using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._GameObject;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using _3D_Renderer._Renderable._UIElement;
using _3D_Renderer._Camera;

namespace _3D_Renderer._Rendering._Renderers
{
    /// <summary>
    /// Renders <see cref="UIElement"/>(s)
    /// </summary>
    internal class UIRenderer : Renderer
    {
        public override void RenderCollection(Collection collection, Camera camera, 
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            foreach(Renderable renderable in collection.renderables)
            {
                int tris = renderable.ApplyRenderable(projectionMatrix, cameraMatrix);

                GL.DrawElements(PrimitiveType.Triangles, tris,
                    DrawElementsType.UnsignedInt, 0);

                window.renderStats.NewDrawCall(tris);
            }
        }
    }
}
