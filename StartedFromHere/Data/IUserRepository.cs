using System.Runtime.CompilerServices;
using DotnetAPI.Models;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public IEnumerable<T> GetEntitiesList<T>() where T: class;
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToAdd);
        public IEnumerable<User> GetUsers();
        public User GetSingleUser(int userId);
        public UserSalary GetSingleUserSalary(int userId);
        public UserJobInfo GetSingleUserJobInfo(int userId);
    }
}