using System;

namespace Mshwf.Charger
{
    /// <summary>
    /// Take an appropriate action when the target property charged by DeepCharging is null.
    /// </summary>
    public enum NullTargetAction
    {
        /// <summary>
        /// Throw null reference exception when the target property is null.
        /// </summary>
        ThrowException,
        /// <summary>
        /// Ignore charging a property if it's null.
        /// </summary>
        IgnoreCharging,
    }
    /// <summary>
    /// Charge a property of a custom type from the source's property (of a different custom type), consider using SourcePropertyAttribute if names are different.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DeepChargingAttribute : Attribute
    {
        /// <summary>
        /// Set the action taken by Charger when a property set by DeepCharging is null, default is ThrowException.
        /// </summary>
        public NullTargetAction NullTargetAction { get; set; }
    }
}
