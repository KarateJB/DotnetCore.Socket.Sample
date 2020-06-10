# DotNetCore.Socket.Sample

Support 2 types of data:

- Message
- File uploading


## SSL Socket Server/Client

### Create development certificate

```s
$ cd src/DotNetCore.SslSocket.Server/Certs
$ MakeCert -ss Root -sr LocalMachine -a SHA256 -n "CN=127.0.0.1,CN=localhost" -sv local.pvk local.cer -pe -e 12/31/2099 -len 2048
$ pvk2pfx.exe -pvk local.pvk -spc local.cer -pfx local.pfx
```


- `local.pfx` is for SSL Socket Server
- `local.cer`(Public key) is for SSL Socket client