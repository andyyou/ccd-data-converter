using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcdDataConverter.Model
{
    class LogFormat
    {
        public LogFormat()
        {
            InputDateTime = "";
            InputData = "";
            Delimiter = ",";
            OutputData = "";
            OutputDateTime = "";
            Status = "N";
        }

        public string InputDateTime { get; set; }
        //public string FlawID { get; set; }
        //public string FlawName { get; set; }
        //public string InputMD { get; set; }
        //public string InputCD { get; set; }
        //public string JobID { get; set; }
        public string InputData { get; set; }
        public string Delimiter { get; set; }
        //public string Type { get; set; }
        //public string OutputMD { get; set; }
        //public string OutputCD { get; set; }
        public string OutputData { get; set; }
        public string OutputDateTime { get; set; }
        public string Status { get; set; }
    }
}
