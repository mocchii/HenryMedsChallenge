using HenryMedsApp.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HenryMedsApp.Models;

public partial class ClientBooking
{
    [Key]
    public int BookingId { get; set; }
    [Required(ErrorMessage = "ClientId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ClientId must be greater than 0.")]
    public int ClientId { get; set; }
    [Required(ErrorMessage = "ProviderId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ProviderId must be greater than 0.")]
    public int ProviderId { get; set; }
    [Required(ErrorMessage = "Start date is required.")]
    [AppointmentInterval(ErrorMessage = "Start date must be in 15 minute intervals.")]
    public DateTime StartDate { get; set; }
    [Required(ErrorMessage = "End date is required.")]
    [AppointmentInterval(ErrorMessage = "End date must be in 15 minute intervals.")]
    public DateTime EndDate { get; set; }

    public bool IsReserved { get; set; }

    public bool Active { get; set; }

    public DateTime CreateDate { get; set; }
}
