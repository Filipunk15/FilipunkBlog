using System;
using System.Collections.Generic;
using System.Text;

namespace FilipunkBlog.Application.Contracts
{
    public interface IImageService
    {
        Task<string> UploadAsync(Stream stream, string fileName);
        void Delete(string fileName);
    }
}
