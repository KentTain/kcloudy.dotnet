var globalData;
if (!globalData) globalData = {};
globalData.currentAppId = '';
globalData.apps = [];

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};

Array.prototype.contains = function (item) {
    return RegExp("\\b" + item + "\\b").test(this);
};
Array.prototype.remove = function (index) {
    if (isNaN(index) || index > this.length) {
        return false;
    }
    this.splice(index, 1);
};

Array.prototype.indexOf = function(object) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == object) {
            return i;
        }
    }
    return -1;
};

Array.prototype.removeChild = function(object) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == object) {
            this.remove(i);
            break;
        }
    }
};

(function ($) {
    $.fn.serializeJson = function () {
        var serializeObj = {};
        var array = this.serializeArray();
        var str = this.serialize();
        $(array).each(function () {
            if (serializeObj[this.name]) {
                if ($.isArray(serializeObj[this.name])) {
                    serializeObj[this.name].push(this.value);
                } else {
                    serializeObj[this.name] = [serializeObj[this.name], this.value];
                }
            } else {
                serializeObj[this.name] = this.value;
            }
        });
        return serializeObj;
    };
});


//2015-04-29 10:26:30  add
String.prototype.format = function(args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof(args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        } else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                } else {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, '');
                }
            }
        }
    }
    return result;
};

function GetRandomNumber() {
    return Math.floor(Math.random() * 999999 + 1);
}


//fmoney("12345.675910", 3)，返回12,345.676 
function fmoney(s, n) //s:传入的float数字 ，n:希望返回小数点几位 
{
    n = n > 0 && n <= 20 ? n : 2;
    s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
    var l = s.split(".")[0].split("").reverse(),
    r = s.split(".")[1];
    var t = "";
    for (var i = 0; i < l.length; i++) {
        t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
    }
    return t.split("").reverse().join("") + "." + r;
}

//rmoney(12,345.676) //返回结果为：12345.676 
function rmoney(s) {
    return parseFloat(s.replace(/[^\d\.-]/g, ""));
}

function JsonDateFormat(jsonDate) {
    try {
        var date = new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
        var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
        var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
        return date.getFullYear() + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;
    } catch (ex) {
        return "";
    }
}

function formatShortDate(d) {
    return d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();
}

var Common = {

    //EasyUI用DataGrid用日期格式化
    BoolFormatter: function(value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        if (value) {
            return "是";
        } else {
            return "否";
        }
    },
    SexFormatter: function(value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        if (value) {
            return "男";
        } else {
            return "女";
        }
    },
    TimeFormatter: function(value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }
        var val = dateValue.format("yyyy-MM-dd HH:mm");
        return val.substr(11, 5);
    },
    DateTimeFormatter: function(value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd HH:mm");
    },
    DateTimeSecondFormatter: function(value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd HH:mm:ss");
    },

    //EasyUI用DataGrid用日期格式化
    DateFormatter: function(value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd");
    },
    TitleFormatter: function(value, rec, index) {
        if (value.length > 10) value = value.substr(0, 8) + "...";
        return value;
    },
    LongTitleFormatter: function(value, rec, index) {
        if (value.length > 15) value = value.substr(0, 12) + "...";
        return value;
    },
    isEmail: function(str) {
        return /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i.test(str);
    },
    isMobile: function(str) {
        return /^(13|14|15|17|18)\d{9}$/i.test(str);
    }
};

var defaultGuid = '{00000000-0000-0000-0000-000000000000}';

//cfwin 上传组件
(function ($) {
    var $cfwinUploadSettings = {
        btnStartUpload: null,
        btnAddFile: null,
        btnContinueAddFile:null,
        fileList: null,
        baseUrl: '/js/cfw.webuploader/',
        postUrl: '/Home/UploadFileToTemp',
        chunkCheckUrl: '/Home/ChunkCheck',
        chunksMergeUrl: '/Home/ChunksMerge',
        chunkSize: 3 * 1024 * 1024, //分块大小
        fileNumLimit: 10,
        type: 0, //0:image,1:file,2:all
        params: { tenatName: '', userId:'',blobId: '' },
        configure: {
            imageMaxSize: 0,
            imageExt: '',
            fileMaxSize: 0,
            fileExt: ''
        },
        callback: {
            uploadProgress: function (file, percentage) {

            },
            uploadComplete: function (file) {//不管成功或者失败，文件上传完成时触发

            },
            uploadSuccess: function (file, response) {

            },
            uploadError: function (file, reason) {

            },
            onFileQueued: function (file) {
                
            }
        }
    };

    $.fn.cfwinUploader = {
        upload: function (zSetting) {
            var settings = $cfwinUploadSettings;
            $.extend(true, settings, zSetting);
            var maxSize = 0, ext = '', filesMd5 = {}, state = 'pending', uploader, uploadType;
            switch ($cfwinUploadSettings.type) {
                case 0:
                    maxSize = settings.configure.imageMaxSize;
                    ext = settings.configure.imageExt;
                    uploadType = 'uploadimage';
                    break;
                case 1:
                    maxSize = settings.configure.fileMaxSize;
                    ext = settings.configure.fileExt;
                    uploadType = 'uploadfile';
                    break;
                default:
                    maxSize = settings.configure.fileMaxSize;
                    ext = settings.configure.imageExt + ',' + settings.configure.fileExt;
                    uploadType = '';
                    break;
            }

            WebUploader.Uploader.register({
                "before-send-file": "beforeSendFile",
                "before-send": "beforeSend",
                "after-send-file": "afterSendFile"
            }, {
                beforeSendFile: function (file) {
                    var task = new $.Deferred();
                    var start = new Date().getTime();
                    (new WebUploader.Uploader()).md5File(file, 0, 10 * 1024 * 1024).progress(function (percentage) {
                        console.log(percentage);
                    }).then(function (val) {
                        console.log("总耗时: " + ((new Date().getTime()) - start) / 1000);

                        task.resolve();
                        //拿到上传文件的唯一名称，用于断点续传
                        filesMd5[file.name] = md5('' + settings.tenatName + file.name + file.type + file.lastModifiedDate + file.size);
                    });
                    return $.when(task);
                },
                beforeSend: function (block) {
                    //分片验证是否已传过，用于断点续传
                    var task = new $.Deferred();
                    //不需要分片上传
                    if (block.chunks == 1) {
                        task.resolve();
                    } else {
                        $.ajax({
                            type: "POST",
                            url: settings.chunkCheckUrl,
                            data: {
                                name: filesMd5[block.file.name],
                                chunkIndex: block.chunk,
                                size: block.end - block.start
                            },
                            cache: false,
                            timeout: 1000 //todo 超时的话，只能认为该分片未上传过
                            ,
                            dataType: "json"
                        }).then(function (data, textStatus, jqXHR) {
                            if (data.ifExist) { //若存在，返回失败给WebUploader，表明该分块不需要上传
                                task.reject();
                            } else {
                                task.resolve();
                            }
                        }, function (jqXHR, textStatus, errorThrown) { //任何形式的验证失败，都触发重新上传
                            task.resolve();
                        });
                    }

                    return $.when(task);
                },
                afterSendFile: function (file) {
                    var chunksTotal = Math.ceil(file.size / settings.chunkSize);
                    if (chunksTotal > 1) {
                        //合并请求
                        var task = new $.Deferred();
                        $.ajax({
                            type: "POST",
                            url: settings.chunksMergeUrl,
                            data: {
                                folder: filesMd5[file.name],
                                chunks: chunksTotal,
                                name: file.name,
                                type: uploadType,
                                ext: file.ext,
                                blobId: '',
                                userId: settings.params.userId,
                                tenatName: settings.params.tenatName
                            },
                            cache: false,
                            dataType: "json"
                        }).then(function (data, textStatus, jqXHR) {

                            //todo 检查响应是否正常

                            task.resolve();
                            file.path = data.path;
                            uploadComlate(file, data);

                        }, function (jqXHR, textStatus, errorThrown) {
                            task.reject();
                        });

                        return $.when(task);
                    } else {
                        //uploadComlate(file);
                    }
                }
            });

            uploader = WebUploader.create({
                // 不压缩image
                resize: false,
                // swf文件路径
                swf: settings.baseUrl + '/Uploader.swf',
                server: settings.postUrl
                // 选择文件的按钮。可选。
                // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                , pick: settings.btnAddFile,
                dnd: settings.fileList,
                paste: document.body,
                disableGlobalDnd: true
                , compress: false,
                prepareNextFile: true,
                chunked: true //开启分片上传
                , chunkSize: settings.chunkSize,
                threads: 10,
                formData: settings.params,
                fileNumLimit: settings.fileNumLimit,
                fileSingleSizeLimit: maxSize * 1024 * 1024,
                duplicate: false
                , accept: {
                    title: 'Images',
                    extensions: ext
                    //,mimeTypes: 'image/*'
                }
            });

            uploader.addButton({
                id: settings.btnContinueAddFile,
                label: '继续添加'
            });

            // 当有文件添加进来的时候
            uploader.on('fileQueued', function (file) {
                settings.callback.onFileQueued(file);
            });

            uploader.on('uploadBeforeSend', function (block, data) {
                // block为分块数据。

                // file为分块对应的file对象。
                var file = block.file;

                // 将存在file对象中的md5数据携带发送过去。
                data.md5 = filesMd5[file.name];
                data.uploadType = uploadType;
                data.ext = file.ext;

                // 删除其他数据
                // delete data.key;
            });

            // 文件上传过程中创建进度条实时显示。
            uploader.on('uploadProgress', function (file, percentage) {
                //percentage * 100 + '%'
                settings.callback.uploadProgress(file, percentage);
            });

            uploader.on('uploadSuccess', function (file, response) {
                //单个文件 已上传
                settings.callback.uploadSuccess(file, response);
            });

            uploader.on('uploadError', function (file, reason) {
                //上传出错
                settings.callback.uploadError(file, reason);
            });

            uploader.on('uploadComplete', function (file) {
                settings.callback.uploadComplete(file);
            });

            uploader.on('all', function (type) {
                if (type === 'startUpload') {
                    state = 'uploading';
                } else if (type === 'stopUpload') {
                    state = 'paused';
                } else if (type === 'uploadFinished') {
                    state = 'done';
                }

                //if (state === 'uploading') {
                //    //show 暂停上传
                //    settings.btnStartUpload.text('暂停上传');
                //} else {
                //    //show 开始上传
                //    settings.btnStartUpload.text('开始上传');
                //}
            });
            uploader.on('error', function (handler) {
                switch (handler) {
                    case "Q_EXCEED_NUM_LIMIT":
                        alert("超出允许最大上传数");
                        break;
                    case "F_DUPLICATE":
                        alert("文件重复");
                        break;
                    case "Q_TYPE_DENIED":
                        alert("文件类型不满足");
                        break;
                    case "F_EXCEED_SIZE":
                        alert("文件太大");
                        break;
                }
            });

            //settings.btnStartUpload.on('click', function () {
            //    if (state === 'uploading') {
            //        uploader.stop(true);
            //    } else {
            //        uploader.upload();
            //    }
            //});

            if (settings.fileList != null)
                settings.fileList.on("click", ".del", function() {
                    //移除某一文件, 默认只会标记文件状态为已取消，如果第二个参数为 true 则会从 queue 中移除。
                    //uploader.getFile(id)
                    uploader.removeFile(id);
                });

            function uploadComlate(file, data) {
                settings.callback.uploadSuccess(file, data);
            }

            return uploader;
        }
    };

})(jQuery);

/* demo
$(function () {
    $.fn.cfwinUploader.upload({
        baseUrl: '/js/cfw.webuploader/',
        postUrl: '/Home/UploadFileToTemp',
        btnAddFile:'',
        btnStartUpload: $('#ctlBtn'),
        fileList: $('#thelist'),
        type: 2,
        fileNumLimit: 20,
        params: { tenatName: '', blobId: 'DC394E7E-6652-4422-85F7-B560DFFA865F' },
        configure: {
            imageMaxSize: 15,
            imageExt: 'jpg,jpeg,png,gif,bmp,pdf',
            fileMaxSize: 1500,
            fileExt: 'txt,zip,rar,doc,docx,ppt,pptx,xls,xlsx,pdf'
        },
        callback: {
            uploadProgress: function (file, percentage) {
                $("#" + file.id + " .state").text((percentage * 100).toFixed(2) + "%");
            },
            uploadComplete: function (file) {//不管成功或者失败，文件上传完成时触发

            },
            uploadSuccess: function (file, response) {
                if (!response.success) {
                    //合并异常
                }
                console.log(response);
            },
            uploadError: function (file, reason) {

            }
        }
    });
});
*/

//cfwin上传图片组件
(function ($) {
    var $cfwinUploadSettings = {
        wrap: null,
        queueList:null,
        btnStartUpload: null,
        btnAddFile: null,
        btnContinueAddFile:null,
        baseUrl: '/js/cfw.webuploader/',
        postUrl: '/Home/UploadFileToTemp',
        fileSizeLimit: 30 * 1024 * 1024, //总图片大小
        fileSingleSizeLimit: 3 * 1024 * 1024,
        fileNumLimit: 10,
        type: 0, //0:image,1:file,2:all
        params: { tenatName: '',userId:''},
        callback: {
            uploadSuccess: function (file, response) {

            }
        }
    };

    $.fn.cfwinUploaderForImg = {
        upload: function (zSettings) {
            var settings = $cfwinUploadSettings;
            $.extend(true, settings, zSettings);

            var $wrap = $(settings.wrap),
        // 图片容器
        $queue = $('<ul class="filelist"></ul>').appendTo($wrap.find(settings.queueList)),
        // 状态栏，包括进度和控制按钮
        $statusBar = $wrap.find('.statusBar'),
        // 文件总体选择信息。
        $info = $statusBar.find('.info'),
        // 上传按钮
        $upload = $wrap.find(settings.btnStartUpload),
        // 没选择文件之前的内容。
        $placeHolder = $wrap.find('.placeholder'),
        // 总体进度条
        $progress = $statusBar.find('.progress').hide(),
        // 添加的文件数量
        fileCount = 0,
        // 添加的文件总大小
        fileSize = 0,
        // 优化retina, 在retina下这个值是2
        ratio = window.devicePixelRatio || 1,
        // 缩略图大小
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,
        // 可能有pedding, ready, uploading, confirm, done.
        state = 'pedding',
        // 所有文件的进度信息，key为file id
        percentages = {},
        supportTransition = (function () {
            var s = document.createElement('p').style,
                r = 'transition' in s ||
                        'WebkitTransition' in s ||
                        'MozTransition' in s ||
                        'msTransition' in s ||
                        'OTransition' in s;
            s = null;
            return r;
        })(),

        // WebUploader实例
        uploader;

            if (!WebUploader.Uploader.support()) {
                alert('Web Uploader 不支持您的浏览器！如果你使用的是IE浏览器，请尝试升级 flash 播放器');
                throw new Error('WebUploader does not support the browser you are using.');
            }

            // 实例化
            uploader = WebUploader.create({
                pick: {
                    id: settings.btnAddFile,
                    label: '点击选择图片'
                },
                dnd: settings.wrap + ' .queueList',
                paste: document.body,

                accept: {
                    title: 'Images',
                    extensions: 'gif,jpg,jpeg,bmp,png',
                    mimeTypes: 'image/*'
                },

                // swf文件路径
                swf: settings.baseUrl + '/Uploader.swf',

                disableGlobalDnd: true,

                chunked: true,
                server: settings.postUrl,
                fileNumLimit: settings.fileNumLimit,
                fileSizeLimit: settings.fileSizeLimit,   
                fileSingleSizeLimit: settings.fileSingleSizeLimit,   
                formData: {
                    uploadType: 'uploadimage',
                    tenatName: settings.params.tenatName,
                    userId: settings.params.userId
                }
            });

            // 添加“添加文件”的按钮，
            uploader.addButton({
                id: settings.btnContinueAddFile,
                label: '继续添加'
            });

            // 当有文件添加进来时执行，负责view的创建
            function addFile(file) {
                var $li = $('<li id="' + file.id + '">' +
                        '<p class="title">' + file.name + '</p>' +
                        '<p class="imgWrap"></p>' +
                        '<p class="progress"><span></span></p>' +
                        '</li>'),

                    $btns = $('<div class="file-panel">' +
                        '<span class="cancel">删除</span>' +
                        '<span class="rotateRight">向右旋转</span>' +
                        '<span class="rotateLeft">向左旋转</span></div>').appendTo($li),
                    $prgress = $li.find('p.progress span'),
                    $wrap = $li.find('p.imgWrap'),
                    $info = $('<p class="error"></p>'),

                    showError = function (code) {
                        switch (code) {
                            case 'exceed_size':
                                text = '文件大小超出';
                                break;

                            case 'interrupt':
                                text = '上传暂停';
                                break;

                            default:
                                text = '上传失败，请重试';
                                break;
                        }

                        $info.text(text).appendTo($li);
                    };

                if (file.getStatus() === 'invalid') {
                    showError(file.statusText);
                } else {

                    $wrap.text('预览中');
                    uploader.makeThumb(file, function (error, src) {
                        if (error) {
                            $wrap.text('不能预览');
                            return;
                        }

                        var img = $('<img src="' + src + '">');
                        $wrap.empty().append(img);
                    }, thumbnailWidth, thumbnailHeight);

                    percentages[file.id] = [file.size, 0];
                    file.rotation = 0;
                }

                file.on('statuschange', function (cur, prev) {
                    if (prev === 'progress') {
                        $prgress.hide().width(0);
                    } else if (prev === 'queued') {
                        $li.off('mouseenter mouseleave');
                        $btns.remove();
                    }

                    // 成功
                    if (cur === 'error' || cur === 'invalid') {
                        console.log(file.statusText);
                        showError(file.statusText);
                        percentages[file.id][1] = 1;
                    } else if (cur === 'interrupt') {
                        showError('interrupt');
                    } else if (cur === 'queued') {
                        percentages[file.id][1] = 0;
                    } else if (cur === 'progress') {
                        $info.remove();
                        $prgress.css('display', 'block');
                    } else if (cur === 'complete') {
                        $li.append('<span class="success"></span>');
                    }

                    $li.removeClass('state-' + prev).addClass('state-' + cur);
                });

                $li.on('mouseenter', function () {
                    $btns.stop().animate({ height: 30 });
                });

                $li.on('mouseleave', function () {
                    $btns.stop().animate({ height: 0 });
                });

                $btns.on('click', 'span', function () {
                    var index = $(this).index(),
                        deg;

                    switch (index) {
                        case 0:
                            uploader.removeFile(file);
                            return;

                        case 1:
                            file.rotation += 90;
                            break;

                        case 2:
                            file.rotation -= 90;
                            break;
                    }

                    if (supportTransition) {
                        deg = 'rotate(' + file.rotation + 'deg)';
                        $wrap.css({
                            '-webkit-transform': deg,
                            '-mos-transform': deg,
                            '-o-transform': deg,
                            'transform': deg
                        });
                    } else {
                        $wrap.css('filter', 'progid:DXImageTransform.Microsoft.BasicImage(rotation=' + (~~((file.rotation / 90) % 4 + 4) % 4) + ')');
                        //use jquery animate to rotation
                        $({
                            rotation: rotation
                        }).animate({
                            rotation: file.rotation
                        }, {
                            easing: 'linear',
                            step: function( now ) {
                                now = now * Math.PI / 180;

                                var cos = Math.cos( now ),
                                    sin = Math.sin( now );

                                $wrap.css( 'filter', "progid:DXImageTransform.Microsoft.Matrix(M11=" + cos + ",M12=" + (-sin) + ",M21=" + sin + ",M22=" + cos + ",SizingMethod='auto expand')");
                            }
                        });
                    }


                });

                $li.appendTo($queue);
            }

            // 负责view的销毁
            function removeFile(file) {
                var $li = $('#' + file.id);

                delete percentages[file.id];
                updateTotalProgress();
                $li.off().find('.file-panel').off().end().remove();
            }

            function updateTotalProgress() {
                var loaded = 0,
                    total = 0,
                    spans = $progress.children(),
                    percent;

                $.each(percentages, function (k, v) {
                    total += v[0];
                    loaded += v[0] * v[1];
                });

                percent = total ? loaded / total : 0;

                spans.eq(0).text(Math.round(percent * 100) + '%');
                spans.eq(1).css('width', Math.round(percent * 100) + '%');
                updateStatus();
            }

            function updateStatus() {
                var text = '', stats;

                if (state === 'ready') {
                    text = '选中' + fileCount + '张图片，共' +
                            WebUploader.formatSize(fileSize) + '。';
                } else if (state === 'confirm') {
                    stats = uploader.getStats();
                    if (stats.uploadFailNum) {
                        text = '已成功上传' + stats.successNum + '张照片至XX相册，' +
                            stats.uploadFailNum + '张照片上传失败，<a class="retry" href="#">重新上传</a>失败图片或<a class="ignore" href="#">忽略</a>'
                    }

                } else {
                    stats = uploader.getStats();
                    text = '共' + fileCount + '张（' +
                            WebUploader.formatSize(fileSize) +
                            '），已上传' + stats.successNum + '张';

                    if (stats.uploadFailNum) {
                        text += '，失败' + stats.uploadFailNum + '张';
                    }
                }

                $info.html(text);
            }

            function setState(val) {
                var file, stats;

                if (val === state) {
                    return;
                }

                $upload.removeClass('state-' + state);
                $upload.addClass('state-' + val);
                state = val;

                switch (state) {
                    case 'pedding':
                        $placeHolder.removeClass('element-invisible');
                        $queue.parent().removeClass('filled');
                        $queue.hide();
                        $statusBar.addClass('element-invisible');
                        uploader.refresh();
                        break;

                    case 'ready':
                        $placeHolder.addClass('element-invisible');
                        $(settings.btnContinueAddFile).removeClass('element-invisible');
                        $queue.parent().addClass('filled');
                        $queue.show();
                        $statusBar.removeClass('element-invisible');
                        uploader.refresh();
                        break;

                    case 'uploading':
                        $(settings.btnContinueAddFile).addClass('element-invisible');
                        $progress.show();
                        $upload.text('暂停上传');
                        break;

                    case 'paused':
                        $progress.show();
                        $upload.text('继续上传');
                        break;

                    case 'confirm':
                        $progress.hide();
                        $upload.text('开始上传').addClass('disabled');

                        stats = uploader.getStats();
                        if (stats.successNum && !stats.uploadFailNum) {
                            setState('finish');
                            return;
                        }
                        break;
                    case 'finish':
                        stats = uploader.getStats();
                        if (stats.successNum) {
                            alert('上传成功');
                        } else {
                            // 没有成功的图片，重设
                            state = 'done';
                            location.reload();
                        }
                        break;
                }

                updateStatus();
            }

            uploader.onUploadProgress = function (file, percentage) {
                var $li = $('#' + file.id),
                    $percent = $li.find('.progress span');

                $percent.css('width', percentage * 100 + '%');
                percentages[file.id][1] = percentage;
                updateTotalProgress();
            };

            uploader.onFileQueued = function (file) {
                fileCount++;
                fileSize += file.size;

                if (fileCount === 1) {
                    $placeHolder.addClass('element-invisible');
                    $statusBar.show();
                }

                addFile(file);
                setState('ready');
                updateTotalProgress();
            };

            uploader.onFileDequeued = function (file) {
                fileCount--;
                fileSize -= file.size;

                if (!fileCount) {
                    setState('pedding');
                }

                removeFile(file);
                updateTotalProgress();

            };

            uploader.onUploadBeforeSend = function (object, data, headers) {
                data.ext = object.file.ext;
            };

            uploader.on('all', function (type) {
                var stats;
                switch (type) {
                    case 'uploadFinished':
                        setState('confirm');
                        break;

                    case 'startUpload':
                        setState('uploading');
                        break;

                    case 'stopUpload':
                        setState('paused');
                        break;

                }
            });

            uploader.on('uploadSuccess', function (file, response) {
                settings.callback.uploadSuccess(file, response);
            });

            uploader.onError = function (code) {
                alert('Error: ' + code);
            };

            $upload.on('click', function () {
                if ($(this).hasClass('disabled')) {
                    return false;
                }

                if (state === 'ready') {
                    uploader.upload();
                } else if (state === 'paused') {
                    uploader.upload();
                } else if (state === 'uploading') {
                    uploader.stop();
                }
            });

            $info.on('click', '.retry', function () {
                uploader.retry();
            });

            $upload.addClass('state-' + state);
            updateTotalProgress();
            return uploader;
        }
    };
})(jQuery);

function checkGuidelines(pageName,fn) {
    $.get('/Home/ExistsGuidelines',
        { pageName: pageName },
        function(data) {
            if (data) {
                if (typeof (fn) == 'function') {
                    fn(data.result);
                }
            }
        });
}

function getIntroOptions() {
    return {
        prevLabel: "上一步",
        nextLabel: "下一步",
        skipLabel: "跳过",
        doneLabel: "立即体验",
        showBullets: false,
        showStepNumbers: false,
        disableInteraction: true,
        exitOnOverlayClick: false,
        exitOnEsc: false,
        tooltipClass: 'cfwin-intro'
    };
}

function addGuidelines(pageName) {
    $.post('/MainPage/AddGuidelines',{ pageName: pageName });
}

function getIntroContent(title,content) {
    return '<div class="introDiv"><span class="inteo-title">'+title+'<a href="javascript:closeIntro()" class="intro-close">X</a></span><div class="inteo-content">'+content+'</div></div>';
}

function closeIntro() {
    intro.exit();
}


var filesMd5 = {};
function cfwinUploader(option) {
    var $cfwinUploadSettings = {
        componentName: 'uploader',
        btnStartUpload: null,
        btnAddFile: null,
        btnContinueAddFile: null,
        fileList: null,
        fileStorage: null,
        baseUrl: '/js/cfw.webuploader/',
        postUrl: '/Home/UploadFileToTemp',
        chunkCheckUrl: '/Home/ChunkCheck',
        chunksMergeUrl: '/Home/ChunksMerge',
        chunkSize: 3 * 1024 * 1024, //分块大小
        fileNumLimit: 10,
        type: 0, //0:image,1:file,2:all
        params: { userId: '', blobId: '', isWatermake: true },
        isRegister: false,
        disableWidgets: [],
        configure: {
            imageMaxSize: 0,
            imageExt: '',
            fileMaxSize: 0,
            fileExt: ''
        },
        callback: {
            uploadProgress: function (file, percentage) {
            },
            uploadComplete: function (file) {//不管成功或者失败，文件上传完成时触发
            },
            uploadSuccess: function (file, response) {
            },
            uploadError: function (file, reason) {
            },
            onFileQueued: function (file) {
            }
        }
    };

    var settings = $cfwinUploadSettings;
    $.extend(true, settings, option);
    var maxSize = 0, ext = '', state = 'pending', uploadType;
    switch ($cfwinUploadSettings.type) {
        case 0:
            maxSize = settings.configure.imageMaxSize;
            ext = settings.configure.imageExt;
            uploadType = 'uploadimage';
            break;
        case 1:
            maxSize = settings.configure.fileMaxSize;
            ext = settings.configure.fileExt;
            uploadType = 'uploadfile';
            break;
        default:
            maxSize = settings.configure.fileMaxSize;
            ext = settings.configure.imageExt + ',' + settings.configure.fileExt;
            uploadType = '';
            break;
    }

    if (settings.isRegister) {
        WebUploader.Uploader.register({
            "before-send-file": "beforeSendFile",
            "before-send": "beforeSend",
            "after-send-file": "afterSendFile",
            'name': settings.componentName
        }, {
                beforeSendFile: function (file) {
                    var task = new $.Deferred();
                    var start = new Date().getTime();
                    (new WebUploader.Uploader()).md5File(file, 0, 10 * 1024 * 1024).progress(function (percentage) {
                        console.log(percentage);
                    }).then(function (val) {
                        console.log("总耗时: " + ((new Date().getTime()) - start) / 1000);

                        task.resolve();
                        //拿到上传文件的唯一名称，用于断点续传
                        filesMd5[file.name] = md5('' + settings.params.userId + file.name + file.type + file.lastModifiedDate + file.size);
                    });
                    return $.when(task);
                },
                beforeSend: function (block) {
                    //分片验证是否已传过，用于断点续传
                    var task = new $.Deferred();
                    //不需要分片上传
                    if (block.chunks == 1) {
                        task.resolve();
                    } else {
                        $.ajax({
                            type: "POST",
                            url: settings.chunkCheckUrl,
                            data: {
                                name: filesMd5[block.file.name],
                                chunkIndex: block.chunk,
                                size: block.end - block.start
                            },
                            cache: false,
                            timeout: 10000 //todo 超时的话，只能认为该分片未上传过
                            ,
                            dataType: "json"
                        }).then(function (data, textStatus, jqXHR) {
                            if (data.ifExist) { //若存在，返回失败给WebUploader，表明该分块不需要上传
                                task.reject();
                            } else {
                                task.resolve();
                            }
                        }, function (jqXHR, textStatus, errorThrown) { //任何形式的验证失败，都触发重新上传
                            task.resolve();
                        });
                    }

                    return $.when(task);
                },
                afterSendFile: function (file) {
                    var chunksTotal = Math.ceil(file.size / settings.chunkSize);
                    if (chunksTotal > 1) {
                        //合并请求
                        var task = new $.Deferred();
                        $.ajax({
                            type: "POST",
                            url: settings.chunksMergeUrl,
                            data: {
                                folder: filesMd5[file.name],
                                chunks: chunksTotal,
                                name: file.name,
                                type: uploadType,
                                ext: file.ext,
                                blobId: settings.componentName.options.formData.blobId,
                                userId: settings.params.userId
                            },
                            cache: false,
                            dataType: "json"
                        }).then(function (data, textStatus, jqXHR) {
                            //todo 检查响应是否正常

                            task.reject();
                            file.path = data.path;
                            uploadComlate(file, data);
                        }, function (jqXHR, textStatus, errorThrown) {
                            task.reject();
                        });

                        return $.when(task);
                    } else {
                        //uploadComlate(file);
                    }
                }
            });
    }
    settings.componentName = WebUploader.create({
        // 不压缩image
        resize: false,
        // swf文件路径
        swf: settings.baseUrl + '/Uploader.swf',
        server: settings.postUrl
        // 选择文件的按钮。可选。
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        , pick: settings.btnAddFile,
        dnd: settings.fileList,
        paste: document.body,
        disableGlobalDnd: true
        , compress: false,
        prepareNextFile: true,
        chunked: true //开启分片上传
        , chunkSize: settings.chunkSize,
        threads: 1,
        formData: settings.params,
        fileNumLimit: settings.fileNumLimit,
        fileSingleSizeLimit: maxSize * 1024 * 1024,
        duplicate: false
        , accept: {
            title: 'Images',
            extensions: ext
            //,mimeTypes: 'image/*'
        }, disableWidgets: settings.disableWidgets
    });

    settings.componentName.addButton({
        id: settings.btnContinueAddFile,
        label: '继续添加'
    });

    // 当有文件添加进来的时候
    settings.componentName.on('fileQueued', function (file) {

        settings.callback.onFileQueued(file);
    });

    settings.componentName.on('beforeFileQueued', function (file) {
        if (settings.fileStorage && settings.fileNumLimit) {
            if (settings.fileStorage.children().length + settings.componentName.getStats().queueNum >= settings.fileNumLimit) {
                alert('最多只能上传' + settings.fileNumLimit + '个文件');
                return false;
            }
        }
        return true;
    });


    settings.componentName.on('uploadBeforeSend', function (block, data) {
        // block为分块数据。

        // file为分块对应的file对象。
        var file = block.file;

        // 将存在file对象中的md5数据携带发送过去。
        data.md5 = filesMd5[file.name];
        data.uploadType = uploadType;
        data.ext = file.ext;

        // 删除其他数据
        // delete data.key;
    });

    // 文件上传过程中创建进度条实时显示。
    settings.componentName.on('uploadProgress', function (file, percentage) {
        //percentage * 100 + '%'
        settings.callback.uploadProgress(file, percentage);
    });

    settings.componentName.on('uploadSuccess', function (file, response) {
        //单个文件 已上传
        settings.callback.uploadSuccess(file, response);
    });

    settings.componentName.on('uploadError', function (file, reason) {
        //上传出错
        settings.callback.uploadError(file, reason);
    });

    settings.componentName.on('uploadComplete', function (file) {
        settings.callback.uploadComplete(file);
    });

    settings.componentName.on('all', function (type) {
        if (type === 'startUpload') {
            state = 'uploading';
        } else if (type === 'stopUpload') {
            state = 'paused';
        } else if (type === 'uploadFinished') {
            state = 'done';
        }
    });
    settings.componentName.on('error', function (handler) {
        switch (handler) {
            case "Q_EXCEED_NUM_LIMIT":
                alert("超出允许最大上传数");
                break;
            case "F_DUPLICATE":
                alert("文件重复");
                break;
            case "Q_TYPE_DENIED":
                alert("文件类型不满足");
                break;
            case "F_EXCEED_SIZE":
                alert("文件太大了");
                break;
        }
    });
    //});
    function uploadComlate(file, data) {
        settings.callback.uploadSuccess(file, data);
    }

    return settings.componentName;
}