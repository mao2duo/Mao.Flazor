using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Flazor.Features.Attributes
{
    /// <summary>
    /// 設置 View 的 helper 或 function 將結果作為哪個 jQuery selector 取得物件的 InnerHTML
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    public class FlazorReturnSeletorAttribute : Attribute
    {
        public string Seletor { get; }

        public FlazorReturnSeletorAttribute(string seletor) => Seletor = seletor;
    }
}