using System;
using System.Linq;
using System.Collections.Generic;


namespace Synaptafin.PlayModeConsole {
  public static class Utils {
    public static string GetShortTypeName(Type type) {
      return s_shortTypeNames.TryGetValue(type, out string shortName) ? shortName : type.Name;
    }

    public static IEnumerable<(string match, int distance)> FindMatchesByDistance(List<string> choices, string query, int maxDistance = 2) {
      var matches = new List<(string match, int distance)>();

      foreach (string choice in choices) {
        // Normalize strings (optional, can improve matching)
        string normalizedChoice = choice.ToLower();
        string normalizedQuery = query.ToLower();

        int distance = CalculateLevenshteinDistance(normalizedChoice, normalizedQuery);

        if (distance <= maxDistance) {
          matches.Add((choice, distance));
        }
      }

      // Sort by distance (lower distance means a better match)
      return matches.OrderBy(m => m.distance);
    }

    private static readonly Dictionary<Type, string> s_shortTypeNames = new() {
      { typeof(int), "int" },
      { typeof(float), "float" },
      { typeof(double), "double" },
      { typeof(bool), "bool" },
      { typeof(string), "string" },
      { typeof(char), "char" },
      { typeof(long), "long" },
      { typeof(short), "short" },
      { typeof(byte), "byte" },
      { typeof(uint), "uint" },
      { typeof(ulong), "ulong" },
      { typeof(ushort), "ushort" },
      { typeof(sbyte), "sbyte" },
      { typeof(decimal), "decimal" }
    };


    private static int CalculateLevenshteinDistance(string s, string t) {
      int n = s.Length;
      int m = t.Length;
      int[,] d = new int[n + 1, m + 1];

      if (n == 0) return m;
      if (m == 0) return n;

      for (int i = 0; i <= n; i++) d[i, 0] = i;
      for (int j = 0; j <= m; j++) d[0, j] = j;

      for (int i = 1; i <= n; i++) {
        for (int j = 1; j <= m; j++) {
          int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
          d[i, j] = Math.Min(
            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), // Deletion, Insertion
            d[i - 1, j - 1] + cost                      // Substitution
          );
        }
      }
      return d[n, m];
    }
  }
}
