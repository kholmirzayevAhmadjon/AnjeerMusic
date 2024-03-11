using AnjeerMusic.Domain.Commons;

namespace AnjeerMusic.Domain.Musics;

public class Music : Auditable
{
    public string Name { get; set; }

    public byte[] MusicData { get; set; }
}
