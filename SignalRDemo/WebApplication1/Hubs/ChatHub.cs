﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace WebApplication1
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }

        public static List<User> users = new List<User>();


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="connectionId">目标ID</param>
        /// <param name="message"></param>
        public void SendMessage(string connectionId, string message)
        {
            //Clients.All.hello();
            var user = users.Where(s => s.ConnectionID == connectionId).FirstOrDefault();
            if (user != null)
            {
                //connectionId：指定对方的connectionId
                Clients.Client(connectionId).addMessage(DateTime.Now+"："+ message, Context.ConnectionId);
                // Context.ConnectionId 当前页面的connectionId 使自己也能看到自己发送出去的消息
                Clients.Client(Context.ConnectionId).addMessage(DateTime.Now+"："+message, connectionId);
            }
            else
            {
                Clients.Client(Context.ConnectionId).showMessage("该用户已离线");
            }
        }
        //[HubMethodName("exitChat")]
        public void GetName(string name)
        {
            //查询用户
            var user = users.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (user != null)
            {
                user.Name = name;
                Clients.Client(Context.ConnectionId).showId(Context.ConnectionId);
            }
            GetUsers();
        }

        /// <summary>
        /// 重写连接事件
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            //查询用户
            var user = users.Where(w => w.ConnectionID == Context.ConnectionId).SingleOrDefault();
            //判断用户是否存在，否则添加集合
            if (user == null)
            {
                user = new User("", Context.ConnectionId);
                users.Add(user);
            }
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var user = users.Where(p => p.ConnectionID == Context.ConnectionId).FirstOrDefault();
            //判断用户是否存在，存在则删除
            if (user != null)
            {
                //删除用户
                users.Remove(user);
            }
            GetUsers();//获取所有用户的列表
            return base.OnDisconnected(stopCalled);
        }
        //获取所有用户在线列表
        private void GetUsers()
        {
            var list = users.Select(s => new { s.Name, s.ConnectionID }).ToList();
            string jsonList = JsonConvert.SerializeObject(list);
            Clients.All.getUsers(jsonList);
        }
    }
    public class User
    {
        [Key]
        public string ConnectionID { get; set; }
        public string Name { get; set; }
        public User(string name, string connectionId)
        {
            this.Name = name;
            this.ConnectionID = connectionId;
        }
    }
}