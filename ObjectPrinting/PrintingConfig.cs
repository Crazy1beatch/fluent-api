using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private HashSet<PropertyInfo> excludedProperties = new HashSet<PropertyInfo>();
        private readonly HashSet<Type> excludedTypes = new HashSet<Type>();
        public PrintingConfig<TOwner> ExcludeProperty<TProperty>()
        {
            excludedTypes.Add(typeof(TProperty));
            return this;
        }

        public PrintingPropertyConfig<TOwner, TProperty> ChangeCultureInfoFor<TProperty>()
        {
            throw new NotImplementedException();
        }

        public PrintingConfig<TOwner> ExcludeProperty<TProperty>(Expression<Func<TOwner, TProperty>> property)
        {
            var tOwner = Activator.CreateInstance<TOwner>();
            var ds = property.Body.GetType().GetProperties().ToArray(); 
            return this;
            //excludedProperties.Add(property());
        }

        public PrintingPropertyConfig<TOwner, TProperty> ChangeSerializationFor<TProperty>()
        {
            throw new NotImplementedException();
        }

        public PrintingPropertyConfig<TOwner, TProperty> ChangeSerializationFor<TProperty>
        (
            Func<TOwner, TProperty> Property
        )
        {
            throw new NotImplementedException();
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            //TODO apply configurations
            if (obj == null)
                return "null" + Environment.NewLine;

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties().Where(x => !excludedTypes.Contains(x.PropertyType)))
            {
                sb.Append(identation + propertyInfo.Name + " = " +
                          PrintToString(propertyInfo.GetValue(obj),
                              nestingLevel + 1));
            }

            return sb.ToString();
        }
    }
}
