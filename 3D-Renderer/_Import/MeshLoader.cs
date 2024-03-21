using _3D_Renderer._Renderable;
using _3D_Renderer._Renderable._Mesh;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _3D_Renderer._Import
{
    internal class MeshLoader
    {
        public static Mesh Load(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            string objectName = "";

            List<Vector3> vertexPositions = new List<Vector3>(); 
            List<Vector3> vertexNormals = new List<Vector3>();
            List<Vector2> textureCoordinates = new List<Vector2>();
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            for (int i = 0; i < lines.Length; i++) {
                string[] words = lines[i].Split(' ');
                string lineStart = words[0];
                switch (lineStart)
                {
                    case "o": //Next part is the name of the object
                        if (string.IsNullOrEmpty(objectName))
                        {
                            objectName = lines[i].Substring(2, lines[i].Length - 2);
                        } else
                        {
                            i = lines.Length;
                            continue;
                        }
                        break;
                    case "v": //Vertex position
                        float x = float.Parse(words[1], CultureInfo.InvariantCulture);
                        float y = float.Parse(words[2], CultureInfo.InvariantCulture);
                        float z = float.Parse(words[3], CultureInfo.InvariantCulture);
                        vertexPositions.Add(new Vector3(x, y, z));
                        break;
                    case "vn": //Vertex normal
                        x = float.Parse(words[1], CultureInfo.InvariantCulture);
                        y = float.Parse(words[2], CultureInfo.InvariantCulture);
                        z = float.Parse(words[3], CultureInfo.InvariantCulture);
                        vertexNormals.Add(new Vector3(x, y, z));
                        break;
                    case "vt": //Vertex normal
                        x = float.Parse(words[1], CultureInfo.InvariantCulture);
                        y = float.Parse(words[2], CultureInfo.InvariantCulture);
                        textureCoordinates.Add(new Vector2(x, y));
                        break;
                    case "f": //Indices
                        Vertex[] finalizedVertices = new Vertex[words.Length - 1];

                        for(int j = 0; j < finalizedVertices.Length; j++)
                        {
                            //https://en.wikipedia.org/wiki/Wavefront_.obj_file
                            //Faces are vertex_index/texture_index/normal_index
                            int[] vLocations = words[1 + j].Split('/').Select(loc => 
                            Convert.ToInt32(loc)).ToArray();
                            Vertex v = new Vertex(vertexPositions[vLocations[0] - 1],
                                vertexNormals[vLocations[2] - 1], textureCoordinates[vLocations[1] - 1]);
                            finalizedVertices[j] = v;
                        }

                        for(int j = 0; j < words.Length - 3; j++)
                        {
                            indices.AddRange([vertices.Count + j, 
                                vertices.Count + j + 1, vertices.Count + j + 2]);
                        }
                        vertices.AddRange(finalizedVertices);

                        break;
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = objectName;
            mesh.SetVertices(vertices.ToArray(), BufferUsageHint.StaticDraw);
            mesh.SetIndices(indices.ToArray(), BufferUsageHint.StaticDraw);

            return mesh;
        }
    }
}
