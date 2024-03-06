using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Import
{
    internal class FontCharacter
    {
        public int x, y;
        public int width, height;
        public int xOffset, yOffset;
        public int xAdvance;
        public int page;

        public FontCharacter(int x, int y, int width, int height, int xOffset, 
            int yOffset, int xAdvance, int page)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.xAdvance = xAdvance;
            this.page = page;
        }

        public override string ToString()
        {
            return $"Font Character: \nTexCoords: ({x}, {y})\n" +
                $"Size: ({width}, {height})\nOffset: ({xOffset}, {yOffset})\n" +
                $"xAdvance: {xAdvance}\n" + $"Page: {page}";
        }
    }
}
