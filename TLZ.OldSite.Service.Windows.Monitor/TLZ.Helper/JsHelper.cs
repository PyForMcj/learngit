#region << 版 本 注 释 >>
/*
     * ========================================================================
     * Copyright Notice © 2010-2014 TideBuy.com All rights reserved .
     * ========================================================================
     * 机器名称：USER-429236GLDJ 
     * 文件名：  JsHelper 
     * 版本号：  V1.0.0.0 
     * 创建人：  wangyunpeng 
     * 创建时间：2014/11/30 10:26:15 
     * 描述    :
     * =====================================================================
     * 修改时间：2014/11/30 10:26:15 
     * 修改人  ：  
     * 版本号  ： V1.0.0.0 
     * 描述    ：
*/
#endregion
namespace TLZ.Helper
{

    /// <summary>
    /// JsHelper 的摘要说明
    /// </summary>
    public class JsHelper
    {
        #region 设置窗体状态栏信息
        /// <summary>
        /// 设置窗体状态栏信息
        ///window.status='{0}'
        /// </summary>
        public const string WinStatusMessage = "window.status='{0}';";
        public const string WinStatusMessageScript = "<script>window.status='{0}';</script>";
        #endregion
        #region 加入收藏夹




        /// <summary>
        /// {0}是名称		{1}是url
        /// </summary>
        public const string AddFavorite = "window.external.AddFavorite('{1}','{0}');";
        public const string AddFavoriteScript = "<script>function Addme(){title='{0}';url='{1}';window.external.AddFavorite(url,title);}</script>";

        #endregion
        #region 打印当前页面
        public const string PrintPageFunction = "function PrintCurPage(){document.all.WebBrowser.ExecWB(7,1);}";
        public const string PrintPageFunctionScript = "<script>function PrintCurPage(){document.all.WebBrowser.ExecWB(7,1);}</script>";

        /// <summary>
        /// 打印当前页面
        /// {0} = frames.PirntArea.document.body.innerHTML //要打印的内容
        /// </summary>
        public const string PrintPage = "var PrintWin=window.open('about:blank','Print');PrintWin.document.write('<object id=\"WebBrowser\" width=0 height=0 classid=\"CLSID:8856F961-340A-11D0-A96B-00C04FD705A2\"></object>'+ {0});PrintWin.document.all.WebBrowser.ExecWB(7,1);PrintWin.close();";
        public const string PrintPageScript = "<script>var PrintWin=window.open('about:blank','Print');PrintWin.document.write('<object id=\"WebBrowser\" width=0 height=0 classid=\"CLSID:8856F961-340A-11D0-A96B-00C04FD705A2\"></object>'+ {0});PrintWin.document.all.WebBrowser.ExecWB(7,1);PrintWin.close();</script>";
        #endregion
        #region 弹出提示信息
        /// <summary>
        /// 弹出提示信息
        /// window.alert('{0}')
        /// </summary>
        public const string WinAlert = "window.alert('{0}');";
        public const string WinAlertScript = "<script>window.alert('{0}');</script>";
        #endregion
        #region 关闭窗体
        public const string CloseParentWindow = "window.parent.close();";
        public const string CloseParentWindowScript = "<script>window.parent.close();</script>";
        /// <summary>
        /// 关闭窗体
        /// window.parent.close()
        /// </summary>
        public const string ParentWinClose = "if(confirm('您确定要关闭吗？')){window.parent.close();}";
        public const string ParentWinCloseScript = "<script>if(confirm('您确定要关闭吗？')){window.parent.close();}</script>";
        #endregion
        #region 返回false
        /// <summary>
        /// 返回false
        /// ;return false
        /// </summary>
        public const string ReturnFalse = "return false;";
        #endregion
        #region 预览上传图片
        /// <summary>
        /// 预览上传图片
        /// </summary>
        public const string PreviewUploadImage = "javascript:document.all['{0}'].src = document.all['{1}'].value;";
        #endregion
        #region 鼠标在控件移动背景色
        /// <summary>
        /// 鼠标在控件移动背景色
        /// this.style.backgroundColor='{0}'
        /// </summary>
        public const string OnMoveOverStyle = "this.style.backgroundColor='{0}';this.style.cursor='{1}';";
        #endregion
        #region 鼠标移出控件背景色




        /// <summary>
        /// 鼠标移出控件背景色




        /// this.style.backgroundColor ='{0}'
        /// </summary>
        public const string OnMoveOutStyle = "this.style.backgroundColor ='{0}';";
        #endregion
        #region 提示信息框




        public const string promptValue = "document.getElementById('{0}').value = window.prompt('{1}','{2}');";
        #endregion
        #region 设置窗体的标题




        /// <summary>
        /// 设置窗体的标题




        /// window.document.title='{0}'
        /// </summary>
        public const string WindowTitle = "window.document.title='{0}';";
        public const string WindowTitleScript = "<script>window.document.title='{0}';</script>";
        public const string WindowParentTitleScript = "<script>window.parent.document.title='{0}';</script>";
        #endregion
        #region 设置控件的值




        /// <summary>
        /// 设置控件的值




        /// window.document.all('{0}').value='{1}';
        /// </summary>
        public const string SetControlValue = "window.document.all('{0}').value='{1}';";
        #endregion
        #region 设置控件焦点
        /// <summary>
        /// 设置控件焦点
        /// document.getElementById('{0}').focusd='{1}';
        /// </summary>
        public const string SetControlFocus = "document.getElementById('{0}').focus();";
        public const string SetControlFocusScript = "<script>document.getElementById('{0}').focus();</script>";
        #endregion
        #region 显示确认窗口
        /// <summary>
        /// 显示确认窗口
        /// return window.confirm('{0}')
        /// </summary>
        public const string ConfirmDailog = "return window.confirm('{0}');";
        public const string ConfirmDailogScript = "javascript:return window.confirm('{0}');";
        #endregion
        #region 设置窗体返回值




        /// <summary>
        /// 设置窗体返回值




        /// window.returnValue=\"{0}\"
        /// </summary>
        public const string SetReturnValue = "window.returnValue=\"{0}\";";
        #endregion
        #region 跳转页面
        /// <summary>
        /// 跳转页面
        /// document.location.href='{0}'
        /// </summary>
        public const string RedirectPage = "document.location.href='{0}'";
        public const string RedirectPageScript = "<script>document.location.href='{0}';</script>";
        public const string RedirectPageScriptAlert = "<script>alert('{0}');document.location.href='{1}';</script>";
        public const string RedirectSelfPageScript = "<script>window.self.location.href='{0}';</script>";
        #endregion
        #region 页面跳转到父窗口
        /// <summary>
        /// 页面跳转到父窗口
        /// window.parent.location.href='{0}'
        /// </summary>
        public const string RedirectParentPage = "window.parent.location.href='{0}'";
        public const string RedirectParentPageScript = "<script>window.parent.location.href='{0}';</script>";
        #endregion
        #region 返回上一页




        /// <summary>
        /// 返回上一页




        /// ms-help://MS.MSDNQTR.2003FEB.2052/dhtml/workshop/author/dhtml/reference/objects/obj_history.htm
        /// </summary>
        public const string ReturnBeforePage = "history.back();";
        public const string ReturnBeforePageScript = "<script>history.back();</script>";
        #endregion
        #region 刷新当前页面
        /// <summary>
        /// 刷新当前页面
        /// </summary>
        public const string RefreshCurrentPageScript = "<script>document.location=document.location;</script>";
        public const string RefreshCurrentPageSelfScript = "<script>window.self.location=window.self.location</script>";
        #endregion
        #region 重载当前页面
        /// <summary>
        /// 重载当前页面
        /// </summary>
        public const string ReloadPage = "document.location.reload();";
        public const string ReloadPageScript = "<script>document.location.reload();</script>";
        #endregion
        #region 刷新父窗口所有页面




        /// <summary>
        /// 刷新父窗口所有页面




        /// </summary>
        public const string RefreshParentPageScript = "<script>window.top.location=window.top.location;</script>";
        #endregion
        #region 鼠标形状
        /// <summary>
        /// 鼠标形状
        /// this.style.cursor='{0}'
        /// </summary>
        public const string Cursor = "this.style.cursor='{0}';";
        #endregion
        #region 设置控件不可见




        /// <summary>
        /// 设置控件不可见




        /// </summary>
        public const string HiddenControl = "document.all(\"{0}\").style.visibility=\"hidden\"";
        #endregion
        #region 将字符串中的所有字母都被转换为小写字母
        /// Javscript字符串函数




        /// ms-help://MS.MSDNQTR.2003FEB.2052/jscript7/html/jsoristringobjectpropmeth.htm
        /// <summary>
        /// 将字符串中的所有字母都被转换为小写字母
        /// </summary>
        public static string toLowerCase(string str)
        {
            string lowerstr = "{0}.toLowerCase();";
            return string.Format(lowerstr, str);
        }
        #endregion
        #region 将字符串中的所有字母都被转换为大写字母
        /// <summary>
        /// /// <summary>
        /// 将字符串中的所有字母都被转换为大写字母
        /// </summary>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string toUpperCase(string str)
        {
            string upperstr = "{0}.toUpperCase();";
            return string.Format(upperstr, str);
        }
        #endregion
        #region 返回打开模式窗体
        /// <summary>
        /// 返回打开模式窗体Script
        /// </summary>
        /// <param name="pageName">打开窗体的页名称</param>
        /// <returns>String</returns>
        public static string OpenModalDialogScript(string pageName)
        {
            return OpenModalDialogScript(pageName, 600, 500);
        }
        /// <summary>
        /// 获取打开模式窗体Script
        /// </summary>
        /// <param name="pageName">打开窗体的页名称</param>
        /// <param name="width">高度</param>
        /// <param name="height">宽度</param>
        /// <returns>String</returns>
        public static string OpenModalDialogScript(string pageName, int width, int height)
        {
            string strOpen = "window.showModelessDialog(\"{0}\",\"\",\"dialogHeight:{1}px; dialogWidth:{2}px;"
                + "  edge: Raised; center: Yes; help: No; resizable: No; status: No;\");return false";
            return string.Format(strOpen, pageName, width, height);
        }
        #endregion
        #region 返回打开模式窗体
        /* window.showModalDialog
		 * ms-help://MS.MSDNQTR.2003FEB.2052/dhtml/workshop/author/dhtml/reference/methods/showmodaldialog.htm
		 */
        /// <summary>
        /// 返回打开模式窗体脚本
        /// </summary>
        /// <param name="PageName">页面文件</param>
        /// <returns>string</returns>
        public static string OpenDialog(string PageName)
        {
            return OpenDialog(PageName, 600, 500);
        }
        /// <summary>
        /// 返回指定高,宽度的模式窗体脚本




        /// </summary>
        /// <param name="PageName">页面文件</param>
        /// <param name="width">窗体宽度</param>
        /// <param name="height">窗体高度</param>
        /// <returns>string</returns>
        public static string OpenDialog(string PageName, int width, int height)
        {
            string strOpen = "window.showModalDialog(\"{0}\",\"\",\"dialogHeight: {1}px; dialogWidth: {2}px; edge: Raised; center: Yes; help: No; resizable: No; status: No;\")";
            return string.Format(strOpen, PageName, height, width);
        }
        public static string OpenDialogScript(string PageName)
        {
            return OpenDialogScript(PageName, 600, 500);
        }
        public static string OpenDialogScript(string PageName, int width, int height)
        {
            string strOpen = "<script>window.showModalDialog(\"{0}\",\"\",\"dialogHeight: {1}px; dialogWidth: {2}px; edge: Raised; center: Yes; resizable:1;scroll:0;help:0;status:0;\")</script>";
            return string.Format(strOpen, PageName, height, width);
        }
        /// <summary>
        /// 返回模式窗体脚本
        /// 并把窗体返回指定到相应的控件
        /// </summary>
        /// <param name="PageName">页面文件</param>
        /// <param name="ControlClientID">接收值控件</param>
        /// <returns>string</returns>
        public static string OpenDialog(string PageName, string ControlClientID)
        {
            return OpenDialog(PageName, 600, 500, ControlClientID);
        }
        /// <summary>
        /// 返回指定高,宽度的模式窗体脚本




        /// 并把窗体返回指定到相应的控件
        /// </summary>
        /// <param name="PageName">页面文件</param>
        /// <param name="width">窗体宽度</param>
        /// <param name="height">窗体高度</param>
        /// <param name="ControlClientID">接收值控件</param>
        /// <returns>string</returns>
        public static string OpenDialog(string PageName, int width, int height, string ControlClientID)
        {
            string strOpen = "var getvalue=window.showModalDialog('{0}','','dialogHeight: {1}px; dialogWidth: {2}px; edge: Raised; center: Yes; help: No; resizable: No; status: No;')";
            strOpen += ";if(getvalue !=null) window.document.all('{3}').value=getvalue";
            return string.Format(strOpen, PageName, height, width, ControlClientID);
        }
        public static string OpenDialogScript(string PageName, string ControlClientID)
        {
            return OpenDialogScript(PageName, 600, 500, ControlClientID);
        }
        public static string OpenDialogScript(string PageName, int width, int height, string ControlClientID)
        {
            string strOpen = "<script>var getvalue=window.showModalDialog('{0}','','dialogHeight: {1}px; dialogWidth: {2}px; edge: Raised; center: Yes; help: No; resizable: Yes; status: Yes;')";
            strOpen += ";if(getvalue !=null) window.document.all('{3}').value=getvalue;</script>";
            return string.Format(strOpen, PageName, height, width, ControlClientID);
        }
        #endregion
        #region 返回打开窗口
        /* window.open
		 * ms-help://MS.MSDNQTR.2003FEB.2052/dhtml/workshop/author/dhtml/reference/methods/open_0.htm
		 */
        /// <summary>
        /// 返回打开窗口脚本
        /// </summary>
        /// <param name="PageName">页面文件</param>
        /// <returns></returns>
        public static string OpenWindow(string PageName)
        {
            return OpenWindow(PageName, 600, 700, "yes", "yes");
        }
        /// <summary>
        /// 返回打开窗口脚本
        /// </summary>
        /// <param name="PageName">页面文件</param>
        /// <param name="width">窗口宽度</param>
        /// <param name="height">窗口高度</param>
        /// <returns></returns>
        public static string OpenWindow(string PageName, int width, int height, string resizable, string scrollbars)
        {
            string strOpen = "var screenleft,screentop; var windowwidth = {0}; var windowheight = {1}; screenleft = Math.round(screen.width/2-windowwidth/2); screentop = Math.round(screen.height/2-windowheight/2); window.open('{2}','','toolbar=no,menubar=no,titilebar=yes,directories=no,resizable={3},scrollbars={4},status=no,fullscreen=no,width='+windowwidth+',height='+windowheight+',left='+screenleft+',top='+screentop+'');";
            return string.Format(strOpen, width, height, PageName, resizable, scrollbars);
        }

        /// <summary>
        /// 返回打开窗口脚本
        /// </summary>
        /// <param name="PageName">页面文件</param>
        /// <returns></returns>
        public static string OpenWindowScript(string PageName)
        {
            return OpenWindowScript(PageName, 640, 530, "yes", "yes");
        }
        /// <summary>
        /// 返回打开窗口脚本
        /// </summary>
        /// <param name="PageName">页面文件</param>
        /// <param name="width">窗口宽度</param>
        /// <param name="height">窗口高度</param>
        /// <returns></returns>
        public static string OpenWindowScript(string PageName, int width, int height, string resizable, string scrollbars)
        {
            string strOpen = "<script>var screenleft,screentop; var windowwidth = {0}; var windowheight = {1}; screenleft = Math.round(screen.width/2-windowwidth/2); screentop = Math.round(screen.height/2-windowheight/2); window.open('{2}','','toolbar=yes,menubar=yes,titilebar=yes,directories=no,resizable={3},scrollbars={4},status=no,fullscreen=no,width='+windowwidth+',height='+windowheight+',left='+screenleft+',top='+screentop+'');</script>";
            return string.Format(strOpen, width, height, PageName, resizable, scrollbars);
        }
        #endregion
        #region cookie
        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="sName">cookie名称</param>
        /// <param name="vValue">cookie值</param>
        /// <param name="datespan">有效期(单位:月)</param>
        /// <returns></returns>
        public static string SetCookie(string sName, string vValue, int datespan)
        {
            string strSetCookie = "function setCookie({0},{1},{2}){var expdate = new Date();expdate.setMonth(expdate.getMonth()+parseInt({2}));var eExpDate = expdate.toGMTString();eExpDate = \"; expires=\" + eExpDate;document.cookie = {0} + \"=\" + escape({1},0) + eExpDate + \";\";}";
            return string.Format(strSetCookie, sName, vValue, datespan);
        }

        /// <summary>
        /// 得到cookie值




        /// </summary>
        /// <param name="sCookieName">cookie名字</param>
        /// <returns></returns>
        public static string GetCookie(string sCookieName)
        {
            string strGetCookie = "function getCookie ({0}){var sName = {0} + \"=\", ichSt, ichEnd;var sCookie = document.cookie;if (sCookie.length && (-1 != (ichSt = sCookie.indexOf(sName)) ) ){if (-1 == (ichEnd = sCookie.indexOf(\";\",ichSt + sName.length) ) )ichEnd = sCookie.length;return unescape(sCookie.substring(ichSt + sName.length,ichEnd));}return null;}";
            return string.Format(strGetCookie, sCookieName);
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="sCookieName">cookie名字</param>
        /// <returns></returns>
        public static string DeleteCookie(string sCookieName)
        {
            string strDeleteCookie = "function deleteCookie ({0}){document.cookie = {0} + \"=\"" + GetCookie(sCookieName) + "\";expires=\" + (new Date()).toGMTString() + \";\";}";
            return string.Format(strDeleteCookie, sCookieName);
        }
        #endregion
        #region 添加用来显示提示内容的提示框

        /// <summary>
        /// 添加用来显示提示内容的提示框
        /// </summary>
        public const string DivTip = "document.write(\"<div id='div_content' style='visibility:hidden'></div>\");";
        public const string DivTipScript = "<script>document.write(\"<div id='div_content' style='visibility:hidden'></div>\");</script>";
        /// <summary>
        /// text //要显示的内容
        /// </summary>
        public const string ShowTip = "function tip_show(text){if (document.all&&document.readyState==\"complete\"){document.all.div_content.innerHTML='<table border=0 bgcolor=\"#FFFFFF\" cellpadding=\"0\" cellspacing=\"1\"><tr><td valign=middle algin=center>'text'</td></tr></table>';document.all.div_content.style.pixelLeft=event.clientX+document.body.scrollLeft+10;document.all.div_content.style.pixelTop=event.clientY+document.body.scrollTop+10;document.all.div_content.style.visibility=\"visible\";}}";
        public const string ShowTipScript = "<script>function tip_show(text){if (document.all&&document.readyState==\"complete\"){document.all.div_content.innerHTML='<table border=0 bgcolor=\"#FFFFFF\" cellpadding=\"0\" cellspacing=\"1\"><tr><td valign=middle algin=center>'text'</td></tr></table>';document.all.div_content.style.pixelLeft=event.clientX+document.body.scrollLeft+10;document.all.div_content.style.pixelTop=event.clientY+document.body.scrollTop+10;document.all.div_content.style.visibility=\"visible\";}}</script>";

        public const string HideTip = "function tip_hide(){if (document.all){document.all.div_content.style.visibility=\"hidden\";}else if (document.layers){document.all.div_content.visibility=\"hidden\";}}";
        public const string HideTipScript = "<script>function tip_hide(){if (document.all){document.all.div_content.style.visibility=\"hidden\";}else if (document.layers){document.all.div_content.visibility=\"hidden\";}}</script>";
        #region Notes
        //		Javascript syntax
        //		text is tip's content
        //		function tip_show(text)
        //		{
        //
        //			if (document.all&&document.readyState=="complete")
        //			{
        //				document.all.div_content.innerHTML='<table border=1 bgcolor="#FFFFFF" cellpadding="0" cellspacing="1"><tr><td valign=top>'+text+'</td></tr></table>';
        //				document.all.div_content.style.pixelLeft=event.clientX+document.body.scrollLeft+10;
        //				document.all.div_content.style.pixelTop=event.clientY+document.body.scrollTop+10;
        //				document.all.div_content.style.visibility="visible";
        //			}
        //		}
        //		function tip_hide()
        //		{
        //			if (document.all)
        //				document.all.div_content.style.visibility="hidden";
        //			else if (document.layers)
        //			{
        //				document.all.div_content.visibility="hidden";
        //			}
        //		}
        #endregion
        #endregion
        #region 改变鼠标经过的样子




        #region Notes
        //		javascript syntax
        //		obj often is Table tag
        //		function mouseover(obj)
        //		{
        //			obj.cells(0).innerHTML="<img src='../work/images/ml_2.gif' border='0'>";
        //			obj.cells(1).background ="../work/images/mbg_2.gif";
        //			obj.cells(2).innerHTML="<img src='../work/images/mr_2.gif' border='0'>";
        //		}
        //		function mouseout(obj)
        //		{
        //			obj.cells(0).innerHTML="<img src='../work/images/ml_1.gif' border='0'>";
        //			obj.cells(1).background ="../work/images/mbg_1.gif";
        //			obj.cells(2).innerHTML="<img src='../work/images/mr_1.gif' border='0'>";
        //		}
        #endregion
        #endregion
        #region 事件
        /// <summary>
        /// MouseUp事件无效
        /// </summary>
        public const string EventMouseUpInvalid = "onmouseup=\"document.selection.empty()\"";
        /// <summary>
        /// 上下文菜单事件无效




        /// </summary>
        public const string EventContextMenuInvalid = "oncontextmenu=\"return false\"";
        /// <summary>
        /// 选择事件无效
        /// </summary>
        public const string EventSelectStartInvalid = "onselectstart=\"return false\"";
        /// <summary>
        /// 拖放事件无效
        /// </summary>
        public const string EventDragStartInvalid = "ondragstart=\"return false\"";
        /// <summary>
        /// 开始复制事件无效




        /// </summary>
        public const string EventBeforeCopyInvalid = "onbeforecopy=\"return false\"";
        /// <summary>
        /// 复制事件无效
        /// </summary>
        public const string EvnertCopyInvalid = "oncopy=\"document.selection.empty()\"";
        /// <summary>
        /// 屏蔽鼠标右键
        /// document.oncontextmenu()
        /// </summary>
        /// <returns></returns>
        public const string EventMouseRightInvalid = "function document.oncontextmenu(){event.returnValue=false;}";
        public const string EventMouseRightInvalidScript = "<script>function document.oncontextmenu(){event.returnValue=false;}</script>";

        /// <summary>
        /// 屏蔽F1帮助
        /// window.onhelp()
        /// </summary>
        public const string EventHelpInvalid = "function window.onhelp(){return false}";
        public const string EventHelpInvalidScript = "<script>function window.onhelp(){return false}</script>";

        /// <summary>
        /// 屏蔽功能键




        /// document.onkeydown()
        /// </summary>
        public const string FunctionKeysInvalid = "function document.onkeydown(){if ((window.event.altKey)&&((window.event.keyCode==37)||(window.event.keyCode==39))){event.returnValue=false;}if (event.keyCode==8){event.keyCode=0;event.returnValue=false;}if (event.ctrlKey && event.keyCode==82){event.keyCode=0;event.returnValue=false;}if (event.keyCode==122){event.keyCode=0;event.returnValue=false;}if (event.ctrlKey && event.keyCode==78) {event.returnValue=false;}if (event.shiftKey && event.keyCode==121){event.returnValue=false;}if (window.event.srcElement.tagName == \"A\" && window.event.shiftKey){window.event.returnValue = false;}if ((window.event.altKey)&&(window.event.keyCode==115)){window.showModelessDialog(\"about:blank\",\"\",\"dialogWidth:1px;dialogheight:1px\");return false;}";
        public const string FunctionKeysInvalidScript = "<script>function document.onkeydown(){if ((window.event.altKey)&&((window.event.keyCode==37)||(window.event.keyCode==39))){event.returnValue=false;}if (event.keyCode==8){event.keyCode=0;event.returnValue=false;}if (event.ctrlKey && event.keyCode==82){event.keyCode=0;event.returnValue=false;}if (event.keyCode==122){event.keyCode=0;event.returnValue=false;}if (event.ctrlKey && event.keyCode==78) {event.returnValue=false;}if (event.shiftKey && event.keyCode==121){event.returnValue=false;}if (window.event.srcElement.tagName == \"A\" && window.event.shiftKey){window.event.returnValue = false;}if ((window.event.altKey)&&(window.event.keyCode==115)){window.showModelessDialog(\"about:blank\",\"\",\"dialogWidth:1px;dialogheight:1px\");return false;}</script>";
        #region Notes
        //		JavaScript 
        //		function document.onkeydown() 
        //		{ 
        //			if ((window.event.altKey)&&
        //			((window.event.keyCode==37)||   //屏蔽 Alt+ 方向键 ← 
        //			(window.event.keyCode==39)))   //屏蔽 Alt+ 方向键 → 
        //			{ 
        //				event.returnValue=false; 
        //			} 
        //			//if ((event.keyCode==8)  ||                 //屏蔽退格删除键 
        //			//(event.ctrlKey && event.keyCode==82)){ //Ctrl + R 
        //			//event.keyCode=0; 
        //			//event.returnValue=false; 
        //			//} 
        //			if (event.keyCode==8)
        //			{
        //				event.keyCode=0;
        //				event.returnValue=false;
        //			}
        //   
        //			if (event.ctrlKey && event.keyCode==82)
        //			{
        //				event.keyCode=0;
        //				event.returnValue=false;
        //			}
        //			if (event.keyCode==122){event.keyCode=0;event.returnValue=false;}  //屏蔽F11 
        //			if (event.ctrlKey && event.keyCode==78) event.returnValue=false;   //屏蔽 Ctrl+n 
        //			if (event.shiftKey && event.keyCode==121)event.returnValue=false;  //屏蔽 shift+F10 
        //			if (window.event.srcElement.tagName == "A" && window.event.shiftKey)  
        //				window.event.returnValue = false;             //屏蔽 shift 加鼠标左键新开一网页 
        //			if ((window.event.altKey)&&(window.event.keyCode==115))             //屏蔽Alt+F4 
        //			{ 
        //				window.showModelessDialog("about:blank","","dialogWidth:1px;dialogheight:1px"); 
        //				return false; 
        //			} 
        //		}
        #endregion
        #endregion
        #region 验证Email格式
        /*验证email格式
		 * function IsValidEmail(email)
		{
			var string;
			string = new String(email);
			var len = string.length;
			if (string.indexOf("@",1)==-1 || string.indexOf(".",1)==-1 || string.length<7)
			{
				alert('搜索网站URL格式不正确');
				return false;
			}
			if (string.charAt(len-1)=="." || string.charAt(len-1)=="@")
			{
				alert('搜索网站URL格式不正确');
				return false;
			}
			return true;
		}
		*/
        #endregion
    }
}
