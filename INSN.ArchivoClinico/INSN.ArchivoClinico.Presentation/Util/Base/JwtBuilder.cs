﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using INSN.ArchivoClinico.Models;

namespace INSN.ArchivoClinico.Presentation.Util.Base
{
    public class JwtBuilder : IJwtBuilder
    {
        private readonly IConfiguration _configuration;

        public JwtBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
            //var secretKey = _configuration["Jwt:Key"]; // Accede a la clave secreta desde appsettings.json
        }

        public string GetToken(LoginViewModel usuario)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Usuario),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
