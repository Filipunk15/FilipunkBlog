using Microsoft.AspNetCore.Antiforgery;

namespace FilipunkBlog.Security;

/// <summary>
/// Blazor při renderu interaktivních komponent automaticky zapisuje antiforgery token, a tím pádem
/// i cookie <c>.AspNetCore.Antiforgery.*</c> – i pro anonymního návštěvníka na úvodní stránce, ještě než
/// s čímkoli souhlasil. Veřejné stránky ale žádný formulář s antiforgery nepoužívají (kontakt/komentáře jdou
/// přes SignalR), takže cookie tam není potřeba.
///
/// Dekorátor proto ukládá cookie jen tam, kde se opravdu používá: na přihlašovací stránku (<c>/Account/*</c>)
/// a pro přihlášeného uživatele. Jinde vrací token bez zápisu cookie (<see cref="IAntiforgery.GetTokens"/>).
/// Ověřování (<see cref="ValidateRequestAsync"/>) se nemění.
/// </summary>
public sealed class CookielessPublicAntiforgery(IAntiforgery inner) : IAntiforgery
{
    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        => NeedsCookie(httpContext) ? inner.GetAndStoreTokens(httpContext) : inner.GetTokens(httpContext);

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext) => inner.GetTokens(httpContext);

    public Task<bool> IsRequestValidAsync(HttpContext httpContext) => inner.IsRequestValidAsync(httpContext);

    public Task ValidateRequestAsync(HttpContext httpContext) => inner.ValidateRequestAsync(httpContext);

    public void SetCookieTokenAndHeader(HttpContext httpContext) => inner.SetCookieTokenAndHeader(httpContext);

    private static bool NeedsCookie(HttpContext context)
        => context.User.Identity?.IsAuthenticated == true
           || context.Request.Path.StartsWithSegments("/Account", StringComparison.OrdinalIgnoreCase);
}
