using _3D_Renderer._Camera;
using _3D_Renderer._Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Editor
{
    internal class EditorUI
    {
        private FreeCamera editorCamera;
        public EditorUI(FreeCamera _editorCamera)
        {
            editorCamera = _editorCamera;
        }

        public void DrawOrientation()
        {
            float distanceFromCamera = 15f;
            Vector3 cursorOrigin = -editorCamera.Position() - editorCamera.Forward() * distanceFromCamera;
            WireframeRenderer.RenderRay(cursorOrigin, new Vector3(1, 0, 0),
                editorCamera.GetProjectionMatrix(), editorCamera.CameraMatrix(), Color4.Red);

            WireframeRenderer.RenderRay(cursorOrigin, new Vector3(0, 1, 0),
                editorCamera.GetProjectionMatrix(), editorCamera.CameraMatrix(), Color4.Lime);

            WireframeRenderer.RenderRay(cursorOrigin, new Vector3(0, 0, 1),
                editorCamera.GetProjectionMatrix(), editorCamera.CameraMatrix(), Color4.Blue);
        }
    }
}
