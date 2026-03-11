# Code Signing with Certum Open Source Certificate

## Step 1: Get the Certificate

1. Go to **certum.eu** and find the **Open Source Code Signing** certificate (free)
2. Create an account and apply — you'll need to provide a link to the public GitHub repo
3. Upload a government-issued ID for identity verification
4. Approval takes 1–5 business days
5. Once approved, install the certificate via their **SimplySign Desktop** app or download as `.pfx`

---

## Step 2: Export the Certificate as .pfx

Once installed, export it from Windows Certificate Manager so you can use it on other machines:

1. `Win+R` → `certmgr.msc`
2. Personal → Certificates
3. Right-click your Certum cert → All Tasks → Export
4. Select **Yes, export the private key** → `.pfx` format
5. Set a password and save the file somewhere safe (e.g. a synced/backed-up location)

---

## Step 3: Sign After Publishing

Run this after `dotnet publish`:

```bash
dotnet publish -c Release \
  /p:SigningCertPath="C:\path\to\certum.pfx" \
  /p:SigningCertPassword="yourpassword"
```

The `.csproj` already has a post-publish target that calls `signtool` automatically when `SigningCertPath` is provided.

If `signtool` is not in your PATH, the full path is usually:
```
C:\Program Files (x86)\Windows Kits\10\bin\x64\signtool.exe
```
Update the `Exec` command in `ReminderApp.csproj` if needed.

---

## Step 4: Installing on Other Machines

Copy the `.pfx` file to the other machine, then either:

**Option A — Import into cert store:**
1. `certmgr.msc` → Personal → Certificates → Import → select `.pfx`

**Option B — Just use the file directly:**
No installation needed. Run the same `dotnet publish` command with the `.pfx` path pointing to the file (USB drive, synced folder, etc.).

---

## Notes

- The **Certum timestamp server** is configured in the build step so signatures remain valid after the certificate expires
- SmartScreen reputation builds over time — you may still see a soft warning on first run for a while, but the hard "Windows protected your PC" block won't appear
- The signing step only runs on **Release** builds
