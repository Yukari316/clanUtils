# clanUtils
###### 奇怪の晓工具 by 饼干

## 简介
其实就是一个可以解析[HoshinoBot](https://github.com/Ice-Cirno/HoshinoBot)数据库

并将总出刀表导出为Excel表格的小工具

该工具可以多次调用并且不会覆盖已经生成的表格

如果想要统计单一boss写个bat跑跑就好啦

想要统计多个boss的单独表格就。。。改指令多运行几次就好了（以后也可能写相应功能吧，咕

（以后可能会做有GUI的把,咕咕咕）


~~其实好像也没什么用~~

 ![](https://img.shields.io/github/release/CBGan/clanUtils.svg) ![](https://img.shields.io/github/license/CBGan/clanUtils.svg)

## 使用方法
#### 直接生成表格

- 此方法只能生成总表

把后缀为`.db`的数据库文件拖到`clanUtils.exe`上

按照提示输入相应的参数就可以生成表格了

#### 使用指令

在软件目录下使用 `clanUtils <数据库文件路径> <工会对应群号(可缺省或用0替代)> <所需要进行单一统计的boss编号[1/2/3/4/5]>`

## 开源协议
这个小工具使用了GPL-3.0协议

这个小工具还使用了以下开源库

[System.Data.SQLite](https://system.data.sqlite.org)

[NPOI](https://github.com/tonyqus/npoi)