using System;
using System.Collections.Generic;
using System.Text;

using FilipunkBlog.Application.Contracts;
using Microsoft.AspNetCore.Hosting;

namespace FilipunkBlog.Infrastructure.Services;

public class ImageService(IWebHostEnvironment env) : IImageService
{
    public async Task<string> UploadAsync(Stream stream, string fileName)
    {
        var ext = Path.GetExtension(fileName);
        var newName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(env.WebRootPath, "uploads", newName);

        using var fs = new FileStream(path, FileMode.Create);
        await stream.CopyToAsync(fs);

        return $"/uploads/{newName}";
    }

    public void Delete(string fileName)
    {
        var path = Path.Combine(env.WebRootPath, fileName.TrimStart('/'));
        if (File.Exists(path)) File.Delete(path);
    }
}