using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHessian.IO.Deserialization
{
    internal class DefaultFieldDeserializer : FieldDeserializer
    {
        protected readonly Action<object, object> _assign;

        public DefaultFieldDeserializer(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            if (fieldInfo.FieldType == typeof(float)
                || fieldInfo.FieldType == typeof(ushort)
                || fieldInfo.FieldType == typeof(ulong)
                || fieldInfo.FieldType == typeof(byte)
                || fieldInfo.FieldType == typeof(sbyte)
                || fieldInfo.FieldType == typeof(char)
                || fieldInfo.FieldType == typeof(short)

                )
            {
                _assign = (obj, value) =>
                {
                    fieldInfo.SetValue(obj,Convert.ChangeType(value,fieldInfo.FieldType));
                };
                return;
            }


            ParameterExpression targetExp = Expression.Parameter(typeof(object), "target");
            ParameterExpression valueExp = Expression.Parameter(typeof(object), "value");

            MemberExpression fieldExp = Expression.Field(
                Expression.Convert(targetExp, fieldInfo.DeclaringType),
                fieldInfo);
            BinaryExpression assignExp = Expression.Assign(
                fieldExp,
                Expression.Convert(valueExp, fieldInfo.FieldType));

            _assign = Expression.Lambda<Action<object, object>>(assignExp, targetExp, valueExp).Compile();
        }

        public override void PopulateField(HessianInput input, object obj)
        {
            _assign(obj, input.ReadObject(FieldInfo.FieldType));
        }
    }
}