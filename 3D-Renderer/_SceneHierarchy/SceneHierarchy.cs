using _3D_Renderer._Debug;
using _3D_Renderer._Renderable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._SceneHierarchy
{
    internal static class SceneHierarchy
    {
        private static Dictionary<string, List<Renderable>> collections;

        static SceneHierarchy()
        {
            collections = new Dictionary<string, List<Renderable>>
            {
                { "Scene", new List<Renderable>() }
            };
        }

        public static void NewCollection()
        {
            NewCollection($"Collection #{collections.Count + 1}");
        }
        public static void NewCollection(string name)
        {
            collections.Add(name, new List<Renderable>());
        }


        public static List<Renderable> GetCollection(string collection)
        {
            if (collections.ContainsKey(collection))
                return collections[collection];
            else
            {
                Debug.LogError($"SceneHierarchy contains no collection named {collection}");
                return new List<Renderable>();
            }
        } 

        public static void AddRenderable(string collection, Renderable renderable)
        {
            if(collections.ContainsKey(collection))
                collections[collection].Add(renderable);
            else
                Debug.LogError($"SceneHierarchy contains no collection named {collection}");
        }
        /// <summary>
        /// Removes a renderable from collection
        /// </summary>
        /// <param name="renderable"></param>
        /// <returns><see cref="true"/>: Found and removed the specified <see cref="Renderable"/>.<br></br>
        /// false: Collection didn't contain specified <see cref="Renderable"/>.
        /// </returns>
        public static bool RemoveRenderable(string collection, Renderable renderable)
        {
            if (!collections.ContainsKey(collection))
            {
                Debug.LogError($"SceneHierarchy contains no collection named {collection}");
                return false;
            }

            if (!collections[collection].Contains(renderable))
                return false;
            collections[collection].Remove(renderable);
            return true;
        }
        /// <summary>
        /// Removes the first <see cref="Renderable"/> in <see cref="Collection"/>
        /// with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="true"/>: Found and removed the first <see cref="Renderable"/>.<br></br>
        /// false: No <see cref="Renderable"/> with the specified name was found.
        /// </returns>
        public static bool RemoveRenderable(string collection, string name)
        {
            if (!collections.ContainsKey(collection))
            {
                Debug.LogError($"SceneHierarchy contains no collection named {collection}");
                return false;
            }

            List<Renderable> renderables = collections[collection];
            for (int i = 0; i < renderables.Count; i++)
            {
                if (renderables[i].name == name)
                {
                    renderables.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Removes every <see cref="Renderable"/> in <see cref="Collection"/>
        /// with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="true"/>: Found and removed at least one 
        /// <see cref="Renderable"/>.<br></br>
        /// false: No <see cref="Renderable"/> with the specified name was found.
        /// </returns>
        public static bool RemoveRenderables(string collection, string name)
        {
            if (!collections.ContainsKey(collection))
            {
                Debug.LogError($"SceneHierarchy contains no collection named {collection}");
                return false;
            }

            List<Renderable> renderables = collections[collection];
            bool success = false;
            for (int i = 0; i < renderables.Count; i++)
            {
                if (renderables[i].name == name)
                {
                    renderables.RemoveAt(i);
                    success = true;
                }
            }
            return success;
        }
    }
}
