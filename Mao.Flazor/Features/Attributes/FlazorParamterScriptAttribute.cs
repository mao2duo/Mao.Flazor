using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Flazor.Features.Attributes
{
    /// <summary>
    /// 設置 View 的 helper 或 function 參數來源的 js 語法
    /// <para>EX: $(":text").val()</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FlazorParamterScriptAttribute : Attribute
    {
        public string Script { get; }

        public FlazorParamterScriptAttribute(string script) => Script = script;
    }
}