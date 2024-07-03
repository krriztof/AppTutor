using AppTutor.Controllers;
using AppTutor.Data;
using AppTutor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class StudentControllerTests
    {
        private ApplicationDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(new ApplicationUser
            {
                Id = "testUserId",
                FirstName = "Jan",
                LastName = "Kowalski",
                PhoneNumber = "999999999",
                Role = "Student"

            });

            return userManager;
        }



        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfTeachers()
        {

            var dbContext = CreateDbContext("TestDb_Index");
            var userManager = MockUserManager().Object;
            var controller = new StudentController(dbContext, userManager);

            var result = await controller.Index(null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TeacherViewModel>>(viewResult.ViewData.Model);
        }

    }
}
