using AppTutor.Data;
using AppTutor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;

namespace Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithExpectedModel()
        {
            var dbContext = CreateDbContext("TestDb");

            var mockUserManager = MockUserManager<ApplicationUser>();

            var controller = new AdminController(mockUserManager.Object, dbContext);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AdminViewModel>(viewResult.Model);

            dbContext.Dispose();
        }

        private ApplicationDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new ApplicationDbContext(options);
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            return mgr;
        }

        [Fact]
        public async Task ApproveTeacher_SetsIsApprovedToTrueAndSavesChanges()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var teacherId = "testId";
            var mockUserManager = MockUserManager<ApplicationUser>();

            using var context = new ApplicationDbContext(options);
            var teacher = new Teacher
            {
                Id = teacherId,
                IsApproved = false,
                DocumentPath = "testPath",
                Specialization = "Math",
                UserId = "testUserId" 
            };
            context.Teachers.Add(teacher);
            await context.SaveChangesAsync();

            var controller = new AdminController(mockUserManager.Object, context);

            await controller.ApproveTeacher(teacherId);

            var updatedTeacher = await context.Teachers.FindAsync(teacherId);
            Assert.True(updatedTeacher.IsApproved);
        }

    }




}