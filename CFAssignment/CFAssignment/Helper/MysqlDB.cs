using CFAssignment.Model;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFAssignment.Helper
{
    public class MysqlDB
    {
        public IConfiguration _config;
        internal MySqlConnectionStringBuilder MySqlConnectionStringBuilder { get; set; }

        public MysqlDB(IConfiguration configuration)
        {
            _config = configuration;
            MySqlConnectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = _config["DatabaseDetails:Server"],
                UserID = _config["DatabaseDetails:UserID"],
                Password = _config["DatabaseDetails:Password"],
                Database = _config["DatabaseDetails:Database"],
                SslMode = MySqlSslMode.Required,
            };
        }

        public async Task<List<Student>> GetQuery()
        {
            List<Student> students = new List<Student>();
            using (var conn = new MySqlConnection(MySqlConnectionStringBuilder.ConnectionString))
            {
                await conn.OpenAsync();
                using var command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM student;";
                using var reader = await command.ExecuteReaderAsync();
                var dt = new DataTable();
                dt.Load(reader);
                foreach (DataRow dr in dt.Rows)
                {
                    var student = new Student
                    {
                        Name = dr["name"].ToString(),
                        ClassNumber = dr["class"].ToString(),
                        Mark = int.Parse(dr["mark"].ToString()),
                        Gender = dr["sex"].ToString()
                    };
                    students.Add(student);
                }
                return students;
            }
        }

        public async Task<int> InsertStudentRecord(List<Student> students)
        {
            try
            {
                StringBuilder cmdString = new StringBuilder("INSERT INTO student (name, class, mark, sex) VALUES");
                using var conn = new MySqlConnection(MySqlConnectionStringBuilder.ConnectionString);
                
                List<string> datarows = new List<string>();
                foreach (var student in students)
                    datarows.Add(string.Format("('{0}','{1}', '{2}', '{3}')", MySqlHelper.EscapeString(student.Name), 
                        MySqlHelper.EscapeString(student.ClassNumber), MySqlHelper.EscapeString(student.Mark.ToString()), 
                        MySqlHelper.EscapeString(student.Gender)));
                cmdString.Append(string.Join(",", datarows));
                cmdString.Append(";");
                await conn.OpenAsync();
                int rowCount = 0;
                using (MySqlCommand myCmd = new MySqlCommand(cmdString.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    rowCount += myCmd.ExecuteNonQuery();
                }
                return rowCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
