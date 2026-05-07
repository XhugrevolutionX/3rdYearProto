using System;

namespace Rodak.Hexagons.HexEditor
{
    /// <summary>
    /// Defines methods for auto correcting hexagon values.
    /// </summary>
    public enum HexAutoCorrectMode
    {
        /// <summary>
        /// Both <c>Q</c> and <c>R</c> correct <c>S</c>.
        /// <c>S</c> corrects <c>Q</c>.
        /// </summary>
        TwoComponent,

        /// <summary>
        /// Each component corrects the next one.
        /// <c>Q</c> => <c>R</c> => <c>S</c> => <c>Q</c>
        /// </summary>
        Staggered
    }

    /// <summary>
    /// Overrides the default HexAutoCorrectMode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HexAutoCorrect : Attribute
    {
        public HexAutoCorrectMode AutoCorrectMode { get; private set; }
        public HexAutoCorrect(HexAutoCorrectMode autoCorrectMode)
        {
            AutoCorrectMode = autoCorrectMode;
        }
    }

    public static class HexagonAutoCorrector
    {
        public static (int q, int r, int s) AutoCorrectByQ(int q, int r, int s, HexAutoCorrectMode autoCorrectMode)
        {
            if (q + r + s == 0) return (q, r, s);

            switch (autoCorrectMode)
            {
                case HexAutoCorrectMode.TwoComponent:
                    s = -(q + r);
                    break;
                case HexAutoCorrectMode.Staggered:
                    r = -(q + s);
                    break;
                default:
                    throw new ArgumentException($"{nameof(autoCorrectMode)} has unknown value: ${autoCorrectMode}");
            }
            return (q, r, s);
        }

        public static (int q, int r, int s) AutoCorrectByR(int q, int r, int s, HexAutoCorrectMode autoCorrectMode)
        {
            if (q + r + s == 0) return (q, r, s);

            switch (autoCorrectMode)
            {
                case HexAutoCorrectMode.TwoComponent:
                    s = -(q + r);
                    break;
                case HexAutoCorrectMode.Staggered:
                    s = -(q + r);
                    break;
                default:
                    throw new ArgumentException($"{nameof(autoCorrectMode)} has unknown value: ${autoCorrectMode}");
            }
            return (q, r, s);
        }

        public static (int q, int r, int s) AutoCorrectByS(int q, int r, int s, HexAutoCorrectMode autoCorrectMode)
        {
            if (q + r + s == 0) return (q, r, s);

            switch (autoCorrectMode)
            {
                case HexAutoCorrectMode.TwoComponent:
                    q = -(s + r);
                    break;
                case HexAutoCorrectMode.Staggered:
                    q = -(s + r);
                    break;
                default:
                    throw new ArgumentException($"{nameof(autoCorrectMode)} has unknown value: ${autoCorrectMode}");
            }
            return (q, r, s);
        }
    }
}