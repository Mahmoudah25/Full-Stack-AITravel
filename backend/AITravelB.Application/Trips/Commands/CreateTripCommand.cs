using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Application.Trips.Commands
{
    public class CreateTripCommand : IRequest<Guid> // return guid
    {
        public string Destination { set; get; } = string.Empty;
        public DateTime StartDate { set; get; }
        public int Days { set; get; }
        public decimal BudgetAmount { set; get; }
        public string BudgetCurrency { set; get; } = string.Empty;  
    }
}
