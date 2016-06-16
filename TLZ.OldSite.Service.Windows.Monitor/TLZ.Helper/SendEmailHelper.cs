using System;
using System.Net.Mail;
using System.Text;

namespace TLZ.Helper
{
    /// <summary>
    /// sendEmail 的摘要说明
    /// </summary>
    public static class SendEmailHelper
    {
        private const string _from = "basic_noreply@tidebuy.com";  //发件人邮箱地址
        private const string _username = "basic_noreply@tidebuy.com"; //固定
        private const string _password = "E^Lk%0^7nzVG"; //固定
        private const string _server = "mail.tidebuy.com";    //那个 smtp

        public static string SendMail(string to, string body, string subject)
        {
            return SendHtml(_from, to, subject, body, _server, _username, _password);
        }

        #region MyRegion
        /// <summary>
        /// 发送邮件程序
        /// </summary>
        /// <param name="from">发送人邮件地址</param>
        /// <param name="fromname">发送人显示名称</param>
        /// <param name="to">发送给谁（邮件地址）</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="username">邮件登录名</param>
        /// <param name="password">邮件密码</param>
        /// <param name="server">邮件服务器 smtp服务器地址</param>
        /// <param name="fujian">附件</param>
        /// <returns>send ok</returns>       
        public static string SendMail(string from, string fromname, string to, string subject, string body, string username, string password, string server, string fujian)
        {
            try
            {
                //邮件发送类
                MailMessage mail = new MailMessage();
                //是谁发送的邮件
                mail.From = new MailAddress(from, fromname);
                //发送给谁
                mail.To.Add(to);
                //标题
                mail.Subject = subject;
                //内容编码
                mail.BodyEncoding = Encoding.Default;
                //发送优先级
                mail.Priority = MailPriority.High;
                //邮件内容
                mail.Body = body;
                //是否HTML形式发送
                mail.IsBodyHtml = true;
                //附件
                if (fujian.Length > 0)
                {
                    mail.Attachments.Add(new Attachment(fujian));
                }
                //邮件服务器和端口
                SmtpClient smtp = new SmtpClient(server, 465);
                smtp.UseDefaultCredentials = true;
                //指定发送方式
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //指定登录名和密码
                smtp.Credentials = new System.Net.NetworkCredential(username, password);

                //mail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1"); //basic authentication 
                //mail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", username); //set your username here 
                //mail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", password); //set your password here

                //超时时间
                smtp.EnableSsl = false;
                smtp.Timeout = 10000;
                smtp.Send(mail);
                return "success";
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        #endregion


        ///   <summary>
        ///   发送邮件
        ///   </summary>
        ///   <param   name= "server "> smtp地址 </param>
        ///   <param   name= "username "> 用户名 </param>
        ///   <param   name= "password "> 密码 </param>
        ///   <param   name= "from "> 发信人地址 </param>
        ///   <param   name= "to "> 收信人地址 </param>
        ///   <param   name= "subject "> 邮件标题 </param>
        ///   <param   name= "body "> 邮件正文 </param>
        ///   <param   name= "IsHtml "> 是否是HTML格式的邮件 </param>
        public static string SendMail(string from, string to, string subject, string body, string server, string username, string password, bool IsHtml)
        {
            try
            {
                //设置SMTP 验证,端口默认为25，如果需要其他请修改
                SmtpClient mailClient = new SmtpClient(server, 25);
                //指定如何发送电子邮件。
                //Network   电子邮件通过网络发送到   SMTP   服务器。    
                //PickupDirectoryFromIis   将电子邮件复制到挑选目录，然后通过本地   Internet   信息服务   (IIS)   传送。    
                //SpecifiedPickupDirectory 将电子邮件复制到 SmtpClient.PickupDirectoryLocation 属性指定的目录，然后由外部应用程序传送。  
                mailClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                //创建邮件对象
                MailMessage mailMessage = new MailMessage(from, to, subject, body);
                //定义邮件正文，主题的编码方式
                mailMessage.BodyEncoding = System.Text.Encoding.GetEncoding("gb2312");
                mailMessage.SubjectEncoding = System.Text.Encoding.GetEncoding("gb2312");
                //mailMessage.BodyEncoding = Encoding.Default;
                //获取或者设置一个值，该值表示电子邮件正文是否为HTML
                mailMessage.IsBodyHtml = IsHtml;
                //指定邮件的优先级
                mailMessage.Priority = MailPriority.High;
                //发件人身份验证,否则163   发不了
                //表示当前登陆用户的默认凭据进行身份验证，并且包含用户名密码
                mailClient.UseDefaultCredentials = true;
                mailClient.Credentials = new System.Net.NetworkCredential(username, password);
                //发送
                mailClient.Send(mailMessage);
                return "success";
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        //发送HTML内容
        public static string SendHtml(string from, string to, string subject, string body, string server, string username, string password)
        {
            return SendMail(from, to, subject, body, server, username, password, true);
        }

        ////第三方邮件接口
        //public static void SendMailByApi(string callerGUID, EmailLevel emailLevel, string emailTo, string emailFrom, string title, string context, DateTime sendTime)
        //{
        //    using (SendMailApi.EmailWorkClient workClient = new EmailWorkClient())
        //    {
        //        try
        //        {
        //            workClient.AddEmail(callerGUID, emailLevel, emailTo, emailFrom, title, context, sendTime);
        //        }
        //        catch (TimeoutException ex)
        //        {

        //            throw new Exception("邮件发送超时！");
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("邮件发送失败！" + ex.Message);
        //        }
        //        finally
        //        {

        //            workClient.Abort();
        //        }


        //    }


        //}


    }


    public class MailContent
    {
        public string Body { get; set; }
        public string Subject { get; set; }
    }

    public class DeliveryMailContent
    {
        public string CallGUID { get; set; }

        public string Receiver { get; set; }

        public string Sender { get; set; }

        public string Title { get; set; }

        public string Context { get; set; }

        public DateTime SendTime { get; set; }
    }

    public static class MailTemplate
    {

        public static MailContent ForgetPasswordTemplate(string userName, string retrievePasswordAddress)
        {
            return new MailContent()
            {
                Body = @"<div> 
亲爱的用户 " + userName + @"：您好！<br><br>
&nbsp;&nbsp;&nbsp; 您收到这封这封电子邮件是因为您 (也可能是某人冒充您的名义) 申请了一个新的密码。假如这不是您本人所申请, 请不用理会这封电子邮件, 但是如果您持续收到这类的信件骚扰, 请您尽快联络管理员。
<br><br> 
&nbsp;&nbsp;&nbsp;要使用新的密码, 请使用以下链接启用密码。<br><br>
&nbsp;&nbsp;&nbsp; <a href='" + retrievePasswordAddress + "' target='_blank'>" + retrievePasswordAddress + @"</a><br><br>
 &nbsp;&nbsp;&nbsp; (如果无法点击该URL链接地址，请将它复制并粘帖到浏览器的地址输入框，然后单击回车即可。该链接有效期为12小时，使用后将立即失效。)用户服务支持：
<a href='http://cn.tidebuy.com' target='_blank'>踏浪者国际</a><br><br>
</div>",
                Subject = "配置中心-密码找回"
            };
        }


    }

}