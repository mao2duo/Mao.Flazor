using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Flazor.Features.Attributes
{
    /// <summary>
    /// 設置 View 的 helper 或 function 參數來源的 js 變數名稱
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FlazorParamterVariableAttribute : Attribute
    {
        public string Variable { get; }

        public FlazorParamterVariableAttribute(string variable) => Variable = variable;
    }
}