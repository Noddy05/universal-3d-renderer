using OpenTK.Mathematics;
namespace _3D_Renderer._Saves
{
    internal static partial class SaveFileParser
    {
        /// <summary>
        /// The <see cref="Encoder"/> is a function that returns an encoded string.
        /// The <see cref="Encoder"/> encodes an object so its readable by the decoders.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate string Encoder(object obj);
        private static Dictionary<Type, Encoder> encoders;

        public delegate object Decoder(string data);
        private static Dictionary<Type, Decoder> decoders;

        /// <summary>
        /// Adds a function that helps the <see cref="SaveFileParser"/> 
        /// encode data of a certain type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="decoder"></param>
        public static void AddEncoder(Type type, Encoder encoder)
        {
            if (encoders.ContainsKey(type))
            {
                Debug.LogWarning($"Attempting to create a new Encoder for type: {type} " +
                    $"but type is already supported!");
                return;
            }
            encoders.Add(type, encoder);
        }
        public static Encoder GetEncoder(Type type)
        {
            if (encoders.ContainsKey(type))
            {
                return encoders[type];
            }
            return null;
        }

        /// <summary>
        /// Adds a function that helps the <see cref="SaveFileParser"/> 
        /// decode saved data of a certain type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="decoder"></param>
        public static void AddDecoder(Type type, Decoder decoder)
        {
            if (decoders.ContainsKey(type))
            {
                Debug.LogWarning($"Attempting to create a new Decoder for type: {type} " +
                    $"but type is already supported!");
                return;
            }
            decoders.Add(type, decoder);
        }
        public static Decoder GetDecoder(Type type)
        {
            if (decoders.ContainsKey(type))
            {
                return decoders[type];
            }
            return null;
        }

        //Generate default codecs:
        static SaveFileParser()
        {
            //Create default encoders:
            encoders = new Dictionary<Type, Encoder>()
            {
                { typeof(Vector4),
                    (object obj) => {
                        Vector4 v = (Vector4)obj;
                        return $"{v.X};{v.Y};{v.Z};{v.W}";
                    }
                },
                { typeof(Vector3),
                    (object obj) => {
                        Vector3 v = (Vector3)obj;
                        return $"{v.X};{v.Y};{v.Z}";
                    }
                },
                { typeof(Vector2),
                    (object obj) => {
                        Vector2 v = (Vector2)obj;
                        return $"{v.X};{v.Y}";
                    }
                },
            };

            //Create default decoders:
            decoders = new Dictionary<Type, Decoder>()
            {
                { typeof(Vector4),
                    (string data) =>
                    {
                        string[] parts = data.Split(';');
                        Vector4 parsed = new Vector4();
                        parsed.X = float.Parse(parts[0]);
                        parsed.Y = float.Parse(parts[1]);
                        parsed.Z = float.Parse(parts[2]);
                        parsed.W = float.Parse(parts[3]);
                        return parsed;
                    }
                },
                { typeof(Vector3),
                    (string data) =>
                    {
                        string[] parts = data.Split(';');
                        Vector3 parsed = new Vector3();
                        parsed.X = float.Parse(parts[0]);
                        parsed.Y = float.Parse(parts[1]);
                        parsed.Z = float.Parse(parts[2]);
                        return parsed;
                    }
                },
                { typeof(Vector2),
                    (string data) =>
                    {
                        string[] parts = data.Split(';');
                        Vector2 parsed = new Vector2();
                        parsed.X = float.Parse(parts[0]);
                        parsed.Y = float.Parse(parts[1]);
                        return parsed;
                    }
                },
                { typeof(int),
                    (string data) =>
                    {
                        return int.Parse(data);
                    }
                },
                { typeof(float),
                    (string data) =>
                    {
                        return float.Parse(data);
                    }
                },
                { typeof(double),
                    (string data) =>
                    {
                        return double.Parse(data);
                    }
                },
                { typeof(string),
                    (string data) =>
                    {
                        return data;
                    }
                },
            };
        }
    }
}
