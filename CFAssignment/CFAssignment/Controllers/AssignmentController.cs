using CFAssignment.Helper;
using CFAssignment.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Assignment App.
/// </summary>
namespace CFAssignment.Controllers
{
    /// <summary>
    /// Main controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        public IConfiguration _config;
        private readonly ILogger _logger;
        public MysqlDB MysqlDB;

        public AssignmentController(IConfiguration configuration, MysqlDB mysqlDB, ILogger<AssignmentController> logger)
        {
            _config = configuration;
            _logger = logger;
            MysqlDB = mysqlDB;
        }

        /// <summary>
        /// To get all students from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("students")]
        public async Task<IActionResult> GetStudets()
        {
            try
            {
                _logger.LogInformation("Get API hit.");
                var students = await MysqlDB.GetQuery();
                _logger.LogInformation("Get API successful.");
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Some error occured, check logs for more info.");
            }
        }

        /// <summary>
        /// To Insert a group of students in DB.
        /// </summary>
        /// <param name="students">List of student records</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Insert")]
        public async Task<IActionResult> Insert(List<Student> students)
        {
            try
            {
                _logger.LogInformation("Insert API hit.");
                var response = await MysqlDB.InsertStudentRecord(students);
                _logger.LogInformation("Insert API successfull.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Some error occured, check logs for more info.");
            }
        }

        /// <summary>
        /// Update a student record
        /// </summary>
        /// <param name="student">Student record</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(StudentModel student)
        {
            try
            {
                _logger.LogInformation("Update API Hit");
                bool res = await MysqlDB.UpdateStudentRecord(student);
                _logger.LogInformation($"Result of record update: {res}");
                _logger.LogInformation("Update API successfull.");
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Some error occured, check logs for more info.");
            }
        }

        /// <summary>
        /// Delete a student record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Delete API hit.");
                bool res = await MysqlDB.DeleteStudentRecord(id);
                _logger.LogInformation($"Result of record delete: {res}");
                if (res)
                    return Ok(res);
                else
                    return BadRequest("Student with that Id not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Some error occured, check logs for more info.");
            }
        }
    }
}
