
using OpenTK.Mathematics;

namespace _3D_Renderer
{
    internal class Program
    {
        private static Window window;
        public static Window GetWindow() => window;

        static void Main(string[] args)
        {
            using (Window _window = new Window())
            {
                window = _window;

                window.Run();
            }

        }
    }
}
