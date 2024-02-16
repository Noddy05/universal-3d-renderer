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

        public static void BindVAO(int vboHandle, int vaoHandle)
        {
            GL.BindVertexArray(vaoHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboHandle);

            int length = 8;
            //For each vertex data struct:
            //Position:
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, length * sizeof(float),
                0 * sizeof(float));
            GL.EnableVertexAttribArray(0);

            //Normal:
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, length * sizeof(float),
                3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            //TexCoordinate:
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, length * sizeof(float),
                6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            //Unbind VAO and VBO
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
