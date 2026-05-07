using System.IO;

namespace Rodak.Hexagons.HexIO
{
    /// <summary>
    /// Utility class that allow Hexagon to be saved into binary.
    /// </summary>
    public static class HexagonBinaryExtensions
    {
        /// <summary>
        /// Write the hexagon to the binary writer.
        /// </summary>
        public static void Write(this BinaryWriter bw, Hexagon hexagon)
        {
            bw.Write(hexagon.q);
            bw.Write(hexagon.r);
        }

        /// <summary>
        /// Read the hexagon from the binary reader.
        /// </summary>
        /// <returns>The read Hexagon object.</returns>
        public static Hexagon ReadHexagon(this BinaryReader br)
        {
            int q = br.ReadInt32();
            int r = br.ReadInt32();

            return new(q, r);
        }
    }
}