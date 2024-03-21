using _3D_Renderer._Import;
using _3D_Renderer._Renderable._Mesh;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace _3D_Renderer._Generation
{
    internal class WireframeGeneration
    {
        #region Generations of type O(1)
        public static Mesh Quad()
        {
            //   0------1
            //   |      |
            //   |      |
            //   3------2
            Mesh quad = MeshGeneration.Quad();
            int[] indices = [
                0, 1, 1, 2,
                2, 3, 3, 0
            ];
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
            Mesh cube = MeshGeneration.SmoothCube();
            int[] indices = [
                // Right
                1, 2, 2, 6, 
                6, 5, 5, 1, 
                // Left
                0, 4, 4, 7, 
                7, 3, 3, 0, 
                //Combine left and right
                0, 1, 4, 5,
                7, 6, 2, 3
            ];
            cube.SetIndices(indices, BufferUsageHint.StaticDraw);

            return cube;
        }

        #endregion

        #region Complex Generations
        public static Mesh Circle(int vertices)
        {
            if (vertices < 3)
                throw new Exception("Trying to generate circle mesh failed! " +
                    "(MeshGeneration.CircleMesh(a) requires a ≥ 3)");

            Mesh circle = MeshGeneration.Circle(vertices);
            int[] indices = new int[vertices * 2];
            #region Indices
            for (int i = 0; i < vertices - 1; i++)
            {
                indices[i * 2    ] = i;
                indices[i * 2 + 1] = i + 1;
            }
            indices[(vertices - 1) * 2    ] = 0;
            indices[(vertices - 1) * 2 + 1] = vertices - 1;
            #endregion
            circle.SetIndices(indices, BufferUsageHint.StaticDraw);

            return circle;
        }

        public static Mesh Plane(int xDivisions, int yDivisions)
        {
            if(xDivisions < 0 || yDivisions < 0)
                throw new Exception("Trying to generate plane mesh failed! " +
                    "(MeshGeneration.Plane(a, b) requires (a, b) ≥ 0)");

            Mesh plane = MeshGeneration.Plane(xDivisions, yDivisions);
            int[] indices = new int[(xDivisions + 1) * (yDivisions + 1) * 8];
            #region Indices
            for (int y = 0; y < yDivisions + 1; y++)
            {
                for (int x = 0; x < xDivisions + 1; x++)
                {
                    //   1------2
                    //   |      |
                    //   |      |
                    //   0------3
                    indices[(x + (xDivisions + 1) * y) * 8    ] = x     + (xDivisions + 2) * y;
                    indices[(x + (xDivisions + 1) * y) * 8 + 1] = x     + (xDivisions + 2) * (y + 1);

                    indices[(x + (xDivisions + 1) * y) * 8 + 2] = x     + (xDivisions + 2) * (y + 1);
                    indices[(x + (xDivisions + 1) * y) * 8 + 3] = 1 + x + (xDivisions + 2) * (y + 1);

                    indices[(x + (xDivisions + 1) * y) * 8 + 4] = 1 + x + (xDivisions + 2) * (y + 1);
                    indices[(x + (xDivisions + 1) * y) * 8 + 5] = 1 + x + (xDivisions + 2) * y;

                    indices[(x + (xDivisions + 1) * y) * 8 + 6] = 1 + x + (xDivisions + 2) * y;
                    indices[(x + (xDivisions + 1) * y) * 8 + 7] = x     + (xDivisions + 2) * y;
                }
            }
            #endregion
            plane.SetIndices(indices, BufferUsageHint.StaticDraw);

            return plane;
        }
        #endregion
    }
}
