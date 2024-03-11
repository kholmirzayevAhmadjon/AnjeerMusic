using AnjeerMusic.Domain.Commons;

namespace AnjeerMusic.Domain.Users;

public class User : Auditable
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public long ChatId { get; set; }

    public long PhoneNumber { get; set; }
}
