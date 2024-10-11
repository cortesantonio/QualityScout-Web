using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ScannerCC.Models;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QualityScout.MobileEndpoints
{

    public class LoginRequest
    {
        public string Rut { get; set; }
        public string Password { get; set; }
    }
    public class PasswordVerificationData
    {
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class AuthApi : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthApi(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<AuthApi>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        [HttpPost("login")]
        public IActionResult AuthLogin([FromBody] LoginRequest request)
        {
            var descryptedRut = DecryptText(request.Rut);
            var descryptedPass = DecryptText(request.Password);

            var userFound = _context.Usuario
                .FirstOrDefault(x => x.Rut == descryptedRut);


            if (userFound != null)
            {
                var verificationData = new PasswordVerificationData
                {
                    Password = descryptedPass,
                    PasswordHash = userFound.PasswordHash,
                    PasswordSalt = userFound.PasswordSalt
                };

                if (VerificarPassApi(verificationData))
                {
                    Usuarios UserToSend = new Usuarios();
                    UserToSend.Id = userFound.Id;
                    UserToSend.Nombre = userFound.Nombre;
                    UserToSend.Rut = userFound.Rut;
                    UserToSend.Email = userFound.Email;
                    UserToSend.RolId = userFound.RolId;



                    return Ok(UserToSend.ToJson());
                }
            }

            return Unauthorized("Invalid credentials");
        }
        // POST api/<AuthApi>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AuthApi>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AuthApi>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        public bool VerificarPassApi(PasswordVerificationData data)
        {
            using (var hmac = new HMACSHA512(data.PasswordSalt))
            {
                var pass = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data.Password));
                return pass.SequenceEqual(data.PasswordHash);
            }
        }


        public static string DecryptText(string encryptedText)
        {
            string secretKey = "E93{P254sNRJy2XG";
            string iv = "E93{P254sNRJy2XG";

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(secretKey);
                aes.IV = Encoding.UTF8.GetBytes(iv);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(encryptedBytes))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }



    }
}
