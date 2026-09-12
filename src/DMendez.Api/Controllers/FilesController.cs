using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DMendez.Application.Interfaces.External;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMendez.Api.Controllers
{
    /// <summary>
    /// Servicio para la subida y gestión de archivos multimedia (imágenes de productos, categorías y combos).
    /// </summary>
    public class FilesController : BaseApiController
    {
        private readonly IFileStorageService _fileStorageService;
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public FilesController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Sube una imagen al servidor y retorna su URL pública accesible.
        /// </summary>
        /// <param name="file">Archivo de imagen a subir (formatos: JPG, PNG, WEBP, máx 5MB).</param>
        /// <param name="folder">Carpeta destino (ej. "products", "categories", "combos"). Por defecto: "general".</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="200">Archivo subido correctamente, retorna la URL.</response>
        /// <response code="400">Si el archivo no es válido, excede el tamaño o tiene un formato no permitido.</response>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FileUploadResponse>> Upload(
            [FromForm] IFormFile file, 
            [FromQuery] string folder = "general", 
            CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No se ha proporcionado ningún archivo." });

            if (file.Length > MaxFileSizeInBytes)
                return BadRequest(new { message = "El tamaño del archivo no puede superar los 5 MB." });

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return BadRequest(new { message = $"Extensión no permitida. Formatos admitidos: {string.Join(", ", AllowedExtensions)}." });

            using var stream = file.OpenReadStream();
            var relativeUrl = await _fileStorageService.SaveFileAsync(stream, file.FileName, folder, cancellationToken);

            return Ok(new FileUploadResponse
            {
                Url = relativeUrl,
                FileName = file.FileName,
                SizeBytes = file.Length
            });
        }

        /// <summary>
        /// Elimina un archivo previamente subido a través de su URL relativa.
        /// </summary>
        /// <param name="url">URL relativa del archivo a eliminar (ej. "/uploads/products/xyz.png").</param>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <response code="204">Archivo eliminado satisfactoriamente.</response>
        /// <response code="400">Si la URL es nula o vacía.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete([FromQuery] string url, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest(new { message = "La URL del archivo es requerida." });

            await _fileStorageService.DeleteFileAsync(url, cancellationToken);
            return NoContent();
        }
    }

    /// <summary>
    /// Respuesta devuelta tras una subida de archivo exitosa.
    /// </summary>
    public class FileUploadResponse
    {
        /// <summary>
        /// URL relativa para acceder o vincular la imagen subida.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Nombre original del archivo subido.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Tamaño del archivo en bytes.
        /// </summary>
        public long SizeBytes { get; set; }
    }
}
