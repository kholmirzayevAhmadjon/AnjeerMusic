using AnjeerMusic.Domain.Commons;

namespace AnjeerMusic.Domain.UserMusics;

public class UserMusic : Auditable
{
    public long UserId { get; set; }

    public long MusicId { get; set; }
}
