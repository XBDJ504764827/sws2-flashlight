# SWS2 Flashlight

按 **F** 开关的个人手电筒，光束跟随眼睛位置和视线方向，适合暗图与 KZ 路线观察。默认只向使用者传输灯光，支持颜色、亮度、范围和阴影配置。

## 与其他 SWS2 插件搭配

| 插件 | 职责 | 与本插件的关系 |
| --- | --- | --- |
| [sws2-cs2menus](https://github.com/XBDJ504764827/sws2-cs2menus) | 共享菜单、分页、导航、地图选择 HUD | 可选；不依赖菜单服务 |
| [sws2-cs2mapmanager](https://github.com/XBDJ504764827/sws2-cs2mapmanager) | 统一地图目录、评分、工坊下载和可取消换图 | 可选；不依赖地图服务 |
| [sws2-cs2admin](https://github.com/XBDJ504764827/sws2-cs2admin) | 管理员权限、玩家管理、封禁、换图控制 | 可选；当前手电筒命令无需管理员权限 |
| [sws2-cs2rockthevote](https://github.com/XBDJ504764827/sws2-cs2rockthevote) | 玩家预定地图、RTV 发起投票与选图 | 可选；预定和投票独立工作 |
| [sws2-flashlight](https://github.com/XBDJ504764827/sws2-flashlight) | 随视角移动的手电筒 | 本插件 |
| [sws2-nightvision](https://github.com/XBDJ504764827/sws2-nightvision) | 玩家独立夜视与亮度偏好 | 可选；局部照明与全局曝光可同时使用 |

常用组合：玩家换图安装 **menus + mapmanager + rockthevote**；需要管理面板时再加 **admin**；手电筒和夜视按需加入。工坊地图自动下载和 HUD 工坊资源自动挂载还需要服务器安装 **MultiAddonManager**。本套件不提供 KZ 计时、检查点或 CKZ/VNL 模式本身。

## 安装

仅依赖 [SwiftlyS2](https://github.com/swiftly-solution/swiftlys2)，不需要 menus、admin、RTV、MultiAddonManager 或客户端 HUD addon。

1. 下载本仓库发布包，完整解压 `sws2-flashlight/` 到 `csgo/addons/swiftlys2/plugins/`。
2. 重启服务器，或在服务器控制台运行 `sw plugins load sws2-flashlight`。
3. 进入游戏按 `F` 或输入 `!fl`，再按一次关闭。默认禁止观察者使用。
4. 修改首次生成的 `csgo/addons/swiftlys2/configs/plugins/sws2-flashlight/config.jsonc`；配置文件支持监听重载。

## 玩家用法与命令

聊天前缀以服务器设置为准，常见为 `!` / `/`。所有命令均为开关切换，当前没有 `fl on` / `fl off` 的显式状态子命令。

| 操作 | 作用 |
| --- | --- |
| 键盘 `F` | 切换手电筒开关 |
| `!fl` / `/fl` | 切换开关 |
| `!flashlight` / `/flashlight` | 同一命令的别名 |
| `!fl2` / `/fl2` | 同一命令的别名 |
| 玩家控制台 `fl` / `flashlight` / `fl2` | 无聊天前缀的同等操作 |

如需额外按键，在玩家控制台自行选择未占用的键绑定：

```text
bind "p" "fl"
```

当前命令没有管理员权限要求，也没有自动接入 `!admin` 或 `!yd`；这些插件各自处理自己的功能。

## 配置

```jsonc
{
  "sws2-flashlight": {
    "Debug": false,
    "Behavior": {
      "UpdateIntervalTicks": 1,
      "NotifyOnToggle": true,
      "TurnOffOnDeath": true,
      "TurnOffOnTeamSwitch": true,
      "DisableInSpectator": true
    },
    "Light": {
      "Color": "#FFFFFF",
      "ColorTemperature": 6500.0,
      "Brightness": 1.0,
      "Range": 2048.0,
      "CastShadows": true,
      "LightCookie": "materials/effects/lightcookies/flashlight.vtex",
      "SizeX": 45.0,
      "SizeY": 45.0,
      "SizeZ": 0.03,
      "SoftX": 1.0,
      "SoftY": 1.0,
      "Skirt": 0.5,
      "SkirtNear": 1.0,
      "ForwardDistance": 54.0,
      "StandEyeOffsetZ": 64.0,
      "CrouchEyeOffsetZ": 46.0,
      "OwnerOnly": true
    }
  }
}
```

### 行为

| 字段 | 默认值 | 说明 |
| --- | --- | --- |
| `Debug` | `false` | 调试日志 |
| `Behavior.UpdateIntervalTicks` | `1` | 跟随更新间隔，按 64 tick/s 换算；1 为最频繁 |
| `Behavior.NotifyOnToggle` | `true` | 按键切换时显示聊天提示 |
| `Behavior.TurnOffOnDeath` | `true` | 死亡时关闭 |
| `Behavior.TurnOffOnTeamSwitch` | `true` | 换队时关闭 |
| `Behavior.DisableInSpectator` | `true` | 禁止观察者使用 |

### 光束

| 字段 | 默认值 | 说明 |
| --- | --- | --- |
| `Light.Color` / `ColorTemperature` | `#FFFFFF` / `6500` | RGB 色值与色温 |
| `Light.Brightness` | `1.0` | 光照亮度 |
| `Light.Range` | `2048` | 引擎单位下的照射范围，实际视觉效果由地图决定 |
| `Light.CastShadows` | `true` | 阴影开关，关闭可降低灯光渲染开销 |
| `Light.LightCookie` | 模板材质路径 | 光束投影纹理；需使用可用材质 |
| `Light.SizeX` / `SizeY` / `SizeZ` | `45` / `45` / `0.03` | 光束截面与深度 |
| `Light.SoftX` / `SoftY` | `1` / `1` | 边缘柔和程度 |
| `Light.Skirt` / `SkirtNear` | `0.5` / `1` | 光束形状参数 |
| `Light.ForwardDistance` | `54` | 灯光相对眼睛前方的位置偏移 |
| `Light.StandEyeOffsetZ` / `CrouchEyeOffsetZ` | `64` / `46` | 站立/蹲伏眼高偏移 |
| `Light.OwnerOnly` | `true` | 只向拥有者传输灯光；关闭后其他玩家可见 |

保留模板中的引擎材质路径，先调整亮度、范围和阴影即可。配置重载后应在实际游戏中重新开关并验证效果；调整更新间隔后重新加载插件，确认定时器使用新间隔。

## 与夜视、换图插件一起使用

手电筒照亮视线方向，nightvision 提升个人画面的整体曝光；可同时安装，也可同时启用。在暗图可以先按 `F` 观察局部，再用 `!nv` / `!nvi 2` 调整全局可见度；过亮时降低夜视强度或关闭其中一个。

menus、admin、RTV 不提供自动手电筒控制，也不会同步灯光参数。默认死亡/换队会关闭，断线和地图卸载会清理灯光。由 admin 或 RTV 切换地图后，需要重新按 `F` 开启；本插件不保存玩家亮度或开关偏好。

## 常见问题

| 问题 | 检查 |
| --- | --- |
| 按 F 无反应 | 先在存活状态测试 `!fl`，确认插件加载成功，排除观察者限制和按键输入问题 |
| 光线偏移或打在身体上 | 在站立/蹲伏状态检查 `ForwardDistance` 和眼高偏移 |
| 其他玩家看不到灯 | 默认 `OwnerOnly: true`，这是预期行为 |
| 灯光过亮或叠加夜视泛白 | 降低 `Light.Brightness` 或夜视强度，单独测试两者 |
| 多人开灯性能下降 | 先尝试关闭阴影、降低范围，并验证增加更新间隔后的跟随表现 |

## 构建、升级与许可证

源码构建需要 .NET 10 SDK：

```sh
dotnet restore sws2-flashlight.csproj
dotnet build sws2-flashlight.csproj -c Release
dotnet publish sws2-flashlight.csproj -c Release
```

安装包：`build/sws2-flashlight.zip`。本插件无 SWS2 共享合约依赖，可独立更新；保留运行配置，替换完整包后重新加载并验证灯光。发布工作流根据版本标签计算版本，在发布构建时替换元数据中的版本占位。

[MIT License](LICENSE)。
