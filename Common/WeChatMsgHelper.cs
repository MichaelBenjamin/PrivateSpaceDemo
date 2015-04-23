﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
{
    public class WeChatMsgHelper
    {
        private static string urlFormat = "http://www.xiaohuangji.com/ajax.php/para={0}";
        private static string url = "http://www.xiaohuangji.com/ajax.php";
        private static LogHelper logger = new LogHelper(typeof(WeChatMsgHelper));
        public string ReturnMessage(string postStr)
        {
            string responseContent = "";
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(postStr);
                //xmldoc.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("GB2312").GetBytes(postStr)));
                XmlNode MsgType = xmldoc.SelectSingleNode("/xml/MsgType");
                if (MsgType != null)
                {
                    switch (MsgType.InnerText)
                    {
                        case "event":
                            responseContent = EventHandle(xmldoc);//事件处理
                            break;
                        case "text":
                            responseContent = TextMsgHandle(xmldoc);//接受文本消息处理
                            break;
                        case "image":
                            responseContent = ImageMsgHandle(xmldoc);//图片消息
                            break;
                        case "voice":
                            responseContent = VoiceMsgHandle(xmldoc);//语音消息
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("发生异常，异常信息：" + ex.Message + ex.StackTrace);
            }
            return responseContent;
        }

        private string VoiceMsgHandle(XmlDocument xmldoc)
        {
            string responseContent = "", reply=null;
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Recognition");
            if (Content != null)
            {
                //设置回复消息
                reply =  Common.Helper.DoPostRequest(url,"para="+Content.InnerText);
                if (reply == "网络异常") reply = "The service is not available now,please retry later";
                //reply = "这是回复";
                responseContent = string.Format(ReplyType.Message_Text,
                    FromUserName.InnerText,
                    ToUserName.InnerText,
                    DateTime.Now.Ticks,
                    //"您发送的消息是："+Content.InnerText+"\r\n 我的回复："+reply + "\r\n<a href=\"http://private.chinacloudsites.cn/\">点击进入我们官网</a>"
                    String.IsNullOrEmpty(reply)?"Sorry,I can not follow you." :reply);
            }
            logger.Debug("接受的消息："+Content.InnerText + "\r\n 发送的消息：" + reply);
            return responseContent;
        }

        private string ImageMsgHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode MediaId = xmldoc.SelectSingleNode("/xml/MediaId");
            if (MediaId != null)
            {
                //reply = "这是回复";
                responseContent = string.Format(ReplyType.Message_Image,
                    FromUserName.InnerText,
                    ToUserName.InnerText,
                    DateTime.Now.Ticks,
                    //"您发送的消息是："+Content.InnerText+"\r\n 我的回复："+reply + "\r\n<a href=\"http://private.chinacloudsites.cn/\">点击进入我们官网</a>"
                    MediaId.InnerText
                    );
            }
            return responseContent;
        }

        private string TextMsgHandle(XmlDocument xmldoc)
        {
            string responseContent = "",reply="";
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
            if (Content != null)
            {
                //设置回复消息
                reply = Common.Helper.DoPostRequest(url, "para=" + Content.InnerText);
                if (reply == "网络异常") reply = "The service is not available now,please retry later";
                //reply = "这是回复";
                responseContent = string.Format(ReplyType.Message_Text,
                    FromUserName.InnerText,
                    ToUserName.InnerText,
                    DateTime.Now.Ticks,
                    reply);
            }
            logger.Debug("接受的消息：" + Content.InnerText + "\r\n 发送的消息：" + reply);
            return responseContent;
        }

        private string EventHandle(XmlDocument xmldoc)
        {
            string responseContent = "";
            XmlNode Event = xmldoc.SelectSingleNode("/xml/Event");
            XmlNode EventKey = xmldoc.SelectSingleNode("/xml/EventKey");
            XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
            XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
            if (Event != null)
            {
                //菜单单击事件
                if (Event.InnerText.Equals("CLICK"))
                {
                    if (EventKey.InnerText.Equals("click_one"))//click_one
                    {
                        responseContent = string.Format(ReplyType.Message_Text,
                            FromUserName.InnerText,
                            ToUserName.InnerText,
                            DateTime.Now.Ticks,
                            "你点击的是click_one");
                    }
                    else if (EventKey.InnerText.Equals("click_two"))//click_two
                    {
                        responseContent = string.Format(ReplyType.Message_News_Main,
                            FromUserName.InnerText,
                            ToUserName.InnerText,
                            DateTime.Now.Ticks,
                            "2",
                             string.Format(ReplyType.Message_News_Item, "我要寄件", "",
                             "http://www.soso.com/orderPlace.jpg",
                             "http://www.soso.com/") +
                             string.Format(ReplyType.Message_News_Item, "订单管理", "",
                             "http://www.soso.com/orderManage.jpg",
                             "http://www.soso.com/"));
                    }
                    else if (EventKey.InnerText.Equals("click_three"))//click_three
                    {
                        responseContent = string.Format(ReplyType.Message_News_Main,
                            FromUserName.InnerText,
                            ToUserName.InnerText,
                            DateTime.Now.Ticks,
                            "1",
                             string.Format(ReplyType.Message_News_Item, "标题", "摘要",
                             "http://www.soso.com/jieshao.jpg",
                             "http://www.soso.com/"));
                    }
                }
            }
            return responseContent;
        }
    }

    //回复类型
    public static class ReplyType
    {
        /// <summary>
        /// 普通文本消息
        /// </summary>
        public static string Message_Text
        {
            get { return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[text]]></MsgType>
                            <Content><![CDATA[{3}]]></Content>
                          </xml>"; }
        }

        /// <summary>
        /// 图文消息主体
        /// </summary>
        public static string Message_News_Main
        {
            get
                {
                return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[news]]></MsgType>
                            <ArticleCount>{3}</ArticleCount>
                            <Articles>
                            {4}
                            </Articles>
                            </xml> ";
            }
        }

        /// <summary>
        /// 图文消息项
        /// </summary>
        public static string Message_News_Item
        {
            get
                {
                return @"<item>
                            <Title><![CDATA[{0}]]></Title> 
                            <Description><![CDATA[{1}]]></Description>
                            <PicUrl><![CDATA[{2}]]></PicUrl>
                            <Url><![CDATA[{3}]]></Url>
                         </item>";
            }
        }

        public static string Message_Image
        {
            get { return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[image]]></MsgType>
                            <Image>
                                <MediaId><![CDATA[{3}]]></MediaId>
                            </Image>
                          </xml>"; }
        }
    }
}
