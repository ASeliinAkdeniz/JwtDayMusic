namespace JwtDayMusic.WebApi.Dtos
{
    public class ItunesResponse
    {
        public int resultCount { get; set; }
        public List<ItunesTrack> results { get; set; }
    }

    public class ItunesTrack
    {
        public string trackName { get; set; }
        public string artistName { get; set; }
        public string artworkUrl100 { get; set; }
        public string previewUrl { get; set; }
        public long trackTimeMillis { get; set; }
        public string primaryGenreName { get; set; }
    }
}