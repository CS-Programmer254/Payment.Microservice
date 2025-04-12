using PayPal.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayPal.Application.services
{
    public interface IPayPalService
    {
        Task<PayoutResponseDto> SendPayoutAsync(PayoutRequestDto request);
    }

}
