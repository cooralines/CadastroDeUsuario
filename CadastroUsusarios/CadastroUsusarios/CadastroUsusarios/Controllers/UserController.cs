using CadastroUsusarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace CadastroUsusarios.Models
{
    //CREATE TABLE tbPedido (
    //    Id INT NOT NULL IDENTITY PRIMARY KEY,
    //    Nome VARCHAR(50) NOT NULL,
    //    Valor FLOAT NOT NULL
    //)

    public class PedidoController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            List<Models.User> lista = new List<Models.User>();

            // Cria e abre a conexão com o banco de dados (essa string só serve para acessar o banco localmente)
            // Veja mais strings de conexão em http://www.connectionstrings.com/
            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;Initial Catalog=carolaine;Persist Security Info=False;User ID=cooralines;Password=carol2015#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                conn.Open();

                // Cria um comando para selecionar registros da tabela, trazendo todas as pessoas que nasceram depois de 1/1/1900
                using (SqlCommand cmd = new SqlCommand("SELECT Id, Nome, Email FROM tbUser ORDER BY Nome ASC", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //Obtém os registros, um por vez
                        while (reader.Read() == true)
                        {
                            int id = reader.GetInt32(0);
                            string nome = reader.GetString(1);
                            string email = reader.GetString(2);

                            Models.User user = new Models.User();
                            user.Id = id;
                            user.Nome = nome;
                            user.Email = email;

                            lista.Add(user);
                        }
                    }
                }
            }

            return View(lista);
        }

        public ActionResult Criar(Models.User User)
        {
            // Validar!!!!
            if (string.IsNullOrWhiteSpace(User.Nome))
            {
                return Json("Nome inválido!");
            }

            // Cria e abre a conexão com o banco de dados (essa string só serve para acessar o banco localmente)
            // Veja mais strings de conexão em http://www.connectionstrings.com/
            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;Initial Catalog=carolaine;Persist Security Info=False;User ID=cooralines;Password=carol2015#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                conn.Open();

                // Cria um comando para inserir um novo registro à tabela
                using (SqlCommand cmd = new SqlCommand("INSERT INTO tbUser (Nome, Email) OUTPUT INSERTED.Id VALUES (@nome, @Email)", conn))
                {
                    // Esses valores poderiam vir de qualquer outro lugar, como uma TextBox...
                    cmd.Parameters.AddWithValue("@nome", User.Nome);
                    cmd.Parameters.AddWithValue("@Email", User.Email);

                    int id = (int)cmd.ExecuteScalar();
                }
            }

            return Json("ok");
        }

        public ActionResult ModalEditar(int id)
        {
            Models.User user;

            // Cria e abre a conexão com o banco de dados (essa string só serve para acessar o banco localmente)
            // Veja mais strings de conexão em http://www.connectionstrings.com/
            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;Initial Catalog=carolaine;Persist Security Info=False;User ID=cooralines;Password=carol2015#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                conn.Open();

                // Cria um comando para selecionar registros da tabela, trazendo todas as pessoas que nasceram depois de 1/1/1900
                using (SqlCommand cmd = new SqlCommand("SELECT Id, Nome, Email FROM tbUser WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //Obtém os registros, um por vez
                        if (reader.Read() == true)
                        {
                            string nome = reader.GetString(1);
                            string email = reader.GetString(2);

                            user = new Models.User();
                            user.Id = id;
                            user.Nome = nome;
                            user.Email = email;
                        }
                        else
                        {
                            user = null;
                        }
                    }
                }
            }

            return PartialView("_Editar", user);
        }

        public ActionResult Editar(Models.User user)
        {
            // Validar!!!!
            if (string.IsNullOrWhiteSpace(user.Nome))
            {
                return Json("Nome inválido!");
            }

            // Cria e abre a conexão com o banco de dados (essa string só serve para acessar o banco localmente)
            // Veja mais strings de conexão em http://www.connectionstrings.com/
            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;Initial Catalog=carolaine;Persist Security Info=False;User ID=cooralines;Password=carol2015#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                conn.Open();

                // Cria um comando para inserir um novo registro à tabela
                using (SqlCommand cmd = new SqlCommand("UPDATE tbUser SET Nome = @nome, Email = @email WHERE Id = @id", conn))
                {
                    // Esses valores poderiam vir de qualquer outro lugar, como uma TextBox...
                    cmd.Parameters.AddWithValue("@nome", user.Nome);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@id", user.Id);

                    cmd.ExecuteNonQuery();
                }
            }

            return Json("ok");
        }
    }
}
