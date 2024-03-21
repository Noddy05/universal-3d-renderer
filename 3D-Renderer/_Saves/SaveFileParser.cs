using _3D_Renderer._Debug;
using OpenTK.Mathematics;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;

namespace _3D_Renderer._Saves
{
    /// <summary>
    /// "SaveFileParser.cs" is a partial class, split into two: "SaveFileParser.cs" and "SaveFileCodecs.cs".
    /// "SaveFileParser.cs" encodes and decodes objects, while "SaveFileCodecs.cs" keeps track of the
    /// different codecs.
    /// </summary>
    internal static partial class SaveFileParser
    {
        public static string ParseToString(object obj, object fromObject, string savedAs)
        {
            Type type = obj.GetType();
            if (encoders.ContainsKey(type))
            {
                return encoders[type](obj);
            }
            Debug.LogWarning($"Decoder doesn't contain method to decode type: {type}!\n" +
                $"Consider using AddDecoder(Type type, Decoder decoder).\n" +
                $"(From: { fromObject.GetType().Name}, saved to \"{savedAs}\")");
            return obj.ToString()!;
        }

        public static object ParseFromString(FieldInfo fieldInfo, string data)
        {
            //I swear switch statements dont work for this :crying emoji:
            Type type = fieldInfo.FieldType;
            if (decoders.ContainsKey(type))
            {
                return decoders[type](data);
            }
            Debug.LogError($"Decoder doesn't contain method to decode type: {type}!\n" +
                $"Consider using AddDecoder(Type type, Decoder decoder).\n");
            return null;
        }
    }
}
