namespace PackageTracker.Messages.Queries;

public class GetLatestNotificationsQueryResponse
{
    public IReadOnlyCollection<NotificationDto> Notifications { get; init; } = Array.Empty<NotificationDto>();

    public class NotificationDto
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Content { get; set; } = default!;

        public string Type { get; set; } = default!;

        public bool IsRead { get; set; }
    }
}

