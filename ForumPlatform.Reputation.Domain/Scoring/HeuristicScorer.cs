using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace ForumPlatform.Reputation.Domain.Scoring
{
	public static class HeuristicScorer
	{
		private const int LengthCapWords = 175;
		private const int MaxCountedLinks = 3;

		public static HeuristicSubScores Score(string content)
		{
			var wordCount = CountWords(content);

			return new HeuristicSubScores(
				ScoreLength(wordCount),
				ScoreSources(content),
				ScoreStructure(content),
				ScoreSpamPenalty(content, wordCount),
				ContainsCodeOrMath(content)
			);
		}

		private static decimal ScoreLength(int wordCount)
		{
			if (wordCount <= 0) return 0m;

			var score = Math.Log(wordCount + 1) / Math.Log(LengthCapWords + 1);

			return (decimal)Math.Clamp(score, 0.0, 1.0);
		}

		private static decimal ScoreSources(string content)
		{
			var linkCount = Regex.Matches(content, @"https?://\S+").Count;

			return Math.Min(linkCount, MaxCountedLinks) / (decimal)MaxCountedLinks;
		}

		/// <summary>
		/// Rewards paragraph breaks, list markers, and code fences - count based,
		/// capped at 1.0 so a post can't inflate this indefinitely by repeating the same structural marker many times
		/// </summary>
		private static decimal ScoreStructure(string content)
		{
			var paragraphBreaks = Regex.Matches(content, @"\n\s*\n").Count;
			var listMarkers = Regex.Matches(content, @"(?m)^\s*[-*\d]+[.)]\s").Count;
			var codeBlocks = Regex.Matches(content, @"```").Count / 2; // Pairs of fences

			var structuralSignals = paragraphBreaks + listMarkers + codeBlocks;

			// 3+ distinct structural signals -> full score;
			return Math.Min(structuralSignals / 3.0m, 1.0m);
		}

		private static decimal ScoreSpamPenalty(string content, int wordCount)
		{
			if (string.IsNullOrWhiteSpace(content))
				return 1.0m;

			decimal penalty = 0m;

			if (content.Length > 20)
			{
				var letters = content.Where(char.IsLetter).ToList();

				if (letters.Count > 0)
				{
					var upperRatio = letters.Count(char.IsUpper) / (decimal)letters.Count;

					if (upperRatio > 0.7m)
						penalty += 0.4m;
				}
			}

			// Repeated-character spam.
			if (Regex.IsMatch(content, @"(.)\1{6,}"))
				penalty += 0.3m;

			if (wordCount < 5)
				penalty += 0.3m;

			return Math.Clamp(penalty, 0m, 1m);
		}

		private static bool ContainsCodeOrMath(string content)
		{
			var hasCodeBlock = content.Contains("```");
			var hasInlineCode = Regex.IsMatch(content, @"`[^`\n]+`");

			// Rough math-notation heuristic: LaTeX-style delimiters or common math symbol clustered together.
			// Deliberately permissive - false positives here just mean an unnecessary LLM call, not an incorrect score.
			var hasMathNotation = Regex.IsMatch(content, @"\$[^$\n]+\$|\\\(|\\\[");

			return hasCodeBlock || hasInlineCode || hasMathNotation;
		}

		private static int CountWords(string content)
		{
			return string.IsNullOrWhiteSpace(content)
				? 0
				: content.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
		}
	}
}
