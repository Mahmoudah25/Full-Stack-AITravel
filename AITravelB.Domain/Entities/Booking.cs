using AITravelB.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelB.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid TripId { get; private set; }
        public Guid ActivityId { get; private set; }
        public BookingStatus Status { get; private set; }
        public DateTime BookingDate { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency {  get; private set; }
        public string CustomerEmail { get; private set; } = string.Empty;
        public string? PaymobOrderId { get; private set; } = string.Empty;
        private Booking() { }
        public Booking(Guid tripId, Guid activityId, decimal amount,string currency , string customerEmail)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                throw new ArgumentException("Customer email cannot be null or empty.");
            }
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.");
            }
            if (tripId == Guid.Empty)
            {
                throw new ArgumentException("TripId cannot be empty.");
            }
            if (activityId == Guid.Empty)
            {
                throw new ArgumentException("ActivityId cannot be empty.");
            }
            Id = Guid.NewGuid();
            TripId = tripId;
            ActivityId = activityId;
            Amount = amount;
            Currency = string.IsNullOrWhiteSpace(currency) ? "EGP" : currency.ToUpperInvariant();
            CustomerEmail = customerEmail;
            Status = BookingStatus.Pending;
            BookingDate = DateTime.UtcNow;
        }

        public void MarkAsPaid(string paymobOrderId)
        {
            if (string.IsNullOrWhiteSpace(paymobOrderId))
            {
                throw new ArgumentException("Paymob Order Id cannot be null or empty.");
            }
            PaymobOrderId = paymobOrderId;
            Status = BookingStatus.Paid;
        }
        public void Confirm()
        {
            if (Status != BookingStatus.Paid)
            {
                throw new InvalidOperationException("Booking can only be confirmed if it is paid.");
            }
            Status = BookingStatus.Confirmed;
        }
        public void MarkAsFailed()
        {
            Status = BookingStatus.Failed;
        }
    }
}
