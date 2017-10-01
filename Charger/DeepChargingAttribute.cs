using System;

namespace Mshwf.Charger
{
    /// <summary>
    /// Take an appropriate action when a property used by deep charging is null, on source or target.
    /// </summary>
    public enum NullReferenceAction
    {
        /// <summary>
        /// Throw null reference exception when the property is null.
        /// </summary>
        ThrowException,
        /// <summary>
        /// Ignore charging a property if it's null.
        /// </summary>
        Ignore
    }
    /// <summary>
    /// Charge a property of a custom type from the source's property (of a different custom type), consider using SourcePropertyAttribute if names are different.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DeepChargingAttribute : Attribute
    {
        /// <summary>
        /// Get or set the action taken by Charger when a property used by deep charging is null, default is ThrowException.
        /// </summary>
        public NullReferenceAction NullReferenceAction { get; set; }
    }
}
