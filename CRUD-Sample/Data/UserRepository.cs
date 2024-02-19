using CRUD_Sample.Models;

namespace CRUD_Sample.Data
{
    public class UserRepository
    {
        private readonly List<User> _users = new List<User>();

        public IEnumerable<User> GetUsers()
        {
            return _users;
        }

        public User? GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public void UpdateUser(int id, User updatedUser)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == id);
            if(existingUser != null)
            {
                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.Email = updatedUser.Email;
            }
        }

        public void DeleteUser(int id)
        {
            var userToRemove = _users.FirstOrDefault(u => u.Id == id);
            if(userToRemove != null)
            {
                _users.Remove(userToRemove);
            }
        }
    }
}
