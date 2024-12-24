using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace X.Admin.Core.Demo
{
    public class DemoApplicationService : ApplicationService
    {
        public DemoApplicationService()
        {
            
        }
        [Authorize]
        public string GetMessage()
        {
            return "Your application is running.";
        }
    }
}
