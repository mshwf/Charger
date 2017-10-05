using System;

namespace Mshwf.Charger
{
   
    /// <summary>
    /// Use this attribute to explicitly specify a property name to charge from.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SourcePropertyAttribute : Attribute
    {
        readonly string propertyName;
        /// <summary>
        /// Explicitly specify a property name to charge this property from.
        /// </summary>
        /// <param name="propertyName">Source's property name to charge from.</param>
        public SourcePropertyAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }
 
        /// <summary>
        /// Get the source's property name to charge from.
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
        }

        /// <summary>
        /// When set to false it will ignore the abscence of the property on the source object, otherwise will throw exception, defaults to true.
        /// </summary>
        public bool AlwaysOnSource { get; set; } = true;
    }

}
