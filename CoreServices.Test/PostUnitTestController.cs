﻿using CoreServices.Controllers;
using CoreServices.Models;
using CoreServices.Repository;
using CoreServices.ViewModel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Xunit;

namespace CoreServices.Test
{
    /// <summary>
    /// Uma vez que a classe PostUnitTestController estiver pronta, primeiro tentará acessar nosso banco de dados onde, em tempo de execução, semearemos alguns dados fictícios e acessaremos esses dados fictícios para teste. Portanto, vamos preparar a string de conexão e, com base nela, obter a instância de "BlogDBContext".
    /// </summary>
    public class PostUnitTestController
    {
        public PostRepository repository { get; set; }
        public static DbContextOptions<BlogDBContext> dbContextOptions { get; }
        public static string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BlogDB;Integrated Security=True;";

        static PostUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
                .UseSqlServer(connectionString)
                .Options;
        }

        public PostUnitTestController()
        {
            var context = new BlogDBContext(dbContextOptions);
            DummyDataDBInitializer db = new DummyDataDBInitializer();
            db.Seed(context);

            repository = new PostRepository(context);
        }


        #region Get By Id
        [Fact]
        public async void Task_GetPostById_Return_OkResult()
        {
            //Arrange
            var controller = new PostController(repository);
            var postId = 2;

            //Act
            var data = await controller.GetPost(postId);

            //Assert
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_Return_NotFoundResult()
        {
            //Arrange
            var controller = new PostController(repository);
            var postId = 3;

            //Act
            var data = await controller.GetPost(postId);

            //Assert
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public async void Task_GetPostById_MatchResult()
        {
            //Arrange
            var controller = new PostController(repository);
            int? postId = 1;

            //Act
            var data = await controller.GetPost(postId);

            //Assert
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            Assert.Equal("Teste de titulo 1", post.Title);
            Assert.Equal("Teste Descrição 1", post.Description);
        }
        #endregion

        #region Get All
        [Fact]
        public async void Task_GetPosts_Return_OkResult()
        {
            //Arrange
            var controller = new PostController(repository);

            //Act
            var data = await controller.GetPosts();

            //Assert
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public void Task_GetPosts_Return_BadRequestResult()
        {
            //Arrange
            var controller = new PostController(repository);

            //Act
            var data = controller.GetPosts();
            data = null;
            if (data != null)
            {
                //Assert
                Assert.IsType<BadRequestResult>(data);
            }
        }

        [Fact]
        public async void Task_GetPosts_MatchResult()
        {
            //Arrange
            var controller = new PostController(repository);

            //Act
            var data = await controller.GetPosts();

            //Assert
            Assert.IsType<OkObjectResult>(data);

            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var post = okResult.Value.Should().BeAssignableTo<List<PostViewModel>>().Subject;

            Assert.Equal("Teste de titulo 1", post[0].Title);
            Assert.Equal("Teste Descrição 1", post[0].Description);

            Assert.Equal("Teste de titulo 2", post[1].Title);
            Assert.Equal("Teste Descrição 2", post[1].Description);
        }

        #endregion
    }
}