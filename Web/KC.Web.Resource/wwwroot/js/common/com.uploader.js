//对应后台的类型：FileType、DocFormat、ImageFormat、AudioFormat、VideoFormat;类型转换：MimeTypeHelper
const defaultImageExt = 'jpg,jpeg,png,gif,bmp,tiff,wmf,dwg';
const defaultImageMimeTypes = 'image/jpeg,image/png,image/gif,image/bmp,image/tiff,image/wmf,image/vnd.dwg';
const defaultAudioExt = 'au,wav,ram,rmi,aif,mp3';
const defaultAudioMimeTypes = 'audio/x-wav,audio/mpeg,audio/x-pn-realaudio,audio/mid,audio/x-aiff';
const defaultVideoExt = 'wav,mp4,flv,avi,mov';
const defaultVideoMimeTypes = 'video/x-ms-wmv,video/mp4,video/x-flv,video/x-msvideo,video/quicktime,';
const defaultDocExt = 'txt,doc,docx,xls,xlsx,ppt,pptx,pdf,zip,rar,gzip';
const defaultDocMimeTypes = 'text/plain,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,' +
    'application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,' +
    'application/vnd.ms-powerpoint,application/vnd.openxmlformats-officedocument.presentationml.presentation,' +
    'application/pdf,application/zip,application/x-rar-compressed,application/x-gzip';
const defaultBaseWebUploaderUrl = '/js/webuploader/';
const defaultPostWebUploaderUrl = '/Home/Upload';
const defaultChunkCheckUrl = '/Home/ChunkCheck';
const defaultChunksMergeUrl = '/Home/ChunksMerge';
const defaultShowImageUrl = '/Home/ShowImage?id=';
const defaultDownloadFileUrl = '/Home/DownloadFile?id=';
const defaultFileNumLimit = 1;
const defaultChunkSize = 30 * 1024 * 1024;
const defaultFileSizeLimit = 30 * 1024 * 1024; //总图片大小
const defaultFileSingleSizeLimit = 30 * 1024 * 1024;


/**
 * 图片上传图片组件
 <div class="imageUploaderEditor">
     <div style="display: flex;height: 110px;">
         <div id="imageUploader" class="webuploader-container">
            <a href="javascript:void(0)"><i class="fa fa-plus-square fa-5x"></i></a>
         </div>
        <div id="imageList" class="webUploader-image-list"></div>
     </div>
     <p id="imagePreviewContainer" class="imagePreview"></p>
     <label style="padding: 3px;">文件个数：3个；文件格式：[[${uploadConfig.fileExt}]]；文件大小：[[${uploadConfig.fileMaxSize}]]M</label>
 </div>

 let imageSize = [[${uploadConfig.imageMaxSize}]];
 let imageExt = '[[${uploadConfig.imageExt}]]';
 let fileMaxSize = [[${uploadConfig.fileMaxSize}]];
 let fileExt = '[[${uploadConfig.fileExt}]]';
 let imageWebUploader = null;
 let imageBlobJsonString = '[[${fileBlobJsonString}]]';
 let defaultInitImageUploader = function () {
        let blobId = $('#hiddenImageBlobId').val();
        imageWebUploader = imageUploader({
            btnAddFile: '#imageUploader',
            fileList: '#imageList',
            fileNumLimit: 3,
            showImageUrl: '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/ShowImage?id=',
            downloadFileUrl: '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/DownloadFile?id=',
            params: {blobId: blobId, isArchive: true}, //单文件时可传入文件BlobId，用于覆盖相关文件
            isRegister: true,
            isEditView: false, //显示图片是否可编辑
            configure: {
                imageMaxSize: imageSize,
                imageExt: imageExt,
                fileMaxSize: fileMaxSize,
                fileExt: fileExt
            },
            callback: {
                uploadProgress: function (file, percentage) {
                },
                uploadComplete: function (file) { //不管成功或者失败，文件上传完成时触发
                },
                uploadSuccess: function (file, response, blob) {
                },
                uploadError: function (file, reason) {
                },
                onFileQueued: function (file) {
                }
            }
        });

        //根据已有图片数据，显示图片
        if (imageBlobJsonString !== undefined && imageBlobJsonString !== '') {
            let objString = CommonUtil.CommonUtil.escape2Html(imageBlobJsonString);
            let imageBlob = JSON.parse(objString);
            imageWebUploader.initFileView([imageBlob]);
        } else {
            imageWebUploader.initFileView([]);
        }
    };
 * @param {any} option
 */
let imageUploader = function (option) {
    let token = $("meta[name='_csrf']").attr("content");
    let header = $("meta[name='_csrf_header']").attr("content");
    let data = { tenantName: '', userId: '', blobId: '', isWaterMake: false };
    if (token !== undefined && token != null && token !== "") {
        data = { _csrf: token, tenantName: '', userId: '', blobId: '', isWaterMake: false }
    }

    let uploadType = 'uploadimage';
    let settings = {
        componentName: 'imageUploader',
        imagePreviewContainer: null,
        btnStartUpload: null,
        btnAddFile: null,
        btnContinueAddFile: null,
        type: 0, //0:image,1:file,2:audio,3:video,4:all
        fileList: null,
        fileStorage: null,
        fileNumLimit: defaultFileNumLimit,
        fileSizeLimit: defaultFileSizeLimit,
        fileSingleSizeLimit: defaultFileSingleSizeLimit,
        baseUrl: defaultBaseWebUploaderUrl,
        postUrl: defaultPostWebUploaderUrl + "?parm=" + uploadType,
        showImageUrl: defaultShowImageUrl,
        downloadFileUrl: defaultDownloadFileUrl,
        chunkCheckUrl: defaultChunkCheckUrl,
        chunksMergeUrl: defaultChunksMergeUrl,
        chunkSize: defaultChunkSize, //分块大小
        params: data,
        isRegister: false,
        isSetDefault: false,
        isEditor: false,
        isEditView: true,
        disableWidgets: [],
        configure: {
            imageMaxSize: 10,
            imageExt: defaultImageExt,
            fileMaxSize: 10,
            fileExt: defaultDocExt + ',' + defaultImageExt
        },
        callback: {
            onFileQueued: function (file) {
            },
            uploadStart: function (file) {
            },
            uploadProgress: function (file, percentage) {
            },
            uploadComplete: function (file) {//不管成功或者失败，文件上传完成时触发
            },
            uploadSuccess: function (file, response) {
            },
            uploadError: function (file, reason) {
            },
            onRemoveImage: function (file) {
            }
        }
    };
    $.extend(true, settings, option);

    let uploader = null;
    let filesMd5 = {};
    let state = 'pending';
    let fileCount = 0;
    let imageUploadId = settings.componentName;
    let maxSize = settings.configure.imageMaxSize != null && settings.configure.imageMaxSize !== 0
        ? settings.configure.imageMaxSize
        : 10;
    let ext = settings.configure.imageExt != null && settings.configure.imageExt !== ''
        ? settings.configure.imageExt
        : defaultImageExt;
    //console.info('------imageUploader accept ext: ' + ext + ' and limit image size: ' + maxSize);

    if (settings.isRegister) {
        WebUploader.Uploader.register({
            "before-send-file": "beforeSendFile",
            "before-send": "beforeSend",
            "after-send-file": "afterSendFile",
            'name': imageUploadId
        }, {
            beforeSendFile: function (file) {
                let task = new $.Deferred();
                let start = new Date().getTime();
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
                let task = new $.Deferred();
                //不需要分片上传
                if (block.chunks === 1) {
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
                let chunksTotal = Math.ceil(file.size / settings.chunkSize);
                if (chunksTotal > 1) {
                    //合并请求
                    let task = new $.Deferred();
                    $.ajax({
                        type: "POST",
                        url: settings.chunksMergeUrl,
                        data: {
                            folder: filesMd5[file.name],
                            chunks: chunksTotal,
                            name: file.name,
                            type: uploadType,
                            ext: file.ext,
                            tenantName: settings.params.tenantName,
                            blobId: settings.params.blobId,
                            userId: settings.params.userId,
                            isArchive: settings.params.isArchive
                        },
                        cache: false,
                        dataType: "json"
                    }).then(function (data, textStatus, jqXHR) {
                        //todo 检查响应是否正常
                        if (data && data.success) {
                            file.path = data.path;
                            uploadCompleted(file, data);

                            hiddenFileProgress(file);

                            uploader.removeFile(file);

                            settings.callback.uploadComplete(file);
                        } else {
                            task.reject();
                        }
                    }, function (jqXHR, textStatus, errorThrown) {
                        task.reject();
                    });

                    return $.when(task);
                } else {
                    //uploadCompleted(file);
                }
            }
        });
    }
    uploader = WebUploader.create({
        // 不压缩image
        resize: false,
        // swf文件路径
        swf: settings.baseUrl + '/Uploader.swf',
        server: settings.postUrl,
        // 选择文件的按钮。可选。
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        pick: settings.btnAddFile,
        dnd: settings.fileList,
        paste: document.body,
        threads: 1,     //上传并发数
        chunked: true,  //开启分片上传,
        chunkSize: settings.chunkSize,
        formData: settings.params,
        fileNumLimit: settings.fileNumLimit,
        fileSizeLimit: settings.fileSizeLimit,
        fileSingleSizeLimit: settings.fileSingleSizeLimit,
        disableWidgets: settings.disableWidgets,
        disableGlobalDnd: true,
        compress: false,
        prepareNextFile: true,
        duplicate: false,
        accept: {
            title: '图片类型',
            extensions: ext,
            mimeTypes: defaultImageMimeTypes
        }
    });

    if (settings.btnContinueAddFile) {
        uploader.addButton({
            id: settings.btnContinueAddFile,
            label: '继续添加'
        });
    }

    // 当文件被加入队列以后触发
    uploader.on('fileQueued', function (file) {
        appendImageView(file);

        uploader.upload();

        settings.callback.onFileQueued(file);
    });
    // 当文件被加入队列之前触发，此事件的handler返回值为false，则此文件不会被添加进入队列
    uploader.on('beforeFileQueued', function (file) {
        if (settings.fileNumLimit <= fileCount) {
            $.messager.showInfoTopCenter('系统提示', '最多只能上传' + settings.fileNumLimit + '个文件', 1000);
            return false;
        }

        let fileExt = file.ext;
        if (ext.indexOf(fileExt) < 0) {
            $.messager.showInfoTopCenter('系统提示', '上传的文件类型不正确', 1000);
            return false;
        }
        if (settings.fileStorage && settings.fileNumLimit) {
            if (settings.fileStorage.children().length + uploader.getStats().queueNum >= settings.fileNumLimit) {
                //alert('最多只能上传' + settings.fileNumLimit + '个文件');
                $.messager.showInfoTopCenter('系统提示', '最多只能上传' + settings.fileNumLimit + '个文件', 1000);
                return false;
            }
        }
        return true;
    });
    // 当某个文件的分块在发送前触发，主要用来询问是否要添加附带参数，大文件在开起分片上传的前提下此事件可能会触发多次
    uploader.on('uploadBeforeSend', function (block, data) {
        // block为分块数据。

        // file为分块对应的file对象。
        let file = block.file;

        // 将存在file对象中的md5数据携带发送过去。
        data.md5 = filesMd5[file.name];
        data.uploadType = uploadType;
        data.ext = file.ext;

        // 删除其他数据
        // delete data.key;
    });
    // 某个文件开始上传前触发，一个文件只会触发一次。
    uploader.on('uploadStart', function (file) {
        settings.callback.uploadStart(file);
    });
    // 文件上传过程中创建进度条实时显示。
    uploader.on('uploadProgress', function (file, percentage) {
        setImageProgressValue(file, percentage);
        //percentage * 100 + '%'
        settings.callback.uploadProgress(file, percentage);
    });
    // 当文件上传成功时触发
    uploader.on('uploadSuccess', function (file, response) {
        uploadCompleted(file, response);
    });
    // 当文件上传出错时触发
    uploader.on('uploadError', function (file, reason) {
        removeImageView(file);
        //上传出错
        settings.callback.uploadError(file, reason);
    });
    // 不管成功或者失败，文件上传完成时触发
    uploader.on('uploadComplete', function (file) {
        hiddenImageProgress(file);

        uploader.removeFile(file);

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
    });
    uploader.on('error', function (handler) {
        switch (handler) {
            case "Q_EXCEED_NUM_LIMIT":
                //alert("超出允许最大上传数");
                $.messager.showInfoTopCenter('系统提示', '超出允许最大上传数', 1000);
                break;
            case "F_DUPLICATE":
                //alert("文件重复");
                $.messager.showInfoTopCenter('系统提示', '文件重复', 1000);
                break;
            case "Q_TYPE_DENIED":
                //alert("文件类型不满足");
                $.messager.showInfoTopCenter('系统提示', '文件类型不正确', 1000);
                break;
            case "F_EXCEED_SIZE":
                //alert("文件太大了");
                $.messager.showInfoTopCenter('系统提示', '文件太大了', 1000);
                break;
        }
    });

    // 添加上传图片的前端视图
    let appendImageView = function (file) {
        let appendDiv = $(settings.fileList);
        if (appendDiv === undefined || appendDiv == null)
            return;

        //对应后台数据结构：BlobInfoDTO
        let fileId = file.id;
        let isSelect = file.isSelect !== undefined && file.isSelect != null ? file.isSelect : false;
        let isEditView = settings.isEditView !== undefined && settings.isEditView != null ? settings.isEditView : true;

        let blob = convertFileToBlob(file);
        let propertyId = blob.propertyId !== undefined && blob.propertyId != null ? blob.propertyId : '';
        file.propertyId = propertyId;

        //隐藏Input.imageBlobId控件保存上传控件后的值：blobId: [attr=value],blobName: [attr=title],isSelect: [attr=checked]
        let divFile = imageUploader.getImageListHtmlView(blob, isEditView, settings.isSetDefault, settings.componentName);
        appendDiv.append(divFile);
        //$('#file-' + fileId).tooltip({position: 'bottom'});
        $('#close-' + fileId).click(function () {
            removeImageView(file);
        });
        $('#progress-' + fileId).progressbar();
        if (settings.isSetDefault) {
            $('#rdo-' + fileId).radiobutton({
                check: isSelect,
                onChange: function () {
                    //Radio选中后，将对应存储BlobId的隐藏Input的属性设置为checked
                    let rdoSetDefaults = $(settings.fileList).find('input.imageBlobId');
                    let selectBlobId = $('.setDefault .radiobutton.radiobutton-checked .radiobutton-value').val();
                    $.each(rdoSetDefaults, function (i, rdoSetDefault) {
                        let blobId = $(rdoSetDefault).val();
                        if (selectBlobId === blobId) {
                            $(rdoSetDefault).attr("checked", 'checked');
                        } else {
                            $(rdoSetDefault).removeAttr("checked");
                        }
                    });
                }
            });
        }
    };

    //将file对象转换为Blob对象
    let convertBlobToFile = function (blob) {
        let file = {};
        file.id = blob.blobId;
        file.name = blob.blobName;
        file.ext = blob.ext;
        file.size = blob.size;
        file.downloadFileUrl = blob.downloadFileUrl !== undefined && blob.downloadFileUrl != null ? blob.downloadFileUrl : settings.downloadFileUrl + blob.blobId;
        file.showImageUrl = blob.showImageUrl !== undefined && blob.showImageUrl != null ? blob.showImageUrl : settings.showImageUrl + blob.blobId;
        file.propertyId = blob.propertyId !== undefined && blob.propertyId != null ? blob.propertyId : '';
        file.isSelect = blob.isSelect !== undefined && blob.isSelect != null ? blob.isSelect : false;
        return file;
    };
    //将file对象转换为Blob对象
    let convertFileToBlob = function (file) {
        let blob = {};
        blob.blobId = file.id;
        blob.blobName = file.name;
        blob.ext = file.ext;
        blob.size = file.size;
        blob.downloadFileUrl = file.downloadFileUrl !== undefined && file.downloadFileUrl != null ? file.downloadFileUrl : settings.downloadFileUrl + blob.blobId;
        blob.showImageUrl = file.showImageUrl !== undefined && file.showImageUrl != null ? file.showImageUrl : settings.showImageUrl + blob.blobId;
        blob.propertyId = file.propertyId !== undefined && file.propertyId != null ? file.propertyId : '';
        blob.isSelect = file.isSelect !== undefined && file.isSelect != null ? file.isSelect : false;
        return blob;
    };
    // 上传图片完成后，给相关的视图赋值
    let setImageViewValue = function (file, blobId) {
        if (settings.fileList === undefined || settings.fileList == null)
            return;

        //对应后台数据结构：BlobInfoDTO
        let fileId = file.id;
        let fileName = file.name;
        let fileExt = file.ext;
        let fileSize = file.size;
        let propertyId = file.propertyId !== undefined && file.propertyId != null ? file.propertyId : '';
        let isSelect = file.isSelect !== undefined && file.isSelect != null ? file.isSelect : false;
        let imageUrl = file.showImageUrl !== undefined && file.showImageUrl != null ? file.showImageUrl : settings.showImageUrl + blobId;

        const imgId = '#img-' + fileId;
        const txtId = '#txt-' + fileId;
        const rdoId = '#rdo-' + fileId;
        $(imgId).attr('src', imageUrl + "&" + Math.random());
        $(imgId).hover(function (event) {
            let e = event || window.event;
            let scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
            let scrollY = document.documentElement.scrollTop || document.body.scrollTop;
            let x = e.pageX || e.clientX + scrollX;
            let y = e.pageY || e.clientY + scrollY;

            // if (settings.isEditor !== undefined && settings.isEditor != null && settings.isEditor){
            //     let actualLeft = getOffsetLeftByBody(this);
            //     let actualTop = getOffsetTopByBody(this);
            //
            //     x = actualLeft - 200; //需要减去左侧菜单的宽度：200
            //     y = actualTop - 126; //需要减去上方功能菜单高度：43，MainTab页高度：39+3(height+padding)，SubTab页的高度：38+3(height+padding)
            // }
            //console.info("x: " + x + ", y: " + y);
            //console.info("actualLeft: " + actualLeft + ", actualTop: " + actualTop);

            let imagePreview = $(".imagePreview");
            if (settings.imagePreviewContainer !== undefined
                && settings.imagePreviewContainer != null
                && settings.imagePreviewContainer !== '') {
                imagePreview = $(settings.imagePreviewContainer);
            }
            imagePreview.css("top", (y + 2) + "px")
                .css("left", (x + 2) + "px")
                .html("<img style='max-width:400px;overflow:hidden;' src=" + $(this).attr("src") + " alt='" + $(this).attr("alt") + "' /><br/>" + $(this).attr("alt"))
                .fadeIn("slow");
        }, function () {
            let imagePreview = $(".imagePreview");
            if (settings.imagePreviewContainer !== undefined
                && settings.imagePreviewContainer != null
                && settings.imagePreviewContainer !== '') {
                imagePreview = $(settings.imagePreviewContainer);
            }
            imagePreview.fadeOut("fast");
        });

        $(txtId).val(blobId);//隐藏Input，用于保持BlobId及是否选中IsSelect属性
        $(txtId).attr('title', fileName);
        $(txtId).attr('ext', fileExt);
        $(txtId).attr('size', fileSize);
        $(txtId).attr('propertyId', propertyId);
        //settings.params.blobId = blobId;
        if (settings.isSetDefault) {
            $(rdoId).radiobutton('setValue', blobId);
            if (isSelect) {
                $(rdoId).radiobutton('check');
                $(txtId).attr('checked', 'checked');
            }
        }
    };
    let getOffsetTopByBody = function (el) {
        let offsetTop = 0;
        while (el && el.tagName !== 'BODY') {
            offsetTop += el.offsetTop;
            el = el.offsetParent;
        }
        return offsetTop
    };
    let getOffsetLeftByBody = function (el) {
        let offsetLeft = 0;
        while (el && el.tagName !== 'BODY') {
            offsetLeft += el.offsetLeft;
            el = el.offsetParent
        }
        return offsetLeft;
    };
    // 设置文件上传进度条
    let setImageProgressValue = function (file, percentage) {
        if (settings.fileList === undefined || settings.fileList == null)
            return;

        let fileId = file.id;
        if (percentage < 100) {
            $('#progress-' + fileId).progressbar('setValue', parseInt(percentage * 100));
        } else {
            $('#progress-' + fileId).hide();
        }
    };
    // 隐藏上传进度条
    let hiddenImageProgress = function (file) {
        if (settings.fileList === undefined || settings.fileList == null)
            return;

        let fileId = file.id;
        $('#progress-' + fileId).hide();
    };
    // 删除图片的前端视图
    let removeImageView = function (file) {
        if (settings.fileList === undefined || settings.fileList == null)
            return;

        let divFileId = '#file-' + file.id;
        //$(divFileId).tooltip('destroy');

        if (settings.callback !== undefined && settings.callback != null
            && settings.callback.onRemoveImage !== uploader && settings.callback.onRemoveImage != null) {
            //如果回调函数（onRemoveImage）有返回值（删除是否成功），如果为false时，前端页面不做处理
            //如果回调函数涉及异步操作时，isDelete值为：undefined，视同为true
            let isDelete = settings.callback.onRemoveImage(file);
            if (isDelete !== undefined && isDelete != null) {
                if (!isDelete)
                    return;
            }
        }

        //删除已经选中的image时，设置第一张图片为默认封面
        if (settings.isSetDefault) {
            //debugger;
            let isChecked = $('#rdo-' + file.id).radiobutton("options").checked;
            if (isChecked) {
                let divImages = $(divFileId).parent().siblings();
                if (divImages !== undefined && divImages != null && divImages.length > 0) {
                    let firstDivImages = divImages[0];
                    let rdoDefault = $(firstDivImages).find('.rdoSetDefault');
                    if (rdoDefault !== undefined && rdoDefault != null && rdoDefault.length > 0) {
                        $(rdoDefault).radiobutton("check");
                    }
                    let txtBlob = $(firstDivImages).find('input.imageBlobId');
                    if (txtBlob !== undefined && txtBlob != null && txtBlob.length > 0) {
                        $(txtBlob).attr('checked', 'checked');
                    }
                }
            }
        }

        $(divFileId).parent().remove();

        fileCount--;
    };
    let uploadCompleted = function (file, response) {
        if (!response.success) {
            removeImageView(file);
            //合并异常
            $.messager.showErrorTopCenter('错误消息', response.error);
            return;
        }

        let fileId = file.id;
        let fileName = file.name;
        let blobId = response.id;

        //对应后台数据结构：BlobInfoDTO
        let blob = {};
        blob.blobId = blobId;
        blob.fileId = fileId;
        blob.blobName = fileName;
        blob.ext = file.ext;
        blob.size = file.size;
        blob.showImageUrl = settings.showImageUrl + blob.blobId;
        blob.downloadFileUrl = settings.downloadFileUrl + blob.blobId;
        if (response.result) {
            let fileBlob = response.result;
            blob.tenantName = fileBlob.tenantName ? fileBlob.tenantName : "";
            blob.downloadFileUrl = fileBlob.downloadFileUrl !== undefined && fileBlob.downloadFileUrl != null
                ? fileBlob.downloadFileUrl
                : settings.downloadFileUrl + blob.blobId;
            blob.showImageUrl = fileBlob.showImageUrl !== undefined && fileBlob.showImageUrl != null
                ? fileBlob.showImageUrl
                : settings.showImageUrl + blob.blobId;
        }

        //文件上传时，默认设置第一个文件的SetDefault值为true
        if (settings.isSetDefault && fileCount === 0) {
            file.isSelect = true;
        }

        setImageViewValue(file, blobId);

        fileCount++;
        uploader.reset();
        //单个文件 已上传
        settings.callback.uploadSuccess(file, response, blob);
    };

    // 已经成功上传的文件个数
    uploader.fileCount = fileCount;
    // 根据传入的文件列表对象（blobs）初始化ImageView
    uploader.initImageView = function (blobs) {
        if (blobs === undefined || blobs == null)
            return;

        let isSingleFile = settings.fileNumLimit !== undefined && settings.fileNumLimit != null
            ? settings.fileNumLimit === 1
            : false;
        if (isSingleFile && Array.isArray(blobs)) {
            if (blobs.length <= 0) return;

            fileCount = 1;
            let blob = blobs[0];
            //对应后台数据结构：BlobInfoDTO
            let file = convertBlobToFile(blob);
            appendImageView(file);
            hiddenImageProgress(file);
            setImageViewValue(file, blob.blobId);
        } else if (Array.isArray(blobs)) {
            if (blobs.length <= 0) return;

            fileCount = fileCount + blobs.length;
            $.each(blobs, function (i, blob) {
                //对应后台数据结构：BlobInfoDTO
                let file = convertBlobToFile(blob);
                appendImageView(file);
                hiddenImageProgress(file);
                setImageViewValue(file, blob.blobId);
            })
        } else {
            fileCount = 1;
            let blob = blobs;

            let file = convertBlobToFile(blob);
            appendImageView(file);
            hiddenImageProgress(file);
            setImageViewValue(file, blob.blobId);
        }

    };
    // 根据传入的文件列表对象Id（blobIds）移除ImageView
    uploader.removeImageView = function (ids) {
        if (ids === undefined || ids == null)
            return;

        if (Array.isArray(ids)) {
            if (ids.length <= 0) return;
            fileCount = fileCount - ids.length;
            $.each(ids, function (i, blobId) {
                let file = {};
                file.id = blobId;
                file.name = blobId;
                removeImageView(file);
            })
        } else {
            fileCount = 1;
            let id = ids;
            let file = {};
            file.id = id;
            file.name = id;
            removeImageView(file);
        }
    };
    // 获取已经上传的所有文件列表对象（blobs)
    uploader.getAllBlobs = function () {
        let blobs = [];
        let rdoSetDefault = $(settings.fileList).find('.setDefault .radiobutton.radiobutton-checked .radiobutton-value');
        let selectBlobId = '';
        if (rdoSetDefault !== undefined && rdoSetDefault != null)
            selectBlobId = $(rdoSetDefault).val();
        let txtBlobs = $(settings.fileList).find('input.imageBlobId');
        if (txtBlobs !== undefined && txtBlobs != null && txtBlobs.length > 0) {
            $.each(txtBlobs, function (i, txtBlob) {
                let blob = {};
                blob.blobId = $(txtBlob).val();
                blob.blobName = $(txtBlob).attr('title');
                blob.ext = $(txtBlob).attr('ext');
                blob.size = $(txtBlob).attr('size');
                blob.propertyId = $(txtBlob).attr('propertyId');
                blob.isSelect = selectBlobId === blob.blobId;
                blobs.push(blob);
            });
        }

        return blobs;
    };

    return uploader;
};

//静态方法：获取divList中ImageList视图
imageUploader.getImageListHtmlView = function (blob, isEditView, isSetDefault, imageUploadId) {
    let fileId = blob.blobId;
    let fileName = blob.blobName;
    let fileExt = blob.ext;
    let fileSize = blob.size;
    let imageUrl = blob.showImageUrl;
    let propertyId = blob.propertyId !== undefined && blob.propertyId != null ? blob.propertyId : '';
    let isSelect = blob.isSelect !== undefined && blob.isSelect != null ? blob.isSelect : false;
    let isEdit = isEditView !== undefined && isEditView != null ? isEditView : false;
    let isDefault = isSetDefault !== undefined && isSetDefault != null ? isSetDefault : false;
    let uploadId = imageUploadId !== undefined && imageUploadId != null && imageUploadId !== '' ? imageUploadId : 'imageUploader';

    //隐藏Input.imageBlobId控件保存上传控件后的值：blobId: [attr=value],blobName: [attr=title],isSelect: [attr=checked],propertyId: [attr=propertyId]
    let divHiddenInput = '<input id="txt-' + fileId + '" type="hidden" class="imageBlobId" title="' + fileName + '" ext="' + fileExt + '" size="' + fileSize + '" propertyId="' + propertyId + '" value="' + fileId + '"/>';
    if (isSelect)
        divHiddenInput = '<input id="txt-' + fileId + '" type="hidden" class="imageBlobId" title="' + fileId + '" ext="' + fileExt + '" size="' + fileSize + '" propertyId="' + propertyId + '" value="' + fileId + '" checked="checked"/>';

    if (!isEdit) {
        let divShowFile =
            '<div>' +
            '   <div id="file-' + fileId + '" class="imageFile">' +
            divHiddenInput +
            '       <img id="img-' + fileId + '" class="fileImage" src="' + imageUrl + '" alt="' + fileName + '">' +
            '   </div>' +
            '</div>';
        if (isSelect) {
            divShowFile =
                '<div>' +
                '   <div id="file-' + fileId + '" class="imageFile">' +
                divHiddenInput +
                '       <img id="img-' + fileId + '" class="fileImage" src="' + imageUrl + '" alt="' + fileName + '">' +
                '       <a id="close-' + fileId + '" class="checkImg"><div><i class="fa fa-check-circle fa-1x" aria-hidden="true"></i></div></a>' +
                '   </div>' +
                '</div>';
        }

        return divShowFile;
    }

    let divFile =
        '<div>' +
        '   <div id="file-' + fileId + '" class="imageFile">' +
        divHiddenInput +
        '       <img id="img-' + fileId + '" class="fileImage" src="' + imageUrl + '" alt="' + fileName + '">' +
        '       <a id="close-' + fileId + '" class="closeImg"><div><i class="fa fa-times fa-1x" aria-hidden="true"></i></div></a>' +
        '       <div id="progress-' + fileId + '" class="easyui-progressbar"></div>' +
        '   </div>' +
        '</div>';
    if (isDefault) {
        divFile =
            '<div>' +
            '   <div id="file-' + fileId + '" class="imageFile">' +
            divHiddenInput +
            '       <img id="img-' + fileId + '" class="fileImage" src="' + imageUrl + '" alt="' + fileName + '">' +
            '       <a id="close-' + fileId + '" class="closeImg"><div><i class="fa fa-times fa-1x" aria-hidden="true"></i></div></a>' +
            '       <div id="progress-' + fileId + '" class="easyui-progressbar"></div>' +
            '   </div>' +
            '   <div class="setDefault">' +
            '       <input id="rdo-' + fileId + '" class="rdoSetDefault easyui-radiobutton" name="' + uploadId + '" value="' + fileId + '" label="默认封面">' +
            '   </div>' +
            '</div>';
    }
    return divFile;
};

/**
 * 图片上传的简化调用方法，使用范例为：
<input type="hidden" id="hiddenFileBlobId" data-options="required:true" th:field="*{offeringFileBlob}"/>
<div class="imageUploaderEditor">
    <div style="display: flex;height: 110px;">
        <div id="imageUploader" class="webuploader-container">
        <a href="javascript:void(0)"><i class="fa fa-plus-square fa-5x"></i></a>
        </div>
    <div id="imageList" class="webUploader-image-list"></div>
    </div>
    <p id="imagePreviewContainer" class="imagePreview"></p>
    <label style="padding: 3px;">文件个数：3个；文件格式：[[${uploadConfig.fileExt}]]；文件大小：[[${uploadConfig.fileMaxSize}]]M</label>
</div>

//调用图片上传的JS
let fileNumLimit = 1;
let imageSize = @KC.Framework.Base.GlobalConfig.UploadConfig.ImageMaxSize;
let imageExt = '@KC.Framework.Base.GlobalConfig.UploadConfig.ImageExt';
let showImageUrl = '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/ShowImage?id=';
let fileMaxSize = @KC.Framework.Base.GlobalConfig.UploadConfig.FileMaxSize;
let fileExt = '@KC.Framework.Base.GlobalConfig.UploadConfig.FileExt';
let fileDownloadUrl = '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/DownloadFile?id=';
$(function () {
    let fileSetting = {
        fileNumLimit: fileNumLimit,
        fileMaxSize: fileMaxSize,
        fileExt: fileExt,
        downloadFileUrl: fileDownloadUrl,
        imageMaxSize: imageSize,
        imageExt: imageExt,
        showImageUrl: showImageUrl,
        params: { isArchive: true }
    };
    //debugger;
    defaultInitImageUploader('hiddenFileBlobId', 'imageUploader', 'imageList', fileSetting, true, function(blob){});
})
 * @param {any} fileBlobId          保存图片Blob数据的隐藏Input的Id
 * @param {any} fileAddButtonId     图片上传后视图Id，isEditView=false时，可为空
 * @param {any} fileListId          图片上传后视图Id
 * @param {any} fileSetting         图片上传设置对象：{fileNumLimit: 1, imageMaxSize: 1024, imageExt: 'pdf,doc', showImageUrl: 'http://api.kcloudy.com/api/resource/showimage?id='}
 * @param {any} isEditView          显示图片是否可编辑（删除），默认为：true
 */
let defaultInitImageUploader = function (fileBlobId, fileAddButtonId, fileListId, fileSetting, isEditView, fnUploadSuccess) {
    //debugger;
    let imageBlobJsonString = null;
    let imageBlob = null;
    if (fileBlobId !== undefined && fileBlobId !== '')
        imageBlobJsonString = $('#' + fileBlobId).val();
    if (imageBlobJsonString !== undefined && imageBlobJsonString !== '') {
        imageBlob = JSON.parse(imageBlobJsonString);
    }

    let blobId = '';
    if (imageBlob !== undefined && imageBlob !== null && !Array.isArray(imageBlob))
        blobId = imageBlob.blobId;
    //如果设置中传入了blobId，那么需要使用传入值进行替换，为了拷贝其他blob使用的
    if (fileSetting.params && fileSetting.params.blobId)
        blobId = fileSetting.params.blobId;

    let postUrl = defaultPostWebUploaderUrl + "?parm=uploadimage";
    let fileNumLimit = defaultFileNumLimit;
    let fileMaxSize = defaultFileSizeLimit;
    let fileExt = defaultDocExt;
    let downloadFileUrl = defaultDownloadFileUrl;
    let imageMaxSize = defaultFileSizeLimit;
    let imageExt = defaultImageExt;
    let showImageUrl = defaultShowImageUrl;
    let isArchive = true;
    if (fileSetting !== undefined && fileSetting !== null) {
        postUrl = fileSetting.postUrl !== undefined && fileSetting.postUrl !== null
            ? fileSetting.postUrl : defaultPostWebUploaderUrl + "?parm=uploadimage";
        fileNumLimit = fileSetting.fileNumLimit !== undefined && fileSetting.fileNumLimit !== null
            ? fileSetting.fileNumLimit : defaultFileNumLimit;
        fileMaxSize = fileSetting.fileMaxSize !== undefined && fileSetting.fileMaxSize !== null
            ? fileSetting.fileMaxSize : defaultFileSizeLimit;
        fileExt = fileSetting.fileExt !== undefined && fileSetting.fileExt !== null
            ? fileSetting.fileExt : defaultDocExt;
        downloadFileUrl = fileSetting.downloadFileUrl !== undefined && fileSetting.downloadFileUrl !== null
            ? fileSetting.downloadFileUrl : defaultDownloadFileUrl;
        imageMaxSize = fileSetting.imageMaxSize !== undefined && fileSetting.imageMaxSize !== null
            ? fileSetting.imageMaxSize : defaultFileSizeLimit;
        imageExt = fileSetting.imageExt !== undefined && fileSetting.imageExt !== null
            ? fileSetting.imageExt : defaultImageExt;
        showImageUrl = fileSetting.showImageUrl !== undefined && fileSetting.showImageUrl !== null
            ? fileSetting.showImageUrl : defaultShowImageUrl;
        
        isArchive = fileSetting && fileSetting.params && fileSetting.params.isArchive != undefined && fileSetting.params.isArchive != null
            ? fileSetting.params.isArchive : true;
    }
    let imageWebUploader = imageUploader({
        postUrl: postUrl,
        btnAddFile: fileAddButtonId ? '#' + fileAddButtonId : '',
        fileList: fileListId ? '#' + fileListId : '',
        fileNumLimit: fileNumLimit,
        showImageUrl: showImageUrl,
        downloadFileUrl: downloadFileUrl,
        params: { blobId: blobId, isArchive: isArchive },
        isRegister: true,
        isEditView: isEditView !== undefined && isEditView !== null ? isEditView : true,  //显示图片是否可编辑
        configure: {
            imageMaxSize: imageMaxSize,
            imageExt: imageExt,
            fileMaxSize: fileMaxSize,
            fileExt: fileExt
        },
        callback: {
            uploadProgress: function (file, percentage) {
            },
            uploadComplete: function (file) { //不管成功或者失败，文件上传完成时触发
            },
            uploadSuccess: function (file, response, blob) {
                if (fileNumLimit === 1) {
                    let blobJsonString = JSON.stringify(blob);
                    $('#' + fileBlobId).val(blobJsonString);
                } else {
                    let blobs = imageWebUploader.getAllBlobs();
                    let blobsJsonString = JSON.stringify(blobs);
                    $('#' + fileBlobId).val(blobsJsonString);
                }

                if (fnUploadSuccess && $.isFunction(fnUploadSuccess))
                    fnUploadSuccess(blob);
            },
            uploadError: function (file, reason) {
            },
            onFileQueued: function (file) {
            }
        }
    });

    //根据图片数据，初始化显示已有图片
    imageWebUploader.initImageView(imageBlob);

    return imageWebUploader;
};


/**
 * 文件上传组件
 //单文件上传的前端div容器
 <input type="hidden" id="hiddenFileBlobId" data-options="required:true" th:field="*{offeringFileBlob}"/>
 <div class="fileUploaderEditor" style="display: flex;">
 <div id="fileList" class="webUploader-file-single"></div>
 <a id="fileUploader" href="javascript:void(0)" class="easyui-linkbutton" iconcls="fa fa-pencil">选择</a>
 </div>
 <label>文件格式：[[${uploadConfig.fileExt}]]，文件大小：[[${uploadConfig.fileMaxSize}]]M</label>

 //多文件上传的前端div容器
 <div class="fileUploaderEditor">
 <div style="display: flex;height: 110px;">
 <div id="fileUploader" class="webuploader-container">
 <a href="javascript:void(0)"><i class="fa fa-plus-square fa-5x"></i></a>
 </div>
 <div id="fileList" class="webUploader-file-list"></div>
 </div>
 <p class="imagePreview"></p>
 <label style="padding: 3px;">文件个数：3个；文件格式：[[${uploadConfig.fileExt}]]；文件大小：[[${uploadConfig.fileMaxSize}]]M</label>
 </div>

 let imageSize = [[${uploadConfig.imageMaxSize}]];
 let imageExt = '[[${uploadConfig.imageExt}]]';
 let fileMaxSize = [[${uploadConfig.fileMaxSize}]];
 let fileWebUploader = null;
 let fileBlobJsonString = '[[${fileBlobJsonString}]]';
 let defaultInitFileUploader = function () {
        let blobId = $('#hiddenFileBlobId').val();
        fileWebUploader = fileUploader({
            btnAddFile: '#fileUploader',
            fileList: '#fileList',
            fileNumLimit: 3,
            showImageUrl: '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/ShowImage?id=',
            downloadFileUrl: '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/DownloadFile?id=',
            params: {blobId: blobId, isArchive: true}, //单文件时可传入文件BlobId，用于覆盖相关文件
            isRegister: true,
            isEditView: false, //显示图片是否可编辑
            configure: {
                imageMaxSize: imageSize,
                imageExt: imageExt,
                fileMaxSize: fileMaxSize,
                fileExt: fileExt
            },
            callback: {
                uploadProgress: function (file, percentage) {
                },
                uploadComplete: function (file) { //不管成功或者失败，文件上传完成时触发
                },
                uploadSuccess: function (file, response, blob) {
                },
                uploadError: function (file, reason) {
                },
                onFileQueued: function (file) {
                }
            }
        });

        //根据已有图片数据，显示图片
        if (fileBlobJsonString !== undefined && fileBlobJsonString !== '') {
            let objString = CommonUtil.escape2Html(fileBlobJsonString);
            let fileBlob = JSON.parse(objString);
            fileWebUploader.initFileView([fileBlob]);
        } else {
            fileWebUploader.initFileView([]);
        }

    };
 * @param {any} option
 */
let fileUploader = function (option) {
    let token = $("meta[name='_csrf']").attr("content");
    let header = $("meta[name='_csrf_header']").attr("content");
    let data = { tenantName: '', userId: '', blobId: '', isWaterMake: false };
    if (token !== undefined && token != null && token !== "") {
        data = { _csrf: token, tenantName: '', userId: '', blobId: '', isWaterMake: false }
    }

    let uploadType = 'uploadfile';
    let settings = {
        componentName: 'fileUploader',
        btnStartUpload: null,
        btnAddFile: null,
        btnContinueAddFile: null,
        type: 4, //0:image,1:file,2:audio,3:video,4:all
        fileList: null,
        fileStorage: null,
        fileNumLimit: defaultFileNumLimit,
        fileSizeLimit: defaultFileSizeLimit,
        fileSingleSizeLimit: defaultFileSingleSizeLimit,
        baseUrl: defaultBaseWebUploaderUrl,
        postUrl: defaultPostWebUploaderUrl + "?parm=" + uploadType,
        chunkCheckUrl: defaultChunkCheckUrl,
        chunksMergeUrl: defaultChunksMergeUrl,
        showImageUrl: defaultShowImageUrl,
        downloadFileUrl: defaultDownloadFileUrl,
        chunkSize: defaultChunkSize, //分块大小
        params: data,
        isRegister: false,
        isSetDefault: false,
        isEditor: false,
        isEditView: true,
        disableWidgets: [],
        configure: {
            imageMaxSize: 10,
            imageExt: defaultImageExt,
            fileMaxSize: 10,
            fileExt: defaultDocExt + ',' + defaultImageExt
        },
        callback: {
            onFileQueued: function (file) {
            },
            uploadStart: function (file) {
            },
            uploadProgress: function (file, percentage) {
            },
            uploadComplete: function (file) {//不管成功或者失败，文件上传完成时触发
            },
            uploadSuccess: function (file, response) {
            },
            uploadError: function (file, reason) {
            },
            onRemoveFile: function (file) {
            }
        }
    };
    $.extend(true, settings, option);
    let uploader = null;
    let filesMd5 = {};
    let state = 'pending';
    let fileCount = 0;
    let maxSize = settings.configure.fileMaxSize != null && settings.configure.fileMaxSize !== 0
        ? settings.configure.fileMaxSize
        : 10;
    let ext = (settings.configure.imageExt != null && settings.configure.imageExt !== '')
        || (settings.configure.fileExt != null && settings.configure.fileExt !== '')
        ? settings.configure.imageExt + ',' + settings.configure.fileExt
        : defaultImageExt + ',' + defaultDocExt;
    let isSingleFile = settings.fileNumLimit !== undefined && settings.fileNumLimit != null
        ? settings.fileNumLimit === 1
        : false;

    //console.info('------comUploader accept ext: ' + ext + ' and limit file size: ' + maxSize);

    if (settings.isRegister) {
        WebUploader.Uploader.register({
            "before-send-file": "beforeSendFile",
            "before-send": "beforeSend",
            "after-send-file": "afterSendFile",
            'name': settings.componentName
        }, {
            beforeSendFile: function (file) {
                let task = new $.Deferred();
                let start = new Date().getTime();
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
                let task = new $.Deferred();
                //不需要分片上传
                if (block.chunks === 1) {
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
                let chunksTotal = Math.ceil(file.size / settings.chunkSize);
                if (chunksTotal > 1) {
                    //合并请求
                    let task = new $.Deferred();
                    $.ajax({
                        type: "POST",
                        url: settings.chunksMergeUrl,
                        data: {
                            folder: filesMd5[file.name],
                            chunks: chunksTotal,
                            name: file.name,
                            type: uploadType,
                            ext: file.ext,
                            tenantName: settings.params.tenantName,
                            blobId: settings.params.blobId,
                            userId: settings.params.userId,
                            isArchive: settings.params.isArchive
                        },
                        cache: false,
                        dataType: "json"
                    }).then(function (data, textStatus, jqXHR) {
                        //todo 检查响应是否正常
                        if (data && data.success) {
                            file.path = data.path;
                            uploadCompleted(file, data);

                            hiddenFileProgress(file);

                            uploader.removeFile(file);

                            settings.callback.uploadComplete(file);
                        } else {
                            task.reject();
                        }
                    }, function (jqXHR, textStatus, errorThrown) {
                        task.reject();
                    });

                    return $.when(task);
                } else {
                    //uploadCompleted(file);
                }
            }
        });
    }
    uploader = WebUploader.create({
        // 不压缩image
        resize: false,
        // swf文件路径
        swf: settings.baseUrl + '/Uploader.swf',
        server: settings.postUrl,
        // 选择文件的按钮。可选。
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        pick: settings.btnAddFile,
        dnd: settings.fileList,
        paste: document.body,
        threads: 1,     //上传并发数
        chunked: true,  //开启分片上传
        chunkSize: settings.chunkSize,
        formData: settings.params,
        fileNumLimit: settings.fileNumLimit,
        fileSizeLimit: settings.fileSizeLimit,
        fileSingleSizeLimit: settings.fileSingleSizeLimit,
        disableWidgets: settings.disableWidgets,
        duplicate: false,
        disableGlobalDnd: true,
        compress: false,
        prepareNextFile: true,
        accept: {
            title: '文件类型',
            extensions: ext,
            mimeTypes: defaultImageMimeTypes + ',' + defaultDocMimeTypes
        }
    });

    if (settings.btnContinueAddFile) {
        uploader.addButton({
            id: settings.btnContinueAddFile,
            label: '继续添加'
        });
    }

    // 当文件被加入队列以后触发
    uploader.on('fileQueued', function (file) {
        appendFileView(file);

        uploader.upload();

        settings.callback.onFileQueued(file);
    });
    // 当文件被加入队列之前触发，此事件的handler返回值为false，则此文件不会被添加进入队列
    uploader.on('beforeFileQueued', function (file) {
        let fileExt = file.ext;
        if (ext.indexOf(fileExt) < 0) {
            $.messager.showInfoTopCenter('系统提示', '上传的文件类型不正确', 1000);
            return false;
        }

        //一个文件可重复上传，每次上传时，需要将返回的blobId以参数的形式附加到postdata上，以便覆盖文件
        if (settings.fileNumLimit === 1)
            return true;

        if (settings.fileNumLimit <= fileCount) {
            $.messager.showInfoTopCenter('系统提示', '最多只能上传' + settings.fileNumLimit + '个文件', 1000);
            return false;
        }

        if (settings.fileStorage && settings.fileNumLimit) {
            if (settings.fileStorage.children().length + uploader.getStats().queueNum >= settings.fileNumLimit) {
                //alert('最多只能上传' + settings.fileNumLimit + '个文件');
                $.messager.showInfoTopCenter('系统提示', '最多只能上传' + settings.fileNumLimit + '个文件', 1000);
                return false;
            }
        }
        return true;
    });
    // 当某个文件的分块在发送前触发，主要用来询问是否要添加附带参数，大文件在开起分片上传的前提下此事件可能会触发多次
    uploader.on('uploadBeforeSend', function (block, data) {
        // block为分块数据。

        // file为分块对应的file对象。
        let file = block.file;

        // 将存在file对象中的md5数据携带发送过去。
        data.md5 = filesMd5[file.name];
        data.uploadType = uploadType;
        data.ext = file.ext;

        // 删除其他数据
        // delete data.key;
    });
    // 某个文件开始上传前触发，一个文件只会触发一次。
    uploader.on('uploadStart', function (file) {
        settings.callback.uploadStart(file);
    });
    // 文件上传过程中创建进度条实时显示。
    uploader.on('uploadProgress', function (file, percentage) {
        setFileProgressValue(file, percentage);
        //percentage * 100 + '%'
        settings.callback.uploadProgress(file, percentage);
    });
    // 当文件上传成功时触发
    uploader.on('uploadSuccess', function (file, response) {
        uploadCompleted(file, response);
    });
    // 当文件上传出错时触发
    uploader.on('uploadError', function (file, reason) {
        removeFileView(file);
        //上传出错
        settings.callback.uploadError(file, reason);
    });
    // 不管成功或者失败，文件上传完成时触发
    uploader.on('uploadComplete', function (file) {
        hiddenFileProgress(file);

        uploader.removeFile(file);

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
    });
    uploader.on('error', function (handler) {
        switch (handler) {
            case "Q_EXCEED_NUM_LIMIT":
                //alert("超出允许最大上传数");
                $.messager.showInfoTopCenter('系统提示', '超出允许最大上传数', 1000);
                break;
            case "F_DUPLICATE":
                //alert("文件重复");
                $.messager.showInfoTopCenter('系统提示', '文件重复', 1000);
                break;
            case "Q_TYPE_DENIED":
                //alert("文件类型不满足");
                $.messager.showInfoTopCenter('系统提示', '文件类型不正确', 1000);
                break;
            case "F_EXCEED_SIZE":
                //alert("文件太大了");
                $.messager.showInfoTopCenter('系统提示', '文件太大了', 1000);
                break;
        }
    });

    // 添加上传图片的前端视图
    let appendFileView = function (file) {
        let appendDiv = $(settings.fileList);
        if (appendDiv === undefined || appendDiv == null)
            return;

        //对应后台数据结构：BlobInfoDTO
        let fileId = file.id;
        let isEditView = settings.isEditView !== undefined && settings.isEditView != null ? settings.isEditView : true;

        let blob = convertFileToBlob(file);
        let propertyId = blob.propertyId !== undefined && blob.propertyId != null ? blob.propertyId : '';
        file.propertyId = propertyId;

        //隐藏Input.imageBlobId控件保存上传控件后的值：blobId: [attr=value],blobName: [attr=title],isSelect: [attr=checked]
        let divFile = fileUploader.getFileListHtmlView(blob, isEditView, isSingleFile, settings.componentName);
        if (isSingleFile) {
            let singleFile = $(settings.fileList).find('a.singleFile');
            if (singleFile !== undefined && singleFile != null && singleFile.length > 0) {
                return;
            }
            appendDiv.append(divFile);
            $(settings.fileList).find('.progressbar').progressbar();
        } else {
            appendDiv.append(divFile);
            //$('#file-' + fileId).tooltip({position: 'bottom'});
            $('#close-' + fileId).click(function () {
                removeFileView(file);
            });
            $('#progress-' + fileId).progressbar();
        }
    };

    //将file对象转换为Blob对象
    let convertBlobToFile = function (blob) {
        let file = {};
        file.id = blob.blobId;
        file.name = blob.blobName;
        file.ext = blob.ext;
        file.size = blob.size;
        file.downloadFileUrl = blob.downloadFileUrl !== undefined && blob.downloadFileUrl != null ? blob.downloadFileUrl : settings.downloadFileUrl + blob.blobId;
        file.showImageUrl = blob.showImageUrl !== undefined && blob.showImageUrl != null ? blob.showImageUrl : settings.showImageUrl + blob.blobId;
        file.propertyId = blob.propertyId !== undefined && blob.propertyId != null ? blob.propertyId : '';
        file.isSelect = blob.isSelect !== undefined && blob.isSelect != null ? blob.isSelect : false;
        return file;
    };
    //将file对象转换为Blob对象
    let convertFileToBlob = function (file) {
        let blob = {};
        blob.blobId = file.id;
        blob.blobName = file.name;
        blob.ext = file.ext;
        blob.size = file.size;
        blob.downloadFileUrl = file.downloadFileUrl !== undefined && file.downloadFileUrl != null ? file.downloadFileUrl : settings.downloadFileUrl + blob.blobId;
        blob.showImageUrl = file.showImageUrl !== undefined && file.showImageUrl != null ? file.showImageUrl : settings.showImageUrl + blob.blobId;
        blob.propertyId = file.propertyId !== undefined && file.propertyId != null ? file.propertyId : '';
        blob.isSelect = file.isSelect !== undefined && file.isSelect != null ? file.isSelect : false;
        return blob;
    };
    // 上传图片完成后，给相关的视图赋值
    let setFileViewValue = function (file, blobId) {
        if (settings.fileList === undefined || settings.fileList == null)
            return;

        //对应后台数据结构：BlobInfoDTO
        let fileId = file.id;
        let fileName = file.name;
        let fileExt = file.ext;
        let fileSize = file.size;
        let propertyId = file.propertyId !== undefined && file.propertyId != null ? file.propertyId : '';
        let imageUrl = file.downloadFileUrl !== undefined && file.downloadFileUrl != null ? file.downloadFileUrl : settings.downloadFileUrl + blobId;

        if (isSingleFile) {
            let txtBlob = $(settings.fileList).find('input.fileBlobId');
            if (txtBlob !== undefined && txtBlob != null && txtBlob.length > 0) {
                $(txtBlob).val(blobId);//隐藏Input，用于保持BlobId及是否选中IsSelect属性
                $(txtBlob).attr('title', fileName);
                $(txtBlob).attr('ext', fileExt);
                $(txtBlob).attr('size', fileSize);
                $(txtBlob).attr('propertyId', propertyId);
            }
            let singleFile = $(settings.fileList).find('a.singleFile');
            if (singleFile !== undefined && singleFile != null && singleFile.length > 0) {
                $(singleFile).attr('href', imageUrl);
                $(singleFile).text(fileName);
                if ($(singleFile).tooltip)
                    $(singleFile).tooltip({
                        position: 'top',
                        content: '<span style="color:#fff">' + fileName + '</span>',
                        onShow: function () {
                            $(this).tooltip('tip').css({
                                backgroundColor: '#666',
                                borderColor: '#666'
                            });
                        }
                    });
            }
        } else {
            const imgId = '#img-' + fileId;
            const txtId = '#txt-' + fileId;

            $(imgId).attr('href', imageUrl);
            //$(imgId).text(fileName);

            $(txtId).val(blobId);//隐藏Input，用于保持BlobId及是否选中IsSelect属性
            $(txtId).attr('title', fileName);
            $(txtId).attr('ext', fileExt);
            $(txtId).attr('size', fileSize);
            $(txtId).attr('propertyId', propertyId);

            if ($(imgId).tooltip)
                $(imgId).tooltip({
                    position: 'top',
                    content: '<span style="color:#fff">' + fileName + '</span>',
                    onShow: function () {
                        $(this).tooltip('tip').css({
                            backgroundColor: '#666',
                            borderColor: '#666'
                        });
                    }
                });
        }
    };
    let getOffsetTopByBody = function (el) {
        let offsetTop = 0;
        while (el && el.tagName !== 'BODY') {
            offsetTop += el.offsetTop;
            el = el.offsetParent;
        }
        return offsetTop
    };
    let getOffsetLeftByBody = function (el) {
        let offsetLeft = 0;
        while (el && el.tagName !== 'BODY') {
            offsetLeft += el.offsetLeft;
            el = el.offsetParent
        }
        return offsetLeft;
    };
    // 设置文件上传进度条
    let setFileProgressValue = function (file, percentage) {
        if (settings.fileList === undefined || settings.fileList == null)
            return;

        let fileId = file.id;
        if (isSingleFile) {
            if (percentage < 100) {
                $(settings.fileList).find('.progressbar').progressbar('setValue', parseInt(percentage * 100));
            } else {
                $(settings.fileList).find('.progressbar').hide();
            }
        } else {
            if (percentage < 100) {
                $('#progress-' + fileId).progressbar('setValue', parseInt(percentage * 100));
            } else {
                $('#progress-' + fileId).hide();
            }
        }
    };
    // 隐藏上传进度条
    let hiddenFileProgress = function (file) {
        if (settings.fileList === undefined || settings.fileList == null)
            return;

        let fileId = file.id;
        let isSingleFile = settings.fileNumLimit !== undefined && settings.fileNumLimit != null ? settings.fileNumLimit === 1 : false;
        if (isSingleFile) {
            $(settings.fileList).find('.progressbar').hide();
        } else {
            $('#progress-' + fileId).hide();
        }
    };
    // 删除图片的前端视图
    let removeFileView = function (file) {
        if (settings.fileList === undefined || settings.fileList == null)
            return;
        if (isSingleFile)
            return;

        let divFileId = '#file-' + file.id;
        //$(divFileId).tooltip('destroy');

        if (settings.callback !== undefined && settings.callback != null
            && settings.callback.onRemoveImage !== uploader && settings.callback.onRemoveImage != null) {
            //如果回调函数（onRemoveImage）有返回值（删除是否成功），如果为false时，前端页面不做处理
            //如果回调函数涉及异步操作时，isDelete值为：undefined，视同为true
            let isDelete = settings.callback.onRemoveImage(file);
            if (isDelete !== undefined && isDelete != null) {
                if (!isDelete)
                    return;
            }
        }

        $(divFileId).parent().remove();

        fileCount--;
    };
    let uploadCompleted = function (file, response) {
        if (!response.success) {
            removeFileView(file);
            //合并异常
            $.messager.showErrorTopCenter('错误消息', response.message);
            return;
        }

        let fileId = file.id;
        let fileName = file.name;
        let blobId = response.id;

        if (isSingleFile) {
            uploader.options.formData.blobId = blobId;
        }

        //对应后台数据结构：BlobInfoDTO
        let blob = {};
        blob.blobId = blobId;
        blob.fileId = fileId;
        blob.blobName = fileName;
        blob.ext = file.ext;
        blob.size = file.size;
        blob.showImageUrl = settings.showImageUrl + blob.blobId;
        blob.downloadFileUrl = settings.downloadFileUrl + blob.blobId;
        if (response.result) {
            let fileBlob = response.result;
            blob.tenantName = fileBlob.tenantName ? fileBlob.tenantName : "";
            blob.downloadFileUrl = fileBlob.downloadFileUrl !== undefined && fileBlob.downloadFileUrl != null
                ? fileBlob.downloadFileUrl
                : settings.downloadFileUrl + blob.blobId;
            blob.showImageUrl = fileBlob.showImageUrl !== undefined && fileBlob.showImageUrl != null
                ? fileBlob.showImageUrl
                : settings.showImageUrl + blob.blobId;
        }

        //文件上传时，默认设置第一个文件的SetDefault值为true
        if (settings.isSetDefault && fileCount === 0) {
            file.isSelect = true;
        }

        setFileViewValue(file, blobId);

        fileCount++;
        uploader.reset();
        //单个文件 已上传
        settings.callback.uploadSuccess(file, response, blob);
    };

    // 已经成功上传的文件个数
    uploader.fileCount = fileCount;
    // 根据传入的文件列表对象（blobs）初始化ImageView
    uploader.initFileView = function (blobs) {
        if (blobs === undefined || blobs == null)
            return;

        let isSingleFile = settings.fileNumLimit !== undefined && settings.fileNumLimit != null
            ? settings.fileNumLimit === 1
            : false;
        if (isSingleFile && Array.isArray(blobs)) {
            if (blobs.length <= 0) return;

            fileCount = 1;
            let blob = blobs[0];
            //对应后台数据结构：BlobInfoDTO
            let file = convertBlobToFile(blob);
            appendFileView(file);
            hiddenFileProgress(file);
            setFileViewValue(file, blob.blobId);
        } else if (Array.isArray(blobs)) {
            if (blobs.length <= 0) return;

            fileCount = fileCount + blobs.length;
            $.each(blobs, function (i, blob) {
                //对应后台数据结构：BlobInfoDTO
                let file = convertBlobToFile(blob);
                appendFileView(file);
                hiddenFileProgress(file);
                setFileViewValue(file, blob.blobId);
            })
        } else {
            fileCount = 1;
            let blob = blobs;
            //对应后台数据结构：BlobInfoDTO
            let file = convertBlobToFile(blob);
            appendFileView(file);
            hiddenFileProgress(file);
            setFileViewValue(file, blob.blobId);
        }
    };
    // 根据传入的文件列表对象Id（blobIds）移除ImageView
    uploader.removeFileView = function (ids) {
        if (ids === undefined || ids == null)
            return;

        if (Array.isArray(ids)) {
            if (ids.length <= 0) return;
            fileCount = fileCount - ids.length;
            $.each(ids, function (i, blobId) {
                let file = {};
                file.id = blobId;
                file.name = blobId;
                removeFileView(file);
            })
        } else {
            fileCount = 1;
            let id = ids;
            let file = {};
            file.id = id;
            file.name = id;
            removeFileView(file);
        }
    };
    // 获取已经上传的所有文件列表对象（blobs)
    uploader.getAllBlobs = function () {
        let blobs = [];
        let txtBlobs = $(settings.fileList).find('input.fileBlobId');
        if (txtBlobs !== undefined && txtBlobs != null && txtBlobs.length > 0) {
            $.each(txtBlobs, function (i, txtBlob) {
                let blob = {};
                blob.blobId = $(txtBlob).val();
                blob.blobName = $(txtBlob).attr('title');
                blob.ext = $(txtBlob).attr('ext');
                blob.size = $(txtBlob).attr('size');
                blob.propertyId = $(txtBlob).attr('propertyId');
                blobs.push(blob);
            });
        }

        return blobs;
    };
    // 清除所有的文件（blobs)
    uploader.clearBlobs = function () {
        let txtBlobs = $(settings.fileList).find('input.fileBlobId');
        if (txtBlobs !== undefined && txtBlobs != null && txtBlobs.length > 0) {
            $.each(txtBlobs, function (i, txtBlob) {
                let blob = {};
                blob.blobId = $(txtBlob).val();
                blob.blobName = $(txtBlob).attr('title');
                blob.ext = $(txtBlob).attr('ext');
                blob.size = $(txtBlob).attr('size');
                blob.propertyId = $(txtBlob).attr('propertyId');

                if (txtBlob.id) {
                    let txtId = txtBlob.id.replace('txt-', 'img-');
                    $("#" + txtId).text('');
                }

                $("#img-" + blob.blobId).text('');
            });
        }

        uploader.reset();

        return true;
    };
    return uploader;
};

//静态方法：获取divList中ImageList视图
fileUploader.getFileListHtmlView = function (blob, isEditView, isSingleFile, imageUploadId) {
    let fileId = blob.blobId;
    let fileName = blob.blobName;
    let fileExt = blob.ext.toLowerCase();
    let fileSize = blob.size;
    let imageUrl = blob.downloadFileUrl;
    let propertyId = blob.propertyId !== undefined && blob.propertyId != null ? blob.propertyId : '';
    let isSelect = blob.isSelect !== undefined && blob.isSelect != null ? blob.isSelect : false;
    let isEdit = isEditView !== undefined && isEditView != null ? isEditView : false;
    let isSingle = isSingleFile !== undefined && isSingleFile != null ? isSingleFile : false;
    let uploadId = imageUploadId !== undefined && imageUploadId != null && imageUploadId !== '' ? imageUploadId : 'imageUploader';

    let iconClass = CommonUtil.getIconClassByExt(fileExt);

    //隐藏Input.imageBlobId控件保存上传控件后的值：blobId: [attr=value],blobName: [attr=title],isSelect: [attr=checked],propertyId: [attr=propertyId]
    let divHiddenInput = '<input id="txt-' + fileId + '" type="hidden" class="fileBlobId" title="' + fileName + '" ext="' + fileExt + '" size="' + fileSize + '" propertyId="' + propertyId + '" value="' + fileId + '"/>';

    if (isSingle) {
        if (!isEdit) {
            return '<div>' +
                '   <div id="file-' + fileId + '" class="fileIcon">' +
                divHiddenInput +
                '       <div style="white-space:nowrap;overflow:hidden;text-overflow:clip;">' +
                '       <a id="img-' + fileId + '" target="_blank" class="singleFile" href="' + imageUrl + '" download="' + fileName + '">' + fileName + '</a>' +
                '   </div></div>' +
                '</div>';
        }

        return divHiddenInput +
            '<div class="progressbar" style="height: 32px;margin:0;padding:0;border:0;"></div>' +
            '<a id="img-' + fileId + '" target="_blank" class="singleFile" href="' + imageUrl + '" download="' + fileName + '">';
    }

    if (!isEdit) {
        return '<div>' +
            '   <div id="file-' + fileId + '" class="docFile">' +
            divHiddenInput +
            '       <div class="fileImage">' +
            '           <a id="img-' + fileId + '" target="_blank" class="fileIcon" href="' + imageUrl + '" download="' + fileName + '">' +
            '               <div style="font-size:16px;">' +
            '                   <i class="fa ' + iconClass + ' fa-5x" aria-hidden="true"></i>' +
            '               </div>' +
            '           </a>' +
            '           <span style="width:80px;text-align:center;display:block;float:left;">' + fileName + '</span>' +
            '       </div>' +
            '   </div>' +
            '</div>';
    }

    return '<div>' +
        '   <div id="file-' + fileId + '" class="docFile">' +
        divHiddenInput +
        '       <div class="fileImage">' +
        '           <a id="img-' + fileId + '" target="_blank" class="fileIcon" href="' + imageUrl + '" download="' + fileName + '">' +
        '               <div style="font-size:16px;">' +
        '                   <i class="fa ' + iconClass + ' fa-5x" aria-hidden="true"></i>' +
        '               </div>' +
        '           </a>' +
        '           <span style="width:80px;text-align:center;display:block;float:left;">' + fileName + '</span>' +
        '       </div>' +
        '       <a id="close-' + fileId + '" class="closeImg"><div><i class="fa fa-times fa-1x" aria-hidden="true"></i></div></a>' +
        '       <div id="progress-' + fileId + '" class="easyui-progressbar"></div>' +
        '   </div>' +
        '</div>';
};

/**
 * 文件上传的简化调用方法，使用范例为：
 * 
//单文件上传的前端div容器
<input type="hidden" id="hiddenFileBlobId" data-options="required:true" th:field="*{offeringFileBlob}"/>
<div class="fileUploaderEditor" style="display: flex;">
<div id="fileList" class="webUploader-file-single"></div>
<a id="fileUploader" href="javascript:void(0)" class="easyui-linkbutton" iconcls="fa fa-pencil">选择</a>
</div>
<label>文件格式：[[${uploadConfig.fileExt}]]，文件大小：[[${uploadConfig.fileMaxSize}]]M</label>

//多文件上传的前端div容器
<div class="fileUploaderEditor">
<div style="display: flex;height: 110px;">
<div id="fileUploader" class="webuploader-container">
<a href="javascript:void(0)"><i class="fa fa-plus-square fa-5x"></i></a>
</div>
<div id="fileList" class="webUploader-file-list"></div>
</div>
<p class="imagePreview"></p>
<label style="padding: 3px;">文件个数：3个；文件格式：[[${uploadConfig.fileExt}]]；文件大小：[[${uploadConfig.fileMaxSize}]]M</label>
</div>

//调用文件上传的Js
let fileNumLimit = 1;
let imageSize = @KC.Framework.Base.GlobalConfig.UploadConfig.ImageMaxSize;
let imageExt = '@KC.Framework.Base.GlobalConfig.UploadConfig.ImageExt';
let showImageUrl = '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/ShowImage?id=';
let fileMaxSize = @KC.Framework.Base.GlobalConfig.UploadConfig.FileMaxSize;
let fileExt = '@KC.Framework.Base.GlobalConfig.UploadConfig.FileExt';
let fileDownloadUrl = '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/DownloadFile?id=';
$(function () {
    let fileSetting = {
        fileNumLimit: fileNumLimit,
        fileMaxSize: fileMaxSize,
        fileExt: fileExt,
        downloadFileUrl: fileDownloadUrl,
        imageMaxSize: imageSize,
        imageExt: imageExt,
        showImageUrl: showImageUrl,
        params: { isArchive: true }
    };
    //debugger;
    defaultInitFileUploader('hiddenFileBlobId', 'fileUploader', 'fileList', fileSetting, true, function(blob){});
})
 * @param {any} fileBlobId          保存文件Blob数据的隐藏Input的Id
 * @param {any} fileAddButtonId     文件上传后视图Id，isEditView=false时，可为空
 * @param {any} fileListId          文件上传后视图Id
 * @param {any} fileSetting         文件上传设置对象：{fileNumLimit: 1, fileMaxSize: 1024, fileExt: 'pdf,doc', downloadFileUrl: 'http://api.kcloudy.com/api/resource/downloadfile?id='}
 * @param {any} isEditView          显示文件是否可编辑（删除）,默认为：true
 */
let defaultInitFileUploader = function (fileBlobId, fileAddButtonId, fileListId, fileSetting, isEditView, fnUploadSuccess) {
    //debugger;
    let fileBlobJsonString = null;
    let fileBlob = null;
    if (fileBlobId !== undefined && fileBlobId !== '')
        fileBlobJsonString = $('#' + fileBlobId).val();
    if (fileBlobJsonString !== undefined && fileBlobJsonString !== '') {
        fileBlob = JSON.parse(fileBlobJsonString);
    }
    let blobId = '';
    if (fileBlob !== undefined && fileBlob !== null && !Array.isArray(fileBlob))
        blobId = fileBlob.blobId;
    //如果设置中传入了blobId，那么需要使用传入值进行替换，为了拷贝其他blob使用的
    if (fileSetting.params && fileSetting.params.blobId)
        blobId = fileSetting.params.blobId;

    let postUrl = defaultPostWebUploaderUrl + "?parm=uploadfile";
    let fileNumLimit = defaultFileNumLimit;
    let fileMaxSize = defaultFileSizeLimit;
    let fileExt = defaultDocExt;
    let downloadFileUrl = defaultDownloadFileUrl;
    let imageMaxSize = defaultFileSizeLimit;
    let imageExt = defaultImageExt;
    let showImageUrl = defaultShowImageUrl;
    let isArchive = true;
    if (fileSetting !== undefined && fileSetting !== null) {
        postUrl = fileSetting.postUrl !== undefined && fileSetting.postUrl !== null
            ? fileSetting.postUrl : defaultPostWebUploaderUrl + "?parm=uploadfile";
        fileNumLimit = fileSetting.fileNumLimit !== undefined && fileSetting.fileNumLimit !== null
            ? fileSetting.fileNumLimit : defaultFileNumLimit;
        fileMaxSize = fileSetting.fileMaxSize !== undefined && fileSetting.fileMaxSize !== null
            ? fileSetting.fileMaxSize : defaultFileSizeLimit;
        fileExt = fileSetting.fileExt !== undefined && fileSetting.fileExt !== null
            ? fileSetting.fileExt : defaultDocExt;
        downloadFileUrl = fileSetting.downloadFileUrl !== undefined && fileSetting.downloadFileUrl !== null
            ? fileSetting.downloadFileUrl : defaultDownloadFileUrl;
        imageMaxSize = fileSetting.imageMaxSize !== undefined && fileSetting.imageMaxSize !== null
            ? fileSetting.imageMaxSize : defaultFileSizeLimit;
        imageExt = fileSetting.imageExt !== undefined && fileSetting.imageExt !== null
            ? fileSetting.imageExt : defaultImageExt;
        showImageUrl = fileSetting.showImageUrl !== undefined && fileSetting.showImageUrl !== null
            ? fileSetting.showImageUrl : defaultShowImageUrl;

        isArchive = fileSetting && fileSetting.params && fileSetting.params.isArchive != undefined && fileSetting.params.isArchive != null
            ? fileSetting.params.isArchive : true;
    }

    let fileWebUploader = fileUploader({
        postUrl: postUrl,
        btnAddFile: fileAddButtonId ? '#' + fileAddButtonId : '',
        fileList: fileListId ? '#' + fileListId : '',
        fileNumLimit: fileNumLimit,
        showImageUrl: showImageUrl,
        downloadFileUrl: downloadFileUrl,
        params: { blobId: blobId, isArchive: isArchive },
        isRegister: true,
        isEditView: isEditView !== undefined && isEditView !== null ? isEditView : true, //显示图片是否可编辑
        configure: {
            imageMaxSize: imageMaxSize,
            imageExt: imageExt,
            fileMaxSize: fileMaxSize,
            fileExt: fileExt
        },
        callback: {
            uploadProgress: function (file, percentage) {
            },
            uploadComplete: function (file) { //不管成功或者失败，文件上传完成时触发
            },
            uploadSuccess: function (file, response, blob) {
                if (fileNumLimit === 1) {
                    let blobJsonString = JSON.stringify(blob);
                    $('#' + fileBlobId).val(blobJsonString);
                } else {
                    let blobs = fileWebUploader.getAllBlobs();
                    let blobsJsonString = JSON.stringify(blobs);
                    $('#' + fileBlobId).val(blobsJsonString);
                }

                if (fnUploadSuccess && $.isFunction(fnUploadSuccess))
                    fnUploadSuccess(blob);
            },
            uploadError: function (file, reason) {
            },
            onFileQueued: function (file) {
            }
        }
    });

    //根据文件，初始化显示已有文件数据
    fileWebUploader.initFileView(fileBlob);

    return fileWebUploader;
};
