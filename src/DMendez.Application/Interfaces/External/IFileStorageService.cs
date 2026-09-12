using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DMendez.Application.Interfaces.External
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    }
}
