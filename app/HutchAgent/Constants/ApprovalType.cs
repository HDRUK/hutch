using System.Text.Json.Serialization;

namespace HutchAgent.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApprovalType
{
  FullyApproved,
  PartiallyApproved,
  NotApproved
}
