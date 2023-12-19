using System.Runtime.Serialization;

namespace VTAuftragserfassung.Models
{
    public enum Auftragsstatus
    {
        [EnumMember(Value = "Erfasst")]
        Erfasst = 0,
        [EnumMember(Value = "Geprüft")]
        Geprueft = 1,
        [EnumMember(Value = "Erledigt")]
        Erledigt = 2,
        [EnumMember(Value = "Storniert")]
        Storniert = 4
    }
}
