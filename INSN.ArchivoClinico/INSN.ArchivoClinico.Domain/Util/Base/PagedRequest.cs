using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Domain.UtilFactory.Base
{
    public class PagedRequest<T>
    {
        private int _page;
        private int _pageSize;
        private int _rows;
        private T? filtro;

        public PagedRequest()
        {
            this._pageSize = 10;
        }

        public int GetTotalPages(int itemCount)
        {
            this._rows = itemCount;
            return (((itemCount + this.PageSize) - 1) / this.PageSize);
        }

        public int GetTotalRows()
        {
            return this._rows;
        }

        public T Filtro
        {
            get
            {
                return this.filtro;
            }
            set
            {
                this.filtro = value;
            }
        }

        public string? ColumnName { get; set; }

        public int Offset
        {
            get
            {
                return ((this.Page - 1) * this.PageSize);
            }
        }

        //public OrderPagedEnum Order { get; set; }

        public int Page
        {
            get
            {
                return this._page;
            }
            set
            {
                this._page = (value > 0) ? value : 1;
            }
        }

        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                this._pageSize = (value > 0) ? value : this._pageSize;
            }
        }
    }
}

