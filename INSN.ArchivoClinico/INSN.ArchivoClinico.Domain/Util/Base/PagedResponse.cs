using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Domain.UtilFactory.Base
{
    public class PagedResponse<T>
    {
        public PagedResponse<TResult> Transform<TResult>(Func<T, TResult> expression)
        {
            return new PagedResponse<TResult>
            {
                TotalRows = this.TotalRows,
                Data = this.Data.Select<T, TResult>(expression).ToArray<TResult>()
            };
        }
        public ICollection<T> Data { get; set; }
        public int TotalPages { get; set; }
        public int TotalRows { get; set; }
        public string? Message { get; set; }
        public bool success { get; set; }
    }
}
