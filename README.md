## Winter.Monitor 📺

编写 `JSON` 进行配置的单体监控告警工具，适用于少量监控项+服务器资源少的情景。

支持钉钉机器人、邮箱等通知方式。

### 支持的监控项

- [系统](#系统监控配置)
  - 内存
  - 磁盘
  - 进程
- [数据库](#数据库监控配置)
  - MySQL
  - SQL Server
  - Redis
  - MongoDB
- 网络
  - [Ping](#Ping监控配置)
  - [TCP](#TCP监控配置)

### 支持的功能

- [x] 告警通知（如果监控项触发阈值，则发送一条告警通知。）

  ![告警通知](/assets/alarm.png)

- [x] 恢复通知（如果监控项发送过告警通知，恢复服务之后会发送一条恢复通知，**发送通知的时间不代表恢复时间**。）

  ![告警通知](/assets/alarm-recovery.png)

- [x] 告警收敛（告警收敛是指指定时间段内的监控项告警只通知一次。）

- [x] 告警静默（屏蔽指定时间段内所有或设置监控项的告警通知。）

- [x] 定时报告（定时发送健康报告通知。）

### 使用

你可以通过克隆此仓库，运行 `tools\Pulish-And-CompressToZip.ps1` 脚本进行发布，发布之后选择需要的压缩包。

也可以下载已发布的压缩包进行使用。

#### 安装为服务

程序集成了自安装服务功能，你可以通过下面的命令进行操作（**必须在管理员权限下执行**）。

如果安装出现问题，可以看 `Logs` 文件夹中的日志进行排查。

如果想要提前调试的话，可以使用 `Winter.Monitor.exe logs` 命令在控制台进行调试。

##### Windows平台

如果出现卡死、超时等情况，可以参考链接 [Windows中系统服务出现卡在 停止挂起、stopping、starting等状态的问题解决](https://blog.csdn.net/qq_34902590/article/details/82665584)。

```bash
Winter.Monitor.exe start // 安装并启动服务
```

```bash
Winter.Monitor.exe stop // 停止并删除服务
```

```bash
Winter.Monitor.exe logs // 控制台输出服务的日志
Winter.Monitor.exe logs filter="key words" // 控制台输出服务的日志
```

##### Linux平台

需要安装基础依赖 `libicu` 。

```bash
sudo ./Winter.Monitor start // 安装并启动服务
```

```bash
sudo ./Winter.Monitor stop // 停止并删除服务
```

```bash
sudo ./Winter.Monitor logs // 控制台输出服务的日志
sudo ./Winter.Monitor logs filter="key words" // 控制台输出服务的日志
```

### 配置项说明

🎉 配置项集成了 `JSON Schema` ，可以方便的对配置进行校验及提示（需要在支持的编辑器中才会生效，例如 `VS Code` 、`VS`）。

```JSON
{
    "$schema": "https://raw.githubusercontent.com/applebananamilk/winter.monitor/main/schemas/monitor-schema-v1.0.json"
}
```

#### 监控配置

```json
{
    "Monitor": {
        "ServerName": "",
    	"PollingIntervalInSeconds": 10
    }
}
```

- ServerName  : 服务器名称，如果为空则显示计算机名称。建议使用 IP+用途，例如：127.0.0.1(DB)。
- PollingIntervalInSeconds : 轮询间隔，表示收集监控数据的执行间隔（秒），默认值 10。

##### 告警配置

```JSON
{
    "Monitor": {
        "AlarmSetting": {
            "RecoveryNotification": {
                "IsEnabled": true
            },
            "Convergence": {
                "IsEnabled": true,
                "EvalInterval": 1800
            },
            "Silence": {
                "IsEnabled": false,
                "MatchAll": true,
                "HealthCheckNames": [
                    "Process@DingTalk"
                ],
                "PeriodStart": "00:00:00",
                "PeriodEnd": "23:59:59"
            }
        }
    }
}
```

- RecoveryNotification : 恢复通知设置。
  - IsEnabled : 是否启用，默认启用。
- Convergence : 告警收敛设置。
  - IsEnabled : 是否启用，默认启用。
  - EvalInterval : 重复告警收敛周期（秒），默认值 600。
- Silence : 告警静默设置。
  - IsEnabled : 是否启用，默认关闭。
  - MatchAll  : 是否对所有检查项生效，默认启用。
  - HealthCheckNames : 生效检查名字，格式为分组@检查名字，例如：OS@Disk、Process@DingTalk。
    - 分组名字：OS、Process、DB、Ping、Tcp。
  - PeriodStart : 静默时段开始，默认值0:00。
  - PeriodEnd : 静默时段结束，默认值0:00。


##### 系统监控配置

```json
{
    "Monitor": {
        "SystemSetting": {
            "ProcessSettings": [
                {
                    "ProcessName": "DingTalk"
                }
            ],
            "WarningThreshold": {
                "Disk": 90,
                "Memory": 95
            }
        }
    }
}
```

- ProcessSettings : 进程监控设置。
  - ProcessName : 进程名称，名称唯一。
- WarningThreshold : 告警阈值。
  - Disk : 磁盘（百分比），如果超过设置的百分之数值，则视为不健康。
  - Memory : 内存（百分比），如果超过设置的百分之数值，则视为不健康。

##### 数据库监控配置

```json
{
    "Monitor": {
        "DatabaseSettings": [
            {
                "Name": "Redis",
                "ConnectionString": "127.0.0.1:6309",
                "DbType": "Redis",
                "WarningThreshold": {
                    "Timeout": 5000
                }
            }
        ]
    }
}
```

- Name：名称，数据库监控配置内唯一，非空。
- ConnectionString : 连接字符串，非空。
- DbType : 数据库类型，非空。
- WarningThreshold ：告警阈值。
  - Timeout : 超时时间（毫秒）。

##### Ping监控配置

```json
{
    "Monitor": {
        "PingSettings": [
            {
                "Name": "PingName",
                "Host": "127.0.0.1",
                "WarningThreshold": {
                    "Timeout": 2000
                }
            }
        ]
    }
}
```

- Name : 名称，Ping监控配置内唯一，非空。
- Host : Ping的Host地址，非空。
- WarningThreshold ：告警阈值。
  - Timeout : 超时时间（毫秒），默认值2000。

##### TCP监控配置

```JSON
{
    "Monitor": {
        "TcpSettings": [
            {
                "Name": "Zhihu",
                "Host": "140.249.84.135",
                "Port": 443,
                "WarningThreshold": {
                    "Timeout": 5000
                }
            }
        ]
    }
}
```

- Name : 名称，TCP监控配置内唯一，非空。
- Host : Ping的Host地址，非空。
- Port : 端口，非空。
- WarningThreshold ：告警阈值。
  - Timeout : 超时时间（毫秒）。

#### 通知配置

支持多渠道接收通知，但目前只实现了钉钉机器人、邮箱的方式。

```json
{
    "Notifications": {
        "DingTalkRobot": {
            "IsEnabled": true,
            "Webhook": "",
            "AtMobiles": [],
            "IsAtAll": false
        },
        "Email": {
            "IsEnabled": false,
            "Host": "",
            "Port": 465,
            "UserName": "",
            "Password": "",
            "EnableSsl": true,
            "ReceiveEmails": ""
        }
    }
}
```

- Notifications
  - DingTalkRobot : 钉钉机器人配置。
    - IsEnabled : 是否启用。
    - Webhook : Webhook地址，为空不会发送。
    - AtMobiles : 被@的人的手机号。
    - IsAtAll : 是否@所有人。
  - Email : 邮箱配置。
    - IsEnabled : 是否启用。
    - Host : 发送邮件服务器。
    - Port : 端口号。
    - UserName : 一般是邮箱完整的地址。
    - Password : 一般是生成的**授权码**。
    - EnableSsl : 是否启用SSL。
    - ReceiveEmails : 接收邮箱地址，多个使用英文逗号分割”，“。

#### 定时报告配置

```json
{
    "PeriodicReportingWorker": {
        "IsEnabled": true,
        "StartNow": false,
        "Cron": "0 15 10 15 * ?"
    }
}
```

- IsEnabled : 是否启用。
- StartNow : 是否立即执行一次发送报告通知。
- Cron : 定时通知Cron表达式，默认每月15日上午10:15触发。
  - `0 0 15 ? * FRI` : 每个星期五的15点执行任务。
  - `0 15 10 15 * ?` : 每个月的15号上午10点15分执行任务。


##### 报告内容示例

![报告内容](/assets/report-content.jpg)

#### 配置模板

```json
{
  "$schema": "https://raw.githubusercontent.com/applebananamilk/winter.monitor/main/schemas/monitor-schema-v1.0.json",
  "Monitor": {
    "ServerName": "",
    "PollingIntervalInSeconds": 10,
    "AlarmSetting": {
      "RecoveryNotification": {
        "IsEnabled": true
      },
      "Convergence": {
        "IsEnabled": true,
        "EvalInterval": 1800
      },
      "Silence": {
        "IsEnabled": false,
        "MatchAll": true,
        "HealthCheckNames": [
          "Process@DingTalk"
        ],
        "PeriodStart": "00:00:00",
        "PeriodEnd": "23:59:59"
      }
    },
    "SystemSetting": {
      "ProcessSettings": [
        {
          "ProcessName": "DingTalk"
        }
      ],
      "WarningThreshold": {
        "Disk": 90,
        "Memory": 95
      }
    },
    "DatabaseSettings": [
      {
        "Name": "Redis",
        "ConnectionString": "127.0.0.1:6309",
        "DbType": "Redis",
        "WarningThreshold": {
          "Timeout": 5000
        }
      }
    ],
    "PingSettings": [
      {
        "Name": "PingName",
        "Host": "127.0.0.1",
        "WarningThreshold": {
          "Timeout": 5000
        }
      }
    ],
    "TcpSettings": [
      {
        "Name": "Zhihu",
        "Host": "140.249.84.135",
        "Port": 443,
        "WarningThreshold": {
          "Timeout": 5000
        }
      }
    ]
  },
  "Notifications": {
    "DingTalkRobot": {
      "IsEnabled": true,
      "Webhook": "Your webhook url.",
      "AtMobiles": [],
      "IsAtAll": false
    }
  },
  "PeriodicReportingWorker": {
    "IsEnabled": true,
    "StartNow": false,
    "Cron": "0 0 15 ? * FRI"
  }
}
```
