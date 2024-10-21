using System.Collections.Generic;
using System.Reflection;

namespace Infrastructure.UI.MVVM
{
    public static partial class UIViewTypes
    {
        private static readonly Dictionary<int, string> Names;
        
        static UIViewTypes()
        {
            FieldInfo[] fields = typeof(UIViewTypes).GetFields(BindingFlags.Public | BindingFlags.Static);
            
            Names = new Dictionary<int, string>(fields.Length);

            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                Names.Add(i, field.Name);
            }
        }
        
        public static readonly int None = 0;

        public static string GetName(int viewType) => Names[viewType];
    }
}