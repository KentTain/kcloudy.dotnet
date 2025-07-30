/**
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact author: info@jeasyui.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 * jQuery EasyUI 通用插件基础库
 * jeasyui.extension.common
 * 依赖项：jquery.extension.core.js
 */
(function ($, undefined) {

    $.util.namespace("$.easyui");


    $.easyui.getTopEasyuiMessager = function () {
        if ($.util.isUtilTop) { return $.messager; }
        return $.util.$ && $.util.$.messager ? $.util.$.messager : $.messager;
    };
    $.easyui.messager = $.easyui.getTopEasyuiMessager();


    $.easyui.getTopEasyuiTooltip = function () {
        if ($.util.isUtilTop) { return $.fn.tooltip; }
        return $.util.$ && $.util.$.fn && $.util.$.fn.tooltip ? $.util.$.fn.tooltip : $.fn.tooltip;
    };
    $.easyui.tooltip = $.fn.tooltip;

    //  对某个元素设置 easyui-tooltip 属性；该函数定义如下参数：
    //      target:     表示要设置 easyui-tooltip 的元素，可以是一个 jQuery 选择器字符串，也可以是一个 DOM 对象或者 jQuery 对象。
    //      options:    表示初始化 easyui-tooltip 的参数信息，为一个 JSON-Object；
    //  备注：通过该方法设置的 easyui-tooltip 属性，在触发 mouseover 事件时，加载 easyui-tooltip，在 tooltip-tip 隐藏时，easyui-tooltip 自动调用 destroy 销毁；
    $.easyui.tooltip.init = function (target, options) {
        var t = $(target);
        t.mouseover(function () {
            t.tooltip($.extend({ trackMouse: true }, options, {
                onHide: function () {
                    if ($.isFunction(options.onHide)) { options.onHide.apply(this, arguments); }
                    t.tooltip("destroy");
                }
            })).tooltip("show");
        });
    };

    var icons = { "error": "messager-error", "info": "messager-info", "question": "messager-question", "warning": "messager-warning" },
        _show = $.messager.show, _alert = $.messager.alert, _confirm = $.messager.confirm, _prompt = $.messager.prompt,
        defaults = { title: "操作提醒", confirm: "您确认要进行该操作？", prompt: "请输入相应内容：", icon: "info", loading: "正在加载，请稍等..." },
        iconCls = { info: 'icon-standard-information', error: 'icon-standard-exclamation' };

    //  备注： $.messager 表示当前页面的 easyui-messager 对象；
    //   $.easyui.messager 表示可控顶级页面的 easyui-messager 对象；
    $.messager.showInfoTopCenter = function (title, msg, timeout) {
        return _show({ title: title || "操作提醒", msg: msg, timeout: timeout || 2000, showType: "slide", style: { width: 360, right: '', top: document.body.scrollTop + document.documentElement.scrollTop, bottom: '' }, iconCls: iconCls.info });
    };

    $.messager.showInfoTopRight = function (title, msg, timeout) {
        return _show({ title: title || "操作提醒", msg: msg, timeout: timeout || 2000, showType: "show", style: { width: 360, left: '', right: 0, top: document.body.scrollTop + document.documentElement.scrollTop, bottom: '' }, iconCls: iconCls.info });
    };

    $.messager.showInfoCenter = function (title, msg, timeout) {
        return _show({ title: title || "操作提醒", msg: msg, timeout: timeout || 2000, showType: "fade", style: { width: 360, right: '', bottom: '' }, iconCls: iconCls.info });
    };

    $.messager.showInfoBottomRight = function (title, msg, timeout) {
        return _show({ title: title || "操作提醒", msg: msg, timeout: timeout || 2000, showType: "show", style: { width: 360, }, iconCls: iconCls.info });
    };

    $.messager.showErrorTopCenter = function (title, msg, timeout) {
        return _show({ title: title || "失败提醒", msg: msg, timeout: timeout || 2000, showType: "slide", style: { width: 360, right: '', top: document.body.scrollTop + document.documentElement.scrollTop, bottom: '' }, iconCls: iconCls.error, topMost: true });
    };

    $.messager.showErrorTopRight = function (title, msg, timeout) {
        return _show({ title: title || "失败提醒", msg: msg, timeout: timeout || 2000, showType: "show", style: { width: 360, left: '', right: 0, top: document.body.scrollTop + document.documentElement.scrollTop, bottom: '' }, iconCls: iconCls.error });
    };

    $.messager.showErrorCenter = function (title, msg, timeout) {
        return _show({ title: title || "失败提醒", msg: msg, timeout: timeout || 2000, showType: "fade", style: { width: 360, right: '', bottom: '' }, iconCls: iconCls.error });
    };

    $.messager.showErrorBottomRight = function (title, msg, timeout) {
        return _show({ title: title || "失败提醒", msg: msg, timeout: timeout || 2000, showType: "show", style: { width: 360 }, iconCls: iconCls.error });
    };

    //  重写 $.messager.alert 方法，使其支持如下的多种重载方式：
    //      function (message)
    //      function (message, callback)
    //      function (title, message, callback)
    //      function (title, message, icon)
    //      function (title, message, icon, callback)
    //  返回值：返回弹出的消息框 easyui-window 对象
    $.messager.alert = function (title, msg, icon, fn) {
        if (arguments.length == 1) { return _alert(defaults.title, arguments[0], defaults.icon); }
        if (arguments.length == 2) {
            if ($.isFunction(arguments[1])) { return _alert(defaults.title, arguments[0], defaults.icon, arguments[1]); }
            if (arguments[1] in icons) { return _alert(defaults.title, arguments[0], arguments[1]); }
            return _alert.apply(this, arguments);
        }
        if (arguments.length == 3) {
            if ($.isFunction(arguments[2])) {
                return (arguments[1] in icons) ? _alert(defaults.title, arguments[0], arguments[1], arguments[2])
                    : _alert(arguments[0], arguments[1], defaults.icon, arguments[2]);
            }
            return _alert.apply(this, arguments);
        }
        return _alert.apply(this, arguments);
    };

    //  重写 $.messager.confirm 方法，使其支持如下的多种重载方式：
    //      function (message)
    //      function (callback)
    //      function (message, callback)
    //      function (title, message)
    //  返回值：返回弹出的消息框 easyui-window 对象
    $.messager.confirm = function (title, msg, fn) {
        if (arguments.length == 1) {
            return $.isFunction(arguments[0]) ? _confirm(defaults.title, defaults.confirm, arguments[0]) : _confirm(defaults.title, arguments[0]);
        }
        if (arguments.length == 2) {
            return $.isFunction(arguments[1]) ? _confirm(defaults.title, arguments[0], arguments[1]) : _confirm(arguments[0], arguments[1]);
        }
        return _confirm.apply(this, arguments);
    };

    //  增加 $.messager.solicit 方法，该方法弹出一个包含三个按钮("是"、"否" 和 "取消")的对话框，点击任意按钮或者关闭对话框时，执行指定的回调函数；
    //      该函数提供如下重载方式：
    //      function (param: object)
    //      function (callback: function)
    //      function (message: string, callback: function)
    //      function (title: string, message: string, callback: function)
    //  返回值：返回弹出的消息框 easyui-window 对象
    $.messager.solicit = function (title, msg, fn) {
        var args = arguments, type = $.type(args[0]),
            opts = $.extend({}, $.messager.solicit.defaults,
                type == "object" ? args[0] : (
                    type == "function" ? { callback: args[0] } : (
                        args.length == 2 ? { message: args[0], callback: args[1] } : { title: args[0], message: args[1], callback: args[2] }
                    )
                )
            ),
            ret = $.messager.confirm(opts.title, opts.message, opts.callback),
            options = ret.window("options"), onClose = options.onClose;
        options.onClose = function () {
            if ($.isFunction(onClose)) { onClose.apply(this, arguments); }
            if ($.isFunction(opts.callback)) { opts.callback.call(this, undefined); }
        };
        var buttons = ret.find(">div.messager-button").empty();
        $("<a class=\"messager-solicit messager-solicit-yes\"></a>").appendTo(buttons).linkbutton({
            text: opts.yesText,
            onClick: function () {
                options.onClose = onClose; ret.window("close");
                if ($.isFunction(opts.callback)) { opts.callback.call(this, true); }
            }
        });
        $("<a class=\"messager-solicit messager-solicit-no\"></a>").appendTo(buttons).linkbutton({
            text: opts.noText,
            onClick: function () {
                options.onClose = onClose; ret.window("close");
                if ($.isFunction(opts.callback)) { opts.callback.call(this, false); }
            }
        });
        $("<a class=\"messager-solicit messager-solicit-cancel\"></a>").appendTo(buttons).linkbutton({
            text: opts.cancelText,
            onClick: function () {
                options.onClose = onClose; ret.window("close");
                if ($.isFunction(opts.callback)) { opts.callback.call(this, undefined); }
            }
        });
        return ret;
    };

    $.messager.solicit.defaults = { title: "操作提醒", message: null, callback: null, yesText: "是", noText: "否", cancelText: "取消" };


    //  重写 $.messager.prompt 方法，使其支持如下的多种重载方式：
    //      function (callback)
    //      function (message, callback)
    //      function (title, message)
    //      function (title, message, callback)
    //  返回值：返回弹出的消息框 easyui-window 对象
    $.messager.prompt = function (title, msg, fn) {
        if (arguments.length == 1) {
            return $.isFunction(arguments[0]) ? _prompt(defaults.title, defaults.prompt, arguments[0]) : _prompt(defaults.title, defaults.prompt);
        }
        if (arguments.length == 2) {
            return $.isFunction(arguments[1]) ? _prompt(defaults.title, arguments[0], arguments[1]) : _prompt(arguments[0], arguments[1]);
        }
        return _prompt.apply(this, arguments);
    };


    //  显示类似于 easyui-datagrid 在加载远程数据时显示的 mask 状态层；该函数定义如下重载方式：
    //      function ()
    //      function (options)，其中 options 为一个格式为 { msg, locale, topMost } 的 JSON-Object；
    //  上述参数中：
    //      msg 表示加载显示的消息文本内容，默认为 "正在加载，请稍等..."；
    //      locale 表示加载的区域，可以是一个 jQuery 对象选择器字符串，也可以是一个 jQuery 对象或者 HTML-DOM 对象；默认为字符串 "body"。
    //      topMost 为一个布尔类型参数，默认为 false，表示是否在顶级页面加载此 mask 状态层。
    //  返回值：返回表示弹出的数据加载框和层的 jQuery 对象。
    $.easyui.loading = function (options) {
        var opts = $.extend({ msg: defaults.loading, locale: "body", topMost: false }, options || {}),
            jq = opts.topMost ? $.util.$ : $,
            locale = jq(opts.locale),
            array = locale.children().map(function () {
                var zindex = $(this).css("z-index");
                return $.isNumeric(zindex) ? parseInt(zindex) : 0;
            }),
            zindex = $.array.max(array.length ? array : [1]);
        if (!locale.is("body")) {
            locale.addClass("mask-container");
        }
        var mask = jq("<div></div>").addClass("datagrid-mask").css({ display: "block", "z-index": zindex += 1000 }).appendTo(locale);
        var msg = jq("<div></div>").addClass("datagrid-mask-msg").css({ display: "block", left: "50%", "z-index": ++zindex }).html(opts.msg).appendTo(locale);
        msg.css("marginLeft", -msg.outerWidth() / 2);
        return mask.add(msg);
    };

    //  关闭由 $.easyui.loading 方法显示的 "正在加载..." 状态层；该函数定义如下重载方式：
    //      function ()
    //      function (locale)
    //      function (locale, topMost)
    //      function (topMost, locale)
    //      function (options)，其中 options 为一个格式为 { locale, topMost } 的 JSON-Object
    $.easyui.loaded = function (locale, topMost) {
        var opts = { locale: "body", topMost: false };
        if (arguments.length == 1) {
            if ($.isPlainObject(arguments[0])) {
                $.extend(opts, arguments[0]);
            } else if ($.util.isBoolean(arguments[0])) {
                opts.topMost = arguments[0];
            } else {
                opts.locale = arguments[0];
            }
        }
        if (arguments.length == 2) {
            if ($.util.isBoolean(arguments[0])) {
                $.extend(opts, { locale: arguments[1], topMost: arguments[0] });
            } else {
                $.extend(opts, { locale: arguments[0], topMost: arguments[1] });
            }
        }
        var jq = opts.topMost ? $.util.$ : $, locale = jq(opts.locale);
        locale.removeClass("mask-container");
        locale.children("div.datagrid-mask-msg,div.datagrid-mask").remove();
    };




    //  更改 jQuery EasyUI 中部分控件的国际化语言显示。
    $.extend($.fn.panel.defaults, { loadingMessage: defaults.loading });
    $.extend($.fn.window.defaults, { loadingMessage: defaults.loading });
    $.extend($.fn.dialog.defaults, { loadingMessage: defaults.loading });

    //  更改 jeasyui-combo 组件的非空验证提醒消息语言。
    $.extend($.fn.combo.defaults, { missingMessage: $.fn.validatebox.defaults.missingMessage });



    //  基于当前页面 document 触发，当前页面嵌套的所有子级和父级页面均执行一个签名为 function (win, e) 事件触发函数；该方法提供如下参数：
    //      eventName:
    //      eventNamespace:
    //      plugin:
    //      callback: 一个签名为 function (win, e) 的函数，其中 win 表示所在 iframe 执行函数传入的 window 对象，e 表示最初触发该循环函数调用的事件对象。
    $.easyui.bindPageNestedFunc = function (eventName, eventNamespace, plugin, callback) {
        if (arguments.length == 3) { callback = plugin; plugin = "jquery"; }
        if (arguments.length == 4 && !plugin) { plugin = "jquery"; }
        $(document).unbind("." + eventNamespace).bind(eventName + "." + eventNamespace, function (e) {
            var doCall = function (win) { callback.call(win, win, e); },
                doCallUp = function (win) {
                    var p = win.parent;
                    try {
                        if (win != p && p.jQuery && p.jQuery.parser && p.jQuery.parser.plugins && p.jQuery.fn && p.jQuery.fn[plugin]) {
                            doCall(p);
                            doCallUp(p);
                        }
                    } catch (ex) { }
                },
                doCallDown = function (win) {
                    var jq = win.jQuery;
                    jq("iframe,iframe").each(function () {
                        try {
                            if (this.contentWindow && jq.util.isObject(this.contentWindow.document) && this.contentWindow.jQuery && this.contentWindow.jQuery.parser && this.contentWindow.jQuery.parser.plugins && this.contentWindow.jQuery.fn && this.contentWindow.jQuery.fn[plugin]) {
                                doCall(this.contentWindow);
                                doCallDown(this.contentWindow);
                            }
                        } catch (ex) { }
                    });
                },
                doCallAll = function (win) {
                    doCall(win);
                    doCallUp(win);
                    doCallDown(win);
                };
            doCallAll(window);
        });
    };



    //  获取或更改 jQuery EasyUI 部分组件的通用错误提示函数；该方法定义如下重载方式：
    //      function():         获取 jQuery EasyUI 部分组件的通用错误提示函数；
    //      function(callback): 更改 jQuery EasyUI 部分组件的通用错误提示函数；
    //  备注：该方法会设置如下组件的 onLoadError 事件；
    //          easyui-form
    //          easyui-combobox
    //          easyui-combotree
    //          easyui-combogrid
    //          easyui-datagrid
    //          easyui-propertygrid
    //          easyui-tree
    //          easyui-treegrid
    //      同时还会设置 jQuery-ajax 的通用错误事件 error。
    $.easyui.ajaxError = function (callback) {
        if (!arguments.length) { return $.fn.form.defaults.onLoadError; }
        $.fn.form.defaults.onLoadError = callback;
        $.fn.panel.defaults.onLoadError = callback;
        $.fn.combobox.defaults.onLoadError = callback;
        $.fn.combotree.defaults.onLoadError = callback;
        $.fn.combogrid.defaults.onLoadError = callback;
        $.fn.datagrid.defaults.onLoadError = callback;
        $.fn.propertygrid.defaults.onLoadError = callback;
        $.fn.tree.defaults.onLoadError = callback;
        $.fn.treegrid.defaults.onLoadError = callback;
        $.ajaxSetup({ error: callback });
    };

    var onLoadError = function (XMLHttpRequest, textStatus, errorThrown) {
        $.messager.progress("close");
        if ($.easyui.messager != $.messager) { $.easyui.messager.progress("close"); }

        var message = XMLHttpRequest.responseJSON != null ? XMLHttpRequest.responseJSON.Message : '';

        var status = 0;
        var rpStatus = XMLHttpRequest.status != undefined ? XMLHttpRequest.status : XMLHttpRequest.Status;
        switch (rpStatus) {
            case (500):
                //TODO 服务器系统内部错误
                status = 500;
                break;
            case (401):
                //TODO 未登录
                message = '登录超时，请重新登录！';
                status = 401;
                break;
            case (403):
                //TODO 无权限执行此操作
                message = '无操作权限，请联系管理员进行授权！';
                status = 403;
                break;
            case (408):
                //TODO 请求超时
                status = 408;
                message = '请求超时，请重试！';
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

        var msg = (XMLHttpRequest && (!$.string.isNullOrWhiteSpace(message) || !$.string.isNullOrWhiteSpace(XMLHttpRequest.responseText))
            ? "如果该问题重复出现，请联系您的系统管理员并反馈该故障。<hr />" +
            "错误号：" + XMLHttpRequest.status + "(" + XMLHttpRequest.statusText + ")；<br />" +
            "错误消息：" + (!$.string.isNullOrWhiteSpace(message) ? message : XMLHttpRequest.responseText)
            : "系统出现了一个未指明的错误，如果该问题重复出现，请联系您的系统管理员并反馈该故障。");

        var win = $.easyui.messager.alert("错误提醒", '<div style="height:300px;">' + msg + '</div>', "error", function () {
            if (status === 401) {
                location.href = '/Home/Index';
            }
        });
        var opts = win.window("options"), panel = win.window("panel"), width = panel.outerWidth(), height = panel.outerHeight();
        if (width > 800 || height > 800) {
            win.window("resize", { width: width > 800 ? 800 : width, height: height > 800 ? 800 : height });
        } else {
            win.window("resize", { width: 500, height: 400 });
        }
        win.window("center");
    };

    //  备份 jquery ajax 方法的默认参数。
    $.easyui.ajaxDefaults = $.extend({}, $.ajaxSettings);

    //  更改 jQuery EasyUI 部分组件的通用错误提示。
    $.easyui.ajaxError(onLoadError);

    //  更改 jQuery.ajax 函数的部分默认属性。
    $.ajaxSetup({
        dataFilter: function (data, type) {
            return String(type).toLowerCase(type) == "json" ? $.string.toJSONString(data) : data;
        }
        //,beforeSend: function (XMLHttpRequest) {
        //    $.easyui.loading({ msg: "正在将请求数据发送至服务器..." });
        //}
        //,complete: function (XMLHttpRequest, textStatus) {
        //    $.easyui.loaded();
        //}
    });

    $.extend({

        //  判断当前 jQuery 对象是否是指定名称的已经初始化好的 easyui 插件；该方法定义如下参数：
        //      selector:   jQuery 对象选择器，或者 DOM 对象，或者 jQuery 对象均可；
        //      pluginName：要判断的插件名称，例如 "panel"、"dialog"、"datagrid" 等；
        //  返回值：如果 selector 所表示的 jQuery 对象中的第一个 DOM 元素为 pluginName 参数所示的 easyui 插件且已经被初始化，则返回 true，否则返回 false。
        isEasyUI: function (selector, pluginName) {
            if (!$.array.contains($.parser.plugins, pluginName)) { $.error($.string.format("传入的参数 pluginName: {0} 不是 easyui 插件名。")); }
            var t = $(selector);
            if (!t.length) { return false; }
            var state = $.data(t[0], pluginName);
            return state && state.options ? true : false;
        }
    });


    $.fn.extend({

        //  判断当前 jQuery 对象是否是指定名称的已经初始化好的 easyui 插件；该方法定义如下参数：
        //      pluginName：要判断的插件名称，例如 "panel"、"dialog"、"datagrid" 等；
        //  返回值：如果当前 jQuery 对象中的第一个 DOM 元素为 pluginName 参数所示的 easyui 插件且已经被初始化，则返回 true，否则返回 false。
        isEasyUI: function (pluginName) {
            return $.isEasyUI(this, pluginName);
        },


        currentPagination: function () {
            var p = this.closest(".pagination");
            while (p.length && !$.data(p[0], "pagination")) { p = p.parent().closest(".pagination"); }
            return p;
        },

        currentProgressbar: function () {
            var p = this.closest(".progressbar");
            while (p.length && !$.data(p[0], "progressbar")) { p = p.parent().closest(".progressbar"); }
            return p;
        },

        currentPanel: function () {
            var p = this.closest(".panel-body");
            while (p.length && !$.data(p[0], "panel")) { p = p.parent().closest(".panel-body"); }
            return p;
        },

        currentTabPanel: function () {
            var p = this.closest(".panel-body"), panel = p.parent(), panels = panel.parent(), container = panels.parent();
            while (p.length && !($.data(p[0], "panel") && panel.hasClass("panel") && panels.hasClass("tabs-panels") && container.hasClass("tabs-container"))) {
                p = p.parent().closest(".panel-body");
                panel = p.parent();
                panels = panel.parent();
                container = panels.parent();
            }
            return p;
        },

        currentTabIndex: function () {
            var panel = this.currentTabPanel();
            return panel.length ? panel.panel("panel").index() : -1;
        },

        currentTabs: function () {
            var p = this.closest(".tabs-container");
            while (p.length && !$.data(p[0], "tabs")) { p = p.parent().closest(".tabs-container"); }
            return p;
        },

        currentAccordion: function () {
            var p = this.closest(".accordion");
            while (p.length && !$.data(p[0], "accordion")) { p = p.parent().closest(".accordion"); }
            return p;
        },

        currentAccPanel: function () {
            var p = this.closest(".panel-body"), panel = p.parent(), container = panels.parent();
            while (p.length && !($.data(p[0], "panel") && panel.hasClass("panel") && container.hasClass("accordion") && $.data(container[0], "accordion"))) {
                p = p.parent().closest(".panel-body");
                panel = p.parent();
                container = panels.parent();
            }
            return p;
        },

        currentLayout: function () {
            var layout = this.closest(".layout");
            while (layout.length && !$.data(layout[0], "layout")) { layout = layout.closest(".layout"); }
            return layout;
        },

        currentRegion: function () {
            var p = this.closest(".panel.layout-panel"), layout = p.parent(), body = p.children(".panel-body");
            while (p.length && !(layout.hasClass("layout") && $.data(body[0], "panel"))) {
                p = p.parent().closest(".panel.layout-panel");
                layout = p.parent();
                body = p.children(".panel-body");
            }
            return body;
        },

        currentLinkbutton: function () {
            var btn = this.closest(".l-btn");
            while (btn.length && !$.data(btn[0], "linkbutton")) { btn = btn.parent().closest(".layout"); }
            return btn;
        },

        currentCalendar: function () {
            var c = this.closest(".calendar");
            while (c.length && !$.data(c[0], "calendar")) { c = c.parent().closest(".calendar"); }
            return c;
        },

        currentWindow: function () {
            var p = this.closest(".panel-body.window-body");
            while (p.length && !$.data(p[0], "window")) { p = p.parent().closest(".panel-body.window-body"); }
            return p;
        },

        currentDialog: function () {
            var p = this.closest(".panel-body.window-body");
            while (p.length && !$.data(p[0], "dialog")) { p = p.parent().closest(".panel-body.window-body"); }
            return p;
        },

        currentDatagrid: function () {
            var p = this.closest(".datagrid-wrap.panel-body"), dg = p.find(">.datagrid-view>:eq(2)");
            while (p.length && !$.data(dg[0], "datagrid")) {
                p = p.parent().closest(".datagrid-wrap.panel-body");
                dg = p.find(">.datagrid-view>:eq(2)");
            }
            return dg;
        },

        currentPropertygrid: function () {
            var p = this.closest(".datagrid-wrap.panel-body"), pg = p.find(">.datagrid-view>:eq(2)");
            while (p.length && !$.data(pg[0], "propertygrid")) {
                p = p.parent().closest(".datagrid-wrap.panel-body");
                pg = p.find(">.datagrid-view>:eq(2)");
            }
            return pg;
        },

        currentTree: function () {
            var t = this.closest(".tree");
            while (t.length && !$.data(t[0], "tree")) { t = t.parent().closest(".tree"); }
            return t;
        },

        currentTreegrid: function () {
            var p = this.closest(".datagrid-wrap.panel-body"), tg = p.find(">.datagrid-view>:eq(2)");
            while (p.length && !$.data(tg[0], "treegrid")) {
                p = p.parent().closest(".datagrid-wrap.panel-body");
                tg = p.find(">.datagrid-view>:eq(2)");
            }
            return tg;
        }
    });

})(jQuery);


/**
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact author: info@jeasyui.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 * jQuery EasyUI panel 组件扩展
 * jeasyui.extension.panel
 * 依赖项：
 *   1、jquery.extension.core.js
 *   2、jeasyui.extension.common
 */
(function ($, undefined) {


    $.fn.panel.extensions = {};


    //  easyui-panel、easyui-window、easyui-dialog 卸载时回收内存，主要用于 layout、panel(及其继承组件) 使用 iframe 嵌入网页时的内存泄漏问题
    var onBeforeDestroy = function () {
        $("iframe,frame", this).each(function () {
            try {
                if (this.contentWindow && this.contentWindow.document && this.contentWindow.close) {
                    this.contentWindow.document.write("");
                    this.contentWindow.close();
                }
                if ($.isFunction(window.CollectGarbage)) { window.CollectGarbage(); }
            } catch (ex) { }
        }).remove();
    };
    $.fn.panel.defaults.onBeforeDestroy = onBeforeDestroy;
    $.fn.window.defaults.onBeforeDestroy = onBeforeDestroy;
    $.fn.dialog.defaults.onBeforeDestroy = onBeforeDestroy;
    $.fn.datagrid.defaults.onBeforeDestroy = onBeforeDestroy;
    $.fn.propertygrid.defaults.onBeforeDestroy = onBeforeDestroy;
    $.fn.treegrid.defaults.onBeforeDestroy = onBeforeDestroy;

    var _onResize = {
        panel: $.fn.panel.defaults.onResize,
        window: $.fn.window.defaults.onResize,
        dialog: $.fn.dialog.defaults.onResize
    };
    var onResize = function (width, height) {
        var p = $(this), isWin = p.panel("isWindow"), isDia = p.panel("isDialog"),
            plugin = isDia ? "dialog" : (isWin ? "window" : "panel"),
            _onResizeFn = _onResize[plugin];
        if ($.isFunction(_onResizeFn)) { _onResizeFn.apply(this, arguments); }
        if (!p.panel("inLayout")) {
            var opts = p.panel("options"),
                minWidth = $.isNumeric(opts.minWidth) ? opts.minWidth : defaults.minHeight,
                maxWidth = $.isNumeric(opts.maxWidth) ? opts.maxWidth : defaults.maxWidth,
                minHeight = $.isNumeric(opts.minHeight) ? opts.minHeight : defaults.minHeight,
                maxHeight = $.isNumeric(opts.maxHeight) ? opts.maxHeight : defaults.maxHeight;
            var resizable = false;
            if (width > maxWidth) { width = maxWidth; resizable = true; }
            if (width < minWidth) { width = minWidth; resizable = true; }
            if (height > maxHeight) { height = maxHeight; resizable = true; }
            if (height < minHeight) { height = minHeight; resizable = true; }
            if (resizable && !opts.fit) {
                p[plugin]("resize", { width: width, height: height });
            }
        }
    };

    var _onMove = {
        //fixed the panel location is wrong after to call OnMove method
        //panel: $.fn.panel.defaults.onMove,
        window: $.fn.window.defaults.onMove,
        dialog: $.fn.dialog.defaults.onMove
    };
    var onMove = function (left, top) {
        var p = $(this), isWin = p.panel("isWindow"), isDia = p.panel("isDialog"),
            plugin = isDia ? "dialog" : (isWin ? "window" : "panel"),
            _onMoveFn = _onMove[plugin], opts = p.panel("options");
        //fixed the panel location is wrong after to call OnMove method
        if (plugin == "panel") return;
        if ($.isFunction(_onMoveFn)) { _onMoveFn.apply(this, arguments); }
        if (opts.maximized) { return p[plugin]("restore"); }
        if (!opts.inContainer) { return; }
        var panel = p.panel("panel"), parent = panel.parent(), isRoot = parent.is("body"),
            scope = $.extend({}, isRoot ? $.util.windowSize() : { width: parent.innerWidth(), height: parent.innerHeight() }),
            width = $.isNumeric(opts.width) ? opts.width : panel.outerWidth(),
            height = $.isNumeric(opts.height) ? opts.height : panel.outerHeight(),
            moveable = false;
        if (left < 0) { left = 0; moveable = true; }
        if (top < 0) { top = 0; moveable = true; }
        if (moveable) { return p[plugin]("move", { left: left, top: top }); }
        if (left + width > scope.width && left > 0) { left = scope.width - width; moveable = true; }
        if (top + height > scope.height && top > 0) { top = scope.height - height; moveable = true; }
        if (moveable) { return p[plugin]("move", { left: left, top: top }); }
    };



    var inLayout = function (target) {
        var t = $(target), body = t.panel("body"), panel = t.panel("panel");
        return body.hasClass("layout-body") && panel.hasClass("layout-panel");
    };

    var inTabs = function (target) {
        var t = $(target), panel = t.panel("panel"), panels = panel.parent(), container = panels.parent();
        return panels.hasClass("tabs-panels") && container.hasClass("tabs-container");
    };

    var inAccordion = function (target) {
        var t = $(target), body = t.panel("body"), panel = t.panel("panel"), container = panel.parent();
        return (body.hasClass("accordion-body") && container.hasClass("accordion") && $.data(container[0], "accordion")) ? true : false;
    };

    var isWindow = function (target) {
        var t = $(target), body = t.panel("body");
        return body.hasClass("window-body") && body.parent().hasClass("window");
    };

    var isDialog = function (target) {
        var t = $(target), body = t.panel("body");
        return isWindow(target) && (body.children("div.panel").children("div.panel-body.dialog-content").length ? true : false);
    };


    //将options中的href和content隐藏到扩展属性中之后，清空这两个属性
    function parseExtensionsBegin(opts) {
        opts._extensionsPanel = { href: opts.href, content: opts.content, fromTabs: opts.fromTabs };
        if (opts.iniframe) {
            opts.href = null;
            opts.content = null;
        }
    };
    //将隐藏在扩展属性中的href和content重新拿出来赋值给options，若iniframe则refresh
    function parseExtensionsEnd(target) {
        var panel = $(target), opts = panel.panel("options"),
            exts = opts._extensionsPanel && opts._extensionsPanel.href ? opts._extensionsPanel : opts._extensionsPanel = { href: opts.href, content: opts.content, fromTabs: opts.fromTabs };
        opts.href = exts.href; opts.content = exts.content; opts.fromTabs = exts.fromTabs;
        if (opts.bodyStyle) {
            panel.panel("body").css(opts.bodyStyle);
        }
        if (opts.iniframe) {
            refresh(target);
        }
    };

    var _panel = $.fn.panel;
    $.fn.panel = function (options, param) {
        if (typeof options == "string") {
            return _panel.apply(this, arguments);
        }

        options = options || {};
        return this.each(function () {
            var hasInit = $.data(this, "panel") ? true : false,
                opts = hasInit ? options : $.extend({}, $.fn.panel.parseOptions(this), $.parser.parseOptions(this, [
                    { minWidth: "number", maxWidth: "number", minHeight: "number", maxHeight: "number" },
                    { iniframe: "boolean", inContainer: "boolean", keepTitle: "boolean" }
                ]), options);
            parseExtensionsBegin(opts);
            _panel.call($(this), opts, param);
            parseExtensionsEnd(this);
        });
    };
    $.union($.fn.panel, _panel);





    //对应源码中2665行的_1f5
    //参数1：target
    function loadPanel(target, href) {
        var t = $(target), state = $.data(target, "panel"), opts = state.options;
        if (params) {
            opts.queryParams = href;
        }
        if (opts.href) {
            if (!state.isLoaded || !opts.cache) {
                var params = $.extend({}, opts.queryParams);
                if (opts.onBeforeLoad.call(target, params) == false) {
                    return;
                }
                state.isLoaded = false;
                destroyContent(target);
                if (opts.loadingMessage) {
                    t.html($("<div class=\"panel-loading\"></div>").html(opts.loadingMessage));
                }
                opts.loader.call(target, params, function (data) {
                    loadContent(opts.extractor.call(target, data));
                    opts.onLoad.apply(target, arguments);
                    state.isLoaded = true;
                }, function () {
                    opts.onLoadError.apply(target, arguments);
                });
            }
        } else {
            if (opts.content) {
                if (!state.isLoaded) {
                    destroyContent(target);
                    loadContent(opts.content);
                    state.isLoaded = true;
                }
            }
        }
        function loadContent(content) {
            t.html(content);
            $.parser.parse(t);
        };
    };
    //对应源码中的2704行_1fa
    function destroyContent(target) {
        var t = $(target);
        t.find(".combo-f").each(function () {
            $(this).combo("destroy");
        });
        t.find(".m-btn").each(function () {
            $(this).menubutton("destroy");
        });
        t.find(".s-btn").each(function () {
            $(this).splitbutton("destroy");
        });
        t.find(".tooltip-f").each(function () {
            $(this).tooltip("destroy");
        });
        t.children("div").each(function () {
            $(this)._fit(false);
        });
    };


    function refresh(target, href) {
        var state = $.data(target, "panel"), opts = state.options;
        state.isLoaded = false;
        if (href) {
            if (typeof href == "string") {
                opts.href = href;
            } else {
                opts.queryParams = href;
            }
        }
        if (opts.iniframe) {
            if (opts.fromTabs == true) {
                state.isLoaded = true; return;
            }
            var exts = opts._extensionsPanel ? opts._extensionsPanel : opts._extensionsPanel = { href: opts.href, content: opts.content };
            //exts.href = opts.href; exts.content = opts.content;
            opts.href = null;
            opts.content = "<iframe class=\"panel-iframe\" frameborder=\"0\" width=\"100%\" height=\"100%\" marginwidth=\"0px\" marginheight=\"0px\" scrolling=\"auto\"></iframe>";
            loadPanel(target);
            opts.href = exts.href; opts.content = exts.content;
            getIframe(target).bind({
                load: function () {
                    if ($.isFunction(opts.onLoad)) { opts.onLoad.apply(target, arguments); }
                },
                error: function () {
                    if ($.isFunction(opts.onLoadError)) { opts.onLoadError.apply(target, arguments); }
                }
            }).attr("src", opts.href || "");
        } else {
            loadPanel(target);
        }
    };



    function getIframe(target) {
        var p = $(target), body = p.panel("body");
        return body.children("iframe.panel-iframe");
    };

    var _header = $.fn.panel.methods.header;
    function getHeader(target) {
        var t = $(target);
        if (!inTabs(target)) { return _header.call(t, t); }
        var panel = t.panel("panel"), index = panel.index(), tabs = panel.closest(".tabs-container");
        return tabs.find(">div.tabs-header>div.tabs-wrap>ul.tabs>li").eq(index);
    };

    var _setTitle = $.fn.panel.methods.setTitle;
    function setTitle(target, title) {
        var t = $(target);
        if (!inTabs(target)) { return _setTitle.call(t, t, title); }
        if (!title) { return; }
        var opts = t.panel("options"), header = t.panel("header");
        opts.title = title;
        header.find(">a.tabs-inner>span.tabs-title").text(title);
    };

    var methods = $.fn.panel.extensions.methods = {
        //  判断当前 easyui-panel 是否为 easyui-layout 的 panel 部件；
        //  返回值：如果当前 easyui-panel 是 easyui-layout 的 panel 部件，则返回 true，否则返回 false。
        inLayout: function (jq) { return inLayout(jq[0]); },

        //  判断当前 easyui-panel 是否为 easyui-tabs 的选项卡。
        inTabs: function (jq) { return inTabs(jq[0]); },

        //  判断当前 easyui-panel 是否为 easyui-accordion 中的一个折叠面板。
        inAccordion: function (jq) { return inAccordion(jq[0]); },

        //  判断当前 easyui-panel 是否为 easyui-window 组件；
        isWindow: function (jq) { return isWindow(jq[0]); },

        //  判断当前 easyui-panel 是否为 easyui-dialog 组件；
        isDialog: function (jq) { return isDialog(jq[0]); },

        //  增加 easyui-panel 控件的扩展方法；该方法用于获取当前在 iniframe: true 时当前 panel 控件中的 iframe 容器对象；
        //  备注：如果 inirame: false，则该方法返回一个空的 jQuery 对象。
        iframe: function (jq) { return getIframe(jq[0]); },

        //  重写 easyui-panel 控件的 refresh 方法，用于支持 iniframe 属性。
        refresh: function (jq, href) { return jq.each(function () { refresh(this, href); }); },

        //  重写 easyui-panel 控件的 header 方法，支持位于 easyui-tabs 中的 tab-panel 部件获取 header 对象；
        //  备注：如果该 panel 位于 easyui-tabs 中，则该方法返回 easyui-tabs 的 div.tabs-header div.tabs-wrap ul.tabs 中对应该 tab-panel 的 li 对象。
        header: function (jq) { return getHeader(jq[0]); },

        //  重写 easyui-panel 控件的 setTitle 方法，支持位于 easyui-tabs 中的 tab-panel 部件设置 title 操作；
        //  返回值：返回当前选项卡控件 easyui-panel 的 jQuery 链式对象。
        setTitle: function (jq, title) { return jq.each(function () { setTitle(this, title); }); }
    };
    var defaults = $.fn.panel.extensions.defaults = {

        //  增加 easyui-panel 控件的自定义属性，该属性表示 href 加载的远程页面是否装载在一个 iframe 中。
        iniframe: false,

        //  增加 easyui-panel 控件的自定义属性，表示 easyui-panel 面板的最小宽度。
        minWidth: 10,

        //  增加 easyui-panel 控件的自定义属性，表示 easyui-panel 面板的最大宽度。
        maxWidth: 10000,

        //  增加 easyui-panel 控件的自定义属性，表示 easyui-panel 面板的最小高度。
        minHeight: 10,

        //  增加 easyui-panel 控件的自定义属性，表示 easyui-panel 面板的最大高度。
        maxHeight: 10000,

        //  增加 easyui-panel 控件的自定义属性，重新定义的 onResize 事件。用于扩展四个新增属性 minWidth、maxWidth、minHeight、maxHeight 的功能。
        onResize: onResize,

        //  扩展 easyui-panel、easyui-window 以及 easyui-dialog 控件的自定义属性，表示该窗口是否无法移除父级对象边界，默认为 true。
        inContainer: true,

        //  扩展 easyui-panel、easyui-window 以及 easyui-dialog 控件的自定义属性，表示该面板 body 对象的自定义 CSS 样式；
        //  该属性作用于 panel-body 对象；格式请参照 style 属性；
        bodyStyle: null,

        //  重写 easyui-panel、easyui-window 以及 easyui-dialog 控件的原生事件 onMove，以支持相应扩展功能。
        onMove: onMove
    };

    $.extend($.fn.panel.defaults, defaults);
    $.extend($.fn.panel.methods, methods);

})(jQuery);

/**
 * Licensed under the GPL or commercial licenses
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 * jQuery EasyUI menu 组件扩展
 * jeasyui.extension.menu
 * 依赖项：
 *   1、jquery.extension.core.js
 *   2、jeasyui.extension.common
 */
(function ($, undefined) {

    /**
     * initialize the target menu, the function can be invoked only once
     */
    function init(target) {
        var t = $(target).appendTo('body').addClass('menu-top');

        //$(document).unbind('.menu').bind('mousedown.menu', function (e) {
        //    //			var allMenu = $('body>div.menu:visible');
        //    //			var m = $(e.target).closest('div.menu', allMenu);
        //    var m = $(e.target).closest('div.menu,div.combo-p');
        //    if (m.length) { return }
        //    $('body>div.menu-top:visible').menu('hide');
        //});

        var menus = splitMenu(t);
        for (var i = 0; i < menus.length; i++) {
            createMenu(menus[i]);
        }

        function splitMenu(menu) {
            var menus = [];
            menu.addClass('menu');
            menus.push(menu);
            if (!menu.hasClass('menu-content')) {
                menu.children('div').each(function () {
                    var submenu = $(this).children('div');
                    if (submenu.length) {
                        submenu.insertAfter(target);
                        this.submenu = submenu; 	// point to the sub menu
                        var mm = splitMenu(submenu);
                        menus = menus.concat(mm);
                    }
                });
            }
            return menus;
        }

        function createMenu(menu) {
            var wh = $.parser.parseOptions(menu[0], ['width', 'height']);
            menu[0].originalHeight = wh.height || 0;
            if (menu.hasClass('menu-content')) {
                menu[0].originalWidth = wh.width || menu._outerWidth();
            } else {
                menu[0].originalWidth = wh.width || 0;
                menu.children('div').each(function () {
                    var item = $(this);
                    //var itemOpts = $.extend({}, $.parser.parseOptions(this, ['name', 'iconCls', 'href', { separator: 'boolean' }]), {
                    //    disabled: (item.attr('disabled') ? true : undefined)
                    //});
                    //  注释掉上三行代码，并添加了下三行代码，以实现获取 menu-item 的属性 hideOnClick，该参数表示是否在点击菜单项后菜单自动隐藏
                    var itemOpts = $.extend({ hideOnClick: true }, $.parser.parseOptions(this, ['name', 'iconCls', 'href', { hideOnClick: 'boolean', separator: 'boolean' }]), {
                        disabled: (item.attr('disabled') ? true : undefined)
                    });
                    if (itemOpts.separator) {
                        item.addClass('menu-sep');
                    }
                    if (!item.hasClass('menu-sep')) {
                        item[0].itemName = itemOpts.name || '';
                        item[0].itemHref = itemOpts.href || '';

                        //  添加了下一行代码，以实现将 menu-item 的 hideOnClick 绑定到菜单项上
                        item[0].hideOnClick = (itemOpts.hideOnClick == undefined || itemOpts.hideOnClick == null ? true : itemOpts.hideOnClick);

                        var text = item.addClass('menu-item').html();
                        item.empty().append($('<div class="menu-text"></div>').html(text));
                        if (itemOpts.iconCls) {
                            $('<div class="menu-icon"></div>').addClass(itemOpts.iconCls).appendTo(item);
                        }
                        if (itemOpts.disabled) {
                            setDisabled(target, item[0], true);
                        }
                        if (item[0].submenu) {
                            $('<div class="menu-rightarrow"></div>').appendTo(item); // has sub menu
                        }

                        bindMenuItemEvent(target, item);
                    }
                });
                $('<div class="menu-line"></div>').prependTo(menu);
            }
            setMenuWidth(target, menu);
            menu.hide();

            bindMenuEvent(target, menu);
        }
    }

    function setMenuWidth(target, menu) {
        var opts = $.data(target, 'menu').options;
        var style = menu.attr('style') || '';
        menu.css({
            display: 'block',
            left: -10000,
            height: 'auto',
            overflow: 'hidden'
        });

        var el = menu[0];
        var width = el.originalWidth || 0;
        if (!width) {
            width = 0;
            menu.find('div.menu-text').each(function () {
                if (width < $(this)._outerWidth()) {
                    width = $(this)._outerWidth();
                }
                $(this).closest('div.menu-item')._outerHeight($(this)._outerHeight() + 2);
            });
            width += 40;
        }

        width = Math.max(width, opts.minWidth);
        var height = el.originalHeight || menu.outerHeight();
        var lineHeight = Math.max(el.originalHeight, menu.outerHeight()) - 2;
        menu._outerWidth(width)._outerHeight(height);
        menu.children('div.menu-line')._outerHeight(lineHeight);

        //		menu._outerWidth(Math.max((menu[0].originalWidth || 0), width, opts.minWidth));
        //
        //		menu.children('div.menu-line')._outerHeight(menu.outerHeight());

        style += ';width:' + el.style.width + ';height:' + el.style.height;

        menu.attr('style', style);
    }

    /**
     * bind menu event
     */
    function bindMenuEvent(target, menu) {
        var state = $.data(target, 'menu');
        menu.unbind('.menu').bind('mouseenter.menu', function () {
            if (state.timer) {
                clearTimeout(state.timer);
                state.timer = null;
            }
        }).bind('mouseleave.menu', function () {
            if (state.options.hideOnUnhover) {
                state.timer = setTimeout(function () {
                    hideAll(target);
                }, 100);
            }
        });
    }

    /**
     * bind menu item event
     */
    function bindMenuItemEvent(target, item) {
        if (!item.hasClass('menu-item')) { return }
        item.unbind('.menu');
        item.bind('click.menu', function () {
            var t = $(this);
            if (t.hasClass('menu-item-disabled')) {
                return;
            }
            // only the sub menu clicked can hide all menus
            if (!this.submenu) {
                //hideAll(target);
                //  注释掉上面一行代码，并添加下面一行代码，以实现当 menu-item 的属性 hideOnClick 为 false 的情况下，点击菜单项不自动隐藏菜单控件。
                if (this.hideOnClick) { hideAll(target); }

                var href = t.attr('href');
                if (href) {
                    location.href = href;
                }
            }
            var item = $(target).menu('getItem', this);
            $.data(target, 'menu').options.onClick.call(target, item);
        }).bind('mouseenter.menu', function (e) {
            // hide other menu
            item.siblings().each(function () {
                if (this.submenu) {
                    hideMenu(this.submenu);
                }
                $(this).removeClass('menu-active');
            });
            // show this menu
            item.addClass('menu-active');

            if ($(this).hasClass('menu-item-disabled')) {
                item.addClass('menu-active-disabled');
                return;
            }

            var submenu = item[0].submenu;
            if (submenu) {
                $(target).menu('show', {
                    menu: submenu,
                    parent: item
                });
            }
        }).bind('mouseleave.menu', function (e) {
            item.removeClass('menu-active menu-active-disabled');
            var submenu = item[0].submenu;
            if (submenu) {
                if (e.pageX >= parseInt(submenu.css('left'))) {
                    item.addClass('menu-active');
                } else {
                    hideMenu(submenu);
                }

            } else {
                item.removeClass('menu-active');
            }
        });
    }

    /**
     * hide top menu and it's all sub menus
     */
    function hideAll(target) {
        var state = $.data(target, 'menu');
        if (state) {
            if ($(target).is(':visible')) {
                hideMenu($(target));
                state.options.onHide.call(target);
            }
        }
        return false;
    }

    /**
     * show the menu, the 'param' object has one or more properties:
     * left: the left position to display
     * top: the top position to display
     * menu: the menu to display, if not defined, the 'target menu' is used
     * parent: the parent menu item to align to
     * alignTo: the element object to align to
     */
    function showMenu(target, param) {
        var left, top;
        param = param || {};
        var menu = $(param.menu || target);
        if (menu.hasClass('menu-top')) {
            var opts = $.data(target, 'menu').options;
            $.extend(opts, param);
            left = opts.left;
            top = opts.top;
            if (opts.alignTo) {
                var at = $(opts.alignTo);
                left = at.offset().left;
                top = at.offset().top + at._outerHeight();
                if (opts.align == 'right') {
                    left += at.outerWidth() - menu.outerWidth();
                }
            }
            if (left + menu.outerWidth() > $(window)._outerWidth() + $(document)._scrollLeft()) {
                left = $(window)._outerWidth() + $(document).scrollLeft() - menu.outerWidth() - 5;
            }
            if (left < 0) { left = 0; }
            if (top + menu.outerHeight() > $(window)._outerHeight() + $(document).scrollTop()) {
                top = $(window)._outerHeight() + $(document).scrollTop() - menu.outerHeight() - 5;
            }
        } else {
            var parent = param.parent;	// the parent menu item
            left = parent.offset().left + parent.outerWidth() - 2;
            if (left + menu.outerWidth() + 5 > $(window)._outerWidth() + $(document).scrollLeft()) {
                left = parent.offset().left - menu.outerWidth() + 2;
            }
            var top = parent.offset().top - 3;
            if (top + menu.outerHeight() > $(window)._outerHeight() + $(document).scrollTop()) {
                top = $(window)._outerHeight() + $(document).scrollTop() - menu.outerHeight() - 5;
            }
        }
        menu.css({ left: left, top: top });
        menu.show(0, function () {
            if (!menu[0].shadow) {
                menu[0].shadow = $('<div class="menu-shadow"></div>').insertAfter(menu);
            }
            menu[0].shadow.css({
                display: 'block',
                zIndex: $.fn.menu.defaults.zIndex++,
                left: menu.css('left'),
                top: menu.css('top'),
                width: menu.outerWidth(),
                height: menu.outerHeight()
            });
            menu.css('z-index', $.fn.menu.defaults.zIndex++);
            if (menu.hasClass('menu-top')) {
                $.data(menu[0], 'menu').options.onShow.call(menu[0]);
            }
        });
    }

    function hideMenu(menu) {
        if (!menu) return;

        hideit(menu);
        menu.find('div.menu-item').each(function () {
            if (this.submenu) {
                hideMenu(this.submenu);
            }
            $(this).removeClass('menu-active');
        });

        function hideit(m) {
            m.stop(true, true);
            if (m[0].shadow) {
                m[0].shadow.hide();
            }
            m.hide();
        }
    }

    function findItem(target, text) {
        var result = null;
        var tmp = $('<div></div>');
        function find(menu) {
            menu.children('div.menu-item').each(function () {
                var item = $(target).menu('getItem', this);
                var s = tmp.empty().html(item.text).text();
                if (text == $.trim(s)) {
                    result = item;
                } else if (this.submenu && !result) {
                    find(this.submenu);
                }
            });
        }
        find($(target));
        tmp.remove();
        return result;
    }

    function setDisabled(target, itemEl, disabled) {
        var t = $(itemEl);
        if (!t.hasClass('menu-item')) { return }

        if (disabled) {
            t.addClass('menu-item-disabled');
            if (itemEl.onclick) {
                itemEl.onclick1 = itemEl.onclick;
                itemEl.onclick = null;
            }
        } else {
            t.removeClass('menu-item-disabled');
            if (itemEl.onclick1) {
                itemEl.onclick = itemEl.onclick1;
                itemEl.onclick1 = null;
            }
        }
    }

    function appendItem(target, param) {
        var menu = $(target);
        if (param.parent) {
            if (!param.parent.submenu) {
                var submenu = $('<div class="menu"><div class="menu-line"></div></div>').appendTo('body');
                submenu.hide();
                param.parent.submenu = submenu;
                $('<div class="menu-rightarrow"></div>').appendTo(param.parent);
            }
            menu = param.parent.submenu;
        }
        if (param.separator) {
            var item = $('<div class="menu-sep"></div>').appendTo(menu);
        } else {
            var item = $('<div class="menu-item"></div>').appendTo(menu);
            $('<div class="menu-text"></div>').html(param.text).appendTo(item);
        }
        if (param.iconCls) $('<div class="menu-icon"></div>').addClass(param.iconCls).appendTo(item);
        if (param.id) item.attr('id', param.id);
        if (param.name) { item[0].itemName = param.name }
        if (param.href) { item[0].itemHref = param.href }
        if (param.onclick) {
            if (typeof param.onclick == 'string') {
                item.attr('onclick', param.onclick);
            } else {
                item[0].onclick = eval(param.onclick);
            }
        }
        if (param.handler) { item[0].onclick = eval(param.handler) }
        if (param.disabled) { setDisabled(target, item[0], true) }

        bindMenuItemEvent(target, item);
        bindMenuEvent(target, menu);
        setMenuWidth(target, menu);
    }

    function removeItem(target, itemEl) {
        function removeit(el) {
            if (el.submenu) {
                el.submenu.children('div.menu-item').each(function () {
                    removeit(this);
                });
                var shadow = el.submenu[0].shadow;
                if (shadow) shadow.remove();
                el.submenu.remove();
            }
            $(el).remove();
        }
        removeit(itemEl);
    }

    function destroyMenu(target) {
        $(target).children('div.menu-item').each(function () {
            removeItem(target, this);
        });
        if (target.shadow) target.shadow.remove();
        $(target).remove();
    }

    $.fn.menu = function (options, param) {
        if (typeof options == 'string') {
            return $.fn.menu.methods[options](this, param);
        }

        options = options || {};
        return this.each(function () {
            var state = $.data(this, 'menu');
            if (state) {
                $.extend(state.options, options);
            } else {
                state = $.data(this, 'menu', {
                    options: $.extend({}, $.fn.menu.defaults, $.fn.menu.parseOptions(this), options)
                });
                init(this);
            }
            $(this).css({
                left: state.options.left,
                top: state.options.top
            });
        });
    };

    $.fn.menu.methods = {
        options: function (jq) {
            return $.data(jq[0], 'menu').options;
        },
        show: function (jq, pos) {
            return jq.each(function () {
                showMenu(this, pos);
            });
        },
        hide: function (jq) {
            return jq.each(function () {
                hideAll(this);
            });
        },
        destroy: function (jq) {
            return jq.each(function () {
                destroyMenu(this);
            });
        },
        /**
         * set the menu item text
         * param: {
         * 	target: DOM object, indicate the menu item
         * 	text: string, the new text
         * }
         */
        setText: function (jq, param) {
            return jq.each(function () {
                $(param.target).children('div.menu-text').html(param.text);
            });
        },
        /**
         * set the menu icon class
         * param: {
         * 	target: DOM object, indicate the menu item
         * 	iconCls: the menu item icon class
         * }
         */
        setIcon: function (jq, param) {
            return jq.each(function () {
                $(param.target).children('div.menu-icon').remove();
                if (param.iconCls) {
                    $('<div class="menu-icon"></div>').addClass(param.iconCls).appendTo(param.target);
                }
            });
        },
        /**
         * get the menu item data that contains the following property:
         * {
         * 	target: DOM object, the menu item
         *  id: the menu id
         * 	text: the menu item text
         * 	iconCls: the icon class
         *  href: a remote address to redirect to
         *  onclick: a function to be called when the item is clicked
         * }
         */
        getItem: function (jq, itemEl) {
            var t = $(itemEl);
            var item = {
                target: itemEl,
                id: t.attr('id'),
                text: $.trim(t.children('div.menu-text').html()),
                disabled: t.hasClass('menu-item-disabled'),
                //				href: t.attr('href'),
                //				name: t.attr('name'),
                name: itemEl.itemName,
                href: itemEl.itemHref,
                //  增加下面一行代码，使得通过 getItem 方法返回的 menu-item 中包含其 hideOnClick 属性
                hideOnClick: itemEl.hideOnClick,
                onclick: itemEl.onclick
            }
            var icon = t.children('div.menu-icon');
            if (icon.length) {
                var cc = [];
                var aa = icon.attr('class').split(' ');
                for (var i = 0; i < aa.length; i++) {
                    if (aa[i] != 'menu-icon') {
                        cc.push(aa[i]);
                    }
                }
                item.iconCls = cc.join(' ');
            }
            return item;
        },
        findItem: function (jq, text) {
            return findItem(jq[0], text);
        },
        /**
         * append menu item, the param contains following properties:
         * parent,id,text,iconCls,href,onclick
         * when parent property is assigned, append menu item to it
         */
        appendItem: function (jq, param) {
            return jq.each(function () {
                appendItem(this, param);
            });
        },
        removeItem: function (jq, itemEl) {
            return jq.each(function () {
                removeItem(this, itemEl);
            });
        },
        enableItem: function (jq, itemEl) {
            return jq.each(function () {
                setDisabled(this, itemEl, false);
            });
        },
        disableItem: function (jq, itemEl) {
            return jq.each(function () {
                setDisabled(this, itemEl, true);
            });
        }
    };

    $.fn.menu.parseOptions = function (target) {
        return $.extend({}, $.parser.parseOptions(target, ['left', 'top', { minWidth: 'number', hideOnUnhover: "boolean" }]));
    };

    $.fn.menu.defaults = {
        zIndex: 110000,
        left: 0,
        top: 0,
        alignTo: null,
        align: 'left',
        minWidth: 120,
        hideOnUnhover: true,	// Automatically hides the menu when mouse exits it
        onShow: function () { },
        onHide: function () { },
        onClick: function (item) { }
    };

    //  下面这段代码实现即使在跨 IFRAME 的情况下，一个 WEB-PAGE 中也只能同时显示一个 easyui-menu 控件。
    $.easyui.bindPageNestedFunc("mousedown", "jdirkMenu", "menu", function (win, e) {
        var jq = win.jQuery,
            allMenu = jq("body>div.menu:visible"),
            m = jq(e.target).closest('div.menu', allMenu);
        if (m.length) { return }
        var menus = jq('body>div.menu-top:visible');
        if (menus.length) {
            menus.menu('hide');
        }
    });

    var buildMenu = function (options) {
        var menu = $("<div></div>").appendTo("body"),
            opts = $.extend({}, $.fn.menu.defaults, {
                left: window.event ? window.event.clientX : 0,
                top: window.event ? window.event.clientY : 0,
                hideOnUnhover: false,
                slideOut: false, autoDestroy: true,
                items: null, hideDisabledMenu: false, minWidth: 140
            }, options || {});
        opts.items = $.util.likeArrayNotString(opts.items) ? opts.items : [];
        if (opts.id) { menu.attr("id", opts.id); }
        if (opts.name) { menu.attr("name", opts.name); }
        if (!opts.items.length) { opts.items.push({ text: "当前无菜单项", disabled: true }); }
        $.each(opts.items, function (i, item) {
            if (opts.hideDisabledMenu && item.disabled) { return; } appendItemToMenu(menu, item, menu);
        });
        return { menu: menu, options: opts };
    };

    var appendItemToMenu = function (menu, item, menus) {
        if ($.util.isString(item) && $.array.contains(["-", "—", "|"], $.trim(item))) {
            $("<div></div>").addClass("menu-sep").appendTo(menu);
            return;
        }
        item = $.extend({
            id: null, text: null, iconCls: null, href: null, disabled: false,
            onclick: null, handler: null, bold: false, style: null,
            children: null, hideDisabledMenu: false, hideOnClick: true
        }, item || {});
        var onclick = item.onclick, handler = item.handler;
        item.onclick = undefined; item.handler = undefined;
        item = $.util.parseMapFunction(item);
        item.onclick = onclick; item.handler = handler;
        if (item.hideDisabledMenu && item.disabled) { return; }
        var itemEle = $("<div></div>").attr({
            iconCls: item.iconCls, href: item.href, disabled: item.disabled, hideOnClick: item.hideOnClick
        }).appendTo(menu);
        if (item.id) { itemEle.attr("id", item.id); }
        if (item.style) { itemEle.css(item.style); }
        if ($.isFunction(item.handler)) {
            var handler = item.handler;
            item.onclick = function (e, item, menus) { handler.call(this, e, item, menus); };
        }
        if ($.isFunction(item.onclick)) {
            itemEle.click(function (e) {
                if (itemEle.hasClass("menu-item-disabled")) { return; }
                item.onclick.call(this, e, item, menus);
            });
        }
        var hasChild = item.children && item.children.length ? true : false, span = $("<span></span>").appendTo(itemEle);
        if (item.text) { span.text(item.text); }
        if (item.bold) { span.css("font-weight", "bold"); }
        if (hasChild) {
            var itemNode = $("<div></div>").appendTo(itemEle);
            $.each(item.children, function (i, n) {
                var val = $.util.isString(n) && $.array.contains(["-", "—", "|"], $.trim(n)) ? n
                    : $.extend({ hideDisabledMenu: item.hideDisabledMenu }, n);
                appendItemToMenu(itemNode, val, menus);
            });
        }
    };

    $.extend($.easyui, {

        //  根据指定的属性创建 easyui-menu 对象；该方法定义如下参数：
        //      options: JSON 对象类型，参数属性继承 easyui-menu 控件的所有属性和事件（参考官方 API 文档），并在此基础上增加了如下参数：
        //          id: 一个 String 对象，表示创建的菜单对象的 ID 属性。
        //          name: 一个 String 对象，表示创建的菜单对象的 name 属性。
        //          hideOnUnhover: 这是官方 API 中原有属性，此处调整其默认值为 false；
        //          hideDisabledMenu: 一个 Boolean 值，默认为 false；该属性表示当菜单项的 disabled: true，是否自动隐藏该菜单项；
        //          items: 一个 Array 对象，该数组对象中的每一个元素都是一个 JSON 格式对象用于表示一个 menu item （关于 menu item 对象属性，参考官方 API）；
        //                  该数组中每个元素的属性，除 easyui-menu 中 menu item 官方 API 定义的属性外，还增加了如下属性：
        //              hideDisabledMenu: 该属性表示在当前子菜单级别下当菜单项的 disabled: true，是否自动隐藏该菜单项；一个 Boolean 值，取上一级的 hideDisabledMenu 值；
        //              handler: 一个回调函数，表示点击菜单项时触发的事件；
        //                  回调函数 handler 和回调函数 onclick 的签名都为 function(e, item, menu)，其中：
        //                      e:  表示动作事件；
        //                      item:   表示当前点击的菜单项的 options 选项；
        //                      menu:   表示整个菜单控件的 jQuery 对象。
        //                      函数中 this 指向触发事件的对象本身
        //                  另，如果同时定义了 onclick 和 handler，则只处理 handler 而不处理 onclick，所以请不要两个回调函数属性同时使用。
        //              children: 同上一级对象的 items 属性，为一个 Array 对象；
        //          slideOut:   一个 Boolean 类型值，表示菜单是否以滑动方式显示出来；默认为 false；
        //          autoDestroy: Boolean 类型值，表示菜单是否在隐藏时自动销毁，默认为 true；
        //  返回值：返回一个 JSON 格式对象，该返回的对象中具有如下属性：
        //      menu: 依据于传入参数 options 构建出的菜单 DOM 元素对象，这是一个 jQuery 对象，该对象未初始化为 easyui-menu 控件，而只是具有该控件的 DOM 结构；
        //      options: 传入参数 options 解析后的结果，该结果尚未用于但可用于初始化 menu 元素。
        createMenu: buildMenu,

        //  根据指定的属性创建 easyui-menu 对象并立即显示出来；该方法定义的参数和本插件文件中的插件方法 createMenu 相同：
        //  注意：本方法与 createMenu 方法不同之处在于：
        //      createMenu: 仅根据传入的 options 参数创建出符合 easyui-menu DOM 结构要求的 jQuery DOM 对象，但是该对象并未初始化为 easyui-menu 控件；
        //      showMenu: 该方法在 createMenu 方法的基础上，对创建出来的 jQuery DOM 对象立即进行 easyui-menu 结构初始化，并显示出来。
        //  返回值：返回一个 jQuery 对象，该对象表示创建并显示出的 easyui-menu 元素，该返回的元素已经被初始化为 easyui-menu 控件。
        showMenu: function (options) {
            var ret = buildMenu(options), mm = ret.menu, opts = mm.menu(ret.options).menu("options"), onHide = opts.onHide;
            opts.onHide = function () {
                var m = $(this);
                if ($.isFunction(onHide)) { onHide.apply(this, arguments); }
                if (opts.autoDestroy) {
                    $.util.exec(function () { m.menu("destroy"); });
                }
            };
            mm.menu("show", { left: opts.left, top: opts.top });
            if (opts.slideOut) {
                mm.hide().slideDown(200);
                if (mm[0] && mm[0].shadow) { mm[0].shadow.hide().slideDown(200); }
            }
            return mm;
        }
    });

    //  另，增加 easyui-menu 控件中 menu-item 的如下自定义扩展属性:
    //      hideOnClick:    Boolean 类型值，默认为 true；表示点击该菜单项后整个菜单是否会自动隐藏；
    //      bold:           Boolean 类型值，默认为 false；表示该菜单项是否字体加粗；
    //      style:          JSON-Object 类型值，默认为 null；表示要附加到该菜单项的样式；
    //  备注：上述增加的 menu-item 的自定义扩展属性，只有通过 $.easyui.createMenu 或者 $.easyui.showMenu 生成菜单时，才有效。

})(jQuery);

/**
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact author: info@jeasyui.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 * jQuery EasyUI window 组件扩展
 * jeasyui.extension.window
 *
 * 依赖项：
 *   1、jquery.extension.core.js
 *   2、jeasyui.extension.common
 *   3、jeasyui.extension.menu
 *   4、jeasyui.extension.panel
 */
(function ($, undefined) {

    $.fn.window.extensions = {};
    var initialize = function (target) {
        var t = $(target), state = $.data(target, "window"), opts = t.window("options"), win = t.window("window"), body = t.window("body");
        if (!opts._initialized) {
            t.window("header").on({
                dblclick: function () {
                    var opts = t.window("options");
                    if (opts.autoRestore) { if (opts.maximized) { t.window("restore"); } else if (opts.maximizable) { t.window("maximize"); } }
                },
                contextmenu: function (e) {
                    var opts = t.window("options");
                    if (opts.enableHeaderContextMenu) {
                        e.preventDefault();
                        var items = [
                            { text: "最大化", iconCls: "panel-tool-max", disabled: !opts.maximized && opts.maximizable ? false : true, onclick: function () { t.window("maximize"); } },
                            { text: "恢复", iconCls: "panel-tool-restore", disabled: opts.maximized ? false : true, onclick: function () { t.window("restore"); } },
                            "-",
                            { text: "关闭", iconCls: "panel-tool-close", disabled: !opts.closable, onclick: function () { t.window("close"); } }
                        ];
                        var headerContextMenu = $.array.likeArray(opts.headerContextMenu) ? opts.headerContextMenu : [];
                        if (headerContextMenu.length) { $.array.insertRange(items, 0, $.util.merge([], headerContextMenu, "-")); }
                        items = parseContextMenuMap(e, items, t);
                        $.easyui.showMenu({ items: items, left: e.pageX, top: e.pageY });
                    }
                }
            });
            if (opts.bodyCls) { body.addClass(opts.bodyCls); }

            if (opts.draggable) {
                var dragOpts = state.window.draggable("options"), cursor = dragOpts.cursor,
                    onBeforeDrag = dragOpts.onBeforeDrag, onStartDrag = dragOpts.onStartDrag, onStopDrag = dragOpts.onStopDrag, onDrag = dragOpts.onDrag;
                dragOpts.cursor = "default";
                dragOpts.onBeforeDrag = function (e) {
                    var ret = onBeforeDrag.apply(this, arguments);
                    if (ret == false || e.which != 1 || t.window("options").maximized) { return false; }
                    dragOpts.cursor = cursor;
                };
                dragOpts.onStartDrag = function () {
                    onStartDrag.apply(this, arguments);
                    t.window("body").addClass("window-body-hidden").children().addClass("window-body-hidden-proxy");
                };
                dragOpts.onStopDrag = function () {
                    onStopDrag.apply(this, arguments);
                    t.window("body").removeClass("window-body-hidden").children().removeClass("window-body-hidden-proxy");
                    dragOpts.cursor = "default";
                };
                dragOpts.onDrag = function (e) {
                    if (!opts.inContainer) { return onDrag.apply(this, arguments); }
                    var left = e.data.left, top = e.data.top,
                        p = win.parent(), root = p.is("body"),
                        scope = $.extend({}, root ? $.util.windowSize() : { width: p.innerWidth(), height: p.innerHeight() }),
                        width = $.isNumeric(opts.width) ? opts.width : win.outerWidth(),
                        height = $.isNumeric(opts.height) ? opts.height : win.outerHeight();
                    if (left < 0) { left = 0; }
                    if (top < 0) { top = 0; }
                    if (left + width > scope.width && left > 0) { left = scope.width - width; b = true; }
                    if (top + height > scope.height && top > 0) { top = scope.height - height; b = true; }
                    state.proxy.css({
                        display: 'block',
                        left: left,
                        top: top
                    });
                    return false;
                };
            }
            if (opts.resizable) {
                var resizableOpts = state.window.resizable("options"),
                    _onResize = resizableOpts.onResize, _onStopResize = resizableOpts.onStopResize;
                resizableOpts.onResize = function (e) {
                    if (!opts.minWidth && !opts.maxWidth && !opts.minHeight && !opts.maxHeight) {
                        return _onResize.apply(this, arguments);
                    }
                    state.proxy.css({ left: e.data.left, top: e.data.top });
                    var width = e.data.width, height = e.data.height,
                        minWidth = $.isNumeric(opts.minWidth) ? opts.minWidth : defaults.minHeight,
                        maxWidth = $.isNumeric(opts.maxWidth) ? opts.maxWidth : defaults.maxWidth,
                        minHeight = $.isNumeric(opts.minHeight) ? opts.minHeight : defaults.minHeight,
                        maxHeight = $.isNumeric(opts.maxHeight) ? opts.maxHeight : defaults.maxHeight;
                    if (width > opts.maxWidth) { width = maxWidth; resizable = true; }
                    if (width < opts.minWidth) { width = minWidth; resizable = true; }
                    if (height > opts.maxHeight) { height = maxHeight; resizable = true; }
                    if (height < opts.minHeight) { height = minHeight; resizable = true; }
                    state.proxy._outerWidth(width);
                    state.proxy._outerHeight(height);
                    return false;
                };
                resizableOpts.onStopResize = function (e) {
                    var ret = _onStopResize.apply(this, arguments);
                    if (t.window("options").maximized) {
                        t.window("restore").window("maximize");
                    }
                    return ret;
                };
            }

            opts._initialized = true;
        }
    };

    function parseContextMenuMap(e, menus, win) {
        return $.array.map(menus, function (value, index) {
            if (!value || $.util.isString(value)) { return value; }
            var ret = $.extend({}, value);
            ret.id = $.isFunction(value.id) ? value.id.call(ret, e, win) : value.id;
            ret.text = $.isFunction(value.text) ? value.text.call(ret, e, win) : value.text;
            ret.iconCls = $.isFunction(value.iconCls) ? value.iconCls.call(ret, e, win) : value.iconCls;
            ret.disabled = $.isFunction(value.disabled) ? value.disabled.call(ret, e, win) : value.disabled;
            ret.hideOnClick = $.isFunction(value.hideOnClick) ? value.hideOnClick.call(ret, e, win) : value.hideOnClick;
            ret.onclick = $.isFunction(value.onclick) ? function (e, item, menu) { value.onclick.call(this, e, win, item, menu); } : value.onclick;
            ret.handler = $.isFunction(value.handler) ? function (e, item, menu) { value.handler.call(this, e, win, item, menu); } : value.handler;
            if (ret.children && ret.children.length) { ret.children = parseContextMenuMap(e, ret.children, win); }
            return ret;
        });
    };

    var _window = $.fn.window;
    $.fn.window = function (options, param) {
        if (typeof options == "string") {
            return _window.apply(this, arguments);
        }
        options = options || {};
        return this.each(function () {
            var jq = $(this), hasInit = $.data(this, "window") ? true : false,
                opts = hasInit ? options : $.extend({}, $.fn.window.parseOptions(this), $.parser.parseOptions(this, [{
                    autoHCenter: "boolean", autoVCenter: "boolean", autoCloseOnEsc: "boolean",
                    autoRestore: "boolean", enableHeaderContextMenu: "boolean"
                }]), options);
            _window.call(jq, opts);
            initialize(this);
        });
    };
    $.union($.fn.window, _window);

    var methods = $.fn.window.extensions.methods = {};
    var defaults = $.fn.window.extensions.defaults = $.extend({}, $.fn.panel.extensions.defaults, {

        //  扩展 easyui-window 以及 easyui-dialog 控件的自定义属性，表示该窗口对象是否在屏幕大小调整的情况下自动进行左右居中，默认为 true。
        autoHCenter: false,

        //  扩展 easyui-window 以及 easyui-dialog 控件的自定义属性，表示该窗口对象是否在屏幕大小调整的情况下自动进行上下居中，默认为 true。
        autoVCenter: false,

        //  扩展 easyui-window 以及 easyui-dialog 控件的自定义属性，表示该窗口对象是否在按下 ESC，默认为 true。
        autoCloseOnEsc: true,

        //  扩展 easyui-window 以及 easyui-dialog 控件的自定义属性，表示该窗口是否在双击头部时自动最大化。
        autoRestore: true,

        //  扩展 easyui-window 以及 easyui-dialog 控件的自定义属性，表示是否启用该窗口的右键菜单。
        enableHeaderContextMenu: true,

        //  扩展 easyui-window 以及 easyui-dialog 控件的自定义属性，表示该窗口的右键菜单；
        //  这是一个数组格式对象，数组中的每一项都是一个 menu-item 元素；该 menu-item 元素格式定义如下：
        //      id:         表示菜单项的 id；
        //      text:       表示菜单项的显示文本；
        //      iconCls:    表示菜单项的左侧显示图标；
        //      disabled:   表示菜单项是否被禁用(禁用的菜单项点击无效)；
        //      hideOnClick:    表示该菜单项点击后整个右键菜单是否立即自动隐藏；
        //      bold:           Boolean 类型值，默认为 false；表示该菜单项是否字体加粗；
        //      style:          JSON-Object 类型值，默认为 null；表示要附加到该菜单项的样式；
        //      handler:    表示菜单项的点击事件，该事件函数格式为 function(e, win, item, menu)，其中 this 指向菜单项本身
        headerContextMenu: null
    });

    $.extend($.fn.window.defaults, defaults);
    $.extend($.fn.window.methods, methods);

    $(function () {
        //  设置当屏幕大小调整时，所有 easyui-window 或 easyui-dialog 窗口在属性 autoHCenter: true 或 autoVCenter: true 的情况下自动居中。
        $(window).resize(function () {
            $(".panel-body.window-body").each(function () {
                var win = $(this), opts = win.window("options");
                if (opts && opts.draggable) {
                    if (opts.autoHCenter || opts.autoVCenter) {
                        var method = opts.autoHCenter && opts.autoVCenter ? "center" : (opts.autoHCenter ? "hcenter" : "vcenter");
                        win.window(method);
                    } else if (opts.inContainer) { win.window("move"); }
                }
            });
        });

        //  在当前打开 modal:true 的 easyui-window 或者 easyui-dialog 时，按 ESC 键关闭顶层的 easyui-window 或者 easyui-dialog 对象。
        $(document).keydown(function (e) {
            if (e.which == 27) {
                $("div.window-mask:last").prevAll("div.panel.window:first").children(".panel-body.window-body").each(function () {
                    var win = $(this), opts = win.window("options");
                    if (opts && opts.closable && opts.autoCloseOnEsc && !win.window("header").find(".panel-tool a").attr("disabled")) {
                        $.util.exec(function () { win.window("close"); });
                    }
                });
            }
        });

        //  点击模式对话框（例如 easyui-messager、easyui-window、easyui-dialog）的背景遮蔽层使窗口闪动
        $("body").on("click", "div.window-mask:last", function (e) {
            $(this).prevAll("div.panel.window:first").shine();
        });
    });

})(jQuery);

/**
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact author: info@jeasyui.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 * jQuery EasyUI tabs 组件扩展
 * jeasyui.extension.tabs
 * 依赖项：
 *   1、jquery.extension.core.js
 *   2、jeasyui.extension.common
 *   3、jeasyui.extension.menu
 *   4、jeasyui.extension.panel
 *   5、jeasyui.extension.window
 */
(function ($, undefined) {


    $.fn.tabs.extensions = {};


    function initTabsPanelPaddingTopLine(target) {
        var t = $(target), opts = $.data(target, "tabs").options, position = opts.tabPosition;
        if ($.isNumeric(opts.lineHeight) && opts.lineHeight > 0) {
            if (!$.array.contains(["top", "bottom", "left", "right"], position)) { position = "top"; }
            t.children("div.tabs-panels").css("padding-" + position, opts.lineHeight.toString() + "px").children().children();
        }
    };


    var _onContextMenu = $.fn.tabs.defaults.onContextMenu;
    var onContextMenu = function (e, title, index) {
        if ($.isFunction(_onContextMenu)) { _onContextMenu.apply(this, arguments); }
        var t = $(this), opts = t.tabs("options");
        if (opts.enableConextMenu) {
            e.preventDefault();
            var panel = t.tabs("getTab", index),
                panelOpts = panel.panel("options"),
                leftTabs = t.tabs("leftClosableTabs", index),
                rightTabs = t.tabs("rightClosableTabs", index),
                otherTabs = t.tabs("otherClosableTabs", index),
                allTabs = t.tabs("closableTabs"),
                selected = t.tabs("isSelected", index),
                m0 = {
                    text: '在新页面中打开', iconCls: 'icon-standard-shape-move-forwards', disabled: panelOpts.href && panelOpts.iniframe ? false : true,
                    handler: function () { t.tabs("jumpTab", index); }
                },
                m1 = {
                    text: "显示 Option", iconCls: "icon-standard-application-form", disabled: opts.showOption ? false : true, children: [
                        { text: "选项卡组 Option", iconCls: "icon-standard-tab-go", handler: function () { t.tabs("showOption"); } },
                        { text: "该选项卡 Option", iconCls: "icon-standard-tab", handler: function () { t.tabs("showOption", index); } }
                    ]
                },
                m2 = {
                    text: "关闭选项卡", iconCls: "icon-standard-application-form-delete", disabled: panelOpts.closable ? false : true,
                    handler: function () { t.tabs("closeClosable", index); }
                },
                m3 = {
                    text: "关闭其他选项卡", iconCls: "icon-standard-cancel", disabled: otherTabs.length ? false : true,
                    handler: function () { t.tabs("closeOtherClosable", index); }
                },
                m4 = {
                    text: "刷新选项卡", iconCls: "icon-standard-table-refresh", disabled: panelOpts.refreshable && panelOpts.href ? false : true,
                    handler: function () { t.tabs("refresh", index); }
                },
                m5 = {
                    text: "关闭左侧选项卡", iconCls: "icon-standard-tab-close-left", disabled: leftTabs.length ? false : true,
                    handler: function () { t.tabs("closeLeftClosable", index); }
                },
                m6 = {
                    text: "关闭右侧选项卡", iconCls: "icon-standard-tab-close-right", disabled: rightTabs.length ? false : true,
                    handler: function () { t.tabs("closeRightClosable", index); }
                },
                m7 = {
                    text: "关闭所有选项卡", iconCls: "icon-standard-cross", disabled: allTabs.length ? false : true,
                    handler: function () { t.tabs("closeAllClosable"); }
                },
                m8 = {
                    text: "新建选项卡", iconCls: "icon-standard-tab-add", disabled: opts.enableNewTabMenu ? false : true,
                    handler: function () { t.tabs("newTab", index); }
                },
                m9 = {
                    text: "重复选项卡", iconCls: "icon-standard-control-repeat", disabled: panelOpts.repeatable ? false : true,
                    handler: function () { t.tabs("repeat", index); }
                };
            var items = [];
            if ($.array.likeArray(opts.contextMenu) && !$.util.isString(opts.contextMenu)) { $.array.merge(items, opts.contextMenu); }
            if (opts.enableJumpTabMenu) { $.array.merge(items, "-", m0); }
            if (opts.showOption) { $.array.merge(items, "-", m1); }
            $.array.merge(items, panelOpts.closable ? ["-", m2, m3] : ["-", m3]);
            if (panelOpts.refreshable) { $.array.merge(items, "-", m4); }
            $.array.merge(items, "-", m5, m6, m7);
            if (panelOpts.repeatable || opts.enableNewTabMenu) {
                var mm = [];
                if (opts.enableNewTabMenu) { mm.push(m8); }
                if (panelOpts.repeatable) { mm.push(m9); }
                $.array.merge(items, "-", mm);
            }
            items = parseContextMenuMap(e, title, index, items, t);
            if (items[0] == "-") { $.array.removeAt(items, 0); }
            $.easyui.showMenu({ left: e.pageX, top: e.pageY, items: items });
        }
    };
    function parseContextMenuMap(e, title, index, menus, tabs) {
        return $.array.map(menus, function (value) {
            if (!value || $.util.isString(value)) { return value; }
            var ret = $.extend({}, value);
            ret.id = $.isFunction(value.id) ? value.id.call(ret, e, title, index, tabs) : value.id;
            ret.text = $.isFunction(value.text) ? value.text.call(ret, e, title, index, tabs) : value.text;
            ret.iconCls = $.isFunction(value.iconCls) ? value.iconCls.call(ret, e, title, index, tabs) : value.iconCls;
            ret.disabled = $.isFunction(value.disabled) ? value.disabled.call(ret, e, title, index, tabs) : value.disabled;
            ret.hideOnClick = $.isFunction(value.hideOnClick) ? value.hideOnClick.call(ret, e, title, index, tabs) : value.hideOnClick;
            ret.onclick = $.isFunction(value.onclick) ? function (e, item, menu) { value.onclick.call(this, e, title, index, tabs, item, menu); } : value.onclick;
            ret.handler = $.isFunction(value.handler) ? function (e, item, menu) { value.handler.call(this, e, title, index, tabs, item, menu); } : value.handler;
            if (ret.children && ret.children.length) { ret.children = parseContextMenuMap(e, title, index, ret.children, tabs); }
            return ret;
        });
    };


    var _updateTab = $.fn.tabs.methods.update;
    function updateTab(target, param) {
        param = $.extend({ tab: null, options: null }, param);
        var tabs = $(target), opts = $.data(target, "tabs").options,
            index = tabs.tabs("getTabIndex", param.tab),
            panelOpts = $.union({}, param.options, $.fn.tabs.extensions.panelOptions),
            tools = panelOpts.tools,
            onLoad = panelOpts.onLoad, onError = panelOpts.onError,
            updateProgress = $.array.contains(["mask", "progress", "none"], opts.updateProgress) ? opts.updateProgress : "mask",
            loading = function () {
                if (updateProgress == "mask") {
                    $.easyui.loading({ topMost: true, msg: panelOpts.loadingMessage });
                } else if (updateProgress == "progress") {
                    $.easyui.messager.progress({ title: "操作提醒", msg: panelOpts.loadingMessage, interval: 100 });
                }
            },
            loaded = function () {
                if (updateProgress == "mask") {
                    $.easyui.loaded({ topMost: true });
                } else if (updateProgress == "progress") {
                    $.easyui.messager.progress("close");
                }
            },
            refreshButton = {
                iconCls: "icon-mini-refresh", handler: function () {
                    var title = $(this).parent().prev().find("span.tabs-title").text();
                    if (title) { $.util.exec(function () { tabs.tabs("refresh", title); }); }
                }
            };
        if (panelOpts.refreshable) {
            if ($.util.likeArrayNotString(panelOpts.tools)) {
                panelOpts.tools = $.array.merge([], panelOpts.tools, refreshButton);
            } else {
                panelOpts.tools = [refreshButton];
            }
        }
        if (updateProgress != "none" && !$.string.isNullOrWhiteSpace(panelOpts.href) && (panelOpts.selected || tabs.tabs("getSelected") == param.tab)) {
            loading();
            panelOpts.onLoad = function () {
                if ($.isFunction(onLoad)) { onLoad.apply(this, arguments); }
                $.util.exec(loaded);
                $(this).panel("options").onLoad = onLoad;
            };
            panelOpts.onError = function () {
                if ($.isFunction(onError)) { onError.apply(this, arguments); }
                $.util.exec(loaded);
                $(this).panel("options").onError = onError;
            };
        }
        var ret = _updateTab.call(tabs, tabs, { tab: param.tab, options: panelOpts });
        var tab = tabs.tabs("getTab", index);
        panelOpts = tab.panel("options");
        panelOpts.tools = tools;
        initTabsPanelPaddingTopLine(target);
        if (panelOpts.closeOnDblClick && panelOpts.closable) {
            tabs.find(">div.tabs-header>div.tabs-wrap>ul.tabs>li").eq(index).off("dblclick.closeOnDblClick").on("dblclick.closeOnDblClick", function () {
                tabs.tabs("close", panelOpts.title);
            }).attr("title", "双击此选项卡标题可以将其关闭");
        }
        return ret;
    };

    function refreshTab(target, which) {
        var tabs = $(target), opts = tabs.tabs("options"),
            panel = tabs.tabs("getTab", which), panelOpts = panel.panel("options"),
            index = tabs.tabs("getTabIndex", panel);
        if ($.string.isNullOrWhiteSpace(panelOpts.href) && $.string.isNullOrWhiteSpace(panelOpts.content)) { return; }
        panelOpts.fromTabs = false;
        tabs.tabs("update", { tab: panel, options: panelOpts });
        if ($.isFunction(opts.onRefresh)) { opts.onRefresh.call(target, opts.title, index); }
    };

    function isSelected(target, which) {
        var tabs = $(target), selected = tabs.tabs("getSelected"), index = tabs.tabs("getTabIndex", selected);
        var thisTab = tabs.tabs("getTab", which), thisIndex = tabs.tabs("getTabIndex", thisTab);
        return thisIndex == index;
    };

    function isClosable(target, which) {
        var tabs = $(target), panel = tabs.tabs("getTab", which), panelOpts = panel.panel("options");
        return panelOpts.closable;
    };

    function newTab(target, which) {
        var content = $("<table></table>").css({ width: "95%", height: "100%" }),
            txtTitle = $("<input type='text' style='width: 98%;'/>"),
            txtHref = $("<input type='text' style='width: 98%;'/>"),
            ckRefreshable = $("<input id='refreshable' type='checkbox' checked='true' />"),
            ckIniframe = $("<input id='iniframe' type='checkbox' />"),
            lblRefreshable = $("<label>是否可刷新</label>"),
            lblIniframe = $("<label>是否嵌至 IFRAME(浏览器内部窗体) 中</label>");

        var tr1 = $("<tr></tr>").append("<td width='24%' align='right'>选项卡标题：</td>").appendTo(content);
        var tr2 = $("<tr></tr>").append("<td width='24%' align='right'>路径(href)：</td>").appendTo(content);
        var tr3 = $("<tr></tr>").appendTo(content);
        $("<td></td>").append(txtTitle).appendTo(tr1);
        $("<td></td>").append(txtHref).appendTo(tr2);
        $("<td width='24%' align='right'></td>").append(ckRefreshable).append(lblRefreshable).appendTo(tr3);
        $("<td align='right'></td>").append(ckIniframe).append(lblIniframe).appendTo(tr3);

        which = which || 0;
        var tabs = $(target),
            index = $.isNumeric(which) ? which : tabs.tabs("getTabIndex", tabs.tabs("getTab", which)),
            header = tabs.find(">div.tabs-header>div.tabs-wrap>ul.tabs>li:eq(" + index + ")"),
            offset = header.offset(), position = $.extend({}, { left: offset.left + 10, top: offset.top + 10 });
        var dialogOptions = $.extend({
            iconCls: "icon-standard-application-form",
            title: "新建选项卡 - 设置参数",
            width: 400,
            height: 165,
            maximizable: false,
            resizable: false,
            autoVCenter: false,
            autoHCenter: false,
            enableSaveButton: false,
            topMost: false,
            applyButtonIndex: 1,
            applyButtonText: "打开",
            onApply: function (dia) {
                var title = txtTitle.val(), href = txtHref.val();
                href = href || $.fn.tabs.extensions.panelOptions.href;
                if ($.string.isNullOrWhiteSpace(title)) { title = "新建选项卡"; }
                var i = 0; while (tabs.tabs("getTab", title = title + (i ? i : ""))) { i++; }
                if ($.string.isNullOrWhiteSpace(href)) { $.easyui.messager.show("操作提醒", "请输入要创建的选项卡的路径！", "info"); txtHref.focus(); return; }
                var iniframe = ckIniframe.prop("checked"), refreshable = ckRefreshable.prop("checked");
                tabs.tabs("add", { title: title, href: href, refreshable: refreshable, closable: true, iniframe: iniframe });
                dia.dialog("close");
            },
            content: content
        }, position);
        var dia = $.easyui.showDialog(dialogOptions);
        $.util.exec(function () {
            var enter = dia.find(">div.dialog-button>a:first");
            txtTitle.keydown(function (e) { if (e.which == 13) { txtHref.focus(); } });
            txtHref.keydown(function (e) { if (e.which == 13) { ckRefreshable.focus(); } });
            ckRefreshable.keydown(function (e) { if (e.which == 13) { ckIniframe.focus(); } });
            ckIniframe.keydown(function (e) { if (e.which == 13) { enter.focus(); } });
            lblRefreshable.click(function () { ckRefreshable.click(); });
            lblIniframe.click(function () { ckIniframe.click(); });
            enter.focus();
            txtTitle.focus();
        });
    };

    function repeatTab(target, which) {
        var tabs = $(target), panel = tabs.tabs("getTab", which), panelOpts = panel.panel("options");
        var opts = $.extend({}, panelOpts, { selected: true, closable: true }), i = 2, title = opts.title;
        while (tabs.tabs("getTab", opts.title = title + "-" + i.toString())) { i++; }
        tabs.tabs("add", opts);
    };

    function getTabOption(target, which) {
        var t = $(target), tab = t.tabs("getTab", which), tabOpts = tab.panel("options");
        return tabOpts;
    };

    function getSelectedOption(target) {
        var t = $(target), tab = t.tabs("getSelected"), tabOpts = tab.panel("options");
        return tabOpts;
    };

    function getSelectedIndex(target) {
        var t = $(target), tab = t.tabs("getSelected"), index = t.tabs("getTabIndex", tab);
        return index;
    };

    function getSelectedTitle(target) {
        var t = $(target), tabOpts = t.tabs("getSelectedOption"), title = tabOpts.title;
        return title;
    };

    function leftTabs(target, which) {
        var tabs = $(target), index = $.isNumeric(which) ? which : tabs.tabs("getTabIndex", tabs.tabs("getTab", which)),
            panels = tabs.tabs("tabs");
        return $.array.range(panels, 0, index);
    };

    function rightTabs(target, which) {
        var tabs = $(target), index = $.isNumeric(which) ? which : tabs.tabs("getTabIndex", tabs.tabs("getTab", which)),
            panels = tabs.tabs("tabs");
        return $.array.range(panels, index + 1);
    };

    function otherTabs(target, which) {
        var tabs = $(target), index = $.isNumeric(which) ? which : tabs.tabs("getTabIndex", tabs.tabs("getTab", which)),
            panels = tabs.tabs("tabs");
        return $.array.merge($.array.range(panels, 0, index), $.array.range(panels, index + 1));
    };

    function closableFinder(val) {
        if ($.util.isJqueryObject(val) && val.length) {
            var state = $.data(val[0], "panel");
            return state && state.options && state.options.closable;
        } else { return false; }
    };

    function closableTabs(target) {
        var tabs = $(target), panels = tabs.tabs("tabs");
        return $.array.filter(panels, closableFinder);
    };

    function leftClosableTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("leftTabs", which);
        return $.array.filter(panels, closableFinder);
    };

    function rightClosableTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("rightTabs", which);
        return $.array.filter(panels, closableFinder);
    };

    function otherClosableTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("otherTabs", which);
        return $.array.filter(panels, closableFinder);
    };

    function closeLeftTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("leftTabs", which);
        $.each(panels, function () { tabs.tabs("close", tabs.tabs("getTabIndex", this)); });
    };

    function closeRightTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("rightTabs", which);
        $.each(panels, function () { tabs.tabs("close", tabs.tabs("getTabIndex", this)); });
    };

    function closeOtherTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("otherTabs", which);
        $.each(panels, function () { tabs.tabs("close", tabs.tabs("getTabIndex", this)); });
    };

    function closeAllTabs(target) {
        var tabs = $(target), panels = tabs.tabs("tabs");
        $.each($.array.clone(panels), function () { tabs.tabs("close", tabs.tabs("getTabIndex", this)); });
    };

    function closeClosableTab(target, which) {
        var tabs = $(target), panel = tabs.tabs("getTab", which);
        if (panel && panel.panel("options").closable) {
            var index = $.isNumeric(which) ? which : tabs.tabs("getTabIndex", panel);
            tabs.tabs("close", index);
        }
    };

    function closeLeftClosableTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("leftClosableTabs", which);
        $.each($.array.clone(panels), function () { tabs.tabs("close", tabs.tabs("getTabIndex", this)); });
    };

    function closeRightClosableTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("rightClosableTabs", which);
        $.each($.array.clone(panels), function () { tabs.tabs("close", tabs.tabs("getTabIndex", this)); });
    };

    function closeOtherClosableTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("otherClosableTabs", which);
        $.each($.array.clone(panels), function () { tabs.tabs("close", tabs.tabs("getTabIndex", this)); });
    };

    function closeAllClosableTabs(target, which) {
        var tabs = $(target), panels = tabs.tabs("closableTabs", which);
        $.each($.array.clone(panels), function () { tabs.tabs("close", tabs.tabs("getTabIndex", this)); });
    };

    function showOption(target, which) {
        var t = $(target), opts, pos;
        if (which != null && which != undefined) {
            var p = t.tabs("getTab", which);
            opts = p.panel("options");
            pos = p.panel("header").offset();
        } else {
            opts = t.tabs("options");
            pos = t.offset();
        }
        $.extend(pos, { left: pos.left + 25, top: pos.top + 15 });
        $.easyui.showOption(opts, pos);
    };

    function moveTab(tabTarget, param) {
        if (!param || param.source == undefined || param.target == undefined || !param.point) { return; }
        var source = param.source, target = param.target,
            point = $.array.contains(["before", "after"], param.point) ? param.point : "before",
            t = $(tabTarget), tabs = t.tabs("tabs"),
            sourcePanel = t.tabs("getTab", source), targetPanel = t.tabs("getTab", target),
            sourceIndex = t.tabs("getTabIndex", sourcePanel),
            sourceHeader = sourcePanel.panel("header"), targetHeader = targetPanel.panel("header");
        if (!sourcePanel || !targetPanel) { return; }

        $.array.removeAt(tabs, sourceIndex);
        var targetIndex = $.array.indexOf(tabs, targetPanel);
        $.array.insert(tabs, point == "before" ? targetIndex : targetIndex + 1, sourcePanel);

        sourcePanel = sourcePanel.panel("panel"); targetPanel = targetPanel.panel("panel");
        targetPanel[point](sourcePanel); targetHeader[point](sourceHeader);
    };

    function insertTab(tabTarget, options) {
        var target = options.target, t = $(tabTarget);
        options.target = undefined;
        t.tabs("add", options);
        var tabs = t.tabs("tabs");
        t.tabs("move", { source: tabs.length - 1, target: target, point: "before" });
    };

    function setTitle(target, param) {
        if (!param || !(param.which || $.isNumeric(param.which)) || !param.title) { return; }
        var t = $(target), tab = t.tabs("getTab", param.which);
        tab.panel("setTitle", param.title);
    };

    function jumpTab(target, which) {
        var t = $(target),
            tab = (which == null || which == undefined) ? t.tabs("getSelected") : t.tabs("getTab", which),
            opts = tab.panel("options");
        if (opts.href && opts.iniframe) {
            window.open(opts.href, "_blank");
        } else {
            $.easyui.messager.show("\"" + opts.title + "\" 选项卡不可在新页面中打开。");
        }
    };
    function updateIniframe(target, which) {
        var tabs = $(target), panel = tabs.tabs("getTab", which), opts = panel.panel("options");
        if (opts.href && opts.href.length)
            return;
        opts.href = opts.delayLoadIframe;
        refreshTab(target, which);
    };
    var panelOptions = $.fn.tabs.extensions.panelOptions = $.extend({}, $.fn.panel.defaults, {

        //  该选项卡的 href 是否在 iframe 中打开。
        iniframe: false,

        //  该选项卡是否具有重复打开功能
        repeatable: false,

        //  该选项卡是否具有刷新功能。
        refreshable: true,

        //  双击选项卡标题是否能将其关闭，当该选项卡 closable: true 时，该属性有效。
        closeOnDblClick: true,

        href: null,

        iconCls: "",

        fromTabs: true
    });
    var methods = $.fn.tabs.extensions.methods = {
        //  覆盖 easyui-tabs 的原生方法 update，以支持扩展的功能；
        update: function (jq, param) { return jq.each(function () { updateTab(this, param); }); },

        //  刷新指定的选项卡；该方法定义如下参数：
        //      which:  表示被刷新的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        refresh: function (jq, which) { return jq.each(function () { refreshTab(this, which); }); },

        //  判断指定的选项卡是否被选中；该方法定义如下参数：
        //      which:  要判断的选项卡的 索引号 或者 标题。
        //  返回值：如果指定的选项卡被选中，则返回 true，否则返回 false。
        isSelected: function (jq, which) { return isSelected(jq[0], which); },

        //  判断指定的选项卡是否可关闭(closable:true)；该方法定义如下参数：
        //      which:  要判断的选项卡的 索引号 或者 标题。
        //  返回值：如果指定的选项卡可被关闭(closable:true)，则返回 true，否则返回 false。
        isClosable: function (jq, which) { return isClosable(jq[0], which); },

        //  弹出一个 easyui-dialog，可以在该 dialog 中输入参数以在当前选项卡组件中创建一个新的选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题，可选，默认为 0；该参数用于指示弹出的 easyui-dialog 出现的位置。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        newTab: function (jq, which) { return jq.each(function () { newTab(this, which); }); },

        //  创建一个和指定选项卡相同内容的新选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        repeat: function (jq, which) { return jq.each(function () { repeatTab(this, which); }); },

        //  获取指定选项卡的属性值集合(option)；
        getTabOption: function (jq, which) { return getTabOption(jq[0], which); },

        //  获取当前选中的选项卡的属性值集合 (option)；
        getSelectedOption: function (jq) { return getSelectedOption(jq[0]); },

        //  获取当前选中的选项卡的索引号；
        getSelectedIndex: function (jq) { return getSelectedIndex(jq[0]); },

        //  获取当前选中的选项卡的标题。
        getSelectedTitle: function (jq) { return getSelectedTitle(jq[0]); },

        //  获取指定选项卡的左侧所有选项卡元素；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回一个数组，数组中的每一项都是一个表示选项卡页的 panel(jQuery) 对象；
        //      如果指定选项卡左侧没有其他选项卡，则返回一个空数组。
        leftTabs: function (jq, which) { return leftTabs(jq[0], which); },

        //  获取指定选项卡的右侧所有选项卡元素；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回一个数组，数组中的每一项都是一个表示选项卡页的 panel(jQuery) 对象；
        //      如果指定选项卡右侧没有其他选项卡，则返回一个空数组。
        rightTabs: function (jq, which) { return rightTabs(jq[0], which); },

        //  获取当前选项卡控件中除指定选项卡页在的其他所有选项卡元素；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回一个数组，数组中的每一项都是一个表示选项卡页的 panel(jQuery) 对象；
        //      如果当前选项卡控件除指定的选项卡页外没有其他选项卡，则返回一个空数组。
        otherTabs: function (jq, which) { return otherTabs(jq[0], which); },

        //  获取所有可关闭的选项卡页元素集合；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回一个数组，数组中的每一项都是一个表示选项卡页的 panel(jQuery) 对象；
        //      如果没有可关闭的选项卡，则返回一个空数组。
        closableTabs: function (jq) { return closableTabs(jq[0]); },

        //  获取指定选项卡左侧的所有可关闭的选项卡元素；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回一个数组，数组中的每一项都是一个表示选项卡页的 panel(jQuery) 对象；
        //      如果指定选项卡左侧没有可关闭的选项卡，则返回一个空数组。
        leftClosableTabs: function (jq, which) { return leftClosableTabs(jq[0], which); },

        //  获取指定选项卡右侧的所有可关闭的选项卡元素；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回一个数组，数组中的每一项都是一个表示选项卡页的 panel(jQuery) 对象；
        //      如果指定选项卡右侧没有可关闭的选项卡，则返回一个空数组。
        rightClosableTabs: function (jq, which) { return rightClosableTabs(jq[0], which); },

        //  获取当前选项卡控件中除指定选项卡页在的其他所有可关闭的选项卡元素；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回一个数组，数组中的每一项都是一个表示选项卡页的 panel(jQuery) 对象；
        //      如果当前选项卡控件除指定的选项卡页外没有其他可关闭的选项卡，则返回一个空数组。
        otherClosableTabs: function (jq, which) { return otherClosableTabs(jq[0], which); },

        //  关闭指定选项卡左侧的所有选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeLeft: function (jq, which) { return jq.each(function () { closeLeftTabs(this, which); }); },

        //  关闭指定选项卡右侧的所有选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeRight: function (jq, which) { return jq.each(function () { closeRightTabs(this, which); }); },

        //  关闭除指定选项卡外的其他所有选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeOther: function (jq, which) { return jq.each(function () { closeOtherTabs(this, which); }); },

        //  关闭所有选项卡；
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeAll: function (jq) { return jq.each(function () { closeAllTabs(this); }); },

        //  指定指定的选项卡，但是如果该选项卡不可被关闭(closable:false)，则不执行任何动作；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeClosable: function (jq, which) { return jq.each(function () { closeClosableTab(this, which); }); },

        //  指定指定的选项卡左侧的所有可关闭的选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeLeftClosable: function (jq, which) { return jq.each(function () { closeLeftClosableTabs(this, which); }); },

        //  指定指定的选项卡右侧的所有可关闭的选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeRightClosable: function (jq, which) { return jq.each(function () { closeRightClosableTabs(this, which); }); },

        //  指定除指定选项卡外的所有可关闭的选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeOtherClosable: function (jq, which) { return jq.each(function () { closeOtherClosableTabs(this, which); }); },

        //  指定所有可关闭的选项卡；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        closeAllClosable: function (jq) { return jq.each(function () { closeAllClosableTabs(this); }); },

        //  以 easyui-dialog 的方式弹出一个 dialog 对话框窗体，该窗体中显示指定选项卡的所有属性值(options)；该方法定义如下参数：
        //      which:  指定的选项卡的 索引号 或者 标题。该参数可选；如果不定义该参数，则显示选项卡组的 options 信息。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        showOption: function (jq, which) { return jq.each(function () { showOption(this, which); }); },

        //  将指定的 easyui-tabs tab-panel 选项卡页移动至另一位置；该方法定义如下参数：
        //      param:  这是一个 JSON-Object 对象，该对象定义如下属性：
        //          source: Integer 或 String 类型值，表示要移动的 tab-panel 的索引号或者标题 title 值；
        //          target: Integer 或 String 类型值，表示移动目标位置的 tab-panel 的索引号或者标题 title 值；
        //          point:  移动到目标位置的方式，String 类型值，仅限于定义为如下值：
        //              "before":   表示把 source 选项卡移动至 target 选项卡的前面，默认值；
        //              "after":    表示把 source 选项卡移动至 target 选项卡的后面；
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        move: function (jq, param) { return jq.each(function () { moveTab(this, param); }); },

        //  在当前 easyui-tabs 组件上创建一个新的选项卡，并将其移动至指定选项卡的前一格位置；该方法定义如下参数：
        //      options:  表示要创建的新选项卡的属性；是一个 JSON-Object 对象；
        //          该对象的各项属性参考 easyui-tabs 中 add 方法的参数 options，并在此基础上增加了如下属性：
        //          target: Integer 或 String 类型值，表示移动位置的 tab-panel 的索引号或者标题 title 值；
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        insert: function (jq, options) { return jq.each(function () { insertTab(this, options); }); },

        //  重设指定选项卡的标题名；该方法定义如下参数：
        //      param:  这是一个 JSON-Object 对象，该对象定义如下属性：
        //          which: 需要重设标题名的选项卡的 索引号(index) 或者原标题名(title)；
        //          title: 新的标题名；
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        setTitle: function (jq, param) { return jq.each(function () { setTitle(this, param); }); },

        //  将执行的选项卡在新页面中打开；该方法定义如下参数：
        //      which:  可选值参数；表示需要在新页面打开的的选项卡的 索引号(index) 或者原标题名(title)；如果未传入该参数，则对当前选中的选项卡进行操作。
        //  返回值：返回当前选项卡控件 easyui-tabs 的 jQuery 对象。
        jumpTab: function (jq, which) { return jq.each(function () { jumpTab(this, which); }); },

        updateIniframe: function (jq, which) { return jq.each(function () { updateIniframe(this, which); }); }
    };
    var defaults = $.fn.tabs.extensions.defaults = {
        //  增加 easyui-tabs 的自定义扩展属性，该属性表示当前选项卡标题栏和选项卡的 pane-body 之间的空白区域高(宽)度(px)；
        //  该参数是一个 Number 数值，默认为 2.
        lineHeight: 2,

        //  是否启用点击选项卡头的右键菜单。
        enableConextMenu: false,

        //  是否启用 “创建新选项卡” 的右键菜单。
        enableNewTabMenu: false,

        //  是否启用 "在新页面中打开" 选项卡的右键菜单。
        enableJumpTabMenu: false,

        //  定义 easyui-tabs 的 onRefresh 事件，当调用 easyui-tabs 的 refresh 方法后，将触发该事件。
        onRefresh: function (title, index) { },

        //  定义当 enableContextMenu 为 true 时，右键点击选项卡标题时候弹出的自定义右键菜单项内容；
        //  这是一个数组格式对象，数组中的每一项都是一个 menu-item 元素；该 menu-item 元素格式定义如下：
        //      id:         表示菜单项的 id；
        //      text:       表示菜单项的显示文本；
        //      iconCls:    表示菜单项的左侧显示图标；
        //      disabled:   表示菜单项是否被禁用(禁用的菜单项点击无效)；
        //      hideOnClick:    表示该菜单项点击后整个右键菜单是否立即自动隐藏；
        //      bold:           Boolean 类型值，默认为 false；表示该菜单项是否字体加粗；
        //      style:          JSON-Object 类型值，默认为 null；表示要附加到该菜单项的样式；
        //      handler:    表示菜单项的点击事件，该事件函数格式为 function(e, title, index, tabs, item, menu)，其中 this 指向菜单项本身
        contextMenu: null,

        //  覆盖 easyui-tabs 的原生事件属性 onContextMenu，以支持相应扩展功能。
        onContextMenu: onContextMenu,

        //  增加 easyui-tabs 的自定义扩展属性；该属性表示当右键点击选项卡头时，是否显示 "显示该选项卡的 option" 菜单项。
        //  Boolean 类型值，默认为 false。
        showOption: false,

        //  增加 easyui-tabs 的自定义扩展属性；该属性表示或者更新选项卡时，显示的遮蔽层进度条类型。
        //  String 类型值，可选的值限定范围如下：
        //      "mask": 表示遮蔽层 mask-loading 进度显示，默认值
        //      "progress": 表示调用 $.messager.progress 进行进度条效果显示
        //      "none": 表示不显示遮蔽层和进度条
        updateProgress: "mask"
    };

    $.extend($.fn.tabs.defaults, defaults);
    $.extend($.fn.tabs.defaults, { tabHeight: 36, plain: true, narrow: true });
    $.extend($.fn.tabs.methods, methods);




    function closeCurrentTab(target, iniframe) {
        iniframe = iniframe && !$.util.isUtilTop ? true : false;
        var current = $(target),
            currentTabs = current.currentTabs(),
            index;
        if (!iniframe && currentTabs.length) {
            index = current.currentTabIndex();
            if (index > -1) { currentTabs.tabs("close", index); }
        } else {
            var jq = $.util.parent.$;
            current = jq($.util.currentFrame);
            currentTabs = current.currentTabs();
            if (currentTabs.length) {
                index = current.currentTabIndex();
                if (index > -1) { currentTabs.tabs("close", index); }
            }
        }
    };

    function refreshCurrentTab(target, iniframe) {
        iniframe = iniframe && !$.util.isUtilTop ? true : false;
        var current = $(target),
            currentTabs = current.currentTabs(),
            index;
        if (!iniframe && currentTabs.length) {
            index = current.currentTabIndex();
            if (index > -1) { currentTabs.tabs("refresh", index); }
        } else {
            var jq = $.util.parent.$;
            current = jq($.util.currentFrame);
            currentTabs = current.currentTabs();
            if (currentTabs.length) {
                index = current.currentTabIndex();
                if (index > -1) { currentTabs.tabs("refresh", index); }
            }
        }
    };

    $.fn.extend({
        //  扩展 jQuery 对象的实例方法；用于关闭当前对象所在的 easyui-tabs 当前选项卡(支持当前选项卡页面为 iframe 加载的情况)。
        //  该方法定义如下参数：
        //      iniframe: Boolean 类型值，表示是否为关闭当前对象所在的父级页面的选项卡；默认为 false。
        //          如果当前页面为顶级页面，
        //          或者当前对象在 iframe 中但是不在当前iframe中的某个 easyui-tabs 内，则参数参数 inframe 无效。
        //  返回值：返回当前 jQuery 链式对象(实际上返回的 jQuery 对象中，所包含的元素已经被销毁，因为其容器 tab-panel 被关闭销毁了)。
        closeCurrentTab: function (iniframe) { return this.each(function () { closeCurrentTab(this, iniframe); }); },

        //  扩展 jQuery 对象的实例方法；用于刷新当前对象所在的 easyui-tabs 当前选项卡(支持当前选项卡页面为 iframe 加载的情况)。
        //  该方法定义如下参数：
        //      iniframe: Boolean 类型值，表示是否为刷新当前对象所在的父级页面的选项卡；默认为 false。
        //          如果当前页面为顶级页面，
        //          或者当前对象在 iframe 中但是不在当前iframe中的某个 easyui-tabs 内，则参数参数 inframe 无效。
        //  返回值：返回当前 jQuery 链式对象。
        refreshCurrentTab: function (iniframe) { return this.each(function () { refreshCurrentTab(this, iniframe); }); }
    });


})(jQuery);

/**
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact author: info@jeasyui.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 *
 * jQuery EasyUI validatebox 组件扩展
 * jeasyui.extension.validatebox

 * 依赖项：
 *   1、jquery.extension.core.js
 *   2、jeasyui.extension.common
 */
(function ($, undefined) {

    $.fn.validatebox.extensions = {};

    var rules = {
        //  只允许输入英文字母或数字
        engNum: {
            validator: function (value) {
                return /^[0-9a-zA-Z]*$/.test(value);
            },
            message: '请输入英文字母或数字'
        },
        //  只允许汉字、英文字母或数字
        chsEngNum: {
            validator: function (value, param) {
                return /^([\u4E00-\uFA29]|[\uE7C7-\uE7F3]|[a-zA-Z0-9])*$/.test(value);
            },
            message: '只允许汉字、英文字母或数字。'
        },
        //  只允许汉字、英文字母、数字及下划线
        code: {
            validator: function (value, param) {
                return /^[\u0391-\uFFE5\w]+$/.test(value);
            },
            message: '只允许汉字、英文字母、数字及下划线.'
        },
        //  验证是否为合法的用户名
        name: {
            validator: function (value) { return value.isUserName(); },
            message: "用户名不合法(字母开头，允许6-16字节，允许字母数字下划线)"
        },
        //  指定字符最小长度
        minLength: {
            validator: function (value, param) { return $.string.trim(value).length >= param[0]; },
            message: "最少输入 {0} 个字符."
        },
        //  指定字符最大长度
        maxLength: {
            validator: function (value, param) { return $.string.trim(value).length <= param[0]; },
            message: "最多输入 {0} 个字符."
        },
        //  必须包含指定的内容
        contains: {
            validator: function (value, param) { return $.string.contains(value, param[0]); },
            message: "输入的内容必须包含 {0}."
        },
        //  以指定的字符开头
        startsWith: {
            validator: function (value, param) { return $.string.startsWith(value, param[0]); },
            message: "输入的内容必须以 {0} 作为起始字符."
        },
        //  以指定的字符结束
        endsWith: {
            validator: function (value, param) { return $.string.endsWith(value, param[0]); },
            message: "输入的内容必须以 {0} 作为起始字符."
        },
        //  长日期时间(yyyy-MM-dd hh:mm:ss)格式
        longDate: {
            validator: function (value) { return $.string.isLongDate(value); },
            message: "输入的内容必须是长日期时间(yyyy-MM-dd hh:mm:ss)格式."
        },
        //  短日期(yyyy-MM-dd)格式
        shortDate: {
            validator: function (value) { return $.string.isShortDate(value); },
            message: "输入的内容必须是短日期(yyyy-MM-dd)格式."
        },
        //  长日期时间(yyyy-MM-dd hh:mm:ss)或短日期(yyyy-MM-dd)格式
        date: {
            validator: function (value) { return $.string.isDate(value); },
            message: "输入的内容必须是长日期时间(yyyy-MM-dd hh:mm:ss)或短日期(yyyy-MM-dd)格式."
        },
        //  电话号码(中国)格式
        tel: {
            validator: function (value) { return $.string.isTel(value); },
            message: "输入的内容必须是电话号码(中国)格式."
        },
        //  移动电话号码(中国)格式
        mobile: {
            validator: function (value) { return $.string.isMobile(value); },
            message: "输入的内容必须是移动电话号码(中国)格式."
        },
        //  电话号码(中国)或移动电话号码(中国)格式
        telOrMobile: {
            validator: function (value) { return $.string.isTelOrMobile(value); },
            message: "输入的内容必须是电话号码(中国)或移动电话号码(中国)格式."
        },
        //  传真号码(中国)格式
        fax: {
            validator: function (value) { return $.string.isFax(value); },
            message: "输入的内容必须是传真号码(中国)格式."
        },
        //  邮政编码(中国)格式
        zipCode: {
            validator: function (value) { return $.string.isZipCode(value); },
            message: "输入的内容必须是邮政编码(中国)格式."
        },
        //  必须包含中文汉字
        existChinese: {
            validator: function (value) { return $.string.existChinese(value); },
            message: "输入的内容必须是包含中文汉字."
        },
        //  必须是纯中文汉字
        chinese: {
            validator: function (value) { return $.string.isChinese(value); },
            message: "输入的内容必须是纯中文汉字."
        },
        //  必须是纯英文字母
        english: {
            validator: function (value) { return $.string.isEnglish(value); },
            message: "输入的内容必须是纯英文字母."
        },
        //  必须是合法的文件名(不能包含字符 \\/:*?\"<>|)
        fileName: {
            validator: function (value) { return $.string.isFileName(value); },
            message: "输入的内容必须是合法的文件名(不能包含字符 \\/:*?\"<>|)."
        },
        //  必须是正确的 IP地址v4 格式
        ip: {
            validator: function (value) { return $.string.isIPv4(value); },
            message: "输入的内容必须是正确的 IP地址v4 格式."
        },
        //  必须是正确的 url 格式
        url: {
            validator: function (value) { return $.string.isUrl(value); },
            message: "输入的内容必须是正确的 url 格式."
        },
        //  必须是正确的 IP地址v4 或 url 格式
        ipurl: {
            validator: function (value) { return $.string.isUrlOrIPv4(value); },
            message: "输入的内容必须是正确的 IP地址v4 或 url 格式."
        },
        //  必须是正确的货币金额(阿拉伯数字表示法)格式
        currency: {
            validator: function (value) { return $.string.isCurrency(value); },
            message: "输入的内容必须是正确的货币金额(阿拉伯数字表示法)格式."
        },
        //  必须是正确 QQ 号码格式
        qq: {
            validator: function (value) { return $.string.isQQ(value); },
            message: "输入的内容必须是正确 QQ 号码格式."
        },
        //  必须是正确 MSN 账户名格式
        msn: {
            validator: function (value) { return $.string.isMSN(value); },
            message: "输入的内容必须是正确 MSN 账户名格式."
        },
        unNormal: {
            validator: function (value) { return $.string.isUnNormal(value); },
            message: "输入的内容必须是不包含空格和非法字符Z."
        },
        isSpace: {
            validator: function (value) { return !$.string.isSpace(value); },
            message: "输入的内容不能是全空格."
        },
        //  必须是合法的汽车车牌号码格式
        carNo: {
            validator: function (value) { return $.string.isCarNo(value); },
            message: "输入的内容必须是合法的汽车车牌号码格式."
        },
        //  必须是合法的汽车发动机序列号格式
        carEngineNo: {
            validator: function (value) { return $.string.isCarEngineNo(value); },
            message: "输入的内容必须是合法的汽车发动机序列号格式."
        },
        //  必须是合法的身份证号码(中国)格式
        idCard: {
            validator: function (value) { return $.string.isIDCard(value); },
            message: "输入的内容必须是合法的身份证号码(中国)格式."
        },
        //  必须是合法的整数格式
        integer: {
            validator: function (value) { return $.string.isInteger(value); },
            message: "输入的内容必须是合法的整数格式."
        },
        //  必须是合法的整数格式且值介于 {0} 与 {1} 之间
        integerRange: {
            validator: function (value, param) {
                return $.string.isInteger(value) && ((param[0] || value >= param[0]) && (param[1] || value <= param[1]));
            },
            message: "输入的内容必须是合法的整数格式且值介于 {0} 与 {1} 之间."
        },
        //  必须是指定类型的数字格式
        numeric: {
            validator: function (value, param) { return $.string.isNumeric(value, param ? param[0] : undefined); },
            message: "输入的内容必须是指定类型的数字格式."
        },
        //  必须是指定类型的数字格式且介于 {0} 与 {1} 之间
        numericRange: {
            validator: function (value, param) {
                return $.string.isNumeric(value, param ? param[2] : undefined) && ((param[0] || value >= param[0]) && (param[1] || value <= param[1]));
            },
            message: "输入的内容必须是指定类型的数字格式且介于 {0} 与 {1} 之间."
        },
        //  必须是正确的 颜色(#FFFFFF形式) 格式
        color: {
            validator: function (value) { return $.string.isColor(value); },
            message: "输入的内容必须是正确的 颜色(#FFFFFF形式) 格式."
        },
        //  必须是安全的密码字符(由字符和数字组成，至少 6 位)格式
        password: {
            validator: function (value) { return $.string.isSafePassword(value); },
            message: "输入的内容必须是安全的密码字符(由字符和数字组成，至少 6 位)格式."
        },
        //  输入的字符必须是指定的内容相同
        equals: {
            validator: function (value, param) {
                var val = param[0], type = param[1];
                if (type) {
                    switch (String(type).toLowerCase()) {
                        case "jquery":
                        case "dom":
                            val = $(val).val();
                            break;
                        case "id":
                            val = $("#" + val).val();
                            break;
                        case "string":
                        default:
                            break;
                    }
                }
                return value === val;
            },
            message: "输入的内容不匹配."
        }
    };
    $.extend($.fn.validatebox.defaults.rules, rules);

    function initialize(target) {
        var t = $(target);
        var opts = t.validatebox("options");
        if (!opts._initialized) {
            t.addClass("validatebox-f").change(function () {
                opts.value = $(this).val();
                if ($.isFunction(opts.onChange)) {
                    opts.onChange.call(target, opts.value);
                }
            });
            opts.originalValue = opts.value;
            if (opts.value) {
                setValue(target, opts.value);
            }
            if (opts.width && !t.parent().is("span.combo,span.spinner,span.searchbox")) {
                resize(target, opts.width);
            }
            setPrompt(target, opts.prompt, opts);
            if (opts.autoFocus) {
                $.util.exec(function () { t.focus(); });
            }
            if (!opts.autovalidate) {
                t.validatebox("disableValidation").validatebox("enableValidation");
            }
            if (opts.defaultClass) {
                t.addClass(opts.defaultClass);
            }
            setDisabled(target, opts.disabled);
            opts._initialized = true;
        }
    };

    function setPrompt(target, prompt, opts) {
        var t = $(target);
        opts = opts || t.validatebox("options");
        opts.prompt = prompt;
        if ($.html5.testProp("placeholder", t[0].nodeName)) {
            t.attr("placeholder", prompt);
        } else {
            if (!$.isFunction(opts.promptFocus)) {
                opts.promptFocus = function () {
                    if (t.hasClass("validatebox-prompt")) {
                        t.removeClass("validatebox-prompt");
                        if (t.val() == opts.prompt) { t.val(""); }
                    }
                };
                t.focus(opts.promptFocus);
            }
            if (!$.isFunction(opts.promptBlur)) {
                opts.promptBlur = function () {
                    if ($.string.isNullOrEmpty(t.val())) { t.addClass("validatebox-prompt").val(opts.prompt); }
                }
                t.blur(opts.promptBlur);
            }
            if ($.string.isNullOrEmpty(t.val()) && !$.string.isNullOrEmpty(opts.prompt)) {
                $.util.exec(function () {
                    t.addClass("validatebox-prompt").val(opts.prompt);
                });
            }
        }
    }

    var _validate = $.fn.validatebox.methods.isValid;
    function validate(target) {
        var t = $(target);
        if (t.hasClass("validatebox-prompt")) {
            t.removeClass("validatebox-prompt").val("");
        }
        return _validate.call(t, t);
    };


    function setValue(target, value) {
        var t = $(target), opts = t.validatebox("options"), val = t.val();
        if (val != value) {
            t.val(opts.value = (value ? value : ""));
        }
        validate(target);
    };

    function getValue(target) {
        return $(target).val();
    };

    function clear(target) {
        var t = $(target), opts = t.validatebox("options");
        t.validatebox("setValue", "");
    };

    function reset(target) {
        var t = $(target), opts = t.validatebox("options");
        t.validatebox("setValue", opts.originalValue ? opts.originalValue : "");
    };

    function resize(target, width) {
        var t = $(target), opts = t.validatebox("options");
        t._outerWidth(opts.width = width);
    };

    function setDisabled(target, disabled) {
        var t = $(target), state = $.data(target, "validatebox");
        if (disabled) {
            if (state && state.options) { state.options.disabled = true; }
            t.attr("disabled", true);
        } else {
            if (state && state.options) { state.options.disabled = false; }
            t.removeAttr("disabled");
        }
    };


    var _validatebox = $.fn.validatebox;
    $.fn.validatebox = function (options, param) {
        if (typeof options == "string") {
            return _validatebox.apply(this, arguments);
        }
        options = options || {};
        return this.each(function () {
            var jq = $(this), hasInit = $.data(this, "validatebox") ? true : false,
                opts = hasInit ? options : $.extend({}, $.fn.validatebox.parseOptions(this), $.parser.parseOptions(this, [
                    "prompt", { autoFocus: "boolean" }
                ]), options);
            opts.value = opts.value || jq.val();
            _validatebox.call(jq, opts);
            initialize(this);
        });
    };
    $.union($.fn.validatebox, _validatebox);


    var methods = $.fn.validatebox.extensions.methods = {
        //  扩展 easyui-validatebox 的自定义扩展方法；设置当前 easyui-validatebox 控件的 prompt 值；该方法的参数 prompt 表示将被设置的 prompt 值；
        //  返回值：返回表示当前 easyui-validatebox 的 jQuery 链式对象。
        setPrompt: function (jq, prompt) { return jq.each(function () { setPrompt(this, prompt); }); },

        //  重写 easyui-validatebox 的原生方法；以支持相应扩展功能或属性。
        //  返回值：返回表示当前 easyui-validatebox 的 jQuery 链式对象。
        validate: function (jq) { return jq.each(function () { validate(this); }) },

        //  重写 easyui-validatebox 的原生方法；以支持相应扩展功能或属性。
        isValid: function (jq) { return validate(jq[0]); },

        setValue: function (jq, value) { return jq.each(function () { setValue(this, value); }); },

        getValue: function (jq) { return getValue(jq[0]); },

        clear: function (jq) { return jq.each(function () { clear(this); }); },

        reset: function (jq) { return jq.each(function () { reset(this); }); },

        resize: function (jq, width) { return jq.each(function () { resize(this, width); }); },

        enable: function (jq) { return jq.each(function () { setDisabled(this, false); }); },

        disable: function (jq) { return jq.each(function () { setDisabled(this, true); }); }
    };
    var defaults = $.fn.validatebox.extensions.defaults = {
        //  增加 easyui-validatebox 的扩展属性 prompt，该属性功能类似于 easyui-searchbox 的 prompt 属性。
        //  表示该验证输入框的提示文本；String 类型值，默认为 null。
        prompt: null,

        //  增加 easyui-validatebox 的扩展属性 autoFocus，该属性表示在当前页面加载完成后，该 easyui-validatebox 控件是否自动获得焦点。
        //  Boolean 类型值，默认为 false。
        autoFocus: false,

        //  增加 easyui-validatebox 的扩展属性 value，表示其初始化时的值
        value: null,

        //  增加 easyui-validatebox 的扩展属性 width，表示其初始化时的宽度值
        width: null,

        //  增加 easyui-validatebox 的扩展属性 autovalidate，表示是否在该控件初始化完成后立即进行一次验证；默认为 true。
        autovalidate: true,

        //  增加 easyui-validatebox 的扩展属性 disabled，表示该控件在初始化完成后是否设置其为禁用状态(disabled)；默认为 false。
        disabled: false,

        //  增加 easyui-validatebox 的扩展属性 defaultClass，表示 easyui-validatebox 初始化时默认需要加载的样式类名；
        //  该值将会被作为 html-class 属性在 easyui-validatebox 初始化完成后加载至 html 标签上。
        defaultClass: "",

        //  增加 easyui-validatebox 的扩展事件 onChange，表示输入框在值改变时所触发的事件
        onChange: function (value) { }
    };

    $.extend($.fn.validatebox.defaults, defaults);
    $.extend($.fn.validatebox.methods, methods);


    if ($.fn.form && $.isArray($.fn.form.otherList)) {
        $.fn.form.otherList.push("validatebox");
        //$.array.insert($.fn.form.otherList, 0, "validatebox");
    }

    //  修改 jQuery 本身的成员方法 val；使之支持 easyui-validatebox 的扩展属性 prompt。
    var core_val = $.fn.val;
    $.fn.val = function (value) {
        if (this.length > 0 && this.is(".validatebox-text.validatebox-prompt") && !$.html5.testProp("placeholder", this[0].nodeName)) {
            var val, opts = this.validatebox("options");
            if (arguments.length == 0) {
                val = core_val.apply(this, arguments);
                return val == opts.prompt ? "" : val;
            }
            if (value && value != opts.prompt) {
                this.removeClass("validatebox-prompt");
            }
        }
        return core_val.apply(this, arguments);
    };


})(jQuery);

/**
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact author: info@jeasyui.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 *
 * jQuery EasyUI form 组件扩展
 * jeasyui.extension.form
 *
 * 依赖项：
 *   1、jquery.extension.core.js
 *   2、jeasyui.extension.common
 *   3、jeasyui.extension.validatebox
 */
(function ($, undefined) {

    $.fn.form.extensions = {};

    function getData(target, param) {
        if (!param) {
            var t = $(target), state = $.data(target, "form"), opts = state ? state.options : $.fn.form.defaults;
            param = opts.serializer;
        }
        return $(target).serializeObject(param);
    };

    var _submit = $.fn.form.methods.submit;
    function submit(target, options) {
        var t = $(target), state = $.data(target, "form"),
            isForm = (/^(?:form)$/i.test(target.nodeName) && state) ? true : false,
            opts = $.extend(
                {}, (state ? state.options : $.fn.form.defaults), (typeof options == "string") ? { url: options } : ($.isFunction(options) ? { success: options } : options || {})
            ),
            loading = function () {
                if (opts.showLoading) {
                    $.easyui.loading({ msg: opts.loadingMessage, locale: opts.loadingLocale });
                }
            },
            loaded = function () {
                if (opts.showLoading) {
                    if (opts.loadedDelay) {
                        $.util.exec(function () { $.easyui.loaded(opts.loadingLocale); }, opts.loadedDelay);
                    } else {
                        $.easyui.loaded(opts.loadingLocale);
                    }
                }
            };

        if (!opts.url) { opts.url = window.location.href; }
        if (isForm) { return _submit.call(t, t, opts); }

        var param = t.form("getData");
        if ($.isFunction(opts.onSubmit) && opts.onSubmit.call(target, param) == false) { return loaded(); }
        var beforeSend = $.ajaxSettings.beforeSend, complete = $.ajaxSettings.complete;
        $.ajax({
            url: opts.url, type: opts.method, data: param,
            success: function (data) {
                if ($.isFunction(opts.success)) { return opts.success.call(target, data); }
            },
            beforeSend: function () {
                var ret = $.isFunction(beforeSend) ? beforeSend.apply(this, arguments) : undefined;
                loading();
                return ret;
            },
            complete: function () {
                var ret = $.isFunction(complete) ? complete.apply(this, arguments) : undefined;
                loaded();
                return ret;
            }
        });

        //$[opts.method](opts.url, param, function (data) { opts.success(data); });
    };

    function load(target, data) {
        if (!$.data(target, 'form')) {
            $.data(target, 'form', {
                options: $.extend({}, $.fn.form.defaults)
            });
        }
        var t = $(target), opts = $.data(target, 'form').options;

        if (typeof data == 'string') {
            var param = {};
            if (opts.onBeforeLoad.call(target, param) == false) { return; }
            $.ajax({
                url: data, data: param, dataType: 'json', type: opts.method,
                success: function (data) { _load(data); },
                error: function () { opts.onLoadError.apply(target, arguments); }
            });
        } else {
            _load(data);
        }

        function _load(data) {
            for (var name in data) {
                var val = data[name];
                var rr = _checkField(name, val);
                if (!rr.length) {
                    var count = _loadOther(name, val);
                    if (!count) {
                        $.each($.fn.form.valueMarkList, function (i, mark) {
                            $(mark + '[name="' + name + '"]', t).val(val);
                        });
                        $.each($.fn.form.textMarkList, function (i, mark) {
                            $(mark + '[name="' + name + '"]', t).text(val);
                        });
                    }
                }
                _loadCombo(name, val);
            }
            opts.onLoadSuccess.call(target, data);
            t.form("validate");
        }

        /**
         * check the checkbox and radio fields
         */
        function _checkField(name, val) {
            var rr = t.find('input[name="' + name + '"][type=radio], input[name="' + name + '"][type=checkbox]');
            rr._propAttr('checked', false);
            rr.each(function () {
                var f = $(this);
                if (f.val() == String(val) || $.inArray(f.val(), $.isArray(val) ? val : [val]) >= 0) {
                    f._propAttr('checked', true);
                }
            });
            return rr;
        }

        function _loadOther(name, val) {
            var count = 0;
            var pp = $.fn.form.otherList;
            for (var i = 0; i < pp.length; i++) {
                var p = pp[i];
                var f = t.find('[' + p + 'Name="' + name + '"]');
                if (f.length) {
                    f[p]('setValue', val);
                    count += f.length;
                }
            }
            return count;
        }

        function _loadCombo(name, val) {
            var cc = $.fn.form.comboList;
            var c = t.find('[comboName="' + name + '"]');
            if (c.length) {
                for (var i = cc.length - 1; i >= 0; i--) {
                    //for (var i = 0; i < cc.length; i++) {
                    var type = cc[i];
                    if (c.hasClass(type + '-f')) {
                        if (c[type]('options').multiple) {
                            c[type]('setValues', val);
                        } else {
                            c[type]('setValue', val);
                        }
                        return;
                    }
                }
            }
        }
    };

    function clear(target) {
        $($.fn.form.valueMarkList.join(","), target).each(function () {
            var t = this.type, tag = this.tagName.toLowerCase();
            if (t == 'text' || t == 'hidden' || t == 'password' || tag == 'textarea') {
                this.value = '';
            } else if (t == 'file') {
                var file = $(this), newfile = file.clone().insertAfter(file).val('');
                if (file.data('validatebox')) {
                    file.validatebox('destroy');
                    newfile.validatebox();
                } else {
                    file.remove();
                }
            } else if (t == 'checkbox' || t == 'radio') {
                this.checked = false;
            } else if (tag == 'select') {
                this.selectedIndex = -1;
            }
        });

        var t = $(target),
            plugins = $.array.distinct($.array.merge([], $.fn.form.otherList, $.fn.form.spinnerList, $.fn.form.comboList));
        for (var i = 0; i < plugins.length; i++) {
            var plugin = plugins[i],
                r = t.find('.' + plugin + '-f');
            if (r.length && $.fn[plugin] && $.fn[plugin]["methods"]) {
                $.util.tryExec(function () {
                    r[plugin]("clear");
                });
            }
        }
        t.form("validate");
    };

    function reset(target) {
        var t = $(target), state = $.data(target, "form"), isForm = /^(?:form)$/i.test(target.nodeName) && state ? true : false;
        if (isForm) {
            target.reset();
        }
        var plugins = $.array.distinct($.array.merge([], $.fn.form.otherList, $.fn.form.spinnerList, $.fn.form.comboList));
        for (var i = 0; i < plugins.length; i++) {
            var plugin = plugins[i];
            var r = t.find('.' + plugin + '-f');
            if (r.length && $.fn[plugin] && $.fn[plugin]["methods"]) {
                $.util.tryExec(function () {
                    r[plugin]("reset");
                });
            }
        }
        t.form("validate");
    };


    function validate(target) {
        var t = $(target);

        if ($.fn.validatebox) {
            t.find('.validatebox-text:not(:disabled)').validatebox('validate');
            var invalidbox = t.find('.validatebox-invalid');
            invalidbox.filter(':not(:disabled):first').focus();
            if (invalidbox.length) {
                return false;
            }
        }

        var plugins = $.array.distinct($.array.merge([], $.fn.form.otherList, $.fn.form.spinnerList, $.fn.form.comboList));
        for (var i = 0; i < plugins.length; i++) {
            var plugin = plugins[i];
            var r = t.find('.' + plugin + '-f');
            if (r.length && $.fn[plugin] && $.fn[plugin]["methods"]) {
                if ($.util.tryExec(function () { return r[plugin]("isValid"); }) === false) {
                    $.util.tryExec({
                        code: function () { r[plugin]("focus"); },
                        error: function () { r[plugin]("textbox").focus(); },
                        tryError: true
                    });
                    return false;
                }
            }
        }

        return true;
    }

    function setValidation(target, novalidate) {
        var t = $(target);
        t.find('.validatebox-text:not(:disabled)').validatebox(novalidate ? 'disableValidation' : 'enableValidation');

        var plugins = $.array.distinct($.array.merge([], $.fn.form.otherList, $.fn.form.spinnerList, $.fn.form.comboList));
        for (var i = 0; i < plugins.length; i++) {
            var plugin = plugins[i];
            var r = t.find('.' + plugin + '-f');
            if (r.length && $.fn[plugin] && $.fn[plugin]["methods"]) {
                $.util.tryExec(function () {
                    r[plugin](novalidate ? 'disableValidation' : 'enableValidation');
                });
            }
        }
    };

    function setFormDisabled(target, disabled, withButton) {
        var t = $(target), state = $.data(target, "form");
        disabled = disabled ? true : false;

        if (state && state.options) { state.disabled = disabled; }
        var cc = withButton ? t.find("input, select, textarea") : t.find("input, select, textarea, button, a.l-btn, .m-btn, .s-btn");
        if (withButton) {
            $.each(cc, function (i, elem) {
                var item = $(elem);
                if (item.is(".s-btn")) {
                    item.splitbutton(disabled ? "disable" : "enable");
                } else if (item.is(".m-btn")) {
                    item.menubutton(disabled ? "disable" : "enable");
                } else if (item.is("a.l-btn")) {
                    item.linkbutton(disabled ? "disable" : "enable");
                } else {
                    disabled ? item.attr("disabled", true) : item.removeAttr("disabled");
                }
            });
        }

        if ($.fn.validatebox) {
            t.find('.validatebox-text').validatebox(disabled ? "disable" : "enable");
        }
        var plugins = $.array.distinct($.array.merge([], $.fn.form.otherList, $.fn.form.spinnerList, $.fn.form.comboList));
        for (var i = 0; i < plugins.length; i++) {
            var plugin = plugins[i];
            var r = t.find('.' + plugin + '-f');
            if (r.length && $.fn[plugin] && $.fn[plugin]["methods"]) {
                $.util.tryExec(function () {
                    r[plugin](disabled ? "disable" : "enable");
                });
            }
        }
    };

    var methods = $.fn.form.extensions.methods = {
        //  获取 easyui-form 控件容器内所有表单控件的 JSON 序列化数据；该方法的参数 param 可以定义为如下格式：
        //      1、JSON-Object  ：该对象定义如下属性：
        //          onlyEnabled:    表示返回的结果数据中是否仅包含启用(disabled == false)的 HTML 表单控件；Boolean 类型值，默认为 false。
        //          transcript :    表示当范围内存在重名(name 相同时)的 DOM 元素时，对重复元素的取值规则；
        ///                 这是一个 String 类型值，可选的值限定在以下范围：
        //              cover  :    覆盖方式，只取后面元素 的值，丢弃前面元素的值；默认值；
        //              discard:    丢弃后面元素的值，只取前面元素的值；
        //              overlay:    将所有元素的值进行叠加；
        //          overtype   :    元素叠加方式，当 transcript 的值定义为 "overlay" 时，此属性方有效；
        //                  这是一个 String 类型值，可选的值限定在以下范围：
        //              array  :    将所有重复的元素叠加为一个数组；
        //              append :    将所有的重复元素叠加为一个字符串；默认值；
        //          separator  :    元素叠加的分隔符，定义将所有重名元素叠加为一个字符串时用于拼接字符串的分隔符；
        //                  这是一个 String 类型值，默认为 ","；当 transcript 的值定义为 "overlay" 且 overtype 的值定义为 "append" 时，此属性方有效。
        //      2、String 类型值:   表示当范围内存在重名(name 相同时)的 DOM 元素时，对重复元素的取值规则；
        //              其取值范围和当参数格式为 JSON-Object 时的属性 transcript 一样。
        //  返回值：该方法返回一个 JSON Object，返回对象中的每个数据都表示一个表单控件值。
        getData: function (jq, param) { return getData(jq[0], param); },

        //  重写 easyui-form 控件的 submit 方法，使之除了支持 form 标签提交外，还支持 div 等其他容器标签的提交。
        //      该方法中的参数 options 可以同 easyui-form 的原生方法 submit 参数格式一样；
        //      也可以是一个 String 类型值表示提交的服务器端 url 地址；
        //      也可以是一个 function 回调函数表示 ajax 提交成功后的回调函数；
        submit: function (jq, options) { return jq.each(function () { submit(this, options); }); },

        //  重写 easyui-form 控件的 clear 方法，使其支持扩展的 easyui 插件操作；
        clear: function (jq) { return jq.each(function () { clear(this); }); },

        //  重写 easyui-form 控件的 reset 方法，使其支持扩展的 easyui 插件操作；
        reset: function (jq) { return jq.each(function () { reset(this); }); },

        //  重写 easyui-form 控件的 validate 方法，使其支持扩展的 easyui 插件操作；
        validate: function (jq) { return validate(jq[0]); },

        //  重写 easyui-form 控件的 enableValidation 方法，使其支持扩展的 easyui 插件操作；
        enableValidation: function (jq) { return jq.each(function () { setValidation(this, false); }); },

        //  重写 easyui-form 控件的 disableValidation 方法，使其支持扩展的 easyui 插件操作；
        disableValidation: function (jq) { return jq.each(function () { setValidation(this, true); }); },

        //  增加 easyui-form 控件的自定义方法；启用该表单 DOM 所有子级节点的输入效果(移除所有子级可输入控件的 disabled 效果)
        //  该方法的参数 withButton 表示是否连同表单中的按钮控件(html-button、html-input-button、easyui-menu|linkbutton|menubutton|splitbutton)一并启用；
        enable: function (jq, withButton) { return jq.each(function () { setFormDisabled(this, false, withButton); }); },

        //  增加 easyui-form 控件的自定义方法；禁用该表单 DOM 所有子级节点的输入效果(给所有子级可输入控件增加 disabled 效果)
        //  该方法的参数 withButton 表示是否连同表单中的按钮控件(html-button、html-input-button、easyui-menu|linkbutton|menubutton|splitbutton)一并禁用；
        disable: function (jq, withButton) { return jq.each(function () { setFormDisabled(this, true, withButton); }); },

        //  重写 easyui-form 控件的 load 方法。
        load: function (jq, data) { return jq.each(function () { load(this, data); }); }
    };
    var defaults = $.fn.form.extensions.defaults = {

        method: "post",

        showLoading: true,

        loadingLocale: "body",

        loadingMessage: "正在将数据发送至服务器...",

        loadedDelay: 300,

        serializer: { onlyEnabled: true, transcript: "overlay", overtype: "append", separator: "," }
    };

    $.extend($.fn.form.defaults, defaults);
    $.extend($.fn.form.methods, methods);

    $.fn.form.comboList = ['combo', 'datebox', 'datetimebox', 'combogrid', 'combotree', 'combobox'];
    //$.fn.form.comboList = ['combobox', 'combotree', 'combogrid', 'datetimebox', 'datebox', 'combo'];
    $.fn.form.spinnerList = ['timespinner', 'numberspinner', 'spinner'];
    $.fn.form.valueMarkList = ["input", "textarea", "select"];
    $.fn.form.textMarkList = ["span", "label", "div", "p"];
    $.fn.form.otherList = ["numberbox", "slider"];

})(jQuery);


/**
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact author: info@jeasyui.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 *
 * jQuery EasyUI layout 组件扩展
 * jeasyui.extension.layout
 *
 * 依赖项：
 *   1、jquery.extension.core.js
 *   2、jeasyui.extension.common
 *
 */
(function ($, undefined) {

    $.fn.layout.extensions = { resizeDelay: 500 };

    function getPanels(target, withCenter) {
        var l = $(target),
            flag = (withCenter == null || withCenter == undefined) ? true : withCenter,
            regions = flag ? ["north", "west", "east", "center", "south"] : ["north", "west", "east", "south"];
        return $.array.reduce(regions, function (prev, val, index) {
            var p = l.layout("panel", val);
            if (p && p.length) {
                prev.push({ region: val, panel: p });
                prev[val] = p;
            }
            return prev;
        }, []);
    };

    function collapseRegion(l, region) {
        var p = l.layout("panel", region);
        if (p && p.length) {
            var opts = p.panel("options");
            if (!opts.collapsed) { l.layout("collapse", region); }
        }
    };

    function collapseAll(target) {
        var l = $(target), panels = l.layout("panels", false);
        $.each(panels, function (index, item) {
            var opts = item.panel.panel("options");
            if (!opts.collapsed) { l.layout("collapse", item.region); }
        });
        $.util.exec(function () { l.layout("resize"); }, $.fn.layout.extensions.resizeDelay);
    };

    function expandRegion(l, region) {
        var p = l.layout("panel", region);
        if (p && p.length) {
            var opts = p.panel("options");
            if (opts.collapsed) { l.layout("expand", region); }
        }
    };

    function expandAll(target) {
        var l = $(target), panels = l.layout("panels", false);
        $.each(panels, function (index, item) {
            var opts = item.panel.panel("options");
            if (opts.collapsed) { l.layout("expand", item.region); }
        });
        $.util.exec(function () { l.layout("resize"); }, $.fn.layout.extensions.resizeDelay);
    };


    function toggle(target, region) {
        if (!$.array.contains(["north", "west", "east", "center", "south"], region)) { return; }
        var l = $(target), p = l.layout("panel", region);
        if (p && p.length) {
            var opts = p.panel("options");
            if (opts.collapsed) { l.layout("expand", region); } else { l.layout("collapse", region); }
        }
    };

    function toggleAll(target, type) {
        if (!$.array.contains(["collapse", "expand", "toggle"], type)) { type = "toggle"; }
        var l = $(target), regions = ["north", "west", "east", "south"],
            hasCollapsed = $.array.some(regions, function (region) {
                var p = l.layout("panel", region);
                if (p && p.length) { var opts = p.panel("options"); return opts.collapsed ? true : false; } else { return false; }
            }),
            hasExpanded = $.array.some(regions, function (region) {
                var p = l.layout("panel", region);
                if (p && p.length) { var opts = p.panel("options"); return !opts.collapsed ? true : false; } else { return false; }
            });
        switch (type) {
            case "collapse":
                l.layout(hasExpanded ? "collapseAll" : "expandAll");
                break;
            case "expand":
                l.layout(hasCollapsed ? "expandAll" : "collapseAll");
                break;
            case "toggle":
                toggleRegions();
                break;
            default:
                toggleRegions();
                break;
        }
        function toggleRegions() {
            $.each(regions, function (i, region) {
                l.layout("toggle", region);
            });
            $.util.exec(function () { l.layout("resize"); }, $.fn.layout.extensions.resizeDelay);
        };
    };

    var defaults = $.fn.layout.extensions.defaults = {};

    var methods = $.fn.layout.extensions.methods = {

        // 扩展 easyui-layout 组件的自定义方法；获取 easyui-layout 组件的所有 panel 面板；
        // 该方法的参数 withCenter 是一个 boolean 类型值，默认为 true；表示返回的数组中是否包含 center panel。
        // 返回值：该方法返回一个 Array 数组对象；数组中的每个元素都是一个包含如下属性定义的 JSON-Object：
        //      region  : String 类型值，表示该面板所在的位置，可能的值为 "north"、"west"、"east"、"center"、"south"；
        //      panel   : jQuery 对象，表示 easyui-panel 面板对象；
        panels: function (jq, withCenter) { return getPanels(jq[0], withCenter); },

        //  扩展 easyui-layout 组件的自定义方法；用于折叠 easyui-layout 组件除 center 位置外的所有 panel 面板；
        //  返回值：返回表示当前 easyui-combo layout jQuery 链式对象。
        collapseAll: function (jq) { return jq.each(function () { collapseAll(this); }); },

        //  扩展 easyui-layout 组件的自定义方法；用于展开 easyui-layout 组件除 center 位置外的所有 panel 面板；
        //  返回值：返回表示当前 easyui-combo layout jQuery 链式对象。
        expandAll: function (jq) { return jq.each(function () { expandAll(this); }); },

        //  扩展 easyui-layout 组件的自定义方法；用于切换 panel 面板的 折叠/展开 状态；该方法定义如下参数：
        //      region: String 类型值，表示要切换 折叠/展开 状态的面板的位置；
        //  返回值：返回表示当前 easyui-combo layout jQuery 链式对象。
        toggle: function (jq, region) { return jq.each(function () { toggle(this, region); }); },

        //  扩展 easyui-layout 组件的自定义方法；用于切换所有 panel 面板的 折叠/展开 状态；该方法定义如下参数：
        //      type:   String 类型值，表示在进行 折叠/展开 操作时的操作方式；该参数传入的值限定在以下范围内：
        //          "collapse": 当既有展开的面板也有折叠的面板时，对所有面板执行折叠操作；
        //          "expand"  : 当既有展开的面板也有折叠的面板时，对所有面板执行展开操作；
        //          "toggle"  : 当既有展开的面板也有折叠的面板时，对所有面板执行切换 折叠/展开 状态操作；默认值。
        //  返回值：返回表示当前 easyui-combo layout jQuery 链式对象。
        toggleAll: function (jq, type) { return jq.each(function () { toggleAll(this, type); }); }
    };


    $.extend($.fn.layout.defaults, defaults);
    $.extend($.fn.layout.methods, methods);

})(jQuery);

/**
 *  tab收缩时，显示title
 */
(function ($) {
    var buttonDir = { north: 'down', south: 'up', east: 'left', west: 'right' };
    $.extend($.fn.layout.paneldefaults, {
        onBeforeCollapse: function () {
            /**/
            var popts = $(this).panel('options');
            var dir = popts.region;
            var btnDir = buttonDir[dir];
            if (!btnDir) return false;

            setTimeout(function () {
                var pDiv = $('.layout-button-' + btnDir).closest('.layout-expand').css({
                    textAlign: 'center', lineHeight: '18px', fontWeight: 'bold'
                });

                if (popts.title) {
                    var vTitle = popts.title;
                    if (dir == "east" || dir == "west") {
                        var vTitle = popts.title.split('').join('<br/>');
                        pDiv.find('.panel-body').html(vTitle);
                    } else {
                        $('.layout-button-' + btnDir).closest('.layout-expand').find('.panel-title')
                            .css({ textAlign: 'left' })
                            .html(vTitle)
                    }
                }
            }, 100);
        }
    });
})(jQuery);

/**
 * Licensed under the GPL or commercial licenses
 * To use it on other terms please contact author: info@jeasyui.com
 * http://www.gnu.org/licenses/gpl.txt
 * http://www.jeasyui.com/license_commercial.php
 *
 * jQuery EasyUI dialog 组件扩展
 * jeasyui.extension.dialog.js
 *
 * 依赖项：
 *   1、jquery.extension.core.js
 *   2、jeasyui.extension.common
 *   3、jeasyui.extension.menu
 *   4、jeasyui.extension.panel
 *   5、jeasyui.extension.window
 */
(function ($, undefined) {
    $.fn.dialog.extensions = {};

    var easyui = $.util.$.easyui ? $.util.$.easyui : $.easyui,
        cache = easyui.frameMapCache ? easyui.frameMapCache : easyui.frameMapCache = [];

    function resetCache(iframe) {
        var array = $.array.filter(cache, function (val) { return val.current == iframe; }), l = array.length;
        while (l--) { $.array.remove(cache, array[l]); };
    };

    var getParent = function () {
        var current = $.util.currentFrame;
        if (!current) { return $.util.top; }
        var p = $.array.first(cache, function (val) { return val.current == current; });
        return (p && p.parent && p.parent.contentWindow) ? p.parent.contentWindow : $.util.parent;
    };
    //  该属性仅可以在通过 $.easyui.showDialog 打开的 easyui-dialog 中的 iframe 中使用；
    //  该属性表示父级页面的 window 对象。
    $.easyui.parent = getParent();

    //  该方法仅可以在通过 $.easyui.showDialog 打开的 easyui-dialog 中的 iframe 中使用；
    //  关闭当前页面所在的 easyui-dialog 窗体。
    $.easyui.parent.closeDialog = $.easyui.closeCurrentDialog = function () {
        if ($.util.isUtilTop) { return; }
        $.easyui.parent.$($.util.currentFrame).closest("div.window-body").dialog("close");
    };

    $.easyui._showDialog = function (opts, currentFrame) {
        if (opts.onApply == null || opts.onApply == undefined) { opts.onApply = opts.onSave; }
        if (opts.onSave == null || opts.onSave == undefined) { opts.onSave = opts.onApply; }

        var _onClose = opts.onClose;
        opts.onClose = function () {
            if ($.isFunction(_onClose)) { _onClose.apply(this, arguments); }
            $.fn.dialog.defaults.onClose.apply(this, arguments);
            if (opts.autoDestroy) {
                $(this).dialog("destroy");
            }
        };

        var _onBeforeDestroy = opts.onBeforeDestroy;
        opts.onBeforeDestroy = function () {
            if (opts.iniframe) {
                var iframe = $(this).dialog("iframe");
                resetCache(iframe[0]);
            }
            var ret;
            if ($.isFunction(_onBeforeDestroy)) {
                ret = _onBeforeDestroy.apply(this, arguments);
            }
            if ($.fn.dialog.defaults.onBeforeDestroy.apply(this, arguments) == false) {
                return false;
            }
            return ret;
        };

        if (opts.locale) { opts.inline = true; }
        var dialog = $("<div></div>").appendTo(opts.locale ? opts.locale : "body");

        if (!$.util.likeArray(opts.toolbar)) { opts.toolbar = []; }
        if ($.isArray(opts.toolbar)) {
            $.each(opts.toolbar, function () {
                var handler = this.handler;
                if ($.isFunction(handler)) { this.handler = function () { handler.call(dialog, dialog); }; }
            });
            if (!opts.toolbar.length) { opts.toolbar = null; }
        }

        var buttons = [
            btnSave = {
                id: "save", text: opts.saveButtonText, iconCls: opts.saveButtonIconCls,
                index: opts.saveButtonIndex, hidden: opts.enableSaveButton ? false : true,
                handler: function (dia) {
                    var isFunc = $.isFunction(opts.onSave);
                    if (!isFunc || isFunc && opts.onSave.call(this, dia) !== false) {
                        $.util.exec(function () { dia.dialog("close"); });
                    }
                }
            },
            btnClose = {
                id: "close", text: opts.closeButtonText, iconCls: opts.closeButtonIconCls,
                index: opts.closeButtonIndex, hidden: opts.enableCloseButton ? false : true,
                handler: function (dia) { dia.dialog("close"); }
            },
            btnApply = {
                id: "apply", text: opts.applyButtonText, iconCls: opts.applyButtonIconCls,
                index: opts.applyButtonIndex, hidden: opts.enableApplyButton ? false : true,
                handler: function (dia) {
                    var isFunc = $.isFunction(opts.onApply);
                    if (!isFunc || isFunc && opts.onApply.call(this, dia) !== false) {
                        dia.applyButton.linkbutton("disable");
                    }
                }
            }
        ];

        if (!$.util.likeArrayNotString(opts.buttons)) { opts.buttons = []; }
        $.array.merge(opts.buttons, buttons);
        opts.buttons = $.array.filter(opts.buttons, function (val) { return $.util.parseFunction(val.hidden, val) ? false : true; });
        $.each(opts.buttons, function (i, btn) {
            var handler = btn.handler;
            if ($.isFunction(handler)) { btn.handler = function () { handler.call(this, dialog); }; }
        });
        $.array.sort(opts.buttons, function (a, b) {
            return ($.isNumeric(a.index) ? a.index : 0) - ($.isNumeric(b.index) ? b.index : 0);
        });

        if (!opts.buttons.length) { opts.buttons = null; }

        opts = dialog.dialog(opts).dialog("options");

        var dialogBody = dialog.dialog("body"),
            buttonbar = dialogBody.children(".dialog-button").each(function () {
                var color = dialog.css("border-bottom-color");
                $(this).addClass("calendar-header").css({ "height": "auto", "border-top-color": color });
            }),
            bottombuttons = buttonbar.children("a");
        if (opts.buttonsPlain) { bottombuttons.linkbutton("setPlain", true); }
        if (!opts.iniframe) {
            if (opts.href) {
                var toolbuttons = dialog.dialog("header").find(".panel-tool a");
                //toolbuttons.attr("disabled", "disabled");
                bottombuttons.linkbutton("disable");
                var onLoad = opts.onLoad;
                opts.onLoad = function () {
                    if ($.isFunction(onLoad)) { onLoad.apply(this, arguments); }
                    $.util.exec(function () {
                        toolbuttons.removeAttr("disabled");
                        bottombuttons.linkbutton("enable");
                    });
                };
            }
        } else {
            //var iframe = dialog.dialog("iframe");
            //if (iframe &&　iframe.length) { cache.push({ current: iframe[0], parent: currentFrame }); }
        }

        $.extend(dialog, {
            options: opts,
            //iframe: iframe,
            buttons: bottombuttons,
            closeButtn: buttonbar.children("#close"),
            saveButton: buttonbar.children("#save"),
            applyButton: buttonbar.children("#apply"),
            save: function () { btnSave.handler(); },
            close: function () { btnClose.handler(); },
            apply: function () { btnApply.handler(); }
        });

        return dialog;
    };

    //  以 easyui-dialog 方法在当前浏览器窗口的顶级(可访问)窗体中弹出对话框窗口；该函数定义如下参数：
    //      options:    一个 JSON Object，具体格式参考 easyui-dialog 官方 api 中的属性列表。
    //          该参数在 easyui-dialog 官方 api 所有原属性列表基础上，增加如下属性：
    //          iniframe:
    //          enableSaveButton:
    //          enableApplyButton:
    //          enableCloseButton:
    //          onSave:
    //          onClose:
    //          saveButtonText:
    //          applyButtonText:
    //          closeButtonText:
    //          saveButtonIconCls:
    //          applyButtonIconCls:
    //          closeButtonIconCls:
    //          buttonsPlain:
    //      另，重写 easyui-dialog 官方 api 的 buttons 属性，使其不支持 String-jQuerySelector 格式
    //  备注：
    //  返回值：返回弹出的 easyui-dialog 的 jQuery 对象。
    $.easyui.showDialog = function (options) {
        var opts = $.extend({}, $.easyui.showDialog.defaults, options);
        if (opts.locale) { opts.topMost = false; }
        var currentFrame = $.util.currentFrame, fn = opts.topMost ? $.util.$.easyui._showDialog : $.easyui._showDialog;
        return fn(opts, currentFrame);
    };

    //  通过调用 $.easyui.showDialog 方法，以 easyui-dialog 的方式显示一个 JSON - Object 对象的所有属性值；该函数定义如下参数：
    //      options:    需要显示的 JSON - Object；
    //      dialogOption:  该参数可选，表示要打开的 easyui-dialog 的 options。
    //  备注：该方法一般用于对象值显示，例如可以用于项目开发过程中的参数显示调试。
    //  返回值：返回弹出的 easyui-dialog 的 jQuery 对象。
    $.easyui.showOption = function (options, dialogOption) {
        options = options || "无数据显示。";
        var opts = $.extend({
            topMost: $.easyui.showDialog.defaults.topMost,
            title: "显示 options 值",
            width: 480,
            height: 260,
            minWidth: 360,
            minHeight: 220,
            autoVCenter: false,
            autoHCenter: false,
            enableSaveButton: false,
            enableApplyButton: false
        }, dialogOption || {}), jq = opts.topMost ? $.util.$ : $;
        var content = jq("<table class=\"dialog-options-body\" ></table>"), type = jq.type(options);
        if ($.array.contains(["array", "object", "function"], type)) {
            for (var key in options) {
                content.append("<tr class=\"dialog-options-row\"><td class=\"dialog-options-cell\">" + key + ":</td><td class=\"dialog-options-cell-content\">" + $.string.toHtmlEncode(options[key]) + "</td></tr>");
            }
        } else {
            content.append("<tr class=\"dialog-options-row\"><td class=\"dialog-options-cell\">options:</td><td class=\"dialog-options-cell-content\">" + $.string.toHtmlEncode(String(options)) + "</td></tr>");
        }
        opts.content = content;
        return $.easyui.showDialog(opts);
    };

    function loadPanel(target) {
        var state = $.data(target, "panel");
        if (!state.isLoaded || !opts.cache) {
            var t = $(target),
                opts = state.options,
                param = $.extend({}, opts.queryParams);
            if (opts.onBeforeLoad.call(target, param) == false) {
                return;
            }
            state.isLoaded = false;
            t.panel("clear");
            if (opts.loadingMessage) {
                t.html($("<div class=\"panel-loading\"></div>").html(opts.loadingMessage));
            }
            opts.loader.call(target, param, success, error);
            function success(data) {
                var html = opts.extractor.call(target, data);
                $(target).html(html);
                $.parser.parse($(target));
                opts.onLoad.apply(target, arguments);
                state.isLoaded = true;
            }
            function error() {
                opts.onLoadError.apply(target, arguments);
            }
        }
    }

    function loadPanelIframe(target) {
        var t = $(target),
            state = $.data(target, "panel"),
            opts = state.options,
            param = $.extend({}, opts.queryParams);
        if (opts.onBeforeLoad.call(target, param) === false) {
            return;
        }
        var content = "<iframe class='panel-iframe' frameborder='0' width='100%' height='100%' marginwidth='0px' marginheight='0px' scrolling='auto'></iframe>";

        t.panel("clear");
        t.addClass("panel-body-withframe").html(content);
        $.easyui.loading({ locale: t });
        t.panel("iframe").bind({
            load: function () {
                if ($.isFunction(opts.onLoad)) {
                    $.easyui.loaded({ locale: t });
                    opts.onLoad.apply(target, arguments);
                }
            },
            error: function () {
                if ($.isFunction(opts.onLoadError)) {
                    opts.onLoadError.apply(target, arguments);
                }
            }
        }).attr("src", opts.href || "");
        state.isLoaded = true;
    }

    function refresh(target, href) {
        var t = $(target), op = t.dialog("options"),
            state = $.data(target, "panel"),
            opts = state.options;
        state.isLoaded = false;
        opts.href = op.href;
        opts.iniframe = op.iniframe;
        opts.queryParams = op.queryParams;
        opts.delayLoadIframe = op.delayLoadIframe;
        opts.isLoad = op.isLoad;
        if (href) {
            if (typeof href == "string") {
                opts.href = href;
            } else {
                opts.queryParams = href;
            }
        }

        if (!opts.href)
            return;

        t.panel("body").removeClass("panel-body-withframe");
        if (opts.iniframe) {
            loadPanelIframe(target);
        } else {
            loadPanel(target);
        }
    };

    function getContentPanel(target) {
        var state = $.data(target, "dialog");
        return state ? state.contentPanel : null;
    };

    function getIframe(target) {
        var panel = getContentPanel(target);
        if (panel) {
            return panel.panel("iframe");
        }
    };

    function parseExtensionsBegin(options) {
        options._extensionsDialog = {
            href: options.href, content: options.content, iniframe: options.iniframe,
            bodyCls: options.bodyCls, bodyStyle: options.bodyStyle
        };
        options.bodyCls = null;
        options.bodyStyle = null;
        if (options.iniframe) {
            options.href = null;
            options.content = null;
            options.iniframe = false;
        }
    };
    function parseExtensionsEnd(target) {
        var d = $(target), opts = d.dialog("options"), cp = getContentPanel(target),
            exts = opts._extensionsDialog ? opts._extensionsDialog : opts._extensionsDialog = { href: opts.href, content: opts.content, iniframe: opts.iniframe };
        opts.href = exts.href; opts.content = exts.content; opts.iniframe = exts.iniframe;
        opts.bodyCls = exts.bodyCls; opts.bodyStyle = exts.bodyStyle;
        if (cp && cp.length) {
            if (opts.bodyCls) { cp.addClass(opts.bodyCls); }
            if (opts.bodyStyle) { cp.css(opts.bodyStyle); }
        }
        if (opts.iniframe) {
            refresh(target);
        }
    };

    var _dialog = $.fn.dialog;
    $.fn.dialog = function (options, param) {
        if (typeof options == "string") {
            return _dialog.apply(this, arguments);
        }
        options = options || {};
        return this.each(function () {
            var jq = $(this), hasInit = $.data(this, "dialog") ? true : false,
                opts = hasInit ? options : $.extend({}, $.fn.dialog.parseOptions(this), options);
            parseExtensionsBegin(opts);
            _dialog.call(jq, opts);
            parseExtensionsEnd(this);
        });
    };
    $.union($.fn.dialog, _dialog);


    var methods = $.fn.dialog.extensions.methods = {
        //  修复 easyui-dialog 组件的 options 方法返回的 width 和 height 属性不正确的 BUG
        options: function (jq) {
            var state = $.data(jq[0], "dialog"), opts = state.options,
                pp = jq.panel("options");
            $.extend(opts, {
                closed: pp.closed, collapsed: pp.collapsed, minimized: pp.minimized, maximized: pp.maximized,
                width: pp.width, height: pp.height
            });
            return opts;
        },

        //  扩展 easyui-dialog 控件的自定义方法；获取当前 easyui-dialog 控件的内容面板 panel 对象。
        contentPanel: function (jq) {
            return getContentPanel(jq[0]);
        },

        //  重写 easyui-panel 控件的自定义方法 iframe；获取当前 easyui-dialog 控件内容面板 panel 对象中的 iframe 对象。
        //  备注：如果 inirame: false，则该方法返回一个空的 jQuery 对象。
        iframe: function (jq) { return getIframe(jq[0]); },

        //  重写 easyui-dialog 控件的 refresh 方法，用于支持 iniframe 属性。
        refresh: function (jq, href) { return jq.each(function () { refresh(this, href); }); }
    };
    var defaults = $.fn.dialog.extensions.defaults = $.extend({}, $.fn.window.extensions.defaults, {

    });

    $.extend($.fn.dialog.defaults, defaults);
    $.extend($.fn.dialog.methods, methods);

    //  定义 $.easyui.showDialog 方法打开 easyui-dialog 窗体的默认属性。
    //  备注：该默认属性定义仅在方法 $.easyui.showDialog 中被调用。
    $.easyui.showDialog.defaults = {
        title: "新建对话框",
        iconCls: "icon-standard-application-form",
        width: 600,
        height: 360,
        modal: true,
        collapsible: false,
        maximizable: false,
        closable: true,
        draggable: true,
        resizable: true,
        shadow: true,
        minimizable: false,
        href: null,

        //  表示弹出的 easyui-dialog 窗体是否在关闭时自动销毁并释放浏览器资源；
        //  Boolean 类型值，默认为 true。
        autoDestroy: true,

        //  表示将要打开的 easyui-dialog 的父级容器；可以是一个表示 jQuery 元素选择器的表达式字符串，也可以是一个 html-dom 或 jQuery-dom 对象。
        //  注意：如果设置了该参数，则 topMost 属性将自动设置为 false。
        //      如果为 null 或者 undefined 则表示父级容器为 body 标签。
        locale: null,

        //  是否在顶级窗口打开此 easyui-dialog 组件。
        topMost: false,

        //  是否在iframe加载远程 href 页面数据
        iniframe: false,

        //  是否启用保存按钮，保存按钮点击后会关闭模式对话框
        enableSaveButton: true,

        //  是否启用应用按钮
        enableApplyButton: true,

        //  是否启用关闭按钮
        enableCloseButton: true,

        saveButtonIndex: 101,

        closeButtonIndex: 102,

        applyButtonIndex: 103,

        //  点击保存按钮触发的事件，如果该事件范围 false，则点击保存后窗口不关闭。
        onSave: null,

        //  点击应用按钮触发的事件，如果该事件范围 false，则点击应用后该按钮不被自动禁用。
        onApply: null,

        //  关闭窗口时应触发的事件，easyui-dialog本身就有
        onClose: null,

        //  保存按钮的文字内容
        saveButtonText: "确定",

        //  关闭按钮的文字内容
        closeButtonText: "取消",

        //  应用按钮的文字内容
        applyButtonText: "应用",

        //  保存按钮的图标样式
        saveButtonIconCls: "icon-save",

        //  应用按钮的图标样式
        applyButtonIconCls: "icon-ok",

        //  关闭按钮的图标样式
        closeButtonIconCls: "icon-cancel",

        //  底部工具栏的所有按钮是否全部设置 plain: true
        buttonsPlain: false
    };

})(jQuery);


function showMaxDialog(title, url) {
    return $.easyui.showDialog({
        title: title,
        modal: true,
        iniframe: true,
        href: url,
        fit: true,
        border: false,
        resizable: false,
        draggable: false,
        enableHeaderContextMenu: false,
        enableApplyButton: false,
        enableSaveButton: false,
        enableCloseButton: false
    });
}