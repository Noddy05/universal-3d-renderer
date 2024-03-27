using _3D_Renderer._Import;
using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._Mesh;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Generation
{
    internal class MeshGeneration
    {
        #region Generations of type O(1)
        private static MeshBuilder QuadBuilder()
        {
            MeshBuilder quad = new MeshBuilder();

            //   0------1
            //   | \    |
            //   |    \ |
            //   3------2
            Vertex[] vertices = [
                new Vertex(new Vector3(-1, 1, 0), new Vector3(0, 0, 1), new Vector2(0, 1)),
                new Vertex(new Vector3(1, 1, 0), new Vector3(0, 0, 1), new Vector2(1, 1)),
                new Vertex(new Vector3(1, -1, 0), new Vector3(0, 0, 1), new Vector2(1, 0)),
                new Vertex(new Vector3(-1, -1, 0), new Vector3(0, 0, 1), new Vector2(0, 0)),
            ];
            int[] indices = [
                0,
                2,
                1,
                0,
                3,
                2,
            ];
            quad.SetVertices(vertices);
            quad.SetIndices(indices);

            return quad;
        }
        /// <summary>
        /// Generates a quad.
        /// </summary>
        /// <returns></returns>
        public static Mesh Quad()
            => QuadBuilder().Build(BufferUsageHint.StaticDraw);

        private static MeshBuilder FontQuadBuilder(float xOffset, float yOffset, float width,
            float height, float tx, float ty)
        {
            MeshBuilder quad = new MeshBuilder();

            //   0------1
            //   | \    |
            //   |    \ |
            //   3------2
            Vertex[] vertices = [
                new Vertex(new Vector3(xOffset, -yOffset, 0),
                    new Vector3(0, 0, 1), new Vector2(tx, ty)),

                new Vertex(new Vector3(xOffset + width, -yOffset, 0),
                    new Vector3(0, 0, 1), new Vector2(tx + width, ty)),

                new Vertex(new Vector3(xOffset + width, -yOffset - height, 0),
                    new Vector3(0, 0, 1), new Vector2(tx + width, ty + height)),

                new Vertex(new Vector3(xOffset, -yOffset - height, 0),
                    new Vector3(0, 0, 1), new Vector2(tx, ty + height)),
            ];
            int[] indices = [
                0,
                2,
                1,
                0,
                3,
                2,
            ];
            quad.SetVertices(vertices);
            quad.SetIndices(indices);

            return quad;
        }
        /// <summary>
        /// Generates a quad, used for generating text meshes.
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <returns></returns>
        public static Mesh FontQuad(float xOffset, float yOffset, float width,
            float height, float tx, float ty)
            => FontQuadBuilder(xOffset, yOffset, width, height, tx, ty).Build(BufferUsageHint.StaticDraw);

        private static MeshBuilder SmoothCubeBuilder()
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

            MeshBuilder cube = new MeshBuilder();
            cube.SetVertices(vertices);
            cube.SetIndices(indices);

            return cube;
        }
        /// <summary>
        /// Generates a cube with soft edges.
        /// </summary>
        /// <returns></returns>
        public static Mesh SmoothCube() //Smooth cube being a cube with shared vertices
            => SmoothCubeBuilder().Build(BufferUsageHint.StaticDraw);

        #endregion

        #region More Complex Generations
        private static MeshBuilder CircleBuilder(int vertices, bool centerVertex = false)
        {
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
            if (centerVertex)
            {
                indices = new int[3 * vertices];
                for (int i = 0; i < vertices; i++)
                {
                    indices[3 * i + 0] = 0;
                    indices[3 * i + 1] = i + 1;
                    if (i + 1 >= vertices)
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
            MeshBuilder circle = new MeshBuilder();
            circle.SetVertices(verts);
            circle.SetIndices(indices);

            return circle;
        }
        /// <summary>
        /// Creates a polygon with n-vertices.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="centerVertex"></param>
        /// <returns></returns>
        public static Mesh Circle(int vertices, bool centerVertex = false)
            => CircleBuilder(vertices, centerVertex).Build(BufferUsageHint.StaticDraw);

        private static MeshBuilder PlaneBuilder(int xDivisions, int yDivisions)
        {
            if (xDivisions < 0 || yDivisions < 0)
                throw new Exception("Trying to generate plane mesh failed! " +
                    "(MeshGeneration.Plane(a, b) requires (a, b) ≥ 0)");

            MeshBuilder plane = new MeshBuilder();

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
                    indices[(x + (xDivisions + 1) * y) * 6] = x + (xDivisions + 2) * y;
                    indices[(x + (xDivisions + 1) * y) * 6 + 1] = x + (xDivisions + 2) * (y + 1);
                    indices[(x + (xDivisions + 1) * y) * 6 + 2] = 1 + x + (xDivisions + 2) * y;
                    indices[(x + (xDivisions + 1) * y) * 6 + 3] = 1 + x + (xDivisions + 2) * y;
                    indices[(x + (xDivisions + 1) * y) * 6 + 4] = x + (xDivisions + 2) * (y + 1);
                    indices[(x + (xDivisions + 1) * y) * 6 + 5] = 1 + x + (xDivisions + 2) * (y + 1);
                }
            }
            #endregion
            plane.SetVertices(vertices);
            plane.SetIndices(indices);

            return plane;
        }
        /// <summary>
        /// Generates a plane with subdivisions on the x- and y-axis.
        /// </summary>
        /// <param name="xDivisions"></param>
        /// <param name="yDivisions"></param>
        /// <returns></returns>
        public static Mesh Plane(int xDivisions, int yDivisions)
            => PlaneBuilder(xDivisions, yDivisions).Build(BufferUsageHint.StaticDraw);

        private static MeshBuilder CubeBuilder(int divisions)
        {
            MeshBuilder bottom = PlaneBuilder(divisions, divisions);
            bottom.FlipFacesUVsNormals();
            bottom.PermanentlyTransformVertices(Matrix4.CreateTranslation(0, -0.5f, 0));
            MeshBuilder top = PlaneBuilder(divisions, divisions);
            top.PermanentlyTransformVertices(Matrix4.CreateTranslation(0, 0.5f, 0));
            MeshBuilder combinedSideMesh = new MeshBuilder();
            Matrix4 sideTranslation = Matrix4.CreateTranslation(0, 0.5f, 0);
            Matrix4 sideRotation = Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(MathF.PI / 2, 0, 0));
            Matrix4 sideRotationOffset = Matrix4.CreateFromQuaternion(
                Quaternion.FromEulerAngles(0, MathF.PI / 2, 0));
            for (int i = 0; i < 4; i++)
            {
                MeshBuilder side = PlaneBuilder(divisions, divisions);
                side.PermanentlyTransformVertices(sideTranslation * sideRotation);
                side.PermanentlyTransformNormals(sideRotation);
                sideRotation *= sideRotationOffset;
                combinedSideMesh += side;
            }

            return bottom + top + combinedSideMesh;
        }
        /// <summary>
        /// Generates a cube with hard edges and subdivisions on the sides.
        /// </summary>
        /// <param name="divisions"></param>
        /// <returns></returns>
        public static Mesh Cube(int divisions)
            => CubeBuilder(divisions).Build(BufferUsageHint.StaticDraw);

        private static MeshBuilder CubeSphereBuilder(int divisions, float sphericalNess = 1)
        {
            MeshBuilder combinedMesh = CubeBuilder(divisions);
            Vertex[] combinedVertices = combinedMesh.GetVertices();
            for (int i = 0; i < combinedVertices.Length; i++)
            {
                Vector3 normalizedPosition = combinedVertices[i].vertexPosition.Normalized();
                Vector3 finalizedPosition = Vector3.Lerp(combinedVertices[i].vertexPosition,
                    normalizedPosition, sphericalNess);

                combinedVertices[i] = new Vertex(finalizedPosition,
                    finalizedPosition, combinedVertices[i].textureCoordinate);
            }

            return combinedMesh;
        }
        /// <summary>
        /// Generates a cube with hard edges and then normalizes the distance to the center.
        /// </summary>
        /// <param name="divisions"></param>
        /// <param name="sphericalNess"></param>
        /// <returns></returns>
        public static Mesh CubeSphere(int divisions, float sphericalNess = 1)
            => CubeSphereBuilder(divisions, sphericalNess).Build(BufferUsageHint.StaticDraw);

        private static MeshBuilder[] TextBuilder(int fontHandle, string text, out float lengthOfText)
        {
            Font font = FontLoader.GetFont(fontHandle);
            char[] characters = text.ToCharArray();
            MeshBuilder[] textMesh = new MeshBuilder[font.textureAtlasHandle.Length];
            float xCursor = 0;
            for (int i = 0; i < characters.Length; i++)
            {
                FontCharacter characterInfo = font.GetCharacterInfo(characters[i]);
                int meshIndex = characterInfo.page;

                MeshBuilder characterQuad = FontQuadBuilder(
                    (characterInfo.xOffset + xCursor) / (float)font.width,
                    characterInfo.yOffset / (float)font.height,
                    characterInfo.width / (float)font.width, characterInfo.height / (float)font.height,
                    characterInfo.x / (float)font.width, characterInfo.y / (float)font.height);
                xCursor += characterInfo.xAdvance;

                if (textMesh[meshIndex] == null)
                {
                    textMesh[meshIndex] = characterQuad;
                    continue;
                }
                textMesh[meshIndex] += characterQuad;
            }
            lengthOfText = xCursor / font.width;
            for (int i = 0; i < textMesh.Length; i++)
            {
                textMesh[i] -= new Vector3(lengthOfText / 2f, 0, 0);
                textMesh[i].name = text;
            }

            return textMesh;
        }
        /// <summary>
        /// Generates text-meshes with a certain font.
        /// </summary>
        /// <param name="fontHandle"></param>
        /// <param name="text"></param>
        /// <param name="lengthOfText"></param>
        /// <returns></returns>
        public static Mesh[] Text(int fontHandle, string text, out float lengthOfText)
        {
            MeshBuilder[] builders = TextBuilder(fontHandle, text, out lengthOfText);
            Mesh[] meshes = new Mesh[builders.Length];
            for(int i = 0; i < builders.Length; i++)
            {
                meshes[i] = builders[i].Build(BufferUsageHint.StaticDraw);
            }
            return meshes;
        }

        public static Mesh UVSphere(int rows, int columns)
        {
            if(rows < 1 || columns < 3)
            {
                Debug.LogError("UV sphere must have at least 1 row and 3 columns!");
                return null;
            }

            MeshBuilder combinedMesh = new MeshBuilder();
            //Generates the vertices:
            List<Vertex> vertices = new List<Vertex>()
            {
                new Vertex(new Vector3(0, 1, 0))
            };
            for (int i = 0; i < rows; i++)
            {
                float radian = MathF.PI * (i + 1f) / (rows + 1f);
                vertices.AddRange((CircleBuilder(columns) * MathF.Sin(radian) 
                    + new Vector3(0, MathF.Cos(radian), 0)).GetVertices());
            }
            vertices.Add(new Vertex(new Vector3(0, -1, 0)));

            //Generate sides (not top and bottom)
            List<int> indices = new List<int>();
            for(int r = 0; r < rows - 1; r++) { 
                for(int c = 0; c < columns; c++)
                {
                    indices.AddRange([
                        1 + c + columns + r * columns,
                        1 + c + r * columns,
                        1 + (c + 1) % columns + r * columns,
                    ]);
                    indices.AddRange([
                        1 + (c + 1) % columns + columns + r * columns,
                        1 + c + columns + r * columns,
                        1 + (c + 1) % columns + r * columns,
                    ]);
                }
            }
            //Now connect top and bottom
            for(int i = 0; i < columns; i++)
            {
                indices.AddRange([
                    0,
                    1 + (i + 1) % columns,
                    1 + i,
                ]);
                indices.AddRange([
                    vertices.Count - 1,
                    1 + columns * (rows - 1) + i,
                    1 + columns * (rows - 1) + (i + 1) % columns,
                ]);
            }

            combinedMesh.SetIndices(indices.ToArray());
            combinedMesh.SetVertices(vertices.ToArray());
            return combinedMesh.Build(BufferUsageHint.StaticDraw);
        }
        #endregion
    }
}
