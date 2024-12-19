using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Pokemon.Interview.Enums;


public enum ResultStatus
{
    [Display(Name = "Ok")]
    Ok = 200,
    [Display(Name = "Invalid")]
    Invalid = 400,
    [Display(Name = "NotFound")]
    NotFound = 404,
}
