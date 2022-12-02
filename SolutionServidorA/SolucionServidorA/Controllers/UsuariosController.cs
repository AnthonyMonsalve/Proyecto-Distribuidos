using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SolucionServidorA.Data;
using SolucionServidorA.Entities;

namespace SolucionServidorA.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioContext _context;

        public UsuariosController(UsuarioContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<List<Usuario>> Index()
        {
            return await _context.Usuarios.ToListAsync();
        }


        [HttpPost]
        [Route("CrearUsuario/")]
        public async Task<Usuario> CrearUsuario([FromBody] string nombre)
        {

            if (ModelState.IsValid)
            {
                var usuario = new Usuario();
                usuario.Id = Guid.NewGuid();
                usuario.nombre = nombre;
                var random = new Random();
                while (true == true)
                {

                    usuario.clave = random.Next(10000000, 99999999);
                    if (_context.Usuarios.Where(x => x.clave == usuario.clave).ToList().Count == 0)
                    {
                        break;
                    }
                }
                
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return usuario;
            }
            return null;
        }

        public class EntradaFirma
        {
            public string nombreUsuario { get; set; }
            public string mensaje { get; set; }
            public int clave { get; set; }
        }


        [HttpPost]
        [Route("Firmar/")]
        public async Task<Firma> Firmar([FromBody] EntradaFirma entrada)
        {
            //verificar usuario
            var user = _context.Usuarios.Where(x => x.nombre == entrada.nombreUsuario && x.clave == entrada.clave);
            if (user == null)
            {
                return null;
            }

            //Hashear Mensaje
            var bytemensaje = Encoding.UTF32.GetBytes(entrada.mensaje);
            var mySHA256 = SHA256.Create();
            byte[] hashValue = mySHA256.ComputeHash(bytemensaje);
            string hashstring = Convert.ToHexString(hashValue);

            //Cifrar Hash
            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF32.GetBytes(entrada.clave.ToString());
            byte[] IV = Encoding.UTF8.GetBytes("ClaveSecreta1234");
            aes.IV = IV;
            var hashEncriptado = EncryptStringToBytes_Aes(hashstring, aes.Key, aes.IV);
            var hashencriptadoString = Convert.ToHexString(hashEncriptado);

            var firma = new Firma()
            {
                Id = Guid.NewGuid(),
                firma = hashencriptadoString,
                clave = entrada.clave
            };
            return firma;
        }

        public class EntradaIntegridad
        {
            public string mensaje { get; set; }
            public int clave { get; set; }
            public string hashEncriptado { get; set; }
        }

        [HttpPost]
        [Route("Integridad/")]
        public async Task<string> verificarIntegridad([FromBody] EntradaIntegridad entrada)
        {
            //Desencriptando Mensaje llegada
            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF32.GetBytes(entrada.clave.ToString());
            byte[] IV = Encoding.UTF8.GetBytes("ClaveSecreta1234");
            aes.IV = IV;
            byte[] hashDesencriptado = Convert.FromHexString(entrada.hashEncriptado);
            string hashEntrante = DecryptStringFromBytes_Aes(hashDesencriptado, aes.Key, aes.IV);

            //Hasheando Mensaje
            var bytemensaje = Encoding.UTF32.GetBytes(entrada.mensaje);
            var mySHA256 = SHA256.Create();
            byte[] hashCalculado = mySHA256.ComputeHash(bytemensaje);
            string hashCalculadostring = Convert.ToHexString(hashCalculado);

            if (hashCalculadostring.Equals(hashEntrante))
            {
                return "Integro";
            }
            return "No Integro";
        }

        public class EntradaAutenticar
        {
            public string nombreUsuario { get; set; }
            public int clave { get; set; }
        }

        [HttpPost]
        [Route("Autenticar/")]
        public async Task<string> Autenticar([FromBody] EntradaAutenticar entrada)
        {
            //verificar usuario
            var user = _context.Usuarios.Where(x => x.nombre == entrada.nombreUsuario && x.clave == entrada.clave).FirstOrDefault();
            if (user == null)
            {
                return "NO AUTORIZADO";
            }
            return "AUTORIZADO";
        }


        static byte[] EncryptStringToBytes_Aes(string plaintext, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plaintext == null || plaintext.Length <= 0)
                throw new ArgumentNullException("hash");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plaintext);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        /*
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;
            
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                //aesAlg.Padding = PaddingMode.PKCS7;
                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }*/

    }
}
