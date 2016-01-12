using MeterActor.Interfaces;
using MeterGroupActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MeterActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IProjName  interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by ProjName objects.
    /// </remarks>
    internal class MeterActor : StatefulActor<MeterActor.ActorState>, IMeterActor, IRemindable
    {
        /// <summary>
        /// This class contains each actor's replicated state.
        /// Each instance of this class is serialized and replicated every time an actor's state is saved.
        /// For more information, see http://aka.ms/servicefabricactorsstateserialization
        /// </summary>
        [DataContract]
        internal sealed class ActorState
        {
            [DataMember]
            public long DeviceId { get; set; }

            [DataMember]
            public long GroupId { get; set; }

            [DataMember]
            public bool Activated { get; set; }

            [DataMember]
            public int LastReading { get; set; }
        }

        private Random _random;

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                // This is the first time this actor has ever been activated.
                // Set the actor's initial state values.
                this.State = new ActorState { LastReading = 0 };
            }

            _random = new Random();

            return Task.FromResult(true);
        }

        async Task IMeterActor.ActivateAsync(long deviceId, long groupId)
        {
            if (this.State.Activated)
            {
                throw new InvalidOperationException(
                    string.Format("Device {0} is already activated.", deviceId));
            }

            this.State.DeviceId = deviceId;
            this.State.GroupId = groupId;
            this.State.Activated = true;
            this.State.LastReading = 0;

            var groupActor = ActorProxy.Create<IMeterGroupActor>(new ActorId(groupId));
            await groupActor.RegisterMeterAsync(deviceId);

            await this.RegisterReminderAsync("ReadDevice", null, TimeSpan.Zero, TimeSpan.FromSeconds(1), 
                ActorReminderAttributes.None);
        }

        [Readonly]
        Task<int> IMeterActor.GetReading()
        {
            return Task.FromResult(this.State.LastReading);
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == "ReadDevice")
            {
                var reading = _random.Next(this.State.LastReading + 1, this.State.LastReading + 10);

                this.State.LastReading = reading;

                #region Events
                this.PublishReading(reading);
                #endregion
            }

            return Task.FromResult(true);
        }

        #region Events

        private void PublishReading(int reading)
        {
            var @event = this.GetEvent<IReadingEvents>();

            @event.ReadingAvailable(this.State.DeviceId, reading, 
                this.ActorService.ServicePartition.PartitionInfo.Id);
        }

        #endregion
    }
}
