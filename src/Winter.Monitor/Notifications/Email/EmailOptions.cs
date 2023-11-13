namespace Winter.Monitor.Notifications.Email;

public class EmailOptions
{
    public bool IsEnabled { get; set; } = false;

    public string? Host { get; set; }

    public int Port { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public bool EnableSsl { get; set; }

    public string ReceiveEmails { get; set; } = string.Empty;
}
