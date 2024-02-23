using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._GameObject;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using _3D_Renderer._Renderable._Cubemap;
using _3D_Renderer._Generation;
using _3D_Renderer._Shading._Materials;
using _3D_Renderer._Camera;
using _3D_Renderer._Geometry;
using System.Xml.Schema;
using _3D_Renderer._Shading;
using _3D_Renderer._Import;

namespace _3D_Renderer._Rendering._Renderers
{
    /// <summary>
    /// Renders basic <see cref="GameObject"/>(s) and <see cref="Cubemap"/>(s)
    /// </summary>
    internal class InstancedRenderer : Renderer
    {
        public InstancedRenderer()
        {

        }

        public void RenderInstancedObject(Renderable renderable, int instances,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            if (renderable.GetMesh() == null) //If mesh is null skip this renderable.
                return;

            int tris = renderable.ApplyRenderable(projectionMatrix, cameraMatrix);
            if (tris <= 0) //If triangles are none skip this renderable.
                return;

            GL.DrawElementsInstanced(PrimitiveType.Triangles,
                tris, DrawElementsType.UnsignedInt, IntPtr.Zero, instances);

            window.renderStats.NewDrawCall(tris);
        }

        public void RenderInstancedObject(Renderable renderable, Matrix4[] matrices,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            if (renderable.GetMesh() == null)
                return;

            int tris = renderable.ApplyRenderable(projectionMatrix, cameraMatrix);
            if (tris <= 0) //If mesh is null skip this renderable.
                return;

            GL.DrawElementsInstanced(PrimitiveType.Triangles,
                tris, DrawElementsType.UnsignedInt, IntPtr.Zero, matrices.Length);

            window.renderStats.NewDrawCall(tris);
        }
    }
}
