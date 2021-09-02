using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Flazor.Features.Attributes
{
    /// <summary>
    /// 設置 View 的 helper 或 function 將結果作為哪個 js 方法的參數呼叫方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
    public class FlazorReturnCallbackAttribute : Attribute
    {
        public string Function { get; }

        public FlazorReturnCallbackAttribute(string function) => Function = function;
    }
}