using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace RetailManagementSystem.Services.IServices
{
    public interface IBlobService
    {
        public Task<string> GetBlob(string blobName, string ContainerName);
        public Task<bool> DeleteBlob(string blobName, string ContainerName);
        public Task<string> UploadBlob(string blobName, string ContainerName, IFormFile file);
    }
}
