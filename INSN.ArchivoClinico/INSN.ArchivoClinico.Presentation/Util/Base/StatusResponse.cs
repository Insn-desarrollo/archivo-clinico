using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.UtilFactory.Base
{
    public class StatusResponse<T> : StatusResponse
    {
        public T? Data { get; set; }

        //public List<T> Lista { get; set; }
    }
    public class StatusResponse
    {
        public StatusResponse()
        {
            this.Messages = new List<string>();
            this.ListMessages = new Dictionary<string, object>();
        }

        public bool Success { get; set; }
        //public string Url { get; set; }

        public string? Message { get; set; }

        //public object Value { get; set; }

        public List<string> Messages { get; set; }

        public Dictionary<string, object> ListMessages { get; set; }
        //public string ResultBody { get; set; }
    }
}

