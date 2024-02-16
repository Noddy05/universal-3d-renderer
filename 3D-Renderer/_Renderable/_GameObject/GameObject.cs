using _3D_Renderer._Camera;
using _3D_Renderer._Shading;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable._GameObject
{
    /// <summary>
    /// This is an collection of everything a renderable should contain
    /// </summary>
    internal class GameObject : Renderable
    {
        public string name;
        public Transform transform;
        public Mesh mesh;
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

        public GameObject()
        {
            transform = new Transform();
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
            Matrix4 camMatrix = cameraMatrix;
            Matrix4 projMatrix = projectionMatrix;
            GL.UniformMatrix4(UL_cameraMatrix, false, ref camMatrix);
            GL.UniformMatrix4(UL_projectionMatrix, false, ref projMatrix);

            return mesh.IndicesCount();
        }

        public GameObject Clone()
        {
            GameObject obj = new GameObject();
            obj.transform = new Transform();
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.transform.size = transform.size;

            obj.SetMaterial(material);
            obj.mesh = mesh;
            obj.name = name + " (Clone)";

            return obj;
        }
    }
}
