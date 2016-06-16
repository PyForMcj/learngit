/// <reference path="../jquery-1.12.3.js" />
$(document).on("click", "#select1 dd", function () {
    $(this).addClass("search-selected").siblings().removeClass("search-selected");
    if ($(this).hasClass("select-all")) {
        $("#selectA").remove();
    } else {
        var copyThisA = $(this).clone();
        if ($("#selectA").length > 0) {
            $("#selectA a").html($(this).text());
        } else {
            $(".search-select-result dl").append(copyThisA.attr("id", "selectA"));
        }
    }
});

$(document).on("click", "#select2 dd", function () {
    $(this).addClass("search-selected").siblings().removeClass("search-selected");
    if ($(this).hasClass("select-all")) {
        $("#selectB").remove();
    } else {
        var copyThisB = $(this).clone();
        if ($("#selectB").length > 0) {
            $("#selectB a").html($(this).text());
        } else {
            $(".search-select-result dl").append(copyThisB.attr("id", "selectB"));
        }
    }
});

$(document).on("click", "#select3 dd", function () {
    $(this).addClass("search-selected").siblings().removeClass("search-selected");
    if ($(this).hasClass("select-all")) {
        $("#selectC").remove();
    } else {
        var copyThisC = $(this).clone();
        if ($("#selectC").length > 0) {
            $("#selectC a").html($(this).text());
        } else {
            $(".search-select-result dl").append(copyThisC.attr("id", "selectC"));
        }
    }
});

$(document).on("click", "#selectA", function () {
    $(this).remove();
    $("#select1 .select-all").addClass("search-selected").siblings().removeClass("search-selected");
});

$(document).on("click", "#selectB", function () {
    $(this).remove();
    $("#select2 .select-all").addClass("search-selected").siblings().removeClass("search-selected");
});

$(document).on("click", "#selectC", function () {
    $(this).remove();
    $("#select3 .select-all").addClass("search-selected").siblings().removeClass("search-selected");
});

$(document).on("click", ".search-select-ul dd", function () {
    if ($(".search-select-result dd").length > 1) {
        $(".search-select-no").hide();
    } else {
        $(".search-select-no").show();
    }
});


$(document).on("click", "#search-button", function () {
    var pageContent = $("#page_content:visible");
    var $this = $(this);
    var filter = "?";
    filter = filter + "SearchKeyWord=" + ($("#SearchKeyWord").val() == undefined ? "" : $("#SearchKeyWord").val()) + "&";
    filter = filter + "SearchDateTimeStart=" + ($("#SearchDateTimeStart").val() == undefined ? "" : $("#SearchDateTimeStart").val()) + "&";
    filter = filter + "SearchDateTimeEnd=" + ($("#SearchDateTimeEnd").val() == undefined ? "" : $("#SearchDateTimeEnd").val()) + "&";
    filter = filter + "SearchBiao=" + ($("#SearchBiao").val() == undefined ? "" : $("#SearchBiao").val()) + "&";
    filter = filter + "SearchKu=" + ($("#SearchKu").val() == undefined ? "" : $("#SearchKu").val());
    pageContent.attr("search-filter", filter);
    $.ajax({
        async: true,
        type: "GET",
        cache: false,
        url: pageContent.attr("search-address") + filter,
        dataType: "html",
        success: function (evt) {
            $("#page_content").html(evt);
        }

    });
});