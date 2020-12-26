using CFAssignment.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CFAssignment.Helper
{
    public class MysqlDB
    {
        public IConfiguration _config;
        private readonly ILogger _logger;
        internal MySqlConnectionStringBuilder MySqlConnectionStringBuilder { get; set; }
        internal List<string> GenderList = new List<string> { "male", "female", "other" };

        public MysqlDB(IConfiguration configuration, ILogger<MysqlDB> logger)
        {
            _config = configuration;
            _logger = logger;
            MySqlConnectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = _config["DatabaseDetails:Server"],
                UserID = _config["DatabaseDetails:UserID"],
                Password = _config["DatabaseDetails:Password"],
                Database = _config["DatabaseDetails:Database"],
                SslMode = MySqlSslMode.Required,
            };
        }

        public async Task<ResponseObject> GetQuery()
        {
            ResponseObject responseObject = new ResponseObject
            {
                APIDetails = new APIDetails(),
                Data = new List<StudentModel>()
            };
            using (var conn = new MySqlConnection(MySqlConnectionStringBuilder.ConnectionString))
            {
                await conn.OpenAsync();
                using var command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM student;";
                DateTime startTime = DateTime.Now;
                using var reader = await command.ExecuteReaderAsync();
                var dt = new DataTable();
                dt.Load(reader);
                DateTime endTime = DateTime.Now;
                var resTime = endTime - startTime;
                foreach (DataRow dr in dt.Rows)
                {
                    var student = new StudentModel
                    {
                        id = dr["id"].ToString(),
                        Name = dr["name"].ToString(),
                        ClassNumber = dr["class"].ToString(),
                        Mark = int.Parse(dr["mark"].ToString()),
                        Gender = dr["sex"].ToString()
                    };
                    responseObject.Data.Add(student);
                }
                responseObject.APIDetails.NumberOfRecords = responseObject.Data.Count;
                responseObject.APIDetails.ResponseTimeinSec = resTime.TotalSeconds.ToString();
                return responseObject;
            }
        }

        public async Task<APIDetails> InsertStudentRecord(List<Student> students)
        {
            try
            {
                APIDetails aPIDetails = new APIDetails();
                StringBuilder cmdString = new StringBuilder("INSERT INTO student (name, class, mark, sex) VALUES");
                using var conn = new MySqlConnection(MySqlConnectionStringBuilder.ConnectionString);
                
                List<string> datarows = new List<string>();
                int rejectCount = 0;
                foreach (var student in students)
                {
                    // some validations
                    if (student.Mark > 100 || student.Name.Length > 50 || student.ClassNumber.Length > 50 
                        || string.IsNullOrEmpty(GenderList.Find(x => student.Gender.Equals(x, StringComparison.OrdinalIgnoreCase))))
                    {
                        _logger.LogInformation($"Record rejected: {JsonConvert.SerializeObject(student)}");
                        rejectCount++;
                        continue;
                    }
                    datarows.Add(string.Format("('{0}','{1}', '{2}', '{3}')", MySqlHelper.EscapeString(student.Name),
                        MySqlHelper.EscapeString(student.ClassNumber), MySqlHelper.EscapeString(student.Mark.ToString()),
                        MySqlHelper.EscapeString(student.Gender?.ToLower())));
                }
                cmdString.Append(string.Join(",", datarows));
                cmdString.Append(";");
                await conn.OpenAsync();
                int rowCount = 0;
                DateTime startTime = DateTime.Now;
                using (MySqlCommand myCmd = new MySqlCommand(cmdString.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    rowCount = myCmd.ExecuteNonQuery();
                }
                var resTime = DateTime.Now - startTime;
                _logger.LogInformation($"Number of records rejected: {rejectCount}");
                _logger.LogInformation($"Number of records inserted: {rowCount}");
                aPIDetails.ResponseTimeinSec = resTime.TotalSeconds.ToString();
                aPIDetails.NumberOfRecords = rowCount;
                return aPIDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateStudentRecord(StudentModel student)
        {
            try
            {
                using var conn = new MySqlConnection(MySqlConnectionStringBuilder.ConnectionString);
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE student SET name = @name, class = @class, mark = @mark, sex = @sex WHERE id = @id;";
                    command.Parameters.AddWithValue("@id", student.id);
                    command.Parameters.AddWithValue("@name", student.Name);
                    command.Parameters.AddWithValue("@class", student.ClassNumber);
                    command.Parameters.AddWithValue("@mark", student.Mark);
                    command.Parameters.AddWithValue("@sex", student.Gender);
                    int rowCount = await command.ExecuteNonQueryAsync();
                    return rowCount > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteStudentRecord(string id)
        {
            try
            {
                using var conn = new MySqlConnection(MySqlConnectionStringBuilder.ConnectionString);
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "DELETE FROM student WHERE id = @id;";
                    command.Parameters.AddWithValue("@id", id);
                    int rowCount = await command.ExecuteNonQueryAsync();
                    return rowCount > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
