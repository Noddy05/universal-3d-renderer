using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Shading
{
    internal class Shader
    {
        private int handle = -1;
        public int GetHandle() => handle;

        //Automatically returns shaderhandle when casting to int:
        public static implicit operator int(Shader shader) => shader.GetHandle();


        private List<int> attachedHandles = new List<int>();

        public Shader(string vertexShaderLocation, string fragmentShaderLocation)
        {
            handle = GL.CreateProgram();

            AttachShader(vertexShaderLocation, ShaderType.VertexShader);
            AttachShader(fragmentShaderLocation, ShaderType.FragmentShader);
            FinalizeShader();
        }
        public Shader(string vertexShaderLocation, string fragmentShaderLocation,
            params (string location, ShaderType shaderType)[] otherShaders)
        {
            handle = GL.CreateProgram();

            AttachShader(vertexShaderLocation, ShaderType.VertexShader);
            AttachShader(fragmentShaderLocation, ShaderType.FragmentShader);
            for(int i = 0; i < otherShaders.Length; i++)
            {
                AttachShader(otherShaders[i].location, otherShaders[i].shaderType);
            }
            FinalizeShader();
        }

        /// <summary>
        /// Next step is to attach shaders using AttachShader(location, shaderType), 
        /// and finally: FinalizeShader()
        /// </summary>
        public Shader()
        {
            handle = GL.CreateProgram();
        }

        public void AttachShader(string shaderCodeLocation, ShaderType shaderType)
        {
            //Read shader code:
            string shaderCode = IncludeLibraries(File.ReadAllLines(shaderCodeLocation));

            //Create shader:
            int shaderHandle = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderHandle, shaderCode);
            GL.CompileShader(shaderHandle);
            attachedHandles.Add(shaderHandle);
        }

        //Reads shader code and includes all referenced libraries
        //Libraries are included with: #include folder/shader_code_location.glsl
        private string IncludeLibraries(string[] shaderCode)
        {
            string compiledCode = "";
            for(int i = 0; i < shaderCode.Length; i++)
            {
                string libraryInclude = "#include";
                int firstLibraryReferenceLocation = shaderCode[i].IndexOf(libraryInclude);
                //If #include was found in line:
                if (firstLibraryReferenceLocation != -1)
                {
                    int firstQuotationMark = shaderCode[i].IndexOf('"', firstLibraryReferenceLocation);
                    if (firstQuotationMark == -1)
                    {
                        continue;
                    }
                    int secondQuotationMark = shaderCode[i].IndexOf('"', firstQuotationMark + 1);
                    if (secondQuotationMark == -1)
                    {
                        continue;
                    }

                    if (firstLibraryReferenceLocation > 0)
                        compiledCode += shaderCode[i].Substring(0, firstLibraryReferenceLocation);

                    string location = shaderCode[i].Substring(
                        firstQuotationMark + 1,
                        secondQuotationMark - firstQuotationMark - 1);

                    compiledCode += IncludeLibraries(File.ReadAllLines(location));
                } 
                else
                {
                    compiledCode += shaderCode[i] + "\n";
                }
            }


            return compiledCode;
        }

        public void FinalizeShader()
        {
            //Must be called after binding shaders

            foreach (int handle in attachedHandles)
            {
                GL.AttachShader(this.handle, handle);
            }

            GL.LinkProgram(handle);

            //Detach all attached shaders:
            foreach (int handle in attachedHandles)
            {
                GL.DetachShader(this.handle, handle);
                GL.DeleteShader(handle);
            }
            attachedHandles = new List<int>();

            //Handle exceptions
            string outputLog = GL.GetProgramInfoLog(handle);
            if (!string.IsNullOrEmpty(outputLog))
            {
                throw new Exception(outputLog);
            }
        }
    }
}
