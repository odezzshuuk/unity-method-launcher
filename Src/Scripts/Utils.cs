using System;
using System.Linq;
using System.Collections.Generic;


namespace Odezzshuuk.Workflow.MethodLauncher {
  public static class Utils {
    public static string GetShortTypeName(Type type) {
      return s_ShortTypeNames.TryGetValue(type, out string shortName) ? shortName : type.Name;
    }

    private static readonly Dictionary<Type, string> s_ShortTypeNames = new() {
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
  }

  public static class FuzzyMatcher {
    public class MatchResult {
      public string Text { get; set; }
      public float Score { get; set; }
      public List<int> MatchedIndices { get; set; }

      public MatchResult(string text, float score, List<int> matchedIndices) {
        Text = text;
        Score = score;
        MatchedIndices = matchedIndices;
      }
    }

    /// <summary>
    /// Performs fuzzy matching on a list of strings against a query.
    /// Returns results sorted by score (highest first).
    /// </summary>
    // public static List<MatchResult> Match(IEnumerable<string> candidates, string query, int maxResults = -1) {
    //   if (string.IsNullOrEmpty(query)) {
    //     return candidates.Select(c => new MatchResult(c, 0f, new List<int>())).ToList();
    //   }
    //
    //   var results = new List<MatchResult>();
    //
    //   foreach (string candidate in candidates) {
    //     float score = CalculateScore(candidate, query, out List<int> matchedIndices);
    //     if (score > 0) {
    //       results.Add(new MatchResult(candidate, score, matchedIndices));
    //     }
    //   }
    //
    //   results.Sort((a, b) => b.Score.CompareTo(a.Score));
    //
    //   if (maxResults > 0 && results.Count > maxResults) {
    //     results = results.GetRange(0, maxResults);
    //   }
    //
    //   return results;
    // }
    //
    /// <summary>
    /// Calculates the fuzzy match score for a single candidate string.
    /// Returns 0 if no match, higher scores for better matches.
    /// </summary>
    public static float CalculateScore(string candidate, string query) {
      List<int> matchedIndices = new();

      if (string.IsNullOrEmpty(candidate) || string.IsNullOrEmpty(query)) {
        return 0f;
      }

      // Check for exact match (case-insensitive)
      if (candidate.Equals(query, StringComparison.OrdinalIgnoreCase)) {
        for (int i = 0; i < candidate.Length; i++) {
          matchedIndices.Add(i);
        }
        return 1000f + (candidate.Equals(query, StringComparison.Ordinal) ? 100f : 0f);
      }

      // Check if candidate contains all query characters in order
      int candidateIdx = 0;
      int queryIdx = 0;
      var potentialMatches = new List<int>();

      while (candidateIdx < candidate.Length && queryIdx < query.Length) {
        if (char.ToLowerInvariant(candidate[candidateIdx]) == char.ToLowerInvariant(query[queryIdx])) {
          potentialMatches.Add(candidateIdx);
          queryIdx++;
        }
        candidateIdx++;
      }

      // No match if not all query characters found
      if (queryIdx < query.Length) {
        return 0f;
      }

      matchedIndices = potentialMatches;

      // Calculate score based on various factors
      float score = 0f;

      // Base score for finding all characters
      score += 100f;

      // Bonus for prefix match
      if (candidate.StartsWith(query, StringComparison.OrdinalIgnoreCase)) {
        score += 200f;
        if (candidate.StartsWith(query, StringComparison.Ordinal)) {
          score += 50f; // Extra bonus for case match
        }
      }

      // Bonus for consecutive character matches
      int consecutiveCount = 1;
      int maxConsecutive = 1;
      for (int i = 1; i < matchedIndices.Count; i++) {
        if (matchedIndices[i] == matchedIndices[i - 1] + 1) {
          consecutiveCount++;
          maxConsecutive = Math.Max(maxConsecutive, consecutiveCount);
        } else {
          consecutiveCount = 1;
        }
      }
      score += maxConsecutive * 100f;

      // Bonus for word boundary matches (camelCase, PascalCase, snake_case, etc.)
      int wordBoundaryMatches = 0;
      for (int i = 0; i < matchedIndices.Count; i++) {
        int idx = matchedIndices[i];
        if (IsWordBoundary(candidate, idx)) {
          wordBoundaryMatches++;
          score += 15f;
        }
      }

      // Bonus for case-sensitive matches
      for (int i = 0; i < matchedIndices.Count; i++) {
        if (candidate[matchedIndices[i]] == query[i]) {
          score += 5f;
        }
      }

      // Penalty for gaps between matches (earlier matches are better)
      float averagePosition = (float)matchedIndices.Average();
      float positionPenalty = averagePosition / candidate.Length * 50f;
      score -= positionPenalty;

      // Bonus for matching a higher percentage of the candidate
      float matchPercentage = (float)query.Length / candidate.Length;
      score += matchPercentage * 100f;

      // Penalty for longer candidates (shorter matches are generally better)
      score -= (candidate.Length - query.Length) * 0.5f;

      return Math.Max(0f, score);
    }

    /// <summary>
    /// Checks if a character position is at a word boundary.
    /// </summary>
    private static bool IsWordBoundary(string text, int index) {
      if (index == 0) return true;

      char current = text[index];
      char previous = text[index - 1];

      // Check for uppercase after lowercase (camelCase, PascalCase)
      if (char.IsUpper(current) && char.IsLower(previous)) {
        return true;
      }

      // Check for letter after non-letter (snake_case, kebab-case, etc.)
      if (char.IsLetter(current) && !char.IsLetter(previous)) {
        return true;
      }

      // Check for digit boundaries
      if (char.IsDigit(current) && !char.IsDigit(previous)) {
        return true;
      }

      return false;
    }
  }
}
