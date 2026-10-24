using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ForumPlatform.Forum.Infrastructure.Search
{
	public class VoyageOptions
	{
		public string ApiKey { get; init; } = "";
		public string Model { get; init; } = "voyage-4";
		public string BaseUrl { get; init; } = "https://api.voyageai.com/v1";
	}

	public class VoyageEmbeddingGenerator : IEmbeddingGenerator
	{
		private readonly HttpClient _http;
		private readonly VoyageOptions _options;

		public VoyageEmbeddingGenerator(HttpClient http, VoyageOptions options)
		{
			_http = http;
			_options = options;
		}

		public bool IsConfigured => !string.IsNullOrWhiteSpace(_options.ApiKey);
		public string ModelName => _options.Model;

		public Task<IReadOnlyList<float[]>> EmbedDocumentsAsync(IReadOnlyList<string> texts, CancellationToken ct)
			=> EmbedAsync(texts, "document", ct);

		public async Task<float[]> EmbedQueryAsync(string text, CancellationToken ct)
			=> (await EmbedAsync(new[] { text }, "query", ct))[0];

		private async Task<IReadOnlyList<float[]>> EmbedAsync(
			IReadOnlyList<string> texts, string inputType, CancellationToken ct)
		{
			if (!IsConfigured)
				throw new InvalidOperationException("Voyage:ApiKey is not configured.");

			if (texts.Count == 0)
				return Array.Empty<float[]>();

			// Dimension is left at the model's default (1024), which matches EmbeddingSettings.Dimensions.
			using var request = new HttpRequestMessage(HttpMethod.Post, "embeddings")
			{
				Content = JsonContent.Create(new { input = texts, model = _options.Model, input_type = inputType }),
			};

			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

			using var response = await _http.SendAsync(request, ct);

			if (!response.IsSuccessStatusCode)
			{
				var body = await response.Content.ReadAsStringAsync(ct);

				throw new HttpRequestException(
					$"Embedding request failed ({(int)response.StatusCode}): {body}", null, response.StatusCode);
			}

			var payload = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken: ct)
				?? throw new InvalidOperationException("Empty embedding response.");

			var vectors = payload.Data.OrderBy(d => d.Index).Select(d => d.Embedding).ToList();

			if (vectors.Count != texts.Count || vectors.Any(v => v.Length != EmbeddingSettings.Dimensions))
				throw new InvalidOperationException(
					$"Unexpected embedding shape: expected {texts.Count} vectors of {EmbeddingSettings.Dimensions} values.");

			return vectors;
		}

		private record EmbeddingResponse([property: JsonPropertyName("data")] List<EmbeddingItem> Data);

		private record EmbeddingItem(
			[property: JsonPropertyName("index")] int Index,
			[property: JsonPropertyName("embedding")] float[] Embedding);
	}
}
