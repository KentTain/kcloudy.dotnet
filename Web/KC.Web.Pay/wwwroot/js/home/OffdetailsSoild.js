$(document).ready(function () {
    // 图片上下滚动
    var count = $("#imageMenu li").length - 4; /* 显示 6 个 li标签内容 */
    var interval = $("#imageMenu li:first").width()+19;
    var curIndex = 0;
    $('.scrollbutton').click(function () {

        if ($(this).hasClass('disabled')) return false;

        if ($(this).hasClass('smallImgUp'))--curIndex;
        else ++curIndex;

        $('.scrollbutton').removeClass('disabled');
        if (curIndex == 0) $('.smallImgUp').addClass('disabled');
        if (curIndex == count) $('.smallImgDown').addClass('disabled');

        $("#imageMenu ul").stop(false, true).animate({ "marginLeft": -curIndex * interval + "px" }, 600);
    });
 
    
});