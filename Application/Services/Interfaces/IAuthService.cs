using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct);
        Task<AuthResponseDto> RefreshAsync(string refreshToken, CancellationToken ct);
    }
}
