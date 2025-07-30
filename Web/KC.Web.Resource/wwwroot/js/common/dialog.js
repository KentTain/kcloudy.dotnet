/*
所有的消息都可是文本和html
window.tipSuccess(null/''/'提示','消息主体'); or window.tipSuccess(null/''/'提示','消息主题',css,function(){});
*/
window.tipSuccess = function (title, msg, closeCallback, callback) {
    if (title == null || title.length == 0)
        title = '成功提示';
    if (layer != undefined && layer != null) {
        var dialog = layer.msg(msg, {
            icon: 1, //可传入的值有：0（感叹）1（对）2（错）3（问号）4（锁）
            title: title,
            content: msg,
            offset: 't',
            time: 3000, //3s后自动关闭
        }, function (dialogRef) {
            if (typeof (closeCallback) == 'function') {
                closeCallback();
            }
        },
        );
    } else if (BootstrapDialog != undefined && BootstrapDialog != null) {
        var dialog = BootstrapDialog.show({
            message: msg,
            title: title,
            cssClass: 'center',
            type: BootstrapDialog.TYPE_SUCCESS,
            closable: true,
            closeByBackdrop: false,
            closeByKeyboard: false,
            onhidden: function (dialogRef) {
                if (typeof (closeCallback) == 'function') {
                    closeCallback();
                }
            }
        });
        setTimeout(function () {
            dialog.close();
        }, 3000);


    }

    if (typeof (callback) == 'function') {
        callback(dialog);
    }
}

/*
window.tipInfo(null/''/'提示','消息主题',function(dialog){dialog.close();});
*/
window.tipInfo = function (title, msg, closeCallback, callback) {
    if (title == null || title.length == 0)
        title = '信息提示';
    if (layer != undefined && layer != null) {
        var dialog = layer.msg(msg, {
            icon: 0, //可传入的值有：0（感叹）1（对）2（错）3（问号）4（锁）
            title: title,
            content: msg,
            offset: 't',
            time: 3000, //3s后自动关闭
        }, function (dialogRef) {
            if (typeof (closeCallback) == 'function') {
                closeCallback();
            }
        },
        );
    } else if (BootstrapDialog != undefined && BootstrapDialog != null) {
        var dialog = BootstrapDialog.show({
            message: msg,
            title: title,
            cssClass: 'center',
            type: BootstrapDialog.TYPE_PRIMARY,
            closable: true,
            closeByBackdrop: false,
            closeByKeyboard: false,
            onhidden: function (dialogRef) {
                if (typeof (closeCallback) == 'function') {
                    closeCallback();
                }
            }
        });


    }

    if (typeof (callback) == 'function') {
        callback(dialog);
    }

}

/*
 window.tipWarning(null/''/'提示','消息主体'); or window.tipWarning(null/''/'提示','消息主题',function(){});
 */
window.tipWarning = function (title, msg, closeCallback, callback) {
    if (title == null || title.length == 0)
        title = '警告';

    if (layer != undefined && layer != null) {
        var dialog = layer.msg(msg, {
            icon: 0, //可传入的值有：0（感叹）1（对）2（错）3（问号）4（锁）
            title: title,
            content: msg,
            offset: 't',
            time: 3000, //3s后自动关闭
        }, function (dialogRef) {
            if (typeof (closeCallback) == 'function') {
                closeCallback();
            }
        },
        );
    } else if (BootstrapDialog != undefined && BootstrapDialog != null) {
        var dialog = BootstrapDialog.show({
            message: msg,
            title: title,
            cssClass: 'center',
            closable: true,
            closeByBackdrop: false,
            closeByKeyboard: false,
            type: BootstrapDialog.TYPE_WARNING,
            onhidden: function (dialogRef) {
                if (typeof (closeCallback) == 'function') {
                    closeCallback();
                }
            }
        });

        setTimeout(function () {
            dialog.close();
        }, 3000);


    }

    if (typeof (callback) == 'function') {
        callback();
    }
}

/*
 /js/webuploader(null/''/'提示','消息主体'); or window.tipError(null/''/'提示','消息主题',function(){});
 */
window.tipError = function (title, msg, closeCallback, callback) {
    if (title == null || title.length == 0)
        title = '错误提示';
    if (layer != undefined && layer != null) {
        var dialog = layer.msg(msg, {
            icon: 2, //可传入的值有：0（感叹）1（对）2（错）3（问号）4（锁）
            title: title,
            content: msg,
            offset: 't',
            time: 3000, //3s后自动关闭
        }, function (dialogRef) {
            if (typeof (closeCallback) == 'function') {
                closeCallback();
            }
        },
        );
    } else if (BootstrapDialog != undefined && BootstrapDialog != null) {
        var dialog = BootstrapDialog.show({
            message: msg,
            title: title,
            cssClass: 'center',
            closable: true,
            closeByBackdrop: false,
            closeByKeyboard: false,
            type: BootstrapDialog.TYPE_DANGER,
            onhidden: function (dialogRef) {
                if (typeof (closeCallback) == 'function') {
                    closeCallback();
                }
            }
        });
        setTimeout(function () {
            dialog.close();
        }, 3000);

    }

    if (typeof (callback) == 'function') {
        callback();
    }
}


/*
window.tipLoading(null/''/'提示','消息主题',function(dialog){dialog.close();});
*/
window.tipLoading = function (title, msg, closeCallback) {
    if (title == null || title.length == 0)
        title = '处理中...';

    if (layer != undefined && layer != null) {
        //边缘弹出
        layer.load(1, {
            icon: 1,
            title: title,
            content: msg
        },
            function (index) {
                if (typeof (callback) == 'function') {
                    callback();
                }
                layer.close(index);
            }
        );
    } else if (BootstrapDialog != undefined && BootstrapDialog != null) {
        var dialog = BootstrapDialog.show({
            message: msg,
            title: title,
            cssClass: 'center loadinggif',
            type: BootstrapDialog.TYPE_PRIMARY,
            closable: false,
            onhidden: function (dialogRef) {
                if (typeof (closeCallback) == 'function') {
                    closeCallback();
                }
            }
        });
    }
}


/*
window.tipConfirm(null/''/'提示','消息主题',function(result){if(result){}else{}});
*/
window.tipConfirm = function (title, msg, callback) {
    if (title == null || title.length == 0)
        title = '确认？';
    if (layer != undefined && layer != null) {
        //边缘弹出
        layer.confirm(msg, {
            icon: 1,
            title: title,
            content: msg,
            offset: 't',
        },
            function (index) {
                if (typeof (callback) == 'function') {
                    callback();
                }
                layer.close(index);
            }
        );
    } else if (BootstrapDialog != undefined && BootstrapDialog != null) {
        new BootstrapDialog({
            title: title,
            message: msg,
            closable: false,
            data: {
                'callback': callback
            },
            buttons: [{
                label: ' 取 消 ',
                action: function (dialog) {
                    typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(false);
                    dialog.close();
                }
            }, {
                label: ' 确 定 ',
                cssClass: 'btn-primary',
                action: function (dialog) {
                    typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(true);
                    dialog.close();
                }
            }]
        }).open();
    }
};

/*
window.remote(null/''/'提示',url,css,function(dialog){dialog.close();});
*/
window.remote = function (title, url, css, callback) {
    if (title == null || title.length == 0)
        title = '远程页面';
    if (css != null && css.length > 0)
        css = css + ' center';
    else
        css = 'center';
    var dialog = BootstrapDialog.show({
        message: $('<div></div>').load(url),
        title: title,
        closable: true,
        closeByBackdrop: false,
        closeByKeyboard: false,
        cssClass: css
        //closable: true,
        //type: BootstrapDialog.TYPE_DANGER
    });
    if (typeof (callback) == 'function') {
        callback(dialog);
    }
}