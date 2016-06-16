/// <reference path="../jquery-1.12.3.js" />

jQuery(document).ready(function () {

    /*
     *调用backstretch插件进行背景图片处理
     */
    $.backstretch("../Images/Login/login-background.jpg");

    /*
     *表单
     */
    $('.login-form input[type="text"], .login-form input[type="password"], .login-form textarea').on('focus', function () {
        $(this).removeClass('input-error');
    });

    $('.login-form').on('submit', function (e) {

        $(this).find('input[type="text"], input[type="password"], textarea').each(function () {
            if ($(this).val() == "") {
                e.preventDefault();
                $(this).addClass('input-error');
            }
            else {
                $(this).removeClass('input-error');
            }
        });
    });


});
