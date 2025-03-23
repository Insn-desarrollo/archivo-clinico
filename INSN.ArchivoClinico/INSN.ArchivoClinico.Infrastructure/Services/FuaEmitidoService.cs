using INSN.ArchivoClinico.Application.Interfaces;
using INSN.ArchivoClinico.Domain.Entities;
using INSN.ArchivoClinico.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace INSN.ArchivoClinico.Infrastructure.Services
{
    public class FuaEmitidoService : IFuaEmitidoService
    {
        private readonly IAtencionRepository _atencionRepository;
        private readonly IConfiguration _configuration;

        public FuaEmitidoService(IAtencionRepository atencionRepository, IConfiguration configuration)
        {
            _atencionRepository = atencionRepository;
            _configuration = configuration;
        }

        public async Task<long> RegistrarFuaAsync(Atencion atencion)
        {
            return await _atencionRepository.RegistrarFuaAsync(atencion);
        }



    }
       

}
