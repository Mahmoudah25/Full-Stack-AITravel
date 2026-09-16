using AITravelB.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Common.Interfaces
{
    public interface IPlacesService
    {
        Task<List<PlaceDto>> SearchPlacesAsync(string city,string category);
    }
}
