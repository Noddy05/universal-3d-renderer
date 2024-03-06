using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace _3D_Renderer._Import
{
    internal class TextureLoader
    {
        public static int LoadTexture(string filePath, float mipmapLODBias)
        {
            int textureHandle = LoadTexture(filePath, true);
            SetLODBias(textureHandle, mipmapLODBias);
            return textureHandle;
        }

        public static int LoadTexture(string filePath, bool generateMipmaps = true) 
            //AI generated, using ChatGPT
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Could not find file at path: " + filePath);
            }

            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            // Load the image data
            int width, height;
            byte[] imageData = LoadImageData(filePath, out width, out height);

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, 
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, 
                TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, 
                TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, 
                TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            // Load the texture data into OpenGL
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, imageData);

            if (generateMipmaps) { 
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                    (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);

                SetLODBias(textureId, -1f);
            }

            return textureId;
        }

        /// <summary>
        /// Changes how subtle the mipmapping of a texture is.
        /// </summary>
        /// <param name="textureHandle">The handle of the texture you want to modify</param>
        /// <param name="bias">High bias will make the mipmapping effect very noticable, whereas
        /// low/negative biases will make it more subtle.</param>
        public static void SetLODBias(int textureHandle, float bias)
        {
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias,
                bias);
        }

        /// <summary>
        /// Array <paramref name="filePaths"/> must be in this specific order:<br></br>
        /// 0) Right Face, <br></br>
        /// 1) Left Face, <br></br>
        /// 2) Top Face, <br></br>
        /// 3) Bottom Face, <br></br>
        /// 4) Forward Face, <br></br>
        /// 5) Backward Face,
        /// </summary>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static int LoadCubemap(string[] filePaths)
        {
            if (filePaths.Length != 6)
                throw new Exception("Cubemap requires 6 filepaths");

            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, textureId);

            for(int i = 0; i < 6; i++) { 
                if (!File.Exists(filePaths[i]))
                {
                    throw new FileNotFoundException("Could not find file at path: " + filePaths[i]);
                }

                // Load the image data
                int width, height;
                byte[] imageData = LoadImageData(filePaths[i], out width, out height);

                // Set texture parameters
                GL.TexParameter(TextureTarget.TextureCubeMap, 
                    TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, 
                    TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.TextureCubeMap, 
                    TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap,
                    TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.TextureCubeMap,
                    TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

                // Load the texture data into OpenGL
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 
                    0, PixelInternalFormat.Rgba, width, height, 0,
                    PixelFormat.Bgra, PixelType.UnsignedByte, imageData);
            }
            return textureId;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability",
        "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        public static byte[] LoadImageData(string filePath, out int width, out int height) 
            //AI generated, using ChatGPT
        {
            // Use System.Drawing.Bitmap to load the image and get the width and height
            using Bitmap bitmap = new(filePath);
            width = bitmap.Width;
            height = bitmap.Height;

            // Lock the bitmap data and copy it into a byte array
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] bytes = new byte[data.Stride * data.Height];
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            bitmap.UnlockBits(data);

            return bytes;
        }
    }
}
