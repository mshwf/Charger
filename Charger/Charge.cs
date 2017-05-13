using System;
using System.Reflection;

namespace Charger
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
            }

        }
    }
}
