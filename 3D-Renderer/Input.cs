using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3D_Renderer
{
    internal static class Input
    {
        public static Dictionary<string, Keys> buttons = new Dictionary<string, Keys>();
        public static Dictionary<string, MouseButton> 
            mouseButtons = new Dictionary<string, MouseButton>();

        private static Window window;

        /// <summary>
        /// Automatically instantiates <see cref="Input"/> and allows for easy input reading.
        /// </summary>
        static Input()
        {
            window = Program.GetWindow();
            InitializeButtons();
        }

        private static void InitializeButtons()
        {
            buttons = new Dictionary<string, Keys>()
            {
                { "Jump", Keys.Space },
            };
            mouseButtons = new Dictionary<string, MouseButton>()
            {
                { "Left", MouseButton.Left },
                { "Right", MouseButton.Right },
            };
        }

        /// <summary>
        /// Returns WASD input or arrow keys
        /// </summary>
        /// <returns>
        /// <see cref="Vector2"/>(x, y) direction of the input.<br></br>
        /// x: -1 if A or Left is pressed | 1 if D or Right is pressed<br></br>
        /// y: -1 if S or Down is pressed | 1 if W or Up is pressed
        /// </returns>
        public static Vector2 Movement()
        {
            int x = 0;
            if (window.IsKeyDown(Keys.A) || window.IsKeyDown(Keys.Left))
                x--;
            if (window.IsKeyDown(Keys.D) || window.IsKeyDown(Keys.Right))
                x++;

            int y = 0;
            if (window.IsKeyDown(Keys.W) || window.IsKeyDown(Keys.Up))
                y++;
            if (window.IsKeyDown(Keys.S) || window.IsKeyDown(Keys.Down))
                y--;

            return new Vector2(x, y);
        }


        public static bool GetButton(string identifier) =>
            window.IsKeyDown(buttons[identifier]);
        public static bool GetButtonDown(string identifier) => 
            window.IsKeyPressed(buttons[identifier]);
        public static bool GetButtonUp(string identifier) => 
            window.IsKeyReleased(buttons[identifier]);


        public static bool GetKey(Keys identifier) =>
            window.IsKeyDown(identifier);
        public static bool GetKeyDown(Keys identifier) =>
            window.IsKeyPressed(identifier);
        public static bool GetKeyUp(Keys identifier) =>
            window.IsKeyReleased(identifier);


        public static bool GetMouseButton(string identifier) =>
            window.IsMouseButtonDown(mouseButtons[identifier]);
        public static bool GetMouseButtonDown(string identifier) =>
            window.IsMouseButtonPressed(mouseButtons[identifier]);
        public static bool GetMouseButtonUp(string identifier) =>
            window.IsMouseButtonReleased(mouseButtons[identifier]);


        public static bool GetMouseButton(MouseButton identifier) =>
            window.IsMouseButtonDown(identifier);
        public static bool GetMouseButtonDown(MouseButton identifier) =>
            window.IsMouseButtonPressed(identifier);
        public static bool GetMouseButtonUp(MouseButton identifier) =>
            window.IsMouseButtonReleased(identifier);
    }
}
