﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs.Account
{
    public class TwoFactorAuthSetupRequestDto
    {
        public required string VerificationCode { get; set; }
    }
}