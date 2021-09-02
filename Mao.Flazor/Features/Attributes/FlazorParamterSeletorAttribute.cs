using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Flazor.Features.Attributes
{
    /// <summary>
    /// 設置 View 的 helper 或 function 參數來源的 jQuery selector
    /// <para>EX: #input-name</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FlazorParamterSeletorAttribute : Attribute
    {
        public string Seletor { get; }

        public FlazorParamterSeletorAttribute(string seletor) => Seletor = seletor;
    }
}