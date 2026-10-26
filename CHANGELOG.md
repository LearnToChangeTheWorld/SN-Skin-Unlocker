# Changelog

All notable changes to this project are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [1.0.0] - 2026-10-25

### Added
- Standalone launcher (`SNLauncher.exe`) that drives the original licence shell's
  `Program.Main` with a pre-injected payload key, skipping the card UI and all
  network calls while preserving the complete extract → inject → patch pipeline.
- Offline payload rebuild: `KEK = SHA256(card|hwid|salt)` → AES-128-CBC unwrap →
  AES-256-CBC decrypt of `r0.enc` … `r4.enc` into
  `Snarl 2.9.2.exe`, `SnarlAuthCore.dll`, `SnarlFix.exe`, `SnarlStub.ini`,
  `SnarlStubCore.dll`.
- `Launch.bat` convenience wrapper (elevation + stale-file cleanup + start).
- Administrator manifest so the patcher can open the game process.
- Console banner with author attribution (`gkyovo`).
- Per-binary version metadata (`CompanyName` / `ProductName` / `LegalCopyright` = gkyovo).

### Changed
- Sanitised the embedded shell (`snarl_inner.dll`):
  - removed the community-group string (`交流群 <id>`) via equal-length UTF-16
    space substitution, leaving heap layout and metadata untouched.
  - rebuilt the encrypted payload resource `r2.enc` (`SnarlFix.exe`): its console
    banner `=== 交流群 <id> ===` is now `===  gkyovo  ===`. The resource was
    decrypted, edited, re-encrypted with the original IV (PKCS7, identical length)
    and written back in place — the assembly stays byte-length identical and the
    executable remains a valid PE (x64, 18 sections, 93,552 bytes).

### Notes
- `payload_key.txt` is the recovered 32-byte payload key and can be replaced to
  re-target another build without recompiling.
- Launching `SnarlFix.exe` alone produces an injection with no effect: the actual
  memory patching is performed by the shell via `Patcher.OpenProcess` /
  `Patcher.Sweep`, which this launcher reproduces.
