using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mshwf.Charger
{
    /// <summary>
    /// Charger copies values from the source's object to the target.
    /// </summary>
    public static class Charger
    {
        static readonly string assemblyName = typeof(Charger).Assembly.GetName().Name;

        /// <summary>
        /// Copies values of the source's properties to the target's properties.
        /// </summary>
        /// <param name="target">The object to be charghed.</param>
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
        /// <typeparam name="TTarget">The object type to be mapped to.</typeparam>
        /// <param name="source">The object to squeeze.</param>
        /// <returns></returns>
        public static TTarget Squeeze<TTarget>(this object source) where TTarget : class, new()
        {
            if (source == null)
                throw new NullReferenceException() { Source = assemblyName };
            TTarget target = new TTarget();
            ChargeProperties(target, source);
            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="targetList"></param>
        /// <param name="sourceList"></param>
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
        /// 
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="sourceList"></param>
        /// <returns></returns>
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

            PropertyInfo[] targetProperties = target.GetType().GetProperties();
            PropertyInfo[] sourceProperties = source.GetType().GetProperties();

            foreach (PropertyInfo propTarget in targetProperties)
            {
                var attrs = Attribute.GetCustomAttributes(propTarget);
                SourcePropertyAttribute sourcePropAttr;
                DeepChargingAttribute deepChargingAttr;

                if (!(attrs.Any(x => x is SourcePropertyAttribute)) && !(attrs.Any(x => x is DeepChargingAttribute)))
                {
                    if (Attribute.IsDefined(propTarget, typeof(NotChargedAttribute)))
                    {
                        continue;
                    }

                    var propSource = sourceProperties.SingleOrDefault(x => x.Name == propTarget.Name && x.PropertyType == propTarget.PropertyType);
                    if (propSource != null)
                    {
                        propTarget.SetValue(target, propSource.GetValue(source));
                    }

                }

                else if (attrs.Any(x => x is SourcePropertyAttribute) && attrs.Any(x => x is DeepChargingAttribute))
                {
                    deepChargingAttr = (DeepChargingAttribute)Attribute.GetCustomAttribute(propTarget, typeof(DeepChargingAttribute));
                    sourcePropAttr = (SourcePropertyAttribute)Attribute.GetCustomAttribute(propTarget, typeof(SourcePropertyAttribute));
                    var sp = sourceProperties.SingleOrDefault(x => x.Name == sourcePropAttr.PropertyName);
                    if (sp == null)
                    {
                        if (sourcePropAttr.AlwaysOnSource)
                            throw new NullReferenceException($"The property name '{sourcePropAttr.PropertyName}' couldn't be found on the source object.") { Source = assemblyName };
                        else
                            continue;
                    }

                    var tar = propTarget.GetValue(target);
                    var sour = sp.GetValue(source);
                    switch (deepChargingAttr.NullReferenceAction)
                    {
                        case NullReferenceAction.ThrowException:
                            ChargeFrom(tar, sour);
                            break;
                        case NullReferenceAction.Ignore:
                            //if (sour == null)
                            //{
                            //    propTarget.SetValue(target, null);
                            //}
                            if (sour == null || tar == null)
                                continue;
                            else
                                ChargeFrom(tar, sour);
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
                            throw new NullReferenceException($"The property name '{sourcePropAttr.PropertyName}' couldn't be found on the source object.") { Source = assemblyName };
                        else
                            continue;
                    }
                    propTarget.SetValue(target, sp.GetValue(source));

                }
                else if (attrs.Any(x => x is DeepChargingAttribute))
                {
                    var tar = propTarget.GetValue(target);
                    var sp = sourceProperties.SingleOrDefault(x => x.Name == propTarget.Name);
                    if (sp == null)
                        throw new NullReferenceException($"No property found on source object named '{propTarget.Name}', consider using SourceProperty attribute if the property name differ in the source object.") { Source = assemblyName };
                    var sour = sp.GetValue(source);
                    deepChargingAttr = (DeepChargingAttribute)Attribute.GetCustomAttribute(propTarget, typeof(DeepChargingAttribute));

                    switch (deepChargingAttr.NullReferenceAction)
                    {
                        
                        case NullReferenceAction.ThrowException:
                            ChargeFrom(tar, sour);
                            break;
                        case NullReferenceAction.Ignore:
                            if (sour == null || tar == null)
                                continue;
                            else
                                ChargeFrom(tar, sour);
                            break;
                    }
                }
            }
        }
        static void Chh(object tar, object sour, NullReferenceAction action)
        {

        }
    }
}
