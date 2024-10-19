using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using QualityScout.Models;
using ScannerCC.Models;
using System.Net;
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

            var userFound = _context.Usuario.Include(r => r.Rol)
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
                    UsuarioToSend UserToSend = new UsuarioToSend();
                    UserToSend.Id = userFound.Id;
                    UserToSend.Nombre = userFound.Nombre;
                    UserToSend.Rut = userFound.Rut;
                    UserToSend.Email = userFound.Email;
                    UserToSend.NombreRol = userFound.Rol.Nombre;
                    UserToSend.Token = userFound.Token;
                    UserToSend.Activo = userFound.Activo;



                    return Ok(UserToSend.ToJson());
                }
            }

            return Unauthorized("Invalid credentials");
        }



        [HttpPost("CreateUsuario")]
        public async Task<ActionResult<Usuarios>> CreateUsuario([FromHeader(Name = "Authorization")] string authorization, UserToReceive userToReceive)
        {
            // Extrae el token de la cabecera Authorization
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized("Token no proporcionado o no válido.");
            }

            var token = authorization.Substring("Bearer ".Length).Trim();

            var usuarioPeticion = await _context.Usuario.FirstOrDefaultAsync(u => u.Token == token);
            if (usuarioPeticion == null)
            {
                return Unauthorized("Token no válido.");
            }

            var usuarioExiste = _context.Usuario.Where(u => u.Rut.Equals(userToReceive.Rut)).FirstOrDefault();
            if (usuarioExiste != null)
            {
                //el usuario ya esta registrado con el Rut ingresado
                return BadRequest(new { message = "El usuario ya está registrado" });
            }

            CreatePasswordHash(userToReceive.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Crear el usuario
            var usuario = new Usuarios
            {
                Nombre = userToReceive.Nombre,
                Rut = userToReceive.Rut,
                Email = userToReceive.Email,
                RolId = userToReceive.RolId,
                Activo = true, 
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Token = Guid.NewGuid().ToString()
            };

            _context.Add(usuario);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut("UpdatePassword/{Rut}")]
        public async Task<IActionResult> UpdatePassword([FromHeader(Name = "Authorization")] string authorization, string Rut, [FromBody] string Password)
        {

            // Extrae el token de la cabecera Authorization
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized("Token no proporcionado o no válido.");
            }

            var token = authorization.Substring("Bearer ".Length).Trim();

            var usuarioPeticion = await _context.Usuario.FirstOrDefaultAsync(u => u.Token == token);
            if (usuarioPeticion == null)
            {
                return Unauthorized("Token no válido.");
            }

            if (_context.Usuario == null)
            {
                return Problem("Entity set 'AppDbContext.Usuario' is null.");
            }

            var usuario = await _context.Usuario.Where(x => x.Rut == Rut).FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);

            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            try
            {
                _context.Usuario.Update(usuario);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Contraseña de usuario actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el estado del usuario: {ex.Message}");
            }
        }




        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
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
