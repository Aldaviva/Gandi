using System.Diagnostics.CodeAnalysis;

namespace Gandi.Dns;

/// <summary>
/// <para>Types of DNS records that Gandi LiveDNS can host.</para>
/// <para>Documentation: <see href="https://docs.gandi.net/en/domain_names/faq/dns_records.html"/></para>
/// <para>Source: <see href="https://api.gandi.net/v5/livedns/dns/rrtypes"/></para>
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")] // these names are always represented in all capital letters
public enum RecordType: ushort {

    /// <summary>
    /// <para>Points to an IPv4 address.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/a_record.html"/></para>
    /// </summary>
    A = 1,

    /// <summary>
    /// Like an <see cref="A"/> record, but it points to an IPv6 address instead of IPv4.
    /// </summary>
    AAAA = 28,

    /// <summary>
    /// <para>Like a <see cref="CNAME"/>, but may only be used at the root level of a domain (when the record name is <c>@</c>).</para>
    /// <para><c>CNAME</c> records are only allowed on subdomains, not the domain itself.</para>
    /// <para>These records conflict with root-level A and AAAA records, and also break DNSSEC.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/alias_record.html"/></para>
    /// </summary>
    ALIAS = 32767, // doesn't have an official ID, this is just an unassigned ID (https://www.iana.org/assignments/dns-parameters/dns-parameters.xhtml#table-dns-parameters-4)

    /// <summary>
    /// <para>Allow only specified certificate authorities to issue certificates for your domain.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/caa_record.html"/></para>
    /// </summary>
    CAA = 257,

    /// <summary>
    /// <para>Child DNSSEC signing key</para>
    /// <para>You shouldn't create or modify these records yourself, because Gandi has to manage them for your domain.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/advanced_users/dnssec.html"/></para>
    /// </summary>
    CDS = 59,

    /// <summary>
    /// <para>Point a subdomain to another domain name or subdomain.</para>
    /// <para>The value must have a period at the end if it's a fully-qualified domain name, otherwise DNS will treat the value as a subdomain of your domain.</para>
    /// <para>Does not work at the top level of your domain, where <see cref="ALIAS"/> must be used instead.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/cname_record.html"/></para>
    /// </summary>
    CNAME = 5,

    /// <summary>
    /// <para>Like a <see cref="CNAME"/> with subdomain wildcards instead of being an exact, single-level match.</para>
    /// </summary>
    DNAME = 39,

    /// <summary>
    /// <para>DNSSEC signing key</para>
    /// <para>You shouldn't create or modify these records yourself, because Gandi has to manage them for your domain.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/advanced_users/dnssec.html"/></para>
    /// </summary>
    DS = 43,

    /// <summary>
    /// Deprecated, obsolete records historically used for DNSSEC before it switched to using <see cref="DS"/> and <c>DNSKEY</c>.
    /// </summary>
    [Obsolete($"DNSSEC uses {nameof(DS)} and DNSKEY now")]
    KEY = 25,

    /// <summary>
    /// Geographic latitude and longitude coordinates.
    /// </summary>
    LOC = 29,

    /// <summary>
    /// <para>Specify which SMTP servers receive mail to your domain.</para>
    /// <para>The value must be a domain name, not an IP address or <c>@</c>.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/mx_record.html"/></para>
    /// </summary>
    MX = 15,

    /// <summary>
    /// <para>Dial plan information for SIP clients, used, for example, for translating phone numbers into SIP URIs and therefore <see cref="SRV"/> records for the correct domain.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/naptr_record.html"/></para>
    /// </summary>
    NAPTR = 35,

    /// <summary>
    /// <para>Delegate DNS lookups for a subdomain to a different DNS server.</para>
    /// <para>The value must be a domain name (ending in a period), not an IP address.</para>
    /// <para>To use a different DNS server for your entire domain, and not just a subdomain, it is recommended to reconfigure your Gandi account, instead of creating an NS record at the root of your domain.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/ns_record.html"/></para>
    /// </summary>
    NS = 2,

    /// <summary>
    /// Exposes an OpenPGP public key for an email address, used in DANE.
    /// </summary>
    OPENPGPKEY = 61,

    /// <summary>
    /// <para>Provide an IP address for a reverse DNS lookup of a domain name. Generally only useful for web hosts and ISPs.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/ptr_record.html"/></para>
    /// </summary>
    PTR = 12,

    /// <summary>
    /// <para>Exposes the email address of the responsible party for the domain, similiar to WHOIS.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/rp_record.html"/></para>
    /// </summary>
    RP = 17,

    /// <summary>
    /// <para>Specify which SMTP servers are allowed to send mail that claims to be from this domain.</para>
    /// <para>Historically, SPF used <see cref="SPF"/> records, but today it uses <see cref="TXT"/>, so this <see cref="RecordType"/> should not be used anymore.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/spf_record.html"/></para>
    /// </summary>
    [Obsolete($"Use {nameof(TXT)} records instead, SPF records are deprecated.")]
    SPF = 99,

    /// <summary>
    /// <para>Used for discovery of various services like SIP and IMAP.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/srv_record.html"/></para>
    /// </summary>
    SRV = 33,

    /// <summary>
    /// <para>Advertises the fingerprint of an SSH server.</para>
    /// <para>Not useful in practice, almost every SSH client has <c>VerifyHostKeyDNS</c> set to <c>no</c> by default, and enabling it exposes some clients to CVE-2025-26465. Manual fingerprint verification is recommended instead: <see href="https://www.bleepingcomputer.com/news/security/new-openssh-flaws-expose-ssh-servers-to-mitm-and-dos-attacks/"/>.</para>
    /// </summary>
    SSHFP = 44,

    /// <summary>
    /// <para>Advertises a SHA-2 digest of an SMTP server's TLS certificate public key for DANE.</para>
    /// </summary>
    TLSA = 52,

    /// <summary>
    /// <para>Used to store arbitrary text, like SPF, DKIM, DMARC, MTA-STS, and verification codes that prove domain ownership.</para>
    /// <para>Values will always be returned with surrounding double quotation marks, although these are optional when setting a record's value.</para>
    /// <para>See <see href="https://docs.gandi.net/en/domain_names/faq/record_types/txt_record.html"/></para>
    /// </summary>
    TXT = 16,

    /// <summary>
    /// <para>Advertises well-known services.</para>
    /// <para>Deprecated, obsolete, and not used anymore.</para>
    /// <para>Use MX or SRV records or HTTPS <c>/.well-known/</c> instead.</para>
    /// </summary>
    [Obsolete($"An old attempt at {nameof(SRV)} records.")]
    WKS = 11

}

/// <summary>
/// Enum methods
/// </summary>
public static class RecordTypeMethods {

    /// <summary>
    /// Convert the enum value into its representation when sent to the Gandi v5 API in URIs.
    /// </summary>
    /// <returns>All capital letters record name</returns>
    public static string ToUriString(this RecordType type) => type.ToString().ToUpperInvariant();

}