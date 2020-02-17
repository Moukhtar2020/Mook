using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;

using Student_Mgt_System_2019.Models;
 
namespace Student_Mgt_System_2019.Controllers
{
    public class HomeController : Controller
    {

        //connection string and connection variables (change connection string in web.config file)

        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlDataReader reader = null;
        SqlConnection conn = null;


        
        //Controllers that View,Edit and delete
        //[HttpPost] receives data after form subittion and adds to database
        //Action result return either redirect or VIEW or particular page
        //Async means it will me threaded...
        //ViewBag sends data from c# back end file to front end view
        //image.saveAs functions saves/uploades image and map function gets its directory where the image is on pc and where to upload


        public ActionResult Index()
        {

            DataTable lab = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT *FROM lab", sqlConn);
                adapter.Fill(lab);
            }

            return View(lab);
        }


        public ActionResult UpdateLab()
        {

            return View();

        }


        [HttpPost]
        public async Task<ActionResult> UpdateLab(UpdateLabModel model, string returnUrl)
        {


            if (ModelState.IsValid)
            {


                using (SqlConnection sqlConn = new SqlConnection(connString))
                {


                    try
                    {
                        conn = new SqlConnection(connString);
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("UPDATE lab set lab_text=@text", conn);

                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@text";
                        param.Value = model.Description;


                        cmd.Parameters.Add(param);


                        reader = cmd.ExecuteReader();



                    }
                    catch (SqlException e)
                    { }
                    finally
                    {

                        if (reader != null)
                        {
                            reader.Close();
                        }

                        if (conn != null)
                        {
                            conn.Close();
                        }
                    }



                }


            }
            return RedirectToAction("Index", "Home");

        }


        public ActionResult Register()
        {

            if (!String.IsNullOrEmpty(Request.QueryString["msg"]))
            {
               
                ViewBag.msg = (Request.QueryString["msg"]);
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model, string returnUrl)
        {

           


                using (SqlConnection sqlConn = new SqlConnection(connString))
                {


                    try
                    {
                        conn = new SqlConnection(connString);
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("INSERT INTO registration(reg_name,reg_username,reg_password,reg_role,reg_status) values(@name,@username,@password,@role,@status)", conn);
                        cmd.Parameters.AddWithValue("@name", model.name);
                        cmd.Parameters.AddWithValue("@username", model.username);
                        cmd.Parameters.AddWithValue("@password", model.password);
                        cmd.Parameters.AddWithValue("@role", model.role);
                        cmd.Parameters.AddWithValue("@status", model.status); 

                        reader = cmd.ExecuteReader();



                    }
                    catch (SqlException e)
                    { }
                    finally
                    {

                        if (reader != null)
                        {
                            reader.Close();
                        }

                        if (conn != null)
                        {
                            conn.Close();
                        }
                    }



                }
            return RedirectToAction("Register", "Home");
        }


        public ActionResult login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult login(LoginModel model,String returnUrl)
        {
            if (ModelState.IsValid)
            {


                using (SqlConnection sqlConn = new SqlConnection(connString))
                {


                    try
                    {
                        conn = new SqlConnection(connString);
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SELECT *FROM registration WHERE reg_username=@username AND reg_password=@pass", conn);

                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@username";
                        param.Value = model.username;

                        SqlParameter param2 = new SqlParameter();
                        param2.ParameterName = "@pass";
                        param2.Value = model.password;


                        cmd.Parameters.Add(param);
                        cmd.Parameters.Add(param2);

                        reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            Session["user_type"] = reader["reg_role"];
                            Session["user_id"] = reader["reg_id"];

                            if ((string)reader["reg_role"] == "1") //1 user
                            {
                                return RedirectToAction("ProjectArea", "Home");
                            }
                            else if ((string)reader["reg_role"].ToString() == "0") //0 admin
                            {
                                return RedirectToAction("Index", "Home");
                            }

                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid username or password.");
                        }

                    }
                    catch (SqlException e)
                    { }
                    finally
                    {

                        if (reader != null)
                        {
                            reader.Close();
                        }

                        if (conn != null)
                        {
                            conn.Close();
                        }
                    }



                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult logout()
        {
            Session["user_type"] = null;
            Session["user_id"] = null;

            return RedirectToAction("index", "Home");
        }

        public ActionResult ProjectArea()
        {

            DataTable project = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT *FROM registration inner join project on student_id_fk=reg_id", sqlConn);
                adapter.Fill(project);



            }

            return View(project);
        }


        public ActionResult EditStudentProject(int id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult EditStudentProject(ProjectModel model,string returnUrl)
        {

            String file_name = Path.GetFileNameWithoutExtension(model.project_image.FileName);
            String extension = Path.GetExtension(model.project_image.FileName);

            file_name += DateTime.Now.ToString("yymmssff")+extension;

            model.picture = file_name;

            file_name = Path.Combine(Server.MapPath("~/Assets/"), file_name);

            model.project_image.SaveAs(file_name);

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Update project set project_title=@title,project_text=@text,project_picture=@picture WHERE project_id=@id ", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@title";
                    param.Value = model.title;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@text";
                    param2.Value = model.text;

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@picture";
                    param3.Value = model.picture;

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@id";
                    param4.Value = model.id;

           
                   


                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    cmd.Parameters.Add(param4);
               

                    reader = cmd.ExecuteReader();

                    

                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }

                return RedirectToAction("ProjectArea", "Home");

            }
        }


        public ActionResult ResearchProject()
        {
            DataTable research = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT *FROM research", sqlConn);
                adapter.Fill(research);


            }
            return View(research);
        }


        public ActionResult AdminProject()
        {
          

            DataTable projects = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT *FROM registration inner join project on student_id_fk=reg_id", sqlConn);
                adapter.Fill(projects);



            }
            return View(projects);
        }

        public ActionResult AdminProjectEdit(int id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult AdminProjectEdit(ProjectModel model,string returnUrl)
        {
            String file_name = Path.GetFileNameWithoutExtension(model.project_image.FileName);
            String extension = Path.GetExtension(model.project_image.FileName);

            file_name += DateTime.Now.ToString("yymmssff") + extension;

            model.picture = file_name;

            file_name = Path.Combine(Server.MapPath("~/Assets/"), file_name);

            model.project_image.SaveAs(file_name);

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Update project set project_title=@title,project_text=@text,project_picture=@picture,student_id_fk=@student_fk WHERE project_id=@id ", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@title";
                    param.Value = model.title;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@text";
                    param2.Value = model.text;

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@picture";
                    param3.Value = model.picture;

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@id";
                    param4.Value = model.id;

                    SqlParameter param5 = new SqlParameter();
                    param5.ParameterName = "@student_fk";
                    param5.Value = model.student_id_fk;
;



                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    cmd.Parameters.Add(param4);
                    cmd.Parameters.Add(param5);
                    reader = cmd.ExecuteReader();

                    
                
                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }

            return RedirectToAction("AdminProject", "Home");
        }

        public ActionResult AdminProjectAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminProjectAdd(ProjectModel model,string returnUrl)
        {
            String file_name = Path.GetFileNameWithoutExtension(model.project_image.FileName);
            String extension = Path.GetExtension(model.project_image.FileName);

            file_name += DateTime.Now.ToString("yymmssff") + extension;

            model.picture = file_name;

            file_name = Path.Combine(Server.MapPath("~/Assets/"), file_name);

            model.project_image.SaveAs(file_name);

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO project(project_title,project_text,project_picture,student_id_fk) Values(@title,@text,@picture,@student_id)", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@title";
                    param.Value = model.title;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@text";
                    param2.Value = model.text;

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@picture";
                    param3.Value = model.picture;

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@student_id";
                    param4.Value = model.student_id_fk;


                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    cmd.Parameters.Add(param4);

                    reader = cmd.ExecuteReader();


                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }

            return RedirectToAction("AdminProject", "Home");
        }

        public ActionResult AdminProjectDelete(int id)
        {
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM project WHERE project_id=@id", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@id";
                    param.Value = id;

                    cmd.Parameters.Add(param);

                    reader = cmd.ExecuteReader();

                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }

           return  RedirectToAction("AdminProject", "Home");
            
        }


        public ActionResult AdminResearch()
        {

            DataTable research = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT *FROM research", sqlConn);
                adapter.Fill(research);



            }
            return View(research);
        }

        public ActionResult AdminResearchAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminResearchAdd(ResearchModel model, string returnUrl)
        {
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO research(research_title,research_people,research_year) Values(@title,@people,@year)", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@title";
                    param.Value = model.title;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@people";
                    param2.Value = model.people_involved;


                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@year";
                    param3.Value = model.year;


                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
      
                    reader = cmd.ExecuteReader();


                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            return RedirectToAction("AdminResearch", "Home");
        }


        public ActionResult AdminResearchEdit(int id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult AdminResearchEdit(ResearchModel model, string returnUrl)
        {

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE research set research_title=@title,research_people=@people,research_year=@year WHERE research_id=@id", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@title";
                    param.Value = model.title;


                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@id";
                    param2.Value = model.id;


                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@people";
                    param3.Value = model.people_involved;

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@year";
                    param4.Value = model.year;

                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    cmd.Parameters.Add(param4);
        

                    reader = cmd.ExecuteReader();


                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }

            return RedirectToAction("AdminResearch", "Home");
        }

        public ActionResult AdminResearchDelete(int id)
        {
             try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM research WHERE research_id=@id", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@id";
                    param.Value = id;

                    cmd.Parameters.Add(param);

                    reader = cmd.ExecuteReader();

                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            

           return  RedirectToAction("AdminResearch", "Home");
            
        }


        public ActionResult AdminGroup()
        {

            DataTable group = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM registration INNER JOIN studentgroup on student_id_fk=reg_id", sqlConn);
                adapter.Fill(group);



            }

            return View(group);
        }

        public ActionResult AdminGroupAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminGroupAdd(GroupModel model,string returnUrl)
        {
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO studentgroup(group_title,student_id_fk) Values (@title,@student_id)", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@title";
                    param.Value = model.title;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@student_id";
                    param2.Value = model.sstudent_id_fk;


                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);

                    reader = cmd.ExecuteReader();

                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }

            }
            return RedirectToAction("AdminGroup", "Home");
        }

        public ActionResult AdminGroupEdit(int id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult AdminGroupEdit(GroupModel model,string returnUrl)
        {


            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE  studentgroup set group_title=@title,student_id_fk=@student_id WHERE group_id=@id", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@title";
                    param.Value = model.title;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@student_id";
                    param2.Value = model.sstudent_id_fk;

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@id";
                    param3.Value = model.id;


                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);

                    reader = cmd.ExecuteReader();

                   

                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }

            return RedirectToAction("AdminGroup", "Home");
        }

        public ActionResult AdminGroupDelete(int id)
        {
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM studentgroup WHERE group_id=@id", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@id";
                    param.Value = id;

                    cmd.Parameters.Add(param);

                    reader = cmd.ExecuteReader();

                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }

            return RedirectToAction("AdminGroup", "Home");
            
        }

        public ActionResult Group()
        {
            DataTable group = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT *FROM registration INNER JOIN studentgroup on student_id_fk=reg_id", sqlConn);
                adapter.Fill(group);



            }


            return View(group);
        }


        public ActionResult Profile()
        {

            ViewBag.id = Session["user_id"];

            return View();
        }

        [HttpPost]
        public ActionResult Profile(RegisterModel model, string returnUrl)
        {


            String file_name = Path.GetFileNameWithoutExtension(model.user_image.FileName);
            String extension = Path.GetExtension(model.user_image.FileName);

            file_name += DateTime.Now.ToString("yymmssff") + extension;

            model.picture = file_name;

            file_name = Path.Combine(Server.MapPath("~/Assets/"), file_name);

            model.user_image.SaveAs(file_name);

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Update registration set reg_username=@username,reg_password=@password,reg_name=@name,reg_pic=@pic WHERE reg_id=@id ", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@username";
                    param.Value = model.username;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@password";
                    param2.Value = model.password;

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@pic";
                    param3.Value = model.picture;

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@id";
                    param4.Value = model.id;


                    SqlParameter param5 = new SqlParameter();
                    param5.ParameterName = "@name";
                    param5.Value = model.name;





                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    cmd.Parameters.Add(param4);
                    cmd.Parameters.Add(param5);

                    reader = cmd.ExecuteReader();



                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }

                return RedirectToAction("Index", "Home");

            }

        }


        [HttpGet]
        public ActionResult Profile2(int id)
        {

          

            DataTable group = new DataTable();

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                

                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT *FROM studentprofile WHERE student_fk_id=@id", sqlConn);
                adapter.SelectCommand.Parameters.AddWithValue("@id",id);
                adapter.Fill(group);



            }


            return View(group);
        }


        public ActionResult EditProfile2()
        {

            return View();
        }

        [HttpPost]
        public ActionResult EditProfile2(Profile model,String returnUrl)
        {

            String file_name = Path.GetFileNameWithoutExtension(model.profile_image.FileName);
            String extension = Path.GetExtension(model.profile_image.FileName);

            file_name += DateTime.Now.ToString("yymmssff") + extension;

            model.picture = file_name;

            file_name = Path.Combine(Server.MapPath("~/Assets/"), file_name);

            model.profile_image.SaveAs(file_name);

            using (SqlConnection sqlConn = new SqlConnection(connString))
            {


                try
                {
                    conn = new SqlConnection(connString);
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("Update studentprofile set profile_title=@title,profile_collaborator=@collab,profile_industrial_collaborator=@industrial,profile_description=@text,profile_image=@pic WHERE student_fk_id=@ppp ", conn);

                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@title";
                    param.Value = model.title;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@ppp";
                    param2.Value = Session["user_id"];

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@collab";
                    param3.Value = model.collaborator;

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@industrial";
                    param4.Value = model.industrial;


                    SqlParameter param5 = new SqlParameter();
                    param5.ParameterName = "@text";
                    param5.Value = model.description;


                    SqlParameter param6 = new SqlParameter();
                    param6.ParameterName = "@pic";
                    param6.Value = model.picture;

                    cmd.Parameters.Add(param);
                    cmd.Parameters.Add(param2);
                    cmd.Parameters.Add(param3);
                    cmd.Parameters.Add(param4);
                    cmd.Parameters.Add(param5);
                    cmd.Parameters.Add(param6);


                    reader = cmd.ExecuteReader();



                }
                catch (SqlException e)
                { }
                finally
                {

                    if (reader != null)
                    {
                        reader.Close();
                    }

                    if (conn != null)
                    {
                        conn.Close();
                    }
                }

                return RedirectToAction("ProjectArea", "Home");

            }
        }

    }
}