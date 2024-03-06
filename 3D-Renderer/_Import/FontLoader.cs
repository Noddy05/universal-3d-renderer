using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Import
{
    internal class FontLoader
    {
        private static List<Font> fonts = new List<Font>();

        /// <summary>
        /// Sets up a new <see cref="Font"/>.
        /// </summary>
        /// <param name="fontAtlasPath"></param>
        /// <param name="fontMetaPath"></param>
        /// <returns>The font handle (only recognized by the <see cref="FontLoader"/>)</returns>
        public static int LoadFont(string fontMetaPath, params string[] fontAtlasPath)
        {
            int[] textureAtlases = new int[fontAtlasPath.Length];
            for(int i = 0; i < textureAtlases.Length; i++)
            {
                textureAtlases[i] = TextureLoader.LoadTexture(fontAtlasPath[i]);
            }
            string[] fontMeta = File.ReadAllLines(fontMetaPath);
            string name = SearchLines(fontMeta, "face=\"", "\"", out _)!;
            int size = int.Parse(SearchLines(fontMeta, "size=", " ", out _)!);
            int lineHeight = int.Parse(SearchLines(fontMeta, "lineHeight=", " ", out _)!);
            int atlasWidth = int.Parse(SearchLines(fontMeta, "scaleW=", " ", out _)!);
            int atlasHeight = int.Parse(SearchLines(fontMeta, "scaleH=", " ", out _)!);
            bool bold = Convert.ToBoolean(Convert.ToInt16(
                SearchLines(fontMeta, "bold=", " ", out _)));
            bool italic = Convert.ToBoolean(Convert.ToInt16(
                SearchLines(fontMeta, "italic=", " ", out _)));

            //Read and cache character info
            Dictionary<int, FontCharacter> characters = new Dictionary<int, FontCharacter>();
            for(int i = 0; i < fontMeta.Length; i++)
            {
                string line = fontMeta[i];
                if(line.Contains("char id="))
                {
                    int charId = int.Parse(ReadBetween(line, "char id=", " ", 0)!);
                    int x = int.Parse(ReadBetween(line, "x=", " ", 0)!);
                    int y = int.Parse(ReadBetween(line, "y=", " ", 0)!);
                    int width = int.Parse(ReadBetween(line, "width=", " ", 0)!);
                    int height = int.Parse(ReadBetween(line, "height=", " ", 0)!);
                    int xOffset = int.Parse(ReadBetween(line, "xoffset=", " ", 0)!);
                    int yOffset = int.Parse(ReadBetween(line, "yoffset=", " ", 0)!);
                    int xAdvance = int.Parse(ReadBetween(line, "xadvance=", " ", 0)!);
                    int page = int.Parse(ReadBetween(line, "page=", " ", 0)!);

                    FontCharacter fontChar = new FontCharacter(x, y,
                        width, height, xOffset, yOffset, xAdvance, page);
                    characters.Add(charId, fontChar);
                }
            }

            Font font = new Font(name, textureAtlases, bold, italic, size, 
                lineHeight, atlasWidth, atlasHeight);
            font.characters = characters;
            fonts.Add(font);

            return fonts.Count - 1;
        }

        public static Font GetFont(int fontHandle)
        {
            return fonts[fontHandle];
        }
        public static FontCharacter GetCharacterInfo(int fontHandle, int charAscii)
        {
            return fonts[fontHandle].GetCharacterInfo(charAscii);
        }
        public static FontCharacter GetCharacterInfo(int fontHandle, char charAscii)
        {
            return fonts[fontHandle].GetCharacterInfo(charAscii);
        }

        public static string? SearchLines(string[] lines, string beginning, string end, 
            out int foundOnLine)
        {
            string? output = null;
            int lineIndex = 0;
            foundOnLine = -1;
            while (output == null)
            {
                if (lineIndex > lines.Length)
                {
                    throw new Exception("Failed to find text!");
                }
                output = ReadBetween(lines[lineIndex], beginning, end, 0);
                foundOnLine = lineIndex;
                lineIndex++;
            }
            return output;
        }

        public static string? ReadBetween(string text, string beginning, string end, int startAt)
        {
            if (!text.Contains(beginning))
                return null;
            int startIndex = text.IndexOf(beginning, startAt) + beginning.Length;
            if (startIndex < 0 || !text.Contains(end))
                return null;
            int endIndex = text.IndexOf(end, startIndex);
            if (endIndex < 0)
                return null;
            int length = endIndex - startIndex;
            return text.Substring(startIndex, length);
        }
    }
}
