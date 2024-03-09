using _3D_Renderer._Camera;
using _3D_Renderer._Debug;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Saves
{
    internal static class EditorMemory
    {
        static EditorMemory()
        {
            Program.GetWindow().Unload += SaveNow;
        }

        private const string saveFileName = "editor_auto_save.txt";
        private static string? latestSaveLocation;
        private static string? fullPath;
        public static void SetLatestSaveLocation(string location)
        {
            latestSaveLocation = location;
            fullPath = latestSaveLocation + $"/{saveFileName}";
        }

        //By ChatGPT:
        private static List<FieldInfo> GetFieldsWithAttribute(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                       .Where(field => Attribute.IsDefined(field, typeof(SaveOnCloseAttribute)))
                       .ToList();
        }
        //End

        private static void SaveNow()
        {
            if (latestSaveLocation == null || fullPath == null) {
                Debug.LogFatalError("EditorMemory has nowhere to save to!");
            }

            FreeCamera editorView = Program.GetWindow().GetEditorCamera();
            if (!File.Exists(fullPath))
            {
                //File doesn't exists
                File.Create(fullPath!);
            }

            using(StreamWriter stream = new StreamWriter(fullPath!, false))
            {
                stream.WriteLine("# Editor auto save at " + DateTime.Now);
                foreach(KeyValuePair<string, object> objectSaved in objectsToSave)
                {
                    List<FieldInfo> fieldsWithAttribute = 
                        GetFieldsWithAttribute(objectSaved.Value.GetType());
                    foreach (FieldInfo field in fieldsWithAttribute)
                    {
                        object value = field.GetValue(objectSaved.Value)!;
                        stream.WriteLine($"\"{objectSaved.Key}\" " +
                            objectSaved.Value.GetType().Name +
                            $" {field.Name} {SaveFileParser.ParseToString(value, 
                            objectSaved.Value, objectSaved.Key)}");
                    }
                }
            }
        }

        private static Dictionary<string, object> objectsToSave = new Dictionary<string, object>();
        public static void AttachObject(string name, object obj)
        {
            objectsToSave.Add(name, obj);
            if (!File.Exists(fullPath))
            {
                //No save file found
                return;
            }

            string[] lines = File.ReadAllLines(fullPath);
            for (int i = 0; i < lines.Length; i++)
            {
                //this line is a comment, skip
                if (lines[i][0] == '#')
                    continue;

                string startsWith = $"\"{name}\" ";
                //If this line refers to this object:
                if (lines[i].StartsWith(startsWith))
                {
                    string[] segmentedString = lines[i].Split(' ', ':');
                    string typeName = obj.GetType().Name;
                    if (typeName != segmentedString[1])
                    {
                        Debug.LogWarning($"Type mismatch! Object of key: {name} was expected to "
                           + $"attach to object of type {segmentedString[1]}, " +
                           $"instead recieved type {typeName}.");
                    }
                    FieldInfo? fieldInfo = obj.GetType().GetField(segmentedString[2], 
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if(fieldInfo == null)
                    {
                        Debug.LogError("Field info is null!");
                        continue;
                    }
                    object value = SaveFileParser.ParseFromString(fieldInfo, segmentedString[3]);
                    fieldInfo.SetValue(obj, value);
                }
            }
        }

        public static void LoadLatestSave()
        {

        }
    }
}
