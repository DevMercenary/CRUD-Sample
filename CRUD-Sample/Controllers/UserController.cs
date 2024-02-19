using CRUD_Sample.Data;
using CRUD_Sample.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Sample.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(UserRepository userRepository) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(userRepository.GetUsers());
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            var user = userRepository.GetUserById(id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userRepository.AddUser(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, User updatedUser)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userRepository.UpdateUser(id, updatedUser);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            userRepository.DeleteUser(id);
            return NoContent();
        }
    }
}
