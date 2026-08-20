namespace JwtDayMusic.WebApi.Services.ImportServices
{
    public interface IImportService
    {
        Task<int> ImportFromItunesAsync();   // eklenen şarkı sayısını döndürür
    }
}