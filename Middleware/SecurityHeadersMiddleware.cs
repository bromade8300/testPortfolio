namespace testPortfolio.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Empêche le navigateur de deviner le type MIME (protection contre les attaques MIME sniffing)
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Empêche l'affichage du site dans une iframe (protection contre le clickjacking)
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // Protection XSS du navigateur (legacy, mais toujours utile pour les anciens navigateurs)
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Contrôle les informations envoyées dans le header Referer
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Empêche le site d'être affiché si détecté comme téléchargement (IE)
            context.Response.Headers.Append("X-Download-Options", "noopen");

            // Désactive la mise en cache des données sensibles
            context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate, proxy-revalidate");
            context.Response.Headers.Append("Pragma", "no-cache");

            // Content Security Policy - Restrictive mais permet Bootstrap et jQuery
            var csp = "default-src 'self'; " +
                      "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                      "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                      "font-src 'self' https://fonts.gstatic.com; " +
                      "img-src 'self' data: blob:; " +
                      "frame-ancestors 'none'; " +
                      "form-action 'self'; " +
                      "base-uri 'self';";
            context.Response.Headers.Append("Content-Security-Policy", csp);

            // Permissions Policy (anciennement Feature-Policy)
            context.Response.Headers.Append("Permissions-Policy",
                "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

            await _next(context);
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
