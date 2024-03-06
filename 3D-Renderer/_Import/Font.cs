using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Import
{
    internal class Font
    {
        public string name;
        public int[] textureAtlasHandle;
        public bool bold;
        public bool italic;
        public int size;
        public int lineHeight;
        public int width;
        public int height;
        public Dictionary<int, FontCharacter> characters = new Dictionary<int, FontCharacter>();

        public Font(string name, int[] textureAtlasHandle, bool bold, bool italic, int size, int lineHeight,
            int width, int height)
        {
            this.name = name;
            this.textureAtlasHandle = textureAtlasHandle;
            this.bold = bold;
            this.italic = italic;
            this.size = size;
            this.lineHeight = lineHeight;
            this.width = width;
            this.height = height;
        }
        public FontCharacter GetCharacterInfo(int charAscii)
        {
            return characters[charAscii];
        }
    }
}
