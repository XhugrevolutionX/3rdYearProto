 using UnityEngine;

  public readonly struct Hex
  {
      public readonly int Q, R, S;

      public Hex(int q, int r)
      {
          Q = q;
          R = r;
          S = -q - r;
      }

      // The 6 neighbors in cube coordinate offsets
      private static readonly Hex[] Directions = {
          new(1, 0), new(1, -1), new(0, -1),
          new(-1, 0), new(-1, 1), new(0, 1)
      };

      public Hex Neighbor(int direction) =>
          new(Q + Directions[direction].Q, R + Directions[direction].R);

      /// <summary>Returns all 6 neighbours of this hex.</summary>
      public Hex[] Neighbours()
      {
          var result = new Hex[6];
          for (int i = 0; i < 6; i++) result[i] = Neighbor(i);
          return result;
      }

      public int DistanceTo(Hex other) =>
          (Mathf.Abs(Q - other.Q) + Mathf.Abs(R - other.R) + Mathf.Abs(S - other.S)) / 2;

      public static Hex operator +(Hex a, Hex b) => new(a.Q + b.Q, a.R + b.R);
  }