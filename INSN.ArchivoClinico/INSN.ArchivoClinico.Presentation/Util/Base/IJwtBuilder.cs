using INSN.ArchivoClinico.Models;

namespace INSN.ArchivoClinico.Presentation.Util.Base
{
    public interface IJwtBuilder
    {        string GetToken(LoginViewModel usuario);
    }
}
