﻿using hass_workstation_service.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HWND = System.IntPtr;

namespace hass_workstation_service.Domain.Sensors
{
    public class IdleTimeSensor : AbstractSensor
    {
        
        public IdleTimeSensor(MqttPublisher publisher, int? updateInterval = 10, string name = "IdleTime", Guid id = default) : base(publisher, name ?? "IdleTime", updateInterval ?? 10, id)
        {
        

        }

        public override AutoDiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            return this._autoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new AutoDiscoveryConfigModel()
            {
                Name = this.Name,
                Unique_id = this.Id.ToString(),
                Device = this.Publisher.DeviceConfigModel,
                State_topic = $"homeassistant/sensor/{this.Name}/state",
                Icon = "mdi:window-maximize",
            });
        }

        public override string GetState()
        {

            return GetLastInputTime().ToString();
        }


        

        static int GetLastInputTime()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                int lastInputTick = Convert.ToInt32(lastInputInfo.dwTime);

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : idleTime);
        }



        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }
    }
}