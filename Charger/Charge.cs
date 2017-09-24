using System;
using System.Linq;
using System.Reflection;

namespace Mshwf.Charger
{
    /// <summary>
    /// Charger copies values from the source's object to the target.
    /// </summary>
    public static class Charge
    {
        /// <summary>
        /// Copies values of the source's properties to the target's properties.
        /// </summary>
        /// <param name="target">The object to be charghed.</param>
        /// <param name="source">The object to charge from.</param>
        public static void ChargeFrom(this object target, object source)
        {
            if (target == null || source == null)
                throw new NullReferenceException();
            PropertyInfo[] targetProperties = target.GetType().GetProperties();
            PropertyInfo[] sourceProperties = source.GetType().GetProperties();
            foreach (PropertyInfo propTarget in targetProperties)
            {
                foreach (PropertyInfo propSource in sourceProperties)
                {
                    if (propTarget.Name == propSource.Name && propTarget.PropertyType == propSource.PropertyType)
                    {
                        propTarget.SetValue(target, propSource.GetValue(source));
                    }
                }
                if (Attribute.IsDefined(propTarget, typeof(ConnectAttribute)))
                {
                    var connAttr = (ConnectAttribute)Attribute.GetCustomAttribute(propTarget, typeof(ConnectAttribute));
                    var sp = sourceProperties.SingleOrDefault(x => x.Name == connAttr.PropName);
                    if (sp == null)
                    {
                        if (connAttr.AlwaysOnSource)
                            throw new NullReferenceException($"The property name '{connAttr.PropName}' couldn't be found on the source object.") { Source = "Mshwf.Charger" };
                        else
                            continue;
                    }

                    if (propTarget.PropertyType.Name != sp.PropertyType.Name)
                    {
                        throw new Exception($"Cannot charge '{propTarget.Name}' from '{sp.Name}', they are not of the same type.") { Source = "Mshwf.Charger" };
                    }
                    propTarget.SetValue(target, sp.GetValue(source));
                }
            }

        }
    }
}
