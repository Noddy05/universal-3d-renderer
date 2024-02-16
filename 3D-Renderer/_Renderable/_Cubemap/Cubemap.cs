using _3D_Renderer._Generation;
using _3D_Renderer._Renderable._GameObject;
using _3D_Renderer._Shading;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable._Cubemap
{
    internal class Cubemap : Renderable
    {
        public string name = "";
        private Mesh mesh;
        private Material material;

        private int UL_projectionMatrix = -1;
        private int UL_cameraMatrix = -1;

        public void SetMaterial(Material material)
        {
            this.material = material;
            UL_projectionMatrix = GL.GetUniformLocation(material.shader, "projectionMatrix");
            UL_cameraMatrix = GL.GetUniformLocation(material.shader, "cameraMatrix");
        }

        public Cubemap()
        {
            mesh = MeshGeneration.SmoothCube();
            mesh.FlipFaces();
        }

        //For rendering: (returns number of indices)
        public override int ApplyRenderable(Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            //Bind material, mesh and transformation matrix
            material.ApplyMaterial();
            mesh.Bind();

            //Bind camera and projection matrix
            Matrix4 camMatrix = cameraMatrix;
            Matrix4 projMatrix = projectionMatrix;
            GL.UniformMatrix4(UL_cameraMatrix, false, ref camMatrix);
            GL.UniformMatrix4(UL_projectionMatrix, false, ref projMatrix);
            return mesh.IndicesCount();
        }
    }
}
