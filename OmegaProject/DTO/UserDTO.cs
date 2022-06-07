using System.Collections.Generic;

namespace OmegaProject.DTO
{
    public class UserDTO
    {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone { get; set; }
            public int Role { get; set; }
            public string IdCard { get; set; }
            public  List<MessageDTO> Messages { get; set; }
    }

}
