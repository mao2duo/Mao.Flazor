using Mao.Flazor.Features.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mao.Flazor.Views.Home
{
    public abstract class Demo03 : WebViewPage
    {
        // 2. 透過 [FlazorReturnVariable] 設置將結果存放置變數 secondaryOptions
        [FlazorReturnVariable("secondaryOptions")]
        SelectListItem[] GetSecondaryOptions(
            // 1. 透過 [FlazorParamterSeletor] 設置參數來源於元件 select-primary
            [FlazorParamterSeletor("#select-primary")] string primarySelectedValue)
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