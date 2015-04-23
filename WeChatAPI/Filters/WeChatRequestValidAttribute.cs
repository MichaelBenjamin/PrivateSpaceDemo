﻿using System;
using System.Web.Mvc;
using System.Web.Security;

namespace PrivateSpace.WeChatAPI.Filters
{
    public class WeChatRequestValidAttribute : ActionFilterAttribute
    {
        private const string Token = "PrivateSpace";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //参数适配
            Model.FormatModel.WeChatMsgRequestModel model = new Model.FormatModel.WeChatMsgRequestModel() { nonce= filterContext.HttpContext.Request.QueryString["nonce"],msg_signature= filterContext.HttpContext.Request.QueryString["msg_signature"],timestamp= filterContext.HttpContext.Request.QueryString["timestamp"] };
            //验证
            if (CheckSignature(model))
            {
                base.OnActionExecuting(filterContext);
            }        
        }

        private bool CheckSignature(Model.FormatModel.WeChatMsgRequestModel model)
        {
            string signature, timestamp, nonce, tempStr;
            //获取请求来的参数
            signature = model.msg_signature;
            timestamp = model.timestamp;
            nonce = model.nonce;
            //创建数组，将 Token, timestamp, nonce 三个参数加入数组
            string[] array = { Token, timestamp, nonce };
            //进行排序
            Array.Sort(array);
            //拼接为一个字符串
            tempStr = String.Join("", array);
            //对字符串进行 SHA1加密
            tempStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tempStr, "SHA1").ToLower();
            //判断signature 是否正确
            if (tempStr.Equals(signature))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
