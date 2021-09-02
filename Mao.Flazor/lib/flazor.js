var Flazor = (function () {
    var viewPathArray = [];
    var bindVariableUrl = "";
    var variableChangedUrl = "";
    function addViewPath(viewPath) {
        viewPathArray.push(viewPath);
    }
    function setBindVariableUrl(url) {
        bindVariableUrl = url;
    }
    function setVariableChangedUrl(url) {
        variableChangedUrl = url;
    }
    /** 從 View 的 functions/helpers 取得對應變數的結果 */
    function bindVariable(variable) {
        function step1() {
            $.ajax({
                type: "GET",
                async: true,
                cache: false,
                url: bindVariableUrl,
                data: {
                    viewPathArray: viewPathArray,
                    variable: variable
                },
                dataType: "JSON",
            }).done(function (response) {
                var parameterVariables = [{ key: "", value: "" }];
                var parameterSeletors = [{ key: "", value: "" }];
                var parameterScripts = [{ key: "", value: "" }];
                response.Variables.forEach(function (x) {
                    parameterVariables.push({ key: x, value: JSON.stringify(window[x]) });
                });
                response.Seletors.forEach(function (x) {
                    parameterSeletors.push({ key: x, value: JSON.stringify($(x).val()) });
                });
                response.Scripts.forEach(function (x) {
                    parameterScripts.push({ key: x, value: JSON.stringify(eval(x)) });
                });
                step2(parameterVariables, parameterSeletors, parameterScripts);
            }).fail(function (xhr, textStatus, errorThrown) {
            });
        }
        function step2(parameterVariables, parameterSeletors, parameterScripts) {
            $.ajax({
                type: "POST",
                async: true,
                cache: false,
                url: bindVariableUrl,
                data: {
                    viewPathArray: viewPathArray,
                    variable: variable,
                    parameterVariables,
                    parameterSeletors,
                    parameterScripts
                },
                dataType: "JSON",
            }).done(function (response) {
                window[variable] = response;
                variableChanged(variable);
            }).fail(function (xhr, textStatus, errorThrown) {
            });
        }
        step1();
    }
    /** 將變數的值傳遞至 View 的 functions/helpers 來更新所有對應的結果 */
    function variableChanged(variable) {
        function step1() {
            $.ajax({
                type: "GET",
                async: true,
                cache: false,
                url: variableChangedUrl,
                data: {
                    viewPathArray: viewPathArray,
                    variable: variable
                },
                dataType: "JSON",
            }).done(function (methodParameter) {
                var parameterVariables = [{ key: "", value: "" }];
                var parameterSeletors = [{ key: "", value: "" }];
                var parameterScripts = [{ key: "", value: "" }];
                methodParameter.Variables.forEach(function (x) {
                    parameterVariables.push({ key: x, value: JSON.stringify(window[x]) });
                });
                methodParameter.Seletors.forEach(function (x) {
                    parameterSeletors.push({ key: x, value: JSON.stringify($(x).val()) });
                });
                methodParameter.Scripts.forEach(function (x) {
                    parameterScripts.push({ key: x, value: JSON.stringify(eval(x)) });
                });
                step2(parameterVariables, parameterSeletors, parameterScripts);
            }).fail(function (xhr, textStatus, errorThrown) {
            });
        }
        function step2(parameterVariables, parameterSeletors, parameterScripts) {
            $.ajax({
                type: "POST",
                async: true,
                cache: false,
                url: variableChangedUrl,
                data: {
                    viewPathArray: viewPathArray,
                    variable: variable,
                    parameterVariables,
                    parameterSeletors,
                    parameterScripts
                },
                dataType: "JSON",
            }).done(function (response) {
                response.ReturnVariables.forEach(function (x) {
                    window[x.Key] = JSON.parse(x.Value);
                    variableChanged(x.Key);
                });
                response.ReturnSeletors.forEach(function (x) {
                    $(x.Key).html(JSON.parse(x.Value));
                });
                response.ReturnCallbacks.forEach(function (x) {
                    window[x.Key](JSON.parse(x.Value));
                });
            }).fail(function (xhr, textStatus, errorThrown) {
            });
        }
        step1();
    }
    return {
        addViewPath: addViewPath,
        setBindVariableUrl: setBindVariableUrl,
        setVariableChangedUrl: setVariableChangedUrl,
        bindVariable: bindVariable,
        variableChanged: variableChanged
    };
})();