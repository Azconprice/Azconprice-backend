using Application.Models;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Supabase;
using Supabase.Storage;
using System;
using System.IO;
using System.Threading.Tasks;
using FileOptions = Supabase.Storage.FileOptions;

namespace Infrastructure.Services;

public class SupabaseStorageService : IBucketService
{
    private readonly Supabase.Client _supabase;
    private readonly string _bucket;

    public SupabaseStorageService(Supabase.Client supabase, SupabaseSettings settings)
    {
        _supabase = supabase;
        _bucket = settings.BucketName;
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        await _supabase.InitializeAsync();

        var path = $"profile/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var bucket = _supabase.Storage.From(_bucket);

        await using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        await bucket.Upload(fileBytes, path, new FileOptions
        {
            ContentType = file.ContentType,
            CacheControl = "3600"
        });

        return path;
    }

    public async Task<string> GetSignedUrlAsync(string path, int expiresInSeconds = 300)
    {
        await _supabase.InitializeAsync();

        var bucket = _supabase.Storage.From(_bucket);
        var url = await bucket.CreateSignedUrl(path, expiresInSeconds);
        return url;
    }

    public async Task<string> UploadAsync(IFormFile file, string fileName)
    {
        await _supabase.InitializeAsync();

        var path = $"profile/{fileName}";
        var bucket = _supabase.Storage.From(_bucket);

        await using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        // The third parameter 'upsert' set to true will override the file if it exists
        await bucket.Upload(fileBytes, path, new FileOptions
        {
            ContentType = file.ContentType,
            CacheControl = "3600",
            Upsert = true
        });

        return path;
    }

    public async Task<string> UploadExcelAsync(IFormFile file, string firstName, string lastName, string email, string userId)
    {
        await _supabase.InitializeAsync();
        var bucket = _supabase.Storage.From(_bucket);

        // Build the file name
        string safeFirstName = firstName.Replace(" ", "-");
        string safeLastName = lastName.Replace(" ", "-");
        string safeEmail = email.Replace("@", "_at_").Replace(".", "_");
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss");
        string guid = Guid.NewGuid().ToString();
        string extension = Path.GetExtension(file.FileName);

        string fileName = $"{timestamp}__{safeFirstName}-{safeLastName}__{userId}__{safeEmail}__{guid}{extension}";
        string path = $"excel-files/{fileName}";

        // Read file stream
        await using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        await bucket.Upload(fileBytes, path, new FileOptions
        {
            ContentType = file.ContentType,
            CacheControl = "3600",
            Upsert = true
        });

        return path;
    }

    public async Task<string> UploadTaxIdAsync(IFormFile file,string fileName)
    {
        await _supabase.InitializeAsync();

        var path = $"tax/{fileName}";
        var bucket = _supabase.Storage.From(_bucket);

        await using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        // The third parameter 'upsert' set to true will override the file if it exists
        await bucket.Upload(fileBytes, path, new FileOptions
        {
            ContentType = file.ContentType,
            CacheControl = "3600",
            Upsert = true
        });

        return path;
    }
}
