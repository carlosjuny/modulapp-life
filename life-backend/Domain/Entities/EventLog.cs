using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class EventLog : BaseEntity, IHouseholdScoped
{
    public Guid HouseholdId { get; set; }

    public Household? Household { get; set; }

    public Guid ActorUserId { get; set; }

    public User? ActorUser { get; set; }

    public EventLogType Type { get; set; }

    public string Payload { get; set; } = "{}";

    public DateTime CreatedAt { get; set; }
}
