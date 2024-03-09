using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Renderer._Saves
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class SaveOnCloseAttribute : Attribute
    {

    }
}
