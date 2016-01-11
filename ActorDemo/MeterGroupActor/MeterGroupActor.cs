using MeterGroupActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MeterGroupActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IProjName  interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by ProjName objects.
    /// </remarks>
    internal class MeterGroupActor : StatefulActor<MeterGroupActor.ActorState>, IMeterGroupActor
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
            public List<long> DeviceIds { get; set; }
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                // This is the first time this actor has ever been activated.
                // Set the actor's initial state values.
                this.State = new ActorState { DeviceIds = new List<long>() };
            }

            return Task.FromResult(true);
        }

        Task IMeterGroupActor.RegisterMeterAsync(long deviceId)
        {
            this.State.DeviceIds.Add(deviceId);

            return Task.FromResult(true);
        }

        [Readonly]
        Task<GroupStatus> IMeterGroupActor.GetStatusAsync()
        {
            var status = new GroupStatus
            {
                DeviceCount = this.State.DeviceIds.Count,
                AverageReading = -1
            };

            return Task.FromResult(status);
        }
    }
}
