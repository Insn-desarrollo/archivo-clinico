using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INSN.ArchivoClinico.Application.Interfaces
{
    public interface IFuaEmitidoService
    {
        Task<long> RegistrarFuaAsync(Atencion atencion);
    }
}
