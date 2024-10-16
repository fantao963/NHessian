using NHessian.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHessian.IO
{
    public class CSharpTypeBindings : TypeBindings
    {
        public CSharpTypeBindings() { 
            
            CollectTypes();
        }

        private void CollectTypes()
        {
            if (stringToType.Count > 0) return;
            var assembies=AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemb in assembies)
            {
                if (assemb.IsDynamic) continue;
                var types=assemb.GetExportedTypes().Where((ty)=>{
                    return !(ty.IsAbstract || ty.IsInterface || ty.IsEnum);
                });
                foreach (var type in types)
                {
                    var typeString=this.TypeToTypeString(type);
                    if (stringToType.ContainsKey(typeString) == false)
                    {
                        stringToType.Add(typeString, type);
                    }
                }
            }
        }

        private static Dictionary<string,Type> stringToType=new Dictionary<string,Type>();

        public override Type TypeStringToType(string typeString)
        {
            stringToType.TryGetValue(typeString, out var ty);
            return ty;
        }


        public override string TypeToTypeString(Type type)
        {
            var typeString=type.FullName;
            return typeString;
        }
    }
}