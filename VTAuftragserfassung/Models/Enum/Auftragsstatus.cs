using System.Runtime.Serialization;

namespace VTAuftragserfassung.Models.Enum
{
    public enum Auftragsstatus
    {
        [EnumMember(Value = "Default")]
        Default = 0,
        [EnumMember(Value = "Erfasst")]
        Erfasst = 1,
        [EnumMember(Value = "Geprüft")]
        Geprueft = 2,
        [EnumMember(Value = "Erledigt")]
        Erledigt = 3,
        [EnumMember(Value = "Storniert")]
        Storniert = 5
    }
}
