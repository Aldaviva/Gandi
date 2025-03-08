using System.Diagnostics.CodeAnalysis;

namespace Gandi.Dns;

// https://api.gandi.net/v5/livedns/dns/rrtypes
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum RecordType {

    A,
    AAAA,
    ALIAS,
    CAA,
    CDS,
    CNAME,
    DNAME,
    DS,
    KEY,
    LOC,
    MX,
    NAPTR,
    NS,
    OPENPGPKEY,
    PTR,
    RP,
    SPF,
    SRV,
    SSHFP,
    TLSA,
    TXT,
    WKS

}

public static class RecordTypeMethods {

    public static string ToUriString(this RecordType type) => type.ToString().ToUpperInvariant();

}