using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class academicLevelService
    {
        //All Statuses
        public List<SelectListItem> getLevel(int? selectedValue)
        {
            List<SelectListItem> academicOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "BS" },
                new SelectListItem { Value = "2", Text = "MS" },
                new SelectListItem { Value = "3", Text = "Phd" },
                new SelectListItem { Value = "4", Text = "Post Phd" },
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in academicOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return academicOptions;
        }

        // All Shifts
        public List<SelectListItem> getShift(int? selectedValue)
        {
            List<SelectListItem> shiftOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Morning" },
                new SelectListItem { Value = "2", Text = "Afternoon" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in shiftOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return shiftOptions;
        }
    }
}