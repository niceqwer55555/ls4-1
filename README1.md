[![Build status](https://ci.appveyor.com/api/projects/status/7olahkndcs3r295p/branch/indev?svg=true)](https://ci.appveyor.com/project/MythicManiac/gameserver/branch/indev)
[![Build Status](https://travis-ci.org/LeagueSandbox/GameServer.svg?branch=indev)](https://travis-ci.org/LeagueSandbox/GameServer)
[![codecov.io](https://codecov.io/github/LeagueSandbox/GameServer/coverage.svg?branch=indev)](https://codecov.io/github/LeagueSandbox/GameServer?branch=indev)
# League Sandbox 联盟沙盒项目游戏服务器(中国开发者)
项目官方网站及更多技术细节: https://leaguesandbox.github.io/  
项目官方Discord: https://discord.gg/Bz3znAM

# 代码贡献

参照 [此处](https://github.com/NukedBart/GameServer/blob/indev/CONTRIBUTING.md)

# 快速开始指南
* 安装 Microsoft Visual Studio 2019 或更新版本 (不限社区版/专业版)
* 安装最新版本的 .NET Framework 和 Core (VS Installer默认应该会安装, 如果没有请参照 [此处](https://dotnet.microsoft.com/download/dotnet-framework) 来安装.NET Framework, 以及 [此处](https://dotnet.microsoft.com/download/dotnet-core) 安装 .NET Core)
* Editor Guidelines 插件 (https://visualstudiogallery.msdn.microsoft.com/da227a0b-0e31-4a11-8f6b-3a149cf2e459)
	* 每行代码不应超过120字符
	
### 客户端安装程序 (Windows x64)
* 下载并运行 [League Sandbox Auto Setup](https://github.com/LeagueSandbox/LeagueSandboxAutoSetup/releases/download/v1.1/League.Sandbox.Auto.Setup.exe) 
[[Source]](https://github.com/LeagueSandbox/LeagueSandboxAutoSetup/archive/v1.1.zip)
[[Mirror]](https://github.com/LeagueSandbox/LeagueSandboxAutoSetup/archive/v1.1.tar.gz)
* 构建并运行
* 如果安装程序无法自动完成操作请参照下面的 手动安装客户端 环节
		
### 手动安装客户端 (Windows/Mac)
* 下载 4.20 版本的英雄联盟客户端:
	1. [未解包版本](https://mega.nz/#!hpkiQK5A!pFkZJtxCMQktJf4umplAdPC_Fukt0xgMfO7g3bGp1Io)
	2. [已解包，可编辑游戏文件版本](https://drive.google.com/file/d/1JVUGe75nMluczrY14xb0KDXiihFRlGnV)
* 推荐使用 [Git Bash](https://gitforwindows.org/) 运行下列 git 指令
* 使用 ```git clone https://github.com/LeagueSandbox/GameServer.git``` 拉取仓库, 然后执行以下内容下载必要的内容库:
	* ```cd GameServer```
	* ```git submodule init```
	* ```git submodule update```
* 在 VS 中打开 GameServer 解决方案, 将平台设置为 x86, 生成解决方案并运行.


### 手动安装客户端 (Linux)
* 下载 4.20 版本的英雄联盟客户端:
	1. [未解包版本](https://mega.nz/#!hpkiQK5A!pFkZJtxCMQktJf4umplAdPC_Fukt0xgMfO7g3bGp1Io)
	2. [已解包，可编辑游戏文件版本](https://drive.google.com/file/d/1JVUGe75nMluczrY14xb0KDXiihFRlGnV)
* 使用你的 Linux发行版 提供的软件包管理工具安装 git 与 dotnet 相关包 (dotnet-host, dotnet-runtime, dotnet-sdk, dotnet-targeting-pack)
* 使用 ```git clone https://github.com/LeagueSandbox/GameServer.git``` 拉取仓库, 然后执行以下内容下载必要的内容库:
	* ```cd GameServer```
	* ```git submodule init```
	* ```git submodule update```
* 使用 ```dotnet build .``` 构建服务器
* 进入构建文件夹 ```cd GameServerConsole/bin/Debug/netcoreapp3.0/```
* 打开 ```Settings/GameInfo.json``` 并修改 ```"CONTENT_PATH": "../../../../../Content"``` 为 ```"CONTENT_PATH": "../../../../Content"```
* 打开 ```Settings/GameServerSettings.json``` 并修改 ```"autoStartClient": true``` 为 ```"autoStartClient": false```
* 运行服务端 ```./GameServerConsole```

# 运行游戏客户端

#### 由 Visual Studio 或 GameServerConsole.exe 自动运行
点击 本地调试运行.
> 自动运行相关的配置文件将被生成为 `GameServer/GameServerConsole/bin/Debug/netcoreapp3.0/Settings/GameServerSettings.json`, 该文件包含了一个 英雄联盟 的 deploy 文件夹示例.

#### 使用命令行来运行
```
cd "Path/To/Your/League420/RADS/solutions/lol_game_client_sln/releases/0.0.1.68/deploy/"
start "" "League of Legends.exe" "" "" "" "127.0.0.1 5119 17BLOhi6KZsTtldTsizvHg== 1"
```

#### 使用命令行来运行 (Linux)
* 使用你的 Linux发行版 提供的软件包管理工具安装 wine.
* 执行 ```winetricks d3dx9``` - 若不执行, 你仍然能进入游戏, 但游戏会处于黑屏状态
* 在你的 League-of-Legends-4-20 中执行 ```find . -type f -iname "*.exe" -exec chmod +x {} \;``` 来讲所有的 .exe 文件设置为可执行.
* 执行 ```cd /nide/yxlm/youxi/lujing/League-of-Legends-4-20/RADS/solutions/lol_game_client_sln/releases/0.0.1.68/deploy/``` 切换至 英雄联盟 的游戏目录
* 运行游戏:

```
./League\ of\ Legends.exe "" "" "" "127.0.0.1 5119 17BLOhi6KZsTtldTsizvHg== 1"
```

# 许可协议

此仓库使用 [AGPL-3.0](LICENSE) 许可协议.
此仓库的所有代码必须被开源, 无论它在何处运行.

api服务器ip地址：127.0.0.1:8080
CDN服务器IP地址：127.0.0.1:8081/cdn
账号：555
密码：12345678