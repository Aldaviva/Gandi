using Gandi;
using Gandi.Dns;

const string patVarName = "GANDI_AUTH";

string personalAccessToken = Environment.GetEnvironmentVariable(patVarName) ??
    throw new ArgumentNullException($"Pass a Gandi personal access token or API key with permissions to manage a domain's DNS records in the {patVarName} environment variable", (Exception?) null);

using IGandiClient gandi = new GandiClient(personalAccessToken);

ILiveDns liveDns = gandi.LiveDns("bjn.mobi");

foreach (DnsRecord record in await liveDns.List()) {
    Console.WriteLine($"{record.Name}\t{record.Type}\t{record.TimeToLive?.TotalSeconds}\t{string.Join(';', record.Values)}");
}

Console.WriteLine("Upserting record...");
await liveDns.Set(new DnsRecord(RecordType.TXT, "_test", null, DateTime.Now.ToString("F")));

Console.WriteLine("Fetching record...");
Console.WriteLine($"Found: {(await liveDns.Get(RecordType.TXT, "_test"))!.Value.Values.First()}");

await liveDns.Delete(RecordType.TXT, "_test");