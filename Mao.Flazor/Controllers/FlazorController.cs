using Mao.Flazor.Features.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Mao.Flazor.Controllers
{
    public class FlazorController : Controller
    {
        /// <summary>
        /// 取得所有具有 [FlazorReturnVariable(variable)] 的方法需要哪些參數
        /// </summary>
        [HttpGet]
        public JsonResult BindVariable(string variable)
        {
            HashSet<string> parameterVariables = new HashSet<string>();
            HashSet<string> parameterSeletors = new HashSet<string>();
            HashSet<string> parameterScripts = new HashSet<string>();
            var viewPathArray = Request.QueryString.GetValues("viewPathArray[]");
            if (viewPathArray != null)
            {
                foreach (var viewPath in viewPathArray)
                {
                    var viewType = BuildManager.GetCompiledType(viewPath);
                    if (viewType != null)
                    {
                        var method = viewType
                            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                            .Concat(viewType.BaseType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                            .Where(x => x.GetCustomAttribute<FlazorReturnVariableAttribute>()?.Variable == variable
                                || x.GetParameters().Any(y => y.GetCustomAttribute<FlazorReturnVariableAttribute>()?.Variable == variable))
                            .FirstOrDefault();
                        if (method != null)
                        {
                            var parameters = method.GetParameters();
                            foreach (var parameter in parameters)
                            {
                                if (parameter.GetCustomAttribute<FlazorParamterVariableAttribute>() is FlazorParamterVariableAttribute paramterVariable)
                                {
                                    if (!string.IsNullOrEmpty(paramterVariable.Variable))
                                    {
                                        parameterVariables.Add(paramterVariable.Variable);
                                    }
                                }
                                else if (parameter.GetCustomAttribute<FlazorParamterSeletorAttribute>() is FlazorParamterSeletorAttribute paramterSeletor)
                                {
                                    if (!string.IsNullOrEmpty(paramterSeletor.Seletor))
                                    {
                                        parameterSeletors.Add(paramterSeletor.Seletor);
                                    }
                                }
                                else if (parameter.GetCustomAttribute<FlazorParamterScriptAttribute>() is FlazorParamterScriptAttribute paramterScript)
                                {
                                    if (!string.IsNullOrEmpty(paramterScript.Script))
                                    {
                                        parameterScripts.Add(paramterScript.Script);
                                    }
                                }
                                else
                                {
                                    // 預設參數名稱為 js 變數名稱
                                    parameterVariables.Add(parameter.Name);
                                }
                            }
                            break;
                        }
                    }
                }
            }

            return Json(new
            {
                Variables = parameterVariables,
                Seletors = parameterSeletors,
                Scripts = parameterScripts
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 執行所有具有 [FlazorReturnVariable(variable)] 的方法
        /// </summary>
        [HttpPost]
        public JsonResult BindVariable(string[] viewPathArray, string variable,
            Dictionary<string, string> parameterVariables,
            Dictionary<string, string> parameterSeletors,
            Dictionary<string, string> parameterScripts)
        {
            object methodResult = null;
            foreach (var viewPath in viewPathArray)
            {
                var viewType = BuildManager.GetCompiledType(viewPath);
                if (viewType != null)
                {
                    var method = viewType
                        .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Concat(viewType.BaseType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                        .Where(x => x.GetCustomAttribute<FlazorReturnVariableAttribute>()?.Variable == variable
                            || x.GetParameters().Any(y => y.GetCustomAttribute<FlazorReturnVariableAttribute>()?.Variable == variable))
                        .FirstOrDefault();
                    if (method != null)
                    {
                        var methodParameters = method.GetParameters();
                        var invokeParameters = new object[methodParameters.Length];
                        for (int i = 0; i < methodParameters.Length; i++)
                        {
                            var methodParameter = methodParameters[i];
                            string invokeParameterJson = null;
                            if (methodParameter.GetCustomAttribute<FlazorParamterVariableAttribute>() is FlazorParamterVariableAttribute paramterVariable)
                            {
                                parameterVariables.TryGetValue(paramterVariable.Variable, out invokeParameterJson);
                            }
                            else if (methodParameter.GetCustomAttribute<FlazorParamterSeletorAttribute>() is FlazorParamterSeletorAttribute paramterSeletor)
                            {
                                parameterSeletors.TryGetValue(paramterSeletor.Seletor, out invokeParameterJson);
                            }
                            else if (methodParameter.GetCustomAttribute<FlazorParamterScriptAttribute>() is FlazorParamterScriptAttribute paramterScript)
                            {
                                parameterScripts.TryGetValue(paramterScript.Script, out invokeParameterJson);
                            }
                            else
                            {
                                // 預設使用參數名稱當作變數名稱
                                parameterVariables.TryGetValue(methodParameter.Name, out invokeParameterJson);
                            }
                            if (!string.IsNullOrEmpty(invokeParameterJson))
                            {
                                invokeParameters[i] = JsonConvert.DeserializeObject(invokeParameterJson, methodParameter.ParameterType);
                            }
                            else if (methodParameter.HasDefaultValue)
                            {
                                invokeParameters[i] = methodParameter.DefaultValue;
                            }
                            else
                            {
                                // TODO: set type's default value
                            }
                        }
                        var viewInstance = Activator.CreateInstance(viewType) as WebViewPage;
                        using (var stringWriter = new StringWriter())
                        {
                            var viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewPath);
                            var viewContext = new ViewContext(ControllerContext, viewEngineResult.View, ViewData, TempData, stringWriter);
                            viewInstance.ViewContext = viewContext;
                            viewInstance.InitHelpers();
                            methodResult = method.Invoke(viewInstance, invokeParameters);
                            // 如果是 helper 產生的 Html，轉換成 string
                            if (methodResult is HelperResult methodHelperResult)
                            {
                                methodResult = methodHelperResult.ToHtmlString();
                            }
                        }
                        break;
                    }
                }
            }
            return Json(methodResult);
        }
        /// <summary>
        /// 取得所有具有 [FlazorParamterVariable(variable)] 的方法需要哪些參數
        /// </summary>
        [HttpGet]
        public JsonResult VariableChanged(string variable)
        {
            HashSet<string> parameterVariables = new HashSet<string>();
            HashSet<string> parameterSeletors = new HashSet<string>();
            HashSet<string> parameterScripts = new HashSet<string>();
            var viewPathArray = Request.QueryString.GetValues("viewPathArray[]");
            if (viewPathArray != null)
            {
                foreach (var viewPath in viewPathArray.Distinct())
                {
                    var viewType = BuildManager.GetCompiledType(viewPath);
                    if (viewType != null)
                    {
                        var methods = viewType
                            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                            .Concat(viewType.BaseType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                            .Where(x => x.GetParameters().Any(y => y.GetCustomAttribute<FlazorParamterVariableAttribute>()?.Variable == variable));
                        var parameters = methods.SelectMany(x => x.GetParameters());
                        foreach (var parameter in parameters)
                        {
                            if (parameter.GetCustomAttribute<FlazorParamterVariableAttribute>() is FlazorParamterVariableAttribute paramterVariable)
                            {
                                if (!string.IsNullOrEmpty(paramterVariable.Variable))
                                {
                                    parameterVariables.Add(paramterVariable.Variable);
                                }
                            }
                            else if (parameter.GetCustomAttribute<FlazorParamterSeletorAttribute>() is FlazorParamterSeletorAttribute paramterSeletor)
                            {
                                if (!string.IsNullOrEmpty(paramterSeletor.Seletor))
                                {
                                    parameterSeletors.Add(paramterSeletor.Seletor);
                                }
                            }
                            else if (parameter.GetCustomAttribute<FlazorParamterScriptAttribute>() is FlazorParamterScriptAttribute paramterScript)
                            {
                                if (!string.IsNullOrEmpty(paramterScript.Script))
                                {
                                    parameterScripts.Add(paramterScript.Script);
                                }
                            }
                            else
                            {
                                // 預設參數名稱為 js 變數名稱
                                parameterVariables.Add(parameter.Name);
                            }
                        }
                    }
                }
            }
            return Json(new
            {
                Variables = parameterVariables,
                Seletors = parameterSeletors,
                Scripts = parameterScripts
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 執行所有具有 [FlazorParamterVariable(variable)] 的方法
        /// </summary>
        [HttpPost]
        public JsonResult VariableChanged(string[] viewPathArray, string variable,
            Dictionary<string, string> parameterVariables,
            Dictionary<string, string> parameterSeletors,
            Dictionary<string, string> parameterScripts)
        {
            var returnVariables = new Dictionary<string, string>();
            var returnSeletors = new Dictionary<string, string>();
            var returnCallbacks = new Dictionary<string, string>();
            foreach (var viewPath in viewPathArray.Distinct())
            {
                var viewType = BuildManager.GetCompiledType(viewPath);
                if (viewType != null)
                {
                    var methods = viewType
                        .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Concat(viewType.BaseType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                        .Where(x => x.GetParameters().Any(y => y.GetCustomAttribute<FlazorParamterVariableAttribute>()?.Variable == variable));
                    foreach (var method in methods)
                    {
                        var methodParameters = method.GetParameters();
                        var invokeParameters = new object[methodParameters.Length];
                        for (int i = 0; i < methodParameters.Length; i++)
                        {
                            var methodParameter = methodParameters[i];
                            string invokeParameterJson = null;
                            if (methodParameter.GetCustomAttribute<FlazorParamterVariableAttribute>() is FlazorParamterVariableAttribute paramterVariable)
                            {
                                parameterVariables.TryGetValue(paramterVariable.Variable, out invokeParameterJson);
                            }
                            else if (methodParameter.GetCustomAttribute<FlazorParamterSeletorAttribute>() is FlazorParamterSeletorAttribute paramterSeletor)
                            {
                                parameterSeletors.TryGetValue(paramterSeletor.Seletor, out invokeParameterJson);
                            }
                            else if (methodParameter.GetCustomAttribute<FlazorParamterScriptAttribute>() is FlazorParamterScriptAttribute paramterScript)
                            {
                                parameterScripts.TryGetValue(paramterScript.Script, out invokeParameterJson);
                            }
                            else
                            {
                                // 預設使用參數名稱當作變數名稱
                                parameterVariables.TryGetValue(methodParameter.Name, out invokeParameterJson);
                            }
                            if (!string.IsNullOrEmpty(invokeParameterJson))
                            {
                                invokeParameters[i] = JsonConvert.DeserializeObject(invokeParameterJson, methodParameter.ParameterType);
                            }
                            else if (methodParameter.HasDefaultValue)
                            {
                                invokeParameters[i] = methodParameter.DefaultValue;
                            }
                            else
                            {
                                // TODO: set type's default value
                            }
                        }
                        var viewInstance = Activator.CreateInstance(viewType) as WebViewPage;
                        object methodResult;
                        using (var stringWriter = new StringWriter())
                        {
                            var viewEngineResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewPath);
                            var viewContext = new ViewContext(ControllerContext, viewEngineResult.View, ViewData, TempData, stringWriter);
                            viewInstance.ViewContext = viewContext;
                            viewInstance.InitHelpers();
                            methodResult = method.Invoke(viewInstance, invokeParameters);
                            // 如果是 helper 產生的 Html，轉換成 string
                            if (methodResult is HelperResult methodHelperResult)
                            {
                                methodResult = methodHelperResult.ToHtmlString();
                            }
                        }
                        // Add To Result Arrays
                        FlazorReturnVariableAttribute returnVariable =
                            method.GetCustomAttribute<FlazorReturnVariableAttribute>() ??
                            methodParameters
                                .FirstOrDefault(x => x.IsDefined(typeof(FlazorReturnVariableAttribute)))?
                                .GetCustomAttribute<FlazorReturnVariableAttribute>();
                        if (returnVariable != null)
                        {
                            returnVariables.Add(returnVariable.Variable, JsonConvert.SerializeObject(methodResult));
                            continue;
                        }
                        FlazorReturnSeletorAttribute returnSeletor =
                            method.GetCustomAttribute<FlazorReturnSeletorAttribute>() ??
                            methodParameters
                                .FirstOrDefault(x => x.IsDefined(typeof(FlazorReturnSeletorAttribute)))?
                                .GetCustomAttribute<FlazorReturnSeletorAttribute>();
                        if (returnSeletor != null)
                        {
                            returnSeletors.Add(returnSeletor.Seletor, JsonConvert.SerializeObject(methodResult));
                            continue;
                        }
                        FlazorReturnCallbackAttribute returnCallback =
                            method.GetCustomAttribute<FlazorReturnCallbackAttribute>() ??
                            methodParameters
                                .FirstOrDefault(x => x.IsDefined(typeof(FlazorReturnCallbackAttribute)))?
                                .GetCustomAttribute<FlazorReturnCallbackAttribute>();
                        if (returnCallback != null)
                        {
                            returnCallbacks.Add(returnCallback.Function, JsonConvert.SerializeObject(methodResult));
                            continue;
                        }
                    }
                }
            }
            return Json(new
            {
                ReturnVariables = returnVariables.Select(x => new { x.Key, x.Value }).ToArray(),
                ReturnSeletors = returnSeletors.Select(x => new { x.Key, x.Value }).ToArray(),
                ReturnCallbacks = returnCallbacks.Select(x => new { x.Key, x.Value }).ToArray()
            });
        }
    }
}