using _3D_Renderer._Generation;
using _3D_Renderer._Import;
using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._GameObject;
using _3D_Renderer._Shading._Materials;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Rendering
{
    internal static class WireframeRenderer
    {
        private static GameObject? wireframeBox;
        private static GameObject? wireframeRay;
        private static GameObject? wireframeSphere;
        private static Window? window;

        /// <summary>
        /// Sets up all the wireframe models needed to draw wireframes.
        /// </summary>
        static WireframeRenderer()
        {
            UnlitMaterial material = new UnlitMaterial(Color4.Blue);
            window = Program.GetWindow();

            //Box:
            wireframeBox = new GameObject();
            wireframeBox.SetMesh(WireframeGeneration.SmoothCube());
            wireframeBox.SetMaterial(material);

            //Sphere:
            wireframeSphere = new GameObject();
            Mesh fullCircleMesh = WireframeGeneration.Circle(64);
            Mesh circle = WireframeGeneration.Circle(64);
            circle.PermanentlyTransformVertices(Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(new Vector3(MathF.PI / 2f, 0, 0))));
            fullCircleMesh += circle;
            circle.PermanentlyTransformVertices(Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(new Vector3(0, MathF.PI / 2f, 0))));
            fullCircleMesh += circle;
            wireframeSphere.SetMesh(fullCircleMesh);
            wireframeSphere.SetMaterial(material);

            wireframeRay = new GameObject();
            Mesh rayMesh = new Mesh();
            rayMesh.SetVertices([new Vertex(new Vector3(0, 0, 0)),
                new Vertex(new Vector3(1, 0, 0))], BufferUsageHint.StaticDraw);
            rayMesh.SetIndices([0, 1], BufferUsageHint.StaticDraw);
            wireframeRay.SetMesh(rayMesh);
            wireframeRay.SetMaterial(material);

        }

        public static void RenderRay(Vector3 position, Vector3 eulerAngles, 
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            RenderRay(position, eulerAngles, projectionMatrix, cameraMatrix, Color4.Blue);
        }
        public static void RenderRay(Vector3 position, Vector3 eulerAngles,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix, Color4 color)
        {
            wireframeRay!.transform.rotation = eulerAngles;
            RenderWireframeObject(wireframeRay, position,
                Vector3.One, projectionMatrix, cameraMatrix, color);
        }
        public static void RenderRay(Matrix4 transformationMatrix,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix, Color4 color)
        {
            RenderWireframeObject(wireframeRay, transformationMatrix,
                projectionMatrix, cameraMatrix, color);
        }
        public static void RenderLine(Vector3 from, Vector3 to,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix, Color4 color)
        {
            Mesh rayMesh = wireframeRay!.GetMesh()!;
            rayMesh.UpdateVBO([new Vertex(), new Vertex(to - from)]);
            RenderWireframeObject(wireframeRay, from, Vector3.One,
                projectionMatrix, cameraMatrix, color);
        }
        public static void RenderDirection(Vector3 from, Vector3 direction,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix, Color4 color)
        {
            Mesh rayMesh = wireframeRay!.GetMesh()!;
            rayMesh.UpdateVBO([new Vertex(), new Vertex(direction)]);
            RenderWireframeObject(wireframeRay, from, Vector3.One,
                projectionMatrix, cameraMatrix, color);
        }

        public static void RenderWireframeBox(Vector3 position, Vector3 size,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            RenderWireframeBox(position, size, projectionMatrix, cameraMatrix, Color4.Blue);
        }
        /// <summary>
        /// Renders a wireframe box in a set position, with a set size.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        /// <exception cref="Exception"></exception>
        public static void RenderWireframeBox(Vector3 position, Vector3 size,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix, Color4 color)
        {
            RenderWireframeObject(wireframeBox, position, size, projectionMatrix, cameraMatrix, color);
        }

        public static void RenderWireframeSphere(Vector3 position, Vector3 size,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            RenderWireframeSphere(position, size, projectionMatrix, cameraMatrix, Color4.Blue);
        }
        /// <summary>
        /// Renders a wireframe sphere in a set position, with a set radius.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        /// <exception cref="Exception"></exception>
        public static void RenderWireframeSphere(Vector3 position, Vector3 size,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix, Color4 color)
        {
            RenderWireframeObject(wireframeSphere, position, size, projectionMatrix, cameraMatrix, color);
        }

        /// <summary>
        /// Renders a specified wireframe mesh.
        /// </summary>
        /// <param name="renderable"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraMatrix"></param>
        private static void RenderWireframeObject(GameObject renderable, Vector3 position, Vector3 size,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix, Color4 color)
        {
            ((UnlitMaterial)renderable.GetMaterial()).color = color;
            renderable.transform.position = position;
            renderable.transform.scale = size;

            int tris = renderable.ApplyRenderable(projectionMatrix, cameraMatrix);

            GL.DrawElements(PrimitiveType.Lines, tris,
                DrawElementsType.UnsignedInt, 0);

            window!.renderStats.NewDrawCall(0);
        }
        private static void RenderWireframeObject(GameObject renderable, Matrix4 transformMatrix, 
            Matrix4 projectionMatrix, Matrix4 cameraMatrix, Color4 color)
        {
            ((UnlitMaterial)renderable.GetMaterial()).color = color;
            int tris = renderable.ApplyRenderable(transformMatrix, projectionMatrix, cameraMatrix);

            GL.DrawElements(PrimitiveType.Lines, tris,
                DrawElementsType.UnsignedInt, 0);

            window!.renderStats.NewDrawCall(0);
        }
    }
}
