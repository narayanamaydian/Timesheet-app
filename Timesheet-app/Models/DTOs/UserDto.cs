
namespace Timesheet_app.Models.DTOs
{
    public class UserDto
    {
        public required string Id { get; set; }    
        public required string Name { get; set; }
        public required string VendorName { get; set; }
        public required string NoSpk { get; set; }

        public static implicit operator UserDto(UserModel v)
        {
            throw new NotImplementedException();
        }
    }
}