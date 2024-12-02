using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

using ScannerCC.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;


namespace ScannerCC.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string user, string password)
        {
            // Busca el usuario si existe en la base de datos e incluye el rol de este.
            var us = _context.Usuario.Include(r => r.Rol).Where(u => u.Email.Equals(user) || u.Rut.Equals(user)).FirstOrDefault();
            if (us != null)
            {
             // Si el usuario existe pasa a verificar la contraseña
                if (VerificarPass(password, us.PasswordHash, us.PasswordSalt))
                {

                    // si la contraseña existe, verifica el rol del usuario. 
                    if (us.Rol.Nombre == "Especialista") {
                            var Claims = new List<Claim>
                            {
                            new Claim(ClaimTypes.Name, us.Rut),
                            new Claim(ClaimTypes.NameIdentifier, us.Rut),
                            new Claim(ClaimTypes.Role, "Especialista") //se define el rol del usuario
                            };
                            var identity = new ClaimsIdentity(Claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity);
                            //Guarda la session en las cookies 
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                                new AuthenticationProperties { IsPersistent = true }
                                );

                            return RedirectToAction("Index", "Especialista");
                    }
                    else
                    {
                            var Claims = new List<Claim>
                            {
                            new Claim(ClaimTypes.Name, us.Rut),
                            new Claim(ClaimTypes.NameIdentifier, us.Rut),
                            new Claim(ClaimTypes.Role, "Control de Calidad") //se define el rol del usuario
                            }; 
                            var identity = new ClaimsIdentity(Claims,
                                CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity); 
                            //Guarda la session en las cookies 
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                                new AuthenticationProperties { IsPersistent = true }
                                );

                            return RedirectToAction("Index", "Controlcalidad");
                    }
                }
                else
                {
                    // Usuario correcto pero contraseña mala.
                    TempData["ErrorMessage"] = "Contraseña incorrecta. Escriba la contraseña correcta e inténtelo de nuevo.";
                    return RedirectToAction("Index2", "Home");
                }
            }
            else
            {
                // Usuario No Existe.
                TempData["ErrorMessage"] = "Usuario no encontrado. Escriba las credenciales correctas e inténtelo de nuevo.";
                return RedirectToAction("Index2", "Home");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            // Elimina la session de las cookies.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    
        // Se crea el usuario base, el administrador inicial.    
        public IActionResult CreateAdminUser() 
        {
            Usuarios U = new Usuarios();
            U.Rut = "1.111.111-1"; //CREDENCIAL DE ACCESO RUT: 1.111.111-1
            U.Email = "espe";
            U.Nombre = "Espe";
            U.Token = Guid.NewGuid().ToString();
            CreatePasswordHash("espe", out byte[] passwordHash, out byte[] passwordSalt); //CREDENCIAL DE ACCESO, PASSWORD: espe

            U.PasswordHash = passwordHash;
            U.PasswordSalt = passwordSalt;
            U.RolId = _context.Rol.Where(x => x.Nombre == "Especialista").FirstOrDefault().idRol;
            U.Activo = true;
            _context.Usuario.Add(U);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Create(string Email, string Rut, string Nombre, string Password, int Rol)
        {
            var us = _context.Usuario.Where(u => u.Rut.Equals(Rut)).FirstOrDefault();
            if (us != null)
            {
                //el usuario ya esta registrado con el Rut ingresado
                ModelState.AddModelError("", "RUT ya rgistrado!");
                return RedirectToAction("GestionUsuarios", "Usuario");
                
            }
            else
            {

                Usuarios U = new Usuarios();
                U.Nombre = Nombre;
                U.Email = Email;
                U.Rut = Rut;
                U.RolId = Rol;
                U.Activo=true;
                U.Token = Guid.NewGuid().ToString(); // Genera un identificador único global para el Token.

                CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);

                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;
                _context.Usuario.Add(U);
                _context.SaveChanges();
                return RedirectToAction("GestionUsuarios", "Usuario");
            }
        }

        [HttpPost]
        public IActionResult Edit(int Id, string Email, string Rut, string Nombre, string Password, int Rol)
        {

            var usuario = _context.Usuario.FirstOrDefault(u => u.Id == Id);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return RedirectToAction("GestionUsuarios", "Usuario");
            }

            // Verificar si el RolId existe en la tabla Rol
            var rolExiste = _context.Rol.Any(r => r.idRol == Rol);
            if (!rolExiste)
            {
                ModelState.AddModelError("", "Rol no válido.");
                return RedirectToAction("GestionUsuarios", "Usuario");
            }

            usuario.Nombre = Nombre;
            usuario.Email = Email;
            usuario.Rut = Rut;
            usuario.RolId = Rol;

            if (!string.IsNullOrEmpty(Password))
            {
                CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);
                usuario.PasswordHash = passwordHash;
                usuario.PasswordSalt = passwordSalt;
            }

            _context.Usuario.Update(usuario);
            _context.SaveChanges();

            return RedirectToAction("GestionUsuarios", "Usuario");
        }

        [HttpPost]
        public IActionResult EditC(int Id, string Password)
        {

            // Buscar el usuario por ID
            var usuario = _context.Usuario
                          .Include(u => u.Rol) 
                          .FirstOrDefault(u => u.Id == Id);


            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return RedirectToAction("Index", "Home");
            }

            // Si se proporciona una nueva contraseña, se procede a actualizarla
            if (!string.IsNullOrEmpty(Password))
            {
                CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);
                usuario.PasswordHash = passwordHash;
                usuario.PasswordSalt = passwordSalt;

                // Guardar cambios en la base de datos
                _context.Usuario.Update(usuario);
                _context.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "La contraseña no puede estar vacía.");
                if (usuario.Id == Id)
                {
                    if (User.IsInRole("Especialista"))
                    {
                        return RedirectToAction("Index", "Especialista");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Controlcalidad");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            if (usuario.Id == Id) 
            {
                if (User.IsInRole("Especialista"))
                {
                    return RedirectToAction("Index", "Especialista");
                }
                else
                {
                    return RedirectToAction("Index", "Controlcalidad");
                }
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }

        }


        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerificarPass(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var pass = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return pass.SequenceEqual(passwordHash);
            }
        }
    }
}
