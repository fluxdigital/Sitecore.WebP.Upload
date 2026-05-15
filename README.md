# Sitecore WebP Upload

Adds WebP image support to the Sitecore 10.4 Media Library.

Since Sitecore 10.4 there is support for serving WebP images via the media handler, but there is no built-in support for uploading WebP files. This module fills that gap.

## What It Does

- Registers WebP as a media type with MIME type `image/webp`
- Provides unversioned and versioned WebP item templates (shipped as Items as Resources)
- Custom thumbnail generation and image processing via [SkiaSharp](https://github.com/mono/SkiaSharp) — no OS-level codec required

## Requirements

- Sitecore 10.4

## Installation

Install via the Sitecore Package Installer at `/sitecore/admin/StartInstallWizard.aspx` using the package from the [Releases](../../releases) page.

**Files installed:**

| File | Description |
|---|---|
| `App_Config/Include/Foundation/Sitecore.WebP.Upload/Foundation.Sitecore.WebP.Upload.config` | Media type registration config patch |
| `bin/Foundation.Sitecore.WebP.Upload.dll` | Image processing library |
| `bin/SkiaSharp.dll` | SkiaSharp managed library |
| `bin/x64/libSkiaSharp.dll` | SkiaSharp native library |
| `sitecore modules/items/master/items.master.Sitecore.WebP.Upload.dat` | WebP item templates (IAR) |

## Building from Source

```
dotnet build src/Foundation/Sitecore.WebP.Upload/Foundation.Sitecore.WebP.Upload.csproj -c Release
```

## Author

Adam Seabridge
