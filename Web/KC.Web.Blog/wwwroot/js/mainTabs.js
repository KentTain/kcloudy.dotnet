// opr: showLoadingMessage、showInfoMessage、showErrorMessage
// message: 消息内容
// code：错误消息的Code
function MainPage_ShowMessage(opr, message, code) {
    if (opr == "showLoadingMessage") {
        message = '{"command": "showLoadingMessage", "data": {"code": "", "message" : "' + message + '"}}';
        window.top.postMessage(message, '*');
        return;
    }

    if (message == undefined || message == null || message == '')
        return;
    if (code == undefined || code == null || code == '')
        code = '';

    switch (opr) {
        case "showInfoMessage":
            message = '{"command": "showInfoMessage", "data": {"code": "' + code + '", "message" : "' + message + '"}}';
            break;
        case "showErrorMessage":
            message = '{"command": "showErrorMessage", "data": {"code": "' + code + '", "message" : "' + message + '"}}';
            break;
    }

    window.top.postMessage(message, '*');
}

// opr: openPage、openSubPage、closePage、closeSubPage、getPage、refreshPage
// url: /AreaName/ControllerName/ActionName
// queryParms：id=xxxxx
// refresh：true/false
function MainPage_PostMessage(opr, url, queryParms, refresh) {
    if (queryParms == undefined || queryParms == null || queryParms == '')
        queryParms = '';
    if (refresh == undefined || refresh == null)
        refresh = true;

    $.ajax({
        type: "http",
        dataType: "json",
        url: "/Home/GetMenuIdByUrl?url=" + url,
        success: function (data) {
            if (data.success) {
                if (data.result) {
                    var message;
                    switch (opr) {
                        case "openSubPage":
                            message = '{"command":"openSubPage","data":{"page_id":' + data.result + ', "parame":"' + queryParms + '","refresh":' + refresh + '}}';
                            break;
                        case "openPage":
                            message = '{"command":"openPage","data":{"page_id":' + data.result + ', "parame":"' + queryParms + '","refresh":' + refresh + '}}';
                            break;
                        case "closePage":
                            message = '{"command":"closePage","data":{"page_id":' + data.result + '}}';
                            break;
                        case "closeSubPage":
                            message = '{"command":"closeSubPage","data":{"page_id":' + data.result + '}}';
                            break;
                        case "refreshPage":
                            message = '{"command":"refreshPage","data":{"page_id":' + data.result + '}}';
                            break;
                        case "getPage":
                            message = '{"command":"getPage","data":{"page_id":' + data.result + '}}';
                            break;
                        default:
                            var errMsg = '不支持的操作方式';
                            $.messager.showErrorTopCenter('系统提示', errMsg, 1000);
                            return;
                            break;
                    }

                    console.log(opr + ": " + message);
                    window.top.postMessage(message, '*');
                    return data.result;
                } else {
                    var errMsg = '未解析用户操作（' + opr + '）所属URL{' + url + '}对应的招行菜单所属Key';
                    $.messager.showErrorTopCenter('系统提示', errMsg, 1000);
                    return '';
                    //return '{"command": "' + opr + '", "data": "", "errcode": "CFW0007", "errmsg": "' + errMsg+'" }';
                }
            } else {
                var errMsg = '系统内部错误：执行数据库操作出错';
                $.messager.showErrorTopCenter('系统提示', errMsg, 1000);
                return '';
                //return '{"command": "' + opr + '", "data": "", "errcode": "CFW0008", "errmsg": "' + errMsg + '" }';
            }
        },
        complete: function () {
            $.easyui.loaded();
        }
    });
}