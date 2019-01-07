using System;
using Microsoft.EntityFrameworkCore;

namespace Samples.UsersApi.Database
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
