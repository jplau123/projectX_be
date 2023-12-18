using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace project_backend.Helpers
{
    public static class JwtHelper
    {
        public static SecurityToken GetJwtToken(
        string userName,
        string jwtKey,
        string issuer,
        string audience,
        TimeSpan expiration,
        ICollection<Claim> additionalClaims = null!)
        {
            Claim[] claims =
            [
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            if(additionalClaims is object)
            {
                List<Claim> claimList = new(claims);
                claimList.AddRange(additionalClaims);
                claims = claimList.ToArray();
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiration),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = creds
            };

            return new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
        }
    }
}
