Gandi
===

[![NuGet](https://img.shields.io/nuget/v/Gandi?logo=nuget)](https://www.nuget.org/packages/Gandi) [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/Aldaviva/Gandi/dotnetpackage.yml?branch=master&logo=github)](https://github.com/Aldaviva/Gandi/actions/workflows/dotnetpackage.yml) [![Testspace](https://img.shields.io/testspace/tests/Aldaviva/Aldaviva:Gandi/master?passed_label=passing&failed_label=failing&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA4NTkgODYxIj48cGF0aCBkPSJtNTk4IDUxMy05NCA5NCAyOCAyNyA5NC05NC0yOC0yN3pNMzA2IDIyNmwtOTQgOTQgMjggMjggOTQtOTQtMjgtMjh6bS00NiAyODctMjcgMjcgOTQgOTQgMjctMjctOTQtOTR6bTI5My0yODctMjcgMjggOTQgOTQgMjctMjgtOTQtOTR6TTQzMiA4NjFjNDEuMzMgMCA3Ni44My0xNC42NyAxMDYuNS00NFM1ODMgNzUyIDU4MyA3MTBjMC00MS4zMy0xNC44My03Ni44My00NC41LTEwNi41UzQ3My4zMyA1NTkgNDMyIDU1OWMtNDIgMC03Ny42NyAxNC44My0xMDcgNDQuNXMtNDQgNjUuMTctNDQgMTA2LjVjMCA0MiAxNC42NyA3Ny42NyA0NCAxMDdzNjUgNDQgMTA3IDQ0em0wLTU1OWM0MS4zMyAwIDc2LjgzLTE0LjgzIDEwNi41LTQ0LjVTNTgzIDE5Mi4zMyA1ODMgMTUxYzAtNDItMTQuODMtNzcuNjctNDQuNS0xMDdTNDczLjMzIDAgNDMyIDBjLTQyIDAtNzcuNjcgMTQuNjctMTA3IDQ0cy00NCA2NS00NCAxMDdjMCA0MS4zMyAxNC42NyA3Ni44MyA0NCAxMDYuNVMzOTAgMzAyIDQzMiAzMDJ6bTI3NiAyODJjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjY3IDE0LjY3LTEwNiA0NHMtNDQgNjUtNDQgMTA3YzAgNDEuMzMgMTQuNjcgNzYuODMgNDQgMTA2LjVTNjY2LjY3IDU4NCA3MDggNTg0em0tNTU3IDBjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjgzIDE0LjY3LTEwNi41IDQ0UzAgMzkxIDAgNDMzYzAgNDEuMzMgMTQuODMgNzYuODMgNDQuNSAxMDYuNVMxMDkuNjcgNTg0IDE1MSA1ODR6IiBmaWxsPSIjZmZmIi8%2BPC9zdmc%2B)](https://aldaviva.testspace.com/spaces/300345) [![Coveralls](https://img.shields.io/coveralls/github/Aldaviva/Gandi?logo=coveralls)](https://coveralls.io/github/Aldaviva/Gandi?branch=master)

*.NET REST client for the [Gandi v5 API](https://api.gandi.net/docs/)*

This library allows you to create, read, update, and delete [LiveDNS](https://www.gandi.net/en-US/domain/dns) DNS records in your [Gandi](https://www.gandi.net/) [domains](https://www.gandi.net/en-US/domain).

It's similar to [G6.GandiLiveDns](https://www.nuget.org/packages/G6.GandiLiveDns), but it's compatible with both Personal Access Token and API Key authentication, public types have interfaces so you can mock and actually test your dependent code, and it's compatible with a wider variety of runtimes.

<!-- MarkdownTOC autolink="true" bracket="round" autoanchor="false" levels="1,2,3,4" bullets="1.,-" -->

1. [Prerequisites](#prerequisites)
1. [Installation](#installation)
1. [Configuration](#configuration)
1. [Usage](#usage)
    - [LiveDNS](#livedns)
        1. [Find DNS records](#find-dns-records)
        1. [Get a DNS record](#get-a-dns-record)
        1. [Create or update a DNS record](#create-or-update-a-dns-record)
        1. [Delete a record](#delete-a-record)

<!-- /MarkdownTOC -->

## Prerequisites
- [.NET](https://dot.net) runtime compatible with [.NET Standard 2.0](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)
    - .NET 5 or later
    - .NET Core 2 or later
    - .NET Framework 4.6.2 or later
- [Gandi domain name](https://www.gandi.net/en-US/domain)
    - ✅ Domain must be using [LiveDNS](https://www.gandi.net/en-US/domain/dns), the default for new domains (`ns-*-*.gandi.net`)
    - ❌ Classic DNS (`*.dns.gandi.net`) is incompatible; you will need to [migrate to LiveDNS](https://docs.gandi.net/en/domain_names/common_operations/changing_nameservers.html#switching-to-livedns)
    - ❌ External nameservers (with glue records) are incompatible; you will need to update the record on the external nameserver instead of on Gandi's nameservers

## Installation

This library is available in the [**`Gandi`**](https://www.nuget.org/packages/Gandi) package on NuGet Gallery.

```sh
dotnet add package Gandi
```

## Configuration

1. Construct a new instance of `GandiClient`.
    ```cs
    using Gandi;

    using var gandi = new GandiClient();
    ```
1. Get a Gandi authentication token, and pass it to the `AuthToken` property of `GandiClient`. This token can be either an API Key or Personal Access Token.
    - If you already have a Gandi API Key, you can use it to authenticate this client to your Gandi account.
        - If your Gandi account doesn't already have an API Key, it's too late, you [can't make one anymore](https://api.gandi.net/docs/authentication/).
        - If you don't remember your API Key, you can regenerate it using [Account](https://account.gandi.net/) › Authentication options › Developer access › API key.
        - Pass the API Key to the `AuthToken` property of your `GandiClient` instance.
        ```cs
        gandi.AuthToken = "<your API key>";
        ```
    - You can always create a Personal Access Token.
        1. Go to either [User Settings](https://admin.gandi.net/organizations/account) or [Organizations](https://admin.gandi.net/organizations/) › ⚙ Manage.
        1. Click **Create a token**.
        1. If prompted, choose the organization which owns the domain whose DNS records you want to manage, then click Next.
        1. Choose a name and expiration period for the token.
        1. Choose whether the token will be allowed to access all domains in the organization, or just a subset of them.
        1. Allow permission to "Manage domain name technical configurations."
        1. Click **Create**.
        1. Copy the token text. Keep it somewhere safe, because Gandi won't show it to you again.
        1. Set a calendar reminder to notify you before this token expires. There is no way for Gandi to notify you or for clients to refresh or renew tokens, so you will have to repeat all of these steps when your token eventually expires.
        1. Click Done.
        1. Pass the Personal Access Token to the `AuthToken` property of your `GandiClient` instance.
        ```cs
        gandi.AuthToken = "<your personal access token>";
        ```

##### Advanced configuration
- You can access the `HttpClient` instance used by `GandiClient` if you want to configure timeouts, extra request headers, or add requests or response filters.
- If you want to provide your own `HttpClient`, you can pass it to the `GandiClient(HttpClient?)` constructor. Make sure the `HttpClient`'s `HttpMessageHandler` inherits from [`IUnfuckedHttpHandler`](https://github.com/Aldaviva/Unfucked/blob/master/HTTP/UnfuckedHttpHandler.cs) (one easy way to do this is for `HttpClient` to be an [`UnfuckedHttpClient`](https://github.com/Aldaviva/Unfucked/blob/master/HTTP/UnfuckedHttpClient.cs)) so that authentication client request filters work.

## Usage

### LiveDNS

#### Find DNS records
Return a list of all DNS records in the domain, optionally filtered by name or type (A, CNAME, etc).
```cs
var liveDns = gandi.LiveDns("mydomain.com");
IEnumerable<DnsRecord> allRecords = await liveDns.List();
IEnumerable<DnsRecord> cnames = await liveDns.List(RecordType.CNAME);
```

#### Get a DNS record
Return one DNS record in the domain with the given name and type, or `null` if it was not found.
```cs
DnsRecord? wwwRecord = await liveDns.Get(RecordType.A, "www");
```

#### Create or update a DNS record
Set a DNS record's value, automatically creating it if it didn't already exist, or modifying it if it already existed (upserting).
```cs
await liveDns.Set(new DnsRecord(RecordType.A, "www", TimeToLive.FromHours(1), "1.2.3.4"));
```

#### Delete a record
Remove a record with the given name and optionally the given type. If the type is not specified, records of all types with the given name are deleted. This method returns successfully even if the record did not exist, because either way it doesn't exist after the method completes, so it's in the desired state.
```cs
await liveDns.Delete(RecordTytpe.CNAME, "www");
```