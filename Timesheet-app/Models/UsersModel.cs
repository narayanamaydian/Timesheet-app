using MongoDB.Bson;

namespace Timesheet_app.Models
{
    public class UsersModel
    {
        public ObjectId Id { get; set; }    
        public string Name {  get; set; }
        public string VendorName { get; set; }
        public string NoSpk { get; set; }

    }
}
