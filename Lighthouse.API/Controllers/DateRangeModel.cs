using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Lighthouse.API.Controllers;

public class DateRangeModel
{
  [FromQuery]
  [Required(ErrorMessage = "StartDate is required.")]
  public DateTime StartDate { get; set; }
  
  [FromQuery]
  [Required(ErrorMessage = "EndDate is required.")]
  public DateTime EndDate { get; set; }
}