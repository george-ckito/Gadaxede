using Gadaxede.Models;

namespace Gadaxede.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        void RegisterUser(User user);
        void EditUser(User user);
        User GetUser(int id);
        void DeleteUser(int id);
        void AddSensor(User user, Sensor sensor);
    }
}
