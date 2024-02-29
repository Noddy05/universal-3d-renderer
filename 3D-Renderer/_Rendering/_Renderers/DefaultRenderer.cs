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
    internal class DefaultRenderer : Renderer
    {
        public DefaultRenderer()
        {

        }

        public override void RenderCollection(Collection collection, Camera camera,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            Frustum frustum = null;
            if (camera.GetType() == typeof(FreeCamera))
            {
                FreeCamera cam = (FreeCamera)camera;
                frustum = cam.CameraFrustum();
                frustum.frustumRotationMatrix = cam.RotationMatrix();
            }

            foreach (Renderable renderable in collection.renderables)
            {
                //If no mesh, this should be skipped
                if (renderable.GetMesh() == null)
                    continue;

                //Draw debug sphere around renderable
                if(renderable.cull && frustum != null)
                {
                    float longestScale = MathF.Max(MathF.Max(renderable.transform.scale.X,
                        renderable.transform.scale.Y), renderable.transform.scale.Z);
                    if (!frustum.Intersects(renderable.transform.position + camera.Position(),
                        renderable.GetMesh()!.GetBoundingRadius() * longestScale))
                    {
                        //Cull this object => skip this one
                        continue;
                    }
                }

                //Draw bounding box for renderable:
                DrawHitboxes(renderable, projectionMatrix, cameraMatrix);

                int tris = renderable.ApplyRenderable(projectionMatrix, cameraMatrix);
                if (tris <= 0) //If mesh is null skip this renderable.
                    continue;

                int instanceCount = renderable.GetMesh()!.InstanceCount();
                if (instanceCount > 1)
                {
                    //Draw instanced:
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, tris,
                        DrawElementsType.UnsignedInt, 0, instanceCount);
                } 
                else
                {
                    //Draw regular:
                    GL.DrawElements(PrimitiveType.Triangles, tris,
                        DrawElementsType.UnsignedInt, 0);
                }

                window.renderStats.NewDrawCall(tris * instanceCount);
            }
        }

        /// <summary>
        /// Calls the <see cref="WireframeRenderer"/> for drawing hitboxes.
        /// </summary>
        /// <param name="renderable"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        private void DrawHitboxes(Renderable renderable, Matrix4 projectionMatrix, 
            Matrix4 cameraMatrix)
        {
            //Draw bounding box for renderable:
            if (renderable.showBoundingBox)
            {
                renderable.CalculateBoundingBox(true);
                (Vector3 center, Vector3 size) boundingBox = renderable.GetMesh()!.GetBoundingBox();
                WireframeRenderer.RenderWireframeBox(boundingBox.center * renderable.transform.scale
                    + renderable.transform.position,
                    boundingBox.size * renderable.transform.scale,
                    projectionMatrix, cameraMatrix);

                WireframeRenderer.RenderWireframeSphere(renderable.transform.position,
                    renderable.GetMesh()!.GetBoundingRadius() * renderable.transform.scale,
                    projectionMatrix, cameraMatrix);
            }
        }
    }
}
