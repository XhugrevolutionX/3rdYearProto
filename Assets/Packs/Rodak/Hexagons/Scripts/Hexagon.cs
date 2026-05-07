/**
 * Just Hexagons
 * Author: Radek Titěra
 */

using System;
using UnityEngine;

namespace Rodak.Hexagons
{
    /// <summary>
    /// Immutable definition of a 2D hexagon made out of 3 components Q, R and S.
    /// Defined: Q + R + S = 0
    /// </summary>
    public readonly struct Hexagon
    {
        /// <summary>
        /// The origin hexagon (0, 0, 0).
        /// </summary>
        public static Hexagon Zero => new(0, 0, 0);

        /// <summary>
        /// The direction where Q is unchanged.
        /// </summary>
        public static Hexagon QZero => new(0, -1, 1);

        /// <summary>
        /// The direction where R is unchanged.
        /// </summary>
        public static Hexagon RZero => new(-1, 0, 1);

        /// <summary>
        /// The direction where S is unchanged.
        /// </summary>
        public static Hexagon SZero => new(-1, 1, 0);

        /// <summary>
        /// Returns the closest hexagon based on rounded and precise values.
        /// This method corrects rounding errors to ensure Q + R + S = 0.
        /// </summary>
        /// <returns>The nearest valid Hexagon.</returns>
        public static Hexagon GetNearestHexagon(int q, int r, int s, float qFloat, float rFloat, float sFloat)
        {
            float qDiff = Math.Abs(q - qFloat);
            float rDiff = Math.Abs(r - rFloat);
            float sDiff = Math.Abs(s - sFloat);

            if (qDiff > rDiff && qDiff > sDiff)
                q = -r - s;
            else if (rDiff > sDiff)
                r = -q - s;
            else
                s = -q - r;

            return new(q, r, s);
        }

        /// <summary>
        /// Returns the rounded hexagon based on precise values.
        /// The components are rounded first, then corrected to ensure Q + R + S = 0.
        /// </summary>
        /// <returns>Rounded hexagon</returns>
        public static Hexagon GetNearestHexagonRound(float qFloat, float rFloat, float sFloat)
        {
            int q = (int)Math.Round(qFloat);
            int r = (int)Math.Round(rFloat);
            int s = (int)Math.Round(sFloat);
            return GetNearestHexagon(q, r, s, qFloat, rFloat, sFloat);
        }

        /// <summary>
        /// Returns the rounded hexagon based on precise Q and R values, calculating S.
        /// </summary>
        /// <returns>Rounded hexagon</returns>
        public static Hexagon GetNearestHexagonRound(float qFloat, float rFloat) => GetNearestHexagonRound(qFloat, rFloat, -(qFloat + rFloat));


        /// <summary>
        /// Returns the floored hexagon based on precise values.
        /// The components are floored first, then corrected to ensure Q + R + S = 0.
        /// </summary>
        /// <returns>Floored hexagon</returns>
        public static Hexagon GetNearestHexagonFloor(float qFloat, float rFloat, float sFloat)
        {
            int q = (int)Math.Floor(qFloat);
            int r = (int)Math.Floor(rFloat);
            int s = (int)Math.Floor(sFloat);
            return GetNearestHexagon(q, r, s, qFloat, rFloat, sFloat);
        }

        /// <summary>
        /// Returns the floored hexagon based on precise Q and R values, calculating S.
        /// </summary>
        /// <returns>Floored hexagon</returns>
        public static Hexagon GetNearestHexagonFloor(float qFloat, float rFloat) => GetNearestHexagonFloor(qFloat, rFloat, -(qFloat + rFloat));

        /// <summary>
        /// Calculates the distance between two hexagons.
        /// </summary>
        /// <returns>Distance</returns>
        public static float Distance(Hexagon a, Hexagon b)
        {
            Hexagon diff = a - b;
            return (Math.Abs(diff.q) + Math.Abs(diff.r) + Math.Abs(diff.s)) / 2f;
        }

        /// <summary>
        /// Linearly interpolates between two hexagons.
        /// </summary>
        /// <returns>Interpolated hexagon.</returns>
        public static Hexagon Lerp(Hexagon start, Hexagon end, float t)
        {
            t = Mathf.Clamp01(t);

            float qFloat = Mathf.Lerp(start.q, end.q, t);
            float rFloat = Mathf.Lerp(start.r, end.r, t);
            float sFloat = Mathf.Lerp(start.s, end.s, t);

            return GetNearestHexagonRound(qFloat, rFloat, sFloat);
        }

        /// <summary>
        /// Checks whether the values represent a valid hexagon.
        /// Q + R + S = 0
        /// </summary>
        /// <returns>True if valid, false otherwise.</returns>
        public static bool IsValid(int q, int r, int s)
        {
            return (q + r + s) == 0;
        }

        /// <summary>Rounds the scalar multiplication of a hexagon to the nearest integer coordinates.</summary>
        public static Hexagon MultRound(Hexagon a, float scalar) => GetNearestHexagonRound(a.q * scalar, a.r * scalar, a.s * scalar);
        /// <summary>Floors the scalar multiplication of a hexagon to the nearest integer coordinates.</summary>
        public static Hexagon MultFloor(Hexagon a, float scalar) => GetNearestHexagonFloor(a.q * scalar, a.r * scalar, a.s * scalar);

        /// <summary>Rounds the scalar division of a hexagon to the nearest integer coordinates.</summary>
        public static Hexagon DivRound(Hexagon a, float scalar) => GetNearestHexagonRound(a.q / scalar, a.r / scalar, a.s / scalar);
        /// <summary>Floors the scalar division of a hexagon to the nearest integer coordinates.</summary>
        public static Hexagon DivFloor(Hexagon a, float scalar) => GetNearestHexagonFloor(a.q / scalar, a.r / scalar, a.s / scalar);

        public static Hexagon operator +(Hexagon a) => a;
        public static Hexagon operator -(Hexagon a) => new(-a.q, -a.r, -a.s);

        public static Hexagon operator +(Hexagon a, Hexagon b) => new(a.q + b.q, a.r + b.r, a.s + b.s);
        public static Hexagon operator -(Hexagon a, Hexagon b) => new(a.q - b.q, a.r - b.r, a.s - b.s);

        public static Hexagon operator *(Hexagon a, float scalar) => MultRound(a, scalar);
        public static Hexagon operator /(Hexagon a, float scalar) => DivRound(a, scalar);

        public static bool operator ==(Hexagon a, Hexagon b) => a.q == b.q && a.r == b.r;
        public static bool operator !=(Hexagon a, Hexagon b) => a.q != b.q && a.r != b.r;

        /// <summary>
        /// Q axis position.
        /// </summary>
        public readonly int q;
        /// <summary>
        /// R axis position.
        /// </summary>
        public readonly int r;
        /// <summary>
        /// S axis position.
        /// </summary>
        public readonly int s;

        /// <summary>
        /// New hexagon
        /// </summary>
        /// <exception cref="ArgumentException">Q + R + S != 0</exception>
        public Hexagon(int q, int r, int s)
        {
            if (!IsValid(q, r, s))
                throw new ArgumentException($"Invalid hexagon, {nameof(q)} + {nameof(r)} + {nameof(s)} must be equal to {0}");
            this.q = q;
            this.r = r;
            this.s = s;
        }

        /// <summary>
        /// New hexagon with S = -(Q + R).
        /// </summary>
        public Hexagon(int q, int r) : this(q, r, -(q + r)) { }

        public override int GetHashCode()
        {
            return HashCode.Combine(q, r, s);
        }

        public override bool Equals(object obj)
        {
            if (obj is Hexagon other)
            {
                return other == this;
            }
            return false;
        }

        public override string ToString()
        {
            return $"Hex[{q}, {r}, {s}]";
        }
    }
}