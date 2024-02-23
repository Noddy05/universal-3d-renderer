using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using System.Reflection.Metadata;
using OpenTK.Graphics.OpenGL;
using _3D_Renderer._BufferObjects;

namespace _3D_Renderer._Renderable
{
    internal class Vertex
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
            foreach(Vertex vertex in vertices)
            {
                vertexList.AddRange(vertex.ToArray());
            }

            return vertexList.ToArray();
        }

        public static void BindVAO(VBO vbo, VAO vao)
        {
            int length = 8;
            //For each vertex data struct:
            //Position:
            vao.Bind(vbo, 0, 3, length, 0, true, false);

            //Normal:
            vao.Bind(vbo, 1, 3, length, 3, false, false);

            //TexCoordinate:
            vao.Bind(vbo, 2, 2, length, 6, false, true);
        }

        public Vertex Copy() => new Vertex(vertexPosition, vertexNormal, textureCoordinate);
    }
}
