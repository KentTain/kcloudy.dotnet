//搜索
$(function () {

    var sal = $("#search_type").children();//"请输入企业名称，关键词等",
    var s = ["请输入产品名称，例如：自动化、数控机床", "请输入求购标题，关键词等"];
    var key = $("#txt_search_keys");
    //key.val(s[0]);
    key.attr('placeholder', s[0]);
    sal.click(function () {
        var type = $("#txt_search_type");
        sal.removeClass("active");
        $(this).addClass("active");
        type.val(sal.index(this) + 1);
        //key.val('');
        key.attr('placeholder', s[sal.index(this)]);
        //key.val(s[sal.index(this)]);
    });
    //[头部搜索]文本框获取焦点后触发回车
    key.on('keypress', function (e) {
        if (e.keyCode == 13) {
            $("#search_btn").click();
            return false;
        }
    }).on('focus', function () {
        var index = $("#search_type").find(".active").index();
        if ($(this).val() === s[index]) {
            key.val('');
        }
    }).on('blur', function () {
        var index = $("#search_type").find(".active").index();
        if ($(this).val().length == 0) {
            //key.val(s[index]);
            key.attr('placeholder', s[index]);
        }
    });
    $(".header_row1 li").mouseover(function() {
        $(".header_station").css("display", "block");
    }).mouseleave(function() {
        $(".header_station").css("display", "none");
    });
    $(".header_row1 li").click(function () {
        if ($(this).attr("class") == "header_station") {
            var a1 = $(this).html();
            var a2 = $(".header_station_active span").html();
            $(this).html(a2);
            $(".header_station_active span").html(a1);
            if (a1 == "全站搜产品") {
                $("#txt_search_type").attr("value","2");
            } else {
                $("#txt_search_type").attr("value","1");
            }
            $(this).css("display","none");
        } 
    });
    //头部查询按钮处理
    $(".btn_search").click(function () {
        var keys = encodeURIComponent($.trim($(".header_search").val()));
        var type = $("#txt_search_type").val();

        if (keys.length >= 1) {
            var link='';
            //keys = escape(keys);
            switch (type) {
                case "1":
                    link = "/Offering?SearchKey=" + keys;
                    break;
                case "2":
                    link = starluDomain + "Offering?SearchKey=" + keys;
                    break;
            }
            if (link.length > 1) { location.replace(link); } else { alert("搜索出现错误！"); }

        } else { alert('请输入要搜索的内容'); }

    });
    $("#searchInStarlu").click(function () {
        var keys = escape($.trim($("#txt_search_keys").val()));
        var type = $("#txt_search_type").val();
        if (keys.length >= 1) {
            var link = '';
            //keys = escape(keys);
            switch (type) {
                case "1":
                    link =starluDomain+ "/Offering?SearchKey=" + keys;
                    break;
                case "2":
                    link = starluDomain+"/Buy/Search?SearchKey=" + keys;
                    break;
            }
            if (link.length > 1) { location.replace(link); } else { alert("搜索出现错误！"); }

        } else { alert('请输入要搜索的内容'); }

    });
});

(function ($) {
    $.fn.textRemindAuto = function (options) {
        options = options || {};
        var defaults = {
            blurColor: "#999",
            focusColor: "#333",
            auto: true,
            chgClass: ""
        };
        var settings = $.extend(defaults, options);
        $(this).each(function () {
            if (defaults.auto) {
                $(this).css("color", settings.blurColor);
            }
            var v = $.trim($(this).val());
            if (v) {
                $(this).focus(function () {
                    if ($.trim($(this).val()) === v) {
                        $(this).val("");
                    }
                    $(this).css("color", settings.focusColor);
                    if (settings.chgClass) {
                        $(this).toggleClass(settings.chgClass);
                    }
                }).blur(function () {
                    if ($.trim($(this).val()) === "") {
                        $(this).val(v);
                    }
                    $(this).css("color", settings.blurColor);
                    if (settings.chgClass) {
                        $(this).toggleClass(settings.chgClass);
                    }
                });
            }
        });
    };
})(jQuery);

//浮动导航
$(function () {
    if ($('.floatnav').length) {
        var wtop = $(window).height();
        $(window).scroll(function () {
            var sltop = $(this).scrollTop();
            if (sltop > wtop) {
                $(".floatnav").show();
            } else {
                $(".floatnav").hide();
            }
        });
        $(".floatnav ul li a").mouseenter(function () {
            $(this).children(".txt").show();
        });
        $(".floatnav ul li a").mouseleave(function () {
            $(this).children(".txt").hide();
        });
    }
});

function setHomepage(setUrl) {
    if (document.all) {
        document.body.style.behavior = 'url(#default#homepage)'; document.body.setHomePage(setUrl);
    }
    else if (window.sidebar) {
        if (window.netscape) {
            try {
                netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
            }
            catch (e) {
                alert("此操作被浏览器拒绝！ 请在浏览器地址栏输入'about:config'并回车 然后将[signed.applets.codebase_principal_support]设置为true");
            }
        }
        var prefs = Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefBranch);
        prefs.setCharPref('browser.startup.homepage', setUrl);
    }
    else {
        alert('您的浏览器不支持自动设置首页, 请使用浏览器菜单手动设置!');
    }
}

function AddFavorite(sURL, sTitle) {
    try {
        window.external.addFavorite(sURL, sTitle);
    }
    catch (e) {
        try {
            window.sidebar.addPanel(sTitle, sURL, "");
        }
        catch (e) {
            alert("加入收藏失败，请使用Ctrl+D进行添加");
        }
    }
}

/*tab选项卡*/
$(function () {
    var $tab_nav = $(".tab_nav ul li");
    $tab_nav.mouseenter(function () {
        $(this).addClass("selected").siblings().removeClass("selected");
        var index = $tab_nav.index(this);
        $("div>.tab_box > ul").eq(index).show().siblings().hide();
    });
});

function tab_(oUlId, oDivId) {
    var oUl = document.getElementById(oUlId);
    var oDiv = document.getElementById(oDivId);
    var oLi = oUl.getElementsByTagName('li');
    var oShow = oDiv.getElementsByTagName('ul');

    for (var i = 0; i < oLi.length; i++) {
        oLi[i].index = i;
        oLi[i].onmouseover = function () {
            for (var i = 0; i < oLi.length; i++) {
                oLi[i].className = '';
                oShow[i].style.display = 'none';
            }
            this.className = 'active';
            oShow[this.index].style.display = 'block';
        }
    }

}

storageUtils = {
    setParam: function (name, value) {
        localStorage.setItem(name, value);
    },
    getParam: function (name) {
        return localStorage.getItem(name);
    },
    removeItem: function (sKey) {
      localStorage.removeItem(sKey);
    }
}
 

function searchStoreOffering(domain,obj) {
    var $txtSearchStoreOffering = $.trim($(obj).val());
    //if ($txtSearchStoreOffering.length) {
        location.replace('/Store/' + domain + '?Key=' + escape($txtSearchStoreOffering) + "#right-content");
    //} else {
    //    window.tipError('请输入产品名称再点击搜索按钮!','s');
    //}
}

if (!Array.prototype.indexOf){  
    Array.prototype.indexOf = function(elt /*, from*/){  
    var len = this.length >>> 0;  
    var from = Number(arguments[1]) || 0;  
    from = (from < 0)  
            ? Math.ceil(from)  
            : Math.floor(from);  
    if (from < 0)  
        from += len;  
    for (; from < len; from++)  
    {  
        if (from in this &&  
            this[from] === elt)  
        return from;  
    }  
    return -1;  
    };  
}  