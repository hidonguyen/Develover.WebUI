using System;

namespace Develover.WebUI.Models
{
    public class UserSettingViewModel
    {
        public Guid UserId { get; set; }
        public Guid SettingId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultSelector { get; set; }
        public string Value { get; set; }
        public string Selector { get; set; }

        public UserSettingViewModel(Guid userId, Guid settingId, string name, string description, string defaultValue, string defaultSelector, string value, string selector)
        {
            UserId = userId;
            SettingId = settingId;
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            DefaultSelector = defaultSelector;
            Value = value;
            Selector = selector;
        }
    }
}
