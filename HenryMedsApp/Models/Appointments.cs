namespace HenryMedsApp.Models
{
    public class Appointments
    {
        public Appointments() { }
        public Appointments(int providerId, DateTime startTime, DateTime endTime) { 
            this.ProviderId = providerId;
            this.StartDate = startTime;
            this.EndDate = endTime;
        }
        public int ProviderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Appointments other = (Appointments)obj;
            return ProviderId == other.ProviderId && StartDate == other.StartDate && EndDate == other.EndDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProviderId, StartDate, EndDate);
        }
    }
}
