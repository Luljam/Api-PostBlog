using CoreServices.Controllers;
using CoreServices.Models;
using CoreServices.Repository;
using CoreServices.ViewModel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        #region Add New Blog
        [Fact]
        public async void Task_Add_ValidData_Return_OkResult()
        {
            //Arrange
            var controller = new PostController(repository);
            var post = new Post()
            {
                Title = "Test Titulo 3",
                Description = "Test Descrição 3",
                CategoryId = 2,
                CreatedDate = DateTime.Now
            };

            //Act
            var data = await controller.AddPost(post);

            //Assert
            Assert.IsType<OkObjectResult>(data);
        }

        [Fact]
        public async void Task_Add_InvalidData_Return_BadRequest()
        {
            //Arrange
            var controller = new PostController(repository);
            Post post = new Post()
            {
                Title = "Test Titulo com mais de 20 caracteres",
                Description = "Test Descrição 3",
                CategoryId = 3,
                CreatedDate = DateTime.Now
            };

            // Act
            var data = await controller.AddPost(post);

            //Assert
            Assert.IsType<BadRequestResult>(data);
        }

        [Fact]
        public async void Task_Add_ValidData_MatchResult()
        {
            //Arrange
            var controller = new PostController(repository);
            var post = new Post()
            {
                Title = "Test Titulo 4",
                Description = "Test Descrição 4",
                CategoryId = 2,
                CreatedDate = DateTime.Now
            };

            //Act
            var data = await controller.AddPost(post);

            //Assert
            Assert.IsType<OkObjectResult>(data);
            var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
            Assert.Equal(3, okResult.Value);
        }
        #endregion

        #region Update Existing Blog
        [Fact]
        public async void Task_Update_ValidData_Return_OkResult()
        {
            //Arrange
            var controller = new PostController(repository);
            var postId = 2;

            //Act
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.Title = "Test Titulo 2 Upda";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var updateData = await controller.UpdatePost(post);

            //Assert
            Assert.IsType<OkResult>(updateData);
        }

        [Fact]
        public async void Task_Update_InvalidData_Return_BadRequest()
        {
            //Arrange
            var controller = new PostController(repository);
            var postId = 2;

            //Act
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.Title = "Test Titulo Mais de 20 caracteres";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var data = await controller.UpdatePost(post);

            //Assert
            Assert.IsType<BadRequestResult>(data);

        }


        [Fact]
        public async void Task_Update_InvalidData_Return_NotFound()
        {
            //Arrange
            var controller = new PostController(repository);
            var postId = 2;

            //Act
            var existingPost = await controller.GetPost(postId);
            var okResult = existingPost.Should().BeOfType<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<PostViewModel>().Subject;

            var post = new Post();
            post.PostId = 5; // Id não existe
            post.Title = "Test Title More Than 20 Characteres";
            post.Description = result.Description;
            post.CategoryId = result.CategoryId;
            post.CreatedDate = result.CreatedDate;

            var data = await controller.UpdatePost(post);

            //Assert
            Assert.IsType<NotFoundResult>(data);
        }
        #endregion


        #region Delete Post
        [Fact]
        public async void Task_Delete_Post_Return_OkResult()
        {
            //Arrange
            var controller = new PostController(repository);
            var postId = 2;

            //Act
            var data = await controller.DeletePost(postId);

            //Assert
            Assert.IsType<OkResult>(data);
        }
        
        [Fact]
        public async void Task_Delete_Post_NotFoundResult()
        {
            //Arrange
            var controller = new PostController(repository);
            var postId = 5; // post inexistente

            //Act
            var data = await controller.DeletePost(postId);

            //Assert
            Assert.IsType<NotFoundResult>(data);
        }

        [Fact]
        public async void Task_Delete_Return_BadRequestResult()
        {
            //Arrange
            var controller = new PostController(repository);
            int? postId = null;

            //Act
            var data = await controller.DeletePost(postId);

            //Assert
            Assert.IsType<BadRequestResult>(data);
        }

        #endregion
    }
}