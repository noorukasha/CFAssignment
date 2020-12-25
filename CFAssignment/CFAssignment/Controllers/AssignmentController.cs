using CFAssignment.Helper;
using CFAssignment.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CFAssignment.Controllers
{
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

        [HttpGet]
        [Route("students")]
        public async Task<IActionResult> GetStudets()
        {
            try
            {
                var students = await MysqlDB.GetQuery();
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Some error occured, check logs for more info.");
            }
        }


        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(List<Student> student)
        {
            try
            {
                int rows = await MysqlDB.InsertStudentRecord(student);
                _logger.LogInformation($"Number of rows added successfully: {rows}");
                return Ok($"Number of rows added: {rows}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Some error occured, check logs for more info.");
            }
        }
    }
}
