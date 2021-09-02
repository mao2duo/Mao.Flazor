using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Flazor.Features.Attributes
{
    /// <summary>
    /// 設置 View 的 helper 或 function 將結果存放至哪個 js 的變數名稱
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    public class FlazorReturnVariableAttribute : Attribute
    {
        public string Variable { get; }

        public FlazorReturnVariableAttribute(string variable) => Variable = variable;
    }
}