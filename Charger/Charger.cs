using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mshwf.Charger
{
    /// <summary>
    /// Charger is an object mapper, for easily mapping between properties in two objects.
    /// </summary>
    public static class Charger
    {
        static readonly string assemblyName = typeof(Charger).Assembly.GetName().Name;
        static readonly string exAlwaysOnSource = "The property name '{0}' couldn't be found on the source object, if the property on target class isn't always on source object, set AlwaysOnSource to false in the SourcePropertyAttribute to ignore charging it.";
        static readonly string exNullAction = "Property '{0}' in the target object is null and cannot be charged from property '{1}' in the source object, if you want to ignore charging null properties, specify NullTargetAction.IgnoreCharging in DeepChargingAttribute.";

        /// <summary>
        /// Charge an object with values of another object.
        /// </summary>
        /// <param name="target">The object to charge.</param>
        /// <param name="source">The object to charge from.</param>
        public static void ChargeFrom(this object target, object source)
        {
            if (target == null || source == null)
                throw new NullReferenceException() { Source = assemblyName };
            ChargeProperties(target, source);
        }
        /// <summary>
        /// Map the properties in the source object to a specified object type.
        /// </summary>
        /// <typeparam name="TTarget">The object type to be mapped to, it should support a default constructor.</typeparam>
        /// <param name="source">The object to squeeze.</param>
        /// <returns>A new object charged with values of the source object.</returns>
        public static TTarget Squeeze<TTarget>(this object source) where TTarget : class, new()
        {
            if (source == null)
                throw new NullReferenceException() { Source = assemblyName };
            TTarget target = new TTarget();
            ChargeProperties(target, source);
            return target;
        }

        public static TTarget Squeeze<TTarget>(this TTarget source) where TTarget : class, new()
        {
            if (source == null)
                throw new NullReferenceException() { Source = assemblyName };
            TTarget target = new TTarget();
            ChargeProperties(target, source);
            return target;
        }


        /// <summary>
        /// Charge a list from another list of different type.
        /// </summary>
        /// <typeparam name="TTarget">The type argument of the target list to charge.</typeparam>
        /// <param name="targetList">The list to charge.</param>
        /// <param name="sourceList">The list to charge from.</param>
        public static void ChargeFrom<TTarget>(this ICollection<TTarget> targetList, ICollection sourceList) where TTarget : class, new()
        {
            if (targetList == null || sourceList == null)
                throw new NullReferenceException() { Source = assemblyName };
            foreach (var item in sourceList)
            {
                targetList.Add(Squeeze<TTarget>(item));
            }
        }
        /// <summary>
        /// Map the items in the source list to a list of a specified type.
        /// </summary>
        /// <typeparam name="TTarget">The argument type of the list to charge, it should support a default constructor.</typeparam>
        /// <param name="sourceList">The list to squeez.</param>
        /// <returns>A new list charged from the source list.</returns>
        public static ICollection<TTarget> Squeeze<TTarget>(this ICollection sourceList) where TTarget : class, new()
        {
            if (sourceList == null)
                throw new NullReferenceException() { Source = assemblyName };
            List<TTarget> targetList = new List<TTarget>();

            foreach (var item in sourceList)
            {
                targetList.Add(Squeeze<TTarget>(item));
            }
            return targetList;
        }

        static void ChargeProperties(object target, object source)
        {
            try
            {
                PropertyInfo[] targetProperties = target.GetType().GetProperties();
                PropertyInfo[] sourceProperties = source.GetType().GetProperties();

                foreach (PropertyInfo propTarget in targetProperties)
                {
                    if (Attribute.IsDefined(propTarget, typeof(NotChargedAttribute)))
                    {
                        continue;
                    }
                    var attrs = Attribute.GetCustomAttributes(propTarget);
                    SourcePropertyAttribute sourcePropAttr;
                    DeepChargingAttribute deepChargingAttr;

                    if (!(attrs.Any(x => x is SourcePropertyAttribute)) && !(attrs.Any(x => x is DeepChargingAttribute)))
                    {
                        var propSource = sourceProperties.SingleOrDefault(x => x.Name == propTarget.Name && x.PropertyType == propTarget.PropertyType);
                        if (propSource != null)
                        {
                            propTarget.SetValue(target, propSource.GetValue(source));
                        }
                    }

                    else if (attrs.Any(x => x is SourcePropertyAttribute) && attrs.Any(x => x is DeepChargingAttribute))
                    {
                        CheckSysType(propTarget);
                        deepChargingAttr = (DeepChargingAttribute)Attribute.GetCustomAttribute(propTarget, typeof(DeepChargingAttribute));
                        sourcePropAttr = (SourcePropertyAttribute)Attribute.GetCustomAttribute(propTarget, typeof(SourcePropertyAttribute));
                        var sp = sourceProperties.SingleOrDefault(x => x.Name == sourcePropAttr.PropertyName);
                        if (sp == null)
                        {
                            if (sourcePropAttr.AlwaysOnSource)
                                throw new NullReferenceException(string.Format(exAlwaysOnSource, sourcePropAttr.PropertyName)) { Source = assemblyName };
                            else
                                continue;
                        }

                        var tar = propTarget.GetValue(target);
                        var sour = sp.GetValue(source);

                        if (tar != null && sour != null)
                        {
                            ChargeFrom(tar, sour);
                            continue;
                        }
                        if (tar == null && sour == null)
                            continue;
                        switch (deepChargingAttr.NullTargetAction)
                        {
                            case NullTargetAction.ThrowException:
                                if (tar == null)
                                    throw new NullReferenceException(string.Format(exNullAction, propTarget.Name, sp.Name)) { Source = assemblyName };
                                else
                                    propTarget.SetValue(target, null);
                                break;
                            case NullTargetAction.IgnoreCharging:
                                if (tar != null)
                                    propTarget.SetValue(target, null);
                                break;
                        }
                        continue;
                    }
                    else if (attrs.Any(x => x is SourcePropertyAttribute))
                    {
                        sourcePropAttr = (SourcePropertyAttribute)Attribute.GetCustomAttribute(propTarget, typeof(SourcePropertyAttribute));
                        var sp = sourceProperties.SingleOrDefault(x => x.Name == sourcePropAttr.PropertyName);

                        if (sp == null)
                        {
                            if (sourcePropAttr.AlwaysOnSource)
                                throw new NullReferenceException(string.Format(exAlwaysOnSource, sourcePropAttr.PropertyName)) { Source = assemblyName };
                            else
                                continue;
                        }
                        propTarget.SetValue(target, sp.GetValue(source));

                    }
                    else if (attrs.Any(x => x is DeepChargingAttribute))
                    {
                        CheckSysType(propTarget);
                        var sp = sourceProperties.SingleOrDefault(x => x.Name == propTarget.Name);
                        if (sp == null)
                            throw new NullReferenceException($"No property found on source object named '{propTarget.Name}', consider using SourceProperty attribute if the property name differ in the source object.") { Source = assemblyName };
                        deepChargingAttr = (DeepChargingAttribute)Attribute.GetCustomAttribute(propTarget, typeof(DeepChargingAttribute));

                        var tar = propTarget.GetValue(target);
                        var sour = sp.GetValue(source);
                        if (tar != null && sour != null)
                        {
                            ChargeFrom(tar, sour);
                            continue;
                        }
                        if (tar == null && sour == null)
                            continue;
                        switch (deepChargingAttr.NullTargetAction)
                        {
                            case NullTargetAction.ThrowException:
                                if (tar == null)
                                    throw new NullReferenceException(string.Format(exNullAction, propTarget.Name, sp.Name)) { Source = assemblyName };
                                else
                                    propTarget.SetValue(target, null);
                                break;
                            case NullTargetAction.IgnoreCharging:
                                if (tar != null)
                                    propTarget.SetValue(target, null);
                                break;
                        }
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Source = assemblyName;
                throw;
            }
        }

        private static void CheckSysType(PropertyInfo propTarget)
        {
            if (propTarget.PropertyType.FullName.StartsWith("System"))//DeepCharging on system property
            {
                throw new Exception($"You cannot declare DeepChargingAttribute on System types properties.\nProperty name: {propTarget.Name}\nProperty type: {propTarget.PropertyType}.") { Source = assemblyName };
            }
        }
    }
}
