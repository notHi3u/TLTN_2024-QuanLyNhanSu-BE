﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Application.DTOs
{
    public class RecoveryCodeLoginRequestDto
    {
        public string UserName { get; set; }
        public string RecoveryCode {  get; set; }
    }
}
