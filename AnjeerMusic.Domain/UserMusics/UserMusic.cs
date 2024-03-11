using AnjeerMusic.Domain.Commons;
using AnjeerMusic.Domain.Musics;
using AnjeerMusic.Domain.Users;

namespace AnjeerMusic.Domain.UserMusics;

public class UserMusic : Auditable
{
    public long UserId { get; set; }
    public User User { get; set; }
    public long MusicId { get; set; }
    public Music Music { get; set; }
}
