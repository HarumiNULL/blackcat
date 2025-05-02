using Microsoft.EntityFrameworkCore;

namespace blackcat.Models;

public class DbInitializer
{
    public static void Seed(BlackcatDbContext context)
    {
        // Aplica migraciones pendientes
        context.Database.Migrate();

        // Evita repetir datos si ya existen
        if (!context.Libros.Any())
        {
            context.Libros.AddRange(
                new Libro()
                {
                    NombreL = "Alas de sangre",Autor = "Rebecca Yarros", Archivo = "/staticFiles/libros/Alas_de_sangre_Rebecca_Yarros.epub", Descripcion = "una emocionante fantasía romántica donde Violet Sorrengail, una joven destinada a la tranquila vida académica, es obligada a entrenar como jinete de dragón en una brutal academia militar. Entre peligros mortales, alianzas inciertas y una atracción prohibida, " +
                        "Violet deberá luchar por sobrevivir en un mundo donde solo los más fuertes sobreviven... y los dragones no eligen a cualquiera.",Imagen ="/staticFiles/images/alas_de_sangre.jpg" 
                }
                
            );    
        }

        if (!context.Rols.Any())
        {
            context.Rols.AddRange(
                new Rol()
                { Nombre = "Administrador", },
                new Rol()
                    { Nombre = "Moderador", },
                new Rol()
                    { Nombre = "Publico", }
            ); 
        }
        
        if (!context.EstadoUsulibros.Any())
        {
            context.EstadoUsulibros.AddRange(
                new EstadoUsulibro()
                    { Estado = "Activo", },
                new EstadoUsulibro()
                    { Estado = "Inactivo", },
                new EstadoUsulibro()
                    { Estado = "Bloqueado",}
            ); 
        }
        
        if (!context.Usuarios.Any())
        {
            context.Usuarios.AddRange(
                new Usuario()
                    { IdRol = 1 , IdEstado = 1, NombreU = "shirly", CorreoU = "shirleyp0848@gmail.com",Cont =  BCrypt.Net.BCrypt.HashPassword("1234"),}
            ); 
        }

        context.SaveChanges();
    }
}