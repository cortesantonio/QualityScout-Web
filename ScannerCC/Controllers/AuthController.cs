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
            //Trabajador
            var us = _context.Usuario.Include(r => r.Rol).Where(u => u.Email.Equals(user) || u.Rut.Equals(user)).FirstOrDefault();
            if (us != null)
            {
                //Usuario Encontrado
                if (VerificarPass(password, us.PasswordHash, us.PasswordSalt))
                {
                    if (us.Rol.Nombre == "Especialista") {
                            var Claims = new List<Claim>
                            {
                            new Claim(ClaimTypes.Name, us.Rut),
                            new Claim(ClaimTypes.NameIdentifier, us.Rut),
                            new Claim(ClaimTypes.Role, "Especialista")
                            };
                            var identity = new ClaimsIdentity(Claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity);

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
                            new Claim(ClaimTypes.Role, "Control de Calidad")
                            }; 
                            var identity = new ClaimsIdentity(Claims,
                                CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity);

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                                new AuthenticationProperties { IsPersistent = true }
                                );

                            return RedirectToAction("Index", "Controlcalidad");
                    }
                }
                else
                {
                    //Usuario correcto pero contraseña mala
                    ModelState.AddModelError("", "Contraseña Incorrecta");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                //Usuario No Existe
                ModelState.AddModelError("", "Usuario no Encontrado!");
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateAdminUser()
        {
            Usuarios U = new Usuarios();
            U.Rut = "espe";
            U.Email = "espe";
            U.Nombre = "espe";
            U.Token = Guid.NewGuid().ToString();
            CreatePasswordHash("espe", out byte[] passwordHash, out byte[] passwordSalt);

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
                return RedirectToAction("Index", "Home");
                
            }
            else
            {

                Usuarios U = new Usuarios();
                U.Nombre = Nombre;
                U.Email = Email;
                U.Rut = Rut;
                U.RolId = Rol;
                U.Activo=true;
                U.Token = Guid.NewGuid().ToString();

                CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);

                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;
                _context.Usuario.Add(U);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Edit(string Email, string Rut, string Nombre, string Password, int Rol)
        {
            var U = _context.Usuario.FirstOrDefault(u => u.RolId.Equals(Rol)); 

            if (U != null)
            {
                if (U.RolId == Rol)
                {
                    U.Nombre = Nombre;
                    U.Email = Email;
                    U.Rut = Rut;
                }
                else
                {
                    U.Nombre = Nombre;
                    U.Email = Email;
                    U.Rut = Rut;
                    U.RolId = Rol;
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);
                    U.PasswordHash = passwordHash;
                    U.PasswordSalt = passwordSalt;
                }

                _context.Usuario.Update(U);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
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
