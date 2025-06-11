using Necli.Entidades.Entidades;
using Necli.Persistencia.DbContext;
using Necli.Persistencia.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necli.Persistencia.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly NecliDbContext _context;

        public UsuarioRepository(NecliDbContext context)
        {
            _context = context;
        }

        public Usuario InsertarUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return usuario;
        }

        public Usuario ObtenerPorCedula(string cedula)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Cedula == cedula);
        }

        public Usuario ObtenerPorEmail(string email)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Email == email);
        }

        public bool EmailExistente(string email)
        {
            return _context.Usuarios.Any(u => u.Email == email);
        }

        public bool EliminarUsuario(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            return _context.SaveChanges() > 0;
        }

        public Usuario ObtenerMedianteEmail(string email)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Email == email);
        }

        public bool CedulaExistente(string cedula)
        {
            return _context.Usuarios.Any(u => u.Cedula == cedula);
        }

        public Usuario ObtenerPorTokenReset(string token)
        {
            return _context.Usuarios.FirstOrDefault(u => u.TokenResetPassword == token);
        }

        public void Actualizar(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            _context.SaveChanges();
        }

    }
}
