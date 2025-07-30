//document.domain = topDomain;
function isJSON(str) {
    if (typeof str == 'string') {
        try {
            var obj=JSON.parse(str);
            return true;
        } catch(e) {
            return false;
        }
    }
    return false;
}

function parseResponse(response) {
    var status = 0;
    var message = '';
    var rpStatus = response.status != undefined ? response.status : response.Status;
    switch (rpStatus) {
        case (500):
            //TODO 服务器系统内部错误
            status = 500;
            break;
        case (401):
            //TODO 未登录
            if (isJSON(response.responseText)) {
                var responseText = JSON.parse(response.responseText);
                message = responseText.Message;
            } else {
                message = '登录超时，请重新登录！';
            }
            status = 401;
            break;
        case (403):
            //TODO 无权限执行此操作
            if (isJSON(response.responseText)) {
                var responseText = JSON.parse(response.responseText);
                message = responseText.Message;
            } else {
               if (response.Message != undefined && response.Message != null) {
                   message = response.Message;
               } else {
                   message = '无权限操作！';
               }
            }
            status = 403;
            break;
        case (408):
            //TODO 请求超时
            status = 408;
            break;
        case (0):
            //TODO cancel
            break;
        case (200):
            return;
        default:
            status = 1;
            //TODO 未知错误
    }

    if (status > 0) {
        $.messager.alert(message.length ? message : response.statusText + '请联系网站管理员，错误代码：' + response.status,'error',
            function() {
                if (status === 401) {
                    location.href = ssoDomain + '/Account/Sigin';
                }
            });
    }
}

//$(document).ajaxError(function (event, jqXhr) {
//    parseResponse(jqXhr);
//    $('.datagrid-mask').remove();
//    $('.datagrid-mask-msg').remove();
//});

function setHeight() {
    var c = $('.child-layout');
    if (c.length) {
        var p = c.layout('panel', 'center');
        if (p.length) {
            var oldHeight = p.panel('panel').outerHeight();
            p.panel('resize', { height: 'auto' });
            var newHeight = p.panel('panel').outerHeight();
            c.layout('resize', {
                height: (c.height() + newHeight - oldHeight)
            });
        }
    }
}

function setWidth() {
    var c = $('.child-layout');
    if (c.length) {
        c.layout('resize', {
            width: $('.child-body').width()
        });
    }
}

function NoPermissionTip(parent) {
    if (parent == null) {
        parent = "body";
    }
    $(".NoAuth", parent).unbind('click').attr("href", "javascript:void(0)").removeAttr("onclick").addClass('l-btn-disabled').tooltip({
        content: '<span style="color:#fff">您无该操作的权限</span>',
        onShow: function () {
            $(this).tooltip('tip').css({
                backgroundColor: '#c0c0c0',
                borderColor: '#444'
            });
        }
    });
}

$(function () {
    NoPermissionTip();
    $(window).resize(function () {
        setWidth();
        setHeight();
    });
})