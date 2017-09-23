using System;

namespace Charger
{
    /// <summary>
    /// Use this attribute to connect a property value to a differet -but same type- property 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class ConnectAttribute : Attribute
    {
        readonly string propName;
        /// <summary>
        /// Explicitly specify a property name to charge this property from. 
        /// </summary>
        /// <param name="propName">Source's property name to charge from.</param>
        public ConnectAttribute(string propName)
        {
            this.propName = propName;
        }

        public string PropName
        {
            get { return propName; }
        }

        /// <summary>
        /// When set to false it will ignore the abscence of the property on the source object (as well any typos in propName argument), otherwise will throw exception, defaults to true.
        /// </summary>
        public bool AlwaysOnSource { get; set; } = true;

    }

}
