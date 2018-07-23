using Azure;
using CadastroUsusarios.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CadastroUsusarios.Models {
    public class UserController : Controller {
        
        public ActionResult Index() {
            List<Models.User> lista = new List<Models.User>();

            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;" +
                "Initial Catalog=carolaine;Persist Security Info=False;User ID=xxxx;Password=xxxx;" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")) {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Id, Nome, Email FROM tbUsuario ORDER BY Nome ASC", conn)) {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                      
                        while (reader.Read() == true) {
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

        public ActionResult Novo() {
            return PartialView("_Novo");
        }

        public ActionResult Criar(User user) {
            if (string.IsNullOrWhiteSpace(user.Nome)) {
                return Json("Nome inválido");
            }
            
            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;" +
                "Initial Catalog=carolaine;Persist Security Info=False;User ID=xxxx;Password=xxxx;" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")) {
                conn.Open();
                
                using (SqlCommand cmd = new SqlCommand("INSERT INTO tbUsuario (Nome, Email) OUTPUT INSERTED.Id VALUES (@nome, @email)", conn)) {
                  
                    cmd.Parameters.AddWithValue("@nome", user.Nome);
                    cmd.Parameters.AddWithValue("@email", user.Email);

                    int id = (int)cmd.ExecuteScalar();
                    UploadFoto(user, id);
                }
            }
            return Json("SUCESSO");
        }

        public ActionResult ModalEditar(int id) {
            Models.User user;

            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;" +
                "Initial Catalog=carolaine;Persist Security Info=False;User ID=xxxx;Password=xxxx;" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")) {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Id, Nome, Email FROM tbUsuario WHERE Id = @id", conn)) {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        
                        if (reader.Read() == true) {
                            string nome = reader.GetString(1);
                            string email = reader.GetString(2);

                            user = new Models.User();
                            user.Id = id;
                            user.Nome = nome;
                            user.Email = email;
                        } else {
                            user = null;
                        }
                    }
                }
            }

            return PartialView("_Editar", user);
        }

        public ActionResult Editar(User user, int id) {
            
            if (string.IsNullOrWhiteSpace(user.Nome)) {
                return Json("Nome inválido!");
            }

            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;" +
                "Initial Catalog=carolaine;Persist Security Info=False;User ID=xxxx;Password=xxxx;" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")) {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("UPDATE tbUsuario SET Nome = @nome, Email = @email WHERE Id = @id", conn)) {
                    
                    cmd.Parameters.AddWithValue("@nome", user.Nome);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@id", user.Id);

                    cmd.ExecuteNonQuery();
                    UploadFoto(user, id);
                }
            }

            return Json("ok");
        }

        public ActionResult ModalExcluir(int id) {
            User user;

            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;" +
                "Initial Catalog=carolaine;Persist Security Info=False;User ID=xxxx;Password=xxxx;" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")) {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Id, Nome, Email FROM tbUsuario WHERE Id = @id", conn)) {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        
                        if (reader.Read() == true) {
                            string nome = reader.GetString(1);
                            string email = reader.GetString(2);

                            user = new Models.User();
                            user.Id = id;
                            user.Nome = nome;
                            user.Email = email;
                        } else {
                            user = null;
                        }
                    }
                }
            }

            return PartialView("_Excluir", user);
        }

        public void Excluir(int Id) {
            using (SqlConnection conn = new SqlConnection("Server=tcp:carolaine.database.windows.net,1433;" +
                "Initial Catalog=carolaine;Persist Security Info=False;User ID=xxxx;Password=xxxx;" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")) {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM tbUsuario WHERE Id = @id", conn)) {
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                    return;
                }
            }
        }

        public ActionResult UploadFoto(User pessoa, int id) {

            if (Request.Files.Count == 0) {
                return Json("Nenhum arquivo selecionado!", JsonRequestBehavior.AllowGet);
            }

            if (Request.Files[0].ContentType != "image/jpeg") {
                return Json("Apenas imagens JPEG!", JsonRequestBehavior.AllowGet);
            }

            try {
                using (Image bmp = Image.FromStream(Request.Files[0].InputStream)) {
                    if (bmp.Width > 2000 || bmp.Height > 2000) {
                        return Json("A imagem deve ter dimensões menores do que 2000x2000!", JsonRequestBehavior.AllowGet);
                    }
                }

                Request.Files[0].InputStream.Position = 0;
            } catch {
                return Json("Imagem JPEG inválida!", JsonRequestBehavior.AllowGet);
            }

            AzureStorage.Upload(
                "fotos", 
                id + ".jpg", 
                Request.Files[0].InputStream,
                "cloud", 
                "UseDevelopmentStorage=true"); 

            return Json("Imagem registrada com sucesso", JsonRequestBehavior.AllowGet);
        }
    }
}
