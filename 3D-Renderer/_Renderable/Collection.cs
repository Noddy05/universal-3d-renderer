using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3D_Renderer._Renderable._GameObject;

namespace _3D_Renderer._Renderable
{
    /// <summary>
    /// In order for the renderer to render specific objects they must be collected in these collections
    /// </summary>
    internal class Collection
    {
        public List<Renderable> renderables = new List<Renderable>();
    }
}
