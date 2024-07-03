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
    public class TeacherControllerTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            var testTeacher = new Teacher
            {
                UserId = "testUserId",
                TutorSessions = new List<TutorSession>(),
                DocumentPath = "path/to/document",
                Specialization = "Matematyka"
            };
            _dbContext.Teachers.Add(testTeacher);
            _dbContext.SaveChanges();

            _userManager = MockUserManager<ApplicationUser>().Object;
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);

            userManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("testUserId");

            return userManager;
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithMeetingRequests()
        {

            var controller = new TeacherController(_dbContext, _userManager);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<TutorSessionViewModel>>(viewResult.Model);

        }

    }
}
