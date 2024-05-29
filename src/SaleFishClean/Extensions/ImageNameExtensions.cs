using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SaleFishClean.Domains.Exceptions;

namespace Contract.Extensions
{
    public static class CreateImageNameExtensions
    {
        public static async Task<string> SaveImageAsync(this IFormFile imageFile, IWebHostEnvironment env)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(env.WebRootPath, "lib", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return "/lib/" + fileName;
            }

            return null;
        }
    }
}
