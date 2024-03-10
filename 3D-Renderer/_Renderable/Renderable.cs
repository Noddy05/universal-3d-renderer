using _3D_Renderer._Behaviour;
using _3D_Renderer._Renderable._GameObject;
using _3D_Renderer._Shading;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Renderable
{
    /// <summary>
    /// This should probably be changed into two classes:
    /// Renderable and ActiveRenderable. Since static renderables dont need to 
    /// change their bounding box, etc...
    /// </summary>
    internal abstract class Renderable : ActiveObject
    {
        public bool cull = true;
        public string name = "";
        public bool showBoundingBox = false;
        public RenderableTransform transform;
        protected Mesh? mesh;
        public Mesh? GetMesh() => mesh;
        protected Material material;
        public virtual void SetMaterial(Material material) { }
        public Material GetMaterial() => material;

        public Renderable(RenderableTransform transform)
        {
            material = Program.GetWindow().GetDefaultMaterial();
            this.transform = transform;
            CalculateBoundingBox(false);
        }

        public void SetMesh(Mesh newMesh)
        {
            mesh = newMesh;
            CalculateBoundingBox(false);
        }

        /// <summary>
        /// Prepares the renderable for the renderer.
        /// </summary>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual int ApplyRenderable(Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            throw new NotImplementedException();
        }
        public virtual int ApplyRenderable(Matrix4 transformation, 
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            throw new NotImplementedException();
        }

        public void CalculateBoundingBox(bool recalculateOnlyIfRotationChanged)
        {
            if (mesh == null)
                return;
            //Bounding box hasn't changed:
            if (recalculateOnlyIfRotationChanged 
                && !transform.RotationChangedSinceLastCheck())
                return;

            boundingBox = mesh.CalculateBoundingBox(transform.RotationMatrix(), mesh.GetVertices());
        }

        protected (Vector3 center, Vector3 size) boundingBox;
        public (Vector3 center, Vector3 size) GetBoundingBox() => boundingBox;
    }
}
