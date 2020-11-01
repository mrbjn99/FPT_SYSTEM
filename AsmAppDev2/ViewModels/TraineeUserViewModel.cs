using AsmAppDev2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AsmAppDev2.ViewModels
{
  public class TraineeUserViewModel
  {
    public TraineeUser TraineeUser { get; set; }
    public IEnumerable<ApplicationUser> Trainees { get; set; }
  }
}