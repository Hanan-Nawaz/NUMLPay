using NUMLPay_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NUMLPay_WebApp.Services
{
    public class statusService
    {
        //All Statuses
        public List<SelectListItem> getStatus(int? selectedValue)
        {
            List<SelectListItem> statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Active" },
                new SelectListItem { Value = "2", Text = "Inactive" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in statusOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return statusOptions;
        }

        //All Methods
        public List<SelectListItem> getMethod(int? selectedValue)
        {
            List<SelectListItem> methodOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Online" },
                new SelectListItem { Value = "2", Text = "By Bank" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in methodOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return methodOptions;
        }

        //All Statuses Users
        public List<SelectListItem> getStatusUsers(int? selectedValue)
        {
            List<SelectListItem> statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Verified" },
                new SelectListItem { Value = "2", Text = "UnVerified" },
                new SelectListItem { Value = "3", Text = "Inactive" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in statusOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return statusOptions;
        }

        //All Statuses Degrees
        public List<SelectListItem> getStatusDegree(int? selectedValue)
        {
            List<SelectListItem> statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "InComplete" },
                new SelectListItem { Value = "2", Text = "Complete" },
                new SelectListItem { Value = "3", Text = "Freeze" },
                new SelectListItem { Value = "4", Text = "Ceased" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in statusOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return statusOptions;
        }

        //All Fees
        public List<SelectListItem> getFeesUsers(int? selectedValue)
        {
            List<SelectListItem> statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Yes" },
                new SelectListItem { Value = "2", Text = "No" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in statusOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return statusOptions;
        }

        //All Fees
        public List<SelectListItem> getFeeFor(int? selectedValue)
        {
            List<SelectListItem> feeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Tuition Fee" },
                new SelectListItem { Value = "2", Text = "Bus Fee" },
                new SelectListItem { Value = "3", Text = "Hostel Fee" },
                new SelectListItem { Value = "4", Text = "Mess Fee" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in feeOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return feeOptions;
        }

        public List<SelectListItem> getOtherFeeFor(int? selectedValue)
        {
            List<SelectListItem> feeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "2", Text = "Bus Fee" },
                new SelectListItem { Value = "3", Text = "Hostel Fee" },
                new SelectListItem { Value = "4", Text = "Mess Fee" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in feeOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return feeOptions;
        }

        public List<SelectListItem> getRoute(int? selectedValue)
        {
            List<SelectListItem> feeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "ISB / Pindi " },
                new SelectListItem { Value = "2", Text = "Taxila" }            };

            if (selectedValue.HasValue)
            {
                foreach (var option in feeOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return feeOptions;
        }

        //All Fees
        public List<SelectListItem> getFeeForTuition(int? selectedValue)
        {
            List<SelectListItem> feeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Tuition Fee" },
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in feeOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return feeOptions;
        }

        //All Fines
        public List<SelectListItem> getFineFor(int? selectedValue)
        {
            List<SelectListItem> feeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Tuition Fee" },
                new SelectListItem { Value = "2", Text = "Bus Fee" },
                new SelectListItem { Value = "3", Text = "Hostel Fee" },
                new SelectListItem { Value = "4", Text = "Mess Fee" },
                new SelectListItem { Value = "5", Text = "Other Fee" },
                new SelectListItem { Value = "7", Text = "Repeat Course Fee" },
                new SelectListItem { Value = "6", Text = "Summer Fee" }
            };

            if (selectedValue.HasValue)
            {
                foreach (var option in feeOptions)
                {
                    option.Selected = option.Value == selectedValue.Value.ToString();
                }
            }

            return feeOptions;
        }
    }
}