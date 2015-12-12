using System;
using Microsoft.SPOT;

using Verdant.Node.Common;

namespace VerdantOxygenHost.Services
{
    class SettingsEntity
    {
        public string ConfigUrl { get; set; }
        public string NodeId { get; set; }
        public string HaveConfigId { get; set; }

        public string WifiPassword { get; set; }
        public string WifiSSID { get; set; }
        public string GatewayUrl { get; set; }

        public int BootCount { get; set; }
        public int DroppedMessages { get; set; }
    }

    class SettingsProvider : ISettingsProvider
    {
        private ExtendedWeakReference _ewrSettings;
        private SettingsEntity _currentSettings;

        public void Initialize(Type appType)
        {
            _ewrSettings = ExtendedWeakReference.RecoverOrCreate(appType, 0, ExtendedWeakReference.c_SurvivePowerdown);
            _ewrSettings.Priority = (Int32)ExtendedWeakReference.PriorityLevel.Important;
            if (_ewrSettings.Target==null)
            {
                _currentSettings = new SettingsEntity()
                {
                    ConfigUrl = "https://vauthscus.blob.core.windows.net/deviceconfig/",
                    NodeId = "85331753-a1ef-4aaa-bfa1-4fa00343bf49",
                    HaveConfigId = "",
                    
                    WifiSSID = "cloudgate", WifiPassword = "Escal8shun", // because we don't have onboarding figured out yet
                    GatewayUrl = "",

                    BootCount = 0, DroppedMessages = 0
                };
                CommitChanges();
            }
            else
            {
                _currentSettings = (SettingsEntity)_ewrSettings.Target;
            }
        }

        public void CommitChanges()
        {
            _ewrSettings.Target = _currentSettings;
        }

        public int BootCount
        {
            get { return _currentSettings.BootCount; }
            set { _currentSettings.BootCount = value; }
        }

        public int DroppedMessages
        {
            get { return _currentSettings.DroppedMessages; }
            set { _currentSettings.DroppedMessages = value; }
        }

        public string GatewayUrl
        {
            get { return _currentSettings.GatewayUrl; }
            set { _currentSettings.GatewayUrl = value; }
        }

        public string HaveConfigId
        {
            get { return _currentSettings.HaveConfigId; }
            set { _currentSettings.HaveConfigId = value; }
        }

        public string NodeId
        {
            get { return _currentSettings.NodeId; }
            set { _currentSettings.NodeId = value; }
        }

        public string WifiPassword
        {
            get { return _currentSettings.WifiPassword; }
            set { _currentSettings.WifiPassword = value; }
        }

        public string WifiSSID
        {
            get { return _currentSettings.WifiSSID; }
            set { _currentSettings.WifiSSID = value; }
        }

        public string ConfigUrl
        {
            get { return _currentSettings.ConfigUrl; }
            set { _currentSettings.ConfigUrl = value; }
        }
    }
}
