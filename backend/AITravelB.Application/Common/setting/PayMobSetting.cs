using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.setting
{
    public class PayMobSetting
    {
        public string ApiKey { get; set; } = string.Empty;
        public int IntegrationId { get; set; }
        public string HmacSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string IFrameId { get; set; } = string.Empty;
    }
}
