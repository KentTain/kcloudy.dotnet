/*
所有的消息都可是文本和html
window.tipSuccess(null/''/'提示','消息主体'，css); or window.tipSuccess(null/''/'提示','消息主题',css,function(){});
*/
window.tipSuccess = function (title, msg, css, callback, closeCallback) {
    if (title == null || title.length == 0)
        title = '成功提示';
    if (css != null && css.length > 0)
        css = css + ' center';
    else
        css = 'center';
    var dialog = BootstrapDialog.show({
        message: msg,
        title: title,
        cssClass: css,
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
    if (typeof (callback) == 'function') {
        callback();
    }
}

/*
window.tipInfo(null/''/'提示','消息主题',css,function(dialog){dialog.close();});
*/
window.tipInfo = function (title, msg, css, callback, closeCallback) {
    if (title == null || title.length == 0)
        title = '信息提示';
    if (css != null && css.length > 0)
        css = css + ' center';
    else
        css = 'center';
    var dialog = BootstrapDialog.show({
        message: msg,
        title: title,
        cssClass: css,
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
    if (typeof (callback) == 'function') {
        callback(dialog);
    }
}

/*
 window.tipWarning(null/''/'提示','消息主体'，css); or window.tipWarning(null/''/'提示','消息主题',css,function(){});
 */
window.tipWarning = function (title, msg, css, callback, closeCallback) {
    if (title == null || title.length == 0)
        title = '警告';
    if (css != null && css.length > 0)
        css = css + ' center';
    else
        css = 'center';
    var dialog = BootstrapDialog.show({
        message: msg,
        title: title,
        cssClass: css,
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
    if (typeof (callback) == 'function') {
        callback();
    }
}

/*
window.tipLoading(null/''/'提示','消息主题',css,function(dialog){dialog.close();});
*/
window.tipLoading = function (title, msg, css, callback, closeCallback) {
    if (title == null || title.length == 0)
        title = '处理中...';
    if (css != null && css.length > 0)
        css = css + ' center loadinggif';
    else
        css = 'center loadinggif';
    var dialog = BootstrapDialog.show({
        message: msg,
        title: title,
        cssClass: css,
        type: BootstrapDialog.TYPE_PRIMARY,
        closable: false,
        onhidden: function (dialogRef) {
            if (typeof (closeCallback) == 'function') {
                closeCallback();
            }
        }
    });
    //dialog.getModalHeader().hide();
    if (typeof (callback) == 'function') {
        callback(dialog);
    }
}

/*
 /js/webuploader(null/''/'提示','消息主体'，css); or window.tipError(null/''/'提示','消息主题',css,function(){});
 */
window.tipError = function (title, msg, css, callback, closeCallback) {
    if (title == null || title.length == 0)
        title = '错误提示';
    if (css != null && css.length > 0)
        css = css + ' center';
    else
        css = 'center';
    var dialog = BootstrapDialog.show({
        message: msg,
        title: title,
        cssClass: css,
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
    if (typeof (callback) == 'function') {
        callback();
    }
}

/*
window.tipConfirm(null/''/'提示','消息主题',function(result){if(result){}else{}});
*/
window.tipConfirm = function (title, msg, callback) {
    if (title == null || title.length == 0)
        title = '确认？';
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