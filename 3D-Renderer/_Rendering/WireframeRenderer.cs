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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Rendering
{
    internal class WireframeRenderer
    {
        private static GameObject? wireframeBox;
        private static GameObject? wireframeSphere;
        private static Window? window;

        public static void Initialize()
        {
            UnlitMaterial material = new UnlitMaterial(Color4.Blue);
            wireframeBox = new GameObject();
            wireframeBox.SetMesh(WireframeGeneration.SmoothCube());
            wireframeBox.SetMaterial(material);

            wireframeSphere = new GameObject();
            Mesh fullCircleMesh = WireframeGeneration.Circle(64);
            Mesh circle = WireframeGeneration.Circle(64);
            circle.ApplyTransformation(Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(new Vector3(MathF.PI / 2f, 0, 0))));
            fullCircleMesh += circle;
            circle.ApplyTransformation(Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(new Vector3(0, MathF.PI / 2f, 0))));
            fullCircleMesh += circle;
            wireframeSphere.SetMesh(fullCircleMesh);
            wireframeSphere.SetMaterial(material);

            window = Program.GetWindow();
        }

        public static void RenderWireframeBox(Vector3 position, Vector3 size,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            if (wireframeBox == null)
                throw new Exception("WireframeRenderer's wireframeBox is null. Remember to" +
                    "initilize renderer with WireframeRenderer.Initialize()");

            RenderWireframeObject(wireframeBox, position, size, projectionMatrix, cameraMatrix);
        }
        public static void RenderWireframeSphere(Vector3 position, Vector3 size,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            if (wireframeSphere == null)
                throw new Exception("WireframeRenderer's wireframeSphere is null. Remember to" +
                    "initilize renderer with WireframeRenderer.Initialize()");

            RenderWireframeObject(wireframeSphere, position, size, projectionMatrix, cameraMatrix);
        }

        private static void RenderWireframeObject(GameObject renderable, Vector3 position, Vector3 size,
            Matrix4 projectionMatrix, Matrix4 cameraMatrix)
        {
            renderable.transform.position = position;
            renderable.transform.scale = size;

            int tris = renderable.ApplyRenderable(projectionMatrix, cameraMatrix);

            GL.DrawElements(PrimitiveType.Lines, tris,
                DrawElementsType.UnsignedInt, 0);

            window!.renderStats.NewDrawCall(tris);
        }
    }
}
