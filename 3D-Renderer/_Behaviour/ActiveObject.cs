using OpenTK.Windowing.Common;

namespace _3D_Renderer._Behaviour
{
    internal class ActiveObject
    {
        private Window window;

        public ActiveObject() {
            window = Program.GetWindow();
            window.RenderFrame += Update;
        }

        public virtual void Update(FrameEventArgs args)
        {

        }
    }
}
