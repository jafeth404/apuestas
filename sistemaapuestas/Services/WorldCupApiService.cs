using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using sistemaapuestas.Models;

namespace sistemaapuestas.Services;

public interface IWorldCupApiService
{
    Task<int> SeedAllMatches();
    Task RefreshMatches();
}

public class WorldCupApiService : IWorldCupApiService
{
    private readonly Data.AppDbContext _db;
    private readonly ILogger<WorldCupApiService> _logger;
    private readonly HttpClient _http;

    private const string BaseUrl = "https://worldcup26.ir";

    public WorldCupApiService(Data.AppDbContext db, ILogger<WorldCupApiService> logger)
    {
        _db = db;
        _logger = logger;
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task<int> SeedAllMatches()
    {
        var games = await FetchGames();
        if (games == null) return 0;

        var existingIds = _db.Partidos
            .Select(p => p.EquipoLocal + p.EquipoVisitante + p.FechaHora.ToString("yyyyMMddHHmm"))
            .ToHashSet();

        var count = 0;
        foreach (var g in games)
        {
            var local = GetTeamName(g.home_team_name_en, g.home_team_label);
            var visitante = GetTeamName(g.away_team_name_en, g.away_team_label);
            if (string.IsNullOrWhiteSpace(local) || string.IsNullOrWhiteSpace(visitante))
                continue;

            var date = ParseDate(g.local_date);
            var key = local + visitante + date.ToString("yyyyMMddHHmm");
            if (existingIds.Contains(key)) continue;

            var fase = MapType(g.type, g.group);

            var partido = new Partido
            {
                EquipoLocal = local,
                EquipoVisitante = visitante,
                FechaHora = date,
                Fase = fase,
                Estado = g.finished == "TRUE" ? "finalizado" : "programado",
                ApuestasHabilitadas = g.finished != "TRUE",
                GolesLocal = g.finished == "TRUE" && int.TryParse(g.home_score, out var hl) ? hl : null,
                GolesVisitante = g.finished == "TRUE" && int.TryParse(g.away_score, out var va) ? va : null
            };

            _db.Partidos.Add(partido);
            await _db.SaveChangesAsync();

            _db.Cuotas.Add(new Cuota
            {
                PartidoId = partido.Id,
                Tipo = $"{local} Gana",
                Valor = GenerateOdds(local, visitante, "local"),
                FechaActualizacion = DateTime.UtcNow
            });
            _db.Cuotas.Add(new Cuota
            {
                PartidoId = partido.Id,
                Tipo = "Empate",
                Valor = Math.Round((decimal)(2.8 + Random.Shared.NextDouble() * 0.6), 2),
                FechaActualizacion = DateTime.UtcNow
            });
            _db.Cuotas.Add(new Cuota
            {
                PartidoId = partido.Id,
                Tipo = $"{visitante} Gana",
                Valor = GenerateOdds(visitante, local, "visitante"),
                FechaActualizacion = DateTime.UtcNow
            });

            existingIds.Add(key);
            count++;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} new matches", count);
        return count;
    }

    public async Task RefreshMatches()
    {
        var games = await FetchGames();
        if (games == null) return;

        var existing = await _db.Partidos.ToListAsync();
        var existingByKey = existing.ToDictionary(p => p.EquipoLocal + p.EquipoVisitante + p.FechaHora.ToString("yyyyMMddHHmm"));

        foreach (var g in games)
        {
            var local = GetTeamName(g.home_team_name_en, g.home_team_label);
            var visitante = GetTeamName(g.away_team_name_en, g.away_team_label);
            if (string.IsNullOrWhiteSpace(local) || string.IsNullOrWhiteSpace(visitante))
                continue;

            var date = ParseDate(g.local_date);
            var key = local + visitante + date.ToString("yyyyMMddHHmm");

            if (existingByKey.TryGetValue(key, out var partido))
            {
                var newEstado = g.finished == "TRUE" ? "finalizado" : "programado";
                var newGolesLocal = g.finished == "TRUE" && int.TryParse(g.home_score, out var hl) ? hl : (int?)null;
                var newGolesVisitante = g.finished == "TRUE" && int.TryParse(g.away_score, out var va) ? va : (int?)null;

                if (partido.Estado != newEstado ||
                    partido.GolesLocal != newGolesLocal ||
                    partido.GolesVisitante != newGolesVisitante)
                {
                    partido.Estado = newEstado;
                    partido.GolesLocal = newGolesLocal;
                    partido.GolesVisitante = newGolesVisitante;
                    partido.ApuestasHabilitadas = g.finished != "TRUE";
                }
            }
            else
            {
                var fase = MapType(g.type, g.group);
                var nuevo = new Partido
                {
                    EquipoLocal = local,
                    EquipoVisitante = visitante,
                    FechaHora = date,
                    Fase = fase,
                    Estado = g.finished == "TRUE" ? "finalizado" : "programado",
                    ApuestasHabilitadas = g.finished != "TRUE",
                    GolesLocal = g.finished == "TRUE" && int.TryParse(g.home_score, out var hl) ? hl : null,
                    GolesVisitante = g.finished == "TRUE" && int.TryParse(g.away_score, out var va) ? va : null
                };
                _db.Partidos.Add(nuevo);
                await _db.SaveChangesAsync();

                _db.Cuotas.Add(new Cuota
                {
                    PartidoId = nuevo.Id,
                    Tipo = $"{local} Gana",
                    Valor = GenerateOdds(local, visitante, "local"),
                    FechaActualizacion = DateTime.UtcNow
                });
                _db.Cuotas.Add(new Cuota
                {
                    PartidoId = nuevo.Id,
                    Tipo = "Empate",
                    Valor = Math.Round((decimal)(2.8 + Random.Shared.NextDouble() * 0.6), 2),
                    FechaActualizacion = DateTime.UtcNow
                });
                _db.Cuotas.Add(new Cuota
                {
                    PartidoId = nuevo.Id,
                    Tipo = $"{visitante} Gana",
                    Valor = GenerateOdds(visitante, local, "visitante"),
                    FechaActualizacion = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Refreshed matches from API");
    }

    private async Task<List<ApiGame>?> FetchGames()
    {
        var response = await _http.GetAsync($"{BaseUrl}/get/games");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var raw = JsonSerializer.Deserialize<JsonElement>(body);

        if (raw.TryGetProperty("games", out var games))
        {
            return JsonSerializer.Deserialize<List<ApiGame>>(games.GetRawText());
        }

        return null;
    }

    private static string GetTeamName(string? nameEn, string? label)
    {
        if (!string.IsNullOrWhiteSpace(nameEn) && nameEn != "0")
            return nameEn;
        if (!string.IsNullOrWhiteSpace(label))
            return label;
        return "";
    }

    private static DateTime ParseDate(string localDate)
    {
        if (DateTime.TryParseExact(localDate, "MM/dd/yyyy HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return dt.ToUniversalTime();
        }
        return DateTime.UtcNow;
    }

    private static string MapType(string type, string group)
    {
        return type switch
        {
            "group" => $"Fase de Grupos - Grupo {group}",
            "r32" => "Ronda de 32",
            "r16" => "Octavos de Final",
            "qf" => "Cuartos de Final",
            "sf" => "Semifinal",
            "third" => "Tercer Puesto",
            "final" => "Final",
            _ => type
        };
    }

    private static decimal GenerateOdds(string team, string opponent, string side)
    {
        var r = Random.Shared.NextDouble();
        return side == "local"
            ? Math.Round((decimal)(1.50 + r * 1.5), 2)
            : Math.Round((decimal)(1.80 + r * 2.0), 2);
    }

    private class ApiGame
    {
        public string id { get; set; } = "";
        public string home_team_name_en { get; set; } = "";
        public string away_team_name_en { get; set; } = "";
        public string home_score { get; set; } = "";
        public string away_score { get; set; } = "";
        public string group { get; set; } = "";
        public string local_date { get; set; } = "";
        public string finished { get; set; } = "";
        public string type { get; set; } = "";
        public string home_team_label { get; set; } = "";
        public string away_team_label { get; set; } = "";
    }
}
