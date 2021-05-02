using CoreServices.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreServices.Test
{
    /// <summary>
    /// Essa classe vamos criar como "DummyDataDBInitializer" com um método "Seed", 
    /// que primeiro excluirá todas as tabelas do banco de dados todas as vezes
    /// e as regenerará com base nas configurações do seu modelo e adicionará alguns dados fictícios. 
    /// Vamos obter ajuda com os seguintes trechos de código. Aqui você pode ver que estamos adicionando alguns dados para as tabelas "Category" e "Post" e,
    /// em seguida, submetendo-os usando o método "context.SaveChanges ()".
    /// </summary>
    public class DummyDataDBInitializer
    {
        public DummyDataDBInitializer()
        {

        }

        public void Seed(BlogDBContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Category.AddRange(
                new Category() { Name = "CSHARP", Slug = "csharp" },
                new Category() { Name = "VISUAL STUDIO", Slug = "visualstudio" },
                new Category() { Name = "ASP.NET CORE", Slug = "aspnetcore" },
                new Category() { Name = "SQL SERVER", Slug = "sqlserver" }
                );
            context.Post.AddRange(
                new Post() { Title = "Teste de titulo 1", Description = "Teste Descrição 1", CategoryId=2, CreatedDate = DateTime.Now},
                new Post() { Title = "Teste de título 2", Description = "Teste Descrição 2", CategoryId=3, CreatedDate = DateTime.Now}
                );
            context.SaveChanges();
        }
    }
}
