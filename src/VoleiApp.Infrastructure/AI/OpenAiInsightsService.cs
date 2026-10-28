using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VoleiApp.Application.DTOs.Insights;
using VoleiApp.Application.DTOs.Partida;
using VoleiApp.Application.Interfaces.Services;

namespace VoleiApp.Infrastructure.AI
{
    public class OpenAiInsightsService : IAiInsightsService
    {
        private readonly HttpClient _httpClient;
        private readonly AiOptions _options;

        public OpenAiInsightsService(HttpClient httpClient, IOptions<AiOptions> aiOptions)
        {
            _httpClient = httpClient;
            _options = aiOptions.Value;
        }

        public async Task<InsightPartidaResponseDTO> GerarInsightPartidaAsync(PartidaResponseDTO partida, GerarInsightPartidaRequestDTO? request, CancellationToken ct = default)
        {
            var apiKey = _options.ApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Chave da IA não configurada.");

            var prompt = MontarPrompt(partida, request?.ContextoAdicional);

            var payload = new
            {
                model = _options.Model,
                messages = new object[]
                {
                new { role = "system", content = "Você é um assistente de análise de partidas de vôlei. Seja objetivo e seguro." },
                new { role = "user", content = prompt }
                },
                max_tokens = _options.MaxOutputTokens,
                temperature = 0.2
            };

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(requestMessage, ct);
            var content = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Falha na IA: {(int)response.StatusCode} - {content}");

            var parsed = JsonSerializer.Deserialize<OpenAiChatResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Resposta inválida da IA.");

            var texto = parsed.Choices?.FirstOrDefault()?.Message?.Content?.Trim();
            if (string.IsNullOrWhiteSpace(texto))
                throw new InvalidOperationException("Resposta da IA sem conteúdo.");

            return new InsightPartidaResponseDTO
            (
                texto,
                new List<string>(),
                _options.Model,
                parsed.Usage?.TotalTokens,
                false
            );
        }

        private string MontarPrompt(PartidaResponseDTO partida, string? contextoAdicional)
        {
            static string Sanitizar(string? texto, int maxChars)
            {
                if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

                var clean = new string(texto
                    .Where(c => !char.IsControl(c) || c is '\n' or '\r' or '\t')
                    .ToArray());

                return clean.Length > maxChars ? clean[..maxChars] : clean;
            }

            var contexto = Sanitizar(contextoAdicional, _options.MaxInputChars);

            var sb = new StringBuilder();
            sb.AppendLine("Analise a partida e retorne um resumo curto + 3 sugestões práticas.");
            sb.AppendLine($"PartidaId: {partida.ID}");
            sb.AppendLine($"Encerrada: {partida.Encerrada}");
            sb.AppendLine($"Time A: {partida.TimeA.Nome} - atletas: {string.Join(", ", partida.TimeA.Atletas.Select(a => a.Nome))}");
            sb.AppendLine($"Time B: {partida.TimeB.Nome} - atletas: {string.Join(", ", partida.TimeB.Atletas.Select(a => a.Nome))}");
            sb.AppendLine($"Reservas: {string.Join(", ", partida.Reservas.Select(r => r.Nome))}");
            sb.AppendLine($"Qtd Substituições: {partida.Substituicoes.Count}");

            if (!string.IsNullOrWhiteSpace(contexto))
                sb.AppendLine($"Contexto adicional do usuário: {contexto}");

            sb.AppendLine("Formato esperado:");
            sb.AppendLine("Resumo: ...");
            sb.AppendLine("Sugestões:");
            sb.AppendLine("- ...");
            sb.AppendLine("- ...");
            sb.AppendLine("- ...");

            return sb.ToString();
        }

        private sealed class OpenAiChatResponse
        {
            public List<Choice>? Choices { get; set; }
            public Usage? Usage { get; set; }
        }

        private sealed class Choice
        {
            public Message? Message { get; set; }
        }

        private sealed class Message
        {
            public string? Content { get; set; }
        }

        private sealed class Usage
        {
            public int? TotalTokens { get; set; }
        }
    }
}
