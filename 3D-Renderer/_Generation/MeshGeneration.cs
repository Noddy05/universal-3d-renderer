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
                // Right   // Left
                1, 2, 6,   0, 4, 7, 
                6, 5, 1,   7, 3, 0,
                // Top     // Bottom
                6, 7, 4,   0, 3, 2, 
                5, 6, 4,   1, 0, 2,
                // Back    // Front
                1, 5, 0,   6, 3, 7, 
                5, 4, 0,   3, 6, 2, 
            ];

            Mesh cube = new Mesh();
            cube.SetVertices(vertices, BufferUsageHint.StaticDraw);
            cube.SetIndices(indices, BufferUsageHint.StaticDraw);

            return cube;
        }

        #endregion

        #region More Complex Generations
        public static Mesh Circle(int vertices, bool centerVertex = false)
        {
            //TODO add indices:

            if (vertices < 3)
                throw new Exception("Trying to generate circle mesh failed! " +
                    "(MeshGeneration.CircleMesh(a) requires a ≥ 3)");

            int centerVertexI = (centerVertex ? 1 : 0);
            Vertex[] verts = new Vertex[vertices + centerVertexI];
            #region Vertices
            for (int i = 0; i < vertices; i++)
            {
                float px = i / (float)vertices * 2 * MathF.PI;
                Vector3 pos = new Vector3(MathF.Cos(px), 0, MathF.Sin(px));
                verts[i + centerVertexI] = new Vertex(pos, Vector3.UnitY, 
                    new Vector2(pos.X, pos.Z));
            }
            if (centerVertex)
            {
                verts[0] = new Vertex(Vector3.Zero, Vector3.UnitY, Vector2.Zero);
            }
            #endregion
            int[] indices;
            #region Indices
            if(centerVertex)
            {
                indices = new int[3 * vertices];
                for(int i = 0; i < vertices; i++)
                {
                    indices[3 * i + 0] = 0;
                    indices[3 * i + 1] = i + 1;
                    if(i + 1 >= vertices)
                    {
                        indices[3 * i + 2] = 1;
                    } 
                    else
                    {
                        indices[3 * i + 2] = i + 2;
                    }
                }
            } 
            else
            {
                indices = new int[3 * (vertices - 2)];
                for (int i = 0; i < vertices - 2; i++)
                {
                    indices[3 * i + 0] = 0;
                    indices[3 * i + 1] = i + 2;
                    indices[3 * i + 2] = i + 1;
                }
            }

            #endregion
            Mesh circle = new Mesh();
            circle.SetVertices(verts, BufferUsageHint.StaticDraw);
            circle.SetIndices(indices, BufferUsageHint.StaticDraw);

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
                        new Vector3(0, 1, 0), new Vector2(vx + 0.5f, vy + 0.5f));
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

        public static Mesh Cube(int divisions)
        {
            Mesh bottom = Plane(divisions, divisions);
            bottom.FlipFacesUVsNormals();
            bottom.PermanentlyTransformVertices(Matrix4.CreateTranslation(0, -0.5f, 0));
            Mesh top = Plane(divisions, divisions);
            top.PermanentlyTransformVertices(Matrix4.CreateTranslation(0, 0.5f, 0));
            Mesh combinedSideMesh = new Mesh();
            Matrix4 sideTranslation = Matrix4.CreateTranslation(0, 0.5f, 0);
            Matrix4 sideRotation = Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(MathF.PI / 2, 0, 0));
            Matrix4 sideRotationOffset = Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(0, MathF.PI / 2, 0));
            for(int i = 0; i < 4; i++)
            {
                Mesh side = Plane(divisions, divisions);
                side.PermanentlyTransformVertices(sideTranslation * sideRotation);
                side.PermanentlyTransformNormals(sideRotation);
                sideRotation *= sideRotationOffset;
                combinedSideMesh += side;
            }

            return bottom + top + combinedSideMesh;
        }

        public static Mesh CubeSphere(int divisions, float sphericalNess = 1)
        {
            Mesh bottom = Plane(divisions, divisions);
            /*bottom.PermanentlyTransformVertices(Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(MathF.PI, 0, 0)));*/
            bottom.FlipFaces();
            bottom.FlipUVs();
            bottom.PermanentlyTransformVertices(Matrix4.CreateTranslation(0, -0.5f, 0));
            bottom.PermanentlyTransformNormals(Matrix4.CreateScale(1, -1, 1));
            Mesh top = Plane(divisions, divisions);
            top.PermanentlyTransformVertices(Matrix4.CreateTranslation(0, 0.5f, 0));
            Mesh combinedSideMesh = new Mesh();
            Matrix4 sideTranslation = Matrix4.CreateTranslation(0, 0.5f, 0);
            Matrix4 sideRotation = Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(MathF.PI / 2, 0, 0));
            Matrix4 sideRotationOffset = Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(0, MathF.PI / 2, 0));
            for (int i = 0; i < 4; i++)
            {
                Mesh side = Plane(divisions, divisions);
                side.PermanentlyTransformVertices(sideTranslation * sideRotation);
                side.PermanentlyTransformNormals(sideRotation);
                sideRotation *= sideRotationOffset;
                combinedSideMesh += side;
            }
            Mesh combinedMesh = bottom + top + combinedSideMesh;
            Vertex[] combinedVertices = combinedMesh.GetVertices();
            Vertex[] modifiedVertices = new Vertex[combinedVertices.Length];
            for (int i = 0; i < combinedVertices.Length; i++)
            {
                Vector3 normalizedPosition = combinedVertices[i].vertexPosition.Normalized();
                Vector3 finalizedPosition = Vector3.Lerp(combinedVertices[i].vertexPosition,
                    normalizedPosition, sphericalNess);

                modifiedVertices[i] = new Vertex(finalizedPosition,
                    finalizedPosition, combinedVertices[i].textureCoordinate);
            }
            combinedMesh.SetVertices(modifiedVertices, BufferUsageHint.StaticDraw);

            return combinedMesh;
        }

        public static Mesh[] Text(int fontHandle, string text, out float lengthOfText)
        {
            Font font = FontLoader.GetFont(fontHandle);
            char[] characters = text.ToCharArray();
            Mesh[] textMesh = new Mesh[font.textureAtlasHandle.Length];
            float xCursor = 0;
            for (int i = 0; i < characters.Length; i++)
            {
                FontCharacter characterInfo = font.GetCharacterInfo(characters[i]);
                int meshIndex = characterInfo.page;

                Mesh characterQuad = FontQuad(
                    (characterInfo.xOffset + xCursor) / (float)font.width, 
                    characterInfo.yOffset / (float)font.height,
                    characterInfo.width / (float)font.width, characterInfo.height / (float)font.height, 
                    characterInfo.x / (float)font.width,     characterInfo.y / (float)font.height);
                xCursor += characterInfo.xAdvance;

                if (textMesh[meshIndex] == null)
                {
                    textMesh[meshIndex] = characterQuad;
                    continue;
                }
                textMesh[meshIndex] += characterQuad;
            }
            lengthOfText = xCursor / font.width;
            for(int i = 0; i < textMesh.Length; i++) { 
                textMesh[i] -= new Vector3(lengthOfText / 2f, 0, 0);
                textMesh[i].name = text;
            }

            return textMesh;
        }
        #endregion
    }
}
