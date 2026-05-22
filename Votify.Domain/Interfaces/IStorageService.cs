namespace Votify.Domain.Interfaces
{
    public interface IStorageService
    {
        Task<string> SubirArchivoAsync(string bucket, Stream archivo, string nombreArchivo, string contentType);
        Task EliminarArchivoAsync(string bucket, string rutaArchivo);
    }
}