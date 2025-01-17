using Gadaxede.Data;
using Gadaxede.Interfaces;
using Gadaxede.Models;

namespace Gadaxede.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy(p => p.Id).ToList();
        }
        public void RegisterUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void EditUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public User GetUser(int id)
        {
            return _context.Users.Find(id);
        }
        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        public void AddSensor(User user, Sensor sensor)
        {
            user.Sensors.Add(sensor);
            _context.SaveChanges();
        }
    }
}
