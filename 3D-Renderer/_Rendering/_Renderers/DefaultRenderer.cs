using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._GameObject;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using _3D_Renderer._Renderable._Cubemap;

namespace _3D_Renderer._Rendering._Renderers
{
    /// <summary>
    /// Renders basic <see cref="GameObject"/>(s) and <see cref="Cubemap"/>(s)
    /// </summary>
    internal class DefaultRenderer : Renderer
    {
        public override void RenderCollection(Collection collection, Matrix4 projectionMatrix,
            Matrix4 cameraMatrix)
        {
            foreach(Renderable renderable in collection.renderables)
            {
                int tris = renderable.ApplyRenderable(projectionMatrix, cameraMatrix, 
                    out bool meshIsWireframe);
                if(meshIsWireframe)
                {
                    GL.DrawElements(PrimitiveType.Lines, tris,
                        DrawElementsType.UnsignedInt, 0);
                } 
                else
                {
                    GL.DrawElements(PrimitiveType.Triangles, tris,
                        DrawElementsType.UnsignedInt, 0);
                }

                Program.window.renderStats.NewDrawCall(tris);
            }
        }
    }
}
