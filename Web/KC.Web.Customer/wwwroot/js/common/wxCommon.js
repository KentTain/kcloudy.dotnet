 AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};
Array.prototype.contains = function (item) {
    return RegExp("\\b" + item + "\\b").test(this);
};
function isMobile(str) {
    return /^(13|14|15|17|18)\d{9}$/i.test(str);
}
function isEmail(str) {
    return /[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?/.test(str);
}

function getPhoneType(){
	var phone_type = '';
    if (/(iPhone|iPad|iPod|iOS)/i.test(navigator.userAgent)) {  //判断iPhone|iPad|iPod|iOS
        phone_type = 'iphone';
    } else if (/(Android)/i.test(navigator.userAgent)) {   //判断Android
        phone_type = 'android';
    }
	return phone_type;
}

(function ($) {
    $.fn.passworddLevel = {
        init: function (th) {

            if (!th.val()) {
                this.funcs.primary();
                return;
            }
            if (th.val().length < 6) {
                this.funcs.weak();
                return;
            }
            var r = this.funcs.checkPassword(th);
            if (r < 1) {
                this.funcs.primary();
                return;
            }

            if (r > 0 && r < 2) {
                this.funcs.weak();
            } else if (r >= 2 && r < 4) {
                this.funcs.medium();
            } else if (r >= 4) {
                this.funcs.tough();
            }
        },
        funcs: {
            primary: function () {
                $('#pwdLevel_1').attr('class', 'zhuce_huixian');
                $('#pwdLevel_2').attr('class', 'zhuce_huixian');
                $('#pwdLevel_3').attr('class', 'zhuce_huixian');
            },
            weak: function () {
                $('#pwdLevel_1').attr('class', 'zhuce_hongxian');
                $('#pwdLevel_2').attr('class', 'zhuce_huixian');
                $('#pwdLevel_3').attr('class', 'zhuce_huixian');
            },
            medium: function () {
                $('#pwdLevel_1').attr('class', 'zhuce_hongxian');
                $('#pwdLevel_2').attr('class', 'zhuce_hongxian2');
                $('#pwdLevel_3').attr('class', 'zhuce_huixian');
            },
            tough: function () {
                $('#pwdLevel_1').attr('class', 'zhuce_hongxian');
                $('#pwdLevel_2').attr('class', 'zhuce_hongxian2');
                $('#pwdLevel_3').attr('class', 'zhuce_hongxian3');
            },
            corpses: function (pwdinput) {
                var cat = /./g;
                var str = $(pwdinput).val();
                var sz = str.match(cat);
                for (var i = 0; i < sz.length; i++) {
                    cat = /\d/;
                    var maths01 = cat.test(sz[i]);
                    cat = /[a-z]/;
                    var smalls01 = cat.test(sz[i]);
                    cat = /[A-Z]/;
                    var bigs01 = cat.test(sz[i]);
                    if (!maths01 && !smalls01 && !bigs01) {
                        return true;
                    }
                }
                return false;
            },
            checkPassword: function (pwdinput) {
                var maths, smalls, bigs, corps, cat, num;
                var str = $(pwdinput).val();
                var len = str.length;

                cat = /.{16}/g;
                if (len == 0) return 1;
                if (len > 16) {
                    $(pwdinput).val(str.match(cat)[0]);
                }
                cat = /.*[\u4e00-\u9fa5]+.*$/;
                if (cat.test(str)) {
                    return -1;
                }
                cat = /\d/;
                maths = cat.test(str);
                cat = /[a-z]/;
                smalls = cat.test(str);
                cat = /[A-Z]/;
                bigs = cat.test(str);
                corps = this.corpses(pwdinput);
                num = maths + smalls + bigs + corps;

                if (len < 6) {
                    return 1;
                }

                if (len >= 6 && len <= 8) {
                    if (num == 1) return 1;
                    if (num == 2 || num == 3) return 2;
                    if (num == 4) return 3;
                }

                if (len > 8 && len <= 11) {
                    if (num == 1) return 2;
                    if (num == 2) return 3;
                    if (num == 3) return 4;
                    if (num == 4) return 5;
                }

                if (len > 11) {
                    if (num == 1) return 3;
                    if (num == 2) return 4;
                    if (num > 2) return 5;
                }
            }
        }    
    };
})(jQuery);

var cfwin = (function($){
	var weixin = function(){
		this.$ = $;
		this.options = {
			clickEvent : ('ontouchstart' in window)?'tap':'click',
			readyEvent : 'ready', //宿主容器的准备事件，默认是document的ready事件
			backEvent : 'backmenu', //宿主容器的返回按钮
			complete : false, //是否启动完成
			crossDomainHandler : null, //跨域请求的处理类
			showPageLoading : false, //ajax默认是否有loading界面
			viewSuffix : '.html', //加载静态文件的默认后缀
			lazyloadPlaceholder : '' //懒人加载默认图片
		};
		
		this.pop = {
			hasAside : false,
			hasPop : false
		};
        this.previewImage=function(url,obj,del) {
            var gallery = '<div class="weui-gallery" style="display:block">';
            gallery += '<span class="weui-gallery__img" style="background-image:url('+url+')"></span>';
            if(del){
	            gallery += '<div class="weui-gallery__opr">';

                gallery += '<a href="javascript:" class="weui-gallery__del">';
                gallery += '<i class="weui-icon-delete weui-icon_gallery-delete"></i>';
                gallery += '</a>';
                gallery += '</div>';
            }
            gallery += '</div>';

            $('body').append(gallery);
            setTimeout(function() {
                $('.weui-gallery__img').on(cfwin.options.clickEvent,
                function() {
                    setTimeout(function() {$('.weui-gallery').remove();},100);
                });
            }, 500);
            if (del) {
                $('.weui-gallery__del').on(this.options.clickEvent,
                function() {
                   $.confirm('确定删除该图片？', function() {
                       var $parent = $(obj).parent();
                        $parent.removeAttr('data-id');
                       $(obj).remove();
                       $('.weui-gallery').remove();
                   });
                });
            }
        }
    
        this.uploader=function(input) {
            var images = {
                localId: [],
                serverId: []
            };
            var $parent = $(input).parent();
            if (!$parent.attr('data-id')) {
                $weixin.chooseImage({
                    count: 1,
                    success: function(res) {
                        images.localId = res.localIds;
                        uploadImg(input);
                    }
                });
            }

            function uploadImg(obj) {
                var i = 0,length = images.localId.length;
                images.serverId = [];

                function upload() {
                    $weixin.uploadImage({
                        localId: images.localId[i],
                        success: function(res) {
                            var url = images.localId[i];
                            var $img = $('<img class="thumbnail" src="' + url + '">');
                            $parent.append($img);
                            $img.on(cfwin.options.clickEvent,
                                function(e) {
                                    cfwin.previewImage(url,this, true);
                                });
                            $parent.attr('data-id', res.serverId);
                            i++;
                            images.serverId.push(res.serverId);
                            if (i < length) {
                                upload();
                            }

                        },
                        fail: function(res) {
                            alert(JSON.stringify(res));
                        }
                    });
                }
                upload();
            }
        }

	};
    return new weixin();
})(jQuery);

function resizeImage(src,callback,w,h){
    var canvas = document.createElement("canvas"),
        ctx = canvas.getContext("2d"),
        im = new Image();
        w = w || 0,
        h = h || 0;
    im.onload = function(){
        //为传入缩放尺寸用原尺寸
        !w && (w = this.width);
        !h && (h = this.height);
        //以长宽最大值作为最终生成图片的依据
        if(w !== this.width || h !== this.height){
            var ratio;
            if(w>h){
                ratio = this.width / w;
                h = this.height / ratio;
            }else if(w===h){
                if(this.width>this.height){
                    ratio = this.width / w;
                    h = this.height / ratio;
                }else{
                    ratio = this.height / h;
                    w = this.width / ratio;
                }
            }else{
                ratio = this.height / h;
                w = this.width / ratio;
            }
        }
        //以传入的长宽作为最终生成图片的尺寸
        if(w>h){
            var offset = (w - h) / 2;
            canvas.width = canvas.height = w;
            ctx.drawImage(im,0,offset,w,h);
        }else if(w<h){
            var offset = (h - w) / 2;
            canvas.width = canvas.height = h;
            ctx.drawImage(im,offset,0,w,h);
        }else{
            canvas.width = canvas.height = h;
            ctx.drawImage(im,0,0,w,h);
        }
        callback(canvas.toDataURL("image/png"));
    }
    im.src = src;
}
