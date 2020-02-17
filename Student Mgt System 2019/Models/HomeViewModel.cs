using System.ComponentModel.DataAnnotations;
using System.Web;
namespace Student_Mgt_System_2019.Models
{
    public class UpdateLabModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    public class LoginModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string username { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Password")]
        public string password { get; set; }
    }

    public class RegisterModel
    {

        [DataType(DataType.Text)]
        [Display(Name = "id")]
        public int id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string username { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Role")]
        public Role role { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Status")]
        public Status status { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "User Picture")]
        public string picture { get; set; }


        public HttpPostedFileBase user_image { get; set; }

        public enum Role
        {
            admin,
            user
        }

        public enum Status
        {
            current,
            past
        }
    }

    public class ProjectModel
    {

        [DataType(DataType.Text)]
        [Display(Name = "Project Id")]
        public int id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Project Title")]
        public string title { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Project Description")]
        public string text { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Project Picture")]
        public string picture { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Link")]
        public string link { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Student Id")]
        public string student_id_fk { get; set; }

        public HttpPostedFileBase project_image { get; set; }
    }

    public class ResearchModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Research Id")]
        public int id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Research title")]
        public string title { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "People Involved")]
        public string people_involved { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Year")]
        public string year { get; set; }
    }

    public class GroupModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Group Id")]
        public int id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Group title")]
        public string title { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Student Id ")]
        public int sstudent_id_fk { get; set; }
    }


    public class Profile
    {
    

        [DataType(DataType.Text)]
        [Display(Name = "Title")]
        public string title { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Collaborators")]
        public string collaborator { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Industrial Collaborators")]
        public string industrial { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        public string description { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Picture")]
        public string picture { get; set; }



        public HttpPostedFileBase profile_image { get; set; }

        

    }

}
