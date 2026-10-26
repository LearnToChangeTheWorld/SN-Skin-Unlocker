# Technical analysis

Target: `SN港服换肤_授权版.exe` (12,006,400 bytes, unsigned, .NET)
Goal: make the bundled client run without the seller's card, shell or server.

---

## 1. Packaging chain

```
SN港服换肤_授权版.exe            PE32+ .NET, assembly "packed_out" v3.0.6.0
 └── resource "inner.enc"        11,998,752 bytes
      └── AES-CBC(GetKey(), IV = blob[0:16])  →  snarl_inner                11,998,720 bytes
           └── r0..r4.enc        10,877,488 / 245,792 / 93,584 / 2,992 / 81,520 bytes
                └── AES-CBC(payloadKey, IV = blob[0:16])  →  5 client files
```

### Layer 1 – the packer

`packed_out` contains a single type `.Packer`:

| Member | Behaviour |
|---|---|
| `GetKey()` | returns the 32-byte AES key `511c678f…13a8cd44` |
| `Decrypt(byte[], byte[])` | AES-128-CBC, `IV = blob[0:16]`, PKCS7 |
| `Main()` | decrypt `inner.enc`, `Assembly.Load(...)`, invoke the payload's `Main` |
| `Debugged()` | `IsDebuggerPresent` / `CheckRemoteDebuggerPresent` / `NtQueryInformationProcess` |

Extraction is therefore a two-liner (reflection against the original assembly, or any
AES implementation with the recovered key). The result is a **mixed image**: it carries
the `IMAGE_FILE_DLL` characteristic and `AddressOfEntryPoint = 0`, so it cannot be started
as an executable — it must be loaded with `Assembly.Load(byte[])` and entered through
`Program.Main`.

### Layer 2 – the seller's licence shell (`snarl_inner`)

Types of interest:

| Type | Role |
|---|---|
| `Guard` | anti-debug (`Debugged()`) |
| `Obf` | obfuscated config strings — `Host`, `Path`, `Ver`, `WrapSalt` |
| `Lic` | the whole licence subsystem |
| `Result` | `{Ok, Msg, Expire, Key, Revoked}` |
| `Pack` | payload decryption / extraction |
| `Patcher` | `OpenProcess` / `ReadProcessMemory` / `WriteProcessMemory` / `Sweep` / `IndexOf` |
| `LoginForm` / `ModeForm` | card UI and the mode picker |
| `Program` | entry point and orchestration |

Decoded configuration:

```
Host      = 38.22.95.166
Path      = /api.php
Ver       = 3.0.6
WrapSalt  = Sn4rl-2026-wr4p-s4lt
```

---

## 2. Licence protocol

### Request

`POST http://38.22.95.166/api.php`, `Content-Type: application/json`,
`User-Agent: Snarl/3.0.6`

Login (sent by `Lic.Check`):

```json
{"card":"<card>","hwid":"<32 hex>","nonce":"<32 hex>","ts":<unix>,"ver":"3.0.6"}
```

Heartbeat (sent periodically by `Program.StartHeartbeat`):

```json
{"action":"hb","card":"<card>","hwid":"<32 hex>","nonce":"<32 hex>","ts":<unix>,"ver":"3.0.6"}
```

### Response

```json
{"ok":1,"msg":"验证成功","expire":"永久","key":"<base64 wrapped key>",
 "nonce":"<echo>","ts":<echo>,"sig":"<RSA-1024 signature>"}
```

`Lic` verifies `sig` against an embedded RSA-1024 public key
(`Lic.ModulusB64` / `Lic.ExponentB64 = AQAB`), checks `nonce` equality and a timestamp
window, then unwraps `key`.

> The heartbeat reply carries `"key":""` — the payload key is only delivered by the
> **login** reply. A client that only ever sends heartbeats (because a cached offline
> token short-circuits `Lic.CheckAny`) will therefore never see it again.

### Offline tokens

`Lic.CheckAny(s)`:

```
s.Trim().StartsWith("SNL")  →  Lic.CheckOffline(s)     // "SNL2.<payload>.<sig>"
otherwise                   →  Lic.Check(s)            // online
```

Offline tokens are RSA-signed JSON, bound to `hwid`; `SNL1` is retired.

---

## 3. Crypto

```
Lic.Kek(card, hwid)      = SHA256( UTF8( card + "|" + hwid + "|" + Obf.WrapSalt ) )

Lic.UnwrapKey(b64, card, hwid):
    if b64 is empty                       -> throw "空密钥"
    raw = Base64Decode(b64)
    if raw.Length == 32                   -> return raw              // already raw
    if raw.Length <  48                   -> throw "长度不符"
    return AES-128-CBC( Kek(card,hwid), IV = raw[0:16] ).Decrypt( raw[16:] )   // 32 bytes

Pack.Decrypt(blob, key)  = AES-256-CBC( key, IV = blob[0:16] ).Decrypt( blob[16:] )
```

Everything is standard PKCS7, `Mode = CBC`, no authentication tag.

### Payload

```
Pack.Names[] = Snarl 2.9.2.exe, SnarlAuthCore.dll, SnarlFix.exe, SnarlStub.ini, SnarlStubCore.dll
Pack.Dir     = %LOCALAPPDATA%\Temp\SnarlV3
Pack.Extract(key) = for i in 0..4: Decrypt( Res("r{i}.enc"), key ) -> Dir\Names[i]
```

---

## 4. Execution flow (`Program.Main`, from IL)

```
Guard.Debugged()                          -> Fail("程序运行环境异常，已退出。")
Lic.LoadCard()                            // %LOCALAPPDATA%\SnarlV3\lic.dat (DPAPI)
  if card != "" and CheckAny(card).Ok     -> Program.Key / Program.Expire  = result
  if Program.Key == null                  -> LoginForm.Show()               // card UI
  if LoginForm.Canceled or Key == null    -> Environment.Exit(0)
Program.Log("Snarl v3.0.6 验证通过，到期时间: " + Expire)
Program.StartHeartbeat(card)              // no-op when card is empty
ModeForm.Show()                           // ① every kill / ② last kill
Program.ModeName = Mode == 1 ? "② 最后一人" : "① 每次击杀"
File.WriteAllText(Dir\特效模式.txt, "模式：" + ModeName)
if process "Snarl 2.9.2" is running       -> Fail("检测到游戏客户端正在运行…")
Pack.Extract(Program.Key)
Process.Start( Dir\SnarlFix.exe )         // injects SnarlStubCore.dll into the client
  ↓ then, still inside Main:
patched = Pack.Decrypt( Pack.Res(<per-mode resource>), Program.Key )
Patcher.OpenProcess(<game>)
Patcher.Sweep(<game>, baseline, patched, log)     // ← the actual in-game patching
```

**This is the key finding:** the client performs the injection, but the memory patching is
done by the *shell process*, using bytes decrypted with `Program.Key`. Running only
`SnarlFix.exe` therefore yields "injection succeeded, nothing happens".

---

## 5. The stub / injector

`SnarlFix.exe` (+ `SnarlStubCore.dll`, `SnarlAuthCore.dll`) is an offline authorisation
shim: it hooks `WinHttpOpenRequest` / `SendRequest` / `ReceiveResponse` / `QueryHeaders` /
`QueryDataAvailable` / `ReadData` / `CloseHandle` inside `Snarl 2.9.2.exe`, answers
`/api/auth/login` with `HTTP 401` + `Authentication response missing access token.`
(45 bytes), and loads `SnarlAuthCore.dll` afterwards to force the target's "already
logged in" branch and write a native licence blob.

`SnarlStub.ini` exposes the replay knobs (`status_*`, `body_*`, `intercept_other_api`,
`preset_cycle`, `helper*`). The launch key (`/api/auth/launchkey`) is answered with an
**empty body**; the target reports `Invalid launch key response.` and the shim suppresses
the text. This does not block injection — it only means the injected module receives no
launch payload, which is one of the reasons the tool can appear to do nothing.

---

## 6. Standalone reconstruction

```
1. capture one legitimate login reply   (plain HTTP, port 80 → trivially observable)
2. key  = UnwrapKey(reply.key, card, hwid)
3. dump = for i in 0..4: Decrypt(Res("r{i}.enc"), key) -> 5 files
4. run  = set Program.Key  = key
          set Program.Expire = "永久"
          invoke Program.Main     // LoginForm is skipped, everything else is original
```

The launcher is ~120 lines of C# with no third-party dependency; the resulting binary is
7 KB and embeds only the key. Behaviour after `ModeForm` is byte-for-byte the vendor's
own code path, including `Patcher.Sweep`.

### Redistribution sanitisation

`snarl_inner.dll` ships with its community-group label removed. The string lives in the
`#US` heap as UTF-16; it was overwritten **in place with spaces of identical length**, so
heap offsets, method bodies and metadata stay valid (file size unchanged, no rebuild).
The launcher's console banner and PE version metadata attribute the build to `gkyovo`.

---

## 7. Residual unknowns

| Item | Status |
|---|---|
| Per-mode patch resource | decrypted at runtime with `Program.Key`; byte content not analysed statically |
| `Patcher.Sweep` matching strategy | `IndexOf`-style baseline search over the target process; exact baseline not recovered |
| Game-version sensitivity | the 2.9.2 build targets a specific client revision; a mismatched revision will inject but find no pattern to patch |
| `特效模式.txt` | written by the shell as an informational artefact; no consumer found in the client, stub or shell |
