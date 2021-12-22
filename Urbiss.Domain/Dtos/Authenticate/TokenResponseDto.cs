using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Urbiss.Domain.Dtos
{
    public class TokenResponseDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public bool IsVerified { get; set; }
        public string JWToken { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
