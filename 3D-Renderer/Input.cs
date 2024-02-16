using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGL.GL;

namespace _3D_Renderer
{
    internal class Input
    {
        public static Dictionary<string, Keys> buttons = new Dictionary<string, Keys>();
        public static Dictionary<string, MouseButton> 
            mouseButtons = new Dictionary<string, MouseButton>();

        private static Window window;
        public static void Initialize()
        {
            window = Program.window;
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
