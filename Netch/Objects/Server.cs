﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Netch.Objects
{
    public class Server
    {
        /// <summary>
        ///     备注
        /// </summary>
        public string Remark;

        /// <summary>
        ///     组
        /// </summary>
        public string Group = "None";

        /// <summary>
        ///     类型（Socks5、Shadowsocks、ShadowsocksR、VMess）
        /// </summary>
        public string Type;

        /// <summary>
        ///     地址
        /// </summary>
        public string Address;

        /// <summary>
        ///     端口
        /// </summary>
        public int Port;

        /// <summary>
        ///     用户名
        /// </summary>
        public string Username;

        /// <summary>
        ///     密码
        /// </summary>
        public string Password;

        /// <summary>
        ///     加密方式
        /// </summary>
        public string EncryptMethod;

        /// <summary>
        ///     协议
        /// </summary>
        public string Protocol;

        /// <summary>
        ///     协议参数
        /// </summary>
        public string ProtocolParam;

        /// <summary>
        ///     混淆
        /// </summary>
        public string OBFS;

        /// <summary>
        ///     混淆参数
        /// </summary>
        public string OBFSParam;

        /// <summary>
        ///     延迟
        /// </summary>
        public int Delay = -1;

        /// <summary>
		///		获取备注
		/// </summary>
		/// <returns>备注</returns>
		public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(Remark))
            {
                Remark = $"{Address}:{Port}";
            }

            switch (Type)
            {
                case "Socks5":
                    return $"[S5] {Remark}";
                case "Shadowsocks":
                    return $"[SS] {Remark}";
                case "ShadowsocksR":
                    return $"[SR] {Remark}";
                default:
                    return "WTF";
            }
        }

        /// <summary>
        ///		测试延迟
        /// </summary>
        /// <returns>延迟</returns>
        public int Test()
        {
            try
            {
                var destination = Dns.GetHostAddressesAsync(Address);
                if (!destination.Wait(1000))
                {
                    return Delay = 999;
                }

                if (destination.Result.Length == 0)
                {
                    return Delay = 999;
                }

                var list = new Task<int>[3];
                for (int i = 0; i < 3; i++)
                {
                    list[i] = Task.Run<int>(() =>
                    {
                        try
                        {
                            using (var client = new Socket(SocketType.Stream, ProtocolType.Tcp))
                            {
                                var watch = new Stopwatch();
                                watch.Start();

                                var task = client.BeginConnect(new IPEndPoint(destination.Result[0], Port), (result) =>
                                {
                                    watch.Stop();
                                }, 0);

                                if (task.AsyncWaitHandle.WaitOne(1000))
                                {
                                    return (int)(watch.ElapsedMilliseconds >= 460 ? 460 : watch.ElapsedMilliseconds);
                                }

                                return 999;
                            }
                        }
                        catch (Exception)
                        {
                            return 999;
                        }
                    });
                }

                Task.WaitAll(list);

                return Delay = (list[0].Result + list[1].Result + list[2].Result) / 3;
            }
            catch (Exception)
            {
                return Delay = 999;
            }
        }
    }
}
