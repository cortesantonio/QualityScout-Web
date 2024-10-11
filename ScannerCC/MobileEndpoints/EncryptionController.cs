using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QualityScout.MobileEndpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {
        [HttpGet("keyAndIv")]
        public IActionResult GetKeyAndIv()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                var key = aes.Key;
                var iv = aes.IV;

                return Ok(new
                {
                    Key = Convert.ToBase64String(key),
                    IV = Convert.ToBase64String(iv)
                });
            }
        }
    }
}
