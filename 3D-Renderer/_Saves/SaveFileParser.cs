using _3D_Renderer._Debug;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _3D_Renderer._Saves
{
    internal class SaveFileParser
    {
        public static string ParseToString(object obj, object fromObject, string savedAs)
        {
            Type type = obj.GetType();
            if (type == typeof(string))
            {
                return (string)obj;
            }
            if (type == typeof(Vector3))
            {
                Vector3 v = (Vector3)obj;
                return $"{v.X};{v.Y};{v.Z}";
            }
            if (type == typeof(Vector2))
            {
                Vector3 v = (Vector3)obj;
                return $"{v.X};{v.Y}";
            }
            if (type == typeof(float))
            {
                float v = (float)obj;
                return v.ToString();
            }
            if (type == typeof(int))
            {
                int v = (int)obj;
                return v.ToString();
            }
            if (type == typeof(double))
            {
                double v = (double)obj;
                return v.ToString();
            }

            Debug.LogFatalError($"Object of type {obj.GetType().Name} " +
                $"is not currently supported! (From: {fromObject.GetType().Name}, saving to " +
                $"\"{savedAs}\")");
            return ".";
        }

        public static object ParseFromString(FieldInfo fieldInfo, string data)
        {
            //I swear switch statements dont work for this :crying emoji:
            string[] parts = data.Split(';');
            Type type = fieldInfo.FieldType;
            if (type == typeof(string))
            {
                return data;
            }
            if (type == typeof(Vector3))
            {
                Vector3 parsed = new Vector3();
                parsed.X = float.Parse(parts[0]);
                parsed.Y = float.Parse(parts[1]);
                parsed.Z = float.Parse(parts[2]);
                return parsed;
            }
            if (type == typeof(Vector2))
            {
                Vector2 parsed = new Vector2();
                parsed.X = float.Parse(parts[0]);
                parsed.Y = float.Parse(parts[1]);
                return parsed;
            }
            if (type == typeof(float))
            {
                return float.Parse(parts[0]);
            }
            if (type == typeof(double))
            {
                return double.Parse(parts[0]);
            }

            Debug.LogFatalError($"Object of type {type.Name} is not currently supported!");
            return null;
        }
    }
}
