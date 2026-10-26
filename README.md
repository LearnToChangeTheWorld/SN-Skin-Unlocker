# valorant港服换肤· 公益版

Windows 平台的 SN 换肤工具启动器，改编自二次套壳售卖的版本。snarl本体逆向逻辑非本项目内容。当前版本 **v1.0.0**。发行包包含已编译启动程序及配套文件，无需自行编译。

> **不允许二次套壳售卖，仅作公益使用。** 禁止通过改名、换图标、加壳、重新打包、添加卡密或收取激活费等方式收费售卖。
>
> **遇到无法正常使用等问题，请联系个人邮箱：[3069392831@qq.com](mailto:3069392831@qq.com)。**
**使用前确保开启内存完整性以及内核隔离，安全启动等启动港服必要的设置。另需要删除国服ace**

##可能遇到的问题
**驱动加载失败/下载失败**：为系统策略问题。可使用官方工具修复，具体参考snarl.today网站中的解决方式，或联系作者邮箱获得工具。
## 下载

打开 [最新发布版本](https://github.com/LearnToChangeTheWorld/SN-Skin-Unlocker/releases/latest)，在 **Assets** 中下载；也可以直接点击下方链接：

**[下载 Windows 使用包 · v1.0.0](https://github.com/LearnToChangeTheWorld/SN-Skin-Unlocker/releases/download/v1.0.0/SN-Skin-Unlocker-v1.0.0-Windows.zip)**

文件名：`SN-Skin-Unlocker-v1.0.0-Windows.zip`

这是包含程序的使用包。GitHub 自动生成的 `Source code (zip)` 和 `Source code (tar.gz)` 主要供查看源码，普通用户请优先下载上面的 Windows 使用包。

## 使用方法

1. 将 ZIP **完整解压**到一个文件夹，不要直接在压缩软件内运行程序。
2. 确认 `SNLauncher.exe`、`snarl_inner.dll`、`payload_key.txt` 和 `Launch.bat` 位于同一目录。
3. 在适用的游戏环境中，右键 `SNLauncher.exe`，选择“以管理员身份运行”；也可以使用同目录的 `Launch.bat` 启动。
4. 按程序弹窗选择模式，并在使用期间保持启动器运行。

使用包解压后的目录：

```text
SNLauncher.exe        启动程序
snarl_inner.dll       必需组件
payload_key.txt       随附数据文件
Launch.bat            便捷启动入口
使用说明.txt            使用说明与联系方式
README.md             项目说明
LICENSE               公益使用与分发条款
THIRD_PARTY_NOTICES.md 第三方组件与原始许可记录说明
ORIGINAL_LICENSE.txt  原始压缩包随附的许可文本记录
```

运行环境为 Windows 10/11，需 .NET Framework 4.x 和管理员权限。程序会加载组件并修改游戏进程；本次发布仅检查文件完整性，不能保证兼容所有游戏版本与系统环境。

## 问题反馈

遇到无法启动、报错、功能无效果或其他无法正常使用的问题，请发邮件至：

**[3069392831@qq.com](mailto:3069392831@qq.com)**

建议附上版本号、Windows 版本、游戏版本、具体操作步骤和报错截图。请勿发送密码、验证码、账号令牌或其他敏感资料。

## 公益使用与分发

- **不允许二次套壳售卖，仅作公益使用。**
- 禁止通过改名、换图标、修改界面、加壳、重新打包、添加卡密或绑定激活等方式收费售卖。
- 免费转载或分发时，请保留本说明、公益使用要求、联系方式及随附的许可和第三方声明。
- 本项目新增内容及发行整理的使用条款见 [LICENSE](LICENSE)。第三方组件的权利仍归各自权利人，详见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)。

## 源码与目录

```text
src/                      启动器源码、权限清单与编译脚本
dist/                    已编译程序及运行所需文件
docs/ANALYSIS.md          原始包随附的技术分析
docs/ORIGINAL_LICENSE.txt 原始包随附的许可文本记录
CHANGELOG.md              版本记录
README.zh-CN.md            中文说明入口
使用说明.txt                简明使用说明
```

如需自行编译，在 `src` 目录运行 `build.bat`。编译脚本使用 Windows 自带的 .NET Framework C# 编译器，普通下载使用不需要编译。

原始压缩包中已署名gkyovo。随附技术分析属于原始资料，不会在发布整理过程中自动执行。

## English

Download **`SN-Skin-Unlocker-v1.0.0-Windows.zip`** from this repository's Releases page, extract all files, and run `SNLauncher.exe` as administrator. Keep all companion files together. No compilation is required. Actual in-game compatibility has not been verified for this release.

**Free public-interest use only. Repackaging, rebranding, wrapping, or adding activation systems for resale is prohibited.** Preserve the documentation, contact information, license terms and third-party notices when redistributing free copies. Third-party components remain subject to their respective rights and licenses.

For launch failures or other issues, contact **[3069392831@qq.com](mailto:3069392831@qq.com)**.
