namespace JwtDayMusic.WebApi.Dtos
{
    public class LikeResultDto
    {
        public bool Liked { get; set; }   // true=beğenildi, false=geri alındı
        public string Message { get; set; }
    }
}