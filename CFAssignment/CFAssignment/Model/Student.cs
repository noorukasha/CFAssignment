using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFAssignment.Model
{
    /// <summary>
    /// Student Details.
    /// </summary>
    public class Student
    {
        public string Name { get; set; }
        public string ClassNumber { get; set; }
        public int Mark { get; set; }
        public string Gender { get; set; }
    }

    public class StudentModel: Student
    {
        public string id { get; set; }
    }

    public class APIDetails
    {
        public int NumberOfRecords { get; set; }
        public string ResponseTimeinSec { get; set; }
    }

    public class ResponseObject
    {
        public APIDetails APIDetails { get; set; }
        public List<StudentModel> Data { get; set; }
    }
}
