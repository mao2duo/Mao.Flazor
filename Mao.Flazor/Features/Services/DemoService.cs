using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mao.Flazor.Features.Services
{
    public class DemoService
    {
        /// <summary>
        /// 依照第一個下拉選單的值，取得第二個下拉選單的選項
        /// </summary>
        public SelectListItem[] GetSecondaryOptions(string primarySelectedValue)
        {
            List<SelectListItem> secondaryOptions = new List<SelectListItem>();
            if (primarySelectedValue == "A")
            {
                secondaryOptions.Add(new SelectListItem() { Value = "A001", Text = "A001", Selected = false });
                secondaryOptions.Add(new SelectListItem() { Value = "A002", Text = "A002", Selected = true });
                secondaryOptions.Add(new SelectListItem() { Value = "A003", Text = "A003", Selected = false });
            }
            if (primarySelectedValue == "B")
            {
                secondaryOptions.Add(new SelectListItem() { Value = "B001", Text = "B001", Selected = false });
                secondaryOptions.Add(new SelectListItem() { Value = "B002", Text = "B002", Selected = false });
                secondaryOptions.Add(new SelectListItem() { Value = "B003", Text = "B003", Selected = false });
                secondaryOptions.Add(new SelectListItem() { Value = "B004", Text = "B004", Selected = true });
            }
            return secondaryOptions.ToArray();
        }
    }
}