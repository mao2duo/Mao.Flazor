using Mao.Flazor.Features.Attributes;
using Mao.Flazor.Features.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mao.Flazor.Views.Home
{
    public abstract class Demo05<T> : WebViewPage<T>
    {
        // 2. 透過 [FlazorReturnVariable] 設置將結果存放置變數 secondaryOptions
        [FlazorReturnVariable("secondaryOptions")]
        SelectListItem[] GetSecondaryOptions(
            // 1. 透過 [FlazorParamterSeletor] 設置參數來源於元件 select-primary
            [FlazorParamterSeletor("#select-primary")] string primarySelectedValue)
        {
            var demoService = DependencyResolver.Current.GetService<DemoService>();
            var secondaryOptions = demoService.GetSecondaryOptions(primarySelectedValue);
            return secondaryOptions;
        }
    }

    public class Demo05_ViewModel
    {
        public string PrimaryValue { get; set; }
        public SelectListItem[] SecondaryOptions { get; set; }
    }
}