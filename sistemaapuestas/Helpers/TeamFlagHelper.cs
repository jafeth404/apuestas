namespace sistemaapuestas.Helpers;

public static class TeamFlagHelper
{
    private static readonly Dictionary<string, string> CountryCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Mexico", "mx" },
        { "South Africa", "za" },
        { "South Korea", "kr" },
        { "Czech Republic", "cz" },
        { "Canada", "ca" },
        { "Bosnia and Herzegovina", "ba" },
        { "Qatar", "qa" },
        { "Switzerland", "ch" },
        { "Brazil", "br" },
        { "Morocco", "ma" },
        { "Haiti", "ht" },
        { "Scotland", "gb-sct" },
        { "United States", "us" },
        { "Paraguay", "py" },
        { "Australia", "au" },
        { "Turkey", "tr" },
        { "Germany", "de" },
        { "Ivory Coast", "ci" },
        { "Ecuador", "ec" },
        { "Curaçao", "cw" },
        { "Netherlands", "nl" },
        { "Japan", "jp" },
        { "Sweden", "se" },
        { "Tunisia", "tn" },
        { "Belgium", "be" },
        { "Egypt", "eg" },
        { "Iran", "ir" },
        { "New Zealand", "nz" },
        { "Spain", "es" },
        { "Cape Verde", "cv" },
        { "Saudi Arabia", "sa" },
        { "Uruguay", "uy" },
        { "France", "fr" },
        { "Senegal", "sn" },
        { "Iraq", "iq" },
        { "Norway", "no" },
        { "Argentina", "ar" },
        { "Algeria", "dz" },
        { "Austria", "at" },
        { "Jordan", "jo" },
        { "Portugal", "pt" },
        { "Democratic Republic of the Congo", "cd" },
        { "Uzbekistan", "uz" },
        { "Colombia", "co" },
        { "England", "gb-eng" },
        { "Croatia", "hr" },
        { "Ghana", "gh" },
        { "Panama", "pa" },
    };

    public static string GetFlagUrl(string? teamName)
    {
        if (teamName != null && CountryCodes.TryGetValue(teamName, out var code))
            return $"https://flagcdn.com/w40/{code}.png";
        return "";
    }

    public static string GetTeamLabel(string? teamName)
    {
        if (string.IsNullOrWhiteSpace(teamName)) return "";
        if (CountryCodes.TryGetValue(teamName, out var code))
            return $"<img src=\"https://flagcdn.com/w20/{code}.png\" class=\"me-1\" style=\"vertical-align:middle\"> {teamName}";
        return teamName;
    }
}
