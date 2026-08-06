using System.Net;
using System.Net.Mail;
using System.Text;
using ShippingRequestSystem.Models;

namespace ShippingRequestSystem.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, IWebHostEnvironment env, ILogger<EmailService> logger)
    {
        _config = config;
        _env = env;
        _logger = logger;
    }

    public async Task SendFinanceSummaryAsync(ShippingRequest request)
    {
        var host = _config["Smtp:Host"];
        var recipients = _config.GetSection("Smtp:FinanceRecipients").Get<string[]>()
            ?? Array.Empty<string>();

        if (string.IsNullOrWhiteSpace(host) || recipients.Length == 0)
        {
            _logger.LogWarning(
                "Smtp no está configurado (Smtp:Host / Smtp:FinanceRecipients) — no se envió " +
                "el resumen de facturación de la solicitud {Id}", request.Id);
            throw new Exception(
                "El correo a Finanzas no está configurado (appsettings: sección \"Smtp\").");
        }

        var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
        var enableSsl = !bool.TryParse(_config["Smtp:EnableSsl"], out var ssl) || ssl;
        var from = _config["Smtp:From"] ?? "shippingrequest@meax.mx";
        var user = _config["Smtp:User"];
        var password = _config["Smtp:Password"];

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = string.IsNullOrEmpty(user) ? null : new NetworkCredential(user, password)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(from),
            Subject = $"Facturación — {request.Folio ?? $"SR-{request.Id}"}",
            Body = BuildSummaryHtml(request),
            IsBodyHtml = true
        };
        foreach (var r in recipients) message.To.Add(r);

        var selectedQuote = request.Quotes.FirstOrDefault(q => q.IsSelected);
        if (!string.IsNullOrEmpty(selectedQuote?.FilePath))
        {
            var fullPath = Path.Combine(_env.WebRootPath,
                selectedQuote.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                message.Attachments.Add(new Attachment(fullPath));
        }

        await client.SendMailAsync(message);
        _logger.LogInformation(
            "Resumen de facturación enviado a Finanzas para la solicitud {Id}", request.Id);
    }

    private static string BuildSummaryHtml(ShippingRequest request)
    {
        var sb = new StringBuilder();
        sb.Append($"<h2>{request.Folio ?? $"SR-{request.Id}"} — {request.RecipientBusinessName}</h2>");

        sb.Append("<table border='1' cellpadding='6' cellspacing='0'>");
        sb.Append(Row("Solicitante", $"{request.RequestorName} ({request.RequestorEmail})"));
        sb.Append(Row("Departamento / Unidad", $"{request.Department} / {request.BusinessUnit}"));
        sb.Append(Row("Fecha de solicitud", request.RequestDate.ToString("dd/MM/yyyy")));
        sb.Append(Row("Dirección", request.Direction.ToString()));
        sb.Append(Row("Destinatario", $"{request.RecipientBusinessName} — {request.RecipientAddress}"));
        sb.Append(Row("Tipo de Factura", request.InvoiceType.ToString()));
        sb.Append(Row("No. de Factura", request.InvoiceNumber ?? "—"));
        sb.Append(Row("Incoterm", request.Incoterm ?? "—"));
        sb.Append(Row("Costo Aproximado (USD)", request.ApproximateCost.ToString("N2")));
        sb.Append(Row("Responsabilidad de Pago", request.PaymentResponsibility.ToString()));
        if (request.PaymentResponsibility == PaymentResponsibility.Receiver)
            sb.Append(Row("Paquetería / Cuenta", $"{request.Carrier} / {request.ParcelAccount}"));

        if (request.PremiumFreightInfo is not null)
        {
            var pf = request.PremiumFreightInfo;
            sb.Append(Row("Folio Premium Freight", pf.Folio ?? "—"));
            sb.Append(Row("Forwarder", pf.Forwarder ?? "—"));
        }
        sb.Append("</table>");

        sb.Append("<h3>Items</h3><table border='1' cellpadding='6' cellspacing='0'>");
        sb.Append("<tr><th>No. Parte</th><th>Descripción</th><th>Cant.</th>" +
                  "<th>Peso Neto</th><th>Peso Bruto</th><th>Monto</th></tr>");
        foreach (var pkg in request.Packages)
        {
            foreach (var item in pkg.Items)
            {
                sb.Append($"<tr><td>{item.PartNumber}</td><td>{item.Description}</td>" +
                          $"<td>{item.Quantity}</td><td>{item.NetWeight}</td>" +
                          $"<td>{item.GrossWeight}</td><td>${item.Amount:N2}</td></tr>");
            }
        }
        sb.Append("</table>");

        return sb.ToString();
    }

    private static string Row(string label, string value) =>
        $"<tr><td><strong>{label}</strong></td><td>{value}</td></tr>";
}
