using AnjeerMusic.Domain.Commons;
using AnjeerMusic.Domain.UserMusics;

namespace AnjeerMusic.Domain.Users;

public class User : Auditable
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public long ChatId { get; set; }

    public long PhoneNumber { get; set; }
    public IEnumerable<UserMusic>userMusics { get; set; }
}