/// <reference path="../jquery-1.12.3.js" />

/**
 * Created by Administrator on 14-11-16.
 */

$(document).ready(function () {
    $('#menu').tendina({
        openCallback: function (clickedEl) {
            clickedEl.addClass('opened');
        },
        closeCallback: function (clickedEl) {
            clickedEl.addClass('closed');
        }
    });

});
$(function () {

    $("#ad_setting").click(function () {
        $("#ad_setting_ul").show();
    });
    $("#ad_setting_ul").mouseleave(function () {
        $(this).hide();
    });
    $("#ad_setting_ul li").mouseenter(function () {
        $(this).find("a").attr("class", "ad_setting_ul_li_a");
    });
    $("#ad_setting_ul li").mouseleave(function () {
        $(this).find("a").attr("class", "");
    });
});

$(document).on("click", ".default-left-menu", function () {
    var $this = $(this);
    $.ajax({
        async: true,
        type: "GET",
        cache: false,
        url: $this.attr("menu-href"),
        data: { twoMenuName: $this.text(), oneMenuName: $this.parents("ul:first").prev().text() },
        dataType: "html",
        success: function (evt) {
            $("#layout_right_content").html(evt);
        }
    });
});
$(document).on("click", ".page-href", function () {
    var $this = $(this);
    if ($this.parent().hasClass("disabled"))
        return;
    var searchFilter = $("#page_content").attr("search-filter");
    var pageHref = $this.attr("page-href") + "&" + searchFilter.replace('?', '');
    $.ajax({
        async: true,
        type: "GET",
        cache: false,
        url: pageHref,
        dataType: "html",
        success: function (evt) {
            $("#page_content:visible").html(evt);
        }
    });
});