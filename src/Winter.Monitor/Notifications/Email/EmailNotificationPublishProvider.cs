using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Winter.Monitor.Notifications.Email;

public class EmailNotificationPublishProvider : INotificationPublishProvider
{
    private readonly EmailOptions _options;

    public EmailNotificationPublishProvider(IOptionsMonitor<EmailOptions> optionsAccessor)
    {
        _options = optionsAccessor.CurrentValue;
    }

    public async Task PublishAsync(string content, CancellationToken cancellationToken = default)
    {
        if (!_options.IsEnabled)
        {
            return;
        }

        using (var client = await BuildClientAsync())
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("HealthCheck", _options.UserName));

            if (string.IsNullOrEmpty(_options.ReceiveEmails))
            {
                return;
            }

            foreach (var receiveEmail in _options.ReceiveEmails.Split(","))
            {
                message.To.Add(new MailboxAddress("Receiver", receiveEmail));
            }

            message.Subject = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 健康检查";
            message.Body = new TextPart(TextFormat.Plain) { Text = content };

            await client.SendAsync(message);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }

    public async Task<SmtpClient> BuildClientAsync()
    {
        var client = new SmtpClient();

        try
        {
            await ConfigureClient(client);
            return client;
        }
        catch
        {
            client.Dispose();
            throw;
        }
    }

    protected virtual async Task ConfigureClient(SmtpClient client)
    {
        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            GetSecureSocketOption()
        );

        await client.AuthenticateAsync(
            _options.UserName,
            _options.Password
        );
    }

    protected virtual SecureSocketOptions GetSecureSocketOption()
    {
        return _options.EnableSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTlsWhenAvailable;
    }
}
