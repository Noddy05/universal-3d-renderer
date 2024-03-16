using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._GameObject;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using _3D_Renderer._Renderable._Cubemap;
using _3D_Renderer._SceneHierarchy;
using _3D_Renderer._GLObjects._UBO._UniformBlocks;
using _3D_Renderer._GLObjects._UBO;
using _3D_Renderer._Shading;

namespace _3D_Renderer._Rendering._Renderers
{
    /// <summary>
    /// Renders basic <see cref="GameObject"/>(s) and <see cref="Cubemap"/>(s)
    /// </summary>
    internal class ShadowMapRenderer
    {
        private Window window;
        private Material shadowMaterial;
        private int UL_transformationMatrix = -1;
        private int UL_projectionMatrix = -1;
        private Material shadowMaterialInstanced;
        private int UL_instancedTransformationMatrix = -1;
        private int UL_instancedProjectionMatrix = -1;

        private Matrix4 projectionMatrix;
        private Matrix4 lightMatrix;

        public ShadowMapRenderer() {
            window = Program.GetWindow();

            //Hoping to find a better solution to instancing
            Shader shadowShader = new Shader(
                @"../../../_Assets/_Built-In/_Shaders/_Shadow/shadow.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Shadow/shadow.frag");
            UL_transformationMatrix = GL.GetUniformLocation(shadowShader, "transformationMatrix");
            UL_projectionMatrix = GL.GetUniformLocation(shadowShader, "projectionMatrix");
            shadowMaterial = new Material(shadowShader);

            Shader instancedShadowShader = new Shader(
                @"../../../_Assets/_Built-In/_Shaders/_Shadow/shadow_instanced.vert",
                @"../../../_Assets/_Built-In/_Shaders/_Shadow/shadow.frag");
            UL_instancedTransformationMatrix = GL.GetUniformLocation(
                instancedShadowShader, "transformationMatrix");
            UL_projectionMatrix = GL.GetUniformLocation(instancedShadowShader, "projectionMatrix");
            shadowMaterialInstanced = new Material(instancedShadowShader);
        }

        public void RenderCollection(string collection, DirectionalLight directionalLight)
        {
            float distance = 100;
            float width  = 50;
            float height = 50 * window.Size.Y / (float)window.Size.X;

            projectionMatrix = Matrix4.CreateOrthographic(width, height, 0f, distance * 2f);
            //lightMatrix = directionalLight.CalculateRotationMatrix()
            //    * Matrix4.CreateTranslation(-Vector3.UnitZ * distance);
            lightMatrix = Matrix4.Identity;

            //Should have frustum culling
            GL.Disable(EnableCap.CullFace);
            foreach (Renderable renderable in SceneHierarchy.GetCollection(collection))
            {
                Mesh mesh = renderable.GetMesh()!;
                //If no mesh, this should be skipped
                if (mesh == null)
                    continue;

                Program.GetWindow().GetDefaultMaterial().ApplyMaterial();

                int tris = mesh.GetIndices().Length;
                if (tris <= 0)
                    continue;

                int instanceCount = mesh.InstanceCount();
                if (instanceCount > 1)
                {
                    //Draw instanced:
                    ApplyInstancedRenderable(mesh, projectionMatrix, lightMatrix);

                    GL.DrawElementsInstanced(PrimitiveType.Triangles, tris,
                        DrawElementsType.UnsignedInt, 0, instanceCount);
                } 
                else
                {
                    //Draw regular:
                    ApplyRenderable(mesh, projectionMatrix,
                        renderable.transform.TransformationMatrix() * lightMatrix);

                    GL.DrawElements(PrimitiveType.Triangles, tris,
                        DrawElementsType.UnsignedInt, 0);
                }

                window.renderStats.NewDrawCall(tris * instanceCount);
            }
            GL.Enable(EnableCap.CullFace);
        }

        public void ApplyRenderable(Mesh mesh, Matrix4 projection, Matrix4 transformation)
        {
            //Apply shadow material
            shadowMaterial.ApplyMaterial();
            GL.UniformMatrix4(UL_projectionMatrix, false, ref projection);
            GL.UniformMatrix4(UL_transformationMatrix, false, ref transformation);
            mesh.Bind();
        }
        public void ApplyInstancedRenderable(Mesh mesh, Matrix4 projection, Matrix4 transformation)
        {
            //Apply shadow material
            shadowMaterialInstanced.ApplyMaterial();
            GL.UniformMatrix4(UL_instancedProjectionMatrix, false, ref projection);
            GL.UniformMatrix4(UL_instancedTransformationMatrix, false, ref transformation);
            mesh.Bind();
        }
    }
}
