using MeterActor.Interfaces;
using MeterGroupActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                var args = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length > 0)
                {
                    try
                    {

                        switch (args[0].ToLowerInvariant())
                        {
                            case "register":
                                {
                                    AssertArgCount(3, args);
                                    RegisterDevice(long.Parse(args[1]), long.Parse(args[2]));
                                    break;
                                }
                            case "devicestatus":
                                {
                                    AssertArgCount(2, args);
                                    GetDeviceStatus(long.Parse(args[1]));
                                    break;
                                }
                            case "listen":
                                {
                                    AssertArgCount(2, args);
                                    ListenToMeter(long.Parse(args[1]));
                                    break;
                                }
                            case "groupstatus":
                                {
                                    AssertArgCount(2, args);
                                    GetGroupStatus(long.Parse(args[1]));
                                    break;
                                }
                            case "exit":
                                {
                                    return;
                                }
                            default:
                                {
                                    Console.WriteLine("Bad command");
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Bad command: {0}", ex.Message);
                    }
                }
            }
        }

        private static void RegisterDevice(long deviceId, long groupId)
        {
            var meter = ActorProxy.Create<IMeterActor>(new ActorId(deviceId), "fabric:/ActorDemo");
            meter.ActivateAsync(deviceId, groupId).Wait();

            Console.WriteLine("Ok");
        }

        private static void GetDeviceStatus(long deviceId)
        {
            var meter = ActorProxy.Create<IMeterActor>(new ActorId(deviceId), "fabric:/ActorDemo");
            var reading = meter.GetReading().Result;


            Console.WriteLine("Last reading: {0}", reading);
        }

        private static void ListenToMeter(long deviceId)
        {
            var meter = ActorProxy.Create<IMeterActor>(new ActorId(deviceId), "fabric:/ActorDemo");
            var reading = meter.GetReading().Result;

            var eventHandler = new ReadingEventHandler();

            meter.SubscribeAsync<IReadingEvents>(eventHandler).Wait();

            Console.ReadLine();

            meter.UnsubscribeAsync<IReadingEvents>(eventHandler).Wait();
        }

        private static void GetGroupStatus(long groupId)
        {
            var meterGroup = ActorProxy.Create<IMeterGroupActor>(new ActorId(groupId), "fabric:/ActorDemo");
            var status = meterGroup.GetStatusAsync().Result;

            Console.WriteLine("Device count: {0}", status.DeviceCount);
            Console.WriteLine("Average reading: {0}", status.AverageReading);
        }

        private static void AssertArgCount(int expectedCount, string[] actualArgs)
        {
            if (actualArgs.Length != expectedCount)
            {
                throw new ArgumentException("Incorrect number of arguments.");
            }
        }
    }
}
