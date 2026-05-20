using System.Text.RegularExpressions;

namespace Votify.Client.Utilities;

/// <summary>
/// Utilidad para parsear y limpiar mensajes de error del servidor.
/// Extrae mensajes JSON de respuestas HTTP y elimina información técnica innecesaria.
/// </summary>
public static class ErrorMessageParser
{
    /// <summary>
    /// Extrae el mensaje de error limpio de una excepción.
    /// Maneja tanto formato "error" como "Error" en JSON.
    /// Si falla la extracción, retorna el mensaje original de forma segura.
    /// </summary>
    /// <param name="exceptionMessage">Mensaje de excepción del servidor</param>
    /// <returns>Mensaje de error limpio para mostrar al usuario</returns>
    public static string ExtractErrorMessage(string exceptionMessage)
    {
        if (string.IsNullOrEmpty(exceptionMessage))
            return "Error desconocido";

        // Intenta extraer JSON con "error" (minúscula)
        var match = Regex.Match(exceptionMessage, "\"error\"\\s*:\\s*\"([^\"]+)\"");
        if (match.Success && match.Groups.Count > 1)
            return match.Groups[1].Value;

        // Intenta extraer JSON con "Error" (mayúscula)
        match = Regex.Match(exceptionMessage, "\"Error\"\\s*:\\s*\"([^\"]+)\"");
        if (match.Success && match.Groups.Count > 1)
            return match.Groups[1].Value;

        // Si no encuentra JSON, intenta buscar contenido después de "BadRequest"
        if (exceptionMessage.Contains("BadRequest"))
        {
            var jsonStart = exceptionMessage.IndexOf('{');
            if (jsonStart > 0)
            {
                var jsonPart = exceptionMessage.Substring(jsonStart);
                match = Regex.Match(jsonPart, "\"error\"\\s*:\\s*\"([^\"]+)\"");
                if (match.Success && match.Groups.Count > 1)
                    return match.Groups[1].Value;
            }
        }

        // Si todo falla, retorna el mensaje original (sin información técnica si es posible)
        return exceptionMessage.Contains("BadRequest")
            ? "Error en la solicitud"
            : exceptionMessage;
    }

    /// <summary>
    /// Verifica si un mensaje de error contiene palabras clave específicas.
    /// Útil para categorizar errores de límite de votos, permisos, etc.
    /// </summary>
    public static bool ContainsKeyword(string message, params string[] keywords)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        var lowerMessage = message.ToLower();
        return keywords.Any(k => lowerMessage.Contains(k.ToLower()));
    }
}
