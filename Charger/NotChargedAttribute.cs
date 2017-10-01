﻿using System;

namespace Mshwf.Charger
{
    /// <summary>
    /// Use this attribute on properties you want skip charging by ChargeFrom or Squeez.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class NotChargedAttribute : Attribute
    {
    }
}
