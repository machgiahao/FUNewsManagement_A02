﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagement.Services.DTOs.SystemAccount
{
    public class AccountDto
    {

        public short AccountId { get; set; }

        public string? AccountName { get; set; }

        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; }

    }
}
