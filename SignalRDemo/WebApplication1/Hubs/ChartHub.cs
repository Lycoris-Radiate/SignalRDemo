using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebApplication1.Hubs
{
    /// <summary>
    /// 数据广播器
    /// </summary>
    public class Broadcaster
    {
        private readonly static Lazy<Broadcaster> _instance =
            new Lazy<Broadcaster>(() => new Broadcaster());

        private readonly IHubContext _hubContext;

        private Timer _broadcastLoop;

        public Broadcaster()
        {
            // 获取所有连接的句柄，方便后面进行消息广播
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<ChartHub>();
            //定时器
            _broadcastLoop = new Timer(
                BroadcastShape,
                null,
                1000,
                1000);

        }
        private Random random = new Random();


        private void BroadcastShape(object state)
        {
            // 定期执行的方法
            int[] values = new int[10];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = random.Next(100);
            }
            // 定期执行的方法
            _hubContext.Clients.All.sendTest1(values);
        }



        public static Broadcaster Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
  [HubName("ChartHub")]
    public class ChartHub : Hub
    {
        private Broadcaster _broadcaster;
        //在构造函数里边在调用其它形式的构造函数是编译不过去的.
        //构造函数后边加:this(....)就是再调用其他形式的构造函数
        public ChartHub() : this(Broadcaster.Instance)
        {

        }
        public ChartHub(Broadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }
    }
}