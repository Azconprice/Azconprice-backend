using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Supabase;                 // umbrella package
using Supabase.Storage;
using System;

namespace Infrastructure.Services;

public class SupabaseStorageService(Supabase.Client supabase, IOptions<SupabaseSettings> settings) : IBucketService
{
    private readonly Supabase.Client _supabase = supabase;
    private readonly SupabaseOptions _options = new();
    private readonly string _bucket = settings.Value.BucketName;

    public async Task<string> UploadAsync(IFormFile file)
    {
        var path = $"profile/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        var bucket = _supabase.Storage.From(_bucket);
        await using var stream = file.OpenReadStream();

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        await bucket.Upload(fileBytes, path,null);

        return path;
    }

    public async Task<string> GetSignedUrlAsync(string path, int expiresInSeconds = 300)
    {
        var bucket = _supabase.Storage.From(_bucket);
        var url = await bucket.CreateSignedUrl(path, expiresInSeconds);
        return url;                 
    }
}
