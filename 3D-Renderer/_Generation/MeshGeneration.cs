using _3D_Renderer._Import;
using _3D_Renderer._Renderable;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Generation
{
    internal class MeshGeneration
    {
        #region Generations of type O(1)
        public static Mesh Quad()
        {
            Mesh quad = new Mesh();

            //   0------1
            //   | \    |
            //   |    \ |
            //   3------2
            Vertex[] vertices = [
                new Vertex(new Vector3(-1,  1, 0), new Vector3(0, 0, 1), new Vector2(0, 1)), 
                new Vertex(new Vector3( 1,  1, 0), new Vector3(0, 0, 1), new Vector2(1, 1)), 
                new Vertex(new Vector3( 1, -1, 0), new Vector3(0, 0, 1), new Vector2(1, 0)), 
                new Vertex(new Vector3(-1, -1, 0), new Vector3(0, 0, 1), new Vector2(0, 0)), 
            ];
            int[] indices = [
                0, 2, 1,
                0, 3, 2,
            ];
            quad.SetVertices(vertices, BufferUsageHint.StaticDraw);
            quad.SetIndices(indices, BufferUsageHint.StaticDraw);

            return quad;
        }
        public static Mesh FontQuad(float xOffset, float yOffset, float width, 
            float height, float tx, float ty)
        {
            Mesh quad = new Mesh();

            //   0------1
            //   | \    |
            //   |    \ |
            //   3------2
            Vertex[] vertices = [
                new Vertex(new Vector3(xOffset, - yOffset, 0), 
                    new Vector3(0, 0, 1), new Vector2(tx, ty)),

                new Vertex(new Vector3(xOffset + width, -yOffset, 0), 
                    new Vector3(0, 0, 1), new Vector2(tx + width, ty)),

                new Vertex(new Vector3(xOffset + width, -yOffset - height, 0), 
                    new Vector3(0, 0, 1), new Vector2(tx + width, ty + height)),

                new Vertex(new Vector3(xOffset, -yOffset - height, 0), 
                    new Vector3(0, 0, 1), new Vector2(tx, ty + height)),
            ];
            int[] indices = [
                0, 2, 1,
                0, 3, 2,
            ];
            quad.SetVertices(vertices, BufferUsageHint.StaticDraw);
            quad.SetIndices(indices, BufferUsageHint.StaticDraw);

            return quad;
        }

        public static Mesh SmoothCube() //Smooth cube being a cube with shared vertices
        {
            //     7---------6
            //    /|        /|
            //   4-+-------5 |
            //   | |       | |
            //   | 3-------+-2
            //   |/        |/
            //   0---------1
            Vertex[] vertices =
            [
                new Vertex(new Vector3(-1.0f, -1.0f,  1.0f)),  
                new Vertex(new Vector3( 1.0f, -1.0f,  1.0f)),  
                new Vertex(new Vector3( 1.0f, -1.0f, -1.0f)),  
                new Vertex(new Vector3(-1.0f, -1.0f, -1.0f)),  
                new Vertex(new Vector3(-1.0f,  1.0f,  1.0f)),  
                new Vertex(new Vector3( 1.0f,  1.0f,  1.0f)),  
                new Vertex(new Vector3( 1.0f,  1.0f, -1.0f)),  
                new Vertex(new Vector3(-1.0f,  1.0f, -1.0f)),
            ];
            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i].vertexNormal = vertices[i].vertexPosition.Normalized();
            }

            int[] indices = [
                // Right
                1, 2, 6, 
                6, 5, 1, 
                // Left
                0, 4, 7, 
                7, 3, 0, 
                // Top
                6, 7, 4, 
                5, 6, 4, 
                // Bottom
                0, 3, 2, 
                1, 0, 2, 
                // Back
                1, 5, 0, 
                5, 4, 0, 
                // Front
                6, 3, 7, 
                3, 6, 2, 
            ];

            Mesh cube = new Mesh();
            cube.SetVertices(vertices, BufferUsageHint.StaticDraw);
            cube.SetIndices(indices, BufferUsageHint.StaticDraw);

            return cube;
        }

        #endregion

        #region Complex Generations
        public static Mesh Circle(int vertices)
        {
            //TODO add indices:

            if (vertices < 3)
                throw new Exception("Trying to generate circle mesh failed! " +
                    "(MeshGeneration.CircleMesh(a) requires a ≥ 3)");

            Mesh circle = new Mesh();

            Vertex[] verts = new Vertex[vertices];
            #region Vertices
            for (int i = 0; i < vertices; i++)
            {
                float px = i / (float)vertices * 2 * MathF.PI;
                Vector3 pos = new Vector3(MathF.Cos(px), 0, MathF.Sin(px));
                verts[i] = new Vertex(pos, new Vector3(0, 1, 0), 
                    new Vector2(pos.X, pos.Z));
            }
            #endregion
            #region Indices

            #endregion
            circle.SetVertices(verts, BufferUsageHint.StaticDraw);
            //circle.SetIndices(indices, BufferUsageHint.StaticCopy);

            return circle;
        }

        public static Mesh Plane(int xDivisions, int yDivisions)
        {
            if(xDivisions < 0 || yDivisions < 0)
                throw new Exception("Trying to generate plane mesh failed! " +
                    "(MeshGeneration.Plane(a, b) requires (a, b) ≥ 0)");

            Mesh plane = new Mesh();

            Vertex[] vertices = new Vertex[(xDivisions + 2) * (yDivisions + 2)];
            int[] indices = new int[(xDivisions + 1) * (yDivisions + 1) * 6];
            #region Vertices
            for (int y = 0; y < yDivisions + 2; y++)
            {
                for (int x = 0; x < xDivisions + 2; x++)
                {
                    float vx = x / (float)(xDivisions + 1) - 0.5f;
                    float vy = y / (float)(yDivisions + 1) - 0.5f;
                    vertices[x + (xDivisions + 2) * y] = new Vertex(
                        new Vector3(vx, 0, vy),
                        new Vector3(0, 1, 0), new Vector2(vx, vy));
                }
            }
            #endregion
            #region Indices
            for (int y = 0; y < yDivisions + 1; y++)
            {
                for (int x = 0; x < xDivisions + 1; x++)
                {
                    indices[(x + (xDivisions + 1) * y) * 6    ] = x     + (xDivisions + 2) * y;
                    indices[(x + (xDivisions + 1) * y) * 6 + 1] = x     + (xDivisions + 2) * (y + 1);
                    indices[(x + (xDivisions + 1) * y) * 6 + 2] = 1 + x + (xDivisions + 2) * y;
                    indices[(x + (xDivisions + 1) * y) * 6 + 3] = 1 + x + (xDivisions + 2) * y;
                    indices[(x + (xDivisions + 1) * y) * 6 + 4] = x     + (xDivisions + 2) * (y + 1);
                    indices[(x + (xDivisions + 1) * y) * 6 + 5] = 1 + x + (xDivisions + 2) * (y + 1);
                }
            }
            #endregion
            plane.SetVertices(vertices, BufferUsageHint.StaticDraw);
            plane.SetIndices(indices, BufferUsageHint.StaticDraw);

            return plane;
        }

        public static Mesh Text(int fontHandle, string text, out float lengthOfText)
        {
            Font font = FontLoader.GetFont(fontHandle);
            char[] characters = text.ToCharArray();
            Mesh textMesh = new Mesh();
            float xCursor = 0;
            for (int i = 0; i < characters.Length; i++)
            {
                FontCharacter characterInfo = font.GetCharacterInfo(characters[i]);
                Mesh characterQuad = FontQuad(
                    (characterInfo.xOffset + xCursor) / (float)font.width, 
                    characterInfo.yOffset / (float)font.height,
                    characterInfo.width / (float)font.width, characterInfo.height / (float)font.height, 
                    characterInfo.x / (float)font.width,     characterInfo.y / (float)font.height);
                xCursor += characterInfo.xAdvance;

                if (i == 0)
                {
                    textMesh = characterQuad;
                    continue;
                }
                textMesh += characterQuad;
            }
            lengthOfText = xCursor / font.width;
            textMesh -= new Vector3(lengthOfText / 2f, 0, 0);
            textMesh.name = text;

            return textMesh;
        }
        #endregion
    }
}
