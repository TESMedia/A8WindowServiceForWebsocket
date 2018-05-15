using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Webscoket.Models;
using ZeroMQ;

namespace Webscoket
{
    [HubName("MyHub")]
    public class MyHub : Hub
    {
        public void Send(string PublisherUrl)
        {
            Console.WriteLine("MyHub connected to server");
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            string topic = "device";
            string url = PublisherUrl;
            using (var context = new ZContext())
            using (var subscriber = new ZSocket(context, ZSocketType.SUB))
            {
                //Create the Subscriber Connection
                subscriber.Connect(url);
                Console.WriteLine("Subscriber started for Topic with URL : {0} {1}", topic, url);
                subscriber.Subscribe(topic);
                int subscribed = 0;

                //List<ThomsonData> lstLocationData = new List<ThomsonData>();
                List<LocationData> lstLocationData = new List<LocationData>();

                while (true)
                {
                    using (ZMessage message = subscriber.ReceiveMessage())
                    {
                        subscribed++;
                        string contents = message[1].ReadString();
                        Console.WriteLine(contents);

                        LocationData objLocationData = JsonConvert.DeserializeObject<ListOfArea>(contents).device_notification.records.FirstOrDefault();
                        DateTime macFoundDatetime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(objLocationData.last_seen_ts);
                        objLocationData.LastSeenDatetime = macFoundDatetime;
                        //Remove the data if not present on the list for 30 minutes
                        //lstLocationData.Remove(lstLocationData.FirstOrDefault(m => m.LastSeenDatetime.AddMinutes(10) < DateTime.Now));

                        if (lstLocationData.Any(m => m.mac == objLocationData.mac))
                        {
                            Console.WriteLine("Enter into the section to Update old macAddress");
                            //Update the particular MacAddress from the List of data
                            var LocationDataObject = lstLocationData.FirstOrDefault(m => m.mac == objLocationData.mac);
                            //If old MacAddress then just update the x and y axis of the column
                            LocationDataObject.x = objLocationData.x;
                            LocationDataObject.y = objLocationData.y;
                        }
                        else
                        {
                            if (ConnectToDataBase.CheckMacAdressIsTrackableOrNot(objLocationData.mac) == 1)
                            {
                                Console.WriteLine("Enter into the section to Add new macAddress");
                                lstLocationData.Add(objLocationData);
                            }
                        }

                        string strReturn = JsonConvert.SerializeObject(lstLocationData);
                        Clients.All.addMessage(strReturn);
                        Console.Write("Added successfully to server");
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
