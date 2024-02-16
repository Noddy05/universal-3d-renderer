
namespace _3D_Renderer
{
    internal class Program
    {
        public static Window window { get; private set; }

        static void Main(string[] args)
        {

            using (Window _window = new Window())
            {
                window = _window;

                Input.Initialize();
                window.Run();
            }

        }
    }
}
