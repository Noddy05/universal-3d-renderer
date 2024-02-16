using _3D_Renderer._Renderable._GameObject;
using _3D_Renderer._Shading;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable._UIElement
{
    internal class UIElement : Renderable
    {
        public string name;
        public UITransform transform;
        private Mesh mesh;
        private Material material;

        private int UL_transformationMatrix = -1;
        private int UL_projectionMatrix = -1;
        private int UL_cameraMatrix = -1;

        public void SetMaterial(Material material)
        {
            this.material = material;
            UL_transformationMatrix = GL.GetUniformLocation(material.shader, "transformationMatrix");
            UL_projectionMatrix = GL.GetUniformLocation(material.shader, "projectionMatrix");
            UL_cameraMatrix = GL.GetUniformLocation(material.shader, "cameraMatrix");
        }

        public UIElement()
        {
            transform = new UITransform();

            Vertex[] vertices = [
                new Vertex(new Vector3(-1,  1, 0), new Vector3(0, 0, 1), new Vector2(0, 1)), //  0------1
                new Vertex(new Vector3( 1,  1, 0), new Vector3(0, 0, 1), new Vector2(1, 1)), //  | \    |
                new Vertex(new Vector3( 1, -1, 0), new Vector3(0, 0, 1), new Vector2(1, 0)), //  |    \ |
                new Vertex(new Vector3(-1, -1, 0), new Vector3(0, 0, 1), new Vector2(0, 0)), //  3------2
            ];
            int[] indices = [
                0, 2, 1,
                0, 3, 2,
            ];
            mesh = new Mesh();
            mesh.SetVertices(vertices, BufferUsageHint.StaticCopy);
            mesh.SetIndices(indices, BufferUsageHint.StaticCopy);
        }

        //For rendering: (returns number of indices)
        public override int ApplyRenderable(Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            //Bind material, mesh and transformation matrix
            material.ApplyMaterial();
            mesh.Bind();
            Matrix4 transformationMatrix = transform.TransformationMatrix();
            GL.UniformMatrix4(UL_transformationMatrix, false, ref transformationMatrix);

            //Bind camera and projection matrix
            Matrix4 camMatrix = Matrix4.Identity;
            Matrix4 projMatrix = projectionMatrix;
            GL.UniformMatrix4(UL_cameraMatrix, false, ref camMatrix);
            GL.UniformMatrix4(UL_projectionMatrix, false, ref projMatrix);

            return mesh.IndicesCount();
        }
    }
}
