using OpenTK.Mathematics;

namespace _3D_Renderer._Renderable
{
    internal struct Vertex
    {
        public Vector3 vertexPosition;
        public Vector3 vertexNormal;
        public Vector2 textureCoordinate;

        public Vertex(Vector3 position)
        {
            vertexPosition = position;
        }
        public Vertex(Vector3 position, Vector3 normal)
        {
            vertexPosition = position;
            vertexNormal = normal;
        }
        public Vertex(Vector3 position, Vector3 normal, Vector2 textureCoordinates)
        {
            vertexPosition = position;
            vertexNormal = normal;
            textureCoordinate = textureCoordinates;
        }

        public float[] ToArray()
        {
            return [
                vertexPosition.X, 
                vertexPosition.Y, 
                vertexPosition.Z,

                vertexNormal.X, 
                vertexNormal.Y, 
                vertexNormal.Z,

                textureCoordinate.X,
                textureCoordinate.Y,
            ];
        }

        public static float[] VertexToFloatArray(Vertex[] vertices)
        {
            List<float> vertexList = new List<float>();
            foreach (Vertex vertex in vertices)
            {
                vertexList.AddRange(vertex.ToArray());
            }

            return vertexList.ToArray();
        }
    }
}
