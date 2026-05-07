using System;

namespace Rodak.Hexagons.HexEditor
{
    /// <summary>
    /// Makes the EditableHexagon read-only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HexReadOnly : Attribute
    { }
}